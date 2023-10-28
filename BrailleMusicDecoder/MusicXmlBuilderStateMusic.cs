using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using BrailleMusicDecoder.MusicXmlHandlers;
using BrailleMusicDecoder.MusicXmlElements;
using MusicXmlReaderModel; // The NAmeSpace, not the dll!

namespace BrailleMusicDecoder
{
    abstract class MusicXmlBuilderStateMusic : MusicXmlBuilderState
    {
        //int defaultBeats; //  Initialized by constructor !
        //int defaultBeatType; // Initialized by constructor !
        private XmlNode partList = null;
        private XmlNode scorePartwiseNode = null;
        private XmlNode currentPart = null; // The part currently under construction
        public XmlNode CurrentPart { get { return currentPart; } }
        protected MusicXmlMeasureHandler measureHandler;
        protected string CurrentPartName { get { return (CurrentPart == null) ? "null" : currentPart.Attributes.GetNamedItem("id").Value.ToString(); } }

        public const int FirstMeasureNumber = 1;
        private int currentMeasureNumber = FirstMeasureNumber;
        protected int CurrentMeasureNumber { get { return currentMeasureNumber; } set { currentMeasureNumber = value; } }
        public int GetCurrentMeasureNumber() { return currentMeasureNumber; }

        protected XmlNode currentAttributesElement = null;
        public XmlNode CurrentAttributesElement { get { return currentAttributesElement; } }
        //        public const int DivisionsPerQuarterNote = 24; //  Number of divisions of a quarternode. IBOS  MusicXmlReader uses this as default. MuseScore seems to use this as default
        public const int DivisionsPerQuarterNote = 96; //  Number of divisions of a quarternode. Allows for 1/128th which is shortest duration which can be represented in Misic Braille without punctuations.
        private MusicXmlMeasureElement currentMeasure = null;
        public MusicXmlMeasureElement CurrentMeasure
        {
            get
            {
                if (null == currentMeasure)
                {
                    LogCF(": currentMeasure is null. Creating a measure! *****************************************************************************");
                    this.AddNewMeasure();
                    currentPositionWithinMeasure = 0;
                }
                return currentMeasure;
            }
        }

        //protected void SetCurrentMeasure(XmlNode measure)
        //{
        //    currentMeasure = measure;
        //}
        

        protected int currentPositionWithinMeasure; // In Divisions per quarter note
        public int CurrentPositionWithinMeasure { get { return currentPositionWithinMeasure; } } // In Divisions per quarter note
        private int currentVoice = 1; // Voices are numbered from 1 and up
        public int CurrentVoice { get { return currentVoice; } set { currentVoice = value; } }
        protected int currentBeats;
        protected int currentBeatType;
        public int CurrentFullMeasureDivisions { get { return DivisionsPerQuarterNote * 4 * currentBeats / currentBeatType; } }

        protected GraphicRepeatHandler graphicRepeatHandler;

        protected StringBuilder sbText = new StringBuilder(); // During debug only 
        protected bool nextCharIsVersal = false;


        /// <summary>
        /// Convenience function for reporting location for instance while logging.
        /// </summary>
        protected string LoggerLocationInfo
        {
            get
            {
                return string.Format("Part={0} Voice={1} MeasureNumber={2}", CurrentPartName, currentVoice, CurrentMeasureNumber);
            }
        }

        protected bool OnCharacter(InputInterpretation input)
        {          
#warning TODO: If input.Friendlyvalue consists of more than one char only the first should be versalized!
            string nextText = nextCharIsVersal ? input.FriendlyValue.ToUpper() : input.FriendlyValue;
            sbText.Append(nextText); // During debug  
            nextCharIsVersal = false;
            return false;
        }

        protected void OnEpilog(bool flushText)
        {
            // During debug we collect and log texts
            if (flushText & sbText.Length > 0)
            {
                string s = sbText.ToString();
                Logger.LogCF(string.Format(": Text='{0}'", s));
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (!OnSpecialTextDirection(s))
                    {
                        XmlNode direction = musicXmlElementFactory.DirectionElement(s);
                        CurrentMeasure.AppendChild(direction);
                    }
                   
                }
                sbText.Clear();
            }
        }


