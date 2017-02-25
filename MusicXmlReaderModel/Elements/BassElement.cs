using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Class for handling HarmonyElement.bass 
    /// </summary>
    class BassElement : PitchElementBase
    {
        private string bassStep;
        private string bassAlter;

        public string BassStep
        {
            get
            {
                return bassStep;
            }
        }

        public string BassAlter
        {
            get
            {
                return bassAlter;
            }
        }

        BassElement(XmlNode node) // : base(node, stepName)
        {
            // Here we do not parse the "octave" and we only allow alter in [-1 ..+1]
            bassStep = Utilities.GetChildValue(node, "bass-step");
            step = GetFullStep(bassStep); // Special parsing of step
            bassAlter = Utilities.GetChildValue(node, "bass-alter");
            Utilities.Parse(bassAlter,ref alter, -1, +1, "BassElement: alter", true);
        }

        public static BassElement Create(XmlNode node)
        {
            return new BassElement(node);
        }

    }
}
