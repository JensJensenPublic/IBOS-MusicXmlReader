using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace MusicXmlReaderModel
{

    public enum TechnicalElementEnum
    {
        unknown,
        arrow,
        bend,
        doubleTongue,
        downBow,
        fingering,
        fingernails,
        fret,
        hammerOn,
        handbell,
        harmonic,
        heel,
        hole,
        openString,
        otherTechnical,
        pluck,
        pullOff,
        snapPizzicato,
        stopped,
        stringTechnical,
        tap,
        thumbPosition,
        toe,
        tripleTongue,
        upBow
    };

    /// <summary>
    /// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-technical.htm
    /// </summary>
    public class TechnicalElement : Element
    {
        private string className = "TechnicalElement";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TechnicalElement()
        { }

        private List<TechnicalElementEnum> technicals;
        private List<string> values;

        private TechnicalElementEnum ToEnum(XmlNode n)
        {
            string functionName = "ToEnum";
            switch (n.Name)
            {
                case "arrow": return TechnicalElementEnum.arrow;
                case "bend": return TechnicalElementEnum.bend;
                case "double-tongue": return TechnicalElementEnum.doubleTongue;
                case "down-bow": return TechnicalElementEnum.downBow;
                case "fingering": return TechnicalElementEnum.fingering;
                case "fingernails": return TechnicalElementEnum.fingernails;
                case "fret":  return TechnicalElementEnum.fret;
                case "hammer-on":return TechnicalElementEnum.hammerOn;
                case "handbell": return TechnicalElementEnum.handbell;
                case "harmonic": return TechnicalElementEnum.harmonic;
                case "heel":return TechnicalElementEnum.heel;
                case "hole":return TechnicalElementEnum.hole;
                case "open-string": return TechnicalElementEnum.openString;
                case "other-technical":return TechnicalElementEnum.otherTechnical;
                case "pluck": return TechnicalElementEnum.pluck;
                case "pull-off":return TechnicalElementEnum.pullOff;
                case "snap-pizzicato": return TechnicalElementEnum.snapPizzicato;
                case "stopped": return TechnicalElementEnum.stopped;
                case "string": return TechnicalElementEnum.stringTechnical;
                case "tap": return TechnicalElementEnum.tap;
                case "thumb-position": return TechnicalElementEnum.thumbPosition;
                case "toe": return TechnicalElementEnum.toe;
                case "triple-tongue": return TechnicalElementEnum.tripleTongue;
                case "up-bow":return TechnicalElementEnum.upBow;
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unknown child:'{2}'", className, functionName, n.Name));
                    return TechnicalElementEnum.unknown;
            }
        }





    /// <summary>
    /// Private constructor, used by the Crate() method
    /// </summary>
    /// <param name="node"></param>
    private TechnicalElement(XmlNode node)
        {
            const string functionName = "TechnicalElement";
            technicals = new List<TechnicalElementEnum>();
            values = new List<string>();
            foreach (XmlNode n in node.ChildNodes)
            {
             
                TechnicalElementEnum t = ToEnum(n);
                string value = n.InnerText;
                technicals.Add(t); // For now we just collect the technicals ! 
                values.Add(value); // and the corresponding values            
            }
        }

        public static TechnicalElement Create(XmlNode node)
        {
            return new TechnicalElement(node);
        }

        public override string ToString() // To be localized when implemented
        {
            string functionName = "ToString";
            StringBuilder allTecnnicals = new StringBuilder();
            string delimiter = "";
            for ( int i = 0 ; (i < technicals.Count); i ++)
            {
                string s = delimiter + technicals[i].ToString() + (("" == values[i]) ? "" : "=" + values[i]);
                if (!string.IsNullOrEmpty(s))
                {
                    Logger.LogOnce(string.Format("{0}.{1}: Localization is missing for '{2}'", className, functionName,s));
                }
                allTecnnicals.Append(s);
                delimiter = " ";
            }
            string result = allTecnnicals.ToString();
    
            return result; // TODO: Implement localization !
            // return string.Format("{0}", technicals.ToString()); // TODO: Implement localization !
        }
    }
}