        /// <summary>
        /// Some transscribers use informal texts instead for the more formal Music Braille definitions
        /// In that case we attempt to handle the most typical ones:
        /// (Only act once, on the first match!)
        /// The formal (and more correct) symbols are handled in "OnDaCapoAndDalSegno()" below
        /// </summary>
        /// <param name="direction"></param>
        private bool OnSpecialTextDirection(string direction)
        {
            string lowDirection = direction.ToLower();

            if (IsInformalDirection(lowDirection,"d. s. al fi")) // Because the terminating "ne" is missing in "Tears from heaven" !!
            {
                XmlNode alSegnoAlFineDirectionElement = musicXmlElementFactory.DalSegnoAlFineDirectionElement();
                CurrentMeasure.AppendChild(alSegnoAlFineDirectionElement);
                return true;
            }

            if (IsInformalDirection(lowDirection, "fine"))
            {
                XmlNode fineDirectionElement = musicXmlElementFactory.FineDirectionElement();
                CurrentMeasure.AppendChild(fineDirectionElement);
                return true;
            }

            // More to come...
            return false;
        }


        /// <summary>
        /// Checks the inputstring against a fixed pattern and returns true if the pattern is found
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private bool IsInformalDirection(string direction, string pattern)
        {
            if (!(direction.Contains(pattern))) return false;
            Logger.LogCF(string.Format(": '{0}' contains '{1}'", direction, pattern));
            return true;
        }

        protected void OnBeat(InputInterpretation input)
        {
            measureHandler.OnBeat();
            GetBeatParameters(input, out currentBeats, out currentBeatType);
            currentAttributesElement.ReplaceChild(musicXmlElementFactory.TimeElement(currentBeats, currentBeatType), currentAttributesElement.SelectSingleNode("time"));
            // Append a TimeElement, but it must be packed within an AttributesElement.
            XmlNode emptyAttributesElement = musicXmlElementFactory.EmptyAttributesElement();
            CurrentMeasure.AppendChild(emptyAttributesElement);
            emptyAttributesElement.AppendChild(musicXmlElementFactory.TimeElement(currentBeats, currentBeatType));
        }
        
        protected void OnDaCapoAndDalSegno(InputInterpretation input)
        {
            InputSubCategoryEnum subCategory = input.SubCategory;
            switch (input.SubCategory)
            {
                case InputSubCategoryEnum.DaCapoAndDalSegnoPrintSegno:
                    Logger.LogCF(string.Format(": Category={0} Subcategory={1} FriendlyValue={2}", input.Category, input.SubCategory, input.FriendlyValue));
                    XmlNode segnoDirectionElement = musicXmlElementFactory.SegnoDirectionElement();
                    CurrentMeasure.AppendChild(segnoDirectionElement);
                    break; 

                case InputSubCategoryEnum.EndOfBrailleOnlySegnoPassage:
                case InputSubCategoryEnum.PrintDaCapoOrDC:
                case InputSubCategoryEnum.BrailleOnlyDaCapo:
                case InputSubCategoryEnum.BrailleOnlySegnoWithLetter:
                case InputSubCategoryEnum.BrailleOnlyDalSegnoWithLetter:
                case InputSubCategoryEnum.PrintEncircledCrossCodaSign:
                    Logger.LogCF(string.Format(": Category={0} Unimplemented subcategory={1}", input.Category, input.SubCategory));
                    break;

                default:
                    Logger.LogCF(string.Format(": Category={0} Unexpected subcategory={1}", input.Category, input.SubCategory));
                    break;
            }
        } 



        protected void OnFinalDoubleBar()
        {
            XmlNode rightBar = musicXmlElementFactory.BarLineElement(MusicXmlElementFactory.BarStyleEnum.lightHeavy, MusicXmlElementFactory.BarLocationEnum.right);
            CurrentMeasure.AppendChild(rightBar);
        }

