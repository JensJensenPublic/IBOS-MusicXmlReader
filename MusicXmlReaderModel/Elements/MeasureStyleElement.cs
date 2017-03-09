using System.Xml;
using System.Text;

// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-measure-style.htm

namespace MusicXmlReaderModel
{
    class MeasureStyleElement : EventElement
    {
        const string className = "MeasureStyleElement";
        private int staffNumber = 1;
        private MeasureRepeatElement measureRepeatElement;
        private BeatRepeatElement beatRepeatElement;
        private MultipleRestElement multipleRestElement;
        private SlashElement slashElement;

        private MeasureStyleElement(XmlNode node)
        {
            const string functionName = "MeasureStyleElement";
  
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)                {
   
                    case "slashes": Utilities.Parse(a.Value, ref staffNumber, 1, int.MaxValue, functionName, false); break;
                    case "font-family":
                    case "font-style":
                    case "font-size":
                    case "font-weight":
                    case "color": break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unexpected attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value));
                        break;
                }
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "measure-repeat": measureRepeatElement = MeasureRepeatElement.Create(child); break;
                    case "beat-repeat":    beatRepeatElement = BeatRepeatElement.Create(child); break;
                    case "multiple-rest":  multipleRestElement = MultipleRestElement.Create(child); break;
                    case "slash":          slashElement = SlashElement.Create(child);break; 
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unexpected element. Name={2}", className, functionName, child.Name));
                        break;
                }
            }
        }

        public override string ToString()
        {
            // TO DO: Find out what the mucisians really want here !
            StringBuilder sb = new StringBuilder();
            if (null != measureRepeatElement) sb.Append(measureRepeatElement.ToString());
            if (null != beatRepeatElement) sb.Append(beatRepeatElement.ToString());
            if (null != multipleRestElement) sb.Append(multipleRestElement.ToString());
            if (null != slashElement) sb.Append(slashElement.ToString());
            return sb.ToString(); 
        }


        public int StaffNumber
        {
            get
            {
                return staffNumber;
            } 
        }

        public static MeasureStyleElement Create(XmlNode node)
        {
            return new MeasureStyleElement(node);
        }
    }
}
