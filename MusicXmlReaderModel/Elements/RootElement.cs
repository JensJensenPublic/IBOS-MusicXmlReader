using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    class RootElement : PitchElement
    {
        RootElement(XmlNode node) // : base(node, stepName)
        {
            // Here we do not parse the "octave" and we only allow alter in [-1 ..+1]
            step = GetFullStep(Utilities.GetChildValue(node, "root-step")); // Special parsing of step
            Utilities.Parse(Utilities.GetChildValue(node, "root-alter"), ref alter, -1, +1, "RootElement: alter", true);
        }

        public static new RootElement Create(XmlNode node)
        {
            return new RootElement(node);
        }


    }
}

