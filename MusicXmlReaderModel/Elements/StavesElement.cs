using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-staves.htm


    //  Primarily graphics information, but might be needed for IntervalNotation etc. 
    public class StavesElement : EventElement
    {
        private StavesElement()
        { }

        private IntParameter staves;

        private StavesElement(XmlNode node)
        {
            foreach (XmlAttribute a in node.Attributes)
            {
                LogFormatter.Log(once,a);
            }

            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "#text": staves = IntParameter.Parse(n.Value, 1, 4, true); break; // 4 : left/right * hand/foot
                    default: LogFormatter.Log(once, n); break;
                }
            }

            // LogFormatter.Log(once,  string.Format("Staves({0})", ToDebugString()));

            if (3 == staves.Value)
            {
                LogFormatter.Log(once, "Staves=3"); // Only for placing a breakpoint !
            }

        }

        public string ToDebugString()
        {
            return string.Format("Staves={0}", staves.ToDebugString());
        }

        public static StavesElement Create(XmlNode node)
        {
            return new StavesElement(node);
        }
    }
}
