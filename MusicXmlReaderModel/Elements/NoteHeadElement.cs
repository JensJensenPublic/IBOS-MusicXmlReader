using System.Xml;
using System.Globalization;


namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-notehead.htm

    enum NoteHeadTypeEnum
    {
        unDefined = 0,
        slash,
        triangle,
        diamond,
        square,
        cross,
        x,
        circleX,
        invertedTriangle,
        arrowDown,
        arrowUp,
        slashed,
        backSlashed,
        normal,
        cluster,
        circleDot,
        leftTriangle,
        rectangle,
        none,
        Do, // do is a reserved word !
        re,
        mi,
        fa,
        faUp,
        so,
        la,
        ti
    }



    class NoteHeadElement
    {
        const string className = "NoteHeadElement";
        bool filled = false;
        bool parentheses = false;
        string innerText = "";
        NoteHeadTypeEnum type = NoteHeadTypeEnum.unDefined;

        public string InnerText
        {
            get
            {
                return innerText;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private NoteHeadElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private NoteHeadElement(XmlNode node)
        {
            const string functionName = "NoteHeadElement";
            // Dig out attributes

            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "filled":
                        Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref filled); break;
                    case "parentheses":
                        Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref parentheses); break;
                    case "font-family":
                    case "font-style":
                    case "font-size":
                    case "font-weight":
                    case "color": break; // Explicitly ignore graphical attributes.
                    default: Logger.LogOnce(string.Format("{0}.{1}: Unknown attribute name={2} with value={3}", className, functionName, a.Name, a.Value)); break;
                }
            }
            innerText = node.InnerText;

            switch (innerText)
            {
                case "slash":  type = NoteHeadTypeEnum.slash; break;
                case "triangle": type = NoteHeadTypeEnum.triangle; break;
                case "diamond": type = NoteHeadTypeEnum.diamond; break;
                case "square": type = NoteHeadTypeEnum.square; break;
                case "cross": type = NoteHeadTypeEnum.cross; break;
                case "x": type = NoteHeadTypeEnum.x; break;
                case "circle-x": type = NoteHeadTypeEnum.circleX; break;
                case "inverted triangle": type = NoteHeadTypeEnum.invertedTriangle; break;
                case "arrow down": type = NoteHeadTypeEnum.arrowDown; break;
                case "arrow up": type = NoteHeadTypeEnum.arrowUp; break;
                case "slashed": type = NoteHeadTypeEnum.slashed; break;
                case "back slashed": type = NoteHeadTypeEnum.backSlashed;break;
                case "normal": type = NoteHeadTypeEnum.normal; break;
                case "cluster": type = NoteHeadTypeEnum.cluster; break;
                case "circle dot": type = NoteHeadTypeEnum.circleDot; break;
                case "left triangle": type = NoteHeadTypeEnum.leftTriangle; break;
                case "rectangle": type = NoteHeadTypeEnum.rectangle; break;
                case "none": type = NoteHeadTypeEnum.none; break;
                case "do": type = NoteHeadTypeEnum.Do; break;
                case "re": type = NoteHeadTypeEnum.re; break;
                case "mi": type = NoteHeadTypeEnum.mi; break;
                case "fa": type = NoteHeadTypeEnum.fa; break;
                case "fa up":type = NoteHeadTypeEnum.faUp;break;
                case "so": type = NoteHeadTypeEnum.so; break;
                case "la": type = NoteHeadTypeEnum.la; break;
                case "ti": type = NoteHeadTypeEnum.ti; break; 
                default:
                    type = NoteHeadTypeEnum.unDefined;
                    Logger.LogOnce(string.Format("{0}.{1}: Unexpected value='{2}'", className, functionName, innerText));
                    break;

            }

            if (type != NoteHeadTypeEnum.unDefined)
            {
                // Logger.LogOnce(string.Format("{0}.{1}: NoteElementType='{2}'", className, functionName, type.ToString()));
            }

        }


        public static NoteHeadElement Create(XmlNode node)
        {
            return new NoteHeadElement(node);
        }


        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                                filled? ResourcesForModel.NoteHeadElement_Filled + " " : "", // 0
                                parentheses? ResourcesForModel.NoteHeadElement_Parentheses + " ": "", // 1
                                innerText // 2
                                );
        }
    }

}

