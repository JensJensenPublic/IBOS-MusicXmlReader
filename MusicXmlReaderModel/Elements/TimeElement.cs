using System.Xml;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
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
        private string LocalizeBeatType(int beatType)
        {
            string functionName = "LocalizeBeatType";
            switch (beatType)
            {
                case  1: return ResourcesForModel.TimeElement_wholes; // "hele";
                case  2: return ResourcesForModel.TimeElement_halves; // "halve";
                case  4: return ResourcesForModel.TimeElement_quarters; // "fjerdedele";
                case  8: return ResourcesForModel.TimeElement_eights; // "ottendele";
                case 16: return ResourcesForModel.TimeElement_sixteenths; // "sekstendedele";
                case 32: return ResourcesForModel.TimeElement_thirtyseconds; // "toogtredivtedele";
                default:
                    Logger.Log(string.Format("{0}: Illegal value for beatTyoe={1}", functionName, beatType.ToString())); break;
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

        public override string ToString()
        {
            return string.Format("{0}:{1}-{2}",  ResourcesForModel.TimeElement_pulse,  localizedBeats, localizedBeatType);
        }
    }
}
