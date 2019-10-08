using System.Xml;

namespace MusicXmlReaderModel
{

    // 

    public class MeasureNumberingElement : Element
    {
        private FloatParameter defaultX;
        private FloatParameter defaultY;
        private FloatParameter relativeX;
        private FloatParameter relativeY;
        private StringParameter fontFamily;
        private FontStyleParameter fontStyle;
        private FontWeightParameter fontWeight;
        private HorizontalAlignParameter horizontalAlign;
        private VerticalAlignParameter verticalAlign; 
        private StringParameter color;
        enum HorizontalAlignEnum { unknown, left, center, right }
        enum VerticalAlignEnum { unknown, top,middle,bottom,baseline }

        public string ToDebugString()
        {
            string result = string.Format("MeasureNumbering({0}{1}{2}{3} Font({4}{5}{6}) Align({7}{8}{9}) )",
            Parameter.ToDebugString("dX=", defaultX), // 0
            Parameter.ToDebugString("dY=", defaultY ), // 1
            Parameter.ToDebugString("rX=", relativeX), //2
            Parameter.ToDebugString("rY=", relativeY), //3
            Parameter.ToDebugString("FontFamily=", fontFamily), //4
            Parameter.ToDebugString("FontStyle=", fontStyle), // 5
            Parameter.ToDebugString("FontWeight=", fontWeight), // 6
            Parameter.ToDebugString("hAlign=", horizontalAlign), // 7
            Parameter.ToDebugString("vAlign=", verticalAlign), // 8
            Parameter.ToDebugString("Color=", color)); // 9
            return result;
        }



        private MeasureNumberingElement()
        { }



        private MeasureNumberingElement(XmlNode node)
        {

            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
#warning TODO Fix Errorstring
                    case "default-x":   defaultX = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, "ErrorString"); break;
                    case "default-y":   defaultY = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, "ErrorString"); break;
                    case "relative-x":  relativeX = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, "ErrorString"); break;
                    case "relative-y":  relativeY = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, "ErrorString"); break;
                    case "font-family": fontFamily = StringParameter.Create(a.Value); break;
                    case "font-style": fontStyle = FontStyleParameter.Create(a.InnerXml); break;
                    case "font-size":  LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unsupported, a); break;
                    case "font-weight": fontWeight = FontWeightParameter.Create(a.InnerXml); break;
                    case "color":  color = StringParameter.Create(a.Value); break;
                    case "halign": horizontalAlign = HorizontalAlignParameter.Create(a.Value); break;
                    case "valign": verticalAlign = VerticalAlignParameter.Create(a.Value); break;
                    default:
                        LogFormatter.Log(LogFormatter.LogOptions.Once , a);
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

        public static MeasureNumberingElement Create(XmlNode node)
        {
            return new MeasureNumberingElement(node);
        }
    }
}


