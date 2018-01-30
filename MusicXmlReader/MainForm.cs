using System;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
// using MusicXmlReaderUI;
using MusicXmlReaderModel;
using System.Collections.Generic;
using System.IO;

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
        bool consoleTrace = false;
        enum MusicPlayerStateEnum { unknown, stopped, running };
        string applicationName = "";  // Locakized application name. Will be re-initialized later using localization!
        string executingAssemblyFullPath  = ""; // The (unlocalized) name and location of the program, 
        string executingAssemblyShortName = ""; // The (unlocalized) short name of the program, used by for instance JAWS to name configuration file! 
        Model model;        // The Model containing all of the business logic.        
        bool autoReload;    // Used to optimize performance when changing large parts of the UI within short time
        UserSettingsHandler userSettingsHandler; // Contains all settings that can be configured by the user
        // MusicPlayerStateEnum musicPlayerState = MusicPlayerStateEnum.stopped; // Assume the musicplayer is innitially stopped
        // ShortcutHandler shortCutHandler;
        //UserCommandInterpreter commandInterpreter;
        string myMusicXmlDirectory; // Default location for MusicXml files belonging to thos user. Wil be populated with sample filer!
        ImportHandler importHandler;
        DetailsHandler detailsHandler;

        public string ApplicationName
        {
            get
            {
                return applicationName;
            }
        }


        public MainForm()
        {
            string functionName = "MainForm"; // Only for logging
            try
            {
                executingAssemblyFullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                executingAssemblyShortName = System.IO.Path.GetFileNameWithoutExtension(executingAssemblyFullPath);
                InitializeComponent();
                Logger.Open(null); // null => Use the default logfile name
                Logger.Log(""); // An empty line to catch the eye
                Logger.Log(string.Format("{0}.{1} Starting: Date={2}", className, functionName, System.DateTime.Now.ToLongDateString()));
                Application.ApplicationExit += Application_ApplicationExit;
                LogSystemInformation();
          
                // If the execution directory contains a file named "Language.txt" containing the string "en-US"
                // the application language will be changed to english evin if running on a danish PC!
                LogGLobalisationInformation();
                // Do any UI localization before we create the model. In this way we avoid showing unlocalized texts
                // if an error is reported by a messagebox.
                // musicPlayerState = MusicPlayerStateEnum.stopped;
                //LocalizeStartStopButton(musicPlayerState);
                applicationName = ResourcesForUI.MainForm_ApplicationName;
                textBoxScreenReader.Hide(); // This textbox gets Focus used during long-lasting operation and thus draws the Screenreaders attensio to itself, avoiding too much Speech !
                LocalizeMenuStrip(); // Overwrite all items in MenuStrip with localized texts
                listBoxTimes.AccessibleName = ResourcesForUI.ListView_Accessible_Name; // Overwrite all items in listBoxTimes with localized texts
                userSettingsTreeView.AccessibleName = ResourcesForUI.TreeView_Accessible_Name; // Overwrite all items in userSettingsTreeview with localized texts
                textBoxStatusInformation.AccessibleName = ResourcesForUI.StatusLine_Accessible_Name; // Overwrite with localized text
                listBoxDetails.AccessibleRole = AccessibleRole.None; // ListboxDetails is intensionally kept anonymous to the user. It is only used for outputting texts via JAWS


                Utilities.UtilityClient = (this as IUtilityClient); //Decide how to show error messages and warnings 
                model = Model.Create((this as IObjectCollection), (this as IDebugDisplayerClient), applicationName);
                importHandler = ImportHandler.Create(model,this);
                this.Text = applicationName;
                WriteStatusInformation(model.ScreenReaderName);

                // XCopy MusicXml samples from the "MusicXml samples" directory in the installation files to myMusicXmlDirectory during first activation ! 
                myMusicXmlDirectory = model.InitMusicXmlFiles(applicationName, ResourcesForUI.DirectoryNames_Samples, ResourcesForUI.DirectoryNames_Downloads);

                // Create a handler for the user settinge, in this case modelled as a treeview.
                userSettingsHandler = UserSettingsHandler.Create(this, this.userSettingsTreeView, model);
                userSettingsHandler.Init(); // Builds up the fixed part of the treeview
                userSettingsTreeView.CollapseAll();
                // Create a handler for handling all Keyboard shortcuts
                // shortCutHandler = ShortcutHandler.Create(this, model);
                // commandInterpreter = UserCommandInterpreter.Create(this.textBoxCommand, this.listBoxTimes, model);
                detailsHandler = DetailsHandler.Create(listBoxTimes,listBoxDetails,model,this as IDebugDisplayerClient);
                LoadIcon();


#if true
                // Investigate The Left/Right problem (Error 296)
                // https://stackoverflow.com/questions/16305454/getting-the-left-and-right-arrow-keys-to-select-the-previous-next-menu-instead-o
                // https://connect.microsoft.com/VisualStudio/feedback/details/786382/menustrip-control-issues-with-rightalignedmenus
                // https://connect.microsoft.com/VisualStudio/feedback/details/796965/menustrip-right-left-arrow-keys-work-reversely-when-a-submenu-is-open-dropped-down
                Logger.Log(string.Format("->SystemInformation.RightAlignedMenus={0}", System.Windows.Forms.SystemInformation.RightAlignedMenus.ToString()));
                Logger.Log(string.Format("->MainMenu.RightToLeft={0}", this.RightToLeft.ToString()));
                Logger.Log(string.Format("->MainMenu.RightToLeftLayout={0}",this.RightToLeftLayout.ToString()));
#warning Remove experiments !!
#endif
                //MenuStripWorkAround(); // Does not solve the problem !! !!
#if false
                if (SystemInformation.RightAlignedMenus)
                {
#warning Find a real solution instead of this terrible hack !
                    Beep();
                    MessageBox.Show(ResourcesForUI.Message_RightAlignedMenus);
                }
#endif

                // throw (new Exception("For test only")); // Insert this line to test the Last Resort handler below
            }
            catch (Exception e)
            {
                // Last resort handler: An unhandled exception occured.
                // The best we can do is to show a warning in a messagebox and log the exception message          
                Logger.Log(String.Format("{0}.{1} Exception occurred: {2}",className,functionName,e.Message));
                ShowWarning((int)ModelMessageEnum.UnspecifiedInitializationError,"","");
            }
        }

        private void Beep()
        {
            System.Media.SystemSounds.Beep.Play();
        }

