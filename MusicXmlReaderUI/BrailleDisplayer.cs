using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

//Unicode for Braille
//https://en.wikipedia.org/wiki/Braille_Patterns
//http://www.unicode.org/charts/PDF/U2800.pdf

namespace MusicXmlReaderUI
{

    /// <summary>
    /// Similar function to MusicPlyuer
    /// </summary>
    class BrailleDisplayer
    {
        public static char UnicodeBrailleBase = (char)0x2800;

        private FSBrlDspAPIWrapper fSBrlDspAPIWrapper;
        private TextBox musicBrailleTextBox; // The textbox used for writing MusicBraille bytes, repredsented as UniCode
        private Thread brailleDisplayThread;
        private bool displaying;
        private string latestMessage = null; // Latest message sent to Braille display using NvdaControllerClientWrapper.nvdaController_brailleMessage
        private string emptyBrailleString;

        /// <summary>
        /// Thread needed for refreshing the MusicBraille Message sent to the Braille Display to prevent it from being overwritten by LyricBraille
        /// </summary>
        private void DisplayerThreadStart()
        {
            while (displaying)
            {
                Thread.Sleep(1000); // Refresh the display every second as long as needed
                {
                    if (!string.IsNullOrEmpty(latestMessage))
                    {
                        int brailleMessageResult = NvdaControllerClientWrapper.nvdaController_brailleMessage(latestMessage);
                        if (0 != brailleMessageResult)
                        {
                            Model.Log(string.Format("NvdaControllerClientWrapper.nvdaController_brailleMessage failed. Result={0}", brailleMessageResult));
                        }
                    }
                }
            }
        }

        private BrailleDisplayer(TextBox tb)
        {
            musicBrailleTextBox = tb;
            fSBrlDspAPIWrapper = FSBrlDspAPIWrapper.Create();
            fSBrlDspAPIWrapper.Open();

            displaying = true;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; (i < 14); i++) { sb.Append(UnicodeBrailleBase);};
            emptyBrailleString = sb.ToString();
            {
                brailleDisplayThread = new System.Threading.Thread(new System.Threading.ThreadStart(DisplayerThreadStart));
                Model.Log(string.Format("Starting PlayerThread et priority={0}", brailleDisplayThread.Priority.ToString()));
                brailleDisplayThread.Start();
            }             
        }

  
        public static BrailleDisplayer Create(TextBox tb)
        {
             return new BrailleDisplayer(tb);
        }

        
        /// <summary>
        /// Used when the playing manually.
        /// The User selects a note at a time. 
        /// or
        /// The user selects an EventDescription at a time. This may contain several notes to be played simultaneously
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="selectedObject"></param>
        internal void SelectedIndexChanged(int selectedIndex, object selectedObject)
        {
            StopRefreshing(); // Stop refreshing the Braille Display; Also happens when controllooses focus      

            //if (playing) return;
            if (null == selectedObject) return;
            if ((selectedObject is NoteElement))
            {
                //NoteElement noteElement = selectedObject as NoteElement;
                //if (noteElement.IsPause) return; // This is a pause
                //// new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
            }
            else if ((selectedObject is EventDescription))
            {
  
                EventDescription eventDescription = selectedObject as EventDescription;
                int cancelSpeechResult0 = NvdaControllerClientWrapper.nvdaController_cancelSpeech();
                //System.Threading.Thread.Sleep(100);
                //int speekTextResult = NvdaControllerClientWrapper.nvdaController_speakText(eventDescription.ToNormalTextString());
                // int cancelSpeechResult1 = NvdaControllerClientWrapper.nvdaController_cancelSpeech();
                //System.Threading.Thread.Sleep(100);

                string text = eventDescription.ToString();          // The text currently shown on the visual display
                List<byte> bytes = eventDescription.ToBraille();    // The Braille pattern to show on the Braill display

                byte[] byteArray = bytes.ToArray();

                // Write these bytes to the Braille display
                //if (byteArray.Length > 0)
                //{
                //    fSBrlDspAPIWrapper.Write(byteArray);
                //}

                // Write these bytes to the MusicBraille textbox, represented as UniCode                               
                StringBuilder musicBrailleStringBuilder = new StringBuilder();
           
                foreach (byte b in bytes)
                {
                    musicBrailleStringBuilder.Append((char)(UnicodeBrailleBase + (char)b));
                }
                int paddingLength = 14 - musicBrailleStringBuilder.Length;
                for (int i = 0; (i < paddingLength); i++)
                {
                    musicBrailleStringBuilder.Append(UnicodeBrailleBase); // Fill with 0 Braille chars
                }
                musicBrailleTextBox.Text = musicBrailleStringBuilder.ToString();


                //Write these bytes to the Braille Diaplay through the NVDA Client, overwriting the Lyric - Braille with Music-Braille
                latestMessage = musicBrailleStringBuilder.ToString();
                int brailleMessageResult = NvdaControllerClientWrapper.nvdaController_brailleMessage(latestMessage);
                if (0 != brailleMessageResult)
                {
                    Model.Log(string.Format("NvdaControllerClientWrapper.nvdaController_brailleMessage failed. Result={0}", brailleMessageResult));
                }

            }
            return;
        }

        public void StopRefreshing()
        {
            latestMessage = string.Empty; // Stop refreshing the physical Braille Display
            musicBrailleTextBox.Text = emptyBrailleString;
        }

    }


}
