using System;
using JSJ.MusicSynthesis;
using System.Windows.Forms;
using NAudio.Midi;


namespace MusicXmlReaderUI
{
    public class MusicPlayer
    {

        MidiNote latestNotePlayed = null;
        MidiOut midiOut = null;
        ListBox listBox = null;
        System.Diagnostics.Stopwatch stopWatch = null;
        long nextActionTime;
        int tempo;

        float userSlowDown;


        /// <summary>
        /// Constructor
        /// </summary>
        public MusicPlayer(ListBox listBox, MidiOut midiOut)
        {
            this.midiOut = midiOut;
            this.listBox = listBox;
            // this.userSlowdown = 1.0F;
            this.userSlowDown = 1.0F;
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
            if (!(selectedObject is NoteElement)) return;
            NoteElement noteElement = selectedObject as NoteElement;
            if (string.IsNullOrEmpty(noteElement.Step)) return; // This is a pause
            new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
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
                NoteElement nodeElement = selectedObject as NoteElement;

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
                
                if (! nodeElement.TieStop)
                {
                    if (null != latestNotePlayed)
                    {
                        latestNotePlayed.StopPlaying(midiOut);
                    }

                    if ("" != nodeElement.Step)
                    {
                        // This is a playable note, not a pause !
                        latestNotePlayed = new MidiNote(nodeElement.Step, nodeElement.Alter, nodeElement.Octave, 127, midiOut);
                    }
                }

                // Simple timing-implementation:
                float duration = nodeElement.GetDuration();
                float divisions = int.Parse(nodeElement.Divisions);
                float tempo = this.tempo;
                float durasionInUnitOfMeasures = duration / divisions;
                float durationInUnitOfMilliSeconds = userSlowDown * 60 * 1000 * durasionInUnitOfMeasures / tempo;
                nextActionTime += (int)durationInUnitOfMilliSeconds;
                // System.Threading.Thread.Sleep((int)(durationInUnitOfMilliSeconds));
                //if (null != latestNotePlayed)
                //{
                //    latestNotePlayed.StopPlaying(midiOut);
                //}
                //
                return;
            }
            //else if (selectedObject is MeasureElement)
            //{
            //    MeasureElement measureElement = selectedObject as MeasureElement;

            //    // Same code as above !!
            //    if (0 == nextActionTime)
            //    {
            //        // We  play the first note or pause immediately but remember when we did it.
            //        nextActionTime = stopWatch.ElapsedMilliseconds;
            //    }
            //    else
            //    {
            //        long sleep = nextActionTime - stopWatch.ElapsedMilliseconds;
            //        sleep -= 200; // Show the Measure 200 mS before the first beat in the meeasure to please the screenreader.
            //        sleep = Math.Max(0, sleep); // Hack to avoid crash 
            //        System.Threading.Thread.Sleep((int)sleep);
            //    }
            //
            //}
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

        delegate void SetSelectedIndexCallback(int index);
        private void SetSelectedIndex(int index)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listBox.InvokeRequired)
            {
                SetSelectedIndexCallback d = new SetSelectedIndexCallback(SetSelectedIndex);
                listBox.Invoke(d, new object[] { index });
            }
            else
            {
                listBox.Focus(); // Maybe not needed. How can we force the Screeen-reader to read the selected line? 
                listBox.SelectedIndex = index;
                // System.Threading.Thread.Sleep(100); // HACK Pause the UI thread and let the Screenreader get a chance
                                       
            }
        }


        private void PlayerThreadStart()
        {
            System.Threading.Thread.Sleep(1000); // Allow Screanreader to complete initial actions
            this.stopWatch = new System.Diagnostics.Stopwatch();
            this.stopWatch.Start();
            this.nextActionTime = 0;
            for (int i = 0; ((i < listBox.Items.Count) && (playing)); i++)
            {
                object o = listBox.Items[i]; 
                AutoPlay(o); // Play the next note, using the correct timing! 
                //System.Threading.Thread.Sleep(10); // Allow the UI thread to do its work immediately.
                if (o is NoteElement)
                {
                    // Only select notes (and pauses) to allow for correct timing!
                    SetSelectedIndex(i); // Select the corresponding line in the Listbox,  handling Cross-thread issue
                }
                //System.Threading.Thread.Sleep(10); // Allow the UI thread to do its work immediately.          
            }
        }

        public void StartPlaying()
        {
            playing = true;
            playerThread = new System.Threading.Thread(new System.Threading.ThreadStart(PlayerThreadStart));
            playerThread.Start();
        }

        public void StopPlaying()
        {
            playing = false;
        }
    }
}
