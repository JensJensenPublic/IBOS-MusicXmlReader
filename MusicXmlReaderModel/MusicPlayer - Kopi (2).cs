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

    enum MusicPlayerThreadStateEnum { unknown = 0, stopped, running };

    public class MusicPlayer
    {
        string className = "MusicPlayer";
        //MidiNote latestNotePlayed = null;
        MidiChord latestHarmonyPlayed = null;
        MidiOut midiOut = null;
        IObjectCollection objects = null;
        // ListBox listBoxPoly = null;
        System.Diagnostics.Stopwatch stopWatch = null;
        //long nextActionTime;    // For autoplaying monophonic music 
        long firstStopWatchTime = -1;    // For autoplaying polyphonic music 
        int tempo = 60; // Quarter notes per minute.  Use 60 as a default
        int numberOfParts;
        int startIndex;
        List<MidiNote> notesCurrentlyPlaying; // Contains all notes currently playing. Used when playing is stopped by user
        volatile MusicPlayerThreadStateEnum musicPlayerThreadState; // Set by the MusicPlayerThread, read by the UI thread ! 
        int playerThreadId = 0; // Used for debugging only to keep track of various instances of the MusicPlayerThread.

        long musicXmlTimeOffset = 0; // Needed for handling change in Tempo while aulo-playing. Unit is the same as for Duration

        bool repeating = false;
        int firstRepetitionIndex;
        int lastRepetitionIndex;

        // User settings
        UserSettings userSettings;

        bool userTemopChanged = false;
        //int   userTempo = 100; // Percentage of tempo indicated in score
        float userTempoFactor = (float)1;

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
                foreach (List<NoteElement> noteElementList in eventDescription.NoteLists)
                {
                    foreach (NoteElement noteElement in noteElementList)
                    {
                        if (!noteElement.IsPause)
                        { // This is a real note, not a pause
                            if (userSettings.partsToPlay[noteElement.PartNumber])
                            {
                                MidiNote midiNote = new MidiNote(GetChromaticStep(noteElement.Step), noteElement.Alter, noteElement.Octave, noteElement.Transpose, noteElement.DynamicsIntValue, noteElement.MidiChannel, midiOut);
                                notesCurrentlyPlaying.Add(midiNote);
                            }
                        }
                    }
                }
                PlayHarmonies(eventDescription);   // Handle harmonies
            }
            return;
        }


        private ChromaticStep GetChromaticStep(PitchElement.FullStepEnum fullStep)
        {
            switch (fullStep)
            {
                case PitchElement.FullStepEnum.C: return ChromaticStep.C;
                case PitchElement.FullStepEnum.D: return ChromaticStep.D;
                case PitchElement.FullStepEnum.E: return ChromaticStep.E;
                case PitchElement.FullStepEnum.F: return ChromaticStep.F;
                case PitchElement.FullStepEnum.G: return ChromaticStep.G;
                case PitchElement.FullStepEnum.A: return ChromaticStep.A;
                case PitchElement.FullStepEnum.B: return ChromaticStep.B;
                //case "H": return ChromaticStep.B;
                default: throw new System.ArgumentException(string.Format("Unknown step:{0}", fullStep));
            }
        }




        /// <summary>
        /// Common helper, used from autoplaying, Called from method Play()
        /// and from manual playing, called from method SelectedIndexChanged()
        /// </summary>
        /// <param name="eventDescription"></param>
        private void PlayHarmonies(EventDescription eventDescription)
        {
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
                        List<string> errors = new List<string>(); // MidiChord has no access to the logging system. Instead we log errors in this way: 
                        latestHarmonyPlayed = new MidiChord(h.ChromaticStep, 4, 127, h.ChordType, h.ChromaticBassStep, h.Degrees, errors); // The last 2 parameters will be used for non-standard harmonies 
                        latestHarmonyPlayed.StartPlaying(midiOut);
                        foreach (string error in errors)
                        {
                            Logger.LogOnce(error);
                        }
                    }
                    else
                    {
                        Logger.Log(string.Format("MusicPlayer: Becifring {0} er ikke implementeret", h.Kind));
                    }
                }
            }
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


        //public void Reset(int numberOfParts,int startIndex)
        //{
        //    string functionName = "Reset";
        //    Logger.Log(string.Format("{0}.{1}({2},{3})", className, functionName, numberOfParts, startIndex));
        //    this.numberOfParts = numberOfParts;
        //    this.startIndex = (startIndex < 0) ? 0 : startIndex;
        //}


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
            float eventTimeInMilliSeconds = ((float)(startTime - this.musicXmlTimeOffset) * mSPerMinute) / ((float)NoteElement.commonDivisions * (float)this.tempo * userTempoFactor);
            long sleep = ((long)eventTimeInMilliSeconds - (this.stopWatch.ElapsedMilliseconds - this.firstStopWatchTime));
            return (int)Math.Max(0, sleep);
        }


        /// <summary>
        /// This method allows musical Tempo to be changed by any Eventdescriptor, either caused by
        /// 1) A SoundElement contained in the EventDescriptor and describing a change of Tempo found in the MusicXml file.
        /// 2) A User interaction (which is for simplicity executed by the next EventDescriptor)
        /// </summary>
        /// <param name="eventStartTime">The Start time for the EventDescriptor</param>
        private void ChangeTimingOffsets(int eventStartTime)
        {
            firstStopWatchTime = stopWatch.ElapsedMilliseconds; // From now on all stopwatch times are relative to this value (now) 
            musicXmlTimeOffset = eventStartTime; // From now on all musicXml times are ralative to this value (starttime of the current event
        }



        /// <summary>
        /// Used for playing polyphonic music.
        /// The EventDescription contains a set of noteElements to be handled simultaneously
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void Play(EventDescription eventDescription)
        {

            List<NoteElement> noteElementsToStopPlaying = new List<NoteElement>();
            List<NoteElement> noteElementsToStartPlaying = new List<NoteElement>();

            // Application.DoEvents(); // Experiment. Makes no difference 
            // Sleep until the StartTime of the next event occurs.
            System.Threading.Thread.Sleep(MilliSecondsToSleep(eventDescription.StartTime));

            // Stop playing these notes: 
            if (null != eventDescription.EndEventElements)
            {
                foreach (EndEventElement endEventElement in eventDescription.EndEventElements)
                {
                    NoteElement noteElement = endEventElement.StartElement as NoteElement;
                    if (!noteElement.IsPause)
                    {
                        noteElementsToStopPlaying.Add(noteElement);
                        noteElement.Tied = false;
                    }
                    //    MidiNote midiNote = (endEventElement.StartElement as NoteElement).MidiNote;                 
                    //    if (null != midiNote)
                    //    {
                    //        midiNote.StopPlaying(midiOut);
                    //        notesCurrentlyPlaying.Remove(midiNote);
                    //    }
                }

            }

            //if (!playing)
            //{
            //    return; // Let the currently existing notes be stopped on time, but do not start any new notes !
            //}

            // Start playing these notes:
            // Itetrate through all parts: 
            if (playing) // Let the currently existing notes be stopped on time, but do not start any new notes !
            {
                for (int i = 0; (i < numberOfParts); i++)
                {
                    List<NoteElement> noteElementList = eventDescription.NoteLists[i];
                    foreach (NoteElement noteElement in noteElementList)
                    {

                        if (!noteElement.IsPause)
                        {
                            // This is a playable note, not a pause !
                            if (userSettings.partsToPlay[i])
                            {
                                noteElementsToStartPlaying.Add(noteElement);
                                noteElement.Tied = false;
                            }
                        }
                        //    if (noteElement.TieStop) continue; // Let the note continue
                        //    if (!noteElement.IsPause)
                        //    {
                        //        // This is a playable note, not a pause !
                        //        if (userSettings.partsToPlay[i])
                        //        {
                        //            // This part is selected to be played (for instance from the GUI)                        
                        //            noteElement.MidiNote = new MidiNote(GetChromaticStep(noteElement.Step), noteElement.Alter, noteElement.Octave, noteElement.Transpose,noteElement.DynamicsIntValue, noteElement.MidiChannel, midiOut);
                        //            notesCurrentlyPlaying.Add(noteElement.MidiNote);
                        //            //noteElement.MidiNote = new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, 1, midiOut);
                        //        }
                        //    }
                        //}
                    }
                }
            }


            // Remove tied notes from both lists:
            foreach (NoteElement noteElementToStop in noteElementsToStopPlaying)
            {

                CHECK
               if (noteElementToStop.TieStart)
                {
                    foreach (NoteElement noteElementToStart in noteElementsToStartPlaying)
                    {
                        if (noteElementToStart.TieStop)
                        {
                            if (noteElementToStart.IsTiedTo(noteElementToStop))
                            {
                                // Mark both NoteElements so we can avoid stopping and restarting the MidiNote
                                noteElementToStart.Tied = true;
                                noteElementToStop.Tied = true;
                            }
                        }
                    }
                }
            }

            // Stop all notes that have not been tied
            foreach (NoteElement noteElement in noteElementsToStopPlaying)
            {
                if (!noteElement.Tied)
                {
                    MidiNote midiNote = noteElement.MidiNote;
                    if (null != midiNote)
                    {
                        midiNote.StopPlaying(midiOut);
                        notesCurrentlyPlaying.Remove(midiNote);
                    }
                }
            }

            // Start all notes that have not been tied
            foreach (NoteElement noteElement in noteElementsToStartPlaying)
            {
                if (!noteElement.Tied)
                {
                    noteElement.MidiNote = new MidiNote(GetChromaticStep(noteElement.Step), noteElement.Alter, noteElement.Octave, noteElement.Transpose, noteElement.DynamicsIntValue, noteElement.MidiChannel, midiOut);
                    notesCurrentlyPlaying.Add(noteElement.MidiNote);
                }
            }



            PlayHarmonies(eventDescription);     // Handle harmonies

            // Handle Sound desriptions, such as "Tempo". NOTE: May cause a change of musical tempo!
            if (null != eventDescription.SoundElements)
            {
                foreach (SoundElement soundElement in eventDescription.SoundElements)
                {
                    // int newTempo = soundElement.GetTempo();
                    int newTempo = 0;
                    if ((null != soundElement.Tempo) && int.TryParse(soundElement.Tempo, out newTempo))
                    {
                        ChangeTimingOffsets(eventDescription.StartTime); // Establish new offsets for stopwatch-time and music-time:
                        Logger.Log(string.Format("MusicPlayer: Tempo {0}->{1} firstStopWatchTime={2} musicXmlTimeOffset={3}", this.tempo, newTempo, firstStopWatchTime, musicXmlTimeOffset));
                        this.tempo = newTempo;
                    }
                }
            }


            // Finally handle changes in UserSettings.Tempo. NOTE: May cause a change of musical tempo!
            {
                if (userTemopChanged)
                {
                    float currentUserTempoFactor = userTempoFactor;
                    userTempoFactor = (float)userSettings.UserTempo / (float)100;
                    ChangeTimingOffsets(eventDescription.StartTime); // Establish new offsets for stopwatch-time and music-time:
                    Logger.Log(string.Format("MusicPlayer: UserTempoFactor {0}->{1} firstStopWatchTime={2} musicXmlTimeOffset={3}", currentUserTempoFactor, userTempoFactor, firstStopWatchTime, musicXmlTimeOffset));
                    userTemopChanged = false;
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
                case "EventDescription": Play(selectedObject as EventDescription); break;
                case "SoundElement": this.tempo = (selectedObject as SoundElement).GetTempo(); break;
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

        ///// <summary>
        ///// Can be used for implementing toggle-functionality for starting and stopping the playerThread in the UI
        ///// </summary>
        //public bool PlayerThreadIsRunning
        //{
        //    get
        //    {
        //        return playerThreadIsRunning;
        //    }
        //}

        internal MusicPlayerThreadStateEnum MusicPlayerThreadState
        {
            get
            {
                return musicPlayerThreadState;
            }
        }

        private void PlayerThreadStartPoly()
        {
            PlayerThreadStart(objects);

        }

        private void PlayerThreadStart(IObjectCollection objects)
        {
            int threadId = playerThreadId;
            musicPlayerThreadState = MusicPlayerThreadStateEnum.running;

            string functionName = "PlayerThreadStart";
            //playerThreadIsRunning = true;
            System.Threading.Thread.Sleep(1000); // Allow Screanreader to complete initial actions
            Logger.Log(string.Format("PlayerThread(Id={0}) starting", threadId));
            this.stopWatch = new System.Diagnostics.Stopwatch();
            this.stopWatch.Start();
            //this.nextActionTime = 0;



            //this.nextActionTime = (objects.GetObjectAtIndex(startIndex) as EventDescription).StartTime; // Monophoinic


            int firstIndex = this.startIndex;
            int lastIndex = objects.GetNumberOfObjects();

            if (repeating)
            {
                firstIndex = firstRepetitionIndex; // TO DO: Convert to index !
                lastIndex = lastRepetitionIndex;   // TO DO: Convert to index !
            }

            do
            {
                if (repeating)
                {
                    Logger.Log(string.Format("{0}.{1} Repeating Index[{2},{3}]", className, functionName, firstRepetitionIndex, lastRepetitionIndex));
                }
                notesCurrentlyPlaying = new List<MidiNote>();
                // Establish a common startpoint for computing note duration:
                this.musicXmlTimeOffset = (objects.GetObjectAtIndex(firstIndex) as EventDescription).StartTime;
                this.firstStopWatchTime = stopWatch.ElapsedMilliseconds;
                try
                {
                    for (int i = firstIndex; ((i < lastIndex) && (playing)); i++)
                    {
                        object o = objects.GetObjectAtIndex(i); // listBox.Items[i];
                        AutoPlay(o); // Play the next note, using the correct timing!
                        if (o.GetType() == typeof(EventDescription))
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
                StopAllNotesPlaying();
            } while (repeating && playing);
            Logger.Log(string.Format("PlayerThread(Id={0}) exiting", threadId));
            playing = false;
            musicPlayerThreadState = MusicPlayerThreadStateEnum.stopped;
        }


        /// <summary>
        /// Stop all notes currently playing.
        /// Can be called:
        /// 1) From the UI for stopping a non-dacaying note (for instance an organ-note)
        /// 2) After any musicplayer exception
        /// 3) Immediately before application exit
        /// </summary>
        public void StopAllNotesPlaying()
        {
            string functionName = "StopAllNotesPlaying";
            int n = notesCurrentlyPlaying.Count;
            for (int i = 0; (i < n); i++)
            {
                notesCurrentlyPlaying[i].StopPlaying(this.midiOut);
            }
            Logger.Log(string.Format("{0}.{1}: Stopped {2} notes from playing", className, functionName, n));
        }


        private void WaitForExistingTreadToStop()
        {
            string functionName = "WaitForExistingTreadToStop";
            System.Threading.Thread.Sleep(100);
            // Give the player thread a chance to exit in order to prevent 2 threads running at the same time
            for (int i = 0; (i < 10) && (musicPlayerThreadState == MusicPlayerThreadStateEnum.running); i++)
            {
                if (i == 5)
                {
                    Logger.Log(string.Format("{0}.{1} waiting for MusicPlayerThread to stop", className, functionName));
                }
                System.Threading.Thread.Sleep(100);
            }
            if (musicPlayerThreadState == MusicPlayerThreadStateEnum.running)
            {
                Logger.Log(string.Format("{0}.{1} warning: MusicPlayerThread did not stop within time limit", className, functionName));
            }
        }


        //
        // Public methods for starting and stopping the MusicPlayer
        //

        /// <summary>
        /// Stop the current PlayerThread if any
        /// Unconditionally start playing as specified
        /// </summary>
        /// <param name="numberOfParts">Number of parts in the current score</param>
        /// <param name="startIndex">Index to start playing at</param>
        public bool StartPlaying(int numberOfParts, int startIndex)
        {
            string functionName = "StartPlaying";
            Logger.Log(string.Format("{0}.{1}(Parts={2},StartIndex={3})", className, functionName, numberOfParts, startIndex));

            if (musicPlayerThreadState == MusicPlayerThreadStateEnum.running) return false;
            this.startIndex = startIndex;
            this.numberOfParts = numberOfParts;
            playerThreadId = (playerThreadId + 1) % 1000; // No silly overrun
            playing = true;
            playerThread = new System.Threading.Thread(new System.Threading.ThreadStart(PlayerThreadStartPoly));
            //playerThread.Priority = System.Threading.ThreadPriority.Lowest; // Handle UI even when playing complicated stuff
            Logger.Log(string.Format("Starting PlayerThread at priority={0}", playerThread.Priority.ToString()));
            playerThread.Start();
            return true;

        }

        public void SetUserTempo(int tempo)
        {
            userSettings.UserTempo = tempo;
            userTemopChanged = true;
        }



        /// <summary>
        /// Unconditionally stop playing
        /// Keep the calling (UI) thread back for up to 1000 mS to avoid 2 or more threads running at the same time !
        /// </summary>
        public void StopPlaying()
        {
            string functionName = "StopPlaying";
            Logger.Log(string.Format("{0}.{1}", className, functionName));
            playing = false;
            repeating = false;
        }

        /// <summary>
        /// Stop the current PlayerThread if any
        /// Start playing as specified
        /// </summary>
        /// <param name="numberOfParts">Number of parts in the current score</param>
        /// <param name="firstIndex">First index to repeat</param>
        /// <param name="lastIndex">Last index to repeat</param>
        /// <returns></returns>
        public bool StartRepeating(int numberOfParts, int firstIndex, int lastIndex)
        {

            string functionName = "StartRepeating";
            Logger.Log(string.Format("{0}:{1}(Parts={2},FirstIndex={3},LastIndex={4})", className, functionName, numberOfParts, firstIndex, lastIndex));
            if (musicPlayerThreadState == MusicPlayerThreadStateEnum.running) return false;
            //this.numberOfParts = numberOfParts;
            bool result = true;
            if ((firstIndex >= 0) && (lastIndex >= 0))
            {
                StopPlaying();
                this.repeating = true;
                this.firstRepetitionIndex = firstIndex;
                this.lastRepetitionIndex = lastIndex;
                StartPlaying(numberOfParts, firstIndex);
            }
            else
            {
                repeating = false;
            }
            return result;
        }

        /// <summary>
        /// Stop the current PlayerThread if any 
        /// Implicitly clear the "repeating " flag by calling StopPlaying()
        /// </summary>
        /// <returns></returns>
        public void StopRepeating()
        {
            StopPlaying();
        }


        /// <summary>
        /// If a player thread is running it is stopped
        /// If  no player thread is running a new thread is started as specified.
        /// </summary>
        /// <param name="numberOfParts"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public bool ToggleStartStopPlaying(int numberOfParts, int startIndex)
        {
            string functionName = "ToggleStartStopPlaying";
            Logger.Log(string.Format("{0}:{1}(Parts={2},StartIndex={3})", className, functionName, numberOfParts, startIndex));
            if (musicPlayerThreadState == MusicPlayerThreadStateEnum.running)
            {
                StopPlaying();
                return false;
            }
            else
            {
                StartPlaying(numberOfParts, Math.Max(0, startIndex));
                return true;
            }

        }




    }
}
