using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-sound.htm

    class SoundElement : EventElement
    {
        private const string className = "SoundElement";
        //private string tempo = "";    // Tempo is expressed in quarter notes per minute.
        //                              //  If 0, the sound-generating program should prompt the user at the time of compiling a sound (MIDI) file. 
        private bool damperPedal;
        private float dynamics = 100; // Dynamics(or MIDI velocity) are expressed as a percentage of the default forte value(90 for MIDI 1.0).
        private float tempoValue = 60;  // Tempo is expressed in quarter notes per minute. Use 60 as default
        private bool  tempoValid;        // This Soundelement contains a valid tempo

        public float TempoValue
        {
            get
            {
                return tempoValue;
            }
        }

        public bool DamperPedal
        {
            get
            {
                return damperPedal;
            }
        }

        public bool TempoValid
        {
            get
            {
                return tempoValid;
            }
        }

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
            string functionName = "SoundElement";
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "tempo": tempoValid = Utilities.Parse(a.Value, ref tempoValue, (float)0, (float)float.MaxValue, "SoundElement: Invalid value of tempo"); break; // No upper limit
                    case "damper-pedal": Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref damperPedal);
                        Logger.LogOnce(string.Format("{0}.{1} Damper Pedal = {2}", className, functionName, a.Value));
                        break;
                    case "dynamics":
                        // Logger.LogOnce(string.Format("{0}.{1} Dynamics = {2}", className, functionName, a.Value));
                        Utilities.Parse(a.Value, ref dynamics, (float)0, (float) float.MaxValue, "SoundElement: Invalid value of dynamics"); // No upper limit
                        // Logger.LogOnce(string.Format("{0}.{1} Dynamics = {2}", className, functionName, dynamics.ToString()));
                        Logger.LogOnce(string.Format("{0}.{1} Dynamics", className, functionName)); // Avoid polluting the logfile
                        break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unsupported attribute. Name='{2}' Value= '{3}'", className, functionName, a.Name, a.Value));
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
            return string.Format("{0}:{1}",ResourcesForModel.SoundElement_Tempo, tempoValue);
        }

        ///// <summary>
        ///// Quarter notes per minute.
        ///// This implementation uses 60 as a default
        ///// </summary>
        ///// <returns></returns>
        //public int GetTempo()
        //{
        //    //int iTempo = 60;
        //    float fTempo = (float)60;
        //    if (!string.IsNullOrEmpty(tempoValue))         
        //    {
        //        if (!float.TryParse(tempoValue, out fTempo))
        //        {
        //            //Logger.LogOnce 
        //        } 
        //    }
        //    int iTempo = (int)fTempo;
        //    return iTempo; 
        //    //return string.IsNullOrEmpty(tempo) ? 60 : int.Parse(tempo); // Use 60 quarter notes per minute as default
        //}

        ///// <summary>
        /////  Quarter notes per minute.
        ///// </summary>
        ///// <returns></returns>
        //public string ToLocalizedTempo()
        //{
        //    int tempo = GetTempo();
        //    return string.Format("{0}:{1}",ResourcesForModel.SoundElement_Tempo, tempo.ToString());
        //}

    }
}