#if false
        private void MenuStripWorkAround()
        {
            // NO! this does not solve the problem ! It changes the appearance of the dropdowns, but not the behaviour !!!

            // https://connect.microsoft.com/VisualStudio/feedback/details/786382/menustrip-control-issues-with-rightalignedmenus
            // https://connect.microsoft.com/VisualStudio/feedback/details/796965/menustrip-right-left-arrow-keys-work-reversely-when-a-submenu-is-open-dropped-down
            //Begin workaround
            const string functionName = "MenuStripWorkAround";

            // if (SystemInformation.RightAlignedMenus)
            {
                MainMenuStrip.RightToLeft = RightToLeft.Yes;
                foreach (ToolStripMenuItem toolStripMenuItem in this.MenuStrip.Items)
                {                 
                    toolStripMenuItem.RightToLeft = RightToLeft.Yes;
                    Logger.Log(string.Format("{0}.{1} Setting RightToLeft to 'Yes' for {2} ", className, functionName, toolStripMenuItem.Name));
                    foreach (object o in toolStripMenuItem.DropDownItems)
                    {
                        if (o is ToolStripDropDownItem)
                        {
                            ToolStripDropDownItem toolStripDropDownItem = o as ToolStripDropDownItem;
                            Logger.Log(string.Format("{0}.{1} Setting RightToLeft to 'No' for {2} ", className, functionName, toolStripDropDownItem.Name));
                            toolStripDropDownItem.RightToLeft = RightToLeft.No;
                            
                        }
                    }                 

                }
            }

        }
#endif


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

#if false
        // For development onlY!
        private List<string> shortcutStrings;
        private List<Keys> shortCutKeys;

        private string Add(string text)
        {
            shortcutStrings.Add(text);
            int index = text.IndexOf("&");
            if (text.Length >= index + 1)
            {
                // The character following the & will be interpreted as a shortcut combined with the ALT key
                char ch = text.ToUpper()[index+1];
                Keys keys = Keys.Alt | (Keys.A + ch - 'A');
                Add(keys);
            }
            return text;
        }

        private Keys Add(Keys keys)
        {
            string functionName = "Add";
            if (shortCutKeys.Contains(keys))
            {
                Logger.Log(String.Format("{0}.{1}: Duplicate key = {2}",className,functionName, keys.ToString()));
            }
            else
            {
                shortCutKeys.Add(keys);
            }
           
            return keys;
        }
