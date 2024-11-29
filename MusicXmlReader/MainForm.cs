using System;
using System.Windows.Forms;
using System.Drawing;
using MusicXmlReaderModel;
using MusicBrailleReader;
using System.Runtime.Remoting.Messaging;
using MusicXmlReader.Properties;

namespace MusicXmlReader
{
    /// <summary>
    /// The 4 interfaces are used for
    /// IBrailleDisplayerClient     Let the Model write MusicBraille patterns to the appropriate Textbox
    /// IObjectCollection   Let the Model access the main Listbox when auto-playing music
    /// IUtilityClient      Let the Model show MessageBoxes
    /// IMusicBrailleReaderClient Let the MusicBrailleReader access some of the variables of the MainFoem 
    /// By using these interfaces we avoid that the Model needs to know anything abour Windows Forms!
    /// This makes it much easier to reuse the Model for othea applications and other platforms.
    /// </summary>
    public partial class MainForm : Form, IDebugDisplayerClient, IObjectCollection, IUtilityClient, IMusicBrailleReaderClient, IModelBaseMessageBox
    {
        string className = "MainForm";
        bool developerMode; // Can be set in app.Config
        bool decoderDeveloperMode; // Can be set in app.Config
        bool exportToMusicXml;  // Can be set in app.Config 
        string localizationMessage = ""; // Will contain a formatted message if the default UI Culture is overwritten by App.Config
        //bool experimentalCode;  // Can be set in app.Config
        public static readonly Color FocusedColor = Color.White;         // Mainly for debugging. For released versions use Color.White !
        public static readonly Color NonFocusedColor = Color.WhiteSmoke; // Mainly for debugging. For released versions use Color.White !
        // bool consoleTrace = false;
        enum MusicPlayerStateEnum { unknown, stopped, running };
        string applicationName = "";  // Locakized application name. Will be re-initialized later using localization!
        string executingAssemblyFullPath  = ""; // The (unlocalized) name and location of the program, 
        string executingAssemblyShortName = ""; // The (unlocalized) short name of the program, used by for instance JAWS to name configuration file! 
        string myMusicXmlDirectory; // Default location for MusicXml files belonging to thos user. Will be populated with sample files!
        private OrganisationDependencies organisationDependencies;
        private bool is64Bit = (IntPtr.Size == 8);
        private Version assemblyVersion;

        Model model;        // The Model containing all of the business logic.

        // Most of the UI code in the MainForm class is distributed into the following "Handler" classes, each handling a specific UI control
        UserSettingsHandler     userSettingsHandler; // Contains all settings for each single score score that can be configured by the user
        UserPreferencesHandler  userPreferencesHandler;  // Contains all application-wide settings that can be configured by the user
        ImportHandler           importHandler;
        DetailsHandler          detailsHandler;
        ParameterInputHandler   parameterInputHandler;
        ListBoxTimesHandler     listBoxTimesHandler;
        MessageHandler          messageHandler;
        BrailleMusicExportHandler brailleMusicExportHandler; // Isolates most code for handling export to files of Music Braille
        EmbosserHandler         embosserHandler;
        string[] arguments;

        public MainForm()
        {
            string functionName = "MainForm"; // Only for logging
            try
            {
                UiUtilities.Beep(); // To easily check if the Beep() function works as expected!
                executingAssemblyFullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                executingAssemblyShortName = System.IO.Path.GetFileNameWithoutExtension(executingAssemblyFullPath);
                assemblyVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                arguments = Environment.GetCommandLineArgs();
                InitializeComponent();
                Logger.Open(null); // null => Use the default logfile name
                Logger.Log(""); // An empty line to catch the eye
                Logger.Log(string.Format("{0}.{1} Starting: Date={2}", className, functionName, System.DateTime.Now.ToLongDateString()));
                Application.ApplicationExit += Application_ApplicationExit;

                ModelBaseMessageBox.Init(this as IModelBaseMessageBox); // Allows any part of the application with reference to MusicXmlReaderBase to show a MEssageBox 

                string developerModeString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.DeveloperMode);
                developerMode = ("yes" == developerModeString);
                Logger.DeveloperMode = developerMode;
                Logger.LogArguments(arguments);

                string exportToMusicXmlString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.ExportToMusicXml);
                exportToMusicXml = ("yes" == exportToMusicXmlString);
                Logger.ExportToMusicXml = exportToMusicXml;

