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

        BassElement(XmlNode node) // : base(node, stepName)
        {
            // Here we do not parse the "octave" and we only allow alter in [-1 ..+1]
            step = GetFullStep(Utilities.GetChildValue(node, "bass-step")); // Special parsing of step
            Utilities.Parse(Utilities.GetChildValue(node, "bass-alter"), ref alter, -1, +1, "BassElement: alter", true);
        }

        public static BassElement Create(XmlNode node)
        {
            return new BassElement(node);
        }

    }
}
