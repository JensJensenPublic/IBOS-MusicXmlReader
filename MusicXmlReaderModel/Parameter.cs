using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public abstract class Parameter
    {

        public static string ToDebugString(string text, Parameter p)
        {
            // return (null == p) ? "" : text + p.ToDebugString() + " ";
            return ToDebugString(text, p, false);
        }

        public static string ToDebugString(string text, Parameter p, bool quote)
        {
            if (null == p) return "";
            string value = p.ToDebugString();
            return text + Quote(value, quote) + " "; 
        }

        private static string Quote(string s,  bool b)
        {
            if (null == s) return null;
            return b ? string.Format("'{0}'", s) : s;
        }


        public abstract string ToDebugString();

    }

    public class IntParameter : Parameter
    {
        private int value;
        public int Value { get { return value; } }

        public IntParameter(int i)
        {
            value = i;
        }

        public static IntParameter Create(int i)
        {
            return new IntParameter(i);
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }


        public static IntParameter Parse(string input, int lowValue, int highValue, bool acceptEmptyAsDefault)
        {
            int result = 0;
            if (!Utilities.Parse(input, ref result, lowValue, highValue, "", acceptEmptyAsDefault)) return null;
            return IntParameter.Create(result);
        }


    }

    public class FloatParameter : Parameter
    {
        float value;

        public FloatParameter(float f)
        {
            value = f;
        }
        
        public static FloatParameter Create(float f)
        {
            return new FloatParameter(f);
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }

        public static FloatParameter Parse(string input, float lowValue, float highValue, string errorString)
        {
            float result = 0;
            if (!Utilities.Parse(input, ref result, lowValue, highValue, errorString)) return null;
            return FloatParameter.Create(result);
        }



    }

    public class StringParameter : Parameter
    {
        string value;

        private StringParameter(string s)
        {
            value = string.Copy(s);
        }


        public static StringParameter Create(string s)
        {
            return new StringParameter(s);
        }

        public override string ToDebugString()
        {
            return value;
        }
    }


    public class YesNoParameter : Parameter
    {
        bool value;

        public YesNoParameter(bool b)
        {
            value = b;
        }

        public override string ToDebugString()
        {
            return value ? "Yes" : "No";
        }

        public static YesNoParameter Create(bool b)
        {
            return new YesNoParameter(b);
        }

        public static YesNoParameter ParseYesNoAttributeValue(string functionName, string attributeName, string attributeValue)
        {
            bool b = false;
            if (!Utilities.ParseYesNoAttributeValue(functionName, attributeName, attributeValue, ref b)) return null;
            return YesNoParameter.Create(b);

        }



    }

    public enum FontStyleEnum { unknown, normal, italic };

    public class FontStyleParameter : Parameter
    {
        private FontStyleEnum value;

        public FontStyleParameter(FontStyleEnum fontStyleEnum)
        {
            value = fontStyleEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }


        public static FontStyleParameter Create(string s)
        {
            switch (s)
            {
                case "normal": return new FontStyleParameter(FontStyleEnum.normal);
                case "italic": return new FontStyleParameter(FontStyleEnum.italic);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("font-style={0}", s)); break;
                 
            }
            return null;
        }
    }


    public enum FontWeightEnum { unknown, normal, bold };
    public class FontWeightParameter : Parameter
    {

        private FontWeightEnum value;

        public FontWeightParameter(FontWeightEnum fontWeightEnum)
        {
            value = fontWeightEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }
        
        public static FontWeightParameter Create(string s)
        {
            switch (s)
            {
                case "normal": return new FontWeightParameter(FontWeightEnum.normal);
                case "bold": return new FontWeightParameter(FontWeightEnum.bold);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("font-style={0}", s)); break;
            }
            return null;
        }
    }

    public enum HorizontalAlignEnum { unknown, left, center, right }
    public class HorizontalAlignParameter : Parameter
    {

        private HorizontalAlignEnum value;

        public HorizontalAlignParameter(HorizontalAlignEnum horizontalAlignEnum)
        {
            value = horizontalAlignEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }

        public static  HorizontalAlignParameter Create(string s)
        {
            switch (s)
            {
                case "left":  return new HorizontalAlignParameter(HorizontalAlignEnum.left);
                case "center": return new HorizontalAlignParameter(HorizontalAlignEnum.center);
                case "right": return new HorizontalAlignParameter(HorizontalAlignEnum.right);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("halign={0}", s)); break;
            }
            return null;     
        } 
    }


    public enum VerticalAlignEnum { unknown, top, middle, bottom, baseline }
    public class VerticalAlignParameter : Parameter
    {

        private VerticalAlignEnum value;

        public VerticalAlignParameter(VerticalAlignEnum verticalAlignEnum)
        {
            value = verticalAlignEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }

        public static VerticalAlignParameter Create(string s)
        {
            switch (s)
            {
                case "top":         return new VerticalAlignParameter(VerticalAlignEnum.top);
                case "middle":  return new VerticalAlignParameter(VerticalAlignEnum.middle);
                case "bottom": return new VerticalAlignParameter(VerticalAlignEnum.bottom);
                case "baseline": return new VerticalAlignParameter(VerticalAlignEnum.baseline);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("valign={0}", s)); break;
            }  
            return null;
        }


    }

    public enum MarginTypeEnum {unknown, odd, even,both }

    public class MarginTypeParameter : Parameter
    {
        private MarginTypeEnum value;

        private MarginTypeParameter(MarginTypeEnum marginTypeEnum)
        {
            value = marginTypeEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }

        public static MarginTypeParameter Create(string s)
        {
            switch (s)
            {
                case "odd": return new MarginTypeParameter(MarginTypeEnum.odd);
                case "even": return new MarginTypeParameter(MarginTypeEnum.even);
                case "both": return new MarginTypeParameter(MarginTypeEnum.both);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("type={0}", s)); break;
            }
            return null;
        }
    }


    public enum DefaultPreserveEnum { unknown, odd, Default, preserve}

    public class DefaultPreserveParameter : Parameter
    {
        private DefaultPreserveEnum value;

        private DefaultPreserveParameter(DefaultPreserveEnum defaultPreserveEnum)
        {
            value = defaultPreserveEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }

        public static DefaultPreserveParameter Create(string s)
        {
            switch (s)
            {
                case "default": return new DefaultPreserveParameter(DefaultPreserveEnum.Default);
                case "preserve": return new DefaultPreserveParameter(DefaultPreserveEnum.preserve);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, s); break;
            }
            return null;
        }

    }
 

    public class FloatOrNormalParameter : Parameter
    {
        private bool normal;
        private float value;

        public FloatOrNormalParameter(float f)
        {
            normal = false;
            value = f;
        }

        public FloatOrNormalParameter(bool b)
        {
            normal = b;
            value = 0;
        }

        public static FloatOrNormalParameter Create(float f)
        {
            return new FloatOrNormalParameter(f);
        }

        public override string ToDebugString()
        {
            return normal ? "normal" : value.ToString();
        }

        public static FloatOrNormalParameter Parse(string input, float lowValue, float highValue, string errorString)
        {
            if (0 == string.Compare(input, "normal"))
            {
                return new FloatOrNormalParameter(true);
            }

            float result = 0;
            if (!Utilities.Parse(input, ref result, lowValue, highValue, errorString)) return null;
            return new FloatOrNormalParameter(result);
        }

    }

    //

    //public enum MarginTypeEnum { unknown, odd, even, both }

    //public class MarginTypeParameter : Parameter
    //{
    //    private MarginTypeEnum value;

    //    private MarginTypeParameter(MarginTypeEnum marginTypeEnum)
    //    {
    //        value = marginTypeEnum;
    //    }

    //    public override string ToDebugString()
    //    {
    //        return value.ToString();
    //    }

    //    public static MarginTypeParameter Create(string s)
    //    {
    //        switch (s)
    //        {
    //            case "odd": return new MarginTypeParameter(MarginTypeEnum.odd);
    //            case "even": return new MarginTypeParameter(MarginTypeEnum.even);
    //            case "both": return new MarginTypeParameter(MarginTypeEnum.both);
    //            default:
    //                LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, string.Format("type={0}", s)); break;
    //        }
    //        return null;
    //    }
    //}


    public enum TextDirectionEnum { unknown,ltr,rtl,lro,rlo }

    public class TextDirectionParameter : Parameter
    {
        private TextDirectionEnum value;

        private TextDirectionParameter(TextDirectionEnum TextDirectionEnum)
        {
            value = TextDirectionEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }

        public static TextDirectionParameter Create(string s)
        {
            switch (s)
            {
                case "ltr": return new TextDirectionParameter(TextDirectionEnum.ltr);
                case "rtl": return new TextDirectionParameter(TextDirectionEnum.rtl);
                case "lro": return new TextDirectionParameter(TextDirectionEnum.lro);
                case "rlo": return new TextDirectionParameter(TextDirectionEnum.rlo);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, s); break;
            }
            return null;
        }

    }

    //

    public enum EnclosureShapeEnum { unknown, rectangle, square,oval,circle,bracket,triangle,diamond,none }
    public class EnclosureShapeParameter : Parameter
    {
        // https://usermanuals.musicxml.com/MusicXML/Content/ST-MusicXML-enclosure-shape.htm
        private EnclosureShapeEnum value;

        private EnclosureShapeParameter(EnclosureShapeEnum enclosureShapeEnum)
        {
            value = enclosureShapeEnum;
        }

        public override string ToDebugString()
        {
            return value.ToString();
        }

        public static EnclosureShapeParameter Create(string s)
        {
            switch (s)
            {
                case "rectangle": return new EnclosureShapeParameter(EnclosureShapeEnum.rectangle);
                case "square": return new EnclosureShapeParameter(EnclosureShapeEnum.square);
                case "oval": return new EnclosureShapeParameter(EnclosureShapeEnum.oval);
                case "circle": return new EnclosureShapeParameter(EnclosureShapeEnum.circle);
                case "bracket": return new EnclosureShapeParameter(EnclosureShapeEnum.bracket);
                case "triangle": return new EnclosureShapeParameter(EnclosureShapeEnum.triangle);
                case "diamond": return new EnclosureShapeParameter(EnclosureShapeEnum.diamond);
                case "none": return new EnclosureShapeParameter(EnclosureShapeEnum.none);
                default:
                    LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unknown, s); break;
            }
            return null;
        }

    }




}
