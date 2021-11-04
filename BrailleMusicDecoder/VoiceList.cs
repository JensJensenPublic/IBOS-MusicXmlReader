using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{
    class VoiceList
    {  
        /// <summary>
        /// The main voice (Danish "HovedStemme")
        /// </summary>
        public Voice MainVoice { get { return allVoices[0]; } }

        /// <summary>
        /// The total number of voices, including the main voice
        /// </summary>
        public int NumberOfVoices { get { return allVoices.Count; } }

        /// <summary>
        /// The list of voices except the main voice (Danish "Bistemmer")
        /// </summary>
        public List<Voice> SideVoices
        {
            get
            {
                if (allVoices.Count == 0)
                {
                    return new List<Voice>();
                }
                else
                {
                    return allVoices.GetRange(1, allVoices.Count - 1);
                }
            }
        }

        // Data structures for categorizing the voices
        List<Voice>[] voiceCategories = new List<Voice>[(int)Voice.VoiceCategoryEnum.numberOfCategories];
        List<Voice> Div0Voices  { get { return voiceCategories[(int)Voice.VoiceCategoryEnum.Division0]; } }
        List<Voice> Div1Voices { get { return voiceCategories[(int)Voice.VoiceCategoryEnum.Division1]; } }
        List<Voice> Div2Voices { get { return voiceCategories[(int)Voice.VoiceCategoryEnum.Division2]; } }
        List<Voice> FullVoices  { get { return voiceCategories[(int)Voice.VoiceCategoryEnum.full]; } }

        private List<Voice> allVoices = new List<Voice>();
        public List<Voice> AllVoices { get { return allVoices; } }

        private const bool clone = true; // Parameter for the "Insert" methods

        /// <summary>
        /// Add a voice to the VoiceList.
        /// The first Voice added will become the Main voice (Danish: "HovedStemme") and can be accessed as the MainVoice property
        /// The remaining voices added wull become the Side voices (Danish: "Bistemmer" and can be accessed as the SideVoices property
        /// </summary>
        /// <param name="voice"></param>
        public void Add(Voice voice)
        {
            allVoices.Add(voice);
        }

  

        private VoiceList()
        { }

        const string UnsupportedVoiceList = "Unsupported VoiceList";

        // Issue a Log or a warning with a specific text
        private void LogAndWarn(string s)
        {
            Logger.LogCF1(string.Format(": {0}: {1}", UnsupportedVoiceList, s));
            UserWarnings.LogUserWarning(string.Format("{0}: {1}", UnsupportedVoiceList,s));
        }

        // Issue a Log or an warning when we meet an unsupported configuration
        private void LogAndWarn(int nMeasureDivisionMarks, int nInAccordPartMeasureMarks, int nInAccordFullMeasureMarks)
        {
            // Finally log any unsupported configurations
            if ((nMeasureDivisionMarks == 0) && (nInAccordPartMeasureMarks == 0) && (nInAccordFullMeasureMarks == 0)) return; // The default,linear case
            if ((nMeasureDivisionMarks == 1) && (nInAccordPartMeasureMarks >= 1) && (nInAccordFullMeasureMarks == 0)) return; // The simple InAccordPartMeasure case
            if ((nMeasureDivisionMarks == 0) && (nInAccordPartMeasureMarks == 0) && (nInAccordFullMeasureMarks == 1)) return; // The simple InAccordFullMeasure case
            Logger.LogCF1(string.Format(": {0}: nMeasureDivisionMarks={1} nInAccordPartMeasureMarks={2} nInAccordFullMeasureMarks={3}",
                UnsupportedVoiceList,nMeasureDivisionMarks, nInAccordPartMeasureMarks, nInAccordFullMeasureMarks)); // Not explicitly handled yet !
            UserWarnings.LogUserWarning(string.Format("{0}:  nDivisionsMarks={1}  nPartMeasure={2}  nFullMeasure={3})", UnsupportedVoiceList, nMeasureDivisionMarks, nInAccordPartMeasureMarks, nInAccordFullMeasureMarks));
        }

        private void AddVoice(Voice voice)
        {
            allVoices.Add(voice); // Add to the list of all voices
            voiceCategories[(int)voice.VoiceCategory].Add(voice); // Add to the list of all voices of this category.
        }

        private string ToCategoryString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Voice voice in allVoices)
            {
                sb.Append(voice.ToCategoryString() + " ");
            }
            return sb.ToString();
        }

        private void On1N1()
        {
            Logger.LogCF("");
            List<Voice> tempVoices = new List<Voice>();
            Voice pre = Div0Voices[0];
            Voice post = Div2Voices[0];
            for (int i = 0; (i < Div1Voices.Count); i++)
            {
                Voice common = Div1Voices[i];
                common.InsertAtStart(pre.InputInterpretations, clone);
                common.InsertAtEnd(post.InputInterpretations, clone);
                tempVoices.Add(common);
                // TODO Build up in temp
            }
            allVoices = tempVoices;
        }

        private void OnN1N()
        {
            Logger.LogCF("");
            List<Voice> tempVoices = new List<Voice>();
            Voice common = Div1Voices[0];
            for (int i = 0; i < Div0Voices.Count; i++)
            {
                Voice pre = Div0Voices[i];
                Voice post = Div2Voices[i];
                common.InsertAtStart(pre.InputInterpretations, clone);
                common.InsertAtEnd(post.InputInterpretations, clone);
                tempVoices.Add(common);
                //  TODO Build up in temp
            }
            allVoices = tempVoices;
        }

        private void On1N()
        {
            // The first voice is the common start of each or the remaining voices:
            List<Voice> tempVoices = new List<Voice>();
            Voice theLeftVoice = Div0Voices[0];
            for (int i = 1; i < allVoices.Count; i++)
            {
                allVoices[i].InsertAtStart(theLeftVoice.InputInterpretations, clone);
                tempVoices.Add(allVoices[i]);
            }
            allVoices = tempVoices;
        }




