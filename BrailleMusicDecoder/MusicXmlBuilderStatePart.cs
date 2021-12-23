using System;
using System.Collections.Generic;
using System.Xml;
using BrailleMusicDecoder.MusicXmlElements;
using MusicXmlReaderModel;
using System.Text;
using BrailleMusicDecoder.MusicXmlHandlers;


namespace BrailleMusicDecoder
{
    class MusicXmlBuilderStatePart :  MusicXmlBuilderStateMusic
    {
        private MusicXmlNoteElement currentNote = null;

        // Handler for isolating code for handling various MusicXml objects in separate classes, reducing th amount of code in this class.
        private NoteOctaveHandler noteOctaveHandler;
        private IntervalOctaveHandler intervalOctaveHandler;
        private MusicXmlTieHandler tieHandler;
        private MusicXmlBuilderAccidentalHandler accidentalHandler;
        private BrailleRepeatHandler brailleRepeatHandler;
        private MusicXmlKeySignatureHandler keySignatureHandler;
        private InAccordPartMeasureHandler inAccordPartMeasureHandler;
        private ArticulationsHandler articulationsHandler;
        private GraceNoteHandler graceNoteHandler;
        private StickyAttributesHandler stickyAttributesHandler;
        private MusicXmlSlurHandler slurHandler;
        //private GraphicRepeatHandler graphicRepeatHandler;
        private TimeModificationHandler timeModificationHandler;
        private List<InputSubCategoryEnum> pendingWedgeStops = new List<InputSubCategoryEnum>();

        private bool isContinuedLine = false;

        // Interval notation  
        private IntervalDirectionEnum currentIntervalDirection = IntervalDirectionEnum.up;
        private List<MusicXmlNoteElement> currentIntervalNotes; // All noteElements currently taking part of an Interval Notation
        private MusicXmlNoteElement currentIntervalNotationBase // The noteElement currently acting as the (possible) base of an Interval notation
        {
            get
            {
                if (null == currentIntervalNotes) return null;
                if (0 == currentIntervalNotes.Count) return null;
                return currentIntervalNotes[0];
            }
        }    

        public override MusicXmlBuilderStateEnum GetState()
        {
            return MusicXmlBuilderStateEnum.Part;
        }

        ///// <summary>
        ///// Assure common algorithm for evaluating NoteType and Divisions
        ///// New implementation 2021
        ///// </summary>
        ///// <param name="u">Takes precedens if found</param>
        ///// <param name="a">Fallback value</param>
        ///// <returns></returns>
        //private UnAmbiguousNoteTypeEnum GetNoteType(UnAmbiguousNoteTypeEnum u, InputSubSubCategoryEnum a)
        //{
        //    if (UnAmbiguousNoteTypeEnum.TypeUnknown != u) return u; // Use the unambiguous value
        //    switch (a)
        //    {
        //        // Make an assumption:
        //        case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th: return UnAmbiguousNoteTypeEnum.Type16th; 
        //        case InputSubSubCategoryEnum.NoteTypeHalfOr32nd: return UnAmbiguousNoteTypeEnum.TypeHalf;
        //        case InputSubSubCategoryEnum.NoteTypeQuarterOr64th: return UnAmbiguousNoteTypeEnum.TypeQuarter;
        //        case InputSubSubCategoryEnum.NoteTypeEighthOr128th: return UnAmbiguousNoteTypeEnum.TypeEight;
        //        default: throw new Exception("");      
        //    }
        
        
        //}
#if false
        private string GetType(InputSubSubCategoryEnum type, int currentPositionWithinMeasure)
        {
            switch (type)
            {
                case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th:
                    // First primitive attempt to handle the ambiguity around duration i Music Braille: Unless we are at start of measure assume this is a 32th.
                    if (0 == currentPositionWithinMeasure)
                    {
                        return "whole";
                    }
                    else
                    {
                        return "16th";
                    }
                case InputSubSubCategoryEnum.NoteTypeHalfOr32nd: return "half";
                case InputSubSubCategoryEnum.NoteTypeQuarterOr64th: return "quarter";
                case InputSubSubCategoryEnum.NoteTypeEighthOr128th: return "eighth";
                case InputSubSubCategoryEnum.NoteType16th: return "16th";
            }
            return null;
        }
#endif

        /// <summary>
        /// New implementation 2021
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        protected string GetNoteType(UnAmbiguousNoteTypeEnum u)
        {
            switch (u)
            {
#warning todi difference between whole and fullmeasure
                case UnAmbiguousNoteTypeEnum.TypeFullMeasure: return "whole";
                case UnAmbiguousNoteTypeEnum.TypeWhole: return "whole";
                case UnAmbiguousNoteTypeEnum.TypeHalf: return "half";
                case UnAmbiguousNoteTypeEnum.TypeQuarter: return "quarter";
                case UnAmbiguousNoteTypeEnum.TypeEight: return "eighth";
                case UnAmbiguousNoteTypeEnum.Type16th: return "16th";
                case UnAmbiguousNoteTypeEnum.Type32nd: return "32nd";
                case UnAmbiguousNoteTypeEnum.Type64th: return "64th";
                case UnAmbiguousNoteTypeEnum.Type128th: return "128th";
                default: throw new Exception(string.Format("GetNoteType: Unsupported parameter {0}",u.ToString()));
            }            
        }


