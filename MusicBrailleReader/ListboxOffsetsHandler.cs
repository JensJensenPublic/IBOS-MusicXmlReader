using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MusicXmlReaderModel;

// NOTE: This class is modeled over "ListboxTimesHandler" in teh MusicXmlREader project

namespace MusicBrailleReader
{
    public class ListBoxOffsetsHandler
    {
        //        private string className = "ListBoxTimesHandler";


        private Model model;
        private bool developerMode;
        //       private DetailsHandler detailsHandler;
        private ListBox listBoxOffsets;
        //        private bool autoReload;    // Used to optimize performance when changing large parts of the UI within short time
        //        public bool AutoReload { get { return autoReload; } set { autoReload = value; } }

        //        public void Focus()
        //        {
        //            listBoxOffsets.Focus();
        //        }

        //        public int GetNumberOfObjects()
        //        {
        //            return listBoxOffsets.Items.Count;
        //        }

        //        delegate object GetObjectAtIndexCallback(int index);
        //        public object GetObjectAtIndex(int index)
        //        {
        //            string functionName = "GetObjectAtIndex";
        //            // InvokeRequired required compares the thread ID of the
        //            // calling thread to the thread ID of the creating thread.
        //            // If these threads are different, it returns true.
        //#if true
        //            // If we don't need InvokeRequired we can also skip moving SelectedIndex back and forth (line 825)
        //            if (listBoxOffsets.InvokeRequired)
        //            {
        //                GetObjectAtIndexCallback d = new GetObjectAtIndexCallback(GetObjectAtIndex);
        //                return listBoxOffsets.Invoke(d, new object[] { index });
        //            }
        //            else
        //#endif
        //            {
        //                if ((index < 0) || (index >= listBoxOffsets.Items.Count))
        //                {
        //                    Logger.Log(string.Format("{0}.{1} Index out of range:{2}", className, functionName, index));
        //                    return null;
        //                }
        //                return listBoxOffsets.Items[index];
        //            }
        //        }

        //        delegate void SetSelectedIndexCallback(int index);
        //        public void SetSelectedIndex(int index)
        //        {
        //            const string functionName = "SetSelectedIndex";
        //            // InvokeRequired required compares the thread ID of the
        //            // calling thread to the thread ID of the creating thread.
        //            // If these threads are different, it returns true.
        //            if (listBoxOffsets.InvokeRequired)
        //            {
        //                SetSelectedIndexCallback d = new SetSelectedIndexCallback(SetSelectedIndex);
        //                listBoxOffsets.Invoke(d, new object[] { index });
        //            }
        //            else
        //            {
        //                //listBoxTimes.Focus(); // Maybe not needed. How can we force the Screeen-reader to read the selected line?
        //                if (listBoxOffsets.Items.Count > index) // Prevent crash during program exit
        //                {
        //                    listBoxOffsets.SelectedIndex = index;
        //                }
        //                else
        //                {
        //                    Logger.Log(string.Format("{0}.{1} Attempted to set index={2} when Items.Count={3}", className, functionName, index, listBoxOffsets.Items.Count));
        //                }
        //                // System.Threading.Thread.Sleep(100); // HACK Pause the UI thread and let the Screenreader get a chance
        //            }
        //        }


        //        public void ConditionalLoad()
        //        {
        //            if (autoReload)
        //            {
        //                Load();
        //            }
        //        }

        int savedSelectedIndex;
        int savedNumberOfLines;

        public void SavePosition()
        {
            savedSelectedIndex = listBoxOffsets.SelectedIndex;
            savedNumberOfLines = listBoxOffsets.Items.Count;
        }

        public void RestorePosition()
        {
            if (savedSelectedIndex < 0) return;
            if (savedSelectedIndex >= listBoxOffsets.Items.Count) return;
            listBoxOffsets.SelectedIndex = savedSelectedIndex;
            if (savedNumberOfLines != listBoxOffsets.Items.Count)
            {
                Logger.LogCF(string.Format(": Number of lines changed from {0} to {1}", savedNumberOfLines, listBoxOffsets.Items.Count));
            }
        }



