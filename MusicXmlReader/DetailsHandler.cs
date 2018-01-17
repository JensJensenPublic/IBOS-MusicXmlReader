using System;
using System.Windows.Forms;
using MusicXmlReaderModel;
using System.Collections.Generic;

namespace MusicXmlReader
{
    /// <summary>
    /// For isolating all functionality related to rendering of "Details" 
    /// </summary>
    class DetailsHandler
    {
        string className = "MusicXmlReader";
        public enum DetailsEnum { Unknown, Harmonies, Parts, Notes, NotesForPart, Instruments };
        private ListBox listBoxTimes;
        private ListBox listBoxDetails;
        private Model model;
        private IDebugDisplayerClient client;
        private DetailsEnum currentDetails = DetailsEnum.Unknown;

        // The following 3 variables contain the saved first-level details state when handling second-level details (such as SingleNotes) 
        private List<DetailsDescription> savedItems;
        private int savedIndex = -1;
        private DetailsEnum savedDetails = DetailsEnum.Unknown;

        private string Localize(DetailsEnum state)
        {
            switch (state)
            {
                case DetailsEnum.Harmonies: return "";
                case DetailsEnum.Instruments: return "";
                case DetailsEnum.Notes: return ResourcesForUI.DetailState_AllParts;
                case DetailsEnum.NotesForPart: return ResourcesForUI.DetailState_SingleNotes;
                case DetailsEnum.Parts: return ResourcesForUI.DetailState_SingleParts;
                case DetailsEnum.Unknown: return "";
                default: return "";
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
        

        private DetailsHandler(ListBox listBoxTimes, ListBox listBoxDetails, Model model,IDebugDisplayerClient client)
        {
            this.listBoxTimes = listBoxTimes;
            this.listBoxDetails = listBoxDetails;
            this.model = model;
            this.client = client;
        }



        //DetailsDescription currentDetailsDescription;



        /// <summary>
        /// If the details currently shown describes a part, start showing the single notes of the part, either from top or bottom.
        /// Otherwise just ignore and log an error.
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public int ShowPartAsSingleNotes(bool fromTop)
        {      
            string functionName = "ShowPartAsSingleNotes";
            //Logger.Log(string.Format("{0}.{1} Entry", className, functionName));
            if (DetailsEnum.Parts != currentDetails)
            {
                // This function should not be called in this case !
                UiUtilities.Hand();
                Logger.Log(string.Format("{0}.{1}: Error: CurrentDetails= {2}", className, functionName, currentDetails));
                return 0 ; 
            }
            else
            {
                // Get the DetailsDescription currently selected by the user:         
                DetailsDescription currentDetailsDescription = (DetailsDescription)listBoxDetails.Items[listBoxDetails.SelectedIndex];
                if (null == currentDetailsDescription)
                {
                    UiUtilities.Hand();
                    Logger.Log(string.Format("{0}.{1}: currentDetailsDescription is null", className, functionName));
                    return 0;
                }

                // Save the original contents
                savedItems = new List<DetailsDescription>();
                savedIndex = listBoxDetails.SelectedIndex;
                SetSavedDetails(functionName,currentDetails);
                foreach (object o in listBoxDetails.Items)
                {
                    savedItems.Add((DetailsDescription)o);
                }
                
                DetailsDescription[] newItems = model.GetSingleNoteDetails(currentDetailsDescription);
                listBoxDetails.Items.Clear();
                listBoxDetails.AutoSize = false; // Force the listbox to scrink

                //Load the new items
                foreach (DetailsDescription detailsDescription in newItems)
                {
                    listBoxDetails.Items.Add(detailsDescription);
                }
                SetCurrentDetails(functionName,DetailsEnum.NotesForPart);
                listBoxDetails.AutoSize = true; // Allow listbox to grow to the new size needed
                listBoxDetails.SelectedIndex = fromTop ? 0 : listBoxDetails.Items.Count - 1;
                return 0 ;
            }
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
        }


        /// <summary>
        /// Shows details which are related to an event in the Details listbox
        /// </summary>
        /// <param name="detailsEnum">Determines which kind of details to show</param>
        /// <param name="fromTop">Show details either from top or bottum</param>
        public void ShowEventDetails(DetailsEnum detailsEnum, bool fromTop)
        {
            string functionName = "ShowEventDetails";
            // NOTE ARROW + ALT alone has already been taken by tempo increment/decrement !!!
            if ((DetailsEnum.Harmonies != detailsEnum) && (DetailsEnum.Parts != detailsEnum) && (DetailsEnum.Notes != detailsEnum)) return; // This function only supports these sorts of details.
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
                        case DetailsEnum.Parts: items = model.GetCurrentEventDetails(currentEventDescription, false); break;//  Show details about current parts
                        case DetailsEnum.Notes: items = model.GetCurrentEventDetails(currentEventDescription, true); break;//  Show details about current parts
                        case DetailsEnum.Harmonies: items = model.GetCurrentHarmonyDetails(currentEventDescription); break; // Show details about the current harmony
                        case DetailsEnum.Instruments: items = model.GetAllPartDetails(); break;
                        default: break;
                    };

                    listBoxDetails.Items.AddRange(items);
                    int itemCount = listBoxDetails.Items.Count;
                    if (0 != itemCount)
                    {
                        listBoxDetails.SelectedIndex = fromTop ? 0 : (itemCount - 1);
                    }
                }

                if (0 == listBoxDetails.Items.Count) // For whatever reason
                {
                    listBoxDetails.Items.Add(ResourcesForUI.ListBoxDetails_NoDetailsFound); // Just a fallback ! The detail-implementation can deliver its own one-liner!
                }

                LeaveListboxTimes();
                listBoxDetails.Focus();                
                SetCurrentDetails(functionName,detailsEnum);

                // e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control.
            }
            catch (Exception exception)
            {
                Logger.Log(string.Format("{0}.{1} ({2},{3}) threw an exception: Message={4}", className, functionName, detailsEnum, fromTop, exception.Message));
            }
            return;
        }



