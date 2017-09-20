using System.Xml;

namespace MusicXmlReaderModel
{

    public enum ModeEnum {unknown,minor,major};

    public class KeyElement : EventElement
    {
        string localizedKey = "";
        string localizedmode = "";

        private int fifths = 0;
        private ModeEnum mode;


        /// <summary>
        /// Returns the position on the circle of fifts:
        /// -1 F major / D minor
        ///  0 C major / A minor 
        /// +1 G major / E minor
        /// </summary>
        public int Fifths
        {
            get
            {
                return fifths;
            } 
        }

        public ModeEnum Mode
        {
            get
            {
                return mode;
            }
        }


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
        private string LocalizeMajorKey(int k) 
        {
            switch (k)
            {
                case 0 : return ResourcesForModel.PitchElement_c; // "C";
                case 1 : return ResourcesForModel.PitchElement_g; // "G";
                case 2 : return ResourcesForModel.PitchElement_d; // "D";
                case 3 : return ResourcesForModel.PitchElement_a; // "A";
                case 4 : return ResourcesForModel.PitchElement_e; // "E";
                case 5 : return ResourcesForModel.PitchElement_b; // "H";
                case 6 : return ResourcesForModel.PitchElement_fSharp; // "Fis";
                case 7 : return ResourcesForModel.PitchElement_cSharp; // "Cis";
                case -1: return ResourcesForModel.PitchElement_f; // "F";
                case -2: return ResourcesForModel.PitchElement_Bb; // "Bb";
                case -3: return ResourcesForModel.PitchElement_eFlat; // "Es";
                case -4: return ResourcesForModel.PitchElement_aFlat; // "As";
                case -5: return ResourcesForModel.PitchElement_dFlat; // "Des";
                case -6: return ResourcesForModel.PitchElement_gFlat; // "Ges";
                case -7: return ResourcesForModel.PitchElement_b; //"H";
            }
            return "";
        }

        /// <summary>
        /// Convert from position on the circle of fifths to an (audible) node name
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private string LocalizeMinorKey(int k)
        {
            switch (k)
            {
                case 0: return ResourcesForModel.PitchElement_a; // "A";
                case 1: return ResourcesForModel.PitchElement_e; // "E";
                case 2: return ResourcesForModel.PitchElement_b; // "H";
                case 3: return ResourcesForModel.PitchElement_fSharp; // "Fis";
                case 4: return ResourcesForModel.PitchElement_cSharp; // "Cis";
                case 5: return ResourcesForModel.PitchElement_gSharp; // "G#";
                case 6: return ResourcesForModel.PitchElement_dSharp; // "D#";
                case 7: return ResourcesForModel.PitchElement_Bb; // Bb";
                case -1: return ResourcesForModel.PitchElement_d; // "D";
                case -2: return ResourcesForModel.PitchElement_g; // "G";
                case -3: return ResourcesForModel.PitchElement_c; // "C";
                case -4: return ResourcesForModel.PitchElement_f; // "F";
                case -5: return ResourcesForModel.PitchElement_Bb; // "Bb";
                case -6: return ResourcesForModel.PitchElement_eFlat; // "Es";
                case -7: return ResourcesForModel.PitchElement_aFlat; //"Ab";
            }
            return "";
        }





        private string LocalizeMode(ModeEnum mode) 
        {
            string functionName = "LocalizeMode";
            switch (mode)
            {
                case ModeEnum.major: return ResourcesForModel.KeyElement_major; // "dur";
                case ModeEnum.minor: return ResourcesForModel.KeyElement_minor; // "mol";
                default:
                    Logger.Log(string.Format("{0}: Illegal value for mode={1}", functionName,mode.ToString() )); break;
            }
            return "";
        }        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private KeyElement(XmlNode node)
        {
            const string functionName = "KeyElement()";
            //string fifths = "";
            //string mode = "";
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "fifths": Utilities.Parse(n.InnerText, ref fifths, -7, +7, functionName,false); break;  
                    case "mode":    switch (n.InnerText)
                        {
                            case "minor": mode = ModeEnum.minor; break;
                            case "major": mode = ModeEnum.major; break;
                            default:
                                Logger.Log(string.Format("{0}: node={1} has illegal value={2}", functionName, n.Name, n.InnerText)); break;

                        }
                        break;
                }
            }

            switch (mode)
            {
                case ModeEnum.major:
                    localizedKey = LocalizeMajorKey(fifths) + LocalizeMode(mode);
                    localizedmode = LocalizeMode(mode);
                    break;

                case ModeEnum.minor:
                    localizedKey = LocalizeMinorKey(fifths) + LocalizeMode(mode);
                    localizedmode = LocalizeMode(mode);
                    break;

                default:
                    // We do not know if this is Major or Minor, so we must shos both possibilities
                    string localizedOr = " " + ResourcesForModel.KeyElement_Or + " ";
                    localizedKey = LocalizeMajorKey(fifths) + LocalizeMode(ModeEnum.major) + localizedOr + LocalizeMinorKey(fifths) + LocalizeMode(ModeEnum.minor);
                    localizedmode = "?";
                    break;
            }
        }

        public static KeyElement Create(XmlNode node)
        {
            return new KeyElement(node);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}",ResourcesForModel.KeyElement_key, localizedKey);
        }

        public string ToShortString()
        {      
            if (Fifths > 0) return Fifths.ToString() + "#";
            if (Fifths < 0) return (0-Fifths).ToString() + "b";
            return "0#b";
        }
    } 
}
