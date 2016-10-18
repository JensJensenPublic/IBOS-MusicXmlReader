using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
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


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("{0} {1} {2}",ResourcesForModel.TiedElement_Name, number, Localize(this.StartStopContinueType));
        }
    }
}

