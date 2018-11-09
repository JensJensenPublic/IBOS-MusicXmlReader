using System;
using System.Windows.Forms;
using MusicXmlReaderModel;
using System.Collections.Generic;

namespace MusicXmlReader
{
    /// <summary>
    /// For isolating all functionality related to rendering of "Details" 
    /// </summary>
    public class DetailsHandler
    {
        string className = "MusicXmlReader";
        public enum DetailsEnum { Unknown, Harmonies, Parts, Notes, NotesForPart, Instruments, Status, BrailleFile };
        private ListBox listBoxTimes;
        private ListBox listBoxDetails;
        private Model model;
        private IDebugDisplayerClient client;
        private DetailsEnum currentDetails = DetailsEnum.Unknown;

        // The following 3 variables contain the saved first-level details state when handling second-level details (such as SingleNotes) 
        private List<DetailsDescription> savedItems;
        private int savedIndex = -1;
        private DetailsEnum savedDetails = DetailsEnum.Unknown;
        public enum DetailsDirection { Unknown, FromTop, FromBottom };

        private string Localize(DetailsEnum state)
        {
            string functionName = "Localize";
            switch (state)
            {
                case DetailsEnum.Harmonies: return ResourcesForUI.DetailState_Harmonies;
                case DetailsEnum.Instruments: return ResourcesForUI.DetailState_Instruments;
                case DetailsEnum.Notes: return ResourcesForUI.DetailState_AllParts;
                case DetailsEnum.NotesForPart: return ResourcesForUI.DetailState_SingleNotes;
                case DetailsEnum.Parts: return ResourcesForUI.DetailState_SingleParts;
                case DetailsEnum.Status: return ResourcesForUI.DetailState_Status;
                case DetailsEnum.Unknown: return "";
                default:
                    Logger.Log(string.Format("{0}.{1}: Unsupported value of DetailsEnum:{2}", className, functionName, state));
                    UiUtilities.Beep();
                    return "";
            }
        }


        /// <summary>
        /// Convenience methode for changing the Current details state
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="newDetails"></param>
        private void SetCurrentDetails(string functionName, DetailsEnum newDetails)
        {
            Logger.Log(string.Format("{0}.{1} Changing current details state from {2} to {3}", className, functionName, currentDetails, newDetails));
            currentDetails = newDetails;
            client.WriteStatusInformation(Localize(currentDetails)); // Report detail state through the status line
        }

        /// <summary>
        /// Convenience methode for changing the Current details state
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="newDetails"></param>
        private void SetSavedDetails(string functionName, DetailsEnum newDetails)
        {
            Logger.Log(string.Format("{0}.{1} Changing saved   details state from {2} to {3}", className, functionName, savedDetails, newDetails));
            savedDetails = newDetails;
        }


        private DetailsHandler(ListBox listBoxTimes, ListBox listBoxDetails, Model model, IDebugDisplayerClient client)
        {
            this.listBoxTimes = listBoxTimes;
            this.listBoxDetails = listBoxDetails;
            this.model = model;
            this.client = client;
            this.listBoxDetails.AccessibleName = ""; // Seems to prevent JAWS from announcing "ListBox" at entry
            this.listBoxDetails.AccessibleRole = AccessibleRole.Default; // Seems to prevent JAWS from announcing "N of M" when changing line
            this.listBoxDetails.SelectedIndexChanged += new System.EventHandler(SelectedIndexChanged);
            this.listBoxDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyDown);
            this.listBoxDetails.Leave += new System.EventHandler(Leave);
            this.listBoxDetails.Enter += new System.EventHandler(Enter);
            // Assure that listBoxDetails does not exceed listBoxTimes:
            int height = listBoxTimes.Size.Height;
            this.listBoxDetails.MaximumSize = new System.Drawing.Size(int.MaxValue, height);
            Logger.LogCF(string.Format(": Setting listBoxDetails.MaximumSize to ({0},{1})","int.MaxValue", height));
        }



        /// <summary>
        /// Simple common convenience method for adding items to the listbox.
        /// </summary>
        /// <param name="items">Items to add</param>
        /// <param name="detailsEnum">The type of detail </param>
        /// <param name="detailsDirection">Determines the item initially selected</param>
        private void AddItems(DetailsDescription[] items, DetailsEnum detailsEnum, DetailsDirection detailsDirection,string functionName)
        {
            if (0 == items.Length)
            {
                // Just a fallback ! The detail-implementation can deliver its own one-liner!
                DetailsDescription item = StringDetailsDescription.Create(ResourcesForUI.ListBoxDetails_NoDetailsFound);
                listBoxDetails.Items.Add(item);
            }
            else
            {
                listBoxDetails.Items.AddRange(items);
            }
            // Now items contains at least one item !
            int index = (DetailsDirection.FromTop == detailsDirection) ? 0 : items.Length - 1;
            listBoxDetails.SelectedIndex = index;
            SetCurrentDetails(functionName, detailsEnum);
        }


