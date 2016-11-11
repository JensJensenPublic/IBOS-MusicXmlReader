using System;
using System.Collections.Generic;
using JSJ.MusicSynthesis;
using NAudio.Midi;
using MusicXmlReaderModel;


namespace MusicXmlReaderUI
{

    public interface IObjectCollection
    {
        void SetSelectedIndex(int index);
        int GetNumberOfObjects();
        object GetObjectAtIndex(int index);
    }

    public class MusicPlayer
    {

        MidiNote   latestNotePlayed = null;
        MidiChord latestHarmonyPlayed = null;
        MidiOut midiOut = null;
        IObjectCollection objects = null;
        // ListBox listBoxPoly = null;
        System.Diagnostics.Stopwatch stopWatch = null;
        //long nextActionTime;    // For autoplaying monophonic music 
        long firstStopWatchTime = -1;    // For autoplaying polyphonic music 
        int tempo = 60 ; // Quarter notes per minute.  Use 60 as a default
        int numberOfParts;
        int startIndex;
        List<MidiNote> notesCurrentlyPlaying; // Contains all notes currently playing. Used when playing is stopped by user

        

        long musicXmlTimeOffset  = 0; // Needed for handling change in Tempo while aulo-playing. Unit is the same as for Duration


        // User settings
        UserSettings userSettings;
   
        //float userSlowDown;


        /// <summary>
        /// Constructor
        /// </summary>
        public MusicPlayer(IObjectCollection objects, MidiOut midiOut)
        {
            this.midiOut = midiOut;
            this.objects = objects;

            // Temp start
            //MidiCommand midiCommand = new MidiCommand();
            //midiCommand.ChangeInstrument(19, midiOut); // 19 = Guitar
            // Temp end

            //ChangeInstrumentForAllChannels(20);
            //ChangeInstrument(1, 20);  // Change channel 1 to Church Organ
            //ChangeInstrument(1, 1);   // Change channel 1 to Grand Acoustic Piano
            //ChangeInstrument(1, 43);  // Change channel 1 to Cello
            //ChangeInstrument(1, 25);  // Change channel 1 to Acoustic Guitar


        }

        ///// <summary>
        ///// Simple impementation for changing instrument for all channels!
        ///// See instrument numbers at
        ///// https://en.wikipedia.org/wiki/General_MIDI
        ///// TO DO: refine as needed.
        ///// </summary>
        ///// <param name="instrument"></param>
        //public void ChangeInstrumentForAllChannels(int instrument)
        //{
        //    MidiCommand midiCommand = new MidiCommand();
        //    midiCommand.ChangeInstrument(instrument,this.midiOut); // 20 = Church Organ
        //}


        /// <summary>
        /// Simple implementation for changing instrument for a single midi channel
        /// See instrument numbers at
        /// https://en.wikipedia.org/wiki/General_MIDI
        /// </summary>
        /// <param name="channel">Must be an integer in [1..16]</param>
        /// <param name="instrument">Must be an integer in [1.127]</param>
        public void ChangeInstrument(int channel, int instrument)
        {
            Logger.Log(string.Format("MusicPlayer.ChangeInstrument(channel={0} instrument={1})", channel, instrument));
            MidiCommand midiCommand = new MidiCommand();
            midiCommand.ChangeInstrument(channel, instrument, this.midiOut);
        }


        /// <summary>
        /// Used when the playing manually.
        /// The User selects a note at a time. 
        /// or
        /// The user selects an EventDescription at a time. This may contain several notes to be played simultaneously
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="selectedObject"></param>
        public void SelectedIndexChanged(int selectedIndex, object selectedObject)
        {
            if (playing) return;
            if (null == selectedObject) return;
            if ((selectedObject is NoteElement))
            {
                NoteElement noteElement = selectedObject as NoteElement;
                if (noteElement.IsPause) return; // This is a pause
                // new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
            }
            else if ((selectedObject is EventDescription))
            {
                // Firat stop all notes currently playing:
                if (null != notesCurrentlyPlaying)
                {
                    foreach (MidiNote midiNote in notesCurrentlyPlaying)
                    {
                        midiNote.StopPlaying(midiOut);
                    }
                    notesCurrentlyPlaying.Clear();
                }

                EventDescription eventDescription = selectedObject as EventDescription;
                notesCurrentlyPlaying = new List<MidiNote>();
                foreach(List<NoteElement> noteElementList in eventDescription.NoteLists)
                {
                    foreach (NoteElement noteElement in noteElementList)
                    {
                        if (!noteElement.IsPause)
                        { // This is a real note, not a pause
                            if (userSettings.partsToPlay[noteElement.PartNumber])
                            {
                                MidiNote midiNote = new MidiNote(noteElement.Step.ToString(), noteElement.Alter, noteElement.Octave, 127, midiOut);
                                notesCurrentlyPlaying.Add(midiNote);
                            }
                        }
                    }
                }
            }
            return;
        }

