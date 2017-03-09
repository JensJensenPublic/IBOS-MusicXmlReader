using System.Xml;


// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-measure-repeat.htm

namespace MusicXmlReaderModel
{
    class MeasureRepeatElement : EventElement
    {
        const string className = "MeasureRepeatElement";

        private int slashes = 1;    // 1 is default
        private bool start ;        // The attribute is required

        private MeasureRepeatElement(XmlNode node)
        {

            const string functionName = "MeasureRepeatElement";
  

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":    Utilities.ParseStartStopAttributeValue(functionName, a.Name, a.Value, ref start); break;
                    case "slashes": Utilities.Parse(a.Value,ref slashes,1,int.MaxValue,functionName,false); break;               
                    default: Logger.LogOnce(string.Format("{0}.{1} Unexpected attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value));
                            break;
                }
            }
            // Logger.LogOnce(string.Format("{0}.{1} Type={2} Slashes={3}", className, functionName, start ? "start" : "stop " ,slashes)); // Until wn know how to handle it                      
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

        public override string ToString()
        {
            string startString = ResourcesForModel.StartStopContinueElement_Start;
            string stopString = ResourcesForModel.StartStopContinueElement_Stop;
            return " " + ResourcesForModel.MeasureRepeatElement_Name + ":" + (start ? startString : stopString); 
        }

        public static MeasureRepeatElement Create(XmlNode node)
        {
            return new MeasureRepeatElement(node);
        }
    }
}
