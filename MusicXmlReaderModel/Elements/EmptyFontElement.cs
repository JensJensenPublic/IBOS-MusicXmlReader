using System.Xml;

namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-empty-font.htm

    public class EmptyFontElement : Element
    {
        private FontStyleEnum fontStyle = FontStyleEnum.unknown;
        public FontStyleEnum FontStyle { get { return fontStyle; } }
        private FontWeightEnum fontWeight = FontWeightEnum.unknown;
        public FontWeightEnum FontWeight { get { return fontWeight; } }
        private string fontFamily;
        private float fontSize;

#warning  TODO Also used in MeasureNumberingElement ! Refactor !!

        public enum FontStyleEnum { unknown, normal, italic };
        public enum FontWeightEnum { unknown, normal, bold };
        enum HorizontalAlignEnum { unknown, left, center, right }
        enum VerticalAlignEnum { unknown, top, middle, bottom, baseline }

        public string ToDebugString()
        {
            return "";
        }



        private EmptyFontElement()
        { }



        private EmptyFontElement(XmlNode node)
        {

            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
#warning TODO Fix Errorstring

                    case "font-family": fontFamily = string.Copy(a.Value); break;
                    case "font-style":
                        switch (a.Value)
                        {
                            case "normal": fontStyle = FontStyleEnum.normal; break;
                            case "italic": fontStyle = FontStyleEnum.italic; break;
                            default:
                                LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("font-style={0}", a.Value)); break;
                        }
                        break;
                    case "font-size":Utilities.Parse(a.Value, ref fontSize, float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "font-weight":
                        switch (a.Value)
                        {
                            case "normal": fontWeight = FontWeightEnum.normal; break;
                            case "bold": fontWeight = FontWeightEnum.bold; break;
                            default:
                                LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("font-style={0}", a.Value)); break;
                        }
                        break;

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
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }
        }


        public static EmptyFontElement Create(XmlNode node)
        {
            return new EmptyFontElement(node);
        }
    }
}


