using System.Xml;



namespace MusicXmlReaderModel
{
    public class ScalingElement : Element
    {

        // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-scaling.htm

        private ScalingElement()
        { }

        private float millimeters = 0.0F;
        private float tenths = 0.0F;

        private ScalingElement(XmlNode node)
        {


            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    default:
                        LogFormatter.Log(LogFormatter.LogOptions.Once, a);
                        break;
                }
            }



            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
#warning TODO Fix Errorstring
                    case "millimeters": Utilities.Parse(n.InnerXml, ref millimeters, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "tenths": Utilities.Parse(n.InnerXml, ref tenths, float.MinValue, float.MaxValue, "ErrorString"); break;
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }
        }

        public string ToDebugString()
        {
            return "";
           // return string.Format("Margins(Left={0}, Right={1})", leftMargin, rightMargin);
        }

        public static ScalingElement Create(XmlNode node)
        {
            return new ScalingElement(node);
        }
    }
}


