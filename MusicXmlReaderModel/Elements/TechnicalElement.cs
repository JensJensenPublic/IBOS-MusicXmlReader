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

        private string ToLocalizedString(TechnicalElementEnum technicalElementEnum)
        {
            string functionName = "ToLocalizedString";
            switch (technicalElementEnum)
            {
                case TechnicalElementEnum.arrow: return ResourcesForModel.TechnicalElement_arrow;
                case TechnicalElementEnum.bend: return ResourcesForModel.TechnicalElement_arrow;
                case TechnicalElementEnum.doubleTongue: return ResourcesForModel.TechnicalElement_doubleTongue;
                case TechnicalElementEnum.downBow: return ResourcesForModel.TechnicalElement_downBow;
                case TechnicalElementEnum.fingering: return ResourcesForModel.TechnicalElement_fingering;
                case TechnicalElementEnum.fingernails: return ResourcesForModel.TechnicalElement_fingernails;
                case TechnicalElementEnum.fret: return ResourcesForModel.TechnicalElement_fret;
                case TechnicalElementEnum.hammerOn: return ResourcesForModel.TechnicalElement_hammer_on;
                case TechnicalElementEnum.handbell: return ResourcesForModel.TechnicalElement_handbell;
                case TechnicalElementEnum.harmonic: return ResourcesForModel.TechnicalElement_harmonic;
                case TechnicalElementEnum.heel: return ResourcesForModel.TechnicalElement_heel;
                case TechnicalElementEnum.hole: return ResourcesForModel.TechnicalElement_hole;
                case TechnicalElementEnum.openString: return ResourcesForModel.TechnicalElement_open_string;
                case TechnicalElementEnum.otherTechnical: return ResourcesForModel.TechnicalElement_other_technical;
                case TechnicalElementEnum.pluck: return ResourcesForModel.TechnicalElement_pluck;
                case TechnicalElementEnum.pullOff: return ResourcesForModel.TechnicalElement_pullOff;
                case TechnicalElementEnum.snapPizzicato: return ResourcesForModel.TechnicalElement_snap_pizzicato;
                case TechnicalElementEnum.stopped: return ResourcesForModel.TechnicalElement_stopped;
                case TechnicalElementEnum.stringTechnical: return ResourcesForModel.TechnicalElement_string;
                case TechnicalElementEnum.tap: return ResourcesForModel.TechnicalElement_tap; ;
                case TechnicalElementEnum.thumbPosition: return ResourcesForModel.TechnicalElement_thumb_position;
                case TechnicalElementEnum.toe: return ResourcesForModel.TechnicalElement_toe;
                case TechnicalElementEnum.tripleTongue: return ResourcesForModel.TechnicalElement_tripleTongue;
                case TechnicalElementEnum.upBow: return ResourcesForModel.TechnicalElement_upbow;
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unknown TechnicalElementEnum", className, functionName));
                    return ResourcesForModel.TechnicalElement_unknown;  
            }
        }






        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TechnicalElement(XmlNode node)
        {
            // const string functionName = "TechnicalElement";
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
            //string functionName = "ToString";
            StringBuilder allTecnnicals = new StringBuilder();
            string delimiter = "";
            for ( int i = 0 ; (i < technicals.Count); i ++)
            {
                string localizedString = ToLocalizedString(technicals[i]);
                string s = delimiter + localizedString + (("" == values[i]) ? "" : "=" + values[i]);
                //Logger.LogOnce(string.Format("{0}.{1}: Found '{2}'", className, functionName,s));
                allTecnnicals.Append(s);
                delimiter = " ";
            }
            string result = allTecnnicals.ToString();
    
            return result; 
        }
    }
}

