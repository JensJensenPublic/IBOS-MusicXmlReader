using System.Xml;

// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-measure-style.htm

namespace MusicXmlReaderModel
{
    class MeasureStyleElement : EventElement
    {

        private MeasureStyleElement(XmlNode node)
        {

        }

        public static MeasureStyleElement Create(XmlNode node)
        {
            return new MeasureStyleElement(node);
        }
    }
}