        /// <summary>
        /// If the details currently shown describes a part, start showing the single notes of the part, either from top or bottom.
        /// Otherwise just pass on to default handling
        /// </summary>
        /// <param name="detailsDirection"></param>
        /// <param name="move" The number of positions that the default handler is expected to move the cursor></param>
        /// <returns>true <==> The keypress should be supporeesd by the default key handler</returns>
        private bool DetailsSingleNotes(DetailsDirection detailsDirection, out int move)
        {
            string functionName = "ShowPartAsSingleNotes";
            //Logger.Log(string.Format("{0}.{1} Entry", className, functionName));
            if (DetailsEnum.Parts != currentDetails)
            {
                // Handle as any other keypress
                switch (detailsDirection)
                {
                    case DetailsDirection.FromTop:    move = +1; break;
                    case DetailsDirection.FromBottom: move = -1; break;
                    default: move = 0; break; 
                }
                return false; // Pass on to default handling: Do not suppress keypress
            }
            else
            {
                // Get the DetailsDescription currently selected by the user:         
                DetailsDescription currentDetailsDescription = (DetailsDescription)listBoxDetails.Items[listBoxDetails.SelectedIndex];
                if (null == currentDetailsDescription)
                {
                    UiUtilities.Hand();
                    Logger.Log(string.Format("{0}.{1}: currentDetailsDescription is null", className, functionName));
                    move = 0;
                    return false;
                }

                // Save the original contents
                savedItems = new List<DetailsDescription>();
                savedIndex = listBoxDetails.SelectedIndex;
                SetSavedDetails(functionName, currentDetails);
                foreach (object o in listBoxDetails.Items)
                {
                    savedItems.Add((DetailsDescription)o);
                }

                DetailsDescription[] newItems = model.GetSingleNoteDetails(currentDetailsDescription);
                listBoxDetails.Items.Clear();
                listBoxDetails.AutoSize = false; // Force the listbox to scrink
                Array.Sort(newItems, Compare);

                AddItems(newItems, DetailsEnum.NotesForPart, detailsDirection,functionName);

                listBoxDetails.AutoSize = true; // Allow listbox to grow to the new size needed

                move = 0;    // Do not check for illegal cursor move. 
                return true; // Suppress the keypress: It has already been fully handled above.
            }
        }

        public int Compare(DetailsDescription x, DetailsDescription y)
        {
            const string functionName = "Compare";
            try
            {
                // throw new Exception("test");
                if ((x is SingleNoteDetailsDescription) && (y is SingleNoteDetailsDescription))
                {
                    NoteElement noteX = (x as SingleNoteDetailsDescription).NoteElement;
                    NoteElement noteY = (y as SingleNoteDetailsDescription).NoteElement;
                    // Put pauses and rests at the bottom of the list (last in list)
                    if ((noteX.UnPitched) || (null == noteX.PitchValue) || (noteX.IsPause)) return +1;
                    if ((noteY.UnPitched) || (null == noteY.PitchValue) || (noteY.IsPause)) return -1;
                    // Both NoteElements describe real, pitched notes!
                    // Put high pitch at the top of the list (first in list)
                    if (noteX.Octave > noteY.Octave) return -1;
                    if (noteX.Octave < noteY.Octave) return +1;
                    // Same octeve
                    if (noteX.Step > noteY.Step) return -1;
                    if (noteX.Step < noteY.Step) return +1;
                }
            }
            catch (Exception e)
            {
                Logger.Log(String.Format("{0}.{1} Exception for x={2} y={3} Message={4}", className, functionName, x.ToString(), y.ToString(), e.Message));

            }
            return 0;
        }



        /// <summary>
        /// Restore the original contents
        /// </summary>
        public void ReturnFromPartDetails()
        {
            string functionName = "ReturnFromPartDetails";
            listBoxDetails.Items.Clear();
            listBoxDetails.AutoSize = false;
            if (null != savedItems)
            {
                foreach (DetailsDescription detailsDescription in savedItems)
                {
                    listBoxDetails.Items.Add(detailsDescription);
                }
            }
            else
            {
                // Logger.Log(string.Format("{0}.{1} savedItems is null", className, functionName));
            }
            savedItems = null;
            if ((0 <= savedIndex) && (savedIndex < listBoxDetails.Items.Count))
            {
                listBoxDetails.SelectedIndex = savedIndex;
            }

            SetCurrentDetails(functionName, savedDetails);
            SetSavedDetails(functionName, DetailsEnum.Unknown);  // Primitive solution! We need a stack of stated if we wan to elaborate further on this !!   

            listBoxDetails.AutoSize = true;
            if (DetailsEnum.Unknown == currentDetails)
            {
                // Restore the original listbox
                ReturnToListboxTimes(0);
            }
        }