#endif


        /// <summary>
        /// As default JAWS does not render the Shortcut key, only the access key
        /// By setting the AccessibleName property we can overwrite the default to anything we wish:
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        string GenerateAccessibleName(string text, Keys keys)
        {
            if ( Keys.None == keys) return text;
            return Utilities.RemoveAmpersant(text) + " " +  UiUtilities.KeysToString(keys); // Say the control char before the other character.
        }

        void GenerateAccessibleName(ref ToolStripMenuItem menuItem)
        {
            menuItem.AccessibleName = GenerateAccessibleName(menuItem.Text, menuItem.ShortcutKeys);
        }

        void LocalizeMenuStrip()
        {

            //MenuStrip.Text = "??";
            // Children of MenuStrip
            filesToolStripMenuItem.Text =   ResourcesForUI.ToolStripMenuItem_Files;
            editToolStripMenuItem.Text =    ResourcesForUI.ToolStripMenuItem_Edit;
            viewToolStripMenuItem.Text =    ResourcesForUI.ToolStripMenuItem_View;
            toolsToolStripMenuItem.Text =   ResourcesForUI.ToolStripMenuItem_Tools;
            archivesToolStripMenuItem.Text= ResourcesForUI.ToolsStripMenuItem_Archives;
            helpToolStripMenuItem.Text =    ResourcesForUI.ToolStripMenuItem_Help ; 

            // Children of  fileToolStripMenuItem
            openMusicXmlFileToolStripMenuItem.Text =        ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile; 
            openMusicXmlFileToolStripMenuItem.ShortcutKeys= ShortcutHandler.openMusicXmlFile;
            GenerateAccessibleName(ref openMusicXmlFileToolStripMenuItem);
            importDownloadsToolStripMenuItem.Text =         ResourcesForUI.ToolStripMenuItem_Files_ImportDownloads;
            importNewestDownloadsToolStripMenuItem.Text =   ResourcesForUI.ToolStripMenuItem_Files_ImportNewestDownloads;
            importNewSampleFilesToolStripMenuItem.Text =    ResourcesForUI.ToolStripMenuItem_Files_ImportNewestSamples;
            exitToolStripMenuItem.Text =                    ResourcesForUI.ToolStripMenuItem_Files_Exit;
            exitToolStripMenuItem.ShortcutKeys =            ShortcutHandler.exitApplication;
            GenerateAccessibleName(ref exitToolStripMenuItem);

          // Children of editToolStripMenuItem, referring to the Treeview
          // Texts:  NOTE! Use the same texts as used in the treeview to which these items refer!!
            allItemsToolStripMenuItem.Text =                ResourcesForUI.TreeView_All_Items;
            filterItemsToolStripMenuItem.Text =             ResourcesForUI.TreeWiew_Items;
            musicRepresentationToolStripMenuItem.Text =     ResourcesForUI.TreeView_MusicAsSound;
            textRepresentationToolStripMenuItem.Text =      ResourcesForUI.TreeView_MusicAsSpeech;
            brailleRepresentationToolStripMenuItem.Text =   ResourcesForUI.TreeView_MusicAsBraille;
            partsToolStripMenuItem.Text =                   ResourcesForUI.TreeView_MusicAsSound_Parts;
            detailsToolStripMenuItem.Text =                 ResourcesForUI.TreeView_MusicAsSound_Details;
            uncheckAllToolStripMenuItem.Text =              ResourcesForUI.TreeView_UncheckAll;
            checkAllToolStripMenuItem.Text =                ResourcesForUI.TreeView_CheckAll;

            // Children of editToolStripMenuItem referring to the ParameterInputForm
            repeatToolStripMenuItem.Text =                  ResourcesForUI.ParameterInputForm_Repeat;
            goToToolStripMenuItem.Text=                     ResourcesForUI.ParameterInputForm_GoTo;
            ofNominalTempoToolStripMenuItem.Text =          ResourcesForUI.ParameterInputForm_PctOfNominalTempo;


            // Direct Shortcuts 
            // allItemsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editAllItems; // See comment in ShortcutHandler.cs
            filterItemsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editFilter;             // Select the Filter top node
            GenerateAccessibleName(ref filterItemsToolStripMenuItem);
            musicRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editMusic;      // Select the Music Filter top node
            GenerateAccessibleName(ref musicRepresentationToolStripMenuItem);
            textRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editText;        // Select the Text filter top node
            GenerateAccessibleName(ref textRepresentationToolStripMenuItem);
            brailleRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editBraille;  // Select the Braille Music filter top node
            GenerateAccessibleName(ref brailleRepresentationToolStripMenuItem);
            // partsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editParts;  // See comment in ShortcutHandler.cs
            // detailsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editDetails; // See comment in ShortcutHandler.cs
            uncheckAllToolStripMenuItem.ShortcutKeys = ShortcutHandler.uncheckAll;
            GenerateAccessibleName(ref uncheckAllToolStripMenuItem);
            checkAllToolStripMenuItem.ShortcutKeys = ShortcutHandler.checkAll;
            GenerateAccessibleName(ref checkAllToolStripMenuItem);

            // Direct shortcuts for the 3 commands opening the ParameterInputForm
            repeatToolStripMenuItem.ShortcutKeys = ShortcutHandler.CommandRepeat;
            GenerateAccessibleName(ref repeatToolStripMenuItem);
            goToToolStripMenuItem.ShortcutKeys = ShortcutHandler.CommandGoto;
            GenerateAccessibleName(ref goToToolStripMenuItem);
            ofNominalTempoToolStripMenuItem.ShortcutKeys = ShortcutHandler.CommandTempo;
            GenerateAccessibleName(ref ofNominalTempoToolStripMenuItem);


            //Children of viewToolStripMenuItem:
            instrumentsToolStripMenuItem.Text = ResourcesForUI.ToolsStripMenuItem_View_Instruments;
            instrumentsToolStripMenuItem.ShortcutKeys = ShortcutHandler.DetailsInstruments;
            GenerateAccessibleName(ref instrumentsToolStripMenuItem);

            // Children of  toolsToolStripMenuItem
            museScoreToolStripMenuItem.Text =                   ResourcesForUI.ToolStripMenuItem_Tools_MuseScore;
            sibeliusToolStripMenuItem.Text =                    ResourcesForUI.ToolStripMenuItem_Tools_Sibelius;
            logfileToolStripMenuItem.Text =                     ResourcesForUI.ToolStripMenuItem_Tools_Logfile;
            openXMLFileLocationToolStripMenuItem.Text =         ResourcesForUI.ToolStripMenuItem_Tools_OpenXmlFileLocation;
            openLogFileLocationToolStripMenuItem.Text =         ResourcesForUI.ToolStripMenuItem_Tools_Logfile_Location;
            inspectAsXMLToolStripMenuItem.Text =                ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml;
            viewAsInterpretedXMLToolStripMenuItem.Text =        ResourcesForUI.ToolStripMenuItem_Tools_ViewAsInterpretedXml;
            jAWSSettingsToolStripMenuItem.Text =                ResourcesForUI.ToolStripMenuItem_Tools_JAWS_Settings;

            // Children of  helpToolStripMenuItem
            this.aboutIBOSMusicXmlReaderToolStripMenuItem.Text= string.Format("{0} {1}",ResourcesForUI.ToolStripMenuItem_Help_About,applicationName);
            this.keyboardShortcutsToolStripMenuItem.Text =      ResourcesForUI.ToolStripMenuItem_Help_Shortcuts;
            this.linkToNewestSoftwareToolStripMenuItem.Text =   ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdate;
            this.usersManualToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_ShowUsersManual;

            // CheckShortCuts();
        }

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
                Logger.Log(string.Format("CultureInfo.CurrentUICulture.Name={0} ResourceFile={1}", currentCultureName, ResourcesForUI.ResourceFileName));
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
                Logger.Log(string.Format("LogGLobalisationInformation threw an exception. Message={0}", e.Message));
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

        private long currentSequenceNumber = 0; // For avoiding overwriting relavant status information witn obsolete status information
        public void WriteStatusInformation(string s)
        {
            WriteStatusInformation(s, ++currentSequenceNumber);
        }

        private string latestStatusInformation = "";

        delegate void WriteStatusInformationCallback(string s,long sequenceNumber);
        private void WriteStatusInformation(string s, long sequenceNumber)
        {
            string functionName = "WriteStatusInformation";
            if (textBoxStatusInformation.InvokeRequired)
            {
                Logger.Log(string.Format("{0}.{1}.IR({2})", className, functionName, s));
                WriteStatusInformationCallback d = new WriteStatusInformationCallback(WriteStatusInformation);
                textBoxStatusInformation.Invoke(d, s, sequenceNumber);
            }
            else
            {
                // Logger.Log(string.Format("{0}.{1}.INR({2},{3} Current={4})", className, functionName, s,sequenceNumber,currentSequenceNumber));
                if (sequenceNumber == currentSequenceNumber)
                {    
                    // Logger.Log(string.Format("{0}.{1}.INR: Showed Status:{2}", className, functionName, s));
                    textBoxStatusInformation.Text = s;
                    textBoxStatusInformation.Refresh();
                    latestStatusInformation = s;
                }
                else
                {
                    // Explicitly skip obsolete status information
                    Logger.Log(string.Format("{0}.{1}.INR: Ignored Status:{2}", className, functionName, s));
                }
            }
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
                case ModelMessageEnum.LocationNotDetermined: return ResourcesForUI.Message_LocationNotDetermined;
                case ModelMessageEnum.UnspecifiedInitializationError: return ResourcesForUI.Message_UnspecifiedInitializationError;
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
                case ModelMessageEnum.UnspecifiedInitializationError: return ResourcesForUI.Message_PleaseSeeLogFile;
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
            MessageBox.Show(formattedMessage, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
#endregion

        
        /// <summary>
        ///  Clear the contents of the listbox showing the timed events (important when loading a new file)
        /// </summary>
        private void ClearUI()
        {
            listBoxTimesEmptyLineIndex = -1; // No empty line inserted
            listBoxTimes.Items.Clear();
            listBoxTimes.Refresh();
            listBoxTimes.Show();
            listBoxDetails.Items.Clear();
            listBoxDetails.Refresh();
            listBoxDetails.AutoSize = false;
            textBoxBraille.Clear();
            textBoxBraille.Refresh();
            textBoxText.Clear();
            textBoxText.Refresh();
            textBoxStatusInformation.Clear();
            textBoxStatusInformation.Refresh();
            userSettingsTreeView.CollapseAll();
            userSettingsTreeView.Refresh();
            textBoxNormalText.Clear();
            textBoxNormalText.Refresh();
            this.Text = applicationName; // Remove the name of the previously loaded MusicXml file from the title line
        }


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
        private bool SelectAndOpenMusicXmlFile(object sender, EventArgs e)
        {
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.xml;*.mxl", ResourcesForUI.OpenFileDialog_Filter); // Only present .xml files and .mxl files
           //            openFileDialog.Filter = string.Format("{0}|*.xml|{0}|*.mxl", ResourcesForUI.OpenFileDialog_Filter,ResourcesForUI.OpenFileDialog_Filter_mxl); // Only present .xml files and .mxl files
            openFileDialog.InitialDirectory = GetFileOpenInitialDirectory();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ShowDialog();

            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return false; // Let the user press ESC without warning him
            }

            // model.Silence();   // Stop Screenreader talking about the OpenfileDialog, we just left !!

            // Clear all UI BEFORE starting the time consuming Load operation:
           ClearUI();

            // Start filling the UI with information about the NEW file
//            textBoxNormalText.Focus(); // Move focus to the (empty) textBoxNormalText to prevent JAWS form talking too much !!

            string fullFileName = openFileDialog.FileName;
            string shortFileName = System.IO.Path.GetFileName(fullFileName);
            string extension = System.IO.Path.GetExtension(fullFileName);
            string xmlFileName = "";
            string message = string.Format("{0} '{1}'",ResourcesForUI.TextBox_Messages_Reading_File, shortFileName); 
            WriteStatusInformation(message); // Still write messages to the Status line, but without without Focus on the StatusLine
            //textBoxScreenReader.Text = ResourcesForUI.TextBox_Messages_Reading_File; // As textBoxNormalText has Focus, this will be read by the ScreenReader !
            textBoxScreenReader.Text = shortFileName; // As textBoxNormalText has Focus, this will be read by the ScreenReader !
            textBoxScreenReader.Refresh();

            switch (extension)
            {
                case ".mxl": // Attempt to convert from .mxl to .xml first
                    xmlFileName = model.MxlToXml(openFileDialog.FileName);
                    if (string.IsNullOrEmpty(xmlFileName))
                    {
                        message = string.Format("{0} '{1}' {2}", ResourcesForUI.TextBox_Messages_FailedToConvert_File, shortFileName, ResourcesForUI.TextBox_Messages_ToMusicXml); 
                        WriteStatusInformation(message);
                        textBoxStatusInformation.Focus();
                        Beep();
                        MessageBox.Show(message, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                        return false;                   
                    }
                    break;
                case ".xml": // Continue
                    xmlFileName = fullFileName;
                    break;
                default: // Report unsupported fileformat
                    message = string.Format("{0} '{1}'", ResourcesForUI.TextBox_Messages_UnsupportedFileFormat, shortFileName); 
                    WriteStatusInformation(message);
                    textBoxStatusInformation.Focus();
                    Beep();
                    MessageBox.Show(message, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
            }


            Logger.ClearStatistics();  // Clear statistics to be collected while loading, parsing and rendering the MusicXml file:

            // Now follows the time-consuming operation, where the Model loads and interpretes a new MusicXml file.
            if (!model.LoadMusicXmlFile(xmlFileName)) // Load the selected .xml file into the Model and build all internal data structures.
            {
                // Simple error handling
                message = string.Format("{0} '{1}'", ResourcesForUI.TextBox_Messages_FailedToRead_File, shortFileName); // Short filename for UI
                WriteStatusInformation(message);
                ShowWarning((int)ModelMessageEnum.FailedToReadMusicXmlFile, shortFileName, "");
                MessageBox.Show(message, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Log(string.Format("Failed to read {0}", openFileDialog.FileName)); // Full filename for UI
                Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() until now
                this.Text = applicationName; // Remove any exixting filename from the title bar
                return false;
            }

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
            WriteStatusInformation(GetStatusFromMetaInformation());

            // Transfer from model to ListboxTimes
            // model.Silence();
            foreach (EventDescription eventDescription in model.EventDescriptionList.Events)
            {
                listBoxTimes.Items.Add(eventDescription);
            }
 
            Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() during parsing, interpreting and rendering the file

           this.Text = GetTitleInfo();

            model.SetUserTempo(100); // Play at 100% of tempo specified in MusicXml file

            return true;
        }

        private string GetFileOpenInitialDirectory()
        {
            string functionName = "GetOpenFileInitialDirectory";
            string result = myMusicXmlDirectory;  // When running a user session we want to use the files in the <user>\Documents\IBOS NNodelæser directory 
            //if (model.InitialDirectory.Contains("Visual Studio"))
            //{
            //    result = model.InitialDirectory;   // When running a debug session we want to use the files in the debug\bin directory
            //}
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
                                            , applicationName // 0
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
            string result = string.Format("{0}   {1}   {2}   {3}   {4}   {5}   {6}   {7}"
                                            , "" // 0 No need to repeat the application name here !
                                            , model.MetaInformation.FileName // 1
                                            , model.MetaInformation.MovementTitle // 2
                                            , model.MetaInformation.MovementNumber // 3
                                            , model.MetaInformation.Work // 4
                                            , model.MetaInformation.Source // 5 
                                            , model.MetaInformation.Creator // 6
                                            , model.MetaInformation.Encoding // 7
                                         );
            return result;
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
            const string functionName = "openMusicXmlFileToolStripMenuItem_Click";

            // model.Silence();
   
            try
            {
                // throw new Exception("Test");
                textBoxScreenReader.Text = "";
                textBoxScreenReader.Refresh();         
                textBoxScreenReader.Show();
                textBoxScreenReader.Focus(); // Move focus to the (empty) textBoxScreenreader to prevent JAWS form talking too much !! 

                // Show a standard Select File dialog to allow the user to select and open a MusicXml file
                SelectAndOpenMusicXmlFile(sender, e);

                if (listBoxTimes.Items.Count > 0)
                {
                    // Note: This is a HACK, which forces the JIT compiler to handle the code for scrolling outside the initially visible part of the listbox.
                    // This is needed to prevent unstable rhythm during autoplaying when the cursor leaves the initially visible part of the listbox
                    // and thus forces the first scroll operation.
#if true
                    // Not needed if we kan avoid the "if InvoceRequired mechanism above (Line 459)
                    Logger.Log(string.Format("{0}.{1} Changing SelectedIndex to {2} and back to 0", className, functionName, listBoxTimes.Items.Count - 1));
                    listBoxTimes.SelectedIndex = (listBoxTimes.Items.Count - 1);
#endif
                    listBoxTimes.SelectedIndex = 0;
                }

            }
            catch (Exception exception)
            {
                // Be sure to restore the UI state !
                Beep();
                Logger.Log(string.Format("{0}.{1} Exception. Message={2}", className, functionName, exception.Message));
            }
            // At last move focus (from the NormalText textbox) to the mail Listbox to make the Screenreader do its job
            listBoxTimes.Focus();
            textBoxScreenReader.Hide();
            autoReload = true;

        }



#region ListBoxTimes

        private void ListBoxTimesIndexChanged(int index)
        {
            object o = listBoxTimes.Items[index];
            EventDescription eventDescription = o as EventDescription;
            model.musicPlayer.SelectedIndexChanged(eventDescription);
            model.brailleDisplayer.SelectedIndexChanged(eventDescription);
            model.textDisplayer.SelectedIndexChanged(eventDescription);
        }

        private void ListBoxTimes_GotFocus(object sender, EventArgs e)
        {
            int index = listBoxTimes.SelectedIndex;
            Logger.Trace(string.Format("ListBoxTimes_GotFocus(i={0})", index));
            // Even if we got focus we can not be sure that an item is selected!
            if (-1 != index)
            {
                // If an index is selected do as if Selected Index changed
                ListBoxTimesIndexChanged(index);
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
            ListBoxTimesIndexChanged(index);
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
            string message = ResourcesForUI.TextBox_Messages_TheProgramIsExiting;
            WriteStatusInformation(message);  
            // Remaining actions are taken in Application_ApplicationExit.
            // In this way the Model will always be shut down no matter why the application exits.
            Application.Exit();
        }

#region keyhandlers


        private void listBoxDetails_KeyDown(object sender, KeyEventArgs e)
        {
            detailsHandler.KeyDown(sender, e);
        }


        private void ShowUsersManual()
        {
            string functionName = "ShowUsersManual";
            // Construct the localized full path of the user's manual
            string documentationDirectory = System.IO.Path.Combine(Utilities.GetExecutingDirectory(), "Documentation"); // Not to be localized !!
            string localizedDocumentationDirectory = Path.Combine(documentationDirectory, ResourcesForUI.DirectoryNames_CultureString); //  "da-DK" or "en-US");
            string localizedFileName = ResourcesForUI.FileNames_UsersManual;//  "Brugervejledning.doc");
            string localizedFilenameEstension = Path.GetExtension(localizedFileName);
            string LocalizedFilenameWithoutEstension = Path.GetFileNameWithoutExtension(localizedFileName);
            string loalizedDocumentationDocument = Path.Combine(localizedDocumentationDirectory, localizedFileName);
            string sourcefilename = localizedFileName;
            try
            {
                // In some languages (such as danish) the filename contains special characters such as "æ" in "IBOS Nodelæser"
                // During the Build process we can not handle the "æ" so we look at the start of filename and the extension only.
                // This has to do with limitations in the .cmd file used for copying documentation files during build.
                foreach (string filename in Directory.GetFiles(localizedDocumentationDirectory))
                {
                    if ((Path.GetFileNameWithoutExtension(filename).StartsWith(LocalizedFilenameWithoutEstension))
                    &&  (localizedFilenameEstension == Path.GetExtension(filename)))
                    {
                        sourcefilename = filename;
                        break;
                    }

                }
                // throw new Exception("TEST"); // For test only !!
                // Open the user's manual by the application associated with its extension, for instance Word for a .doc file
                // But first copy the file to the user's temp directory before opening it !!!
                // We do not want to expose the installation path, and in some installations the user probably can't access it except for execution !
                string tempDirectory = Path.Combine(Logger.MusicXmlReaderTempDirectory, "tempDirectoryUsedByShowUsersManual"); // Probably a unique name
                Utilities.CreateEmptyTempDirectory(tempDirectory);
                string tempDocumentFileName = Path.Combine(tempDirectory, localizedFileName);
                bool overWrite = true;
                System.IO.File.Copy(sourcefilename, tempDocumentFileName, overWrite);
                // For instance: C:\Users\<user>\AppData\Local\Temp\MusicXmlReader\tempDirectoryUsedByShowUsersManual\Brugervejledning.doc
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempDocumentFileName);
                Logger.Log(string.Format("{0}.{1}: Process.Start({2}) succeeded", className, functionName, tempDocumentFileName));
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} threw an exception: Message={2}", className, functionName, e.Message));
                string formattedMessage = string.Format("{0} {1}", ResourcesForUI.Message_FailedToShow, localizedFileName); // Show filename only, no path !
                MessageBox.Show(formattedMessage, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxTimes_KeyDown(object sender, KeyEventArgs e)
        {
            // string functionName = "listBoxTimes_KeyDown";

            bool handled = true; // Will be set to false again by the "default:" case if the key is not handled  by one of the specific cases.
            bool warnIfEmpty = true; // Will be set to false on commands that do not require that a MusicXml file is loaded.
            switch (e.KeyData) // KeyDate contains information about the control keys (<CTRL> <ALT> <SHIFT>  etc )as well as about the normal ley
            {    
                // Most functionality is passed directly to the Model
                case ShortcutHandler.togglePlaying:         model.ToggleStartStopPlaying(listBoxTimes.SelectedIndex); break; // Keys.Space
                case ShortcutHandler.tempoDecrement:        model.ChangeUserTempo(-1); break;
                case ShortcutHandler.tempoIncrement:        model.ChangeUserTempo(+1); break;
                case ShortcutHandler.startPlaying:          model.StartPlayingPoly(listBoxTimes.SelectedIndex); break;
                case ShortcutHandler.stopPlaying:           model.StopPlaying(); break;
                case ShortcutHandler.StopAllNotesPlaying:   model.musicPlayer.StopAllNotesPlaying(); warnIfEmpty = false;  break;
                case ShortcutHandler.PreviousMeasure:       model.SelectMeasure(listBoxTimes.SelectedIndex, -1); break;
                case ShortcutHandler.NextMeasure:           model.SelectMeasure(listBoxTimes.SelectedIndex, +1); break;
                case ShortcutHandler.NextEvent:             UiUtilities.WarnAtEnd(listBoxTimes, +1);  handled = false; break; // Let the listbox handle it
                case ShortcutHandler.PreviousEvent:         UiUtilities.WarnAtEnd(listBoxTimes, -1); handled = false; break; // Let the listbox handle it



                // The "Details functionality is handled locally before being passed to the Model:
                case ShortcutHandler.DetailsHarmonyTop:     detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Harmonies, true); break;    // Start from top
                case ShortcutHandler.DetailsPartsTop:       detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Parts, true); break;        // Start from top  
                case ShortcutHandler.DetailsNotesTop:       detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Notes, true); break;        // Start from top  
                case ShortcutHandler.DetailsHarmonyBottom:  detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Harmonies, false); break;   // Start from bottom
                case ShortcutHandler.DetailsPartsBottom:    detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Parts, false); break;       //  Start from bottom
                case ShortcutHandler.DetailsNotesBottom:    detailsHandler.ShowEventDetails(DetailsHandler.DetailsEnum.Notes, false); break;       //  Start from bottom
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

        enum CheckboxOperation { Unknown, Check, Uncheck, ToggleAndCopy};

        /// <summary>
        /// Candle chsckboxes, distributed in the tree
        /// </summary>
        /// <param name="checkboxOperation"></param>
        /// <returns></returns>
        bool UpdateCheckBoxes(CheckboxOperation checkboxOperation)
        {
            string functionName = "UpdateCheckBoxes";
            Logger.Log(string.Format("{0}.{1}({2})", className, functionName, checkboxOperation));
            bool result = false;
            bool newValue;
            // We only handle the shortcuts specified in shortCutHandler
            switch (checkboxOperation)
            {
                case CheckboxOperation.Check:           newValue = true; break;
                case CheckboxOperation.Uncheck:         newValue = false; break;
                case CheckboxOperation.ToggleAndCopy:   newValue = !userSettingsTreeView.SelectedNode.Checked; result = true; break;
                default: return false;
            }

            // This only has meaning if a node is selected !
            if (null == userSettingsTreeView.SelectedNode) return result; 

            // We only handle level 2 nodes
            if (2 != userSettingsTreeView.SelectedNode.Level) return result;

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
            return result;
        }


        /// <summary>
        /// Occurs when a key is pressed while treeView has focus         
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userSettingsTreeView_KeyDown(object sender, KeyEventArgs e)
        {

            if (consoleTrace) Console.WriteLine("userSettingsTreeView_KeyDown");
            if (e.KeyData == ShortcutHandler.listBoxFocus)
            {
                listBoxTimes.Focus(); // Easy way to move the focus to the main listbox
                e.SuppressKeyPress = true;
                return;
            }
                 

            if (null == userSettingsTreeView.SelectedNode)
            {
                e.SuppressKeyPress = true;
                return;
            }

            // Otherwise let the treeview itself handle it

//            switch (e.KeyData)
//            {
//                case ShortcutHandler.uncheckAll:     e.Handled = UpdateCheckBoxes(CheckboxOperation.Uncheck); break;
//                case ShortcutHandler.checkAll:       e.Handled = UpdateCheckBoxes(CheckboxOperation.Check); break;
////                case ShortcutHandler.toggleAndCopy:  e.Handled = UpdateCheckBoxes(CheckboxOperation.ToggleAndCopy); break; // Removed, undocumented feature
//                default: break;
//            }
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
            string caption = applicationName;
            string text = string.Format("{0}={1}", ResourcesForUI.ToolStripMenuItem_Help_About_Version, version.ToString());
            switch (model.ScreenReaderName)
            {
                case "JAWS": break; // JAWS is the default screenreader
                case "DummyScreenReader": break;     // We ignore when no screenreader is running
                default:
                    // Any other screenreader will be reported.
                    text = text + "\r\r" + ResourcesForUI.Message_ConnectedToNonDefaultScreenReader + ": " + model.ScreenReaderName;
                    text = text + "\r" + ResourcesForUI.Message_MayNotWorkAsExpected;
                    break;
            }
                
            MessageBox.Show(text, caption);
        }


        private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string caption = applicationName + " " + ResourcesForUI.ToolStripMenuItem_Help_Shortcuts;
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

#region ignoreAltF4
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // The form is closing
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                // The reason is that the user pressed ALT+F4
                if (DialogResult.Yes != MessageBox.Show(ResourcesForUI.Message_DoYouWantToExitTheProgram, applicationName, MessageBoxButtons.YesNo))
                {
                    e.Cancel = true;
                }
            }
            //altF4Pressed = false;
        }

        private void listBoxDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxDetails.SelectedIndex;
            object o = listBoxDetails.Items[index];
            DetailsDescription detailsDescription = o as DetailsDescription;
            model.SelectedDetailsIndexChanged(detailsDescription);
        }

        private void listBoxDetails_Leave(object sender, EventArgs e)
        {
            model.ListBoxDetailsLeave();
            listBoxDetails.Items.Clear();
            listBoxDetails.AutoSize = false;
            listBoxTimes.Show();
        }

