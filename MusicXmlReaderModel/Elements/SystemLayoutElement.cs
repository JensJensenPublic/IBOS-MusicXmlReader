using System.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class SystemLayoutElement : Element
    {

        // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#CT-MusicXML-system-layout.htm

        private SystemLayoutElement()
        { }

        private float systemDistance = 0.0F;
        private float systemTopDistance = 0.0F;
        private SystemMarginsElement systemMargins;
        private SystemDevidersElement systemDeviders;

        private SystemLayoutElement(XmlNode node)
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
                    case "system-distance":  Utilities.Parse(n.InnerXml, ref systemDistance, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "top-system-distance": Utilities.Parse(n.InnerXml, ref systemTopDistance, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "system-margins": systemMargins = SystemMarginsElement.Create(n); break;
                    case "system-dividers": systemDeviders = SystemDevidersElement.Create(n); break;
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }
        }

        public string ToDebugString()
        {
            string systemMargins = (null != this.systemMargins) ? this.systemMargins.ToDebugString() : "";
            return string.Format("SystemLayout(Distance={0} TopDistance={1} SystemMargins={2}) ", systemDistance, systemTopDistance, systemMargins);
        }


        public static SystemLayoutElement Create(XmlNode node)
        {
            return new SystemLayoutElement(node);
        }
    }
}
