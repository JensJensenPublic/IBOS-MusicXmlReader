using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    //https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-beat-repeat.htm

    class BeatRepeatElement
    {
        const string className = "BeatRepeatElement";
        bool start = false;
        int slashes = 1;
        bool useDots = false;

        private BeatRepeatElement(XmlNode node)
        {

            const string functionName = "BeatRepeatElement";


            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":        Utilities.ParseStartStopAttributeValue(functionName, a.Name, a.Value, ref start); break;
                    case "slashes":     Utilities.Parse(a.Value, ref slashes, 1, int.MaxValue, functionName, false); break;
                    case "use-dots":    Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref useDots); break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unexpected attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value));
                        break;
                }
            }

        }

        public int Slashes
        {
            get
            {
                return slashes;
            }
        }

        public bool Start
        {
            get
            {
                return start;
            }
        }

        public bool UseDots
        {
            get
            {
                return useDots;
            }
        }

        public override string ToString()
        {
            return ResourcesForModel.BeatRepeatElement_Name; 
        }

        public static BeatRepeatElement Create(XmlNode node)
        {
            return new BeatRepeatElement(node);
        }


    }
}