        ///// <summary>
        ///// Simple timing-implementation:
        ///// </summary>
        ///// <param name="noteElement"></param>
        ///// <returns></returns>
        //private int Duration(NoteElement noteElement)
        //{
        //    float duration = noteElement.Duration;
        //    float divisions = noteElement.Divisions;
        //    float tempo = this.tempo;
        //    float durasionInUnitOfMeasures = duration / divisions;
        //    float durationInUnitOfMilliSeconds = userSettings.userSlowDown * 60 * 1000 * durasionInUnitOfMeasures / tempo;
        //    return (int)durationInUnitOfMilliSeconds;
        //}


        ///// <summary>
        ///// Play a single, monophonic note
        ///// </summary>
        ///// <param name="noteElement"></param>
        //private void Play(NoteElement noteElement)
        //{

        //    if (0 == nextActionTime)
        //    {
        //        // We  play the first note or pause immediately but remember when we did it.
        //        nextActionTime = stopWatch.ElapsedMilliseconds;
        //    }
        //    else
        //    {
        //        long sleep = nextActionTime - stopWatch.ElapsedMilliseconds;
        //        sleep = Math.Max(0, sleep); // Hack to avoid crash 
        //        System.Threading.Thread.Sleep((int)sleep);
        //    }

        //    if (!noteElement.TieStop)
        //    {
        //        if (null != latestNotePlayed)
        //        {
        //            latestNotePlayed.StopPlaying(midiOut);
        //        }

        //        if (!noteElement.IsPause)
        //        {
        //            // This is a playable note, not a pause !
        //            latestNotePlayed = new MidiNote(noteElement.Step.ToString(), noteElement.Alter, noteElement.Octave, 127, midiOut);
        //        }
        //    }
        //    nextActionTime += Duration(noteElement);
        //}


        public void Reset(int numberOfParts,int startIndex)
        {
            this.numberOfParts = numberOfParts;
            this.startIndex = (startIndex < 0) ? 0 : startIndex;
        }


        /// <summary>
        /// Computes the number of milliSeconds to wait for the next event to occur.
        /// Assumes that this.firstStopWatchTime and this.musicXmlTimeOffset are changed every time the Tempo is changed.
        /// Uses the following MusicPlayer member variables
        /// this.musicXmlTimeOffset: Time (in units of NoteElement.commonDivisions) since the latest change in Tempo
        /// this.tempo:              Current Tempo in number of quarter notes per minute
        /// this.stopWatch.ElapsedMilliseconds: Number of milliseconds since the stopWatch was started
        /// this.firstStopWatchTime: Time (in units of mS) of latest change in Tempo
        /// </summary>
        /// <param name="startTime">StartTime of the next event in units of NoteElement.commonDivisions</param>
        /// <returns>Number of MilliSeconds to wait.</returns>
        private int MilliSecondsToSleep(int startTime)
        {
            float mSPerMinute = 60000; // Used to conpensate for the use of different Units by the other variables
            float eventTimeInMilliSeconds = ((float)(startTime - this.musicXmlTimeOffset) * mSPerMinute) / ((float)NoteElement.commonDivisions * (float)this.tempo); 
            long sleep = ((long)eventTimeInMilliSeconds - (this.stopWatch.ElapsedMilliseconds - this.firstStopWatchTime));
            return (int)Math.Max(0, sleep);
        }

        /// <summary>
        /// Used for playing polyphonic music.
        /// The EventDescription contains a set of noteElements to be handled simultaneously
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void Play(EventDescription eventDescription)
        {
            // Application.DoEvents(); // Experiment. Makes no difference 
            // Sleep until the StartTime of the next event occurs.
            System.Threading.Thread.Sleep(MilliSecondsToSleep(eventDescription.StartTime));

            // Stop playing these notes: 
            if (null != eventDescription.EndEventElements)
            {
                foreach (EndEventElement endEventElement in eventDescription.EndEventElements)
                {
                    MidiNote midiNote = (endEventElement.StartElement as NoteElement).MidiNote;
                    if (null != midiNote)
                    {
                        midiNote.StopPlaying(midiOut);
                        notesCurrentlyPlaying.Remove(midiNote);
                    }
                }

            }

            if (!playing)
            {
                return; // Let the currently existing notes be stopped on time, but do not start any new notes !
            } 

            // Start playing these notes:
            // Itetrate through all parts: 
            for (int i = 0; (i < numberOfParts); i++)
            {
                List<NoteElement> noteElementList = eventDescription.NoteLists[i];
                foreach (NoteElement noteElement in noteElementList)
                {
                    if (noteElement.TieStop) continue; // Let the note continue
                    if (!noteElement.IsPause)
                    {
                        // This is a playable note, not a pause !
                        if (userSettings.partsToPlay[i])
                        {
                            // This part is selected to be played (for instance from the GUI)                        
                            noteElement.MidiNote = new MidiNote(noteElement.Step.ToString(), noteElement.Alter, noteElement.Octave, noteElement.Transpose,noteElement.DynamicsIntValue, noteElement.MidiChannel, midiOut);
                            notesCurrentlyPlaying.Add(noteElement.MidiNote);
                            //noteElement.MidiNote = new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, 1, midiOut);
                        }
                    }
                }
            }

            // Handle harmonies 
            if (null != eventDescription.HarmonyElement)
            {
                if (null != latestHarmonyPlayed)
                {
                    latestHarmonyPlayed.StopPlaying(midiOut);
                    latestHarmonyPlayed = null;
                }

                if (userSettings.GetPlayerSettings(UserSettings.PlayerSettings.Harmonies))
                {
                    // Play the harmony related to this event
                    HarmonyElement h = eventDescription.HarmonyElement;
                    if (ChordType.UnImplemented != h.ChordType)
                    {
                        latestHarmonyPlayed = new MidiChord(h.ChromaticStep, 4, 127, h.ChordType);
                        latestHarmonyPlayed.StartPlaying(midiOut);
                    }
                    else
                    {
                        Logger.Log(string.Format("MusicPlayer: Becifring {0} er ikke implementeret", h.Kind));
                    }
                }
            }

            // Handle Sound desriptions, such as "Tempo"
            if (null != eventDescription.SoundElements)
            {
                foreach (SoundElement soundElement in eventDescription.SoundElements)
                {
                    int newTempo = soundElement.GetTempo();
                    if (0 != newTempo)
                    {
                        // Model.Log(string.Format("MusicPlayer: Tempo {0}->{1}", this.tempo, newTempo));                   
                        // We must also establish new offsets for stopwatch-time and music-time:
                        firstStopWatchTime = stopWatch.ElapsedMilliseconds; // From now on all stopwatch times are relative to this value (now) 
                        musicXmlTimeOffset = eventDescription.StartTime; // From now on all musicXml times are ralative to this value (starttime of the current event
                        Logger.Log(string.Format("MusicPlayer: Tempo {0}->{1} firstStopWatchTime={1} musicXmlTimeOffset={2}", this.tempo, newTempo, firstStopWatchTime, musicXmlTimeOffset));
                        this.tempo = newTempo;
                    }
                }
            }

        }


