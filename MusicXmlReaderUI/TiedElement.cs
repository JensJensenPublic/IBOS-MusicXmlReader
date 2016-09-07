using System.Xml;

namespace MusicXmlReaderUI
{
    // The tied type represents the notated tie. The tie element represents the tie sound.
    class TiedElement : StartStopContinueElement
    {
        static private int nextId; // Only used for logging

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TiedElement(XmlNode node, int id) : base(node,id)
        {
        }


        public static TiedElement Create(XmlNode node)
        {
            return new TiedElement(node, nextId++);
        }


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("Bindebue {0} {1}", number, Localize(this.StartStopContinueType));
        }
    }
}

