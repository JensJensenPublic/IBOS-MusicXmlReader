using System.Xml;

namespace MusicXmlReaderUI
{
    // The tied type represents the notated tie. The tie element represents the tie sound.
    class TiedElement : StartStopContinueElement
    {

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TiedElement(XmlNode node) : base(node)
        {
        }


        public static TiedElement Create(XmlNode node)
        {
            return new TiedElement(node);
        }


        public override string ToString() // LOCALIZE
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("Bindebue {0} {1}", number, Localize(this.StartStopContinueType));
        }
    }
}