        //        /// <summary>
        //        /// Load the main listbox with information fetched from the Model
        //        /// First all metainformation (Composer, Author, etc) NO ! Se below !!
        //        /// Then all the events describing the music sheet itself
        //        /// </summary>
        //        public void Load()
        //        {
        //            int selectedIndex = listBoxOffsets.SelectedIndex;  // Save index
        //            listBoxOffsets.Items.Clear();
        //            // Move the Meta information somewhere else !
        //            // We only want EventDescriptions here !
        //            //foreach (string s in model.MetaInfoStrings)
        //            //{
        //            //    listBoxTimes.Items.Add(s);
        //            //}
        //            foreach (EventDescription eventDescription in model.EventDescriptionList.Events)
        //            {
        //                listBoxOffsets.Items.Add(eventDescription);
        //            }
        //            // Restore index without exceeding values
        //            listBoxOffsets.SelectedIndex = Math.Min(selectedIndex, listBoxOffsets.Items.Count);
        //        }




        //        /// <summary>
        //        /// Generate an audible Beep if the listBox is empty
        //        /// </summary>
        //        /// <param name="listBox"></param>
        //        private void WarnIfEmpty(ListBox listBox)
        //        {
        //            if (0 == listBox.Items.Count)
        //            {
        //                UiUtilities.Beep();
        //            }
        //        }

        /// <summary>
        /// Occurs when a key is pressed while listBoxOffsets has focus
        /// </summary>
        /// <param name="e"></param>
        private void KeyDown(object sender, KeyEventArgs e)
        {
            // Allow user to copy/paste seplected items from listbox
            // Inspired by https://stackoverflow.com/questions/51306469/how-to-allow-the-user-to-copy-items-from-listbox-and-paste-outside-of-windows-fo/51308473
            if (e.Control && e.KeyCode == Keys.C)
            {
                StringBuilder sb = new StringBuilder();
                foreach (object item in listBoxOffsets.SelectedItems)
                    sb.AppendLine(item.ToString());
                if (sb.Length > 0)
                    Clipboard.SetDataObject(sb.ToString());
            }

            // string functionName = "listBoxTimes_KeyDown";

            //bool handled = true; // Will be set to false again by the "default:" case if the key is not handled  by one of the specific cases.
            //bool warnIfEmpty = true; // Will be set to false on commands that do not require that a MusicXml file is loaded.
            //DetailsHandler.DetailsDirection FromTop = DetailsHandler.DetailsDirection.FromTop;
            //DetailsHandler.DetailsDirection FromBottom = DetailsHandler.DetailsDirection.FromBottom;
            //switch (e.KeyData) // KeyDate contains information about the control keys (<CTRL> <ALT> <SHIFT>  etc )as well as about the normal ley
            //{
            //     Most functionality is passed directly to the Model
            //    case ShortcutHandler.togglePlaying: model.ToggleStartStopPlaying(listBoxOffsets.SelectedIndex); break; // Keys.Space
            //    case ShortcutHandler.tempoDecrement: model.ChangeUserTempo(-1); break;
            //    case ShortcutHandler.tempoIncrement: model.ChangeUserTempo(+1); break;
            //    case ShortcutHandler.startPlaying: model.StartPlayingPoly(listBoxOffsets.SelectedIndex); break;
            //    case ShortcutHandler.stopPlaying: model.StopPlaying(); break;
            //    case ShortcutHandler.StopAllNotesPlaying: model.musicPlayer.StopAllNotesPlaying(); warnIfEmpty = false; break;
            //    case ShortcutHandler.PreviousMeasure: model.SelectMeasure(listBoxOffsets.SelectedIndex, -1); break;
            //    case ShortcutHandler.NextMeasure: model.SelectMeasure(listBoxOffsets.SelectedIndex, +1); break;
            //    case ShortcutHandler.NextEvent: UiUtilities.WarnAtEnd(listBoxOffsets, +1); handled = false; break; // Let the listbox handle it
            //    case ShortcutHandler.PreviousEvent: UiUtilities.WarnAtEnd(listBoxOffsets, -1); handled = false; break; // Let the listbox handle it



            //     The "Details functionality is handled locally before being passed to the Model:          
            //    case ShortcutHandler.DetailsHarmonyTop: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Harmonies, FromTop); break;    // Start from top
            //    case ShortcutHandler.DetailsPartsTop: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Parts, FromTop); break;        // Start from top  
            //    case ShortcutHandler.DetailsNotesTop: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Notes, FromTop); break;        // Start from top  
            //    case ShortcutHandler.DetailsStatusTop: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Status, FromTop); break;
            //    case ShortcutHandler.DetailsHarmonyBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Harmonies, FromBottom); break;   // Start from bottom
            //    case ShortcutHandler.DetailsPartsBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Parts, FromBottom); break;       //  Start from bottom
            //    case ShortcutHandler.DetailsNotesBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Notes, FromBottom); break;       //  Start from bottom
            //    case ShortcutHandler.DetailsStatusBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Status, FromBottom); break;
            //     DetailsInstruments are handled directly from MenuLine->View because they do not depend on which item is selected in the NoteList.
            //    case ShortcutHandler.DetailsInstruments:    detailsHandler.ShowGlobalDetails(DetailsHandler.DetailsEnum.Instruments, true); break;  // Always shown from top
            //    case ShortcutHandler.DetailsInstrumentsButtom:  ShowDetails(DetailsEnum.Instruments, false); break;

            //    default:
            //        handled = false;        // This event must be handled either by the CommandInterpreter or by the Listbox itself
            //        warnIfEmpty = false;    // Do not issue a warning beep even if no valid MusicXml file is loaded,
            //        break;
            //}

            //if (warnIfEmpty)
            //{
            //     Issue a warning if no data is loaded and the command thus has no meaning
            //    WarnIfEmpty(listBoxOffsets);
            //}

            //if (handled)
            //{
            //    e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control.
            //    return;
            //};

            //// Let the command interpreter handle it 
            //commandInterpreter.Add(e);

        }


