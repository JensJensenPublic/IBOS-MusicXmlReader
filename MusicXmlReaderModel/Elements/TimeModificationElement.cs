using System.Xml;

namespace MusicXmlReaderUI
{

    public class TimeModificationElement : Element
    {
        private int actualNotes;
        private int normalNotes;
        private NoteDurationType noteDurationType;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TimeModificationElement()
        { }




        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary> 
        /// <param name="node"></param>
        private TimeModificationElement(XmlNode node,NoteElement noteElement)
        {
            const string functionName = "TimeModificationElement";
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "actual-notes":    Utilities.Parse(n.InnerText, ref actualNotes,1, int.MaxValue, functionName, false); break;
                    case "normal-notes":    Utilities.Parse(n.InnerText, ref normalNotes,1, int.MaxValue, functionName, false); break;
                    case "normal-type":     noteDurationType = noteElement.GetDuration(n.InnerText); break; // Decode as if it were an noteElement
                    case "normal-dot":      Logger.LogOnce(string.Format("{0}: Unimplemented child element. Name={1} Value={2}", functionName, n.Name, n.Value)); break;
                    default:                Logger.LogOnce(string.Format("{0}: Unknown child element. Name={1} Value={2}", functionName, n.Name, n.Value)); break;
                }
            }
        }

        public int ActualNotes
        {
            get
            {
                return actualNotes;
            }
        }

        public int NormalNotes
        {
            get
            {
                return normalNotes;
            }
        }

        public NoteDurationType NoteDurationType
        {
            get
            {
                return noteDurationType;
            }
        }

        public static TimeModificationElement Create(XmlNode node,NoteElement noteElement)
        {
            return new TimeModificationElement(node,noteElement);
        }

        public override string ToString()
        {
            return string.Format("");
        }
    }
}
