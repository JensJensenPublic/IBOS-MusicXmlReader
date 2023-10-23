using System;
using System.Collections.Generic;
using System.Xml;
using BrailleMusicDecoder.MusicXmlElements;
using MusicXmlReaderModel; // Namespace, not reference

namespace BrailleMusicDecoder
{
    class MusicXmlBuilderStateHarmonyPart : MusicXmlBuilderStateMusic
    { 
        private MusicXmlHarmonyElement currentHarmony = null;
        // The folllowing 3 members are used for handling timing by inserting ForwardElements:
        private XmlNode currentForwardElement = null; // The latest ForwardElement within the current measure
        private List<XmlNode> currentForwardElements = null; // The ForwardElements in the current measure, that need a fixup.
        private const int undefinedChordTimingDuration = -1;

        public override MusicXmlBuilderStateEnum GetState()
        {
            return MusicXmlBuilderStateEnum.Harmonies;
        }
        
        //private string ToString(InputSubCategoryEnum inputSubCategory)
        //{
        //    switch (inputSubCategory)
        //    {
        //        // By tradition and MusicXml rules root values are described by UPPERCASE letters
        //        case InputSubCategoryEnum.FullStepA: return "A";
        //        case InputSubCategoryEnum.FullStepB: return "B";
        //        case InputSubCategoryEnum.FullStepC: return "C";
        //        case InputSubCategoryEnum.FullStepD: return "D";
        //        case InputSubCategoryEnum.FullStepE: return "E";
        //        case InputSubCategoryEnum.FullStepF: return "F";
        //        case InputSubCategoryEnum.FullStepG: return "G";
        //        default: return null;
        //    }
        //}


        /// <summary>
        /// ***NOTE*** This method is a first attempt and will probably have to be enhanced or redesigned!  ***NOTE***
        /// Fix the duration all ForwardElements with undefined duration
        /// Finally add a single DurationElement covering the whole measure if no other ´ForwardElements are found.
        /// </summary>
        private void FillinForwardElements()
        {
            int nForwardElements = currentForwardElements.Count;  
            if (0 == currentBeatType)
            {
                LogCF(string.Format(": CurrentBeattype = 0 ********************************"));
            }

            int totalMeasureDuration = 0; 
            int totalForwardDuration = 0;
            if (nForwardElements != 0)
            {
                // Calculate the total duration of this measure in units of divisionsPerQuarterNote
                // Delay division by currentBeatType until it is needed to protect against crash.
                totalMeasureDuration =  currentBeats* DivisionsPerQuarterNote *4 / currentBeatType;
                foreach (XmlNode forwardElement in currentForwardElements)
                {
                    // At this point we know the structure of the measure and can compute a default duration.
                    // Share the remainng time between all forward elements thet have not been changed by a ChordStemSign
                    int individualDuration = totalMeasureDuration / nForwardElements;
                    string existingValue = musicXmlElementFactory.GetChildValue(forwardElement, "duration");
                    int existingDuration = int.Parse(existingValue);
                    if (0 == existingDuration)
                    {
                        if ((totalForwardDuration + individualDuration) > totalMeasureDuration)
                        {
                            // Check if we are about to forward across the nuration of the measure. This could be caused by a missing "nodeHals" at the latest chord in the measure
                            Logger.LogCF(string.Format(": WARNING: (totalForwardDuration={0} + individualDuration={1}) > totalMeasureDuration={2}", totalForwardDuration, individualDuration, totalMeasureDuration));
                            individualDuration = totalMeasureDuration - totalForwardDuration;
                            Logger.LogCF(String.Format(": Reducing individualDuration to {0}", individualDuration));
                        }

                        //LogCF(string.Format(": {0} : Changing ForwardElement.Duration from 0 to {1}", LoggerLocationInfo, individualDuration));
                        musicXmlElementFactory.SetChildValue(forwardElement, "duration", individualDuration.ToString());
                        totalForwardDuration += individualDuration;
                    }
                    else
                    {
                        totalForwardDuration += existingDuration;
                    }
                
                }
            }
            int extraDuration = totalMeasureDuration - totalForwardDuration;
            if (0 != extraDuration)
            {
                // Insert a single ForwardElement at the end of the measure to fill up the measure
                if (null != CurrentMeasure)
                {
                    LogCF(string.Format(": {0} : Adding ForwardElement({1})", LoggerLocationInfo, extraDuration));
                    XmlNode lastChild = CurrentMeasure.LastChild;
                    if ((null != lastChild)
                        && (lastChild.Name == "barline")
                        && (lastChild.Attributes.Count > 0)
                        && (lastChild.Attributes[0].Name == "location")
                        && (lastChild.Attributes[0].Value == "right"))
                    {
                        // If the measure contains a terminating right bar insert the new ForwardElement IN FRONT OF the terminating right bar !
                        CurrentMeasure.RemoveChild(lastChild);
                        CurrentMeasure.AppendChild(musicXmlElementFactory.ForwardElement(extraDuration));
                        CurrentMeasure.AppendChild(lastChild);
                    }
                    else
                    {
                        CurrentMeasure.AppendChild(musicXmlElementFactory.ForwardElement(extraDuration));
                    }
                }
            }
        }

