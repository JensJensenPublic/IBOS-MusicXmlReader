using System.Xml;

namespace MusicXmlReaderModel
{
    class AppearanceElement : Element
    {

        // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-appearance.htm

        private AppearanceElement()
        { }

        private FloatParameter lineWidth;
        private FloatParameter distance;
        private FloatParameter noteSize;
        private StringParameter otherAppearance; // These parameters are NOT interchancable among different programs !!


        private AppearanceElement(XmlNode node)
        {
            foreach (XmlAttribute a in node.Attributes)
            {
                LogFormatter.Log(unknown, a);
            }

            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "line-width": lineWidth = FloatParameter.Parse(n.InnerText, 0, float.MaxValue, ""); break;
                    case "distance":   distance = FloatParameter.Parse(n.InnerText, 0, float.MaxValue, ""); break;
                    case "note-size":  noteSize = FloatParameter.Parse(n.InnerText, 0, float.MaxValue, ""); break;
                    case "other-appearance": otherAppearance = StringParameter.Create(n.InnerText); break;  
                    default:
                        LogFormatter.Log(once | unknown, n); break;
                }
            }
        }

        public string ToDebugString()
        {
            string result = string.Format("Appearance({0}{1}{2}{3})",
                Parameter.ToDebugString("LineWidth=",lineWidth), // 0
                Parameter.ToDebugString("Distance=",distance), // 1
                Parameter.ToDebugString("NoteSize=",noteSize), // 2
                Parameter.ToDebugString("OtherAppearance=",otherAppearance)); // 3 
            return result;
        }


        static public AppearanceElement Create(XmlNode node)
        {
            return new AppearanceElement(node);
        }

    }
}