        //protected void OnSectionalDoubleBar()
        //{
        //    XmlNode rightBar = elements.BarLineElement(MusicXmlBuilderElements.BarStyleEnum.lightLight, MusicXmlBuilderElements.BarLocationEnum.right);
        //    CurrentMeasure.AppendChild(rightBar);
        //}

        protected void OnPrintRepeatsAndEndings(InputSubCategoryEnum subCategory,List<string> values)
        {
            switch (subCategory)
            {
                //Print-repeats: Graphic information, also interpreted by the sighted user.
                case InputSubCategoryEnum.OthervaluesDoubleBarFollowedByDots: graphicRepeatHandler.OnStartRepeat(CurrentMeasure); break; // Graphic "Start repeat"
                case InputSubCategoryEnum.OthervaluesDoubleBarPrecededByDots: graphicRepeatHandler.OnEndRepeat(CurrentMeasure); break;  // Graphic "End repeat"
                case InputSubCategoryEnum.OthervaluesVolta1FirstEnding: graphicRepeatHandler.OnFirstEnding(CurrentMeasure, "1"); break; // Graphic "End repeat, first ending"
                case InputSubCategoryEnum.OthervaluesVolta2SecondEnding: graphicRepeatHandler.OnSecondEnding(CurrentMeasure, "2"); break;  // Graphic "End repeat, second ending"
                case InputSubCategoryEnum.OthervaluesVoltaIntervalEnding: graphicRepeatHandler.OnIntervalEnding(CurrentMeasure, values); break;  // Graphic "End repeat, ending n to m"
                case InputSubCategoryEnum.OthervaluesVoltaNumericEnding: graphicRepeatHandler.OnNumericEnding(CurrentMeasure, values); break; // Graphic "End repeat, ending n > 2"
                default: LogCF(string.Format(": Unexpected subcategory{0}", subCategory)); break;
            }
        }

