using System;
using System.Xml;
using System.Globalization;
using System.Diagnostics; // For finding calling method
using System.Reflection;  // For finding calling method

namespace MusicXmlReaderModel
{


    // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-print.htm


    /// <summary>
    /// Contains information about the graphical formatting 
    /// This could be useful even for a blind user in a learning-situation where other
    /// (sighted) students may refer to systems and page numbers.
    /// </summary>
    public class PrintElement : EventElement
    {
        private YesNoParameter newPage;
        public YesNoParameter NewPage { get { return newPage; } }
        private YesNoParameter newSystem;
        public YesNoParameter NewSystem { get { return newSystem; } }
        private IntParameter pageNumber;
        public IntParameter PageNumber { get { return pageNumber; } }
        private FloatParameter staffSpacing;
        private IntParameter blankPage;

        private SystemLayoutElement systemLayout;
        private StaffLayoutElement staffLayout;
        private MeasureNumberingElement measureNumbering;
        private PageLayoutElement pageLayout;
        private NameDisplayElement partAbbreviationDisplay;
        private NameDisplayElement partNameDisplay;


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private PrintElement()
        { }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private PrintElement(XmlNode node)
        {
            string functionName = "PrintElement.ctor";

            // Dig out Attributes:
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
#warning TODO Errorstring
                    case "new-page"  :  newPage = YesNoParameter.ParseYesNoAttributeValue(functionName, a.Name, a.Value); break;
                    case "new-system":  newSystem =  YesNoParameter.ParseYesNoAttributeValue(functionName, a.Name, a.Value); break;
                    case "page-number": pageNumber = IntParameter.Parse(a.Value, 0, int.MaxValue,  false); break;
                    case "staff-spacing": staffSpacing = FloatParameter.Parse(a.Value,float.MinValue, float.MaxValue, "ErrorString"); break;
                    case "blank-page":    blankPage = IntParameter.Parse(a.Value, 1, int.MaxValue, true); break;
                    default:
                        LogFormatter.Log(once | unknown, a);
                        break;
                }
            }


            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {          
                    case "system-layout": systemLayout = SystemLayoutElement.Create(n);  break;
                    case "staff-layout":  staffLayout = StaffLayoutElement.Create(n);    break;
                    case "page-layout":   pageLayout = PageLayoutElement.Create(n); break;                     
                    case "measure-numbering": measureNumbering = MeasureNumberingElement.Create(n); break;  
                    case "part-abbreviation-display": partAbbreviationDisplay = NameDisplayElement.Create(n); break;
                    case "part-name-display": partNameDisplay = NameDisplayElement.Create(n); break;
                    case "measure-layout":
                        LogFormatter.Log(once | unsupported, n);
                        break;
                    default:
                        //Logger.LogCFOnce(string.Format(": Unknown element. Name='{0}' InnerText='{1}'", n.Name,n.InnerText));
                        LogFormatter.Log(once | unknown, n);
                        break;
                }
            }

            //LogFormatter.Log(once, this.ToDebugString()); // Only during initial debugging !
            //LogFormatter.Log(once, "");
        }

        /// <summary>
        /// Only for debugging ! No localization !
        /// </summary>
        /// <returns></returns>
        public string ToDebugString()
        {
            string attributes = string.Format("{0}{1}{2}{3}{4}",
            Parameter.ToDebugString("NewPage=", newPage), // 0
            Parameter.ToDebugString("NewSystem=", newSystem), //1
            Parameter.ToDebugString("PageNumber=", pageNumber), // 2
            Parameter.ToDebugString("StaffSpacing=", staffSpacing), // 3
            Parameter.ToDebugString("BlankPage=", blankPage)); // 4

            string elements = string.Format("{0}{1}{2}{3}{4}{5}",
            (null != systemLayout) ? systemLayout.ToDebugString() + " " : "", // 0 ;
            (null != staffLayout) ? staffLayout.ToDebugString()+ " " : "", // 1
            (null != measureNumbering) ? measureNumbering.ToDebugString() + " " : "", // 2
            (null != pageLayout) ? pageLayout.ToDebugString() : "", // 3
            (null != partAbbreviationDisplay) ? partAbbreviationDisplay.ToDebugString() + " " : "", // 4
            (null != partNameDisplay) ? partNameDisplay.ToDebugString() + " ": "");

            return attributes + elements;

        }

        public static PrintElement Create(XmlNode node)
        {
            return new PrintElement(node);
        }

        /// <summary>
        /// Easy mechanism for turning on and off support for graphics infirmation
        /// </summary>
        /// <param name="node"></param>
        /// <param name="supported"></param>
        /// <returns></returns>
        public static PrintElement Create(XmlNode node, bool supported)
        {
            if (supported) return new PrintElement(node);
            LogFormatter.Log(always | unsupported, node);
            return null;
        }

        public override string ToString()
        {
        return "";
        }

    }
}
