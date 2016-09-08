using System.Xml;

namespace MusicXmlReaderUI
{
    class GlissandoElement : StartStopContinueElement
    {
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private GlissandoElement(XmlNode node) : base(node)
        {
        }


        public static GlissandoElement Create(XmlNode node)
        {
            return new GlissandoElement(node);
        }


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("Glissando{0} {1}", number, Localize(this.StartStopContinueType));
        }
    }
}
