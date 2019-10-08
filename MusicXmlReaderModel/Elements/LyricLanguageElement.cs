using System.Xml;

namespace MusicXmlReaderModel
{
    class LyricLanguageElement : Element
    {

        private IntParameter number;
        private StringParameter name;
        private StringParameter language;

        private LyricLanguageElement()
        { }

        private LyricLanguageElement(XmlNode node)
        {
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "number": number = IntParameter.Parse(a.Value, 0, int.MaxValue, true); break;
                    case "name": name = StringParameter.Create(a.Value); break;
                    case "xml:lang": language = StringParameter.Create(a.Value); break;
                    default:
                        LogFormatter.Log(once | unknown, a); break;
                }

            }
        }

        public string ToDebugString()
        {
            string result = string.Format("LyricFont({0}{1}{2})",
                Parameter.ToDebugString("Number=", number), // 0
                Parameter.ToDebugString("Name=", name), // 1
                Parameter.ToDebugString("Language=", language, true));// 2 true mans "Show in quotes"
            return result;
        }


        public static LyricLanguageElement Create(XmlNode node)
        {
            return new LyricLanguageElement(node);
        }
    }


    }
