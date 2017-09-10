using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{
    public class DetailsDescription
    {
        private string stringRepresentation = "";
        private HarmonyElement harmonyElement;
        private EventDescription eventDescription;
        private ChromaticStep step = ChromaticStep.NumberOfSteps; // The value meaning "no step"
        private int octave;

        public bool ContainsStep
        {
            get
            {
                return (step != ChromaticStep.NumberOfSteps);
            }
        }

        public ChromaticStep Step
        {
            get
            {
                return step;
            }
        }

        public int Octave
        {
            get
            {
                return octave;
            }
        }

        public HarmonyElement HarmonyElement
        {
            get
            {
                return harmonyElement;
            }

        }

        public override string ToString()
        {
            return stringRepresentation;
        }

        private DetailsDescription(string s, HarmonyElement harmonyElement)
        {
            stringRepresentation = s;
            this.harmonyElement = harmonyElement;
        }

        private DetailsDescription(string s, ChromaticStep pitchRepresentation, int octave)
        {
            this.step = pitchRepresentation;
            this.octave = octave;
            stringRepresentation = s;
        }

        private DetailsDescription(string s, EventDescription eventDescription)
        {
            stringRepresentation = s;
            this.eventDescription = eventDescription;
        }

        private DetailsDescription(string s)
        {
            stringRepresentation = s;
        }


        /// <summary>
        /// USed for Harmony details
        /// </summary>
        /// <param name="s"></param>
        /// <param name="harmonyElement"></param>
        /// <returns></returns>
        public static DetailsDescription Create(string s, HarmonyElement harmonyElement)
        {
            return new DetailsDescription(s, harmonyElement);
        }

        /// <summary>
        /// Used for Harmony details (root and bass)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pitchRepresentation"></param>
        /// <returns></returns>
        public static DetailsDescription Create(string s, ChromaticStep pitchRepresentation, int octave)
        {
            return new DetailsDescription(s, pitchRepresentation, octave);
        }

        /// <summary>
        /// USed for event details
        /// </summary>
        /// <param name="s"></param>
        /// <param name="eventDescription"></param>
        /// <returns></returns>
        public static DetailsDescription Create(string s, EventDescription eventDescription)
        {
            return new DetailsDescription(s,eventDescription);
        }

        public static DetailsDescription Create(string s)
        {
            return new DetailsDescription(s);
        }


        // Add more constructors here to implement other details representations !

    }
}
