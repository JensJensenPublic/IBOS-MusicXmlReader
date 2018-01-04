using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Abstract base class for : 
    ///   StringDetailsDescription        Describing a simple text, without playing any sound  
    ///   NoteListDetailsDescription      Describing a list of notes, for instance all notes within a part
    ///   SingleNoteDetailsDescription    Describing a single note, for instance a note within a part
    ///   HarmonyStepDetailsDescription   Describing a single step within a harmony, including function "Third" and pitch "E" 
    ///   HarmonyDetailsDescription       Describing a full harmony, for instance C7
    /// </summary>
    public abstract class DetailsDescription
    {
        public abstract void Play(MusicPlayer musicPlayer); // Play the sound representation
        public abstract void Stop(MusicPlayer musicPlayer); // Stop playing the sound representation
        public override string ToString()                   // REturn the text representation
        {
            return stringRepresentation;
        }

        protected string stringRepresentation = ""; // The string to be reported as a sinble line in a listbox

        protected DetailsDescription(string s)
        { 
            stringRepresentation = s;
        }

        protected MidiNote GetMidiNote(NoteElement noteElement)
        {
            ChromaticStep chromaticStep = (ChromaticStep)noteElement.PitchValue.SemiToneWithinOctave(noteElement.Step);
            MidiNote midiNote = new MidiNote(chromaticStep, noteElement.Alter, noteElement.Octave, noteElement.Transpose, noteElement.DynamicsIntValue, noteElement.MidiChannel, null);
            return midiNote;
        }
        
        public static DetailsDescription Create(string s, HarmonyElement harmonyElement, int chordOctave)
        {
            return new HarmonyDetailsDescription(s, harmonyElement, chordOctave);
        }
        
        public static DetailsDescription Create(string s, ChromaticStep pitchRepresentation, int octave, int dynamics)
        {
            return new HarmonyStepDetailsDescription(s, pitchRepresentation, octave, dynamics);
        }

        public static DetailsDescription Create(string s, List<NoteElement> notes)
        {
            return new NoteListDetailsDescription(s, notes);
        }
 
        public static DetailsDescription Create(string s, NoteElement note)
        {
            return new SingleNoteDetailsDescription(s, note);
        }

        public static DetailsDescription Create(string s)
        {
            return new StringDetailsDescription(s);
        }


        //public static DetailsDescription Create(string s, EventDescription eventDescription)
        //{
        //    return new EventDetailsDescription(s, eventDescription);
        //}

    }

    /// *****************************************************************************
    /// Derived classes
    /// *****************************************************************************

    /// <summary>
    ///   Describes a simple text, without playing any sound 
    /// </summary>
    public class StringDetailsDescription : DetailsDescription
    {
        public StringDetailsDescription(string s) : base(s)
        {           
        }

        public override void Play(MusicPlayer musicPlayer)
        {
            // Nothing to start
        }

        public override void Stop(MusicPlayer musicPlayer)
        {
            // Nothing to stop
        }
    }


    /// <summary>
    ///  NoteListDetailsDescription      Describes a list of notes, for instance all notes within a part
    /// </summary>
    public class NoteListDetailsDescription : DetailsDescription
    {
        private List<NoteElement> notes;        // If this DetailsDescription represents a list of notes
        public List<NoteElement> Notes
        {
            get
            {
                return notes;
            }
        }

        public NoteListDetailsDescription(string s, List<NoteElement> notes) : base(s)
        {
            this.notes = notes;
        }

        public override void Play(MusicPlayer musicPlayer)
        {
            foreach (NoteElement noteElement in notes)
            {
                if (!noteElement.IsPause)
                {
                    MidiNote currentDetailsMidiNote = GetMidiNote(noteElement);
                    musicPlayer.StartMidiNote(currentDetailsMidiNote);
                }
            }
        }

        public override void Stop(MusicPlayer musicPlayer)
        {
            foreach (NoteElement noteElement in notes)
            {
                if (!noteElement.IsPause)
                {
                    MidiNote currentDetailsMidiNote = GetMidiNote(noteElement);
                    musicPlayer.StopMidiNote(currentDetailsMidiNote);
                }
            }
        }

    }


    /// <summary>
    /// Describes a single step within a harmony, including function "Third" and pitch "E" 
    /// </summary>
    public class HarmonyStepDetailsDescription : DetailsDescription
    {
        private ChromaticStep step = ChromaticStep.NumberOfSteps; // The value meaning "no step"
        private int octave;
        private int dynamics;
        MidiNote currentDetailsMidiNote;

        public HarmonyStepDetailsDescription(string s, ChromaticStep pitchRepresentation, int octave, int dynamics) : base(s)
        {
            this.step = pitchRepresentation;
            this.octave = octave;
            this.dynamics = dynamics;
        }

        public override void  Play(MusicPlayer musicPlayer)
        {   
            if (step != ChromaticStep.NumberOfSteps)
            {
                // This DetailDescription describes a single note
                currentDetailsMidiNote = new MidiNote(step,octave, dynamics, Interval.Unison);
                musicPlayer.StartMidiNote(currentDetailsMidiNote);
            }   
        }

        public override void Stop(MusicPlayer musicPlayer)
        {
            if (null == currentDetailsMidiNote) return;
            musicPlayer.StopMidiNote(currentDetailsMidiNote);
        }
    }

    /// <summary>
    /// SingleNoteDetailsDescription    Describes a single note, for instance a note within a part
    /// </summary>
    public class SingleNoteDetailsDescription : DetailsDescription
    {
        private NoteElement note;               // If this DetailsDescription represents a note
        MidiNote currentDetailsMidiNote;

        public SingleNoteDetailsDescription(string s, NoteElement note) : base(s)
        {
            this.note = note;
        }

        public override void Play(MusicPlayer musicPlayer)
        {
            if (null != note)
            {
                if (note.IsPause) return;
                // MidiNote currentDetailsMidiNote = new MidiNote((ChromaticStep)note.Step, note.Alter, note.Octave, note.Transpose, velocity, note.MidiChannel, null);
                currentDetailsMidiNote = GetMidiNote(note);
                musicPlayer.StartMidiNote(currentDetailsMidiNote);
            }
        }

        public override void Stop(MusicPlayer musicPlayer)
        {
            if (null == currentDetailsMidiNote) return;
            musicPlayer.StopMidiNote(currentDetailsMidiNote);
        }
    }


    /// <summary>
    /// Describes a full harmony, for instance C7
    /// </summary>
    public class HarmonyDetailsDescription : DetailsDescription
    {
        private int chordOctave; // Build the chord starting in 4. octave, possibly spreading into 5. octave
        private HarmonyElement harmonyElement;  // If this DetailsDescription represents a harmony
        private MidiChord currentDetailsMidiChord;

        public HarmonyDetailsDescription(string s, HarmonyElement harmonyElement, int chordOctave) : base(s)
        {
            this.harmonyElement = harmonyElement;
            this.chordOctave = chordOctave;
        }

        public override void Play(MusicPlayer musicPlayer)
        {
            HarmonyElement hE = harmonyElement;
            int dynamics = (int) Dynamics.mf; // Percentage of of max velocity, meaning "Forte" 75 meaning MezzoForte
            if (null != hE)
            {
                // This DetailDescription describes a harmony       
                currentDetailsMidiChord = new MidiChord(hE.ChromaticRootStep, chordOctave, dynamics, hE.ChordType, hE.ChromaticBassStep, null);
                musicPlayer.StartMidiChord(currentDetailsMidiChord);
            }
        }

        public override void Stop(MusicPlayer musicPlayer)
        {
            if (null == currentDetailsMidiChord) return;
            musicPlayer.StopMidiChord(currentDetailsMidiChord);
        }
    }


    // Add more constructors here to implement other details representations !

    //public class EventDetailsDescription : DetailsDescription
    //{
    //    private EventDescription eventDescription;

    //    public EventDetailsDescription(string s, EventDescription eventDescription) : base(s)
    //    {
    //        this.eventDescription = eventDescription;
    //    }

    //    public override Play(MusicPlayer musicPlayer)
    //    {

    //    }
    //}



}
