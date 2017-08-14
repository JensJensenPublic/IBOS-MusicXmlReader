using System.Xml;
using System.Collections.Generic;

namespace MusicXmlReaderModel
{
    public class ScorePartElement : Element
    {

        public string partId = ""; // For instance "P1"
        public string partName = ""; // For instance "Soprano"
        public int partNumber; // A unique artificial index  for this part.
        private ScoreInstrumentElement scoreInstrumentElement;
        private MidiInstrumentElement midiInstrumentElement;
        private TransposeElement transposeElement;
        private bool hasNotes; // Used by  MusicBraille to show an octave mark with first note in each part

        private List<ScoreInstrumentElement> scoreInstruments = new List<ScoreInstrumentElement>() ;
        private List<MidiInstrumentElement> midiInstruments = new List<MidiInstrumentElement>();

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
            ScoreInstrumentElement result = null;
            foreach (ScoreInstrumentElement scoreInstrumentElement in scoreInstruments)
            {
                if (name == scoreInstrumentElement.Id)
                {
                    return scoreInstrumentElement;
                }

            }

            return result; 
        }

        public MidiInstrumentElement GetMidiInstrument(string name)
        {
            MidiInstrumentElement result = null;
            foreach (MidiInstrumentElement midiInstrumentElement in midiInstruments)
            {
                if (name == midiInstrumentElement.Id)
                {
                    return midiInstrumentElement;
                }

            }

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

            //this.partNumber = partNumber;
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        partId = a.Value;                       
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
                        Logger.Log(string.Format(scoreInstrumentElement.ToString())); // Not of interest for the normal user !
                        scoreInstruments.Add(scoreInstrumentElement);
                        break;                
                    case "midi-instrument":
                        midiInstrumentElement = MidiInstrumentElement.Create(n);
                        Logger.Log(string.Format(midiInstrumentElement.ToString())); // Not of interest for the normal user !
                        midiInstruments.Add(midiInstrumentElement);
                        break;        
                }
            }

            if (null == midiInstrumentElement)
            {
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
    }
}
