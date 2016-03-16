using System;
using System.Collections.Generic;
using JSJ.MusicSynthesis;
using System.Windows.Forms;
using NAudio.Midi;


namespace MusicXmlReaderUI
{
    public class MusicPlayer
    {

        MidiNote   latestNotePlayed = null;
        MidiNote[] latestNotesPlayed = null;
        MidiChord latestHarmonyPlayed = null;
        MidiOut midiOut = null;
        ListBox listBox = null;
        ListBox listBoxPoly = null;
        System.Diagnostics.Stopwatch stopWatch = null;
        long nextActionTime;    // For autoplaying monophonic music 
        long firstStopWatchTime = -1;    // For autoplaying polyphonic music 
        int tempo;
        int numberOfParts;


        // User settings
        UserSettings userSettings;
   
        //float userSlowDown;


        /// <summary>
        /// Constructor
        /// </summary>
        public MusicPlayer(ListBox listBox, ListBox listBoxPoly, MidiOut midiOut)
        {
            this.midiOut = midiOut;
            this.listBox = listBox;
            this.listBoxPoly = listBoxPoly;  
        }


        /// <summary>
        /// Used when the playing manually. The User selects a note at a time.
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="selectedObject"></param>
        internal void SelectedIndexChanged(int selectedIndex, object selectedObject)
        {
            if (playing) return;
            if (null == selectedObject) return;
            if ((selectedObject is NoteElement))
            {
                NoteElement noteElement = selectedObject as NoteElement;
                if (string.IsNullOrEmpty(noteElement.Step)) return; // This is a pause
                new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
            }
            else if ((selectedObject is EventDescription))
            {
                EventDescription eventDescription = selectedObject as EventDescription;
                foreach(List<NoteElement> noteElementList in eventDescription.NoteLists)
                {
                    foreach (NoteElement noteElement in noteElementList)
                    {
                        if (!noteElement.IsPause)
                        { // This is a real note, not a pause
                            if (userSettings.partsToPlay[noteElement.PartNumber])
                            {
                                new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Simple timing-implementation:
        /// </summary>
        /// <param name="noteElement"></param>
        /// <returns></returns>
        private int Duration(NoteElement noteElement)
        {
            float duration = noteElement.Duration;
            float divisions = noteElement.Divisions;
            float tempo = this.tempo;
            float durasionInUnitOfMeasures = duration / divisions;
            float durationInUnitOfMilliSeconds = userSettings.userSlowDown * 60 * 1000 * durasionInUnitOfMeasures / tempo;
            return (int)durationInUnitOfMilliSeconds;
        }


        /// <summary>
        /// Play a single, monophonic note
        /// </summary>
        /// <param name="noteElement"></param>
        private void Play(NoteElement noteElement)
        {

            if (0 == nextActionTime)
            {
                // We  play the first note or pause immediately but remember when we did it.
                nextActionTime = stopWatch.ElapsedMilliseconds;
            }
            else
            {
                long sleep = nextActionTime - stopWatch.ElapsedMilliseconds;
                sleep = Math.Max(0, sleep); // Hack to avoid crash 
                System.Threading.Thread.Sleep((int)sleep);
            }

            if (!noteElement.TieStop)
            {
                if (null != latestNotePlayed)
                {
                    latestNotePlayed.StopPlaying(midiOut);
                }

                if ("" != noteElement.Step)
                {
                    // This is a playable note, not a pause !
                    latestNotePlayed = new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
                }
            }
            nextActionTime += Duration(noteElement);
        }


        public void Reset(int numberOfParts)
        {
            this.numberOfParts = numberOfParts;
            // Initialize list of latest played notes.
            latestNotesPlayed = new MidiNote[numberOfParts];
        }

        /// <summary>
        /// Used for playing polyphonic music.
        /// The EventDescription contains a set of noteElements to be handled simultaneously
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void Play(EventDescription eventDescription)
        {
            // Wait for the time to play:
            int factor = 1;
            long sleep = (eventDescription.StartTime / factor) - (stopWatch.ElapsedMilliseconds - firstStopWatchTime);
            System.Threading.Thread.Sleep((int)Math.Max(0, sleep));

            // Itetrate through all parts: 
            for (int i = 0; (i < numberOfParts); i++)
            {
                List<NoteElement> noteElementList = eventDescription.NoteLists[i];
                foreach (NoteElement noteElement in noteElementList)
                {

                    //NoteElement noteElement = eventDescription.Notes[i];
                    //if (null == noteElement) continue; // Nothing happens in this part.
                    if (noteElement.TieStop) continue; // Let the note continue

                    if (null != latestNotesPlayed[i])
                    {
                        latestNotesPlayed[i].StopPlaying(midiOut);
                    }

                    if ("" != noteElement.Step)
                    {
                        // This is a playable note, not a pause !
                        if (userSettings.partsToPlay[i])
                        {
                            // This part is selected to be played (for instance from the GUI)
                            latestNotesPlayed[i] = new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
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

                if (userSettings.playHarmonies)
                {
                    // Play the harmony related to this event
                    HarmonyElement h = eventDescription.HarmonyElement;
                    latestHarmonyPlayed = new MidiChord(ChromaticStep.Bb, 4, 127, ChordType.Major7maj);
                    latestHarmonyPlayed.StartPlaying(midiOut);
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

            // We can't switch on selectedObject.GetType() because it is not an integral type.
            if (selectedObject is NoteElement)
            {
                Play(selectedObject as NoteElement);
                return;
            }
            else if (selectedObject is EventDescription)
            {
                Play(selectedObject as EventDescription);
                return;
            }
            else if (selectedObject is SoundElement)
            {
                this.tempo = (selectedObject as SoundElement).GetTempo();
                //System.Threading.Thread.Sleep(1000); ;
            }
            else
            {
                string typeName = selectedObject.GetType().Name;
                switch (typeName)
                {
                    case "MeasureElement":
                    case "ScorePartElement":
                    case "PartElement":
                    case "SimpleTextElement":
                        break;
                }
                //System.Threading.Thread.Sleep(1000) ;
            }
            // Allow the Screen-reader a short time to catch up in order to make it stop complaining !!
            //System.Threading.Thread.Sleep(100);


            return;
        }

        private System.Threading.Thread playerThread;
        private bool playing = false;

        public UserSettings UserSettings
        {
            set
            {
                userSettings = value;
            }
        }
 
        delegate void SetSelectedIndexCallback(ListBox listBox,int index);
        private void SetSelectedIndex(ListBox listBox, int index)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBox.InvokeRequired)
            {
                SetSelectedIndexCallback d = new SetSelectedIndexCallback(SetSelectedIndex);
                listBox.Invoke(d, new object[] { listBox, index });
            }
            else
            {
                listBox.Focus(); // Maybe not needed. How can we force the Screeen-reader to read the selected line? 
                listBox.SelectedIndex = index;
                // System.Threading.Thread.Sleep(100); // HACK Pause the UI thread and let the Screenreader get a chance

            }
        }




        private void  PlayerThreadStartMono()
        {
            PlayerThreadStart(listBox, typeof(NoteElement));
        }

        public void StartPlayingMono()
        {
            this.numberOfParts = 1;
            playing = true;
            playerThread = new System.Threading.Thread(new System.Threading.ThreadStart(PlayerThreadStartMono));
            playerThread.Start();
        }

        public void StartPlayingPoly()
        {
            playing = true;
            playerThread = new System.Threading.Thread(new System.Threading.ThreadStart(PlayerThreadStartPoly));
            playerThread.Start();
        }

        private void PlayerThreadStartPoly()
        {
            PlayerThreadStart(listBoxPoly, typeof(EventDescription));
        }

        private void PlayerThreadStart(ListBox listBox, Type type)
        {
            System.Threading.Thread.Sleep(1000); // Allow Screanreader to complete initial actions
            this.stopWatch = new System.Diagnostics.Stopwatch();
            this.stopWatch.Start();
            this.nextActionTime = 0;
            this.firstStopWatchTime = stopWatch.ElapsedMilliseconds;
            for (int i = 0; ((i < listBox.Items.Count) && (playing)); i++)
            {
                object o = listBox.Items[i];
                AutoPlay(o); // Play the next note, using the correct timing!
                if (o.GetType() == type)
                {
                    // Only select notes (and pauses) to allow for correct timing!
                    SetSelectedIndex(listBox, i); // Select the corresponding line in the Listbox,  handling Cross-thread issue
                }
            }
        }



        public void StopPlaying()
        {
            playing = false;
        }

    }
}
