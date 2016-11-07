using System;
using System.Windows.Forms;
using System.Globalization;
using MusicXmlReaderUI;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// The 3 interfaces are used for
    /// IBrailleDisplayerClient     Let the Model write MusicBraille patterns to the appropriate Textbox
    /// IObjectCollection   Let the Model access the main Listbox when auto-playing music
    /// IUtilityClient      Let the Model show MessageBoxes
    /// By using these interfaces we avoid that the Model needs to know anything abour Windows Forms!
    /// This makes it much easier to reuse the Model for othea applications and other platforms.
    /// </summary>
    public partial class MainForm : Form, IDebugDisplayerClient, IObjectCollection, IUtilityClient
    {

        enum MusicPlayerStateEnum { unknown, stopped, running };
        string ApplicationName = "";  // Application name. Will be re-initialized later using localization!
        Model model;        // The Model containing all of the business logic.        
        bool autoReload;    // Used to optimize performance when changing large parts of the UI within short time
        UserSettingsHandler userSettingsHandler; // Contains all settings that can be configured by the user
        MusicPlayerStateEnum musicPlayerState = MusicPlayerStateEnum.stopped; // Assume the musicplayer is innitially stopped
        ShortcutHandler shortCutHandler;

        public MainForm()
        {
            InitializeComponent();
            Logger.Open("MusicXmlReaderUI.log");
            Application.ApplicationExit += Application_ApplicationExit;
            LogSystemInformation();
            // If the execution directory contains a file named "Language.txt" containing the string "en-US"
            // the application language will be changed to english evin if running on a danish PC!
            LogGLobalisationInformation();
            // Do any UI localization before we create the model. In this way we avoid showing unlocalized texts
            // if an error is reported by a messagebox.
            musicPlayerState = MusicPlayerStateEnum.stopped;
            LocalizeStartStopButton(musicPlayerState);
            LocalizeMenuStrip(); // Overwrite all items in MenuStrip with localized texts

            ApplicationName = ResourcesForUI.MainForm_ApplicationName;
            Utilities.UtilityClient = (this as IUtilityClient); //Decide how to show error messages and warnings 
            model = Model.Create((this as IObjectCollection), (this as IDebugDisplayerClient), ApplicationName);
            this.Text = ApplicationName;

            // Create a handler for the user settinge, in this case modelled as a treeview.
            userSettingsHandler = UserSettingsHandler.Create(this,this.userSettingsTreeView,model);
            userSettingsHandler.Init(); // Builds up the fixed part of the treeview
            userSettingsTreeView.CollapseAll();
            // Create a handler for handling all Keyboard shortcuts
            shortCutHandler = ShortcutHandler.Create(this, model);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            model.OnApplicationExit(); // Let the Model clean up its resources etc 
        }

        void LocalizeMenuStrip()
        {
            //MenuStrip.Text = "??";
            // Children of MenuStrip
            filesToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files;
            editToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Edit; 
            viewToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_View; 
            toolsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools;
            helpToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help ; 

            // Children of  fileToolStripMenuItem
            openMusicXmlFileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile; ;
            openMusicXmlFileToolStripMenuItem.ShortcutKeys = ShortcutHandler.openMusicXmlFile;
            exitToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_Exit;
            exitToolStripMenuItem.ShortcutKeys = ShortcutHandler.exitApplication;

            // Children of editToolStripMenuItem
            // Texts:  NOTE! Use the same texts as used in the treeview to which these items refer!!
            allItemsToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_All_Items;
            musicRepresentationToolStripMenuItem.Text = "&"+ResourcesForUI.TreeView_MusicAsSound;
            textRepresentationToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsSpeech;
            brailleRepresentationToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsBraille;
            partsToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsSound_Parts;
            detailsToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsSound_Details;
            // Shortcuts
            allItemsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editAllItems;
            musicRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editMusic;
            textRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editText;
            brailleRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editBraille;
            partsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editParts;
            detailsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editDetails;

           // Children of  toolsToolStripMenuItem
            museScoreToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_MuseScore;
            sibeliusToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Sibelius;
            logfileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Logfile;
            openXMLFileLocationToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_OpenXmlFileLocation;
            openLogFileLocationToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Logfile_Location;
            inspectAsXMLToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml;
            viewAsInterpretedXMLToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_ViewAsInterpretedXml;
            saveAsTextToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_SaveAsText;
        }



        void LocalizeStartStopButton(MusicPlayerStateEnum musicPlayerState)
        {
            switch (musicPlayerState)
            {
                case MusicPlayerStateEnum.running: buttonStart.Text = ResourcesForUI.ButtonStart_StopPlaying;  break;
                case MusicPlayerStateEnum.stopped: buttonStart.Text = ResourcesForUI.ButtonStart_StartPlaying; break;
                default: break;
            }
        }

        #region supportcode


        public static void LogSystemInformation()
        {
            Logger.Log(string.Format("ComputerName={0} UserName={1} UserDomainName={2}",
                SystemInformation.ComputerName, SystemInformation.UserName, SystemInformation.UserDomainName));
            Logger.Log(string.Format("OSVersion={0} ProcessorCount={1} Is64BitOperatingSystem={2} Is64BitProcess={3}",
            System.Environment.OSVersion, System.Environment.ProcessorCount, System.Environment.Is64BitOperatingSystem, System.Environment.Is64BitProcess));
        }


        /// <summary>
        /// Log information and implement a temporary mechanism for overwriting the locale on the machine
        /// by placing a simple textfile in the executing directory
        /// </summary>
        public static void LogGLobalisationInformation()
        {
            try
            {
                string currentCultureName = CultureInfo.CurrentUICulture.Name;
                Logger.Log(string.Format("CultureInfo.CurrentUICulture.Name={0}", currentCultureName));
                string LanguageFileName = (System.IO.Path.Combine(System.Environment.CurrentDirectory, "Language.txt"));
                if (System.IO.File.Exists(LanguageFileName))
                {
                    string newCultureName = System.IO.File.ReadAllText(LanguageFileName);   
                    Logger.Log(string.Format("Changing UICulture for UI thread to {0}", newCultureName));
                    System.Threading.Thread thisThread = System.Threading.Thread.CurrentThread;
                    thisThread.CurrentUICulture = new CultureInfo(newCultureName);
                    Logger.Log(string.Format("thisThread.CurrentUICulture={0}", thisThread.CurrentUICulture.Name));
                }

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("LogGLobalisationInformation threw an exception. Message=}0}", e.Message));
            }

        }

        /// <summary>
        /// Assume that a string contains Braille if it is not empty and the first char is a Braille char
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool isBraille(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            char c = s[0];
            return ((0x2800 <= c) && (c <= 0x28ff));
        }

        #region IBrailleDisplayerClient

        public void WriteNormalTextString(string s)
        {
            textBoxNormalText.Text = s;
        }


        public void WriteBrailleString(string s)
        {
                textBoxBraille.Text = s;
        }

        public void WriteTextString(string s)
        {
            textBoxText.Text = s;
        }
        #endregion

        #region  IObjectCollection

        public int GetNumberOfObjects()
        {
            return listBoxTimes.Items.Count;
        }

        public object GetObjectAtIndex(int index)
        {
            return listBoxTimes.Items[index];
        }

        delegate void SetSelectedIndexCallback(int index);
        public void SetSelectedIndex(int index)
        {
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
                listBoxTimes.SelectedIndex = index;
                // System.Threading.Thread.Sleep(100); // HACK Pause the UI thread and let the Screenreader get a chance
            }
        }
        #endregion

        #region IMessageShower 
        // Decide how to show error messages and warnings          
        public void ShowMessage(int messageId,string parameter, string text)
        {
            MessageBox.Show(text);
        }



        private string LocalizeMessage(ModelMessageEnum messageEnum)
        {
            switch (messageEnum)
            {
                case ModelMessageEnum.DirectoryNotFound: return ResourcesForUI.Message_DirectoryNotFound;
                case ModelMessageEnum.FailedToConnectToScreenReader: return ResourcesForUI.Message_FailedToConnectToScreenReader;
                case ModelMessageEnum.ConnectedToNonDefaultScreenReader: return ResourcesForUI.Message_ConnectedToNonDefaultScreenReader;
                case ModelMessageEnum.FailedToStartProgram: return ResourcesForUI.Message_FailedToStartProgram;
                case ModelMessageEnum.FileNotFound: return ResourcesForUI.Message_FileNotFound;
                case ModelMessageEnum.MissingProgramFile: return ResourcesForUI.Message_MissingProgramFile;
                case ModelMessageEnum.FailedToReadMusicXmlFile: return ResourcesForUI.Message_FailedToReadMusicXmlFile;
                case ModelMessageEnum.UnspecifiedMusicXmlFile: return ResourcesForUI.Message_UnspecifiedMusicXmlFile;
                default: return string.Format("{0} {1}",ResourcesForUI.Message_UndefinedMessage,messageEnum.ToString());
            }
        }

        private string LocalizeExtraMessage(ModelMessageEnum messageEnum)
        {
            switch (messageEnum)
            {
                case ModelMessageEnum.DirectoryNotFound: return "";
                case ModelMessageEnum.FailedToConnectToScreenReader: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.ConnectedToNonDefaultScreenReader: return ResourcesForUI.Message_MayNotWorkAsExpected;
                case ModelMessageEnum.FailedToStartProgram: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.FileNotFound: return "";
                case ModelMessageEnum.MissingProgramFile: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.FailedToReadMusicXmlFile: return ResourcesForUI.Message_PleaseSeeLogFile;
                default: return "";
            }
        }

        /// <summary>
        /// Build and show a message consisting of
        /// Line 1: A Message followed by possible parameters. Example: "File not found Stardust.xml"
        /// Line 2: (optional) an extra message. Example: "Plaese see Log File.."
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="text"></param>
        public void ShowWarning(int messageId,string parameter,string text)
        {
            //string parameter = "";
            string localizedMessage = LocalizeMessage((ModelMessageEnum)messageId);
            string localizedExtraMessage = LocalizeExtraMessage((ModelMessageEnum)messageId);
            string formattedMessage = string.Format("{0} {1} {2}",
                localizedMessage,                                                                   // The message
                string.IsNullOrEmpty(parameter) ? "" : "'"+parameter+"'",                           // Possible parameter
                string.IsNullOrEmpty(localizedExtraMessage) ? "": "\r\n"+localizedExtraMessage);    // Possible extra message             
            MessageBox.Show(formattedMessage, ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion






        /// <summary>
        /// Open a standard File Dialog allowing the user select a MusicXml file.
        /// Clear statistic counters describing the operations on the file selected.
        /// Let the Model load the file selected and report any errors detected to the user
        /// Update the UserSettingsTreeview used for filtering depending on the contents of the MusicXml file loaded
        /// Load the contents of the MusicXml file into the main ListBox.
        /// Dump statistic counters describing the operations on the file selected.
        /// </summary>
        /// <param name="sender"> Not used</param>
        /// <param name="e">Not used</param>
        private void SelectAndOpenMusicXmlFile(object sender, EventArgs e)
        {
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.xml", ResourcesForUI.OpenFileDialog_Filter); // Only present .xml files
            openFileDialog.InitialDirectory = model.InitialDirectory;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ShowDialog();

            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return; // Let the user press ESC without warning him
            }

            textBoxMessage.Focus();
            string shortFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
            textBoxMessage.Text = string.Format("{0} '{1}'",ResourcesForUI.TextBox_Messages_Reading_File, shortFileName);

            Logger.ClearStatistics();  // Clear statistics to be collected while loading, parsing and rendering the MusicXml file:

            if (!model.LoadMusicXmlFile(openFileDialog.FileName)) // Load the selected .xml file into the Model and build all internal data structures.
            {
                // Simple error handling
                string message = string.Format("{0} '{1}'", ResourcesForUI.TextBox_Messages_FailedToRead_File, shortFileName); // Short filename for UI
                textBoxMessage.Text = message;
                ShowWarning((int)ModelMessageEnum.FailedToReadMusicXmlFile, shortFileName, "");
                MessageBox.Show(message, ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Log(string.Format("Failed to read {0}", openFileDialog.FileName)); // Full filename for UI
                Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() until now
                return;
            }

            // Clear the contents of the listbox showing the timed events (important when loading a new file)
            listBoxTimes.Items.Clear();
            listBoxTimes.Refresh();


            autoReload = false; // While loading the listbox all changes are  made by user and must be ignored

            // The initial values of the user settings are determined by the model.
            // These settings must be reflected in the UI:
            // Load the Checkboxes controlling the user settings per part
            userSettingsHandler.LoadParts(model.partList);
            // Load the Checkboxes controlled by a fixed number of settings statically defined in the Model.
            userSettingsHandler.LoadDetails(model.UserSettings);
            // Finally expand the tree
            userSettingsHandler.ExpandAllNodes();
            //this.userSettingsTreeView.ExpandAll();
            userSettingsHandler.CheckSelectedNotes();

            // Let the Model do the hard work of transforming to e timed representation.
            LoadListBoxTimes();

            Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() during parsing, interpreting and rendering the file

            autoReload = true; // From now on all changes are  made by user and must be handled

            // Focus on the listbox representing the time representation
            listBoxTimes.Focus();
            //listBoxTimes.SelectedIndex = 0;
        }


        public void ConditionalLoadListBoxTimes()
        {
            if (autoReload)
            {
                LoadListBoxTimes();
            }
        }
        

        /// <summary>
        /// Load the main listbox with information fetched from the Model
        /// First all metainformation (Composer, Author, etc) NO ! Se below !!
        /// Then all the events describing the music sheet itself
        /// </summary>
        private void LoadListBoxTimes()
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

        #endregion


        private void openMusicXmlFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show a standard Select File dialog to allow the user to select and open a MusicXml file
            SelectAndOpenMusicXmlFile(sender, e);
        }



        #region ListBoxTimes

        //private void listBoxTimes_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int index = listBoxTimes.SelectedIndex;
        //    // Model.Trace(string.Format("ListBoxTimes_SelectedIndexChanged(i={0})", index));
        //    object o = listBoxTimes.Items[index];
        //    model.musicPlayer.SelectedIndexChanged(index, o);
        //    model.brailleDisplayer.SelectedIndexChanged(index, o);
        //}

        private void ListBoxTimes_GotFocus(object sender, EventArgs e)
        {
            int index = listBoxTimes.SelectedIndex;
            Logger.Trace(string.Format("ListBoxTimes_GotFocus(i={0})", index));
            // Even if we got focus we can not be sure that an item is selected!
            if (-1 != index)
            {
                // If an index is selected do as if Selected Index changed
                object o = listBoxTimes.Items[index];
                model.musicPlayer.SelectedIndexChanged(index, o);
                model.brailleDisplayer.SelectedIndexChanged(index, o);
                model.textDisplayer.SelectedIndexChanged(index, o);
            }
        }

        private void ListBoxTimes_LostFocus(object sender, EventArgs e)
        {
            Logger.Trace("ListBoxTimes_LostFocus");
            model.StopRefreshingBrailleDevice();
        }
        
        private void listBoxTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxTimes.SelectedIndex;
            // Model.Trace(string.Format("ListBoxTimes_SelectedIndexChanged(i={0})", index));
            object o = listBoxTimes.Items[index];
            model.musicPlayer.SelectedIndexChanged(index, o);
            model.brailleDisplayer.SelectedIndexChanged(index, o);
            model.textDisplayer.SelectedIndexChanged(index, o);
        }

        private bool savedSpeechState;
        private bool savedMusicBrailleState;

        #endregion

        private void TurnOffSpeeshAndMusicBraille()
        {
            savedSpeechState = userSettingsHandler.MusicAsText.Checked;
            savedMusicBrailleState = userSettingsHandler.MusicAsBraille.Checked;
            userSettingsHandler.MusicAsText.Checked = false;
            userSettingsHandler.MusicAsBraille.Checked = false;
            listBoxTimes.Refresh();
        }

        private void RestoreSpeechAndMusicBraille()
        {
            userSettingsHandler.MusicAsText.Checked = savedSpeechState;
            userSettingsHandler.MusicAsBraille.Checked = savedMusicBrailleState;
            listBoxTimes.Refresh();
        }

        
        private void buttonStart_Click(object sender, EventArgs e)
        {
            switch (musicPlayerState)
            {
                case MusicPlayerStateEnum.running:
                    RestoreSpeechAndMusicBraille(); // Restore the original settings BEFORE stopping in order to avoid playing last event multiple times.
                    model.StopPlaying();                 
                    musicPlayerState = MusicPlayerStateEnum.stopped;            
                    break;                 
                case MusicPlayerStateEnum.stopped:          
                    model.StartPlayingPoly(listBoxTimes.SelectedIndex); // Start at selected index
                    musicPlayerState = MusicPlayerStateEnum.running;
                    // NOTE: The use of TurnOffSpeeshAndMusicBraille() and RestoreSpeechAndMusicBraille() is a temporary HACK
                    // TO DO: Find a real solution, allowing the listbox to show visible text without JAWS reading and Brailling it !!
                    TurnOffSpeeshAndMusicBraille(); // Turn off AFTER starting to play in order to avoid playing extra sounds. 
                    break;
                case MusicPlayerStateEnum.unknown:
                    break; // Maybe we will need this later ?
            }
            LocalizeStartStopButton(musicPlayerState);
        }

        private void museScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.StartMuseScore(model.TheMusicXmlFileName);
        }

        private void sibeliusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.StartSibelius(model.TheMusicXmlFileName); 
        }

        private void logfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadLogFile();
        }

        private void openXMLFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenMusicXmlFileLocation(model.TheMusicXmlFileName);
        }

        private void openLogFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenLogFileLocation(); 
        }

        private void inspectAsXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadMusicXmlFile(model.TheMusicXmlFileName);    
        }

        private void viewAsInterpretedXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadInterpretation(model.AllMusicXmlObjecsts,model.TheMusicXmlFileName);           
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxMessage.Text = ResourcesForUI.TextBox_Messages_TheProgramIsExiting;
            textBoxMessage.Refresh();
            // Remaining actions are taken in Application_ApplicationExit.
            // In this way the Model will always be shut down no matter why the application exits.
            Application.Exit();
        }

        #region keyhandlers
        
        /// <summary>
        /// Occurs when a key is pressed while listBoxTimes has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxTimes_KeyDown(object sender, KeyEventArgs e)
        {
            if (shortCutHandler.IsStopPlayingShortcut(e))
            {
                // Pass on to the event handler for the button 
                buttonStart_Click(null, null);
            }

            if (shortCutHandler.IsStartPlayingShortcut(e))
            {
                // Pass on to the event handler for the button 
                buttonStart_Click(null, null);
            }

        }

        /// <summary>
        /// Occurs when a key is pressed while treeView has focus         
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userSettingsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            bool newValue;
            // We only handle the shortcuts specified in shortCutHandler
            if (!shortCutHandler.IsTreeViewMultiControlShortcut(e,out newValue)) return;
            // We only handle level 2 nodes
            if (2 != userSettingsTreeView.SelectedNode.Level) return;
 
            string level2Text = userSettingsTreeView.SelectedNode.Text;
            string level1Text = userSettingsTreeView.SelectedNode.Parent.Text;
            // Locate and check/uncheck all nodes with same parent-name and same node-name
            foreach (TreeNode level0Node in userSettingsTreeView.Nodes)
            {
                foreach (TreeNode level1Node in level0Node.Nodes)
                {
                    if (0 == string.Compare(level1Text, level1Node.Text))
                    {
                        foreach (TreeNode level2Node in level1Node.Nodes)
                        {
                            if (0 == string.Compare(level2Text, level2Node.Text))
                            {
                                level2Node.Checked = newValue;
                            }
                        }
                    }
                }
            }
        }

        #endregion keyhandlers

        #region Edit

        // Items above the delimiter line are represented by level 0 nodes in the tree


        private void musicRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userSettingsHandler.ShowMusic();
        }

        private void textRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userSettingsHandler.ShowText();
        }

        private void brailleRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userSettingsHandler.ShowBraille();
        }

        private void partsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userSettingsHandler.ShowParts();
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userSettingsHandler.ShowDetails();
        }
  
        private void allItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userSettingsHandler.ShowAllItems();
        }

        #endregion

    }

}
