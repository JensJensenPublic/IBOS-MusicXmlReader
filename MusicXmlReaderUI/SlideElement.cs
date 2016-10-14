using System.Xml;
using System.Globalization;
using MusicXmlReaderModel;

namespace MusicXmlReaderUI
{
    class SlideElement : StartStopContinueElement
    {
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private SlideElement(XmlNode node) : base(node)
        {
        }


        public static SlideElement Create(XmlNode node)
        {
            return new SlideElement(node);
        }


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("{0} {1} {2}",ResourcesForModel.SlideElement_Name , Localize(this.StartStopContinueType));
        }
    }
}
