using System;
using System.Text;
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
        string className = "MainForm";
        enum MusicPlayerStateEnum { unknown, stopped, running };
        string ApplicationName = "";  // Locakized application name. Will be re-initialized later using localization!
        string executingAssemblyFullPath  = ""; // The (unlocalized) name and location of the program, 
        string executingAssemblyShortName = ""; // The (unlocalized) short name of the program, used by for instance JAWS to name configuration file! 
        Model model;        // The Model containing all of the business logic.        
        bool autoReload;    // Used to optimize performance when changing large parts of the UI within short time
        UserSettingsHandler userSettingsHandler; // Contains all settings that can be configured by the user
        MusicPlayerStateEnum musicPlayerState = MusicPlayerStateEnum.stopped; // Assume the musicplayer is innitially stopped
        ShortcutHandler shortCutHandler;
        UserCommandInterpreter commandInterpreter;
        string myMusicXmlDirectory; // Default location for MusicXml files belonging to thos user. Wil be populated with sample filer!

        public MainForm()
        {
            string functionName = "MainForm"; // Only for logging
            executingAssemblyFullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingAssemblyShortName = System.IO.Path.GetFileNameWithoutExtension(executingAssemblyFullPath);
            InitializeComponent();
            Logger.Open(null); // null => Use the default logfile name
            Logger.Log(""); // An empty line to catch the eye
            Logger.Log(string.Format("{0}.{1} Starting: Date={2}",className, functionName,System.DateTime.Now.ToLongDateString()));
            Application.ApplicationExit += Application_ApplicationExit;
            LogSystemInformation();
            // If the execution directory contains a file named "Language.txt" containing the string "en-US"
            // the application language will be changed to english evin if running on a danish PC!
            LogGLobalisationInformation();
            // Do any UI localization before we create the model. In this way we avoid showing unlocalized texts
            // if an error is reported by a messagebox.
            musicPlayerState = MusicPlayerStateEnum.stopped;
            //LocalizeStartStopButton(musicPlayerState);
            ApplicationName = ResourcesForUI.MainForm_ApplicationName;
            LocalizeMenuStrip(); // Overwrite all items in MenuStrip with localized texts
            listBoxTimes.AccessibleName = ResourcesForUI.ListView_Accessible_Name; // Overwrite all items in listBoxTimes with localized texts
            userSettingsTreeView.AccessibleName = ResourcesForUI.TreeView_Accessible_Name; // Overwrite all items in userSettingsTreeview with localized texts
            textBoxStatusInformation.AccessibleName = ResourcesForUI.StatusLine_Accessible_Name; // Overwrite with localized text


            Utilities.UtilityClient = (this as IUtilityClient); //Decide how to show error messages and warnings 
            model = Model.Create((this as IObjectCollection), (this as IDebugDisplayerClient), ApplicationName);
            this.Text = ApplicationName;

            // XCopy MusicXml samples from the "MusicXml samples" directory in the installation files to myMusicXmlDirectory during first activation !   
            myMusicXmlDirectory = model.InitMusicXmlFiles(ApplicationName,ResourcesForUI.DirectoryNames_Samples); 

            // Create a handler for the user settinge, in this case modelled as a treeview.
            userSettingsHandler = UserSettingsHandler.Create(this,this.userSettingsTreeView,model);
            userSettingsHandler.Init(); // Builds up the fixed part of the treeview
            userSettingsTreeView.CollapseAll(); 
            // Create a handler for handling all Keyboard shortcuts
            shortCutHandler = ShortcutHandler.Create(this, model);
            commandInterpreter = UserCommandInterpreter.Create(this.textBoxCommand, this.listBoxTimes, model);
            LoadIcon();
        }

        private void LoadIcon()
        {
            const string functionName = "LoadIcon";
            string iconFile = "";
            try
            {
                iconFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(executingAssemblyFullPath), "icon.ico");
                this.Icon = new System.Drawing.Icon(iconFile);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} Could not load icon file {2} ->", className, functionName, iconFile));
                Logger.Log(string.Format("{0}", e.Message));
            }

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
            // viewToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_View;  // Removed,but may be reintroduced later !!!
            toolsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools;
            archivesToolStripMenuItem.Text = ResourcesForUI.ToolsStripMenuItem_Archives;
            helpToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help ; 

            // Children of  fileToolStripMenuItem
            openMusicXmlFileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile; ;
            openMusicXmlFileToolStripMenuItem.ShortcutKeys = ShortcutHandler.openMusicXmlFile;
            exitToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_Exit;
            exitToolStripMenuItem.ShortcutKeys = ShortcutHandler.exitApplication;

            // Children of editToolStripMenuItem
            // Texts:  NOTE! Use the same texts as used in the treeview to which these items refer!!
            allItemsToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_All_Items;
            filterItemsToolStripMenuItem.Text = "&" + ResourcesForUI.TreeWiew_Items;
            musicRepresentationToolStripMenuItem.Text = "&"+ResourcesForUI.TreeView_MusicAsSound;
            textRepresentationToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsSpeech;
            brailleRepresentationToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsBraille;
            partsToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsSound_Parts;
            detailsToolStripMenuItem.Text = "&" + ResourcesForUI.TreeView_MusicAsSound_Details;

            // Shortcuts
            allItemsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editAllItems;
            filterItemsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editItems;
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
            jAWSSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_JAWS_Settings;

            // Children of  helpToolStripMenuItem
            this.aboutIBOSMusicXmlReaderToolStripMenuItem.Text = string.Format("{0} {1}",ResourcesForUI.ToolStripMenuItem_Help_About,ApplicationName);
            this.keyboardShortcutsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_Shortcuts;


        }


