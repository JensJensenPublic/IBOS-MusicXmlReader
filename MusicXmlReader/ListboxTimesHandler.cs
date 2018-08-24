using System;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    public class ListBoxTimesHandler
    {
        private string className = "ListBoxTimesHandler";

  
        private Model model;
        private DetailsHandler detailsHandler;
        private ListBox listBoxTimes;
        private bool autoReload;    // Used to optimize performance when changing large parts of the UI within short time
        public  bool AutoReload { get { return autoReload; }  set { autoReload = value; } }

        public void Focus()
        {
            listBoxTimes.Focus();
        }

        public int GetNumberOfObjects()
        {
            return listBoxTimes.Items.Count;
        }

        delegate object GetObjectAtIndexCallback(int index);
        public object GetObjectAtIndex(int index)
        {
            string functionName = "GetObjectAtIndex";
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
#if true
            // If we don't need InvokeRequired we can also skip moving SelectedIndex back and forth (line 825)
            if (listBoxTimes.InvokeRequired)
            {
                GetObjectAtIndexCallback d = new GetObjectAtIndexCallback(GetObjectAtIndex);
                return listBoxTimes.Invoke(d, new object[] { index });
            }
            else
#endif
            {
                if ((index < 0) || (index >= listBoxTimes.Items.Count))
                {
                    Logger.Log(string.Format("{0}.{1} Index out of range:{2}", className, functionName, index));
                    return null;
                }
                return listBoxTimes.Items[index];
            }
        }

        delegate void SetSelectedIndexCallback(int index);
        public void SetSelectedIndex(int index)
        {
            const string functionName = "SetSelectedIndex";
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBoxTimes.InvokeRequired)
            {
                SetSelectedIndexCallback d = new SetSelectedIndexCallback(SetSelectedIndex);
                listBoxTimes.Invoke(d, new object[] { index });
            }
            else
            {
                //listBoxTimes.Focus(); // Maybe not needed. How can we force the Screeen-reader to read the selected line?
                if (listBoxTimes.Items.Count > index) // Prevent crash during program exit
                {
                    listBoxTimes.SelectedIndex = index;
                }
                else
                {
                    Logger.Log(string.Format("{0}.{1} Attempted to set index={2} when Items.Count={3}", className, functionName, index, listBoxTimes.Items.Count));
                }
                // System.Threading.Thread.Sleep(100); // HACK Pause the UI thread and let the Screenreader get a chance
            }
        }


        public void ConditionalLoad()
        {
            if (autoReload)
            {
                Load();
            }
        }




        /// <summary>
        /// Load the main listbox with information fetched from the Model
        /// First all metainformation (Composer, Author, etc) NO ! Se below !!
        /// Then all the events describing the music sheet itself
        /// </summary>
        public void Load()
        {
            int selectedIndex = listBoxTimes.SelectedIndex;  // Save index
            listBoxTimes.Items.Clear();
            // Move the Meta information somewhere else !
            // We only want EventDescriptions here !
            //foreach (string s in model.MetaInfoStrings)
            //{
            //    listBoxTimes.Items.Add(s);
            //}
            foreach (EventDescription eventDescription in model.EventDescriptionList.Events)
            {
                listBoxTimes.Items.Add(eventDescription);
            }
            // Restore index without exceeding values
            listBoxTimes.SelectedIndex = Math.Min(selectedIndex, listBoxTimes.Items.Count);
        }




        /// <summary>
        /// Generate an audible Beep if the listBox is empty
        /// </summary>
        /// <param name="listBox"></param>
        private void WarnIfEmpty(ListBox listBox)
        {
            if (0 == listBox.Items.Count)
            {
                UiUtilities.Beep();
            }
        }

    

        /// <summary>
        /// Occurs when a key is pressed while listBoxTimes has focus
        /// </summary>
        /// <param name="e"></param>
        private void KeyDown(object sender,KeyEventArgs e)
        {
            // string functionName = "listBoxTimes_KeyDown";

            bool handled = true; // Will be set to false again by the "default:" case if the key is not handled  by one of the specific cases.
            bool warnIfEmpty = true; // Will be set to false on commands that do not require that a MusicXml file is loaded.
            DetailsHandler.DetailsDirection FromTop = DetailsHandler.DetailsDirection.FromTop;
            DetailsHandler.DetailsDirection FromBottom = DetailsHandler.DetailsDirection.FromBottom;
            switch (e.KeyData) // KeyDate contains information about the control keys (<CTRL> <ALT> <SHIFT>  etc )as well as about the normal ley
            {
                // Most functionality is passed directly to the Model
                case ShortcutHandler.togglePlaying: model.ToggleStartStopPlaying(listBoxTimes.SelectedIndex); break; // Keys.Space
                case ShortcutHandler.tempoDecrement: model.ChangeUserTempo(-1); break;
                case ShortcutHandler.tempoIncrement: model.ChangeUserTempo(+1); break;
                case ShortcutHandler.startPlaying: model.StartPlayingPoly(listBoxTimes.SelectedIndex); break;
                case ShortcutHandler.stopPlaying: model.StopPlaying(); break;
                case ShortcutHandler.StopAllNotesPlaying: model.musicPlayer.StopAllNotesPlaying(); warnIfEmpty = false; break;
                case ShortcutHandler.PreviousMeasure: model.SelectMeasure(listBoxTimes.SelectedIndex, -1); break;
                case ShortcutHandler.NextMeasure: model.SelectMeasure(listBoxTimes.SelectedIndex, +1); break;
                case ShortcutHandler.NextEvent: UiUtilities.WarnAtEnd(listBoxTimes, +1); handled = false; break; // Let the listbox handle it
                case ShortcutHandler.PreviousEvent: UiUtilities.WarnAtEnd(listBoxTimes, -1); handled = false; break; // Let the listbox handle it



                // The "Details functionality is handled locally before being passed to the Model:          
                case ShortcutHandler.DetailsHarmonyTop: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Harmonies, FromTop); break;    // Start from top
                case ShortcutHandler.DetailsPartsTop: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Parts, FromTop); break;        // Start from top  
                case ShortcutHandler.DetailsNotesTop: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Notes, FromTop); break;        // Start from top  
                case ShortcutHandler.DetailsStatusTop:  detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Status, FromTop); break;
                case ShortcutHandler.DetailsHarmonyBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Harmonies, FromBottom); break;   // Start from bottom
                case ShortcutHandler.DetailsPartsBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Parts, FromBottom); break;       //  Start from bottom
                case ShortcutHandler.DetailsNotesBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Notes, FromBottom); break;       //  Start from bottom
                case ShortcutHandler.DetailsStatusBottom: detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Status, FromBottom); break;
                // DetailsInstruments are handled directly from MenuLine->View because they do not depend on which item is selected in the NoteList.
                //case ShortcutHandler.DetailsInstruments:    detailsHandler.ShowGlobalDetails(DetailsHandler.DetailsEnum.Instruments, true); break;  // Always shown from top
                //case ShortcutHandler.DetailsInstrumentsButtom:  ShowDetails(DetailsEnum.Instruments, false); break;

                default:
                    handled = false;        // This event must be handled either by the CommandInterpreter or by the Listbox itself
                    warnIfEmpty = false;    // Do not issue a warning beep even if no valid MusicXml file is loaded,
                    break;
            }

            if (warnIfEmpty)
            {
                // Issue a warning if no data is loaded and the command thus has no meaning
                WarnIfEmpty(listBoxTimes);
            }

            if (handled)
            {
                e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control.
                return;
            };

            //// Let the command interpreter handle it 
            //commandInterpreter.Add(e);

        }


        private void ListBoxTimesIndexChanged(int index)
        {
            object o = listBoxTimes.Items[index];
            EventDescription eventDescription = o as EventDescription;
            model.musicPlayer.SelectedIndexChanged(eventDescription);
            model.brailleDisplayer.SelectedIndexChanged(eventDescription);
            model.textDisplayer.SelectedIndexChanged(eventDescription);
        }


        //public void GotFocus()
        //{
        //    int index = listBoxTimes.SelectedIndex;
        //    Logger.Trace(string.Format("ListBoxTimes_GotFocus(i={0})", index));
        //    // Even if we got focus we can not be sure that an item is selected!
        //    if (-1 != index)
        //    {
        //        // If an index is selected do as if Selected Index changed
        //        ListBoxTimesIndexChanged(index);
        //    }
        //}

        //public void LostFocus()
        //{
        //    Logger.Trace("ListBoxTimes_LostFocus");
        //    model.StopRefreshingBrailleDevice();
        //}

        /// <summary>
        /// Unfortunately listBoxTimes_Enter and listBoxTimes_Enter seem to be needed in order to prevent JAWS from reading the selected line in listBoxTimes
        /// after reading the item from the control we are entering, A better solution is wanted !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  void Leave(object sender, EventArgs e)
        {
            string functionName = "Leave";
            listBoxTimes.BackColor = MainForm.NonFocusedColor;
            if (listBoxTimes.SelectedIndex != -1)
            {
                try
                {
                    // This is new for 1.1.0.0 so better safe than sorry !
                    listBoxTimes.Items.Insert(listBoxTimes.SelectedIndex, ""); // Insert an empty line in order to make JAWS read it instead of the real line
                    listBoxTimes.SelectedIndex--;
                    listBoxTimesEmptyLineIndex = listBoxTimes.SelectedIndex;
                }
                catch (Exception ex)
                {
                    Logger.Log(string.Format("{0}.{1} Exception.Message={2}", className, functionName, ex.Message));
                }

            }
        }

        /// <summary>
        /// Unfortunately listBoxTimes_Enter and listBoxTimes_Enter seem to be needed in order to prevent JAWS from reading the selected line in listBoxTimes
        /// after reading the item from the control we are entering, A better solution is wanted !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Enter(object sender, EventArgs e)
        {
            string functionName = "Enter";
            listBoxTimes.BackColor = MainForm.FocusedColor;
            if (listBoxTimesEmptyLineIndex != -1)
            {
                // listBoxTimes.SelectedIndex = listBoxTimesEmptyLineIndex+1; // Avoid removing the selected item !
                try
                {
                    // This is new for 1.1.0.0 so better safe than sorry !
                    // For the time being we must accept (and catch) an exception here to avoid that JAWS reads the NEXT line after returning !
                    listBoxTimes.Items.RemoveAt(listBoxTimesEmptyLineIndex);
                }
                catch (Exception ex)
                {
                    if (!(ex is ArgumentOutOfRangeException)) // Ignore exception for "index = -1"
                    {
                        Logger.Log(string.Format("{0}.{1} Exception.Message={2}", className, functionName, ex.Message));
                    }
                }
                listBoxTimes.SelectedIndex = listBoxTimesEmptyLineIndex; // Select the original selection
                listBoxTimesEmptyLineIndex = -1; // Mark that no extra line is inserted
            }
        }
        

        int listBoxTimesEmptyLineIndex = -1; // Mark that no extra line is inserted



        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxTimes.SelectedIndex;
            // Model.Trace(string.Format("ListBoxTimes_SelectedIndexChanged(i={0})", index));
            ListBoxTimesIndexChanged(index);
        }

        public void Reset()
        {
            listBoxTimesEmptyLineIndex = -1; // No empty line inserted
            listBoxTimes.Items.Clear();
            listBoxTimes.Refresh();
            listBoxTimes.Show();
        }
        

        private ListBoxTimesHandler(Model model, DetailsHandler detailsHandler, ListBox listBoxTimes)
        {
            this.model = model;
            this.detailsHandler = detailsHandler;
            this.listBoxTimes = listBoxTimes;
            this.listBoxTimes.AccessibleName = ResourcesForUI.ListView_Accessible_Name;
            this.listBoxTimes.AccessibleRole = AccessibleRole.Default;  // Seems to prevent JAWS from announcing "N of M" when changing line
            this.listBoxTimes.SelectedIndexChanged += new System.EventHandler(SelectedIndexChanged);
            this.listBoxTimes.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyDown);
            this.listBoxTimes.Enter += new System.EventHandler(Enter);
            this.listBoxTimes.Leave += new System.EventHandler(Leave);
        }

        public static ListBoxTimesHandler Create(Model model, DetailsHandler detailsHandler, ListBox listBoxTimes )
        {
            return new ListBoxTimesHandler(model,detailsHandler, listBoxTimes);
        }

    }
}