        /// <summary>
        /// Show global details, i.e. details which are not related to a specific event, but are global for the whole score,
        /// such the list of instruments.
        /// </summary>
        /// <param name=""></param>
        /// <param name="fromTop"></param>
        public void ShowGlobalDetails(DetailsEnum detailsEnum, bool fromTop)
        {
            string functionName = "ShowGlobalDetails";
            try
            {
                if (DetailsEnum.Instruments != detailsEnum) return; // This function only supports these sorts of details.
                listBoxDetails.Items.Clear();
                DetailsDescription[] items = model.GetAllPartDetails();

                listBoxDetails.Items.AddRange(items);
                int itemCount = listBoxDetails.Items.Count;
                if (0 != itemCount)
                {
                    listBoxDetails.SelectedIndex = fromTop ? 0 : (itemCount - 1);
                }

                if (0 == listBoxDetails.Items.Count) // For whatever reason
                {
                    listBoxDetails.Items.Add(ResourcesForUI.ListBoxDetails_NoDetailsFound); // Just a fallback ! The detail-implementation can deliver its own one-liner!
                }

                LeaveListboxTimes();
                listBoxDetails.Focus();
            }
            catch (Exception exception)
            {
                Logger.Log(string.Format("{0}.{1} ({2},{3}) threw an exception: Message={4}", className, functionName, detailsEnum, fromTop, exception.Message));
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
                    case ShortcutHandler.detailsSingleNotesFromTop:                 
                        if (DetailsEnum.Parts == currentDetails) 
                        {
                            ShowPartAsSingleNotes(true) ;
                            e.SuppressKeyPress = true; // Do NOT pass on to default handler
                        }
                        else
                        {
                            move = +1; //  Pass on to default handler 
                        }
                        break; // (Down)  After that: Pass on to default handler                                                                                                   
                    case ShortcutHandler.detailsSingleNotesFromBottom:  
                        if (DetailsEnum.Parts == currentDetails)
                        {  
                            ShowPartAsSingleNotes(false);
                            e.SuppressKeyPress = true; // Do NOT pass on to default handler
                        }
                        else
                        {
                            move = -1; //  Pass on to default handler 
                        }
                        break;
                    //case ShortcutHandler.detailsOpenSingleNotesFromTop:    ShowPartAsSingleNotes(true); e.SuppressKeyPress = true; break; // Same semantics as in a tree: Expand details !
                    //case ShortcutHandler.detailsOpenSingleNotesFromBottom: ShowPartAsSingleNotes(false); e.SuppressKeyPress = true; break; // Same semantics as in a tree: Expand details !
                    case ShortcutHandler.DetailsNextPart:       move = +1; break;  // (Down)  Pass on to default handler
                    case ShortcutHandler.DetailsPreviousPart:   move = -1; break;   // (Up)    Pass on to default handler
                    case ShortcutHandler.detailsTopDetail: break; // Pass on to default handler
                    case ShortcutHandler.detailsBottomDetail: break; // Pass on to default handler
                    case ShortcutHandler.detailsNextEvent: ReturnToListboxTimes(0); e.SuppressKeyPress = true; break; // +1 confuses JAWS
                    case ShortcutHandler.detailsPreviousEvent: ReturnToListboxTimes(0); e.SuppressKeyPress = true; break;  // -1 confuses JAWS
                    case Keys.Control | Keys.ControlKey: e.SuppressKeyPress = true; break; // Allow for decoding CTRL+UP and CTRL+DOWN later
                    case Keys.Shift  |  Keys.ShiftKey:   e.SuppressKeyPress = true; break; // Allow for decoding SHIFT+?? and SHIFT+?? later
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
