using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-unpitched.htm

    public class UnpitchedElement : PitchElementBase
    {
        string className = "UnpitchedElement";
        FullStepEnum displayStep;
        int displayOctave;

        public FullStepEnum DisplayStep
        {
            get
            {
                return displayStep;
            }
        }

        public int DisplayOctave
        {
            get
            {
                return displayOctave;
            }
        }



        private UnpitchedElement()
        {
        }

        private UnpitchedElement(XmlNode node) // : base(node, stepName)
        {
            string functionName = "UnpitchedElement";
            string displayStepString = Utilities.GetChildValue(node, "display-step");
            displayStep = GetFullStep(displayStepString); // Special parsing of step
            string displayOctaveString = Utilities.GetChildValue(node, "display-octave");
            Utilities.Parse(displayOctaveString, ref displayOctave, 0, 10, "", false);
            Logger.LogOnce(string.Format("{0}.{1} found Display={2}{3}", className, functionName, displayStep, displayOctave));
        }

        public static UnpitchedElement Create(XmlNode node)
        {
            return new UnpitchedElement(node);
        }
        
    }
}
