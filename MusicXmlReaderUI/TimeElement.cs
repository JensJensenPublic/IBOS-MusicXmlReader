using System.Xml;

namespace MusicXmlReaderUI
{

    public class TimeElement : Element
    {
        string localizedBeats = "";
        string localizedBeatType = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TimeElement()
        { }


        /// <summary>
        /// Convert from position on the circle of fifths to an (audible) node name
        /// </summary>
        /// <param name="beats"></param>
        /// <returns></returns>
        private string LocalizeBeatType(string beatType)
        {
            switch (beatType)
            {
                case "1": return "hele";
                case "2": return "halve";
                case "4": return "fjerdedele";
                case "8": return "ottendele";
                case "16": return "sekstendedele";
                case "32": return "toogtredivtedele";
            }
            return "";
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TimeElement(XmlNode node)
        {
            string beats = "";
            string beatType = "";
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "beats": beats = n.InnerText; break;
                    case "beat-type": beatType = n.InnerText; break;
                }
            }

            localizedBeats = beats;
            localizedBeatType = LocalizeBeatType(beatType);
        }

        public static TimeElement Create(XmlNode node)
        {
            return new TimeElement(node);
        }

        public override string ToString()
        {
            return string.Format("Taktart: {0} {1}", localizedBeats, localizedBeatType);
        }
    }
}