        private string GetStem(int voice)
        {
            switch (voice)
            {
                case 1: return "up";
                case 2: return "down";
                case 3: return "down"; // In some very seldom cases we have 3 voices inside a single part. Just take a decision.
                case 4: return "down"; // In some very seldom cases we have 4 voices inside a single part. Just take a decision.
                default:
                    string s = string.Format("GetStem({0}): Invalid parameter", voice);
                    Logger.LogCF(": " + s);
                    // throw new Exception(s);
                    return "down"; 
            }
        }

#if false
        private int GetDivisions(InputSubSubCategoryEnum type, int currentPositionWithinMeasure)
        {
            int divisionsPerQuarterNote = DivisionsPerQuarterNote; // Constant
            switch (type)
            {
#warning TODO Check if NoteTypeFull should involve current beattype and beat
                case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th:
                    // First primitive attempt to handle the ambiguity around duration i Music Braille: Unless we are at start of measure assume this is a 32th.
                    if (0 == currentPositionWithinMeasure)
                    {
                        return divisionsPerQuarterNote * 4; ;
                    }
                    else
                    {
                        return divisionsPerQuarterNote / 4 ;
                    }  
                case InputSubSubCategoryEnum.NoteTypeHalfOr32nd: return divisionsPerQuarterNote * 2;
                case InputSubSubCategoryEnum.NoteTypeQuarterOr64th: return divisionsPerQuarterNote;
                case InputSubSubCategoryEnum.NoteTypeEighthOr128th: return divisionsPerQuarterNote / 2;
                case InputSubSubCategoryEnum.NoteType16th: return divisionsPerQuarterNote / 4;
            }
            throw (new Exception(string.Format("GetDivisions: Unsupported type={0}", type.ToString())));
        }
#endif

//        /// <summary>
//        /// To be used by TypeAmbiguitHandler to detect and resolve conflicts
//        /// </summary>
//        /// <param name="u"></param>
//        /// <param name="a"></param>
//        /// <returns></returns>
//        public int GetDivisions(UnAmbiguousNoteTypeEnum u, InputSubSubCategoryEnum a)
//        {
//            UnAmbiguousNoteTypeEnum u1 = (GetNoteType(u, a));
//            return GetDivisions(u1);
//        }

//        /// <summary>
//        /// New implementation 2021
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        private int GetDivisions(UnAmbiguousNoteTypeEnum type )
//        {
//            int divisionsPerQuarterNote = DivisionsPerQuarterNote; // Constant
//            switch (type)
//            {
//                case UnAmbiguousNoteTypeEnum.TypeFullMeasure: return GetWholeMeasureOrFullNoteDuration();
//                case UnAmbiguousNoteTypeEnum.TypeWhole: return divisionsPerQuarterNote * 4;
//                case UnAmbiguousNoteTypeEnum.TypeHalf: return divisionsPerQuarterNote * 2;
//                case UnAmbiguousNoteTypeEnum.TypeQuarter: return divisionsPerQuarterNote * 1;
//                case UnAmbiguousNoteTypeEnum.TypeEight: return divisionsPerQuarterNote / 2;
//                case UnAmbiguousNoteTypeEnum.Type16th: return divisionsPerQuarterNote / 4;
//                case UnAmbiguousNoteTypeEnum.Type32nd: return divisionsPerQuarterNote / 8;
//                case UnAmbiguousNoteTypeEnum.Type64th: return divisionsPerQuarterNote / 16; // Max?
//                case UnAmbiguousNoteTypeEnum.Type128th: return divisionsPerQuarterNote / 32; // Max?
//#warning ToDo fix the Max
//            }
//            throw (new Exception(string.Format("GetDivisions: Unsupported type={0}", type.ToString())));
//        }


        public override void AddPart(InputInterpretation input) 
        {
            base.AddPart(input);
            currentIntervalDirection = (InputSubCategoryEnum.HandRight == input.SubCategory) ? IntervalDirectionEnum.down : IntervalDirectionEnum.up; // Only down for right hand
        }


        private void LogFC(string s)
        {
            Logger.LogCF1(s);
        }


        private void VerifyCurrentMeasure()
        {
            string location = LoggerLocationInfo;
            // LogCF(string.Format(": {0} : CurrentPositionWithinMeasure={1} ", location, currentPositionWithinMeasure));
            // Check if we are at the end of the current measure as expected.
#warning: TODO avoid  comparing measurenumbers to test for anacrusis
            if ((0 != currentBeatType) && (null != CurrentMeasure) && (CurrentMeasureNumber == FirstMeasureNumber + 1))
            {
                int expectedPosition = currentBeats * DivisionsPerQuarterNote * 4 / currentBeatType;
                if (currentPositionWithinMeasure != expectedPosition)
                {
                    LogCF(string.Format(": {0} : Unexpected position : Got={1} Expected={2}", location, currentPositionWithinMeasure, expectedPosition));
                    if (null != CurrentMeasure.FirstChild)
                    {
                        XmlNode firstNote = null;
                        // Insert immediately before first note
                        foreach (XmlNode node in CurrentMeasure.ChildNodes)
                        {
                            if (0 == string.Compare(node.Name, "note"))
                            {
                                firstNote = node;
                                break;
                            }
                        }
                        if (null != firstNote)
                        {
                            int forwardDuration = expectedPosition - currentPositionWithinMeasure;                
                            LogCF(string.Format(": {0} : Inserted ForwardElement({1})", location, forwardDuration));
                            CurrentMeasure.InsertBefore(musicXmlElementFactory.ForwardElement(forwardDuration), firstNote);
                        }
                    }
                }
            }

        }



