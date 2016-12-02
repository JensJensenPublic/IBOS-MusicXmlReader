
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
        private int measureNumber =-1 ;
        private int beats = -1 ;      // Derived from (latest) TimeElement
        private int beatType = -1;    // Derived from (latest) TimeElement
        private int fifths = -1;      // Derived from (latest) KeyElement
        private ModeEnum mode = ModeEnum.unknown;  // Derived from (latest) KeyElement
        private int tempo = -1;       // Derived from (latest) SoundElement

        #region encapsulation
        public int MeasureNumber
        {
            get
            {
                return measureNumber;
            }

            set
            {      
                LogChange("MeasureNumber", measureNumber, value);
                measureNumber = value;
            }
        }

        public int Beats
        {
            get
            {
                return beats;
            }

            set
            { 
                LogChange("Beats", beats, value);
                beats = value;
            }
        }

        public int BeatType
        {
            get
            {
                return beatType;
            }

            set
            {     
                LogChange("BeatType", beatType, value);
                beatType = value;
            }
        }

        public int Fifths
        {
            get
            {
                return fifths;
            }

            set
            {    
                LogChange("Fifths", fifths, value);
                fifths = value;
            }
        }

        public ModeEnum Mode
        {
            get
            {
                return mode;
            }

            set
            { 
                LogChange("Mode", mode, value);
                mode = value;
            }
        }

        public int Tempo
        {
            get
            {
                return tempo;
            }

            set
            {  
                LogChange("Tempo", tempo, value);
                tempo = value;
            }
        }

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


        #endregion // Encapsulation

        // NOTE!!! Do not forget to add to the Copy-constructor !!!

        // Prevent construction
        private StatusInformation()
        { }


        private StatusInformation (StatusInformation statusInformation)
        {            
            StatusInformation newStatusInformation = new StatusInformation(); // Create a new one
            newStatusInformation.measureNumber = statusInformation.measureNumber; // Fill in 
            newStatusInformation.beats = statusInformation.beats;       // Derived from (latest) TimeElement
            newStatusInformation.beatType = statusInformation.beatType; // Derived from (latest) TimeElement
            newStatusInformation.fifths = statusInformation.fifths;     // Derived from (latest) KeyElement
            newStatusInformation.mode = statusInformation.mode;         // Derived from (latest) KeyElement
            newStatusInformation.tempo = statusInformation.tempo;       // Derived from (latest) SoundElement

             // NOTE!!! Do not forget to add to the Copy-constructor HERE !!
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
