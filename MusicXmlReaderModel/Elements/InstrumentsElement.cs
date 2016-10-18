using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    public class InstrumentsElement : EventElement
    {
        private int instrument = 1;

        private InstrumentsElement(XmlNode node)
        {
            Utilities.Parse(node.InnerText, ref instrument, 0, int.MaxValue,"InstrumentsElement.number", true);
        }

        public int Instrument
        {
            get
            {
                return instrument;
            }
        }

        public static InstrumentsElement Create(XmlNode node)
        {
            return new InstrumentsElement(node);
        }
    }
}
