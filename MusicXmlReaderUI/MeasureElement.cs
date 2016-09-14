using System.Xml;

namespace MusicXmlReaderUI
{
    public class MeasureElement : EventElement
    {

        private int number = 0;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private MeasureElement()
        { }

        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private MeasureElement(XmlNode node)
        {

            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "number":
                        number = 0;
                        Utilities.Parse(a.Value, ref number, 0, int.MaxValue, "MeasureElement.Number", false);
                        break;
                }
            }            
        }

        public int Number
        {
            get
            {
                return number;
            }
        }

        public static MeasureElement Create(XmlNode node)
        {
            return new MeasureElement(node);
        }

        public override string ToString()
        {
            return null;
            // string s = string.Format("Takt {0}", number);
            // return s;
        }
    }
}
