using System.Xml;

namespace MusicXmlReaderUI
{
    class SoundElement : Element
    {
        private string tempo = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private SoundElement()
        { }
        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private SoundElement(XmlNode node)
        {

            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "tempo":
                        tempo = a.Value;
                        break;
                }
            }
        }

        public static SoundElement Create(XmlNode node)
        {
            return new SoundElement(node);
        }

        public override string ToString()
        {
            return string.Format("Tempo {0}", tempo);
        }

        public int GetTempo()
        {
            return string.IsNullOrEmpty(tempo) ? 60 : int.Parse(tempo); // Use 60 beats per second as default
        }
    }
}
