using System;
using System.Xml;
using System.Collections.Generic;


namespace MusicXmlReaderModel
{

    // Abstract base class for the identical classes AccidentalTextElement and FormattedTextElement !
    // 
    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental-text.htm
    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-formatted-text.htm
    //
    // Also used for the implementation of
    //  http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#CT-MusicXML-empty-print-object-style-align.htm
    //
    // This base class implements the UNION af all elements and attributes handled by the derived classes.
    // Each derived class should call the   TextElementBase constructor using 2 arguments, each listig the names of the elements / arguments valid for the derived class.
    // For instance the constructor for the AccidentalTextElement class calls the TextElementBase constructor in the following way:
    // private AccidentalTextElement(XmlNode node) : base(node,MyAttributes,MyChildNodes)
    // Each derived class should define get-acccessors to exactly the protected Parameters in TextElementBase that it elementts

    abstract class TextElementBase : Element
    {
        protected YesNoParameter printObject;
        protected FloatParameter defaultX;
        protected FloatParameter defaultY;
        protected FloatParameter relativeX;
        protected FloatParameter relativeY;
        protected StringParameter fontFamily;
        protected FontStyleParameter fontStyle;
        protected FontWeightParameter fontWeight;
        protected HorizontalAlignParameter horizontalAlign;
        protected VerticalAlignParameter verticalAlign;
        protected StringParameter color;
        protected DefaultPreserveParameter xmlSpace;
        protected IntParameter underLine;
        protected IntParameter overLine;
        protected IntParameter lineThrough;
        protected FloatParameter rotation;
        protected FloatOrNormalParameter letterSpacing;
        protected FloatOrNormalParameter lineHeight;
        protected StringParameter text;
        protected TextDirectionParameter textDirection;
        protected EnclosureShapeParameter enclosureShape;

        protected TextElementBase()
        { }

        /// <summary>
        /// Handles the UNION of all attributes used by all base classes
        /// </summary>
        /// <param name="a"></param>
        protected void Parse(XmlAttribute a)
        {
            switch (a.Name)
            {
                case "print-object": printObject = YesNoParameter.ParseYesNoAttributeValue("", a.Name, a.Value); break;
                case "justify": LogFormatter.Log(once | unsupported, a); break;
                case "default-x": defaultX = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, ""); break;
                case "default-y": defaultY = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, ""); break;
                case "relative-x": relativeX = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, ""); break;
                case "relative-y": relativeY = FloatParameter.Parse(a.InnerXml, 0, float.MaxValue, ""); break;
                case "font-family": fontFamily = StringParameter.Create(a.Value); break;
                case "font-style": fontStyle = FontStyleParameter.Create(a.InnerXml); break;
                case "font-size": LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unsupported, a); break;
                case "font-weight": fontWeight = FontWeightParameter.Create(a.InnerXml); break;
                case "color": color = StringParameter.Create(a.Value); break;
                case "halign": horizontalAlign = HorizontalAlignParameter.Create(a.Value); break;
                case "valign": verticalAlign = VerticalAlignParameter.Create(a.Value); break;
                case "underline": underLine = IntParameter.Parse(a.Value, 0, 3, true); break;
                case "overline": overLine = IntParameter.Parse(a.Value, 0, 3, true); break;
                case "line-through": lineThrough = IntParameter.Parse(a.Value, 0, 3, true); break;
                case "rotation": rotation = FloatParameter.Parse(a.Value, (float)-180, (float)180, ""); break;
                case "letter-spacing": letterSpacing = FloatOrNormalParameter.Parse(a.Value, float.MinValue, float.MaxValue, ""); break; // TODO
                case "line-height": lineHeight = FloatOrNormalParameter.Parse(a.Value, float.MinValue, float.MaxValue, ""); break; // TODO
                case "xml:lang": LogFormatter.Log(once | unsupported, a); break; // TODO
                case "xml:space": xmlSpace = DefaultPreserveParameter.Create(a.Value); break;
                case "dir": textDirection = TextDirectionParameter.Create(a.Value); break;
                case "enclosure": enclosureShape = EnclosureShapeParameter.Create(a.Value); break;
                default: LogFormatter.Log(once | unknown, a); break;
            }

        }

        /// <summary>
        /// Handles the UNION of all child nodes used by all base classes
        /// </summary>
        /// <param name="n"></param>
        protected void Parse(XmlNode n)
        {
            switch (n.Name)
            {
                case "#text": text = StringParameter.Create(n.Value); break;

                default:
                    LogFormatter.Log(once | unknown, n); break;
            }
        }

        protected TextElementBase(XmlNode node, List<string> attributes, List<string> childNodes)
        {
            foreach (XmlAttribute a in node.Attributes)
            {
                if (attributes.Contains(a.Name))
                {
                    Parse(a); // Use the general parse-mechanismin the base class
                }
                else
                {
                    LogFormatter.Log(once | unknown, a);
                }
            }

            foreach (XmlNode n in node.ChildNodes)
            {
                if (childNodes.Contains(n.Name))
                {
                    Parse(n);  // Use the general parse-mechanismin the base class
                }
                else
                {
                    LogFormatter.Log(once | unknown, n);
                }
            }
        }



        protected string Text(string s, Parameter p)
        {
            return Parameter.ToDebugString(s, p);
        }

        public virtual string ToDebugString()
        {
            try
            {
                string result = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}",
                Text("PrintObject=", printObject), // 0
                Text("dX=", defaultX), // 1
                Text("dY=", defaultY), // 2
                Text("rX=", relativeX), // 3
                Text("rY=", relativeY), // 4
                Text("FontFamily=", fontFamily), // 5
                Text("FontStyle=", fontStyle), // 6
                Text("FontWeight=", fontWeight), // 7
                Text("hAlign=", horizontalAlign), // 8
                Text("vAlign=", verticalAlign), // 9
                Text("Color=", color), // 10
                Text("underline=", underLine), // 11
                Text("overline=", overLine), // 12
                Text("line-through=", lineThrough), // 13
                Text("rotation=", rotation), // 14
                Text("letter-spacing", letterSpacing), // 15
                Text("line-height", lineHeight), // 16
                "",  //Text("xml:lang",), // 17 Not implemented yet. This will be reported by the Create() method.
                Text("xml:space=", xmlSpace), // 18
                Text("dir=",textDirection), // 19
                Text("enclosure=",enclosureShape), // 20
                Text("Text=", text)); // 21
                return result;
            }
            catch (Exception e)
            {
                LogFormatter.Log(0, string.Format("Exception.Message={0}", e.Message));
                return "ERROR";
            }
        }        
    }
}



