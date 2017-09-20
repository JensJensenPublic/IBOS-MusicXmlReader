using System.Xml;
using JSJ.MusicSynthesis;
using System.Collections.Generic;
using System.Text;

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
        // string bassStep;
        RootElement rootElement; // The "C" in "C/G"
        BassElement bassElement; // The "G" in "C/G"
        // List<DegreeElement> degreeElements = new List<DegreeElement>(); // (add11) etc
        List<MidiChordDegreeDescription> degrees = new List<MidiChordDegreeDescription>(); // (add11) etc

        // Derived variables
        ChromaticStep chromaticRootStep;
        ChromaticStep chromaticBassStep;
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

        public BassElement BassElement
        {
            get
            {
                return bassElement;
            }

        }


        public string RootAlter
        {
            get
            {
                return rootAlter;
            }
        }
        
        public ChromaticStep ChromaticRootStep
        {
            get
            {
                return chromaticRootStep;
            }
        }

        public ChromaticStep ChromaticBassStep
        {
            get
            {
                return chromaticBassStep;
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

        public List<MidiChordDegreeDescription> Degrees
        {
            get
            {
                return degrees;
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
            bool implemented = true;
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                implemented = true;
                switch (n.Name)
                {
                    case "root": 
                        rootElement = RootElement.Create(n);
                        rootStep = rootElement.RootStep;
                        rootAlter = rootElement.RootAlter;
                        break;

                    case "kind": kind = n.InnerText; break;

                    case "staff":
                        // Logger.LogOnce(string.Format("{0}.{1} Element '{2}' explicitly ignored", className, functionName, n.Name));
                        break;

                    case "degree":
                        DegreeElement degreeElement = DegreeElement.Create(n); // Handles the logging of unimplemented values                                                                           
                        degrees.Add(MidiChordDegreeDescription.Create(degreeElement.DegreeValue,degreeElement.DegreeType,degreeElement.DegreeAlterInteger));
                        break; 

                    case "bass": //bassStep = n.InnerText; // Avoid repeating log for each different InnerTxt (bass note) TODO: Decode step and alter in a way similar to PitchElement
                        bassElement = BassElement.Create(n);
                        //Logger.LogOnce(string.Format("{0}.{1} Harmony element '{2}' is decoded to step={3} alter={4} But the value not used yet", className, functionName, n.Name, bassElement.Step, bassElement.Alter));
                        //Logger.LogOnce(string.Format("{0}.{1} Harmony element '{2}' is decoded  But the value not used yet", className, functionName, n.Name));
                        break;

                    case "function": implemented = false; break;
                    case "inversion": implemented = false; break;
                    case "frame": implemented = false; break;              
                    case "footnote": implemented = false; break;
                    case "level": implemented = false; break;

                    case "offset":
                        if (0 != n.Attributes.Count)
                        {
                            Logger.LogOnce(string.Format("{0}.{1} found offset element with unimplemented attributes.", className, functionName));
                            implemented = false;
                        }
                        break;

                    default:
                        Logger.LogOnce(string.Format("{0}.{1} found unknown harmony element. Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText)); break;
                }
                if (!implemented)
                {
                    Logger.LogOnce(string.Format("{0}.{1} found unimplemented harmony element. Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText));
                }
            }

            if ((null == rootElement) || (string.IsNullOrEmpty(rootElement.RootStep)))
            {
                // This seems to be a possible scenario when building chords from degrees only !
                // Do not know how to handle this in detail.
                Logger.LogOnce(string.Format("{0}.{1} found HarmonyElement with no RootElement", className, functionName));
            }

            if (string.IsNullOrEmpty(kind))
            {
                // This seems to be a possible scenario when building chords from degrees only !
                // Do not know how to handle this in detail.
                Logger.LogOnce(string.Format("{0}.{1} found HarmonyElement with no Kind", className, functionName));
            }

            // Fill in derived values
            chromaticRootStep = MidiNote.GetChromaticStep(rootStep, rootAlter);
            chromaticBassStep = chromaticRootStep; // Default to use the root as 
            if (null != bassElement)
            {
                // The harmony contains an explicit bass note
                chromaticBassStep = MidiNote.GetChromaticStep(bassElement.BassStep, bassElement.BassAlter);
            }

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

        private string ToString(List<MidiChordDegreeDescription> degrees)
        {
            StringBuilder sb = new StringBuilder();
            foreach (MidiChordDegreeDescription degree in degrees)
            {
                sb.Append(degree.ToLocalizedString());
            }
            return sb.ToString();
        }
        
        public string ToLocalizedString()
        {
            string delimiter = ""; // (string.IsNullOrEmpty(localizedChordType)) ? "" : "-"; // Only show delimiter if needed
            string bassTone = (null != bassElement) ? string.Format("/{0}",bassElement.ToString()) : ""; // Only show bassTone if needed
            string degreeString = ((null != degrees) && (0 != degrees.Count)) ? ToString(degrees) : ""; // Only show degrees if needed  
            string s = string.Format("{0}{1}{2}{3} {4}", chromaticRootStep, delimiter, localizedChordType, bassTone, degreeString);
            return s;
        }
    }
}