        private void OnNewMeasure(MusicXmlElementFactory.BarStyleEnum barStyle)
        {
            bool addNewMeasure = measureHandler.OnSpace();
            if (addNewMeasure)
            {
                // Fix the forwardElements in the latest measure if needed                    
                FillinForwardElements(); // First fill in any undefined ForwardElements
                                         //VerifyCurrentMeasure(); // Finally verify the current measure, possibly adding a forward-element at the beginning if the current measure is an anacrusis (Danish: "Optakt")
                                         // Prepare for fixing the forwardElements in the new measure
                currentForwardElement = null;
                currentForwardElements = new List<XmlNode>();
                // Add the new measure
                AddNewMeasure(barStyle);
            }
        }

        /// <summary>
        /// Append currentHarmony to CurrentMeasure
        /// </summary>
        private void AppendCurrentHarmony()
        {
            CurrentMeasure.AppendChild(currentHarmony);
            // Insert a ForwardElement immediately after the HarmonyElement in order to specify a (currently unknown) duration
            currentForwardElement = musicXmlElementFactory.ForwardElement("0");
            CurrentMeasure.AppendChild(currentForwardElement); // Insert as a place holder. Will later be modified by a ChordStemSign or a default value.
            currentForwardElements.Add(currentForwardElement); // List of elements to modify 
        }


        /// <summary>
        /// Only purpose is to issue the warning, no real action!
        /// </summary>
        /// <param name="subCategory"></param>
        private void OnSectionalDoubleBar(InputSubCategoryEnum subCategory)
        {
            if (InputSubCategoryEnum.SectionalDoubleBarFollowedByBarline != subCategory) return;
            base.AddSelectedEvent("Normal barline immediately following a SectionalBarline is explicitly ignored.");
        }


