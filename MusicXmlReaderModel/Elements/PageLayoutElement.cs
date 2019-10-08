using System.Xml;

namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#CT-MusicXML-page-layout.htm

    public class PageLayoutElement : Element
    {
        private FloatParameter pageHeight;
        private FloatParameter pageWidth;
        private PageMarginsElement pageMargins;

        private PageLayoutElement()
        { }

        private PageLayoutElement(XmlNode node)
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
                    case "page-height":  pageHeight = FloatParameter.Parse(n.InnerXml, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "page-width":   pageWidth = FloatParameter.Parse(n.InnerXml, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "page-margins": pageMargins = PageMarginsElement.Create(n); break;
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }
        }

        public string ToDebugString()
        {
            string result = string.Format("PageLayout({0}{1}{2})",
                Parameter.ToDebugString("Height=", pageHeight), // 0
                Parameter.ToDebugString("Width=", pageWidth), // 1
                (null != pageMargins) ? string.Format("PageMargins({0})",pageMargins.ToDebugString()) : ""); // 2
                //"PAGE-MARGINS");
            return result;
        }


        public static PageLayoutElement Create(XmlNode node)
        {
            return new PageLayoutElement(node);
        }
    }
}