        /// <summary>
        /// Shows details which are related to an event in the Details listbox
        /// </summary>
        /// <param name="detailsEnum">Determines which kind of details to show</param>
        /// <param name="fromTop">Show details either from top or bottum</param>
        public void ShowEventDetails(DetailsEnum detailsEnum, DetailsDirection detailsDirection)
        {
            string functionName = "ShowEventDetails     ";
            const bool atNoteLevel = true;
            const bool atPartLevel = false;
            // NOTE ARROW + ALT alone has already been taken by tempo increment/decrement !!!
            if ((DetailsEnum.Harmonies != detailsEnum) && (DetailsEnum.Parts != detailsEnum) && (DetailsEnum.Notes != detailsEnum) && (DetailsEnum.Status != detailsEnum)) return; // This function only supports these sorts of details.
            if (-1 == listBoxTimes.SelectedIndex)
            {
                // It has no meaning to inspect details when nothing is selected !
                return;
            }
            try // This is new code for version 1.0.0.0 so better safe than sorry
            {
                listBoxDetails.Items.Clear();
                object selectedEvent = listBoxTimes.Items[listBoxTimes.SelectedIndex];
                if ((null != selectedEvent) && (selectedEvent is EventDescription))
                {
                    EventDescription currentEventDescription = (listBoxTimes.Items[listBoxTimes.SelectedIndex]) as EventDescription;
                    DetailsDescription[] items = new DetailsDescription[0];
                    switch (detailsEnum)
                    {
                        case DetailsEnum.Parts: items = model.GetCurrentEventDetails(currentEventDescription, atPartLevel); break;//  Show details about current parts
                        case DetailsEnum.Notes: items = model.GetCurrentEventDetails(currentEventDescription, atNoteLevel); break;//  Show details about current parts
                        case DetailsEnum.Harmonies: items = model.GetCurrentHarmonyDetails(currentEventDescription); break; // Show details about the current harmony
                        //case DetailsEnum.Instruments: items = model.GetAllPartDetails(); break;
                        case DetailsEnum.Status: items = model.GetCurrentStatusDetails(currentEventDescription); break;
                        default: break;
                    };

                    AddItems(items, detailsEnum, detailsDirection, functionName);
                    LeaveListboxTimes();
                    listBoxDetails.Focus();
                }
                else
                {
                    Logger.Log(string.Format("{0}.{1}: No event found",className,functionName));
                    UiUtilities.Beep();
                }
                // e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control.
            }
            catch (Exception exception)
            {
                UiUtilities.Beep();
                Logger.Log(string.Format("{0}.{1} ({2},{3}) threw an exception: Message={4}", className, functionName, detailsEnum, detailsDirection.ToString(), exception.Message));
            }
            return;
        }

        /// <summary>
        /// Show global details, i.e. details which are not related to a specific event, but are global for the whole score,
        /// such the list of instruments.
        /// </summary>
        /// <param name=""></param>
        /// <param name="fromTop"></param>
        public void ShowGlobalDetails(DetailsEnum detailsEnum, DetailsDirection detailsDirection)
        {
            string functionName = "ShowGlobalDetails";
            try
            {
                DetailsDescription[] items = null;
                switch (detailsEnum)
                {
                    // This function only supports these sorts of details:
                    case DetailsEnum.Instruments: items = model.GetAllPartDetails(); break;
                    case DetailsEnum.BrailleFile:  items = model.GetBrailleFileDetails(); break;
                    default: Logger.LogCF(string.Format(": Unsupported detail '{0}'", detailsEnum.ToString()));     return;
                }
                if (null == items)
                {
                    UiUtilities.Beep();
                    Logger.LogCF(string.Format(": No items found  for DetailsEnum='{0}'", detailsEnum.ToString()));
                    return;
                }

                listBoxDetails.Items.Clear();          
                AddItems(items, detailsEnum, detailsDirection,functionName);
                LeaveListboxTimes();
                listBoxDetails.Focus();
            }
            catch (Exception exception)
            {
                Logger.Log(string.Format("{0}.{1} ({2},{3}) threw an exception: Message={4}", className, functionName, detailsEnum, detailsDirection.ToString(), exception.Message));
            }
            return;
        }


        private void ReturnToListboxTimes(int move)
        {
            // On All other keys will return focus to the mail listbox
            listBoxDetails.Items.Clear();
            int selectedIndex = listBoxTimes.SelectedIndex;
            int newIndex = selectedIndex + move;
            if ((newIndex >= 0) && (newIndex < listBoxTimes.Items.Count))
            {
                // Select the next detail if possible
                listBoxTimes.SelectedIndex = newIndex;
            }
            // In all other cases just return to the original index and move focus  
            listBoxDetails.AutoSize = false; // Stop using the area temporarily borrowed from ListBoxTimes
            listBoxTimes.Focus();
        }