        private void OnNewMeasure(MusicXmlElementFactory.BarStyleEnum barStyleEnum)
        {
            timeModificationHandler.OnNewMeasure();
            brailleRepeatHandler.OnNewMeasure();
            inAccordPartMeasureHandler.OnNewMeasure();
            stickyAttributesHandler.OnNewMeasure();
            bool addNewMeasure = measureHandler.OnSpace();

            if (addNewMeasure)
            {
                VerifyCurrentMeasure(); // First verify the current measure, possibly adding a forward-element at the beginning if the current measure is an anacrusis (Danish: "Optakt")
                                        //                        repeatHandler = MusicXmlRepeatHandler.Create();
                AddNewMeasure(barStyleEnum);
                currentPositionWithinMeasure = 0;
                accidentalHandler.OnNewMeasure(); // Tell the Accidental Handler that a new measure has atarted.
                SectionHeaderHandler shh = musicXmlBuilder.SectionHeaderHandler; // Just a simple shorthand !
                string sectionHeader =  musicXmlBuilder.SectionHeaderHandler.GetSectionHeader(true); // true <==> SectionHeader returned to first caller only!
                if (!string.IsNullOrEmpty(sectionHeader))
                {
                    XmlNode direction = musicXmlElementFactory.DirectionElement(string.Format("({0})", sectionHeader));
                    LogCF(string.Format(": Adding Direction containing Sectionheader='{0}' to Measure={1} Part={2}", sectionHeader, currentMeasureNumber,CurrentPartName));
                    CurrentMeasure.AppendChild(direction);
                }
            }
        }


        /// <summary>
        /// Either caused by a
        /// 1) A single occurance of the "Interval" symbol or
        /// 2) An occurance of a note after a double occurance of the "Interval" symbol
        /// </summary>
        /// <param name="interval"></param>
        private void AddChordNote(InputSubCategoryEnum interval)
        {
            // Next lines are copied from case Interval in this switch !
            MusicXmlNoteElement chordNote = musicXmlElementFactory.ChordNoteElement(currentNote, interval, currentIntervalDirection, accidentalHandler); // Down for the right hand, up for the left
            int chordOctave = intervalOctaveHandler.CurrentChordOctaveNumber;
            if (IntervalOctaveHandler.undefined != chordOctave)
            {
                chordNote.ModifyOctaveNumber(chordOctave);
            }
            CurrentMeasure.AppendChild(chordNote);
            currentIntervalNotes.Add(chordNote); // Now contains the "Base" NoteElement in position 0 and 1 or more other NoteElements 
        }


        /// <summary>
        /// Some transcribers add a new measure to the start of a new section. some do not. Attemp to handle both situations!
        /// </summary>
        /// <param name="s">Identification of the new section.</param>
        public override void OnSectionHeader(string s)
        {
            LogCF(":");
            XmlNode current = CurrentMeasure;
            int nChildren = CurrentMeasure.ChildNodes.Count;
            LogCF(string.Format("({0}) MeasureNumber={1} nChildren={2}",s,CurrentMeasureNumber,nChildren));

            //Count number of notes and number of rests. If either != 0 create a new measure.
            int nNotes = 0; // All notes
            int nRests = 0; // All rests
            int nOthers = 0;// all other child nodes 
            foreach (XmlNode childNode in CurrentMeasure.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "note": nNotes++; break;
                    case "rest": nRests++; break;
                    default: nOthers++; break;
                }
            } 

            if (0 != (nNotes + nRests))
            {
                // The current measure is not only an empty measure so ve need to add an extra measure at the start of the new section.
                LogCF(string.Format(": Creating a new measure at the start of new section {0} for part={1}.",s,this.CurrentPartName));
                OnNewMeasure(MusicXmlElementFactory.BarStyleEnum.normal);
            }
        }

   

