using System;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Handles the "Octave" Music Braille concept in normal "Note" notation context.
    /// IntervalOctaveHandler does the same thing in interval notation context.
    /// </summary>
    class NoteOctaveHandler
    {
        private int currentOctave;
        public int CurrentOctave { get { return currentOctave; } }
        private const int undefined = -1;

        private int pendingoctaveNumber = undefined;

        public void OnOctaveNumber(int octaveNumber)
        {
            // We do not tnow yet if this is an octavenumber for notes or an octavenumber for chords!
            this.pendingoctaveNumber = octaveNumber;
        }

        public void OnInterval()
        {
            // THe pending octave number was ment for an interval, not for a note.
            pendingoctaveNumber = undefined;
        }

        public void OnNote()
        {
            if (undefined == pendingoctaveNumber) return;
            ExplicitSetOctave(pendingoctaveNumber);
            pendingoctaveNumber = undefined;
        }


        private int currentSemitonesWithinOctave;  
        private void ExplicitSetOctave(int octave)
        {
            currentOctave = octave;
            currentSemitonesWithinOctave = undefined; //  The value of currentOctave was set explicitly
        }

        public int GetSemiTonesWithinOctave(string noteName)
        {
            switch (noteName.ToUpper())
            {
                case "C": return 0;
                case "D": return 2;
                case "E": return 4;
                case "F": return 5;
                case "G": return 7;
                case "A": return 9;
                case "B": return 11;
            }
            throw new Exception(string.Format("GetSemiTonesWithinOctave({0})",noteName));
        }

        private const int third = 4;
        private const int fifth = 7;



        public int GetOctave(int semitonesWithinOctave)
        {
            bool error = false;
            int result = currentOctave;
            if (currentSemitonesWithinOctave == undefined)
            {
                // CurrentOctave has just been set. So it is still valid
            }
            else
            {
                // CurrentOctave has not just been set. This means that: 
                // Either The notes are within the same  octave  and they differ by <= 6 semitones (a fifth) Looks like NOTA uses 7 / enlarged fifth !
                // Or     The notes are within different octaves and they differ by <= 4 semitones (a third)
                int dif = semitonesWithinOctave - currentSemitonesWithinOctave;
                // First assume that they are within the same octave
                if (Math.Abs(dif) <= fifth)
                {
                    // Nothing to do result is already set to currentOctave
                }
                else
                {
                    if (dif > 0)
                    {
                        // We must step down into the next octave and check that we are then inside the limit
                        if (Math.Abs(dif - 12) <= third)
                        {
                            result = (currentOctave - 1);
                        }
                        else
                        {
                            error = true;
                        }

                    }
                    else
                    {
                        // We must step up into the next octave and check that we are then inside the limit
                        if (Math.Abs(dif + 12) <= third)
                        {
                            result = (currentOctave + 1);
                        }
                        else
                        {
                            error = true;
                        }

                    }

                }

                if (error)
                {
                    throw new Exception(string.Format("GetOctave({0}) currentSemitonesWithinOctave={1} CurrentOctave={2}", semitonesWithinOctave, currentSemitonesWithinOctave, currentOctave));
                }



            }
            currentSemitonesWithinOctave = semitonesWithinOctave;
            currentOctave = result;
            return result;

        }



        private NoteOctaveHandler()
        { }

        public static NoteOctaveHandler Create()
        {
            return new NoteOctaveHandler();
        }
    }
}
