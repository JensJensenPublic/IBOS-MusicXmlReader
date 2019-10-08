using System.Xml;

namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#CT-MusicXML-staff-layout.htm

    public class StaffLayoutElement : Element
    {

        private int staffnumber = 1; // "1" as specified in document above !
        private float staffDistance = 0;

        private StaffLayoutElement()
        { }



        private StaffLayoutElement(XmlNode node)
        {


            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
#warning TODO Fix Errorstring
                    case "number" : Utilities.Parse(a.InnerXml, ref staffnumber, 1, int.MaxValue, "ErrorString", false); break;
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
                    case "staff-distance": Utilities.Parse(n.InnerXml, ref staffDistance, float.MinValue, float.MaxValue, "ErrorString"); break;
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }

        }

        public string ToDebugString()
        {
            return string.Format("StaffLayout(Number={0} Distance={1})", this.staffnumber, this.staffDistance);
        }

        public static StaffLayoutElement Create(XmlNode node)
        {
            return new StaffLayoutElement(node);
        }
    }
}