                string decoderDeveloperModeString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.DecoderDeveloperMode);
                decoderDeveloperMode = ("yes" == decoderDeveloperModeString);


                UiUtilities.CheckInstallation(executingAssemblyFullPath, 30, 4); // Warn about unexpected files in the installation directory          

                Logger.LogCF(string.Format(": DeveloperModeString={0} DeveloperMode={1}", developerModeString, developerMode));
                string developerCultureString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.DeveloperCulture);
                Logger.LogCF(string.Format(": DeveloperLanguageString='{0}'", developerCultureString));

                UiUtilities.LogSystemInformation();

                // If App.Config contains an entry named "DeveloperCulture" describing a valid culture string, for instance "en-US" or "ko-KR"
                // the application culture will be changed to that even if running on a danish PC!
                localizationMessage = UiUtilities.LogGLobalisationInformation(developerCultureString);
                organisationDependencies = OrganisationDependencies.Create(executingAssemblyShortName); 

                // Do any UI localization before we create the model. In this way we avoid showing unlocalized texts if an error is reported by a messagebox.
                applicationName = organisationDependencies.ApplicationName; // Defaults to ResourcesForUI.MainForm_ApplicationName;
                UiUtilities.CheckExe(applicationName, executingAssemblyFullPath, "IBOS MusicXmlReader.exe"); // Check that this is not for instance a Git "modstridende kopi" file
                Logger.Log(string.Format("This program is compiled for a {0} bit architechture. It uses the following locally installed executable", is64Bit ? "64" : "32"));
                // Utilities is a static class so we can it call it before creationg the Model!
                Utilities.CheckExe(System.IO.Path.GetFileName(executingAssemblyFullPath), System.IO.Path.GetDirectoryName(executingAssemblyFullPath), is64Bit);
                messageHandler = MessageHandler.Create(applicationName);
           
                textBoxScreenReader.Hide(); // This textbox gets Focus used during long-lasting operation and thus draws the Screenreaders attensio to itself, avoiding too much Speech !
                LocalizeMenuStrip(); // Overwrite all items in MenuStrip with localized texts
                HideDeveloperItems(developerMode);
                textBoxStatusInformation.AccessibleName = ResourcesForUI.StatusLine_Accessible_Name; // Overwrite with localized text 


                Utilities.UtilityClient = (this as IUtilityClient); //Decide how to show error messages and warnings
                model = Model.Create((this as IObjectCollection), (this as IDebugDisplayerClient), applicationName,null);
                model.scriptHandlerForJAWS.OnProgramStart(executingAssemblyFullPath); // Install JAWS scripts if needed

                #region Configuration