        public override MusicXmlBuilderState ApplyNextInput(InputInterpretation input)
        {
            MusicXmlBuilderState result = this;
            bool verbose = false;
            bool flushText = false;
            //XmlNode currentMeasure = musicXmlBuilder.CurrentMeasure;
            //XmlNode currentAttributesElement = musicXmlBuilder.CurrentAttributesElement;
            switch (input.Category)
            {
                case InputCategoryEnum.Beat: OnBeat(input); break;
                    //GetBeatParameters(input, out currentBeats, out currentBeatType);
                    //currentAttributesElement.ReplaceChild(musicXmlElementFactory.TimeElement(currentBeats, currentBeatType), currentAttributesElement.SelectSingleNode("time"));
                    //// Append a TimeElement, but it must be packed within an AttributesElement.
                    //XmlNode emptyAttributesElement = musicXmlElementFactory.EmptyAttributesElement();
                    //CurrentMeasure.AppendChild(emptyAttributesElement);
                    //emptyAttributesElement.AppendChild(musicXmlElementFactory.TimeElement(currentBeats, currentBeatType));
                    //break;


                    //GetBeatParameters(input, out currentBeats, out currentBeatType);
                    //if (null == currentAttributesElement)
                    //{
                    //    currentAttributesElement = base.musicXmlElementFactory.AttributesElement(DivisionsPerQuarterNote,currentBeats,currentBeatType);
                    //}
                    //currentAttributesElement.ReplaceChild(musicXmlElementFactory.TimeElement(currentBeats,currentBeatType), currentAttributesElement.SelectSingleNode("time"));
                    //LogCF(string.Format(": CurrentBeats={0} CurrentBeatType={1}", currentBeats, currentBeatType));
                    //break;

                case InputCategoryEnum.FinalDoubleBar: // Same housekeeping as in case NewMeasure, but without preparing for and adding a new measure 
                    FillinForwardElements(); // First fill in any undefined ForwardElements 
                    base.OnFinalDoubleBar();  // The end of the part !
                    break;

                case InputCategoryEnum.SectionalDoubleBar: OnNewMeasure(MusicXmlElementFactory.BarStyleEnum.lightLight); OnSectionalDoubleBar(input.SubCategory);  break; // A Halfend is nothing but a double measurebar !
                case InputCategoryEnum.NewMeasure:  OnNewMeasure(MusicXmlElementFactory.BarStyleEnum.normal); break; // Almost the same as  in state part !! 

                case InputCategoryEnum.TextVersal:
                    LogCF(input.ToDebugString("Explicitly ignored TextVersat")); // Probably just redundant information !
                    break;

                case InputCategoryEnum.EmbeddedTextRepresentation: base.OnEmbeddedTextRepresentation(input, new List<InputSubCategoryEnum>()); break; // Second param is dummy

                case InputCategoryEnum.ChordSymbolRoot:
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures
                    //LogCF(string.Format(": Category={0} Value={1} subCategory={2}", input.Category, input.FriendlyValue, input.SubCategory));
                    if (verbose) LogCF(input.ToDebugString());
                    if (input.SubCategory == InputSubCategoryEnum.ChordSymbolRootNoRoot) 
                    {
                        // Special case, triggered by the dot36 symbol meaning "No root"
                        Logger.LogCF(string.Format(": SubCategory={0}", input.SubCategory));
                        if (null == currentHarmony) break; // Avoid crashing, hope this makes sense

                        //XmlNode clone = currentHarmony.CloneNode(true); // Deep clone
                        // Idea stolen from MusicXmlBuilderElement.ChordNoteElement ;
                        MusicXmlHarmonyElement clone = musicXmlElementFactory.HarmonyNoteElement(currentHarmony); // Create a (deep) clone
                        XmlNode basNode = clone.SelectSingleNode("bass");
                        if (null != basNode)
                        {
                            clone.RemoveChild(basNode);
                        }
                        currentHarmony = clone;
                        AppendCurrentHarmony();
                    }
                    else
                    {
                        // THe normal casse where the harmony root is explicitly specified:
                        string root = input.ToFullStepString();
                        if (null != root)
                        {
                            // Add the Harmony element (only containing information for "root" to the current Measure.
                            // Alter and kind will be filled in later !
                            //currentHarmony = elements.HarmonyElement(root, input.SubCategoryValue);
                            currentHarmony = musicXmlElementFactory.HarmonyElement(root, input.SubCategoryValue);
                            AppendCurrentHarmony();
                        }
                    }
                    break;

                case InputCategoryEnum.ChordSymbolBass:
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures
                    //LogCF(string.Format(": Category={0} Value={1} subCategory={2}", input.Category, input.FriendlyValue, input.SubCategory));
                    if (verbose) LogCF(input.ToDebugString());
                    string bass = input.ToFullStepString();
                    if (null != bass)
                    {
                        // Add the Harmony element to the current harmony. It goes last, anyway
                        currentHarmony.AppendChild(musicXmlElementFactory.BassElement(bass,input.SubCategoryValue));
                    }
                    break;

                case InputCategoryEnum.ChordSymbol: // Compare to similat code in InputInterpretation.cs
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures
                    //LogCF(string.Format(": Category={0} Value={1} subCategory={2}", input.Category, input.FriendlyValue, input.SubCategory));
                    if (verbose) LogCF(input.ToDebugString());
                    string kind = null;
                    switch (input.SubCategory)
                    {
                        // More cases to come here ! Find  the exact chord kinds in MidiCorrd.cs
                        case InputSubCategoryEnum.ChordSymbolSus2: kind = "suspended-second"; break; // MusicXml does not accept "sus" How is this handled ??
                        case InputSubCategoryEnum.ChordSymbolSus4: kind = "suspended-fourth"; break; // MusicXml does not accept "sus" How is this handled ??
                        case InputSubCategoryEnum.ChordSymbolMinor: kind = "minor"; break; // 
                        case InputSubCategoryEnum.ChordSymbolDim: kind = "diminished"; break; // A circle without a crossing line
                        case InputSubCategoryEnum.ChordSymbolMaj: kind = "major-seventh"; break;
                        case InputSubCategoryEnum.ChordSymbolAug: kind = "augmented-seventh"; break;
                        case InputSubCategoryEnum.ChordSymbolHalfDim: kind = "half-diminished"; break; // A circle with a crossing line
                        case InputSubCategoryEnum.None: break;
                        default:
                            LogCF(string.Format(": Unexpectedted InputCategoryEnum.ChordSymbol={0}",input.SubCategory));
                            break;
                    }
                    if (null != kind)
                    {
                        musicXmlElementFactory.SetChildValue(currentHarmony, "kind", kind); // Such as "m" "sus2" "sus4" "dim" "aug"
                    }
                    break;

                case InputCategoryEnum.ChordStemSign:
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures
                    // The ChordStemSign explicitly indicates the duration of the HarmonyElement latest added.
                    // In the current implementation the duration is specified by a ForwardElement which was added immediately after the HarmonyElement and thus already exists (but with the value 0).
                    // Replace the value of 0 by the value specified in the ChordStepSign.
                    // (If no ChordStepSign i specified for the chord, a default value will be inserted later when the next measure is started).
                    int denominator = int.Parse(input.SubCategoryValue);
                    int chordStemSignDuration = 4 * DivisionsPerQuarterNote / denominator;
                    switch (input.SubCategory)
                    {
                        case InputSubCategoryEnum.ChordStemSign: break;
                        case InputSubCategoryEnum.ChordStemSignWithPunctuation: chordStemSignDuration = chordStemSignDuration * 3 / 2; break;
                        case InputSubCategoryEnum.ChordStemSignWithDoublePunctuation: chordStemSignDuration = chordStemSignDuration * 7 / 4; break;

                        default:
                            //LogCF(string.Format(": Unexpected value: InputSubCategory= {0}", input.SubCategory.ToString()));
                            LogCF(input.ToDebugString("Unexpected value:"));
                            break;
                    }
                    musicXmlElementFactory.SetChildValue(currentForwardElement, "duration", chordStemSignDuration.ToString());
                    if (verbose) LogCF(string.Format(": Category={0} Value={1} subCategory={2} ForwrdElement.Duration set to {3}", input.Category, input.FriendlyValue, input.SubCategory, chordStemSignDuration.ToString()));
                    break;

                case InputCategoryEnum.Rest:
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures
                    // This is not a recommended transscription technique. Warn:
                    string stateName = this.GetState().ToString();
                    string partString = (string.IsNullOrEmpty(this.FriendlyName) ? stateName : this.FriendlyName);
                    base.AddSelectedEvent(string.Format("Part='{0}' contains a 'Rest' symbol, which is not expected in this part.",partString));
                    // Interpret it anyway..
                    int restDuration = base.GetDivisions(input.GetNoteType());
                    AddForwardElement(restDuration);
                    break;
                    
                case InputCategoryEnum.ChordTiming:
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures
                    if (verbose) LogCF(string.Format(": {0} ", LoggerLocationInfo));
                    const int fullNoteDuration = DivisionsPerQuarterNote * 4;
                    int chordTimingDuration = undefinedChordTimingDuration;
                    switch (input.SubSubCategory)
                    {
#warning TODO: Use functions from base class, extending that function with the 4 "dotted" values below.
                        case InputSubSubCategoryEnum.NoteTypeEighthOr128th:  chordTimingDuration = fullNoteDuration / 8; break;
                        case InputSubSubCategoryEnum.NoteTypeQuarterOr64th: chordTimingDuration = fullNoteDuration / 4; break;
                        case InputSubSubCategoryEnum.NoteTypeHalfOr32nd: chordTimingDuration = fullNoteDuration / 2; break;
                        case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th: chordTimingDuration = base.GetWholeMeasureOrFullNoteDuration(); break; // Assuming this is a FullMeasure or a FullNote:
                        // And the same but followed by a Dot3 "Dotted"
                        case InputSubSubCategoryEnum.NoteTypeEighthOr128thDotted: chordTimingDuration = fullNoteDuration * 3 / 16; break; // As above *3/2
                        case InputSubSubCategoryEnum.NoteTypeQuarterOr64thDotted: chordTimingDuration = fullNoteDuration * 3 / 8; break;  // As above *3/2
                        case InputSubSubCategoryEnum.NoteTypeHalfOr32ndDotted: chordTimingDuration = fullNoteDuration *3 / 4; break;  // As above *3/2
                        case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16thDotted: chordTimingDuration = base.GetWholeMeasureOrFullNoteDuration() * 3 / 2; break;  // As above but * 3/2 // Assuming this is a FullMeasure or a FullNote:

                        default: break;                   
                    }
                    string message;
                    if (chordTimingDuration == undefinedChordTimingDuration)
                        message = string.Format("Unexpected value of input.SusSubCategoty:{0}",input.SubSubCategory);
                    else
                        message = string.Format("ChordTimingDuration={0}", chordTimingDuration); 
                    LogCF(string.Format(": {0} {1}", LoggerLocationInfo, message));
                    AddForwardElement(chordTimingDuration);              
                    break;
                    
                case InputCategoryEnum.ChordCharacter:
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures
                    LogNotYetImplemented(input);
                    break;

                case InputCategoryEnum.ChordNumericExtension:
                    measureHandler.OnAnyHarmonyInput(); // Re-enable generation of new measures                   
                    //MusicXmlHarmonyElement harmony = MusicXmlHarmonyElement.Create(currentHarmony,elements);
                    currentHarmony.AddChordNumericExtension(input.FriendlyValue);
                    break;

                case InputCategoryEnum.NonBrailleCharacter:
                    LogCF(input.ToDebugString());
                    break;

                case InputCategoryEnum.ControlCharCRLF: // Implicitly ignore
                    break;
                case InputCategoryEnum.ControlCharCRLFNumber: // Implicitly ignore
                    break;


                case InputCategoryEnum.Hand: // Allows returning to a partstate from HarmonyPart. Needed in Section by Section for instance NOTA "Nr. 18 Morning has broken" 
                    // If the Hand symbol  represents a new part we first create a new MusicXmlBuilderState object to represent it
                    // otherwise we just reuse the existing object.
                    result = MusicXmlBuilderState.Create(MusicXmlBuilderStateEnum.Part, musicXmlBuilder, input.FriendlyValue); // ******************** New code !! *******************
                    break;

                case InputCategoryEnum.SectionHeader:
                    LogCF(string.Format(": SectionHeader {0}", input.FriendlyValue));
                    musicXmlBuilder.SectionHeaderHandler.OnSectionHeader(input.FriendlyValue);                
                    break;

                case InputCategoryEnum.PrintPagination:
                    LogCF(string.Format(": Printpagination {0}", input.FriendlyValue));
                    break;

                case InputCategoryEnum.DaCapoAndDalSegno:
                    LogCF(string.Format(": DaCapoAndDalSegno {0}", input.FriendlyValue));
                    base.OnDaCapoAndDalSegno(input); // Implement in base clas !
                    break;

                case InputCategoryEnum.Character: flushText = base.OnCharacter(input); break;

                case InputCategoryEnum.PartMeasureRepeat:
                    {
                        switch (input.SubCategory)
                        {
                            case InputSubCategoryEnum.PartMeasureRepeatOnce: break;
                            case InputSubCategoryEnum.PartMeasureRepeatTwice: break;
                            case InputSubCategoryEnum.PartMeasureRepeatThreeTimes: break;
                            default: break;
                        }
                    } break;

                case InputCategoryEnum.OtherValues:
                    switch (input.SubCategory)
                    {

                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeat:
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatNoInitialBlank:
//                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatOnce:
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatTwice:
                        case InputSubCategoryEnum.OthervaluesLongAppoggiatura:
                        case InputSubCategoryEnum.OtherValuesOrnament:
                        //case InputSubCategoryEnum.OtherValuesPartMeasureRepeat:
                        //case InputSubCategoryEnum.OtherValuesPartMeasureRepeatTwice:
                        case InputSubCategoryEnum.OthervaluesShortAppoggiatura:
                        case InputSubCategoryEnum.OthervaluesTremoloAlternatingNotes:
                        case InputSubCategoryEnum.OthervaluesTremoloRepeatedNote:
                        case InputSubCategoryEnum.OtherValuesTrill: break;

                        //Print-repeats: Graphic information, also interpreted by the sighted user. Handled by base class
                        case InputSubCategoryEnum.OthervaluesDoubleBarFollowedByDots:
                        case InputSubCategoryEnum.OthervaluesDoubleBarPrecededByDots:
                        case InputSubCategoryEnum.OthervaluesVolta1FirstEnding:
                        case InputSubCategoryEnum.OthervaluesVolta2SecondEnding:
                        case InputSubCategoryEnum.OthervaluesVoltaIntervalEnding:
                        case InputSubCategoryEnum.OthervaluesVoltaNumericEnding:  base.OnPrintRepeatsAndEndings(input.SubCategory, input.Values); break;                  

                        default: break;
                    }
                    break;

                default:
                    // This is probably an error! We do not intend to handle this inputcategory.
                    base.OnUnsupportedInput(string.Format("Part='{0}'", this.friendlyName), input);
                    LogCF(input.ToDebugString(": Unexpected input:")); 
                    break;
            }

            base.OnEpilog(flushText);

            return result;
        }

