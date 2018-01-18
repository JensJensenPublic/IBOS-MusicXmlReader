using System.Xml;
using System.Collections.Generic;

namespace MusicXmlReaderModel
{
    public class ScorePartElement : Element
    {
        bool verbose = false;
        const string className = "ScorePartElement";
        public string partId = ""; // For instance "P1"
        public string partName = ""; // For instance "Soprano"
        public int partNumber; // A unique artificial index  for this part.
        private ScoreInstrumentElement scoreInstrumentElement;
        private MidiInstrumentElement midiInstrumentElement;
        private MidiDeviceElement midiDeviceElement;
        private TransposeElement transposeElement;
        private bool hasNotes; // Used by  MusicBraille to show an octave mark with first note in each part

        private string identification;
        private string partNameDispley;
        private string partAbbreviation;
        private string partAbbreviationDisplay;

        private List<ScoreInstrumentElement> scoreInstruments = new List<ScoreInstrumentElement>() ;
        private List<MidiInstrumentElement> midiInstruments = new List<MidiInstrumentElement>();

        public ScoreInstrumentElement ScoreInstrumentElement
        {
            get
            {
                return scoreInstrumentElement;
            }
        }

        public MidiInstrumentElement MidiInstrumentElement
        {
            get
            {
                return midiInstrumentElement;
            }
        }

        // Only needed during debuggine
        public List<ScoreInstrumentElement> ScoreInstruments
        {
            get
            {
                return scoreInstruments;
            }
        }

        // Only needed during debugging
        public List<MidiInstrumentElement> MidiInstruments
        {
            get
            {
                return midiInstruments;
            }

        }

        public ScoreInstrumentElement GetScoreInstrument(string name)
        {
            string functionName = "GetScoreInstrument";
            ScoreInstrumentElement result = null;
            foreach (ScoreInstrumentElement scoreInstrumentElement in scoreInstruments)
            {
                if (name == scoreInstrumentElement.Id)
                {
                    return scoreInstrumentElement;
                }

            }
            Logger.LogOnce(string.Format("{0}.{1}: ScoreInstrument with Name={2} not found", className, functionName, name));
            return result; 
        }

        public MidiInstrumentElement GetMidiInstrument(string name)
        {
            string functionName = "GetMidiInstrument";
            MidiInstrumentElement result = null;
            foreach (MidiInstrumentElement midiInstrumentElement in midiInstruments)
            {
                if (name == midiInstrumentElement.Id)
                {
                    return midiInstrumentElement;
                }

            }
            Logger.LogOnce(string.Format("{0}.{1}: MidiInstrument with Name={2} not found", className, functionName, name));
            return result;     
        }



        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ScorePartElement()
        { }

        public int MidiInstrument
        {
            get
            {
                return midiInstrumentElement.MidiUnpitchedInstrumentNumber;
            }
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ScorePartElement(XmlNode node)
        {
            const string functionName = "ScorePartElement";
            //this.partNumber = partNumber;
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        partId = a.Value;
                        if (verbose) Logger.Log(string.Format("{0}.{1}: Part={2}", className, functionName, partId));
                        break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1}: Unexpected attribute: Name={2} Value={3}", className, functionName, a.Name, a.Value));
                        break;
                }
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "part-name": partName = n.InnerText; break;
                    case "score-instrument":
                        scoreInstrumentElement = ScoreInstrumentElement.Create(n);
                        if (verbose) Logger.Log(string.Format(scoreInstrumentElement.ToString())); // Not of interest for the normal user !
                        scoreInstruments.Add(scoreInstrumentElement);
                        break;                
                    case "midi-instrument":
                        midiInstrumentElement = MidiInstrumentElement.Create(n);
                        if (verbose) Logger.Log(string.Format(midiInstrumentElement.ToString())); // Not of interest for the normal user !
                        midiInstruments.Add(midiInstrumentElement);
                        break;

                    case "midi-device":
                        midiDeviceElement = MidiDeviceElement.Create(n);
                        if (verbose) Logger.Log(string.Format("{0}.{1}: midi-device found. Port={2})", className, functionName,midiDeviceElement.Port)); // Not of interest for the normal user !
                        break;

