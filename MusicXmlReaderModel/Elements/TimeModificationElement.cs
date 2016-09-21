using System.Xml;

namespace MusicXmlReaderUI
{

    public class TimeModificationElement : Element
    {
        private int actualNotes;
        private int normalNotes;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TimeModificationElement()
        { }




        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TimeModificationElement(XmlNode node)
        {
            const string functionName = "TimeModificationElement";
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "actual-notes": Utilities.Parse(n.InnerText, ref actualNotes,1, int.MaxValue, functionName, false); break;
                    case "normal-notes": Utilities.Parse(n.InnerText, ref normalNotes,1, int.MaxValue, functionName, false); break;
                    case "normal-type":
                    case "normal-dot":  Logger.LogOnce(string.Format("{0}: Unimplemented child element. Name={1} Value={2}", functionName, n.Name, n.Value)); break;
                    default: Logger.LogOnce(string.Format("{0}: Unimplemented child element. Name={1} Value={2}", functionName, n.Name, n.Value)); break;
                }
            }
        }

        public static TimeModificationElement Create(XmlNode node)
        {
            return new TimeModificationElement(node);
        }

        public override string ToString()
        {
            return string.Format("");
        }
    }
}
