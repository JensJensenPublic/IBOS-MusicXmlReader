
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
        private MetronomeElement currentMetronomeElement;
        private MeasureFraction currentMeasureFraction;

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
                bool verbose = false; // Change to true during serious debugging !
                if (value.TempoValid)
                {
                    // Only change currentSoundElement if the new one contains a valid value
                    if (verbose)
                    {
                        LogChange("Tempo", (null == currentSoundElement) ? -1 : (int)currentSoundElement.TempoValue, (int)value.TempoValue);
                    }
                    else
                    {
                        // Logger.LogCFOnce(": Tempo changed");
                    }
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

        public MetronomeElement CurrentMetronomeElement
        {
            get
            {
                return currentMetronomeElement;
            }

            set
            {
                currentMetronomeElement = value;
            }
        }

        public MeasureFraction CurrentMeasureFraction
        {
            get { return currentMeasureFraction; }
            set { currentMeasureFraction = value; }
        }



        #endregion // Encapsulation

        private void LogChange(string attrubuteName, int oldValue, int newValue)
        {
            if (newValue == oldValue) return;
            Logger.Log(string.Format("{0}.{1}: {2} changed to {3}", className, attrubuteName, oldValue, newValue));
        }

        private void LogChange(string attrubuteName, ModeEnum oldValue, ModeEnum newValue)
        {
            if (newValue == oldValue) return;
            Logger.Log(string.Format("{0}.{1}: {2} changed to {3}", className, attrubuteName, oldValue.ToString(), newValue.ToString()));
        }

        private void LogChange(string attrubuteName, string oldValue, string newValue)
        {
            if (0 == string.Compare(newValue,oldValue)) return;
            Logger.Log(string.Format("{0}.{1}: {2} changed to {3}", className, attrubuteName, oldValue, newValue));
        }




        // NOTE!!! Do not forget to add to the Copy-constructor !!!

        // Prevent construction
        private StatusInformation(float tempo)
        {
            this.CurrentSoundElement = SoundElement.Create(tempo);
        }


        private StatusInformation (StatusInformation statusInformation)
        {            

            this.currentMeasureElement  = statusInformation.currentMeasureElement;
            this.currentKeyElement      = statusInformation.currentKeyElement;
            this.currentSoundElement    = statusInformation.currentSoundElement;
            this.currentTimeElement     = statusInformation.currentTimeElement;
            this.currentHarmonyElement  = statusInformation.currentHarmonyElement;
            this.currentMetronomeElement = statusInformation.currentMetronomeElement;

            // NOTE!!! Do not forget to add to the Copy-constructor HERE !!
        }

        private string GetMetronomeString()
        {
            if (null == currentMetronomeElement)
            {
                return "";
            }
            return currentMetronomeElement.BeatsPerMinuteInt.ToString();
        }


        public string GetTempoString()
        {
            string functionName = "GetTempoString";
            string tempo = null;            

            if (null != currentMetronomeElement)
            {
                // As default use the tempo specified in the MetronomeElement
                tempo = currentMetronomeElement.ToString();
            }

            if (null != currentSoundElement)
            {
                // Use the tempo specified in the soundElement if it exists
                tempo = currentSoundElement.ToString();
            }

            if (null == tempo)
            {
                // If neither a metronomeElement nor a SoundElement exist just return blank
                return "";
            }
                
            // USe the local "tempo" 
                                 
            string result = "";
            if (100 == currentTempoModification) 
            {
                result = tempo;
            }
            else
            {
                string resultingTempo = "";
                try
                {
                    int originalTempo = currentSoundElement.TempoValueAsInt;
                    float modifiedTempo = originalTempo * currentTempoModification / 100;
                    resultingTempo = string.Format("={0}", modifiedTempo); // efine number of decimals to 0
                }
                catch (System.Exception e)
                {
                    Logger.Log(string.Format("{0}.{1} failed to compute resulting tempo. Message={2}", className,functionName,e.Message));

                }
                result = string.Format("{0}*{1}%{2} ", tempo, currentTempoModification.ToString(), resultingTempo);
            }
            return result;
        }

        private string Format(string elementType, Element element)
        {
            if (null == element)
            {
                Logger.LogCFOnce(string.Format(": {0} == null",elementType));
                return "??";
            }
            return element.Caption + " " + ((null == element) ? "?" : element.ToString());
        }

        private string Format(string elementType,Element element, string s)
        {
            if (null == element)
            {
                Logger.LogCFOnce(string.Format(": {0} == null", elementType));
                return "??";
            }
            return element.Caption + " " + s;
        }

        private string Format(MeasureFraction fraction)
        {
            return  ((null == CurrentMeasureFraction) ? "?" : currentMeasureFraction.ToString()); 
        }

        /// <summary>
        /// Describes the format of the status information when shown in the status line
        /// Please compare to DetailsPlayer.Format()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = string.Format("{0} {1} {2} {3} {4} {5}",
                Format("MeasureElement",currentMeasureElement), // 0
                Format(currentMeasureFraction), // 1
                Format("TimeElement",currentTimeElement), // 2 
                Format("KeyElement",currentKeyElement), // 3
                (null == currentHarmonyElement) ? "" : Format("HarmonyElement",currentHarmonyElement), // An empty string  means "No Chord"
                Format("MetronomeElement",currentMetronomeElement, GetTempoString())); //5
            return s;
        }

        public static StatusInformation Create(float tempo)
        {
            return new StatusInformation(tempo);
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