#region Import
        private void importNewSampleFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importHandler.ImportNewSample();
        }

        private void importNewestDownloadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importHandler.ImportNewestDownloads();
        }

        private void importDownloadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importHandler.ImportDownloads(openFileDialog);
        }
#endregion // Import


        private void linkToNewestSoftwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // string url = "http://www.ibos.dk/hjaelpemidler/ibos-nodelaeser.html";
            string url = ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdateLink;
            model.ExternalToolsHandler.OpenUrl(url);         
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCheckBoxes(CheckboxOperation.Uncheck);
        }

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCheckBoxes(CheckboxOperation.Check);
        }

        private void instrumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailsHandler.ShowGlobalDetails(DetailsHandler.DetailsEnum.Instruments, true);
        }

        private void usersManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUsersManual();
        }

        /// <summary>
        /// Unfortunately listBoxTimes_Enter and listBoxTimes_Enter seem to be needed in order to prevent JAWS from reading the selected line in listBoxTimes
        /// after reading the item from the control we are entering, A better solution is wanted !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxTimes_Enter(object sender, EventArgs e)
        {
            string functionName = "listBoxTimes_Enter";
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

        /// <summary>
        /// Unfortunately listBoxTimes_Enter and listBoxTimes_Enter seem to be needed in order to prevent JAWS from reading the selected line in listBoxTimes
        /// after reading the item from the control we are entering, A better solution is wanted !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxTimes_Leave(object sender, EventArgs e)
        {
            string functionName = "listBoxTimes_Leave";
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

        int listBoxTimesEmptyLineIndex = -1; // Mark that no extra line is inserted



        /// <summary>
        /// Experimental code for acting on invalid character input to the ToolStripMenu
        /// by playing a beep and returning focus.
        /// Sometimes removes fosus permanently
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuStrip_KeyDown(object sender, KeyEventArgs e)
        {
            // return; // Insert "return" here to disable the functionality

            string functionName = "MenuStrip_KeyDown";
            // Logger.Log(string.Format("{0}.{1} Entry", className, functionName));
            string keyAsString = e.KeyCode.ToString();
            bool passOn = (e.KeyCode == Keys.Menu); //Pass ESC on for default handling

            foreach (ToolStripMenuItem toolStripMenuItem in MenuStrip.Items)
            {
                string text = toolStripMenuItem.Text;
                string shortcutName = Utilities.GetShortcutName(text);
                if (keyAsString == shortcutName)
                {
                    passOn = true; // Pass all characters found  as "&" shortcuts unchanged.
                    break;
                }
            }

            if (passOn)
            {
                Logger.Log(string.Format("{0}.{1} passing '{2}' for default handling", className, functionName, keyAsString));
                return; 
            }


            // On any other key: Report the unexpected key by a Beep and send an ESC to return focus to the previous control
            System.Media.SystemSounds.Beep.Play();
            System.Windows.Forms.SendKeys.Send(@"{ESC}");
            Logger.Log(string.Format("{0}.{1} Beep and ESC for {2}", className, functionName, keyAsString));
        }

        private void userSettingsTreeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (consoleTrace) Console.WriteLine("userSettingsTreeView_KeyPress");
        }

        private void userSettingsTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (consoleTrace) Console.WriteLine("userSettingsTreeView_KeyUp");
        }

