using System;

namespace BrailleMusicDecoder.MusicXmlHandlers
{

    /// <summary>
    /// Handles the "Octave" Music Braille concept in Interval notation context.
    /// NoteOctaveHandler does the same thing in normal "Note" notation context.
    /// </summary>
    public class IntervalOctaveHandler
    {
        public const int undefined = -1;
        int currentChordOctaveNumber = undefined;
        public int CurrentChordOctaveNumber { get { return currentChordOctaveNumber; } }

        public void OnAnyInput(InputCategoryEnum inputCategory)
        {
            switch (inputCategory)
            {
                // Only 3 categories do not clear currentChordOctave
          
                case InputCategoryEnum.Interval: return;
                case InputCategoryEnum.Accidental: return;
                case InputCategoryEnum.Octave: return; // The specific action is taken in the OnOctave() method
                case InputCategoryEnum.Note: return;  // The specific action is taken in the OnNote() method
                default: break; 
            }
            currentChordOctaveNumber = undefined; return;
        }

        public void OnInterval()
        {
            // Explicitly no action: 
        }

        public void OnNote()
        {
            currentChordOctaveNumber = undefined;
        }


        /// <summary>
        /// Sets up a specific octave number for the rest of this chord
        /// </summary>
        /// <param name="octaveNumber"></param>
        public void OnOctave(int octaveNumber)
        {
            this.currentChordOctaveNumber = octaveNumber;
        }

        private IntervalOctaveHandler()
        { }

        public static IntervalOctaveHandler Create()
        {
            return new IntervalOctaveHandler();
        }
    }
}
