using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using JSJ.MusicSynthesis;
using MusicXmlReaderUI;


namespace MusicXmlReaderModel
{

    public class HarmonyElement : EventElement
    {
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
                }
            }

            // Fill in derived values
            chromaticStep = MidiNote.GetChromaticStep(rootStep, rootAlter);
            chordType = MidiChord.GetChordType(kind);
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
    }
}

