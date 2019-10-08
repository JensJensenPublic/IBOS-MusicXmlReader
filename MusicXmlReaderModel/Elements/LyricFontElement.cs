using System.Xml;


namespace MusicXmlReaderModel
{
    class LyricFontElement : Element
    {
        private IntParameter number;
        private StringParameter name;
        private StringParameter fontFamily;
        private FloatParameter fontSize;
        private FontStyleParameter fontStyle;
        private FontWeightParameter fontWeight;        


        private LyricFontElement()
        { }

        private LyricFontElement(XmlNode node)
        {
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "number": number = IntParameter.Parse(a.Value, 0, int.MaxValue, true); break;
                    case "name":    name = StringParameter.Create(a.Value); break;
                    case "font-family": fontFamily = StringParameter.Create(a.Value); break;
                    case "font-style":  fontStyle = FontStyleParameter.Create(a.Value); break;
                    case "font-size":  fontSize = FloatParameter.Parse(a.Value, 0, float.MaxValue, ""); break;
                    case "font-weight":  fontWeight = FontWeightParameter.Create(a.Value); break;
                    default:
                        LogFormatter.Log(once | unknown, a); break;
                }

            }

            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    default:
                        LogFormatter.Log(once | unknown, n); break;
                }
            }
        }


        public string ToDebugString()
        {
            string result = string.Format("LyricFont({0}{1}{2}{3}{4}{5})",
                Parameter.ToDebugString("Number=",number), // 0
                Parameter.ToDebugString("Name=", name ), // 1
                Parameter.ToDebugString("", fontFamily,true ), // 2 true mans "Show in quotes"
                Parameter.ToDebugString("Style=", fontStyle), // 3 
                Parameter.ToDebugString("Size=", fontSize), // 4 
                Parameter.ToDebugString( "Weight=", fontWeight)); // 5 
            return result;
        }


        public static LyricFontElement Create(XmlNode node)
        {
            return new LyricFontElement(node);
        }



    }
}
