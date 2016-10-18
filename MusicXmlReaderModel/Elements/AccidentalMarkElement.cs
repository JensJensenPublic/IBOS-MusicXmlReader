using System.Xml;

namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental-mark.htm

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
                Logger.LogOnce(string.Format("AccidentalMarkElement: Value={0}", value));
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

        private string LocalizeAccicdentalMark(AccidentalMarkEnum accidentalMarkEnum)
        {
            string functionName = "LocalizeAccicdentalMark";
            switch (accidentalMarkEnum)
            {

                case AccidentalMarkEnum.flat: return ResourcesForModel.AccidentalMarkElement_flat;
                case AccidentalMarkEnum.sharp: return ResourcesForModel.AccidentalMarkElement_sharp;
                case AccidentalMarkEnum.natural: return ResourcesForModel.AccidentalMarkElement_natural;
                default:
                    Logger.LogOnce(string.Format("{0}: AccidentalMark has illegal value={1}", functionName, accidentalMark.ToString())); break;                 
            }
            return "";
        }
    
        public override string ToString()
        {
            // Explicitly do not use {0}
             return string.Format("{1}",ResourcesForModel.AccidentalMarkElement_text,LocalizeAccicdentalMark(accidentalMark)); 
        }
    }
}
