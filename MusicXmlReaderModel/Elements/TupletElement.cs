using System.Xml;

namespace MusicXmlReaderUI
{
    class TupletElement : StartStopContinueElement
    { 
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TupletElement(XmlNode node) : base(node)
        {
        }


        public static TupletElement Create(XmlNode node)
        {
            return new TupletElement(node);
        }


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("Tuplet{0} {1}", number, Localize(this.StartStopContinueType));
        }
    }
}