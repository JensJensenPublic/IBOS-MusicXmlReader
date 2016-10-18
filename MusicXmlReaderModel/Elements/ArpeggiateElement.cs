using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
{
    enum ArpeggiateDirectionEnum {undefined, up, down}; 

    class ArpeggiateElement : Element
    {

        private int number = 1;
        private ArpeggiateDirectionEnum arpeggiateDirection = ArpeggiateDirectionEnum.undefined;

        internal ArpeggiateDirectionEnum ArpeggiateDirection
        {
            get
            {
                return arpeggiateDirection;
            } 
        }

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
                Logger.Log(string.Format("ArpeggiateElement: Unexpected child nodes found)"));
            }

            arpeggiateDirection = ArpeggiateDirectionEnum.up; // MusicXml default value 

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
                            default: Logger.LogOnce(string.Format("ArpeggiateElement: Unknown value for attribute 'direction': {0}", a.Value)); break;
                        }
                        break;
                    // Explicitly ignore:
                    case "default-x": break;
                    case "default-y": break;
                    default: Logger.Log(string.Format("ArpeggiateElement: Unknown attribute: {0}", a.Name)); break;
                }
            }
        }


        public static ArpeggiateElement Create(XmlNode node)
        {
            return new ArpeggiateElement(node);
        }

        public override string ToString() 
        {
            switch (arpeggiateDirection)
           {
                case ArpeggiateDirectionEnum.undefined: return ResourcesForModel.ArpeggiateElement_upwards; // "Brudt opad"; // "up" is the dominating default
                case ArpeggiateDirectionEnum.down: return ResourcesForModel.ArpeggiateElement_downwards; //"Brudt nedad";
                case ArpeggiateDirectionEnum.up: return ResourcesForModel.ArpeggiateElement_upwards; //"Brudt opad";
                default: return ""; 
            }
        }
    }
}