#if true

        /// <summary>
        /// Splits a measure into "divisions", controlled by occurances of the InAcccordPartMeasure mechanism
        ///   MeasureDivisionMarks
        ///   InAccordPartMeasureMarks
        ///   InAccordFullMeasureMarks
        /// and (if needed) merges them again, using the rules of   
        /// In the typical case, when the measure does not contain any of these marks, the measure is interpreted as a single voice
        /// When the measure contains any of these marks, the measure is interpreted as a number of "Divisions" each containing one or more Voices.
        /// Finally these voices are merged according to the rules of the InAcccordPartMeasure mechanism .
        /// </summary>
        /// <param name="inputInterpretations">The inputInterpretations of the Measure</param>
        /// <param name="expectedDuration">The expected duration of the Measure</param>
        private VoiceList(List<InputInterpretation> inputInterpretations, int expectedDuration)
        {
            // Establish 4 counters for roughly determining the structure of the measure
            int nMeasureDivisionMarks = 0;
            int nInAccordPartMeasureMarks = 0;
            int nInAccordFullMeasureMarks = 0;
            int nOtherSymbols = 0;

            for (int i = 0; i < voiceCategories.Length; i++)
            {
                voiceCategories[i] = new List<Voice>();
            }

            int nextVoiceNumber = 0;
            Voice.VoiceCategoryEnum currentVoiceCategory = Voice.VoiceCategoryEnum.Division0;
            Voice currentVoice = Voice.Create(nextVoiceNumber++, expectedDuration, inputInterpretations,currentVoiceCategory); 
            AddVoice(currentVoice);  

            foreach (InputInterpretation inputInterpretation in inputInterpretations)
            {  
                switch (inputInterpretation.Category)
                {
                    case InputCategoryEnum.InAccordPartMeasure:  // Start adding to a new voice in the same division 
                        nInAccordPartMeasureMarks++;
                        currentVoice = Voice.Create(nextVoiceNumber++, expectedDuration, inputInterpretations,currentVoiceCategory);  
                        AddVoice(currentVoice);                   
                        break;

                    case InputCategoryEnum.InAccordFullMeasure:     // Start adding to a new voice in the "Full" division
                        nInAccordFullMeasureMarks++;
                        currentVoice = Voice.Create(nextVoiceNumber++, expectedDuration, inputInterpretations, Voice.VoiceCategoryEnum.full); // Do not change currentVoiceCategory
                        AddVoice(currentVoice);
                        break;

                    case InputCategoryEnum.MeasureDivision: // Start adding to a new Voice in the next Division. (No need to add the MeasureDivision Mark to currentVoice)
                        nMeasureDivisionMarks++;
                        currentVoiceCategory = Voice.GetNextCategory(currentVoiceCategory); // Start adding to the next division. 
                        currentVoice = Voice.Create(nextVoiceNumber++, expectedDuration, inputInterpretations, currentVoiceCategory);   // Start adding to a new voice 
                        AddVoice(currentVoice);
                        break;

                    default: // Add to the current voice
                        nOtherSymbols++;
                        currentVoice.Add(inputInterpretation);
                        break;
                }

            }
            int n0 = Div0Voices.Count;
            int n1 = Div1Voices.Count;
            int n2 = Div2Voices.Count;
            int nFull = FullVoices.Count;
            int nAll = allVoices.Count;
            if (n0 + n1 + n2 +nFull != nAll)
            {
                Logger.LogCF(string.Format("n0={0} n1={1} n2={2} Full={3} All={4}", n0, n1, n2, nFull, nAll));
            }

            if ((nInAccordPartMeasureMarks == 0) && (nMeasureDivisionMarks == 0) && (nInAccordFullMeasureMarks == 0)) return; // Simple measure without InAccord. AllVoices already contains the rigth contents !
            if ((nInAccordPartMeasureMarks == 0) && (nMeasureDivisionMarks == 0) && (nInAccordFullMeasureMarks >  0)) return; // No InAccordPartMeasure, but 1 or more InAccordFullMeasure. AllVoices already contains the rigth contents ! 

            if ((nInAccordPartMeasureMarks >= 0) && (nMeasureDivisionMarks == 1) && (nInAccordFullMeasureMarks == 0)) // The simple InAccordPartMeasure case 
            {
                if ((n0 == 1) && (n1 > 0) && (nFull == 0) && (AllVoices[0].VoiceCategory == Voice.VoiceCategoryEnum.Division0))
                {
                    // Single voice first, multiple voices last
                    On1N();
                    return;
                }
            }

            if ((nInAccordPartMeasureMarks > 0) && (nMeasureDivisionMarks >  1) && (nInAccordFullMeasureMarks == 0)) // More than 1 MeasureDivisionMarks and no InAccordFullMeasureMarks
            {        
                if ((n0 == 1) && (n1 > 1) && (n2 == 1))
                {     
                    //  Multiple voices  in the middle, single voices at the ends.            
                    On1N1();
                    return;
                }
                if ((n0 > 1) && (n1 == 1) && (n2 == n0))
                {
                    // Multiple voices at the ends, single voice in the middle.
                    OnN1N();
                    return;
                }

                // We do not know how to handle this subcase!
                LogAndWarn(String.Format("More than one MeasureDivisionMark. n0={0} n1={1} n2={2}", n0, n1, n2));
                return; // We do not know how to handle this !
            }

            // We did not find a known and supported solution. Report:

            if ((nInAccordPartMeasureMarks > 0) && (nMeasureDivisionMarks == 0))
            {
                LogAndWarn("InAccordPartMeasure(s) without MeasureDivisionMarks");
                return; // We do not know how to handle this !
            }


            if ((nMeasureDivisionMarks == 1) && (nInAccordFullMeasureMarks == 1) && (nInAccordPartMeasureMarks == 1))
            {
                // We do not know how to handle this case!
                LogAndWarn(String.Format("One MeasureDivisionMark, One InAccordPartMeasure,  One InAccordFullMeasure"));
                return;
            }

            // A Last chance reporting  unexpected cases:
            string s = this.ToCategoryString();
            Logger.LogCF(string.Format(": Categories={0}", s));       
            LogAndWarn(nMeasureDivisionMarks, nInAccordPartMeasureMarks, nInAccordFullMeasureMarks); // Primarily during debugging


        }
