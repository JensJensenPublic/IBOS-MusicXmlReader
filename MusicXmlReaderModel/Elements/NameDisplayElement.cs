
using System.Xml;



namespace MusicXmlReaderModel
{
    public class NameDisplayElement : Element
    {

        // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-name-display.htm


        private NameDisplayElement()
        { }

        //private bool printObject;

        private YesNoParameter printObject;
        private AccidentalTextElement accidentalText;
        private FormattedTextElement formattedText;


        private NameDisplayElement(XmlNode node)
        {

#warning TODO use newer mechanism for class and function names ! 

            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "print-object": printObject = YesNoParameter.ParseYesNoAttributeValue("", a.Name, a.Value); break;
                        //Utilities.ParseYesNoAttributeValue(functionName,a.Name,a.Value,ref printObject); break;
                    default:
                        LogFormatter.Log(once, a);
                        break;
                }
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "accidental-text": accidentalText = AccidentalTextElement.Create(n); break;
                    case "display-text":    formattedText = FormattedTextElement.Create(n); break; 
                    default:
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }

            LogFormatter.Log(LogFormatter.LogOptions.Once,ToDebugString());
            
                       
        }

        public static NameDisplayElement Create(XmlNode node)
        {
            return new NameDisplayElement(node);
        }

        public string ToDebugString()
        {
            string result = string.Format("{0}{1}{2}",
                (null != printObject) ? printObject.ToDebugString() + " " : "", // 0
                (null != accidentalText) ? accidentalText.ToDebugString() + " " : "", //1
                (null != formattedText) ? formattedText.ToDebugString() + " " : "");  // 2 

            LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.Unsupported, result);
            return "";
        }

    }
}


