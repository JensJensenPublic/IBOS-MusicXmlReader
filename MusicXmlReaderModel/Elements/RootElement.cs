using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Class for handling HarmonyElement.root
    /// </summary>
    class RootElement : PitchElementBase
    {
        private string rootStep;
        private string rootAlter;

        /// <summary>
        /// The string representation
        /// </summary>
        public string RootStep
        {
            get
            {
                return rootStep;
            }
        }

        /// <summary>
        /// The string representation
        /// </summary>
        public string RootAlter
        {
            get
            {
                return rootAlter;
            }
        }

        RootElement(XmlNode node) // : base(node, stepName)
        {
            // Here we do not parse the "octave" and we only allow alter in [-1 ..+1]
            rootStep = Utilities.GetChildValue(node, "root-step");
            step = GetFullStep(rootStep); // Special parsing of step

            rootAlter = Utilities.GetChildValue(node, "root-alter");
            Utilities.Parse(rootAlter, ref alter, -1, +1, "RootElement: alter", true);
        }

        public static RootElement Create(XmlNode node)
        {
            return new RootElement(node);
        }


    }
}

