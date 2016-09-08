using System.Xml;

namespace MusicXmlReaderUI
{

    public class RepeatElement : EventElement
    {
        public enum RepeatDirectionEnum { Undefinded, Forward, Backward };
        RepeatDirectionEnum repeatDirection;

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

                    default: Logger.Log(string.Format("RepeatElement: Unexpected attribute {0} found", a.Name)); break;
                }
            }
        }

        public static RepeatElement Create(XmlNode node)
        {
            return new RepeatElement(node);
        }

        private string LocalizeDirection(RepeatDirectionEnum direction)
        {
            switch (repeatDirection)
            {
                case RepeatDirectionEnum.Forward: return "start";
                case RepeatDirectionEnum.Backward: return "slut";
                default: return "???";
            }
       }


        public override string ToString()
        {

            return string.Format("Gentagelse {0}", LocalizeDirection(repeatDirection));
        }
    }
}
