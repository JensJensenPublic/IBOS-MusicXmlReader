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
            this.userSlowDown = 4.0F;
        }


        internal void SelectedIndexChanged(int selectedIndex, object selectedObject)
        {
            if (null == selectedObject) return;

            // We can't switch on selectedObject.GetType() because it is not an integral type.
            if (selectedObject is NoteElement)
            {
                NoteElement nodeElement = selectedObject as NoteElement;
                //new MidiNote(Step.C, 4, 127, midiOut);

                if (null != stopWatch) // Just continue if we are not playing using a stopwatch
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
                }

                bool tieStop;
                if ("stop" == nodeElement.Tie)
                    tieStop = true;
                else
                    tieStop = false;

                if (!tieStop)
                {
                    if (null != latestNotePlayed)
                    {
                        latestNotePlayed.StopPlaying(midiOut);
                    }

                    if ("" == nodeElement.Step)
                    {
                        //new MidiPause();
                    }
                    else
                    {
                        //   latestNotePlayed = new MidiNote(GetStep(nodeElement.Step, nodeElement.Alter), int.Parse(nodeElement.Octave), 127, midiOut);
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
            else if (selectedObject is SoundElement)
            {
                this.tempo = (selectedObject as SoundElement).GetTempo();
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
                
            }


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
                listBox.SelectedIndex = index;
            }
        }


        private void PlayerThreadStart()
        {
            this.stopWatch = new System.Diagnostics.Stopwatch();
            this.stopWatch.Start();
            this.nextActionTime = 0;
            for (int i = 0; (playing); i++)
            {
                SetSelectedIndex(i); // Handles Cross-thread issue
                System.Threading.Thread.Sleep(100);
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