#warning refactor all configuration stuff into separate methode somewhere
                string experimentalCodeString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.ExperimentalCode);
                model.ExperimentalCode = ("yes" == experimentalCodeString);
                //Logger.LogCF(string.Format(": ExperimentalCodeString={0} ExperimentalCode={1}", experimentalCodeString, model.ExperimentalCode));

                string handleGraphicsString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.HandleGraphics);
                model.HandleGraphics = ("yes" == handleGraphicsString);
                //Logger.LogCF(string.Format(": HandleGraphicsString={0} HandleGraphicsString={1}", handleGraphicsString, model.HandleGraphics));

                string useExternal7ZipString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.UseExternal7Zip);
                model.UseExternal7Zip = ("yes" == useExternal7ZipString);
                //Logger.LogCF(string.Format(": UseExternal7ZipString={0} UseExternal7Zip={1}", useExternal7ZipString,  model.UseExternal7Zip));
                #endregion Configuration

                Utilities.LogSpecialFolders(true); // A Developer facility only!
                importHandler = ImportHandler.Create(model,this,applicationName);
                parameterInputHandler = ParameterInputHandler.Create(model,this);
                userPreferencesHandler = UserPreferencesHandler.Create();
                brailleMusicExportHandler = BrailleMusicExportHandler.Create(model, parameterInputHandler, messageHandler, saveBrailleFileDialog, developerMode, userPreferencesHandler);
                embosserHandler = EmbosserHandler.Create(this.openFileDialog, this.printDialog, this.applicationName);

                this.Text = applicationName + " " + assemblyVersion.ToString();
                WriteStatusInformation(model.ScreenReaderName);

                // XCopy MusicXml samples from the "MusicXml samples" directory in the installation files to myMusicXmlDirectory during first activation ! 
                myMusicXmlDirectory = model.InitMusicXmlFiles(applicationName, ResourcesForUI.DirectoryNames_Samples, ResourcesForUI.DirectoryNames_Downloads);

                // Initially none of the 3 main components has focus.
                listBoxTimes.BackColor = NonFocusedColor;
                listBoxDetails.BackColor = NonFocusedColor;
                userSettingsTreeView.BackColor = NonFocusedColor;
                if (!developerMode) textBoxText.Hide(); // Not for the end user. Can be reenabled in App.Config during debugging of Braille Music handling               

                // Create remaining handlers. Some of the need references to others
                detailsHandler = DetailsHandler.Create(listBoxTimes, listBoxDetails, model, this as IDebugDisplayerClient);
                listBoxTimesHandler = ListBoxTimesHandler.Create(model, detailsHandler, listBoxTimes);

                // Create a handler for the user settinge, in this case modelled as a treeview.
                userSettingsHandler = UserSettingsHandler.Create(userSettingsTreeView, model, listBoxTimesHandler,this);
                userSettingsHandler.Init(); // Builds up the fixed part of the treeview
                userSettingsHandler.Reset();
                // Create a handler for handling all Keyboard shortcuts
                // shortCutHandler = ShortcutHandler.Create(this, model);

                //userPreferencesHandler.Log(); // Just to verify that the mechanism works. We mus wait until AFTER creation to do this !
                //userPreferencesHandler.embosser.Log();
                //userPreferencesHandler.noteTaker.Log();
                //userPreferencesHandler.embosser.Name = "EmbosserName4";
                //userPreferencesHandler.noteTaker.Name = "NoteTakerName4";        
                // userPreferencesHandler.Save();

                // Experiments.LogRightAlignedMenus(this);

                OverwriteAccessibleNames(); // Experimental code
                LoadIcon();

                this.Shown += MainForm_Shown;

                // Handle a commandlineParameter containing the full path to a MusicXml file to load
                InterpretCommandline(arguments);

                // For test only !!!
                //model.AnalyzeLocalization();

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
#if true
        /// <summary>
        /// Experimental code for experimenting with JAWS scripts, espacially the JAWS scripting functions:
        /// "GetObjectInfoByName" which seems to always fail
        /// "GetListOfObjects"    which seems to return an empty string
        /// </summary>
        private void OverwriteAccessibleNames()
        {
            this.AccessibleName = "FormMain";
            this.MainMenuStrip.AccessibleName = "MenuStripMain";
            this.userSettingsTreeView.AccessibleName = "TreeViewUserSettings";
            this.listBoxTimes.AccessibleName = "ListBoxTimes";
            this.listBoxDetails.AccessibleName = "ListBoxDetails";
            this.textBoxBraille.AccessibleName = "TextBoxBraille";
            this.textBoxEmpty.AccessibleName = "TextBoxEmpty";
            this.textBoxNormalText.AccessibleName = "TextBoxNormalText";
            this.textBoxScreenReader.AccessibleName = "TextBoxScreenReader";
            this.textBoxStatusInformation.AccessibleName = "TextBoxStatusInformation";
            this.textBoxText.AccessibleName = "TextBoxText";
        }
