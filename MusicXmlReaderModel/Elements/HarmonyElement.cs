using System.Xml;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-harmony.htm


    public class HarmonyElement : EventElement
    {
        string className = "HarmonyElement";
        // Variables read directly fromthe MusucXml file
        string kind;
        string rootStep;
        string rootAlter;

        // Derived variables
        ChromaticStep chromaticStep;
        ChordType chordType;
        string localizedChordType;

        public string Kind
        {
            get
            {
                return kind;
            }
        }

        public string RootStep
        {
            get
            {
                return rootStep;
            }
        }

        public string RootAlter
        {
            get
            {
                return rootAlter;
            }
        }

        public ChromaticStep ChromaticStep
        {
            get
            {
                return chromaticStep;
            }
        }

        public ChordType ChordType
        {
            get
            {
                return chordType;
            }
        }

        public string LocalizedChordType
        {
            get
            {
                return localizedChordType;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private HarmonyElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private HarmonyElement(XmlNode node)
        {
            string functionName = "HarmonyElement";
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "root":
                        // Dig out elements from the childNode
                        foreach (XmlNode nn in n.ChildNodes)
                        {
                            switch (nn.Name)
                            {
                                case "root-step": rootStep = nn.InnerText; break;
                                case "root-alter": rootAlter = nn.InnerText; break;
                            }
                        }
                        break;
                    case "kind": kind = n.InnerText; break;

                    case "function":
                    case "inversion":
                    case "bass":
                    case "degree":
                    case "frame":
                    case "offset":
                    case "footnote":
                    case "level":
                    case "staff":
                        Logger.LogOnce(string.Format("{0}.{1} found unimplemented harmony element. Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText)); break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} found unknown harmony element. Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText)); break;
                }
            }

            // Fill in derived values
            chromaticStep = MidiNote.GetChromaticStep(rootStep, rootAlter);
            chordType = MidiChord.GetChordType(kind);
            if (ChordType.UnImplemented == chordType)
            {
                Logger.LogOnce(string.Format("{0}.{1} found unimplemented harmony kind={2}", className, functionName, kind));
            }
            if (ChordType.Unknown == chordType)
            {
                Logger.LogOnce(string.Format("{0}.{1} found unknown harmony kind={2}", className, functionName, kind));
            }
            localizedChordType = MidiChord.LocalizeChordType(chordType);
        }

        public static HarmonyElement Create(XmlNode node)
        {
            return new HarmonyElement(node);
        }

        public override string ToString() // LOCALIZE
        {
            return string.Format("{0}: Akkord: {1} {2} {3}",
                startTime,
                string.IsNullOrEmpty(kind) ? "" : kind,
                string.IsNullOrEmpty(rootStep) ? "" : rootStep,
                string.IsNullOrEmpty(rootAlter) ? "" : rootAlter);
        }
        
        public string ToLocalizedString()
        {
            string delimiter = ""; // (string.IsNullOrEmpty(localizedChordType)) ? "" : "-"; // Only show delimiter if needed
            string s = string.Format("{0}:{1}{2}{3}", ResourcesForModel.HarmonyElement_Chord, chromaticStep, delimiter, localizedChordType);
            return s;
        }

    }

}

