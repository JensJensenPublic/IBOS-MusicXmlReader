using System.Xml;

namespace MusicXmlReaderUI
{
    class SlurElement : StartStopContinueElement
    {
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private SlurElement(XmlNode node) : base(node)
        {
        }
        

        public static SlurElement Create(XmlNode node)
        {
            return new SlurElement(node);
        }


        public override string ToString() // LOCALIZE
        {
            string number = (1 == this.NumberLevel) ? "" :  NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("Legato{0} {1}", number, Localize(this.StartStopContinueType));
        }
    }
}