#endif


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>True iff a commandline exists and is successfully executed</returns>
        private bool InterpretCommandline(string[] arguments)
        {
            if (arguments.Length != 2) return false;
            string fileToOpen = arguments[1];
            if (!System.IO.File.Exists(fileToOpen))
            {
                Logger.LogCF(string.Format(": FileToOpen not does not exist: {0} ", fileToOpen));
                return false;
            }
            string extension = System.IO.Path.GetExtension(fileToOpen);
            string upperExtension = extension.ToUpper();
            if (!((upperExtension.EndsWith(".MUSICXML") || upperExtension.EndsWith(".XML"))))
            {
                Logger.LogCF(string.Format(": Unsupported file extension: {0} ", extension));
                return false;
            }

            string shortFileName = System.IO.Path.GetFileName(fileToOpen);
            bool useDefaultSettings = true;
            bool ok = this.LoadMusicXmlFile(fileToOpen, shortFileName, useDefaultSettings);
            Logger.LogCF(string.Format(": {0} loading {1}", ok ? "Success" : "Failure", fileToOpen));
            return ok;
        }


        private void HideDeveloperItems(bool developerMode)
        {
            Hide(generateGraphicInformationToolStripMenuItem, developerMode);
            Hide(interpretFileAsBrailleMusicToolStripMenuItem, developerMode);
            Hide(userPreferencesLocationToolStripMenuItem, developerMode);
            Hide(generateMusicBrailleTestpatternToolStripMenuItem, developerMode);
            Hide(brailleFileToolStripMenuItem, developerMode);
            Hide(userPreferencesLocationToolStripMenuItem, developerMode);
            Hide(viewAsInterpretedXMLToolStripMenuItem, developerMode);
            Hide(analyzeLocalizationToolStripMenuItem, developerMode);
            Hide(logControlpositionsToolStripMenuItem, developerMode);
            Hide(jAWSApplicationspecificScriptToolStripMenuItem, developerMode);
            Hide(jAWSApplicationspecificScriptForCurrentUserToolStripMenuItem, developerMode);
            Hide(jAWSSettingsDirectoryToolStripMenuItem, developerMode);
        }

        private void Hide(ToolStripMenuItem item, bool developermode)
        {
            if (developermode)
            {
                item.ForeColor = Color.MediumVioletRed;
            }
            else
            {
                item.Visible=false;
                item.Enabled=false;
            }
        }

        private void ShowFileAssociationDialog()
        {

#if false // For test: Reset the settint in order to show the dialog again
            Properties.Settings.Default.DoNotShowFileAssociationDialog = false;
            Properties.Settings.Default.Save();
#endif
            bool dontShowAgain = Properties.Settings.Default.DoNotShowFileAssociationDialog;
            Logger.LogCF(string.Format("+: Properties.Settings.Default.DoNotShowFileAssociationDialog = {0}",dontShowAgain));

            if (!dontShowAgain)
            {
                MessageForm mf = new MessageForm(ResourcesForUI.MainForm_ApplicationName, string.Format(ResourcesForUI.Question_DoYouWantToMakeIBOSMusicXmlreaderDefaultAppForMusicXml, ResourcesForUI.MainForm_ApplicationName));
                DialogResult dr = mf.ShowDialog();
                if (dr == DialogResult.Yes)
                {
                    Associate(musicXmlExtension, progIdExecuting, executingAssemblyFullPath);
                    dontShowAgain = true;

                }
                //if ((dontShowAgain) || mf.DontShowAgain) // Probably the best for normal use
                if (mf.DontShowAgain) // Allowing test
                {
                    Properties.Settings.Default.DoNotShowFileAssociationDialog = true;
                    Properties.Settings.Default.Save();
                }
                dontShowAgain = Properties.Settings.Default.DoNotShowFileAssociationDialog;
            }
            Logger.LogCF(string.Format("-: Properties.Settings.Default.DoNotShowFileAssociationDialog = {0}", dontShowAgain));
        }


        /// <summary>
        /// Postpones the reporting of messages generated during the initialisation of Mainform 
        /// to the time when Mainform is first shown.
        /// This will fir instance allow of localisation of these messages if wanted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Shown(object sender, EventArgs e)
        {
            ShowFileAssociationDialog(); // Allow user to set up file association for .MusicXml files

            // Postpone the reporting of messages generated during the initialisation of Mainform 
            /// to the time when Mainform is first shown.
            /// This will for instance allow of localisation of these messages if wanted.
            if (string.IsNullOrEmpty(localizationMessage)) return;
            messageHandler.ShowMessage(localizationMessage);
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
            userPreferencesHandler.Save(); // Save the current user preferences
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


// Implementation of interfaces:

#region IDebugDisplayerClient

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

        delegate void WriteStatusInformationCallback(string s, long sequenceNumber);
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
                    model.LatestStatusInformation = latestStatusInformation;   // Make accessible for all model users  
                }
                else
                {
                    // Explicitly skip obsolete status information
                    Logger.Log(string.Format("{0}.{1}.INR: Ignored Status:{2}", className, functionName, s));
                }
            }
        }