        //        private void ListBoxTimesIndexChanged(int index)
        //        {
        //            object o = listBoxOffsets.Items[index];
        //            EventDescription eventDescription = o as EventDescription;
        //            model.musicPlayer.SelectedIndexChanged(eventDescription);
        //            model.brailleDisplayer.SelectedIndexChanged(eventDescription);
        //            model.textDisplayer.SelectedIndexChanged(eventDescription);
        //        }


        //        //public void GotFocus()
        //        //{
        //        //    int index = listBoxTimes.SelectedIndex;
        //        //    Logger.Trace(string.Format("ListBoxTimes_GotFocus(i={0})", index));
        //        //    // Even if we got focus we can not be sure that an item is selected!
        //        //    if (-1 != index)
        //        //    {
        //        //        // If an index is selected do as if Selected Index changed
        //        //        ListBoxTimesIndexChanged(index);
        //        //    }
        //        //}

        //        //public void LostFocus()
        //        //{
        //        //    Logger.Trace("ListBoxTimes_LostFocus");
        //        //    model.StopRefreshingBrailleDevice();
        //        //}

        /// <summary>
        /// Unfortunately listBoxTimes_Enter and listBoxTimes_Enter seem to be needed in order to prevent JAWS from reading the selected line in listBoxTimes
        /// after reading the item from the control we are entering, A better solution is wanted !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Leave(object sender, EventArgs e)
        {
            //string functionName = "Leave";
            listBoxOffsets.BackColor = MusicBrailleReaderMainForm.NonFocusedColor;
            //if (listBoxOffsets.SelectedIndex != -1)
            //{
            //    try
            //    {
            //        // This is new for 1.1.0.0 so better safe than sorry !
            //        Logger.LogCF(string.Format(": Inserting empty line at index={0}", listBoxOffsets.SelectedIndex));
            //        listBoxOffsets.Items.Insert(listBoxOffsets.SelectedIndex, ""); // Insert an empty line in order to make JAWS read it instead of the real line
            //        listBoxOffsets.SelectedIndex--;
            //        listBoxTimesEmptyLineIndex = listBoxOffsets.SelectedIndex;
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.Log(string.Format("{0}.{1} Exception.Message={2}", className, functionName, ex.Message));
            //    }

            //}
        }