        public override MusicXmlBuilderState ApplyNextInput(InputInterpretation input)
        {
            MusicXmlBuilderState result = this;
            XmlNode typeNode = null; // Used for placing some new nodes
            //XmlNode currentMeasure = CurrentMeasure;
            XmlNode currentAttributesElement = CurrentAttributesElement;
            bool flushText = true; // USed during debug only to inspect text items
            intervalOctaveHandler.OnAnyInput(input.Category);
            bool isLineContinuation = false; 
            switch (input.Category)
            {
                case InputCategoryEnum.LineContinuation:
                    break; // A CRLF followed by 4 spaces is not a new measure, but a continuation forced by a limited page width.

                case InputCategoryEnum.FinalDoubleBar: base.OnFinalDoubleBar(); break; // The end of the part !
                case InputCategoryEnum.SectionalDoubleBar: OnNewMeasure(MusicXmlElementFactory.BarStyleEnum.lightLight); break; // A Halfend is nothing but a double measurebar !
                case InputCategoryEnum.NewMeasure: OnNewMeasure(MusicXmlElementFactory.BarStyleEnum.normal);  break; // A plain normal barline
             

                case InputCategoryEnum.Note:
                    noteOctaveHandler.OnNote();
                    intervalOctaveHandler.OnNote(); // Tell the ChordOctaveHandler that any pending octavenumber is not for an interval
                    measureHandler.OnNote();
                    // Add a new note to the current measure
                    string noteName = input.ToFullStepString() ; // In the "Note" case inputSubCategory carries the step information
                    //                    string type = GetType(input.SubSubCategory, currentPositionWithinMeasure); // In the "Note" case inputSubSubCategory carries the type information
                    //                    int divisions = GetDivisions(input.SubSubCategory, currentPositionWithinMeasure);
                    UnAmbiguousNoteTypeEnum unAmbiguousNoteType = input.GetNoteType();
                    string type = GetNoteType(unAmbiguousNoteType);
                    int divisions = GetDivisions(unAmbiguousNoteType);
                    int semitonesWithinOctave = noteOctaveHandler.GetSemiTonesWithinOctave(noteName);
                    int octave = noteOctaveHandler.GetOctave(semitonesWithinOctave);
                    accidentalHandler.Set(input.SubCategory); // Tie the current accidental to this FullStep for the rest of the measure
                    int alter = accidentalHandler.GetAlter(input.SubCategory); // Lookup up the Alter value for this fullStep                
                    XmlNode timeModification = null;
                    XmlNode tupletBracket =  null;
                    timeModificationHandler.GetNextTupleInfo(out timeModification, out tupletBracket, ref divisions); // Get information for handling Time modification. Typically both are null !
                    currentNote = musicXmlElementFactory.NoteElement(musicXmlElementFactory.PitchElement(noteName, octave, alter), divisions, type, timeModification,CurrentVoice, GetStem(CurrentVoice), tupletBracket);
                    CurrentMeasure.AppendChild(currentNote);
                    input.XmlNode = currentNote; // For generating synthesized sound
               
                    currentIntervalNotes = new List<MusicXmlNoteElement>();
                    currentIntervalNotes.Add(currentNote);
                    tieHandler.AddTieStop(currentNote);

                    // If the stickyAttributesHandler contains "sticky" articulations we must add them all before we add the articulations to the note
                    foreach (InputSubCategoryEnum articulation in stickyAttributesHandler.Articulations)
                    {
                        articulationsHandler.Add(articulation);
                    }
                    articulationsHandler.AddArticulations(currentNote,true); 
                                
                    brailleRepeatHandler.OnNote(currentNote);
                    if (!graceNoteHandler.OnNote(currentNote))
                    {
                        // Grace notes have no duration !
                        currentPositionWithinMeasure += divisions;
                    }

                    // If the stickyAttributesHandler contains "sticky" intervals we must add them all:
                    foreach (InputSubCategoryEnum interval in stickyAttributesHandler.Intervals)
                    {
                        AddChordNote(interval); 
                    }
         

                    break;

                case InputCategoryEnum.Octave:
                    int explicitOctave = int.Parse(input.FriendlyValue);
                    noteOctaveHandler.OnOctaveNumber(explicitOctave); // Handling of octave numbers outside chords
                    intervalOctaveHandler.OnOctave(explicitOctave); // Handling of octave numbers within chords
                    break;

                case InputCategoryEnum.InsertedRest: // An inserted rest is not found in the graphic  notesheet, but has been inserted by th Music Braille transscriber for clarity !
                case InputCategoryEnum.Rest:
                    measureHandler.OnRest();
                    //string restType = GetType(input.SubSubCategory, currentPositionWithinMeasure);
                    //int restDivisions = GetDivisions(input.SubSubCategory, currentPositionWithinMeasure);
                    UnAmbiguousNoteTypeEnum unAmbiguousRestType = input.GetNoteType();
                    string restType = GetNoteType(unAmbiguousRestType);
                    int restDivisions = GetDivisions(unAmbiguousRestType);
                    MusicXmlNoteElement restElement = musicXmlElementFactory.RestElement(restDivisions, restType, CurrentVoice);
                    currentNote = restElement; // Rests are handled much like notes
                    CurrentMeasure.AppendChild(restElement);
                    currentPositionWithinMeasure += restDivisions;
                    brailleRepeatHandler.OnRest(restElement);
                    break;

                case InputCategoryEnum.InAccordFullMeasure:
                    // LogCF(string.Format(": Category={0} Input={1}",input.Category, input.ToDebugString("")));
                    brailleRepeatHandler.OnInAccordFullMeasure();
                    measureHandler.OnFullMeasureInAccord();
                    // This is the "FullMeasure" variant, so w go back to the start of the measure.
                    if (0 != currentPositionWithinMeasure)
                    {
                        XmlNode backUpElement = musicXmlElementFactory.BackUpElement(currentPositionWithinMeasure);
                        CurrentMeasure.AppendChild(backUpElement);
                        currentPositionWithinMeasure = 0;
                    }
                    else
                    {
                        Logger.LogCF(string.Format(": Skipping generation og BackUpElement with zero duration"));
                    }
                    CurrentVoice++;
                    break;

#warning TODO Implement "Backup"

                case InputCategoryEnum.MeasureDivision:
                    inAccordPartMeasureHandler.OnMeasureDivision(currentPositionWithinMeasure);
                    break;

                case InputCategoryEnum.InAccordPartMeasure:
                    measureHandler.OnPartMeasureInAccord();
                    int backup = inAccordPartMeasureHandler.OnInAccordPartMeasure(currentPositionWithinMeasure);
                    // This is the "PartMeasure" variant, so we go back to the latest MeasureDivision sign 
                    if (backup > 0)
                    {
                        XmlNode backUpElement = musicXmlElementFactory.BackUpElement(backup);
                        CurrentMeasure.AppendChild(backUpElement);
                        currentPositionWithinMeasure -= backup;
                    }
                    else
                    {
                        Logger.LogCF(string.Format(": Skipping generation og BackUpElement with zero or negative duration"));
                    }
                    CurrentVoice++;
                    break;

                case InputCategoryEnum.Hand:
                    if (this.isContinuedLine)
                    {
                        string warning = string.Format("Linecontinuation immediately followed by change of hand is not supported!");
                        base.AddSelectedEvent(warning);                       
                        Logger.LogCF(string.Format(": {0}", warning));
                    }

                    // If the Hand symbol  represents a new part we first create a new MusicXmlBuilderState object to represent it
                    // otherwise we just reuse the existing object.
                    result = MusicXmlBuilderState.Create(MusicXmlBuilderStateEnum.Part, musicXmlBuilder, input.FriendlyValue); // ******************** New code !! *******************
                    //if (result != this)
                    // NO! Only if result represents a newle created state !!!!

                    if (input.SubSubCategory == InputSubSubCategoryEnum.HandRightIntervalsReadUpward)
                    {
                        Logger.LogCF(": InputSubSubCategoryEnum.HandRightIntervalsReadUpward");
                    }

                    if ((result as MusicXmlBuilderStatePart).CurrentPart == null)
                    {  
                        result.AddPart(input);
                    }
                    break;

                case InputCategoryEnum.SectionHeader:
                    base.AddSelectedEvent(string.Format("SectionHeader='{0}' Part={1} PartName='{2}', MeasureNumber={3}", input.FriendlyValue, CurrentPartName, this.friendlyName, CurrentMeasureNumber));
                    break;

                case InputCategoryEnum.NonBrailleCharacter:
                    LogCF(input.ToDebugString());
                    break;

                case InputCategoryEnum.Clef:
                    //currentAttributesElement.AppendChild(ClefElement(inputValue));
                    currentAttributesElement.ReplaceChild(musicXmlElementFactory.ClefElement(input.Category, input.FriendlyValue, input.SubCategory), currentAttributesElement.SelectSingleNode("clef"));
                    break;
#warning TODO Remove existing ClefElement first


                case InputCategoryEnum.Beat: OnBeat(input); break;
                    //GetBeatParameters(input, out currentBeats, out currentBeatType);
                    //currentAttributesElement.ReplaceChild(musicXmlElementFactory.TimeElement(currentBeats, currentBeatType), currentAttributesElement.SelectSingleNode("time"));
                    //// Append a TimeElement, but it must be packed within an AttributesElement.
                    //XmlNode emptyAttributesElement =  musicXmlElementFactory.EmptyAttributesElement();
                    //CurrentMeasure.AppendChild(emptyAttributesElement);
                    //emptyAttributesElement.AppendChild(musicXmlElementFactory.TimeElement(currentBeats, currentBeatType));                
                    //break;


                case InputCategoryEnum.InAccordTie:
                    // Do the same as for the normal tie, but for each note in currentIntervalNotes
                    //LogCF(string.Format("CurrentIntervalNotes contains {0} NoteElements", currentIntervalNotes.Count));
                    // LogCF(string.Format(": Category={0} Input={1}",input.Category, input.ToDebugString()));
                    foreach (MusicXmlNoteElement note in currentIntervalNotes)
                    {                    
                        note.AddTieAndTied(musicXmlElementFactory, "start");
                        tieHandler.Add(note);
                    }
                    break;

                case InputCategoryEnum.Tie:
                    // Do the same as for the InAccordTie, but only for currentNote
                    //LogCF(string.Format("({0},{1})", input.Category.ToString(), input.FriendlyValue));
                    // LogCF(input.ToDebugString());
                    currentNote.AddTieAndTied(musicXmlElementFactory, "start");
                    tieHandler.Add(currentNote);
                    break;

                case InputCategoryEnum.Punctuation:
                    typeNode = currentNote.SelectSingleNode("type");
                    XmlNode durationNode = currentNote.SelectSingleNode("duration");
                    string durationValue = durationNode.FirstChild.Value;
                    int existingDuration = int.Parse(durationValue);
                    int newDuration = existingDuration;
                    int nDots = 0;
                    switch (input.SubCategory)
                    {
                        case InputSubCategoryEnum.PunctuationSingle: newDuration = (existingDuration / 2) * 3; nDots = 1; break;
                        case InputSubCategoryEnum.PunctuationDouble: newDuration = (existingDuration / 4) * 7; nDots = 2; break;
                        case InputSubCategoryEnum.PunctuationTriple: newDuration = (existingDuration / 8) * 15; nDots = 3; break;
                        default: throw new Exception(string.Format("Unexpected value of SubCategory: {0}", input.SubCategory));

                    }
                    durationNode.FirstChild.Value = (newDuration).ToString();
                    currentPositionWithinMeasure += (newDuration - existingDuration); // Needed to implement FullMeasureInAccord notation using BackUpElement
                    // Insert 1,2 or 3 dots after the typeNode
                    for (int i = 0; i < nDots; i++)
                    {
                        XmlNode dotNode = musicXmlElementFactory.EmptyElement("dot");
                        currentNote.InsertAfter(dotNode, typeNode);
                    }
                    break;

                case InputCategoryEnum.Accidental:
                    // Note: Accidentals in Music Braille are placed BEFORE the note they relate to !
                    accidentalHandler.CurrentAccidental = input.SubCategory; // Save until the NoteElement is available
                    break;

                case InputCategoryEnum.Interval:
                    intervalOctaveHandler.OnInterval();
                    noteOctaveHandler.OnInterval(); // Tell the cotaveHandler that any pending octave number is not for notes
                    // LogCF(input.ToDebugString(": Category=Interval Input="));
                    // In Music Braille the Interval sign represents a new note identical to the previous one, except that pitch is changed according to the interval size.
                    // In MusicXml we represent this with a new NoteElement with a different pitch and containing a ChordElement.
                    if (stickyAttributesHandler.OnInterval(input.SubCategory, input.SubSubCategory))
                    {
                        AddChordNote(input.SubCategory);
                    }
                    break;

                case InputCategoryEnum.Slur: slurHandler.OnSlur(currentNote,input.SubCategory,input.SubSubCategory); break;

                case InputCategoryEnum.Chords:
                    input.FriendlyValue = ""; // Not needed.
                    result = MusicXmlBuilderState.Create(MusicXmlBuilderStateEnum.Harmonies, this.musicXmlBuilder); // Change state !
#if true
                    if ((result as MusicXmlBuilderStateMusic).CurrentPart == null)
                    {
                        // Only add the harmony part first time it occurs, exactly in the same way as normal perts ! 
                        result.AddPart(input);
                    }
#else
                    result.AddPart(input); // For the moment we plan to model the chords as a separate part named "Chords"
#endif
                    break;

                case InputCategoryEnum.RepeatSequence:
                    // Call the most general version of Measure repeat, specifying 2 parameters: Relative offset to start repeat at and numberof measures to repeat! 
                    string repeatOffsetString = input.Values[0];
                    string repeatLengthString = input.Values[1];
                    int repeatOffset = int.Parse(repeatOffsetString);
                    int repeatLength = int.Parse(repeatLengthString);
                    base.RepeatFullMeasures(repeatOffset, repeatLength);           
                    break;

                case InputCategoryEnum.EmbeddedTextRepresentation: base.OnEmbeddedTextRepresentation(input, pendingWedgeStops);   break;

                case InputCategoryEnum.GeneralSigns:
                    switch (input.SubCategory)
                    {
                        case InputSubCategoryEnum.PianoPedalDown:
                            XmlNode direction = musicXmlElementFactory.DirectionElement("Pedal down");
                            CurrentMeasure.AppendChild(direction);
//                            Logger.LogCF(string.Format(": PianoPedal Down"));
                            break;
                        default: LogCF(input.ToDebugString()); break;
                    }                  
                    break;

                case InputCategoryEnum.TextVersal:
                    nextCharIsVersal = true;
                    flushText = false;
                    break;

                case InputCategoryEnum.ChordCharacter:
                case InputCategoryEnum.ChordStemSign:
                case InputCategoryEnum.ChordTiming:
                case InputCategoryEnum.ChordNumericExtension:
                case InputCategoryEnum.ChordSymbolRoot:
                case InputCategoryEnum.ChordSymbolBass:
                case InputCategoryEnum.ChordSymbol:
                    OnError(input, true);
                    break; 

                case InputCategoryEnum.ToNumber:
//                    repeatHandler.OnToDigit();
                    LogCF(input.ToDebugString(": Unexpected input "));
                    break;

                case InputCategoryEnum.Digit:
 //                   repeatHandler.OnDigit(int.Parse(input.FriendlyValue));
                    LogCF(input.ToDebugString(": Unexpected Input "));
                    break;

                case InputCategoryEnum.Space:
                    LogCF(input.ToDebugString(": Unexpected Input: InputCategoryEnum.Space"));
                  //  throw new Exception("Unexpected input ");
                    break;

                case InputCategoryEnum.KeySignature:
                    if ((input.SubSubCategory == InputSubSubCategoryEnum.KeySignatureCancel) && (null != keySignatureHandler))
                    {
                        LogCF(string.Format(": Canceling current Key signature={0} by applying {1} naturals", keySignatureHandler.Fifths, input.FriendlyValue));
#warning TODO Handle cancelling of the 5 flats in for instance "Four Piano Blues.2 Andor Foldes measure 40 by showing 5 "natural" signs in the graphics. (They were added by text in measure 29, not explicitly) !
                    }
                    keySignatureHandler = MusicXmlKeySignatureHandler.Create(input.FriendlyValue); // Contains a string describing an integer in the interval [-7 .. 7]
                    XmlNode newKeyElement = musicXmlElementFactory.KeyElement(keySignatureHandler.Fifths);
                    currentAttributesElement.ReplaceChild(newKeyElement, currentAttributesElement.SelectSingleNode("key"));
                    accidentalHandler = MusicXmlBuilderAccidentalHandler.Create(keySignatureHandler.Fifths); // Create an AccidentalHandler with knowledge of the key signature.
#warning ToDo repaace the existing key with this one.
                    break;

                case InputCategoryEnum.KeySignatureText:
                    // Thi is a key signature found within text-context, not within note context
                    string s = input.ToDebugString();
                    LogCF(input.ToDebugString(string.Format(": InputCategoryEnum.KeySignatureText {0}",s)));
                    sbText.Append(input.LocalizedCategoryName + " " + input.LocalizedFriendlyValue);
                    break;


                case InputCategoryEnum.UnusualBarLine:
                    switch (input.SubCategory)
                    {
                        case InputSubCategoryEnum.UnusualBarLineSpecialPrintBarline:
                            CurrentMeasure.AppendChild(musicXmlElementFactory.BarLineElement(MusicXmlElementFactory.BarStyleEnum.dotted));
                            LogCF("");
                            break;
                        default:
                            CurrentMeasure.AppendChild(musicXmlElementFactory.BarLineElement(MusicXmlElementFactory.BarStyleEnum.dashed));
                            LogCF("");
                            break;
                    }
                    break;

                case InputCategoryEnum.ControlCharCRLF:
                case InputCategoryEnum.ControlCharCRLFNumber:               
                    switch (input.SubCategory)
                    {
                        case InputSubCategoryEnum.ControlCharCRLFContinued:
                            isLineContinuation = true; // Local measure variable
                            break;
                        case InputSubCategoryEnum.ControlCharCRLFOneDigit:
                        case InputSubCategoryEnum.ControlCharCRLFTwoDigits:
                        case InputSubCategoryEnum.ControlCharCRLFThreeDigits:
                        case InputSubCategoryEnum.None:
                            //LogCF(string.Format(": {0}", input.SubCategory.ToString()));
                            if (!string.IsNullOrEmpty(input.FriendlyValue))
                            {
                                CurrentMeasure.AppendChild(musicXmlElementFactory.DirectionElement("Meas="+input.FriendlyValue));
                            }
                            break;
                        default: LogCF(string.Format(": Category={0} Unexpected value of subCategory={1}",input.Category.ToString(), input.SubCategory.ToString()));  break;
                    }
                    break;

                case InputCategoryEnum.MusicalHyphenAndSpace:
                    // In its nature this symbol is always handled in the context of handling something else and should never occure on its own
                    // Just ignore this in the MusicXml file when it uccurs alone
                    break;

                case InputCategoryEnum.Articulation:
                    if (stickyAttributesHandler.OnArticulation(input.SubCategory, input.SubSubCategory))
                    {
                        articulationsHandler.Add(input.SubCategory);
                    }
                    // Logger.LogCF(input.ToDebugString());
                    break;

                case InputCategoryEnum.PartMeasureRepeat:
                    {
                        switch (input.SubCategory)
                        {
                            case InputSubCategoryEnum.PartMeasureRepeatOnce: brailleRepeatHandler.RepeatFromRepetitionStart(); break;// Repeat from the start of the latest  measure og fullpartInAccord, whatever is latest
                            case InputSubCategoryEnum.PartMeasureRepeatTwice: brailleRepeatHandler.RepeatFromRepetitionStart(2); break; // Repeat twice from the start of the latest  measure og fullpartInAccord, whatever is latest
                            default:    break;

                        }
                        break;
                    }


                case InputCategoryEnum.OtherValues:
                    switch (input.SubCategory)
                    {
                        case InputSubCategoryEnum.OthervaluesLongAppoggiatura:
                        case InputSubCategoryEnum.OthervaluesShortAppoggiatura:
                            // In Music Braille the Appoggitura preceedes the note,so we have to save the information until the NoteElement becomes available.
                            bool slash = true;
                            graceNoteHandler.OnGrace(slash);
                            break;

                        case InputSubCategoryEnum.OtherValuesOrnament: break;
#warning todo Handle ornaments
                        
                        //case InputSubCategoryEnum.OtherValuesPartMeasureRepeat: brailleRepeatHandler.RepeatFromRepetitionStart(); break;// Repeat from the start of the latest  measure og fullpartInAccord, whatever is latest
 
                        //case InputSubCategoryEnum.OtherValuesPartMeasureRepeatTwice: brailleRepeatHandler.RepeatFromRepetitionStart(2); break; // Repeat twice from the start of the latest  measure og fullpartInAccord, whatever is latest
                          
                        // Braille-only Fullmeasure repeats:      
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeat:
                            LogCF(string.Format(": OthervaluesFullMeasureRepeat: {0}",LoggingInfo()));
                            base.RepeatLatestFullMeasure();
                            //this.AddNewMeasure(MusicXmlBuilderElements.BarStyleEnum.normal);
                            break;
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatTwice:
                            LogCF(string.Format(": OthervaluesFullMeasureRepeatTwice: {0}",LoggingInfo()));
                            base.RepeatLatestFullMeasure(); base.RepeatLatestFullMeasure(); break;
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatNoInitialBlank:
                            LogCF(string.Format(": OthervaluesFullMeasureRepeatNoInitialBlank: {0}",LoggingInfo()));
                            brailleRepeatHandler.FullMeasureRepeatNoInitialBlank(); break;

                        case InputSubCategoryEnum.OtherValuesTrill:
                            break;
#warning todo handle trills

                        // Print-repeats: Graphic information, also interpreted by the sighted user. Handled by base class
                        case InputSubCategoryEnum.OthervaluesDoubleBarFollowedByDots:
                        case InputSubCategoryEnum.OthervaluesDoubleBarPrecededByDots:
                        case InputSubCategoryEnum.OthervaluesVolta1FirstEnding:
                        case InputSubCategoryEnum.OthervaluesVolta2SecondEnding:
                        case InputSubCategoryEnum.OthervaluesVoltaIntervalEnding:
                        case InputSubCategoryEnum.OthervaluesVoltaNumericEnding: base.OnPrintRepeatsAndEndings(input.SubCategory, input.Values); break;                                      

#warning todo handle both versions of Tremolo
                        case InputSubCategoryEnum.OthervaluesTremoloRepeatedNote: LogCF(": InputSubCategoryEnum.OthervaluesTremoloRepeatedNote is not implemented yet!"); break;
                        case InputSubCategoryEnum.OthervaluesTremoloAlternatingNotes: LogCF(": InputSubCategoryEnum.OthervaluesTremoloAlternatingNotes is not implemented yet!"); break;

                        default:
                            break;
                    }
                    break;

                case InputCategoryEnum.Character: flushText = base.OnCharacter(input); break;





                        case InputCategoryEnum.ToMusicBraille:
                    // This input has already triggered a statechange. No further action required.
                    break;

                case InputCategoryEnum.PrintPagination:
#warning TODO find out what to do here, if anything!
                    break;

                case InputCategoryEnum.DaCapoAndDalSegno:
                    LogCF(string.Format(": DaCapoAndDalSegno {0}", input.FriendlyValue));
                    base.OnDaCapoAndDalSegno(input); // Implement in base clas !
                    break;
                    
                case InputCategoryEnum.GuideDots:
#warning TODO find out what to do here, if anything!
                    break;

                case InputCategoryEnum.TimeModification:
                    switch (input.SubCategory)
                    {
                        case InputSubCategoryEnum.TimeModificationFermata:
                            XmlNode notations = currentNote.SelectSingleNode("notations");
                            notations.AppendChild(musicXmlElementFactory.Element("fermata"));
                            break;
                        case InputSubCategoryEnum.TimeModificationTriplet: 
                        default:
                            timeModificationHandler.OnTripletStart();            
                            break;
                    }
#warning TODO find out what to do here, if anything!
                    break;

                case InputCategoryEnum.ToText: break; // Ignore here: Causes a state transition, but no further action. 



                // Not seen in sample files yet.
                default:
                    base.OnUnsupportedInput(string.Format("Part='{0}'",this.friendlyName), input);           
                    //Logger.LogUserWarning(string.Format("Part='{0}' Unsupported InputCategory={1}", this.FriendlyName,input.Category.ToString())); // For the list of user warnings
                    string message = input.ToDebugString(": Not observed yet: ");
                    LogCF(input.ToDebugString(message)); // For the Logfile
                    break;
            }

            this.isContinuedLine = isLineContinuation;  // Update member variable from local variable. For detecting lines continued across changing parts

            base.OnEpilog(flushText);

            return result;

        }

