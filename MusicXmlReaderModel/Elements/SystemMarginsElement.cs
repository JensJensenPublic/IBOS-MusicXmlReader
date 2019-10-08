using System.Xml;



namespace MusicXmlReaderModel
{
    public class SystemMarginsElement : Element
    {

        // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#CT-MusicXML-system-margins.htm

        private SystemMarginsElement()
        { }

        private float leftMargin = 0.0F;
        private float rightMargin = 0.0F;

        private SystemMarginsElement(XmlNode node)
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
                    case "left-margin": Utilities.Parse(n.InnerXml, ref leftMargin, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "right-margin": Utilities.Parse(n.InnerXml, ref leftMargin, float.MinValue, float.MaxValue, "ErrorString"); break;
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }
        }

        public string ToDebugString()
        {
            return string.Format("Margins(Left={0}, Right={1})",leftMargin,rightMargin);
        }

        public static SystemMarginsElement Create(XmlNode node)
        {
            return new SystemMarginsElement(node);
        }
    }
}

