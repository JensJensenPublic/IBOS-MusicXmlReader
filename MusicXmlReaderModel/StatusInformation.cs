
namespace MusicXmlReaderModel
{
    /// <summary>
    /// Classs for holding all "state-like" status information, which is valid for a part of the score.
    /// Only values covering all parts are implemented.
    /// </summary>
    public class StatusInformation
    {
        private string className = "StatusInformation";
        // Use -1 as a marker for "unknown"
        //private int measureNumber =-1 ;
        //private int beats = -1 ;      // Derived from (latest) TimeElement
        //private int beatType = -1;    // Derived from (latest) TimeElement
        //private int fifths = -1;      // Derived from (latest) KeyElement
        //private ModeEnum mode = ModeEnum.unknown;  // Derived from (latest) KeyElement
        //private int tempo = -1;       // Derived from (latest) SoundElement


        private MeasureElement  currentMeasureElement;
        private KeyElement currentKeyElement;
        private SoundElement currentSoundElement;
        private TimeElement currentTimeElement;
        private HarmonyElement currentHarmonyElement;

        #region encapsulation
        public MeasureElement CurrentMeasureElement
        {
            get
            {
                return currentMeasureElement;
            }

            set
            {
                LogChange("MeasureNumber", (null == currentMeasureElement) ? -1 : currentMeasureElement.Number, value.Number);
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
                LogChange("Fifths", (null == currentKeyElement) ? -1 :  currentKeyElement.Fifths, value.Fifths);
                LogChange("Mode",   (null == currentKeyElement) ? ModeEnum.unknown : currentKeyElement.Mode, value.Mode);
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
                LogChange("Tempo", (null == currentSoundElement) ? -1 :currentSoundElement.GetTempo(), value.GetTempo());
                currentSoundElement = value;
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
                LogChange("Beats",    (null == currentTimeElement) ? -1 : currentTimeElement.Beats, value.Beats);
                LogChange("BeatType", (null == currentTimeElement) ? -1 : currentTimeElement.BeatType, value.BeatType);
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
                LogChange("Harmony", (null == currentHarmonyElement) ? "" : currentHarmonyElement.ToString(), value.ToString());
                currentHarmonyElement = value;
            }
        }



        //public int MeasureNumber
        //{
        //    get
        //    {
        //        return measureNumber;
        //    }

        //    set
        //    {      
        //        LogChange("MeasureNumber", measureNumber, value);
        //        measureNumber = value;
        //    }
        //}

        //public int Beats
        //{
        //    get
        //    {
        //        return beats;
        //    }

        //    set
        //    { 
        //        LogChange("Beats", beats, value);
        //        beats = value;
        //    }
        //}

        //public int BeatType
        //{
        //    get
        //    {
        //        return beatType;
        //    }

        //    set
        //    {     
        //        LogChange("BeatType", beatType, value);
        //        beatType = value;
        //    }
        //}

        //public int Fifths
        //{
        //    get
        //    {
        //        return fifths;
        //    }

        //    set
        //    {    
        //        LogChange("Fifths", fifths, value);
        //        fifths = value;
        //    }
        //}

        //public ModeEnum Mode
        //{
        //    get
        //    {
        //        return mode;
        //    }

        //    set
        //    { 
        //        LogChange("Mode", mode, value);
        //        mode = value;
        //    }
        //}

        //public int Tempo
        //{
        //    get
        //    {
        //        return tempo;
        //    }

        //    set
        //    {  
        //        LogChange("Tempo", tempo, value);
        //        tempo = value;
        //    }
        //}

        #endregion // Encapsulation

        private void LogChange(string attrubuteName, int oldValue, int newValue)
        {
            if (newValue == oldValue) return;
            // Logger.Log(string.Format("{0}.{1}: changed to {3}", className, attrubuteName, oldValue, newValue));
        }

        private void LogChange(string attrubuteName, ModeEnum oldValue, ModeEnum newValue)
        {
            if (newValue == oldValue) return;
            // Logger.Log(string.Format("{0}.{1}: changed to {3}", className, attrubuteName, oldValue.ToString(), newValue.ToString()));
        }

        private void LogChange(string attrubuteName, string oldValue, string newValue)
        {
            if (0 == string.Compare(newValue,oldValue)) return;
            // Logger.Log(string.Format("{0}.{1}: changed to {3}", className, attrubuteName, oldValue, newValue));
        }




        // NOTE!!! Do not forget to add to the Copy-constructor !!!

        // Prevent construction
        private StatusInformation()
        { }


        private StatusInformation (StatusInformation statusInformation)
        {            
            //StatusInformation newStatusInformation = new StatusInformation(); // Create a new one
            //this.measureNumber = statusInformation.measureNumber; // Fill in 
            //this.beats = statusInformation.beats;       // Derived from (latest) TimeElement
            //this.beatType = statusInformation.beatType; // Derived from (latest) TimeElement
            //this.fifths = statusInformation.fifths;     // Derived from (latest) KeyElement
            //this.mode = statusInformation.mode;         // Derived from (latest) KeyElement
            //this.tempo = statusInformation.tempo;       // Derived from (latest) SoundElement

            this.currentMeasureElement  = statusInformation.currentMeasureElement;
            this.currentKeyElement      = statusInformation.currentKeyElement;
            this.currentSoundElement    = statusInformation.currentSoundElement;
            this.currentTimeElement     = statusInformation.currentTimeElement;
            this.currentHarmonyElement  = statusInformation.currentHarmonyElement;


            // NOTE!!! Do not forget to add to the Copy-constructor HERE !!
        }


        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",
                (null == currentMeasureElement) ? "?" : currentMeasureElement.ToString(), // 0
                (null == currentTimeElement) ? "?" : currentTimeElement.ToString(), //1
                (null == currentKeyElement) ? "?" : currentKeyElement.ToString(),        // 2
                (null == currentHarmonyElement) ? "?" : currentHarmonyElement.ToLocalizedString()); //3
                // (null == currentSoundElement) ? "?" : currentSoundElement.GetTempo().ToString()); // 4
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