        /// <summary>
        /// Called from mainForm.listBoxDetails_KeyDown(object sender, KeyEventArgs e)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeyDown(object sender, KeyEventArgs e)
        {
            string functionName = "listBoxDetails_KeyDown";
            try // This is new code for version 1.0.0.0 so better safe than sorry
            {
                int move = 0;
                switch (e.KeyData)
                {
                    // On keys.Right and keys.Left, Key.Home, Key.End: Do nothing special, but pass the key to the listbox without suppressing it !
                    case ShortcutHandler.detailsSingleNotesFromTop:    e.SuppressKeyPress = DetailsSingleNotes(DetailsDirection.FromTop, out move); break;                                                                                                  
                    case ShortcutHandler.detailsSingleNotesFromBottom: e.SuppressKeyPress = DetailsSingleNotes(DetailsDirection.FromBottom, out move ); break;
                    //case ShortcutHandler.detailsOpenSingleNotesFromTop:    ShowPartAsSingleNotes(true); e.SuppressKeyPress = true; break; // Same semantics as in a tree: Expand details !
                    //case ShortcutHandler.detailsOpenSingleNotesFromBottom: ShowPartAsSingleNotes(false); e.SuppressKeyPress = true; break; // Same semantics as in a tree: Expand details !
                    case ShortcutHandler.DetailsNextPart:       move = +1; break;  // (Down)  Pass on to default handler
                    case ShortcutHandler.DetailsPreviousPart:   move = -1; break;   // (Up)    Pass on to default handler
                    case ShortcutHandler.detailsTopDetail: break; // Pass on to default handler
                    case ShortcutHandler.detailsBottomDetail: break; // Pass on to default handler
                    case ShortcutHandler.detailsNextEvent:      ReturnToListboxTimes(0); e.SuppressKeyPress = true; break; // +1 confuses JAWS
                    case ShortcutHandler.detailsPreviousEvent:  ReturnToListboxTimes(0); e.SuppressKeyPress = true; break;  // -1 confuses JAWS
                    case ShortcutHandler.DetailsStatusBottom:   e.SuppressKeyPress = true; UiUtilities.Beep(); break; // Ignore
                    case ShortcutHandler.DetailsStatusTop:      e.SuppressKeyPress = true; UiUtilities.Beep(); break; // Ignore
                    case ShortcutHandler.DetailsHarmonyBottom:  e.SuppressKeyPress = true; UiUtilities.Beep(); break; // Ignore
                    case ShortcutHandler.DetailsHarmonyTop:     e.SuppressKeyPress = true; UiUtilities.Beep(); break; // Ignore
                    case Keys.Control | Keys.ControlKey: e.SuppressKeyPress = true; break; // Allow for decoding CTRL+UP and CTRL+DOWN later
                    case Keys.Shift | Keys.ShiftKey: e.SuppressKeyPress = true; break; // Allow for decoding SHIFT+?? and SHIFT+?? later
                    case Keys.Escape:    ReturnFromPartDetails(); e.SuppressKeyPress = true; break; // TEST
                    default: ReturnToListboxTimes(0); e.SuppressKeyPress = true; break;
                }

                UiUtilities.WarnAtEnd(listBoxDetails, move); // Warn if the user attempts to move out of  the list
            }
            catch (Exception exception)
            {
                Logger.Log(string.Format("{0}.{1} KeyCode={2} threw an exception: Message={3}", className, functionName, e.KeyCode.ToString(), exception.Message));
            }
            return;
        }

        private void Leave(object sender, EventArgs e)
        {
            listBoxDetails.BackColor = MainForm.NonFocusedColor;
            model.ListBoxDetailsLeave();
            listBoxDetails.Items.Clear();
            listBoxDetails.AutoSize = false;
            listBoxTimes.Show();
        }

        private void Enter(object sender, EventArgs e)
        {
            listBoxDetails.BackColor = MainForm.FocusedColor;
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxDetails.SelectedIndex;
            object o = listBoxDetails.Items[index];
            DetailsDescription detailsDescription = o as DetailsDescription;
            model.SelectedDetailsIndexChanged(detailsDescription);
        }


        private void LeaveListboxTimes()
        {
            listBoxDetails.AutoSize = true; // Use the area normally occupied by listBoxTimes
        }

        static public DetailsHandler Create(ListBox listBoxTimes, ListBox listBoxDetails, Model model, IDebugDisplayerClient client)
        {
            return new DetailsHandler(listBoxTimes,listBoxDetails,model,client);
        }

    }
}
