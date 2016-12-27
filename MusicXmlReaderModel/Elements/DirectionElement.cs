using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-direction.htm
    /// </summary>
    class DirectionElement : EventElement
    {
        string className = "DirectionElement";
        private DynamicsElement dynamicsElement;
        private SoundElement soundElement;
        int staffNumber = 1; // The staff to which this element belongs. Parsed, but not used yet.
        int voiceNumber = 1;  // The voice (within a part, for instance S1 or S2) to which this element belongs. Parsed, but not used yet.
        // Prevent construction
        private DirectionElement()
        {
        }

        private DirectionElement(XmlNode node)
        {
            string functionName = "DirectionElement";
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {

                switch (a.Name)
                {

                    case "default-x": break; // Explicitly ignore some graphical attributes 
                    //    TODO list other attributes to be ignored
                    default: break;
                        //    Logger.LogOnce(string.Format("{0}.{1}:", className,functionName)); break;
                }
            }



            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "direction-type":
                        // Logger.LogOnce(string.Format("{0}.{1} DirectionType Value={2}", className, functionName, n.InnerText));
                        foreach (XmlNode child in n.ChildNodes)
                        {
                            switch (child.Name)
                            {
                                case "dynamics": dynamicsElement = DynamicsElement.Create(child); break;
                                default: break;
                            }
                        }
                        break;
                    case "sound":
                        // The sound element contains general playback parameters.
                        // They can stand alone within a part / measure, or be a component element within a direction.
                        soundElement = SoundElement.Create(n); // We expose it when the DirectionElement.Create() returns.
                        //Logger.LogOnce(string.Format("{0}.{1} Child: {2}", className, functionName, n.Name));
                        break;

                    case "offset":
                    case "footnote":
                        break; // Pure graphic information. Explicitly ignore!
          
                    case "staff":
                        // We parse "staff" nodes  but we do explicitly not support them yet.
                        Utilities.Parse(n.InnerText, ref staffNumber, 1, int.MaxValue, className + "." + functionName, true);
                        if (1 != staffNumber)
                        {
                            // This is actually pure graphical information, so don't bother to log it !
                            //Logger.LogOnce(string.Format("{0}.{1}: Child='{2}' has unsupported value={3}", className,functionName, n.Name,staffNumber));
                        }
                        break;

                    case "voice":
                        // We parse "voice" nodes  but we do explicitly not support them yet. 
                        Utilities.Parse(n.InnerText, ref voiceNumber, 1, int.MaxValue, className + "." + functionName, true);
                        if (1 != voiceNumber)
                        {
                            Logger.LogOnce(string.Format("{0}.{1}: Child='{2}' has unsupported value={3}", className, functionName, n.Name, voiceNumber));
                        }
                        break;

                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unsupported child: {2}", className, functionName, n.Name));
                        break;
                }
            }

        }

        internal DynamicsElement DynamicsElement
        {
            get
            {
                return dynamicsElement;
            }

        }

        internal SoundElement SoundElement
        {
            get
            {
                return soundElement;
            }
        }

        public static DirectionElement Create(XmlNode xmlNode)
        {
            return new DirectionElement(xmlNode);
        }

    }
}
