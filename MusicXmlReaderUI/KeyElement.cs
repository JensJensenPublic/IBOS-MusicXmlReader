using System.Xml;

namespace MusicXmlReaderUI
{

    public class KeyElement : EventElement
    {
        string localizedKey = "";
        string localizedmode = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private KeyElement()
        {            
        }


        /// <summary>
        /// Convert from position on the circle of fifths to an (audible) node name
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private string LocalizeKey(string k)
        {
            switch (k)
            {
                case "0" : return "C";
                case "1" : return "G";
                case "2" : return "D";
                case "3" : return "A";
                case "4" : return "E";
                case "5" : return "H";
                case "6" : return "Fis";
                case "7" : return "Cis";
                case "-1": return "F";
                case "-2": return "Bb";
                case "-3": return "Es";
                case "-4": return "As";
                case "-5": return "Des";
                case "-6": return "Ges";
                case "-7": return "H";
            }
            return "";
        }

        private string LocalizeMode(string mode)
        {
            switch (mode)
            {
                case "major": return "dur";
                case "minor": return "mol";
            }
            return "";
        }        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private KeyElement(XmlNode node)
        {
            string fifths = "";
            string mode = "";
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "fifths": fifths = n.InnerText; break;
                    case "mode":    mode = n.InnerText; break;
                }
            }

            localizedKey  = LocalizeKey(fifths);
            localizedmode = LocalizeMode(mode);
        }

        public static KeyElement Create(XmlNode node)
        {
            return new KeyElement(node);
        }

        public override string ToString()
        {
            return string.Format("Toneart:{0}{1} ", localizedKey, localizedmode);
        }
    } 
}
