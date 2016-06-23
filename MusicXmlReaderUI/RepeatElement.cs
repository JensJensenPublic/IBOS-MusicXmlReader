using System.Xml;

namespace MusicXmlReaderUI
{

    public class RepeatElement : EventElement
    {
        string direction = "";

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
                        direction = a.Value;
                        break;
                }
            }
        }

        public static RepeatElement Create(XmlNode node)
        {
            return new RepeatElement(node);
        }

        private string LocalizeDirection(string direction)
        {
            switch (direction)
            {
                case "forward": return "start"; break;
                case "backward": return "slut"; break;
                default: return "???";
            }
       }


    public override string ToString()
        {

            return string.Format("Gentagelse {0}", LocalizeDirection(direction));
        }
    }
}
