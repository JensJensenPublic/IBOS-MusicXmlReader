using System.Collections.Generic;
using NAudio.Midi;

namespace JSJ.MusicSynthesis
{
    public class MidiChord
    {

        private List<MidiNote> midinotes = new List<MidiNote>();

        /// <summary>
        /// Constructor
        /// Does NOT start playing the chord.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="octave"></param>
        /// <param name="velocity"></param>
        /// <param name="chordType"></param>
        public MidiChord(ChromaticStep step, int octave,  int velocity, ChordType chordType)
        {     
            byte midicode = (byte)(12 * (octave + 1) + (byte)step);
            switch (chordType)
            {
                case ChordType.Major: 
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    break;
                case ChordType.Minor:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    break;
                case ChordType.Major7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Minor7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Major7maj:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSeventh));
                    break;
                case ChordType.Minor7maj:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSeventh));
                    break;
                case ChordType.Dim:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    break;
                case ChordType.Sus4:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                default:
                    throw new System.ArgumentException(string.Format("Chordtype {0} is not supported", chordType.ToString()));  
            }
        }

        /// <summary>
        /// Starts playing the chord on the device specified
        /// </summary>
        /// <param name="midiOut"></param>
        public void StartPlaying(MidiOut midiOut)
        {
            foreach (MidiNote midiNote in midinotes)
            {
                midiNote.StartPlaying(midiOut);
            }
        }

        /// <summary>
        /// Starts playing the chord on the device specified
        /// </summary>
        /// <param name="midiOut"></param>
        public void StopPlaying(MidiOut midiOut)
        {
            foreach (MidiNote midiNote in midinotes)
            {
                midiNote.StopPlaying(midiOut);
            }
        }
    }
}