        /// <summary>
        /// Used when playing automatically. The user just starts a thread for playing.
        /// </summary>
        /// <param name="selectedObject"></param>
        internal void AutoPlay(object selectedObject)
        {
            if (null == selectedObject) return;

            string typeName = selectedObject.GetType().Name;
            switch (typeName)
            {
                //case "NoteElement":     Play(selectedObject as NoteElement); break;
                case "EventDescription":Play(selectedObject as EventDescription); break;
                case "SoundElement":    this.tempo = (selectedObject as SoundElement).GetTempo(); break;
                case "NoteElement":
                case "MeasureElement":
                case "ScorePartElement":
                case "PartElement":
                case "SimpleTextElement":
                    break;
            }
            return;
        }

        private System.Threading.Thread playerThread;
        private volatile bool playing = false;

        public UserSettings UserSettings
        {
            set
            {
                userSettings = value;
            }
        }

        public bool Playing
        {
            get
            {
                return playing;
            }
        }

        public void StartPlayingPoly()
        {
            playing = true;
            playerThread = new System.Threading.Thread(new System.Threading.ThreadStart(PlayerThreadStartPoly));
            //playerThread.Priority = System.Threading.ThreadPriority.Lowest; // Handle UI even when playing complicated stuff
            Logger.Log(string.Format("Starting PlayerThread et priority={0}", playerThread.Priority.ToString()));
            playerThread.Start();
        }

        private void PlayerThreadStartPoly()
        {
            PlayerThreadStart(objects, typeof(EventDescription));
        }

        private void PlayerThreadStart(IObjectCollection objects, Type type)
        {
            System.Threading.Thread.Sleep(1000); // Allow Screanreader to complete initial actions
            this.stopWatch = new System.Diagnostics.Stopwatch();
            this.stopWatch.Start();
            //this.nextActionTime = 0;
            this.firstStopWatchTime = stopWatch.ElapsedMilliseconds;
            notesCurrentlyPlaying = new List<MidiNote>();
         
            //this.nextActionTime = (objects.GetObjectAtIndex(startIndex) as EventDescription).StartTime; // Monophoinic
            this.musicXmlTimeOffset = (objects.GetObjectAtIndex(this.startIndex) as EventDescription).StartTime;
            try
            {
                for (int i = this.startIndex; ((i < objects.GetNumberOfObjects()) && (playing)); i++)
                {
                    object o = objects.GetObjectAtIndex(i); // listBox.Items[i];
                    AutoPlay(o); // Play the next note, using the correct timing!
                    if (o.GetType() == type)
                    {
                        // Only select notes (and pauses) to allow for correct timing!
                        objects.SetSelectedIndex(i);
                        //SetSelectedIndex(listBox, i); // Select the corresponding line in the Listbox,  handling Cross-thread issue
                    }
                }
            }
            catch (Exception e)
            {
                // If the application is closed the listbox may be disposed before we can stop playing !
                playing = false;
                Logger.Log(string.Format("PlayerThread threw an exception because the program was stopped while playing. Message= {0}", e.Message));
            }
            // Stop all notes currently playing! If they don't decay they will keep playing forever !
            foreach (MidiNote midiNote in notesCurrentlyPlaying)
            {
                midiNote.StopPlaying(this.midiOut);
            }
        }



        public void StopPlaying()
        {
            playing = false;
        }

    }
}