        private void AddForwardElement(int duration)
        {
            if (duration != undefinedChordTimingDuration)
            {
                XmlNode forwardElement = musicXmlElementFactory.ForwardElement(duration);
                CurrentMeasure.AppendChild(forwardElement);
                currentForwardElements.Add(forwardElement);
            }
            else
            {
                Logger.LogCF(string.Format(": Parameter duration={0} is not expected",duration));
            }
        }



        // We intend to handle this input category later !
        //private void LogNotYetImplemented(InputCategoryEnum inputCategory, string inputValue, InputSubCategoryEnum inputSubCategory)
        private void LogNotYetImplemented(InputInterpretation input)
        {
            //LogCF(string.Format(":Not yet implemented: Category={0} Value={1} subCategory={2}", inputCategory, inputValue, inputSubCategory));
            LogCF(string.Format(": {0}",input.ToDebugString()));
        }

        public static MusicXmlBuilderStateHarmonyPart Create(MusicXmlBuilder musicXmlBuilder)
        {
            if (null == musicXmlBuilder.TheHarmonyPart)
            {
                // Only the first time we need a new HarmonyPart !
               musicXmlBuilder.TheHarmonyPart = new MusicXmlBuilderStateHarmonyPart(musicXmlBuilder);
            }
            return musicXmlBuilder.TheHarmonyPart;
        }

        private MusicXmlBuilderStateHarmonyPart(MusicXmlBuilder musicXmlBuilder) : base(musicXmlBuilder)
        {
            currentForwardElements = new List<XmlNode>();
            measureHandler = MusicXmlMeasureHandler.Create();
        }
    }
}
