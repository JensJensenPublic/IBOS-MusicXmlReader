using System.Xml;



namespace MusicXmlReaderModel
{
    public class PageMarginsElement : Element
    {

        // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#CT-MusicXML-page-margins.htm

        private PageMarginsElement()
        { }

        private FloatParameter leftMargin;
        private FloatParameter rightMargin;
        private FloatParameter topMargin;
        private FloatParameter bottomMargin;
        private MarginTypeParameter marginType;

        private PageMarginsElement(XmlNode node)
        {


            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {

                switch (a.Name)
                {
                    case "type": marginType = MarginTypeParameter.Create(a.Value); break;
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
                    case "left-margin": leftMargin = FloatParameter.Parse(n.InnerXml,  float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "right-margin": rightMargin = FloatParameter.Parse(n.InnerXml, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "top-margin": topMargin = FloatParameter.Parse(n.InnerXml, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "bottom-margin": bottomMargin = FloatParameter.Parse(n.InnerXml, float.MinValue, float.MaxValue, "ErrorString"); break;
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }
        }

        public string ToDebugString()
        {
            return string.Format("{0}{1}{2}{3}{4}",
                Parameter.ToDebugString("L=", leftMargin), // 1
                Parameter.ToDebugString("R=", rightMargin), // 2
                Parameter.ToDebugString("T=", topMargin), // 3
                Parameter.ToDebugString("L=", bottomMargin), // 4
                Parameter.ToDebugString("Type=", marginType)); // 5
        }


        public static PageMarginsElement Create(XmlNode node)
        {
            return new PageMarginsElement(node);
        }
    }
}