#region Repeat,GoTo,NormalTempo ***********************************************************************************


        private void ReportSyntax(bool ok, string name, int value, string input)
        {
            const string functionName = "ReportSyntax";
            string s;
            if (ok)
            {
                s = name + value.ToString(); // For test 
            }
            else
            {
                s = name + ": Invalid systax " + input;
            }
            Logger.Log(string.Format("{0}.{1} {2}", className, functionName, s));
        }


        private DialogResult ShowParameterInputForm(ParameterDescription p, out string parameters)
        {
            const string functionName = "ShowParameterInputForm";
            ParameterInputForm parameterInputForm = new ParameterInputForm();
            parameterInputForm.ParameterDescription = p;
            parameterInputForm.Text = p.Name;
            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            DialogResult dialogResult = parameterInputForm.ShowDialog(this);
            Logger.Log(string.Format("{0}.{1} returned {2}", className, functionName, dialogResult));          
            parameters = parameterInputForm.ComboBoxInput;
            parameterInputForm.Dispose();
            return dialogResult;
        }

        private void repeatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string functionName = "repeatToolStripMenuItem_Click";
            Logger.Log(string.Format("{0}.{1}",className,functionName));
            RepeatParameterDescription p = new RepeatParameterDescription();
            string input = "";
            if (DialogResult.OK != ShowParameterInputForm(p, out input)) return;                 
            int n1 = 0;
            int n2 = 0;
            bool ok = (p.CheckSyntax(input, out n1, out n2));
            string s;
            if (ok)
            {
                s = "Repeat from " + n1 + " to " + n2;
            }
            else
            {
                Beep();
                s = "Repeat: Invalid systax " + input;
            }
            Logger.Log(string.Format("{0}.{1} {2}", className, functionName, s));
            int iStart = 0;
            int iStop = 0;
            if ( ok  && (n1 >= 0) && (n2 >= 0) && (model.MeasureToIndex(n1, ref iStart))  && (model.MeasureToIndex(n2 + 1, ref iStop)))
            {
                model.StartRepeating(iStart, iStop + 1); // Means "Repeat [measure n1 to measure n2]"
            }
            else
            {
                Beep();
                model.StopRepeating();
            }
            
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Utilities.RemoveAmpersant(ResourcesForUI.ParameterInputForm_GoTo); // "Localize(GoTo)";
            const string functionName = "goToToolStripMenuItem_Click";
            Logger.Log(string.Format("{0}.{1}", className, functionName));
            SingleIntParameterDescription p = new SingleIntParameterDescription(name);
            string input = ""; ;
            if (DialogResult.OK != ShowParameterInputForm(p, out input)) return;
            int value = 0;
            bool ok = (p.CheckSyntax(input, out value));
            ReportSyntax(ok, name, value, input);
            int index = 0;
            if ( ok && (value >= 0) && model.MeasureToIndex(value, ref index))
            {
                listBoxTimes.SelectedIndex = index;
            }
            else
            {
                Beep();
                Logger.Log(string.Format("{0}.{1} Illegal GoTo-command:'{2}'", className, functionName, input));
            }  
        }

        private void ofNominalTempoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string functionName = "ofNominalTempoToolStripMenuItem_Click";
            Logger.Log(string.Format("{0}.{1}", className, functionName));
            string name = Utilities.RemoveAmpersant(ResourcesForUI.ParameterInputForm_PctOfNominalTempo); // "Localize(% of &Normalt Tempo)";
            SingleIntParameterDescription p = new SingleIntParameterDescription(name);
            string input = "";
            if (DialogResult.OK != ShowParameterInputForm(p, out input)) return;
            int value = 0;
            bool ok = (p.CheckSyntax(input, out value));
            ReportSyntax(ok, name, value, input);

            if (ok && (value >= UiUtilities.TempoFactorMinimum) && (value <= UiUtilities.TempoFactorMaximum))
            {
                model.SetUserTempo(value);
            }
            else
            {
                Beep();
                Logger.Log(string.Format("{0}.{1} Illegal Tempo-Command:'{2}'", className, functionName, input));
            }


        }

        /// <summary>
        /// Seems to be needed to compensate for an error in the communication between WINDOWS and JAWS:
        /// After a form Resize the INSERT+PAGEDOWN JAWS shortcut no longer reads the control positioned at the bottom of the form, in this case the Status Line.
        /// Instead some other undefined information is read, appa rantly depending on the contents of the other controls on the form.
        /// The HACK below seems to compensate for this error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            string functionName = "MainForm_SizeChanged";
            Logger.Log(string.Format("{0}.{1} Form size changed to Width={2} Height={3} Restoring StatusInformation to '{4}'", className, functionName,this.Width,this.Height, latestStatusInformation));
            this.textBoxStatusInformation.Text = latestStatusInformation; // Restore prevopus contents after resize !!
            this.Refresh();
        }

        #endregion ****************************************************************************************

        #endregion

        #endregion
        //*************************************************************************************************

    }

}
