using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
{
    class SoundElement : EventElement
    {
        private string tempo = "";    // Quarter notes per minute  

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
            return string.Format("{0} {1}",ResourcesForModel.SoundElement_Tempo, tempo);
        }

        /// <summary>
        /// Quarter notes per minute.
        /// This implementation uses 60 as a default
        /// </summary>
        /// <returns></returns>
        public int GetTempo()
        {
            return string.IsNullOrEmpty(tempo) ? 60 : int.Parse(tempo); // Use 60 quarter notes per minute as default
        }
    }
}