#endregion IDebugDisplayerClient

#region  IObjectCollection

        public int GetNumberOfObjects()
        {
            return listBoxTimesHandler.GetNumberOfObjects();
        }
        
        public object GetObjectAtIndex(int index)
        {
            return listBoxTimesHandler.GetObjectAtIndex(index);
        }

        public void SetSelectedIndex(int index)
        {
            listBoxTimesHandler.SetSelectedIndex(index);
        }
#endregion IObjectCollection

#region IMessageShower 
        // Decide how to show error messages and warnings          
        public void ShowMessage(int messageId,string parameter, string text)
        {
            messageHandler.ShowMessage(messageId, parameter, text);
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
            messageHandler.ShowWarning(messageId, parameter, text);
        }
        #endregion IMessageShower

#region IMusicBrailleReaderClient
        // Implement IMusicBrailleReaderClient 
        /// <summary>
        /// Allows another form to read the Latest Braille Music Path from the User preferences
        /// </summary>
        /// <returns></returns>
        public string GetLatestBrailleMusicPath()
        {
            return this.userPreferencesHandler.BrailleMusicDirectory;
        }

        /// <summary>
        /// Allows another form to read the value of the default directory for the MusicXmlReader application (Where for instance the MusicXml Sample files are located)
        /// </summary>
        /// <returns></returns>
        public string GetMyMusicXmlDirectory()
        {
            return this.myMusicXmlDirectory;
        }


        /// <summary>
        ///  Allows another form to write the Latest Braille Music Path to the User preferences
        /// </summary>
        /// <param name="s"></param>
        public void SetLatestBrailleMusicPath(string s)
        {
            this.userPreferencesHandler.BrailleMusicDirectory = s;
        }
        #endregion

#region IUserMessageBox
        /// <summary>
        /// Implement IUserMessageBox
        /// Use this mechanism to show messageboxes from anywhere in the application, for instance in case of thrown exceptopns
        /// </summary>
        /// <param name="message"></param>
        public void ShowUserMessageBox(string message, ModelBaseMessageBoxButtons buttons, ModelBaseMessageBoxIcon icon)
        {
            // Assuming that Windows.Forms enumerations do not change we can avoid the lookup
            MessageBox.Show(message, applicationName, (MessageBoxButtons) buttons, (MessageBoxIcon) icon);
        }
#endregion

        /// <summary>
        ///  Clear the contents of the listbox showing the timed events (important when loading a new file)
        /// </summary>
        private void ClearUI()
        {
            listBoxTimesHandler.Reset(); // No empty line inserted
            listBoxDetails.Items.Clear();
            listBoxDetails.Refresh();
            listBoxDetails.AutoSize = false;
            textBoxBraille.Clear();
            textBoxBraille.Refresh();
            textBoxText.Clear();
            textBoxText.Refresh();
            textBoxStatusInformation.Clear();
            textBoxStatusInformation.Refresh();
            userSettingsHandler.Reset();
            textBoxNormalText.Clear();
            textBoxNormalText.Refresh();
            this.Text = applicationName; // Remove the name of the previously loaded MusicXml file from the title line
        }

        
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


    }
}
