using System.Xml;

namespace MusicXmlReaderUI
{

    public class TimeElement : EventElement
    {
        string localizedBeats = "";
        string localizedBeatType = "";

        private int beats;
        private int beatType;

        public int Beats
        {
            get
            {
                return beats;
            }
        }

        public int BeatType
        {
            get
            {
                return beatType;
            }
        }

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
        private string LocalizeBeatType(int beatType) // LOCALIZE
        {
            switch (beatType)
            {
                case 1: return "hele";
                case 2: return "halve";
                case 4: return "fjerdedele";
                case 8: return "ottendele";
                case 16: return "sekstendedele";
                case 32: return "toogtredivtedele";
            }
            return "";
        }

        


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TimeElement(XmlNode node)
        {
            const string functionName = "TimeElement";
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


            bool beatsOk = int.TryParse(beats, out this.beats);
            bool beatTypeOk = int.TryParse(beatType, out this.beatType);
            if (!(beatsOk && beatTypeOk))
            {
                Logger.LogOnce(string.Format("{0}: Invalid TimeElement: Beats={1} BeatType={2}", functionName, beats, beatType));
            }

            localizedBeats = this.beats.ToString();
            localizedBeatType = LocalizeBeatType(this.beatType);
        }

        public static TimeElement Create(XmlNode node)
        {
            return new TimeElement(node);
        }

        public override string ToString() // LOCALIZE
        {
            return string.Format("Takt:{0}{1}", localizedBeats, localizedBeatType);
        }
    }
}