        /// <summary>
        /// Unfortunately listBoxTimes_Enter and listBoxTimes_Leave seem to be needed in order to prevent JAWS from reading the selected line in listBoxTimes
        /// after reading the item from the control we are entering, A better solution is wanted !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Enter(object sender, EventArgs e)
        {
            //string functionName = "Enter";
            listBoxOffsets.BackColor = MusicBrailleReaderMainForm.FocusedColor;
            //if (listBoxTimesEmptyLineIndex != -1)
            //{
            //    // listBoxTimes.SelectedIndex = listBoxTimesEmptyLineIndex+1; // Avoid removing the selected item !
            //    try
            //    {
            //        // This is new for 1.1.0.0 so better safe than sorry !
            //        // For the time being we must accept (and catch) an exception here to avoid that JAWS reads the NEXT line after returning !
            //        string line0 = listBoxOffsets.Items[listBoxTimesEmptyLineIndex].ToString();
            //        if (string.IsNullOrEmpty(line0))
            //        {
            //            // If we do not check for the empty line we will remove a line from  newly loaded listbox! 
            //            Logger.LogCF(string.Format(": Removing empty line at listBoxTimes.Items[{0}]", listBoxTimesEmptyLineIndex));
            //            listBoxOffsets.Items.RemoveAt(listBoxTimesEmptyLineIndex);
            //        }
            //        else
            //        {
            //            Logger.LogCF(": No empty line to remove");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (!(ex is ArgumentOutOfRangeException)) // Ignore exception for "index = -1"
            //        {
            //            Logger.Log(string.Format("{0}.{1} Exception.Message={2}", className, functionName, ex.Message));
            //        }
            //    }
            //    listBoxOffsets.SelectedIndex = listBoxTimesEmptyLineIndex; // Select the original selection
            //    listBoxTimesEmptyLineIndex = -1; // Mark that no extra line is inserted
            //}
        }


        //        int listBoxTimesEmptyLineIndex = -1; // Mark that no extra line is inserted



        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxOffsets.SelectedIndex;
            // Model.Trace(string.Format("ListBoxTimes_SelectedIndexChanged(i={0})", index));
            //ListBoxTimesIndexChanged(index);
        }

        //        public void Reset()
        //        {
        //            listBoxTimesEmptyLineIndex = -1; // No empty line inserted
        //            listBoxOffsets.Items.Clear();
        //            listBoxOffsets.Refresh();
        //            listBoxOffsets.Show();
        //        }


        private ListBoxOffsetsHandler(Model model, ListBox listBoxOffsets, bool developerMode)
        {
            this.model = model;
            this.developerMode = developerMode;
            this.listBoxOffsets = listBoxOffsets;
            this.listBoxOffsets.AccessibleRole = AccessibleRole.Default;  // Seems to prevent JAWS from announcing "N of M" when changing line
            this.listBoxOffsets.SelectedIndexChanged += new System.EventHandler(SelectedIndexChanged);
            this.listBoxOffsets.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyDown);
            this.listBoxOffsets.Enter += new System.EventHandler(Enter);
            this.listBoxOffsets.Leave += new System.EventHandler(Leave);
        }

        public static ListBoxOffsetsHandler Create(Model model, ListBox listBoxOffsets, bool developerMode)
        {
            return new ListBoxOffsetsHandler(model, listBoxOffsets,developerMode);
        }

    }
}