        /// <summary>
        /// Common code for handling TokenReader.EmbeddedTExtRepresentation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pendingWedgeStops"></param>
        protected void OnEmbeddedTextRepresentation(InputInterpretation input, List<InputSubCategoryEnum> pendingWedgeStops)
        {
            string dynamicString = "";
            string wordsString = "";
            string wedgeType = "";
            switch (input.SubCategory)
            {
                case InputSubCategoryEnum.TextPiano: dynamicString = "p"; break;
                case InputSubCategoryEnum.TextPianoPianissimo: dynamicString = "pp"; break;
                case InputSubCategoryEnum.TextMezzoForte: dynamicString = "mf"; break;
                case InputSubCategoryEnum.TextMezzoPiano: dynamicString = "mp"; break;
                case InputSubCategoryEnum.TextForteFortissimo: dynamicString = "ff"; break;
                case InputSubCategoryEnum.TextCrescentoStart: wedgeType = "crescendo"; pendingWedgeStops.Add(InputSubCategoryEnum.TextCrescentoEnd); break;
                case InputSubCategoryEnum.TextDiminiuendoStart: wedgeType = "diminuendo"; pendingWedgeStops.Add(InputSubCategoryEnum.TextDiminiuendoEnd); break;
                case InputSubCategoryEnum.TextCrescentoEnd: wedgeType = "stop"; pendingWedgeStops.Remove(InputSubCategoryEnum.TextCrescentoEnd); break;
                case InputSubCategoryEnum.TextDiminiuendoEnd: wedgeType = "stop"; pendingWedgeStops.Remove(InputSubCategoryEnum.TextDiminiuendoEnd); break;
                case InputSubCategoryEnum.TextFreeText: wordsString = input.FriendlyValue; break; // Such as "HOLDBACK" of "POCO CRES."
                default: break;
            }
            if (!string.IsNullOrEmpty(dynamicString))
            {
                XmlNode direction = musicXmlElementFactory.DirectionElement();
                XmlNode directionType = musicXmlElementFactory.DirectionTypeElement();
                XmlNode dynamics = musicXmlElementFactory.DynamicsElement(dynamicString);
                CurrentMeasure.AppendChild(direction);
                direction.AppendChild(directionType);
                directionType.AppendChild(dynamics);
                // Aplication of a dynamic string causes any wedges to be terminated
                {
                    if (0 != pendingWedgeStops.Count)
                    {
                        wedgeType = "stop";
                    }
                }
            }

            if (!string.IsNullOrEmpty(wordsString))
            {
                if (!wordsString.StartsWith("."))
                {
                    switch (wordsString)
                    {
                        case "fine":
                            //currentMeasure.AppendChild(elements.FineDirectionElement());  break;
                            XmlNode previousMeasure = currentMeasure.PreviousSibling;
                            if (null != previousMeasure)
                            {
                                previousMeasure.AppendChild(musicXmlElementFactory.FineDirectionElement());
                            }
                            else
                            {
                                LogCF(": PreviousMeasure is null"); // Avoid crash, but log!
                            }
                            break;

                        default: XmlNode direction = musicXmlElementFactory.DirectionElement(wordsString);  CurrentMeasure.AppendChild(direction); break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(wedgeType))
            {
                XmlNode direction = musicXmlElementFactory.DirectionElement();
                XmlNode directionType = musicXmlElementFactory.DirectionTypeElement();
                XmlNode wedgeElement = musicXmlElementFactory.WedgeElement(wedgeType);
                CurrentMeasure.AppendChild(direction);
                direction.AppendChild(directionType);
                directionType.AppendChild(wedgeElement);
            }


            // LogCF(string.Format(": {0} Interpretation={1}", input.ToDebugString(), dynamicString));


        }


        public override void AddPart(InputInterpretation input)
        {
            string partName = string.Format("P{0}", partList.ChildNodes.Count + 1); // Generate the "Pn" name.Number from 1 and up 

            // Add a score-part to the partList
            XmlNode scorePart = musicXmlElementFactory.Element("score-part");
            scorePart.Attributes.Append(musicXmlElementFactory.NameValueAttribute("id", partName));
            scorePart.AppendChild(musicXmlElementFactory.NameValuePair("part-name", input.FriendlyValue));
            partList.AppendChild(scorePart);

            // Add a part to the score
            XmlNode part = musicXmlElementFactory.Element("part");
            part.Attributes.Append(musicXmlElementFactory.NameValueAttribute("id", partName));
            scorePartwiseNode.AppendChild(part);
            currentPart = part;
            CurrentMeasureNumber = FirstMeasureNumber;

            // Create an element for holding such things as key, clef, beat etc until we have a MeasureElement available
            // We need to create empty placeholders because the sequence of XML nodes must be correct !
            currentAttributesElement = musicXmlElementFactory.AttributesElement(DivisionsPerQuarterNote, currentBeats, currentBeatType); // This is an Xml Element named "Attributes"
            XmlNode newClefElement = musicXmlElementFactory.ClefElement(input.Category, input.FriendlyValue, input.SubCategory);
            XmlNode oldClefElement = currentAttributesElement.SelectSingleNode("clef");
            //currentAttributesElement.RemoveChild(oldClefElement);
            if (null == newClefElement)
            {
                newClefElement = musicXmlElementFactory.ClefElement("G", 2, 0); // Create an empty default clefelement to please MuseScore
            }

            currentAttributesElement.ReplaceChild(newClefElement, oldClefElement);
        }

        // Common helper method for generating LogLines
        public override string GetDebugInfo()
        {
            string s = string.Format("Part='{0}' CurrentMeasureNumber={1} InnerXml={2}", this.FriendlyName, CurrentMeasureNumber, (null == currentMeasure) ? "" : currentMeasure.InnerXml);
            return s;
        }

        /// <summary>
        /// Simple implementation for cloning last measure
        /// </summary>
        public void RepeatLatestFullMeasure()
        { 
            MusicXmlMeasureElement lastMeasure = CurrentPart.LastChild as MusicXmlMeasureElement;
            MusicXmlMeasureElement clonedMeasure = lastMeasure.Clone(this.CurrentMeasureNumber);
            CurrentPart.AppendChild(clonedMeasure);
            this.CurrentMeasureNumber++;      
            this.currentMeasure = clonedMeasure;
        }

        /// <summary>
        /// The generic mechanism for repeating full measaures
        /// </summary>
        /// <param name="repeatOffset">The offset of the first measure to repeat</param>
        /// <param name="repeatLength">The number of measures to repeat</param>
        protected virtual void RepeatFullMeasures(int repeatOffset, int repeatLength)
        {
            LogCF(string.Format(": {0} : RepeatSequence : Offset={1} Length={2}", LoggerLocationInfo, repeatOffset, repeatLength));
            int initialCurrentMeasureNumber = this.CurrentMeasureNumber;
            // Specified as "Repetitionstegn og partielle forkortelser" in Refsnæs I page 28
            // Append the measures as specified in offset and length.
            XmlNode thisPart = CurrentPart;
            MusicXmlMeasureElement lastChild = thisPart.LastChild as MusicXmlMeasureElement;
            thisPart.RemoveChild(lastChild); // We have just added a new measure, which we eant to replace with the first repeated child.         
            for (int i = 0; (i < repeatLength); i++)
            {
                int measureIndex = repeatOffset - repeatLength + i;
                MusicXmlMeasureElement original = thisPart.ChildNodes[measureIndex] as  MusicXmlMeasureElement;
                MusicXmlMeasureElement clone = original.Clone(original.MeasureNumber + repeatLength); // We need to clone the measure! The original measure can not be inserted twice !
                string message = string.Format(": Repeating Measure {0} (Index={1})  as Measure {2} (Index={3})", original.MeasureNumber, measureIndex, clone.MeasureNumber, measureIndex + repeatLength);
                LogCF(message);
                thisPart.AppendChild(clone);
            }
            int oldLastMeasureNumber = lastChild.MeasureNumber; // For debugging
            int newLastMeasureNumber = oldLastMeasureNumber + repeatLength;
            lastChild.MeasureNumber += repeatLength; // For debugging
            thisPart.AppendChild(lastChild); // Reinsert the last child, now still as the last child, but after the repeated notes.                
            this.currentMeasure = lastChild;
            string exitMessage = string.Format(": ReinInserted the original Measure {0} as Measure {1}", oldLastMeasureNumber, newLastMeasureNumber);
            LogCF(exitMessage);
            this.CurrentMeasureNumber = initialCurrentMeasureNumber + repeatLength; // Ready for next measure
        }

        public void ClonePreviousMeasure()
        {
            LogCF(string.Format(": Entry  {0}", GetDebugInfo()));
            XmlNode previousMeasure = currentMeasure.PreviousSibling;
            XmlNode theClone = previousMeasure.Clone();
            currentPart.InsertAfter(theClone, previousMeasure); // Inserts the clone between previousMeasure and currentMeasure
            theClone.Attributes.GetNamedItem("number").Value = CurrentMeasureNumber.ToString(); // Set the measurenumber of the clone to the measurenumber of the node it replaced.
            CurrentMeasureNumber++;
            currentMeasure.Attributes.GetNamedItem("number").Value = CurrentMeasureNumber.ToString(); // Set the measurenumber of the currentMeasure to its new value
            LogCF(string.Format(": Exit  {0}", GetDebugInfo()));
        }

        public void AddNewMeasure()
        {
            AddNewMeasure(MusicXmlElementFactory.BarStyleEnum.normal);
        }

        public void AddNewMeasure(MusicXmlElementFactory.BarStyleEnum barStyleEnum)
        {
            // Add a new measure to the current part
            currentMeasure = musicXmlElementFactory.MeasureElement(CurrentMeasureNumber);
            currentMeasure.AppendChild(musicXmlElementFactory.BarLineElement(barStyleEnum,MusicXmlElementFactory.BarLocationEnum.left)); // Locate the barline first in the measure

            if (FirstMeasureNumber == CurrentMeasureNumber)
            {
                //// Avoid overlap of work-title and first notes by adding a printeæleemnt hee.
                //XmlNode printElement = elements.PrintElement();
                //currentMeasure.AppendChild(printElement);

                XmlNode attributesElement = currentAttributesElement; // Here we have collected Key, Beats and Clef until the first measureElement became availale !
                                                                      //XmlNode attributesElement = doc.CreateElement("attributes");
                currentMeasure.AppendChild(attributesElement);
                //attributesElement.AppendChild(NameValuePair("divisions", divisionsPerQuarterNote.ToString()));
                // More to come...
            }

            CurrentMeasureNumber++;
            currentVoice = 1; // Voices within a part are numbered from1 and up
            currentPart.AppendChild(currentMeasure);
        }
   
        /// <summary>
        /// New implementation 2021
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetDivisions(UnAmbiguousNoteTypeEnum type)
        {
            int divisionsPerQuarterNote = DivisionsPerQuarterNote; // Constant
            switch (type)
            {
                case UnAmbiguousNoteTypeEnum.TypeFullMeasure: return GetWholeMeasureOrFullNoteDuration();
                case UnAmbiguousNoteTypeEnum.TypeWhole: return divisionsPerQuarterNote * 4;
                case UnAmbiguousNoteTypeEnum.TypeHalf: return divisionsPerQuarterNote * 2;
                case UnAmbiguousNoteTypeEnum.TypeQuarter: return divisionsPerQuarterNote * 1;
                case UnAmbiguousNoteTypeEnum.TypeEight: return divisionsPerQuarterNote / 2;
                case UnAmbiguousNoteTypeEnum.Type16th: return divisionsPerQuarterNote / 4;
                case UnAmbiguousNoteTypeEnum.Type32nd: return divisionsPerQuarterNote / 8;
                case UnAmbiguousNoteTypeEnum.Type64th: return divisionsPerQuarterNote / 16; // Max?
                case UnAmbiguousNoteTypeEnum.Type128th: return divisionsPerQuarterNote / 32; // Max?
#warning ToDo fix the Max
            }
            throw (new Exception(string.Format("GetDivisions: Unsupported type={0}", type.ToString())));
        }






        /// <summary>
        /// Returns the duration of a Full measure or a full note, whichever is smallest. Common for derived classes MusicXmlBuilderStatePart and MusicXmlBuilderStateHarmonyPart
        /// </summary>
        /// <returns></returns>
        protected int GetWholeMeasureOrFullNoteDuration()
        {
            // Assuming this is a FullMeasure or a FullNote:
            const int fullNoteDuration = DivisionsPerQuarterNote * 4;
            int fullMeasureDuration = (fullNoteDuration * currentBeats) / currentBeatType;
            if (fullMeasureDuration < fullNoteDuration)
            {
                // For instance in 3/4 the duration can never be > a full measure !
                LogCF(string.Format(": Returning duration=FullMeasureDuration={0} for BeatType={1}/{2}", fullMeasureDuration, currentBeats, currentBeatType));
                return fullMeasureDuration;
            }
            else
            {
                // LogCF(string.Format(": Returning duration=FullNoteDuration={0} for BeatType={1}/{2}", fullNoteDuration, currentBeats, currentBeatType));
                return fullNoteDuration;
            }
        }




        public string NonBrailleToString(string input)
        {
            switch (input)
            {
                case "\r": return "CR";
                case "\n": return "LF";
                default: return "??";
            }
        }

        protected MusicXmlBuilderStateMusic(MusicXmlBuilder musicXmlBuilder) : base(musicXmlBuilder)
        {
            partList = musicXmlBuilder.PartList;
            scorePartwiseNode = musicXmlBuilder.ScorePartwiseNode;
            this.currentBeats = musicXmlBuilder.DefaultBeats;
            this.currentBeatType = musicXmlBuilder.DefaultBeatType;
            this.graphicRepeatHandler = GraphicRepeatHandler.Create(musicXmlBuilder.MusicXmlElementFactory); 
        }
    }
}