#else
                    // Now follows experimental code, which does not work
                    // It attempts to handle more cases, but fails.
                    //In  Four Piano Blues #1 LeoSmith measure 7 the same measure contains fullmeasureInAccors, PartMeasureInAssoed and MEasuredivision.

        private VoiceList(List<InputInterpretation> inputInterpretations, int expectedDuration)
        {
            int nextVoiceNumber = 0;
            Voice currentVoice = Voice.Create(nextVoiceNumber++, expectedDuration, inputInterpretations);
            List<Voice> leftVoices = new List<Voice>(); // Left of the Measuredivision mark
            List<Voice> rightVoices = new List<Voice>(); // Right of the Measuredivision mark
            List<Voice> currentVoiceList = leftVoices; // Start by adding to leftVoices
            //InputInterpretation currentMeasureDivision = null; 
            currentVoiceList.Add(currentVoice);
            //foreach (InputInterpretation inputInterpretation in inputInterpretations)
            for (int index = 0; index < inputInterpretations.Count; index++)
            {
                InputInterpretation inputInterpretation = inputInterpretations[index];
                switch (inputInterpretation.Category)
                {
                    case InputCategoryEnum.InAccordPartMeasure: // Note: this is not correct code. MArk the voice as "ContainsInAccordPartMeasure";
                        currentVoiceList.Add(currentVoice);
                        // Start adding to a new voice                                      
                        currentVoice = Voice.Create(nextVoiceNumber++, expectedDuration, inputInterpretations);              
                        break;
                    case InputCategoryEnum.InAccordFullMeasure:
                        currentVoiceList.Add(currentVoice);
                        // Start adding to a new voice
                        currentVoice = Voice.Create(nextVoiceNumber++, expectedDuration, inputInterpretations);
                        break;
                    case InputCategoryEnum.MeasureDivision:
                        // Add to the current voice
                        currentVoice.Add(inputInterpretation);
                        currentVoiceList = rightVoices; // From now on add to rightVoices
                        break;
                    default:
                        // Add to the current voice
                        currentVoice.Add(inputInterpretation);
                        break;
                }
            }

            // Now the measure has been devided into 2 voicelists, divided by the measuredivision mark
            int nLeft = leftVoices.Count;
            int nRight = rightVoices.Count;
            if (0 == nRight)
            {
                // This is the simple case with no MeasureDivisionMark.
                allVoices = leftVoices;
                return;
            }

            if (1 == nLeft)
            {
                // One voice left to the mark, one or more right to it
                foreach (Voice rightVoice in rightVoices)
                {
                    Voice theLeftVoice = leftVoices[0];
                    List<InputInterpretation> cloneOfLeftVoice = new List<InputInterpretation>();
                    foreach (InputInterpretation item in theLeftVoice.InputInterpretations)
                    {
                        cloneOfLeftVoice.Add(item.CloneOfTimingValues());
                    }
                    rightVoice.Prepend(cloneOfLeftVoice);
                    allVoices.Add(rightVoice); // Actually they should be inserted in front !!               
                }
                return;
            }

            if (1 == nRight)
            {
                // More than one voice left to the Mark, one after
                foreach (Voice leftVoice in leftVoices)
                {
                    Voice theRightVoice = rightVoices[0];
                    List<InputInterpretation> cloneOfRightVoice = new List<InputInterpretation>();
                    foreach (InputInterpretation item in theRightVoice.InputInterpretations)
                    {
                        cloneOfRightVoice.Add(item.CloneOfTimingValues());
                    }
                    leftVoice.Append(cloneOfRightVoice);
                    allVoices.Add(leftVoice); // Actually they should be inserted in front !!               
                }
                return;
            }

            // Strange case
            Logger.LogCF(string.Format(": nLeft={0} nRight={1}", nLeft, nRight));
        }

#endif



        public void Log(string preAmble)
        {
            for (int i = 0; (i < NumberOfVoices); i++)
            {
                Voice voice = allVoices[i];
                Logger.LogCF(string.Format(": {0} AllVoices[{1}]= ( {2} )",preAmble, voice.VoiceNumber, voice.ToShortDebugString()));
            }
        }



        public string ToShortDebugString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Voice voice in this.allVoices)
            {
                sb.Append(string.Format("( {0} )", voice.ToShortDebugString()));
            }
            return sb.ToString();
        }
        
        public static VoiceList Create(List<InputInterpretation> InputInterpretations, int expectedDuration)
        {
            return new VoiceList(InputInterpretations,expectedDuration);
        }

    }
}