        // For Logging only
        private string LoggingInfo()
        {
            string result = string.Format("Part={0} MeasureNumber={1}", this.CurrentPartName, this.currentMeasureNumber);
            return result;
        }

        private void OnError(InputInterpretation input, bool throwOnError)
        {
            string s = input.ToDebugString(string.Format(": Unexpected input in State='{0}': ", this.GetState()));
            LogCF(s);
            if (throwOnError)
            {
                throw new Exception(s); // Only active during debugging !
            }
        }


        public static MusicXmlBuilderState Create(MusicXmlBuilder musicXmlBuilder, string friendlyName)
        {
            return new MusicXmlBuilderStatePart(musicXmlBuilder,friendlyName);
        }

        private MusicXmlBuilderStatePart(MusicXmlBuilder musicXmlBuilder, string friendlyName) : base(musicXmlBuilder)
        {
            // In order to split the code among several classes we use the following handler classes:
            this.friendlyName = friendlyName;
            noteOctaveHandler = NoteOctaveHandler.Create();
            tieHandler = MusicXmlTieHandler.Create(musicXmlElementFactory);
            accidentalHandler = MusicXmlBuilderAccidentalHandler.Create(0); // Assume C majot / A minor until we see the Key Signature
            measureHandler = MusicXmlMeasureHandler.Create();
            brailleRepeatHandler = BrailleRepeatHandler.Create(musicXmlElementFactory, this);
            keySignatureHandler = MusicXmlKeySignatureHandler.Create();
            inAccordPartMeasureHandler = InAccordPartMeasureHandler.Create(musicXmlElementFactory);
            articulationsHandler = ArticulationsHandler.Create(musicXmlElementFactory);
            graceNoteHandler = GraceNoteHandler.Create(musicXmlElementFactory);
            stickyAttributesHandler = StickyAttributesHandler.Create();
            slurHandler = MusicXmlSlurHandler.Create(musicXmlElementFactory);
            intervalOctaveHandler = IntervalOctaveHandler.Create();
            timeModificationHandler = TimeModificationHandler.Create(musicXmlElementFactory);
    }
}
}

