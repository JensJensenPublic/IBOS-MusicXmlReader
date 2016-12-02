
namespace MusicXmlReaderModel
{
    /// <summary>
    /// Classs for holding all "state-like" status information, which is valid for a part of the score.
    /// Only values covering all parts are implemented.
    /// </summary>
    class StatusInformation
    {
        // Use -1 as a marker for "unknown"
        private int measureNumber =-1 ;
        private int beats = -1 ;      // Derived from (latest) TimeElement
        private int beatType = -1;    // Derived from (latest) TimeElement
        private int fifths = -1;      // Derived from (latest) KeyElement
        private ModeEnum mode = ModeEnum.unknown;  // Derived from (latest) KeyElement
        private int Tempo = -1;       // Derived from (latest) SoundElement

        #region encapsulation
        public int MeasureNumber
        {
            get
            {
                return measureNumber;
            }

            set
            {
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
                mode = value;
            }
        }

        public int Tempo1
        {
            get
            {
                return Tempo;
            }

            set
            {
                Tempo = value;
            }
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
            newStatusInformation.Tempo = statusInformation.Tempo;       // Derived from (latest) SoundElement

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