#if false
        void LocalizeStartStopButton(MusicPlayerStateEnum musicPlayerState)
        {
            switch (musicPlayerState)
            {
                case MusicPlayerStateEnum.running: buttonStart.Text = ResourcesForUI.ButtonStart_StopPlaying;  break;
                case MusicPlayerStateEnum.stopped: buttonStart.Text = ResourcesForUI.ButtonStart_StartPlaying; break;
                default: break;
            }
        }
#endif

#region supportcode


        public static void LogSystemInformation()
        { 
            Logger.Log(string.Format("Executing Assembly='{0}'", System.Reflection.Assembly.GetExecutingAssembly()));
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

        public void WriteStatusInformation(string s)
        {
            textBoxStatusInformation.Text = s;
        }


        #endregion

        #region  IObjectCollection

        public int GetNumberOfObjects()
        {
            return listBoxTimes.Items.Count;
        }

        delegate object GetObjectAtIndexCallback(int index);
        public object GetObjectAtIndex(int index)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBoxTimes.InvokeRequired)
            {
                GetObjectAtIndexCallback d = new GetObjectAtIndexCallback(GetObjectAtIndex);
                return listBoxTimes.Invoke(d, new object[] { index });
            }
            else
            {
                return listBoxTimes.Items[index];
            }
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
            MessageBox.Show(text); // To implement localization: Do as in ShowWarning !!
        }



        private string LocalizeMessage(ModelMessageEnum messageEnum)
        {
            switch (messageEnum)
            {
                case ModelMessageEnum.DirectoryNotFound: return ResourcesForUI.Message_DirectoryNotFound;
                case ModelMessageEnum.FailedToConnectToScreenReader: return ResourcesForUI.Message_JAWSScreenReaderIsNotRunning;
                case ModelMessageEnum.ConnectedToNonDefaultScreenReader: return ResourcesForUI.Message_ConnectedToNonDefaultScreenReader;
                case ModelMessageEnum.FailedToStartProgram: return ResourcesForUI.Message_FailedToStartProgram;
                case ModelMessageEnum.FileNotFound: return ResourcesForUI.Message_FileNotFound;
                case ModelMessageEnum.MissingProgramFile: return ResourcesForUI.Message_MissingProgramFile;
                case ModelMessageEnum.FailedToReadMusicXmlFile: return ResourcesForUI.Message_FailedToReadMusicXmlFile;
                case ModelMessageEnum.UnspecifiedMusicXmlFile: return ResourcesForUI.Message_UnspecifiedMusicXmlFile;
                case ModelMessageEnum.NotAllowedWhilePlaying: return ResourcesForUI.Message_NotAllowedWhilePlaying;
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
                case ModelMessageEnum.NotAllowedWhilePlaying: return ResourcesForUI.Message_StopPlayingFirst;
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
           //            openFileDialog.Filter = string.Format("{0}|*.xml|{0}|*.mxl", ResourcesForUI.OpenFileDialog_Filter,ResourcesForUI.OpenFileDialog_Filter_mxl); // Only present .xml files and .mxl files
            openFileDialog.InitialDirectory = GetFileOpenInitialDirectory();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ShowDialog();

            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return; // Let the user press ESC without warning him
            }

            textBoxStatusInformation.Focus();
            string shortFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
            textBoxStatusInformation.Text = string.Format("{0} '{1}'",ResourcesForUI.TextBox_Messages_Reading_File, shortFileName);

            Logger.ClearStatistics();  // Clear statistics to be collected while loading, parsing and rendering the MusicXml file:

            if (!model.LoadMusicXmlFile(openFileDialog.FileName)) // Load the selected .xml file into the Model and build all internal data structures.
            {
                // Simple error handling
                string message = string.Format("{0} '{1}'", ResourcesForUI.TextBox_Messages_FailedToRead_File, shortFileName); // Short filename for UI
                textBoxStatusInformation.Text = message;
                ShowWarning((int)ModelMessageEnum.FailedToReadMusicXmlFile, shortFileName, "");
                MessageBox.Show(message, ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Log(string.Format("Failed to read {0}", openFileDialog.FileName)); // Full filename for UI
                Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() until now
                this.Text = ApplicationName; // Remove any exixting filename from the title bar
                return;
            }

            // Clear the contents of the listbox showing the timed events (important when loading a new file)
            listBoxTimes.Items.Clear();
            listBoxTimes.Refresh();
            // Clear the contents of all other user controls
            textBoxBraille.Clear();
            textBoxText.Clear();
            textBoxStatusInformation.Clear();


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
            // Use the status line for meta information ontil overwritten by real status information
            textBoxStatusInformation.Text = GetStatusFromMetaInformation(); 

            // Let the Model do the hard work of transforming to e timed representation.
            LoadListBoxTimes();

            Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() during parsing, interpreting and rendering the file

            autoReload = true; // From now on all changes are  made by user and must be handled

            // Focus on the listbox representing the time representation
            listBoxTimes.Focus();
            //listBoxTimes.SelectedIndex = 0;

            //this.Text = string.Format("{0}       {1}",GetTitleInfo(),ApplicationName); // Show the name of the file just loaded in the Title-Line, accessible by <INSERT> + T
            this.Text = GetTitleInfo();

            model.SetUserTempo(100); // Play at 100% of tempo specified in MusicXml file
          
        }

        private string GetFileOpenInitialDirectory()
        {
            string functionName = "GetOpenFileInitialDirectory";
            string result = myMusicXmlDirectory;  // When running a user session we want to use the files in the <user>\Documents\IBOS NNodelæser directory 
            if (model.InitialDirectory.Contains("Visual Studio"))
            {
                result = model.InitialDirectory;   // When running a debug session we want to use the files in the debug\bin directory
            }
            Logger.Log(string.Format("{0}.{1} returns {2}",className,functionName,result));
            return result;
        }


        public void ConditionalLoadListBoxTimes()
        {
#if false
            string functionName = "ConditionalLoadListBoxTimes";
            Logger.Log(String.Format("{0}.{1} autoReload= {2}", className, functionName, autoReload));
#endif
            if (autoReload)
            {
                LoadListBoxTimes();
            }
        }

        /// <summary>
        /// Defines the contents of the title-line
        /// </summary>
        /// <returns></returns>
        private string GetTitleInfo()
        {
            string result = string.Format("{0}  {1}  {2}"
                                            , ApplicationName // 0
                                            , model.MetaInformation.FileName // 1
                                            , model.MetaInformation.MovementTitle // 2
                                            );
            return result;
        }

        /// <summary>
        /// Defines the (initial) contents of the status line
        /// </summary>
        /// <returns></returns>
        private string GetStatusFromMetaInformation()
        {
            string result = string.Format("{0}  {1}  {2}  {3}  {4}  {5}  {6}"
                                            , "" // 0 No need to repeat the application nema here !
                                            , model.MetaInformation.FileName // 1
                                            , model.MetaInformation.MovementTitle // 2
                                            , model.MetaInformation.MovementNumber // 3
                                            , model.MetaInformation.Work // 4
                                            , model.MetaInformation.Source // 5 
                                            , model.MetaInformation.Creator // 6
                                         );
            return result;
        }


        private void LoadTextBoxStatusInformation()
        {
            textBoxStatusInformation.Text = GetStatusFromMetaInformation();
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

            model.TogglePlaying(listBoxTimes.SelectedIndex);
            //LocalizeStartStopButton(musicPlayerState);
        }

        //
        //*************************************************************************************************
        //

#region tools


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


        private void jAWSSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadJawsSettingsFile(executingAssemblyShortName);
        }


        #endregion // tools ***********************************************************************

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxStatusInformation.Text = ResourcesForUI.TextBox_Messages_TheProgramIsExiting;
            textBoxStatusInformation.Refresh();
            // Remaining actions are taken in Application_ApplicationExit.
            // In this way the Model will always be shut down no matter why the application exits.
            Application.Exit();
        }

        #region keyhandlers

        private void listBoxDetails_KeyDown(object sender, KeyEventArgs e)
        {
            string functionName = "listBoxDetails_KeyDown";
            try // This is new code for version 1.0.0.0 so better safe than sorry
            {
                if (shortCutHandler.IsDetailsShortcut(e))
                {
                    // On keys.Right and keys.Left:
                    // Do nothing special, but pass the key to the listbox without suppressing it !
                }
                else
                {
                    // All other keys will return focus to the mail listbox
                    listBoxDetails.Items.Clear();
                    int selectedIndex = listBoxTimes.SelectedIndex;
                    // keys.Right will select the next detail if possible
                    if ((e.KeyCode == ShortcutHandler.detailsNextEvent) && ((selectedIndex + 1) < listBoxTimes.Items.Count))
                    {
                        listBoxTimes.SelectedIndex = selectedIndex + 1;
                    }
                    // keys.Left will select the previous detail if possible
                    if ((e.KeyCode == ShortcutHandler.detailsPreviousEvent) && (selectedIndex > 0))
                    {
                        listBoxTimes.SelectedIndex = selectedIndex - 1;
                    }
                    // In all other cases just return to the original index and move focus
                    listBoxTimes.Focus();
                    e.SuppressKeyPress = true;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(string.Format("{0}.{1} KeyCode={2} threw an exception: Message={3}", className, functionName, e.KeyCode.ToString(), exception.Message));
            }
            return;
        }



        /// <summary>
        /// Occurs when a key is pressed while listBoxTimes has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxTimes_KeyDown(object sender, KeyEventArgs e)
        {
            string functionName = "listBoxTimes_KeyDown";
            if (shortCutHandler.IsTogglePlayingShortcut(e))
            {
                model.ToggleStartStopPlaying(listBoxTimes.SelectedIndex);
                e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control.
                                            // This avoids flickering caused by the special interpretation of SPACE to the underlying Listbox 
                return;            
            }

            if (shortCutHandler.IsTempoDecrementShortcut(e))
            {
                model.ChangeUserTempo(-1);
                e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control. 
                return;
            }

            if (shortCutHandler.IsTempoIncrementShortcut(e))
            {
                model.ChangeUserTempo(1);
                e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control.
                return;
            }


            if (shortCutHandler.IsStartPlayingShortcut(e))
            {
                model.StartPlayingPoly(listBoxTimes.SelectedIndex);
                musicPlayerState = MusicPlayerStateEnum.running;
                return;
            }


            else if (shortCutHandler.IsStopPlayingShortcut(e))
            {
                model.StopPlaying();
                musicPlayerState = MusicPlayerStateEnum.stopped;
                return;
            }

            else if (shortCutHandler.IsStopAllNotesPlayingShortcut(e))
            {
                model.musicPlayer.StopAllNotesPlaying();
                return;
            }

            else if (shortCutHandler.IsDetailsShortcut(e))
            {

                try // This is new code for version 1.0.0.0 so better safe than sorry
                {          
                    listBoxDetails.Items.Clear();
                    object selectedEvent = listBoxTimes.Items[listBoxTimes.SelectedIndex];
                    if ((null != selectedEvent) && (selectedEvent is EventDescription))
                    {
                        EventDescription currentEventDescription = (listBoxTimes.Items[listBoxTimes.SelectedIndex]) as EventDescription;
                        listBoxDetails.Items.AddRange(model.GetCurrentEventDetails(currentEventDescription));
                        int itemCount = listBoxDetails.Items.Count;
                        if (0 != itemCount)
                        {
                            // Select either the first or the last item
                            if (e.KeyCode == ShortcutHandler.detailsPreviousPart)
                            {
                                listBoxDetails.SelectedIndex = (itemCount - 1); // Select the last item
                            }
                            if (e.KeyCode == ShortcutHandler.detailsNextPart)
                            {
                                listBoxDetails.SelectedIndex = 0; // Select the first item
                            }
                        }
                    }

                    if (0 == listBoxDetails.Items.Count) // For whatever reason
                    {
                        listBoxDetails.Items.Add("No details found");
                    }
         
                    listBoxDetails.Focus();
                    e.SuppressKeyPress = true;  // Prevent sending this key event to the underlying control.
                }
                catch (Exception exception)
                {
                    Logger.Log(string.Format("{0}.{1} KeyCode={2} threw an exception: Message={3}", className, functionName, e.KeyCode.ToString(), exception.Message));
                }
                return;
            }


            // Let the command interpreter handle it 
            commandInterpreter.Add(e);

        }

        /// <summary>
        /// Occurs when a key is pressed while treeView has focus         
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userSettingsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == ShortcutHandler.listBoxFocus)
            {
                listBoxTimes.Focus(); // Easy way to move the focus to the main listbox
                return;
            }

            bool newValue;
            bool toggleAndCopy;
            // We only handle the shortcuts specified in shortCutHandler
            if (!shortCutHandler.IsTreeViewMultiControlShortcut(e,out newValue,out toggleAndCopy)) return;
            if (toggleAndCopy)
            {
                // new valye is the togled value of the node that was clisked!
                newValue = !userSettingsTreeView.SelectedNode.Checked;
                e.Handled = true;  // Prevents that the control wil toggle this checkbox once more !                
            }

            // We only handle level 2 nodes
            if (2 != userSettingsTreeView.SelectedNode.Level) return;
 
            string level2Text = userSettingsTreeView.SelectedNode.Text;
            string level1Name = userSettingsTreeView.SelectedNode.Parent.Name;

            bool saveAutoReload = autoReload;
            autoReload = false; // Avoid loading the listbox for each and every change
            // Locate and check/uncheck all nodes with same parent-name and same node-name
            foreach (TreeNode level0Node in userSettingsTreeView.Nodes)
            {
                foreach (TreeNode level1Node in level0Node.Nodes)
                {
                    if (0 == string.Compare(level1Name, level1Node.Name))
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
            autoReload = saveAutoReload; // Restore
            ConditionalLoadListBoxTimes(); // Reload once instead of multiple times
        }
        
        /// <summary>
        /// Occurs when a key is pressed and the textBoxNormalTExt has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxNormalText_KeyDown(object sender, KeyEventArgs e)
        {
        }        

        #endregion keyhandlers 
        //
        //*************************************************************************************************
        //
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
            userSettingsHandler.ShowFilterItems(true);
        }

        private void filterItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            userSettingsHandler.ShowFilterItems(false);
        }

        #endregion // Edit
        //
        //*************************************************************************************************
        //

        #region Help
        private void aboutIBOSMusicXmlReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            string caption = ApplicationName;
            string text = string.Format("{0}={1}", ResourcesForUI.ToolStripMenuItem_Help_About_Version, version.ToString());
            MessageBox.Show(text, caption);
        }


        private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string caption = ApplicationName + " " + ResourcesForUI.ToolStripMenuItem_Help_Shortcuts;
            Form helpForm = new HelpForm();
            helpForm.Show();
            // MessageBox.Show(ShortcutHelp.Create().ToString(), caption);
        }

        #endregion

        #region Archives

        private void httpsmusescorecomsheetmusicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(@"https://musescore.com/sheetmusic");
        }

        private void httpswwwmusicxmlcommusicinmusicxmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwmusicalioncomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpimslporgwikiMainPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwww1cpdlorgwikiindexphpMainPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpickingmusicarchiveorgindexphpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpfolkopediaefdssorgwikiTake6TranscriptionProgrammeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpjosquinstanfordeduToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpneumahumanumfrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwhymnaryorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwshsueduacademicsmusicponchielliindexhtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpsgithubcomMTGSymbTrreleasestagv200ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwvisaudiodesignscomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwlamadeguidocomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httplusthofdermuziekblogspotdk201011gardenofmusicaldelightshtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwfolkotecagalegacomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpopenmusicscoreorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwgutenbergorgwikiGutenbergTheSheetMusicProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwnewhymnsorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwhymnsandcarolsofchristmascomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void httpwwwhausmusikchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.OpenUrl(sender.ToString());
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        #endregion
        //*************************************************************************************************

    }

}
