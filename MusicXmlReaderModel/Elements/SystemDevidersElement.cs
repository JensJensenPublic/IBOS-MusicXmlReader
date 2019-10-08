using System.Xml;

namespace MusicXmlReaderModel
{
    class SystemDevidersElement : Element
    {
        private SystemDevidersElement()
        { }

        private PrintEmptyObjectStyleAlignElement leftDivider;
        private PrintEmptyObjectStyleAlignElement rightDivider;

        SystemDevidersElement(XmlNode node)
        {
            foreach (XmlAttribute a in node.Attributes)
            {
                LogFormatter.Log(unknown, a);
            }

            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "left-divider": leftDivider = PrintEmptyObjectStyleAlignElement.Create(n); break;
                    case "right-divider": rightDivider = PrintEmptyObjectStyleAlignElement.Create(n); break;
                    default: LogFormatter.Log(once | unknown, n); break;
                }
            }

            LogFormatter.Log(once, string.Format("SystemDividersElement({0})", ToDebugString())); // During initial debugging only !!
        }

        protected string Text(string s, Parameter p)
        {
            return Parameter.ToDebugString(s, p);
        }

        public string ToDebugString()
        {
            string result = string.Format("{0}{1}",
                (null != leftDivider) ? string.Format("LeftDivider({0}) ", leftDivider.ToDebugString()) : "",
                (null != rightDivider) ? string.Format("RightDivider({0})", rightDivider.ToDebugString()) : "");
            return result;
        }

        public static SystemDevidersElement Create(XmlNode n)
        {
            return new SystemDevidersElement(n);
        }
    }
}