                    case "identification":
                        identification = n.InnerText;
                        break; 
                    case "part-name-display":
                        partNameDispley = n.InnerText;
                        break;
                    case "part-abbreviation":
                        partAbbreviation = n.InnerText;
                        break;
                    case "part-abbreviation-display":
                        partAbbreviationDisplay = n.InnerText;
                        break;

                    case "group":
                        Logger.Log(string.Format("{0}.{1}: Unimplemented element found: Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText));
                        break;

                    default:
                        Logger.Log(string.Format("{0}.{1}: Unexpected element found: Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText));
                        break;

                }
            }

            if ((null == midiInstrumentElement) && (null != scoreInstrumentElement) && (null != scoreInstrumentElement.VirtualInstrumentElement))
            {
                // Obtain a "Best effort" Midi instrument from the name of the virtual instrument
                int midiProgram = (int) MidiInstrumentMap.GetPitchedMidiInstrument(scoreInstrumentElement.VirtualInstrumentElement);    
                midiInstrumentElement = MidiInstrumentElement.CreateSubstituteForVirtualInstrument(scoreInstrumentElement.VirtualInstrumentElement.ToString(), midiProgram);
            }

            if (null == midiInstrumentElement)
            {
                // Last chance handler
                midiInstrumentElement = MidiInstrumentElement.CreateDefault();
            }

        }




        public static ScorePartElement Create(XmlNode node)
        {
            return new ScorePartElement(node);
        }

        //public override string ToString() // Not called, not localized
        //{
        //    return string.Format("Stemme[{0}] {1} = {2} TODO: Fill in the rest!", partNumber, partId, partName);
        //}

  
        public int MidiChannel
        {
            get
            {
                // Use midi channel 1 as default
                if (null == midiInstrumentElement)
                {
                    Logger.Log(string.Format("ScorePartElement: midiInstrumentElement is null. Using 1 as default value for MidiChannel"));
                    return 1;
                }
                return midiInstrumentElement.MidiChannel; 
            }
        }

        public int MidiProgram
        {
            get
            {
                if(null == midiInstrumentElement)
                {
                    Logger.Log(string.Format("ScorePartElement: midiInstrumentElement is null. Using 1 as default value for MidiProgram"));
                    return 1;
                }
                return midiInstrumentElement.MidiProgram; // Use midi channel 1 as default
            }
        }

        public float MidiVolume
        {
            get
            {
                if (null == midiInstrumentElement)
                {
                    Logger.Log(string.Format("ScorePartElement: midiInstrumentElement is null. Using 127 as default value for MidiVolume"));
                    return 127;
                }
                return midiInstrumentElement.MidiVolume ; // Use midi volume 127 as default
            }
        }

        public string MidiInstrumentString
        {
            get
            {
                if (null == midiInstrumentElement)
                {
                    Logger.Log(string.Format("ScorePartElement: midiInstrumentElement is null. Using empty string as default value for MidiInstrumentString"));
                    return "";
                }
                return midiInstrumentElement.ToString();
            }
        }

        public int MidiUnpitchedInstrumentNumber
        {
            get
            {
                if (null == midiInstrumentElement)
                {
                    Logger.Log(string.Format("ScorePartElement: midiInstrumentElement is null. Using 0 as default value for MidiInstrumentString"));
                    return 0;
                }
                return midiInstrumentElement.MidiUnpitchedInstrumentNumber;
            }
        }

        public string ScoreInstrumentString
        {
            get
            {
                if (null == scoreInstrumentElement)
                {
                    Logger.Log(string.Format("ScorePartElement: scoreInstrumentElement is null. Using empty string as default value for ScoreInstrumentString"));
                    return "";
                }
                return scoreInstrumentElement.ToString();
            }
        }

  
        internal TransposeElement TransposeElement
        {
            get
            {
                return transposeElement;
            }

            set
            {
                transposeElement = value;
            }
        }

        public bool HasNotes
        {
            get
            {
                return hasNotes;
            }

            set
            {
                hasNotes = value;
            }
        }

        public string Identification
        {
            get
            {
                return identification;
            }
        }

        public string PartNameDispley
        {
            get
            {
                return partNameDispley;
            }
        }

        public string PartAbbreviation
        {
            get
            {
                return partAbbreviation;
            }
        }


        public string PartAbbreviationDisplay
        {
            get
            {
                return partAbbreviationDisplay;
            }
        }
        

    }
}
