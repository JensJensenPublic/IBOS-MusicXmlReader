using System.Xml;

namespace MusicXmlReaderUI
{
    enum ArpeggiateDirectionEnum {undefined, up, down};

    class ArpeggiateElement : Element
    {

        private int number = 1;
        private ArpeggiateDirectionEnum arpeggiateDirection = ArpeggiateDirectionEnum.undefined;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ArpeggiateElement()
        {
        }

        private ArpeggiateElement(XmlNode node)
        {
            if (0 != node.ChildNodes.Count)
            {
                Model.Log(string.Format("ArpeggiateElement: Unexpected child nodes found)"));
            }

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "number":
                        Utilities.Parse(a.Value, ref number, 0, 10, "ArpeggiateElement.number", true); break;
                    case "direction":
                        switch (a.Value)
                        {
                            case "up": arpeggiateDirection = ArpeggiateDirectionEnum.up; break;
                            case "down": arpeggiateDirection = ArpeggiateDirectionEnum.down; break;
                            default: Model.Log(string.Format("ArpeggiateElement: Unknown value for attribute 'direction': {0}", a.Value)); break;
                        }
                        break;
                    default: Model.Log(string.Format("ArpeggiateElement: Unknown attribute: {0}", a.Name)); break;
                }
            }
        }


        public static ArpeggiateElement Create(XmlNode node)
        {
            return new ArpeggiateElement(node);
        }


        public override string ToString()
        {
            return string.Format("Brudt");
        }
    }
}
