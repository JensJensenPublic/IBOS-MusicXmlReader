using System.Xml;

namespace MusicXmlReaderUI
{

    enum AccidentalMarkEnum { undefined, flat, natural,sharp };

    /// <summary>
    /// Don't know what this is used for. The real accidentals are placed directly as children of the NoteElements, 
    /// and the accidental-marks are children of the NotationElement. Maybe just for defining graphical x-and y- offsets ??
    /// </summary>
    public class AccidentalMarkElement : Element
    {
        AccidentalMarkEnum accidentalMark;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private AccidentalMarkElement()
        {
        }

        private AccidentalMarkElement(XmlNode node)
        {
            // Dig out elements
            foreach (XmlNode child in node.ChildNodes)
            {
                string value = child.Value;
                Model.Log(string.Format("AccidentalMarkElement: Value={0}", value));
                switch (value)
                {
                    case "flat": accidentalMark = AccidentalMarkEnum.flat; break;
                    case "natural": accidentalMark = AccidentalMarkEnum.natural; break;
                    case "sharp": accidentalMark = AccidentalMarkEnum.sharp; break;
                    default: accidentalMark = AccidentalMarkEnum.undefined; break;
                }
            }            
        }


        public static AccidentalMarkElement Create(XmlNode node)
        {
            return new AccidentalMarkElement(node);
        }


        public override string ToString()
        {
            return string.Format("??");
        }
    }
}
