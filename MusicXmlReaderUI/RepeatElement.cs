using System.Xml;

namespace MusicXmlReaderUI
{

    /// <summary>
    /// http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-repeat.htm
    /// </summary>
    public class RepeatElement : EventElement
    {
        public enum RepeatDirectionEnum { Undefinded, Forward, Backward }; // LOCALIZE
        RepeatDirectionEnum repeatDirection;
        int timesToRepeat;

        public RepeatDirectionEnum RepeatDirection
        {
            get
            {
                return repeatDirection;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private RepeatElement()
        {
        }

        
        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private RepeatElement(XmlNode node)
        {
            string functionName = "RepeatElement";
            Logger.LogOnce(string.Format("RepeatElement constructor"));
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "direction":
                        switch (a.Value)
                        {
                            case "forward": repeatDirection = RepeatDirectionEnum.Forward; break;
                            case "backward": repeatDirection = RepeatDirectionEnum.Backward; break;
                            default: Logger.Log(string.Format("RepeatElement: Unexpected attributevalue {0} found",a.Value)); break;
                        }
                        break;
                    case "times": Utilities.Parse(a.Value, ref timesToRepeat, 0, int.MaxValue, string.Format("{0}.Number", functionName),true); break;
                    case "winged": break; // Pure graphical information    
                    default: Logger.Log(string.Format("RepeatElement: Unexpected attribute {0} found", a.Name)); break;
                }
            }
        }

        public static RepeatElement Create(XmlNode node)
        {
            return new RepeatElement(node);
        }

        private string LocalizeDirection(RepeatDirectionEnum direction) // LOCALIZE
        {
            switch (repeatDirection)
            {
                case RepeatDirectionEnum.Forward:  return "start";
                case RepeatDirectionEnum.Backward: return "slut";
                default: return "???";
            }
       }


        public override string ToString() // LOCALIZE
        {

            return string.Format("Gentagelse {0}", LocalizeDirection(repeatDirection));
        }
    }
}
