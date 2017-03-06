
namespace MusicXmlReaderModel
{
    /// <summary>
    /// Classs for holding all "state-like" status information, which is valid for a part of the score.
    /// Only values covering all parts are implemented.
    /// </summary>
    public class StatusInformation
    {
        private string className = "StatusInformation";
  
        private MeasureElement  currentMeasureElement;
        private KeyElement currentKeyElement;
        private SoundElement currentSoundElement;
        private TimeElement currentTimeElement;
        private HarmonyElement currentHarmonyElement;

        // The tempo modufication is defined from the client, not from the xml file
        // so it is modelled as a static variable and is ´referenced in the copy constructor! 
        private static int currentTempoModification = 100; // 100 %

        #region encapsulation
        public MeasureElement CurrentMeasureElement
        {
            get
            {
                return currentMeasureElement;
            }

            set
            {
                //LogChange("MeasureNumber", (null == currentMeasureElement) ? -1 : currentMeasureElement.Number, value.Number);
                currentMeasureElement = value; 
            }
        }

        public KeyElement CurrentKeyElement
        {
            get
            {
                return currentKeyElement;
            }

            set
            {
                //LogChange("Fifths", (null == currentKeyElement) ? -1 :  currentKeyElement.Fifths, value.Fifths);
                //LogChange("Mode",   (null == currentKeyElement) ? ModeEnum.unknown : currentKeyElement.Mode, value.Mode);
                currentKeyElement = value;
            }
        }

        internal SoundElement CurrentSoundElement
        {
            get
            {
                return currentSoundElement;
            }

            set
            {
                if (value.TempoValid)
                {
                    // Only change currentSoundElement if the new one contains a valid value
                    LogChange("Tempo", (null == currentSoundElement) ? -1 : (int) currentSoundElement.TempoValue, (int) value.TempoValue);
                    currentSoundElement = value;
                }
            }
        }

        public TimeElement CurrentTimeElement
        {
            get
            {
                return currentTimeElement;
            }

            set
            {
                //LogChange("Beats",    (null == currentTimeElement) ? -1 : currentTimeElement.Beats, value.Beats);
                //LogChange("BeatType", (null == currentTimeElement) ? -1 : currentTimeElement.BeatType, value.BeatType);
                currentTimeElement = value;
            }
        }

        public HarmonyElement CurrentHarmonyElement
        {
            get
            {
                return currentHarmonyElement;
            }
            set
            {
                //LogChange("Harmony", (null == currentHarmonyElement) ? "" : currentHarmonyElement.ToString(), value.ToString());
                currentHarmonyElement = value;
            }
        }

        public int CurrentTempoModification
        {
            get
            {
                return currentTempoModification;
            }

            set
            {
                currentTempoModification = value;
            }
        }

        #endregion // Encapsulation

        private void LogChange(string attrubuteName, int oldValue, int newValue)
        {
            if (newValue == oldValue) return;
            Logger.Log(string.Format("{0}.{1}: changed to {3}", className, attrubuteName, oldValue, newValue));
        }

        private void LogChange(string attrubuteName, ModeEnum oldValue, ModeEnum newValue)
        {
            if (newValue == oldValue) return;
            Logger.Log(string.Format("{0}.{1}: changed to {3}", className, attrubuteName, oldValue.ToString(), newValue.ToString()));
        }

        private void LogChange(string attrubuteName, string oldValue, string newValue)
        {
            if (0 == string.Compare(newValue,oldValue)) return;
            Logger.Log(string.Format("{0}.{1}: changed to {3}", className, attrubuteName, oldValue, newValue));
        }




        // NOTE!!! Do not forget to add to the Copy-constructor !!!

        // Prevent construction
        private StatusInformation()
        { }


        private StatusInformation (StatusInformation statusInformation)
        {            

            this.currentMeasureElement  = statusInformation.currentMeasureElement;
            this.currentKeyElement      = statusInformation.currentKeyElement;
            this.currentSoundElement    = statusInformation.currentSoundElement;
            this.currentTimeElement     = statusInformation.currentTimeElement;
            this.currentHarmonyElement  = statusInformation.currentHarmonyElement;

            // NOTE!!! Do not forget to add to the Copy-constructor HERE !!
        }

        private string GetTempoString()
        {
            string functionName = "GetTempoString";
            if (null == currentSoundElement)
            {
                return "";
            }

            string result = "";
            if (100 == currentTempoModification) 
            {
                result = currentSoundElement.ToString();
            }
            else
            {
                string resultingTempo = "";
                try
                {
                    int originalTempo = (int) currentSoundElement.TempoValue;
                    float modifiedTempo = originalTempo * currentTempoModification / 100;
                    resultingTempo = string.Format("={0}", modifiedTempo); // efine number of decimals to 0
                }
                catch (System.Exception e)
                {
                    Logger.Log(string.Format("{0}.{1} failed to compute resulting tempo. Message={2}", className,functionName,e.Message));

                }
                result = string.Format("{0}*{1}%{2} ", currentSoundElement.ToString(), currentTempoModification.ToString(), resultingTempo);
            }
            return result;
        }



        public override string ToString()
        {
            string s = string.Format("{0} {1} {2} {3} {4}",
                (null == currentMeasureElement) ? "?" : currentMeasureElement.ToString(), // 0
                (null == currentTimeElement) ? "?" : currentTimeElement.ToString(), //1
                (null == currentKeyElement) ? "?" : currentKeyElement.ToString(),        // 2
                (null == currentHarmonyElement) ? "" : currentHarmonyElement.ToLocalizedString(), //3 // Ignore the case where no narmony is found
                GetTempoString()); // 4 The string describing the tempo is more complex than the others and need a special mothod.
            return s;
        }

        public static StatusInformation Create()
        {
            return new StatusInformation();
        }


        /// <summary>
        /// Copy-constructor
        /// </summary>
        /// <param name="statusInformation"></param>
        /// <returns></returns>
        public static StatusInformation Create(StatusInformation statusInformation)
        {
            return new StatusInformation(statusInformation);
        }

    }
}
