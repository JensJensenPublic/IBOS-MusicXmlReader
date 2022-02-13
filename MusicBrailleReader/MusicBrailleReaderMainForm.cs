using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MusicXmlReaderModel;
using System.IO;
using System.Xml;
using UiAccessibilityModel; // Common tools for easy implementation af accessibility 
using System.Threading;
using System.Text;
using System.Drawing;

namespace MusicBrailleReader
{

    using LocRes = ResourcesForMusicBrailleReaderMainForm; // Simple shorthand for the one and only localization ressource rerferenced in this file 

    /// <summary>
    /// Simple UI project for demonstrating the Music Braille decoder without involving the maim MusicXmlReader application or ots alia.
    /// Everything is stolen from C:\Users\Jens\Dropbox\Visual Studio 2015\Solutions\Tactile MusicXmlReader\MusicXmlReader\MAinForm.cs
    /// </summary>
    public partial class MusicBrailleReaderMainForm : Form, IRegressionTestClient, IDecoderUiClient
    {
        //const string currentTestFileName = "nr. 30 Godmorgen lille land.txt";
        const string currentTestFileName = "nr 285 Det er forår.txt";
        const string currentTestDirectory = "NOTA fra SN 2021.03.23";

        Model model;        // The Model containing all of the business logic.
        IMusicBrailleReaderClient creatingForm;  // Holds the creating form this class was created by another form through the static Create()
        string executingAssemblyFullPath = ""; // The (unlocalized) name and location of the program, 
        string executingAssemblyShortName = ""; // The (unlocalized) short name of the program, used by for instance JAWS to name configuration file! 
        string fullFileName;
        BrailleFileHandler.FileEncoding fileEncoding;
        string latestMusicXmlFileGenerated = "";
        RegressionTest regressionTest; // For isolating code for Regression test
        bool developerMode = Logger.DeveloperMode;
        private ListBoxOffsetsHandler listboxOffsetsHandler;

        List<DecoderItem> currentInterpretation; // Contains the full interpretation of the current inputfile with the current settings
        DecoderOutputFileHandler decoderOutputFileHandler;
        XmlDocument musicXmlDocument;

        MusicBrailleReaderUserSettingsHandler userSettingsHandler;
        public static readonly Color FocusedColor = Color.White;         // Mainly for debugging. For released versions use Color.White !
        public static readonly Color NonFocusedColor = Color.WhiteSmoke; // Mainly for debugging. For released versions use Color.White !

        string applicationName;
        string openFileDialogFilterName;


        /// <summary>
        /// 
        /// </summary>
        private void LocalizeComponent()
        {
            /// All of these texts are read from the resource files according to "using LocRes = ResourcesForMusicBrailleReaderMainForm;"

            this.applicationName = LocRes.Application_Name; //  "IBOS punktnodelæser"
            this.Text = LocRes.Application_Name; //  "IBOS punktnodelæser"

            // The following items ore only identified by their AccessibleNAme 
            this.userSettingsTreeView.AccessibleName = LocRes.TreeViewUsersSettings_Name; //  "Punktnode filter";
            this.listBoxOffsets.AccessibleName = LocRes.ListBoxOffsets_Name;
            this.textBox3.AccessibleName = LocRes.TextBoxNormalText_Name;
            this.textBoxBraille.AccessibleName = LocRes.TextBoxBraille_Name;
            this.textBoxStatusInformation.AccessibleName = LocRes.TextBoxStatusInformation_Name; //"Status information";
            this.textBoxText.AccessibleName = LocRes.TextBoxTextInformation_Name; //"Valgte symbol i tekstrepræsentation";
            this.openFileDialogFilterName = LocRes.OpenDialog_FilterName; 
        }


        /// <summary>
        /// To be called if this form is used as a part of the MusicXmlReader application or similar, which already has a Model 
        /// </summary>
        /// <param name="creatingForm">An interface to the Form (belonging to the consuming application) which called this Create-method</param>
        /// <param name="model">The MusicXmlReader.Model class (used by the consuming application) to use.</param>
        /// <returns></returns>
        public static  MusicBrailleReaderMainForm Create(IMusicBrailleReaderClient creatingForm, Model model)
        {
            MusicBrailleReaderMainForm result = new MusicBrailleReaderMainForm(creatingForm,model);
            return result;
        }


        /// <summary>
        /// Let the owning application emulate OnOpenClick()
        /// </summary>
        public void  OpenLatestDirectory()
        {            
            // This will cause JAWS to speak the name text from the menuItem as the Name of the FileOpen dialog.
            string menuText = LocRes.ToolStripMenuItem_Files_OpenUsingNOTA;
            OnOpen(menuText.Replace("&", ""), DecoderOptions.RegionalOptionsEnum.Danish);
        }

        /// <summary>
        /// To be called from program.cs if this form is used as a stand-alone program and no MusicXmlReader.Model class is available
        /// </summary>
        /// <returns></returns>
        public static MusicBrailleReaderMainForm Create()
        {
            // Set up for logging by opening and initializing the (static) Logger class
            string applicationName = "IBOS Punktnodelæser";
            Logger.Open(applicationName+".Log"); // "IBOS Punktnodelæser.Log"
            Logger.ShowTimeStampInLog = false; // Use false to compare logfiles while ignoring timestampe.
            Logger.LogCF(""); // An empty line to catch the eye
            Logger.LogCF(string.Format(": Starting: Date={0}", System.DateTime.Now.ToLongDateString()));
            // Get and interpret commandline arguments
            string[] arguments = Environment.GetCommandLineArgs();
            string developerModeString = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.DeveloperMode);
            bool developerMode = ("yes" == developerModeString);        
            Logger.DeveloperMode = developerMode;
            Logger.LogArguments(arguments);
            // Instantiate a Model instance containing lots of common code, originally designed for the MusicXmlReader
            Model model = Model.Create(null, null, applicationName, null);
            // Instantiate a dummy stand in for the MusicXmlReader application, only implementing a few interface functions
            DummyMusicBrailleReaderClient dummyMusicBrailleReaderClient = DummyMusicBrailleReaderClient.Create(applicationName, model);
            // Create the Form and return it
            MusicBrailleReaderMainForm result = new MusicBrailleReaderMainForm(dummyMusicBrailleReaderClient, model);
            return result;
        }

        /// <summary>
        /// Prevent construction
        /// </summary>
        private MusicBrailleReaderMainForm()
        { }


        private MusicBrailleReaderMainForm(IMusicBrailleReaderClient creatingForm, Model model)
        {
            this.model = model;
            this.creatingForm = creatingForm;
            executingAssemblyFullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingAssemblyShortName = System.IO.Path.GetFileNameWithoutExtension(executingAssemblyFullPath);
            InitializeComponent();
            LocalizeComponent();  // Overwrite all items exept in MenuStrip with localized texts
            this.openTestFileToolStripMenuItem.Text = string.Format("Decode '{0}'", currentTestFileName);
            HideDeveloperItems(Logger.DeveloperMode);

            if (!Logger.ExportToMusicXml)
            {
                Hide(exporterSomMusicXmlToolStripMenuItem); // This very important feature is still not in a functional state!!!
            }

            Application.ApplicationExit += Application_ApplicationExit; // Add an exit-handler to be sure all processes will be shut down on application exit !
        
            LocalizeMenuStrip(); // Overwrite all items in MenuStrip with localized texts
            regressionTest = RegressionTest.Create(this as IRegressionTestClient);
#if false
            musicBrailleEditor = MusicBrailleEditor.Create(model, textBoxBraille, listBoxOffsets,textBoxEditResultAsDecodedText,textBoxDebugInfo);
#endif

            // Create a handler for the main listbox, which lists Music Braille information ordered after Offset
            listboxOffsetsHandler = ListBoxOffsetsHandler.Create(model, this.listBoxOffsets,developerMode);

            // Create a handler for the user settings, in this case modelled as a treeview.
            userSettingsHandler = MusicBrailleReaderUserSettingsHandler.Create(userSettingsTreeView, model, listboxOffsetsHandler);
            userSettingsHandler.Init(); // Builds up the fixed part of the treeview
            userSettingsHandler.Reset();

            userSettingsHandler.LoadLevel0And1Nodes(this.model.UserSettings);
            userSettingsHandler.ExpandAllNodes();

            this.userSettingsTreeView.Enter += new System.EventHandler(TreeViewEnter); // Save relevant usersettings at entry
            this.userSettingsTreeView.Leave += new System.EventHandler(TreeViewLeave); // Allow for calling Decode() again if usersettings have changed.
         

        }

        private void Hide(ToolStripMenuItem item)
        {
            item.Enabled = false;
            item.Visible = false;
        }

        private void Hide(TextBox textBox)
        {
            textBox.Enabled = false;
            textBox.Visible = false;
        }
        
        private void HideDeveloperItems(bool developerMode)
        {
            if (developerMode) return; // All items remain visible

            // MenuItems under the "File" menuItem
            Hide(openUsingBrailleOrchProfileToolStripMenuItem);
            Hide(openTestFileToolStripMenuItem);         

            // The whole "Tools" menuItem
            Hide(toolsToolStripMenuItem);

            // The following textboxes are found in the MainForm and could possibly be used here for similar purposes. Right now they are not used:
            Hide(textBox3);
            Hide(textBoxText);
            Hide(textBoxStatusInformation);
        }


        bool musicAsBrailleCheckedAtEntry;
        bool musicAsSoundCheckedAtEntry;
        bool spaceNumberCheckedAtEntry;

        private void TreeViewEnter(object sender, EventArgs e)
        {
            musicAsBrailleCheckedAtEntry = userSettingsHandler.MusicAsBraille.Checked;
            musicAsSoundCheckedAtEntry = userSettingsHandler.MusicAsSound.Checked;
            spaceNumberCheckedAtEntry = userSettingsHandler.SpaceNumber.Checked;
        }

        private bool ListBoxReloadNeeded()
        {
            if (userSettingsHandler.MusicAsBraille.Checked != musicAsBrailleCheckedAtEntry) return true;
            if (userSettingsHandler.SpaceNumber.Checked != spaceNumberCheckedAtEntry) return true;
            return false;
        }

        public void TreeViewLeave(object sender, EventArgs e)
        {
            // When we arrive here we can not be sure that a file has been loaded. In that case we just ignore.
            if (null == fullFileName) return; // No file has been loaded yet.
            if (!File.Exists(fullFileName)) return; //
            if (!ListBoxReloadNeeded()) return; // If nothing has changed, that would change the Listbox we ignore 
            listboxOffsetsHandler.SavePosition();
            listBoxOffsets.Items.Clear();        
            GetInterpretation(latestRegionalOptions, UserWarningOptions.hide); // Use the latest regional options and do not show user warnings 
            listboxOffsetsHandler.RestorePosition();
        }



#region IREgressionTEstClient
        private delegate void SafeCallDelegate(string text);
        public void OnNewLine(String line)
        {
            // Handle cross-thread problem
            if (listBoxOffsets.InvokeRequired)
            {
                var d = new SafeCallDelegate(OnNewLine);
                listBoxOffsets.Invoke(d, new object[] { line });
            }
            else
            {
                Logger.LogCF(string.Format(": {0}", line));
                listBoxOffsets.Items.Add(line);
            }       
        }

        /// <summary>
        /// Method to call when the operation has terminated
        /// </summary>
        /// <param name="ok"></param>
        public void OnTermination(bool ok, string referenceDir)
        {
            Logger.LogCF(string.Format(": {0} {1}",ok,referenceDir));
            if (!Directory.Exists(referenceDir)) return;
            Utilities.RunExeWithDirArgument(Utilities.ExplorerExe, referenceDir);
        }

        public List<MusicXmlReaderModel.DecoderItem> InterpretBrailleMusicFile(string s, BrailleFileHandler.FileEncoding fileEncoding, out XmlDocument musicXmlDocument, DecoderOptions decoderOptions)
        {
            return model.DecoderHandler.InterpretBrailleMusicFile(s, fileEncoding, out musicXmlDocument, decoderOptions);
        }
#endregion

#region IDecoderUiClient
        public void ShowMessageBox(string caption, List<string> messageLines)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string messageLine in messageLines)
            {
                sb.Append(messageLine + "\r\n");
            } 
            MessageBox.Show(sb.ToString(), caption);
        }
#endregion


        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            model.DecoderHandler.OnExit();
            model.OnApplicationExit(); // Let the Model clean up its resources etc 
        }

        private string GetFileOpenInitialDirectory(bool useRecentFile)
        {
            string musicBrailleDirectory = creatingForm.GetLatestBrailleMusicPath();
            if (useRecentFile  && (musicBrailleDirectory != null) && (Directory.Exists(musicBrailleDirectory)))
            {
                // If a preferred Braille Music directory is already set up, use it
                // The  path can be set by either
                //  1) A previous reading of a MusicBraille file by "File->Open" in this form
                //  2) A previous writing of a MusicBraille file by "File->Export Braille Music to file" by the creating form (In this case the MusicXmlReader MainForm) 
                return musicBrailleDirectory;
            }
            // Otherways use the default directory used by the creating form (In this case the MusicXmlReader MainForm.)
            return creatingForm.GetMyMusicXmlDirectory();

            // Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);     
        }

        private bool SelectAndOpenFile(bool useDefaultSettings, bool useRecentFile,string dialogTitle)
        {
            openFileDialog.Reset(); // Prevent survival of strange settings from latest usage of this reused OpenFileDialog
            openFileDialog.Title = dialogTitle; 
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.brl;*.brf;*.txt;*.pef", openFileDialogFilterName); // "Punktnoder" or "Braille Music"
            openFileDialog.InitialDirectory = GetFileOpenInitialDirectory(useRecentFile);
            //openFileDialog.FileName = GetFileOpenInitialFileName(useRecentFile);

            SendKeys.Send("{HOME}"); // HACK Will show the full filename from the beginning:  https://stackoverflow.com/questions/24525606/openfiledialog-cuts-off-pre-populated-file-name

            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            DialogResult dialogResult = openFileDialog.ShowDialog();

            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.
            switch (dialogResult)
            {
                case DialogResult.OK: break;
                default:
                    Logger.LogCF(string.Format(": User cancelled FileOpenDialog with dialogResult={0}", dialogResult.ToString()));
                    return false;
            }


            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                Logger.LogCF(string.Format(": User entered filename, which was null or empty"));
                return false;
            }

            fullFileName = openFileDialog.FileName;
            string shortFileName = System.IO.Path.GetFileName(fullFileName);
            string extension = System.IO.Path.GetExtension(fullFileName);  

            switch (extension.ToLower()) // Accept extensions such as MusicXml or XML, as does MuseScore
            {
                case ".brf": fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII_Ex; break; // .brf files downloaded from BrailleOrg
                case ".brl": fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252; break; // .brl files received from Lars Petersen
                case ".txt": fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252; break; // .txt files from NOTA received via Susanne Nolsøe
                case ".pef": fileEncoding = BrailleFileHandler.FileEncoding.PEF; break; // .pef files, either generated by IBOS MusicXmlReader or received from NOTA
                default: // Report unsupported fileformat
                    string message = string.Format("{0} '{1}'", "Unsupported file format", shortFileName);
                    ;
                    MessageBox.Show(message, "applicationName", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
        
            }
            this.Text = string.Format("{0} - {1}", shortFileName, this.applicationName); // Inspired by Microsoft standard way of showing this.
            creatingForm.SetLatestBrailleMusicPath(Path.GetDirectoryName(fullFileName)); // Remember the path for next time using the User preferences system in the creating form !
            return true;
        }

        private DecoderOptions.RegionalOptionsEnum latestRegionalOptions = DecoderOptions.RegionalOptionsEnum.Unknown;  

        /// <summary>
        /// Calls the Model to create an interpretation (in the form of a list of DecoderItems) of the currently loaded Music Braille file
        /// This may happen as a consequence of that:
        /// 1) The user has opened a new Music Braille input file.
        /// 2) The uses has changed a UserSetting. 
        /// </summary>
        /// <param name="regionalOptions"></param>
        /// <param name="showWarnings"></param>
        /// <returns></returns>
        private List<DecoderItem> GetInterpretation(DecoderOptions.RegionalOptionsEnum regionalOptions, UserWarningOptions options)
        {
            latestRegionalOptions = regionalOptions;
            musicXmlDocument = null;
            List<DecoderItem> result;

            // Exclude some substrings from the string representation
            DecoderOptions.FormatOptionsEnum excludedDecoderOptiones =
                  DecoderOptions.FormatOptionsEnum.indexAndLength
                | DecoderOptions.FormatOptionsEnum.rawValues
                | DecoderOptions.FormatOptionsEnum.apostrophesInValue
                | DecoderOptions.FormatOptionsEnum.lineNumber
                | DecoderOptions.FormatOptionsEnum.rawBrailleLines
                | DecoderOptions.FormatOptionsEnum.extraString
                | DecoderOptions.FormatOptionsEnum.pageNumber
                | DecoderOptions.FormatOptionsEnum.value
                | DecoderOptions.FormatOptionsEnum.none; // No value, only for ease of adding and removing lines

            userSettingsHandler.Update(ref excludedDecoderOptiones);

            // Exclude the names of some categories (The information is fully contained in friendlyString)
            DecoderOptions.CategoryEnum hiddenCategoryNames =
                  DecoderOptions.CategoryEnum.Note
                | DecoderOptions.CategoryEnum.Articulation
                | DecoderOptions.CategoryEnum.Hand ; // No "Note" or "Hand" in front of the note

            DecoderOptions.CategoryEnum hiddenCategories =
                  DecoderOptions.CategoryEnum.Character // Exclude inputDescriptions containing single characters. Handled by AccumulatedText
                | DecoderOptions.CategoryEnum.Digit;  // Exclude inputDescriptions containing single digits.  Handled by AccumulatedText

            DecoderOptions decoderOptions = DecoderOptions.Create(regionalOptions,this.developerMode);
            decoderOptions.ExcludeSubStrings(excludedDecoderOptiones); // Exclude some substrings from the string representation
            decoderOptions.ExcludeCategoryNames(hiddenCategoryNames); // Exclude the names of some categories (The information is fully contained in friendlyString)
            decoderOptions.ExcludeCategories(hiddenCategories); // Exclude inputDescriptions cintaining single characters
            if (!this.developerMode)
            {
                decoderOptions.ExcludeSubStrings(DecoderOptions.FormatOptionsEnum.xmlRepresentation);
            }

            result = model.DecoderHandler.InterpretBrailleMusicFile(fullFileName, fileEncoding, out musicXmlDocument, decoderOptions); // This is where everything happens !!!

            // ShowUserWarnings(options); // Show the warnings through a MessageBox Has been moved to ExportToFileAsMusicXml) because it is only relevant for generation of MusicXml 

            ShowAsMusicXml(ShowAsMusicXmlOptions.hide); // Show the MusicXml genereted- Use "hide" to speed up

            decoderOutputFileHandler = DecoderOutputFileHandler.Create(fullFileName);   

            // Write the decoded output as text to the listbox
            listBoxOffsets.ClearSelected();
            foreach (DecoderItem decoderItem in result)
            {
                listBoxOffsets.Items.Add(decoderItem);
            }
            if (listBoxOffsets.Items.Count != 0)
            {
                listBoxOffsets.SelectedIndex = 0; // Select the first item from start to avoid crashes. 
            }

            return result;
        }

        private bool OnNoMusicBrailleFileLoaded()
        {
            MessageBox.Show("Ingen punktnodefil er åbnet.");
            return false;
        }


        /// <summary>
        /// Exports the interpretation of the current MusicBraille inputfile as an MuxicXml file
        /// </summary>
        /// <param name="musicXmlDocument"></param>
        private bool ExportToFileAsMusicXml(XmlDocument musicXmlDocument)
        {
            // This handler may be called before the data structiures have been established so we need to check
            if (null == decoderOutputFileHandler) return OnNoMusicBrailleFileLoaded();
            if (null == musicXmlDocument) OnNoMusicBrailleFileLoaded();

            DialogResult dialogResult = ShowUserWarnings(UserWarningOptions.details, UserInfoFlagsEnum.AllFlags); // Show All  warnings in a detailled format
           // DialogResult dialogResult = ShowUserWarnings(UserWarningOptions.details, UserInfoEnum.All & ~UserInfoEnum.FailedToFixDuration); // Show some  warnings in a detailled format

            if (DialogResult.OK != dialogResult)
            {
                Logger.LogCF1(": Export cancelled by user");
                return false;
            }
            bool showXmlOnConsole = false;

            string fileName = decoderOutputFileHandler.FullMusicXmlFileName;

            regressionTest.MusicXmlRegressionTest(fileName, musicXmlDocument);

            // Save the MusicXml file
            if (showXmlOnConsole) musicXmlDocument.Save(Console.Out); // To the console. Disable to speed up debugging !
            musicXmlDocument.Save(fileName); // To a file

            latestMusicXmlFileGenerated = fileName;

            if (developerMode)
            {
                // Open Explorer in the default output directory for the application, that is within the appData area:
                Utilities.RunExeWithDirArgument(Utilities.ExplorerExe, decoderOutputFileHandler.OutputDirectory);
                return true;
            }
            else
            {
                return UserSave(fileName);
            }          
        }


        /// <summary>
        /// Let the user decide where to save the MusicXml file generated and wether or not to open an Explorer at the location. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool UserSave(string fileName)
        {
            // Let the User select directory and filename, but suggest decent default values
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Path.GetFileName(fileName);
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (DialogResult.OK != dialogResult) return false;
            string fileNameSelectedByUser = saveFileDialog.FileName;
            bool ok = false;
            try
            {
                musicXmlDocument.Save(fileNameSelectedByUser);
                ok = true;
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            if (!ok)
            {
                MessageBox.Show(string.Format("Kunne ikke gemme filen\r\n'{0}'", fileNameSelectedByUser, this.applicationName));
                return false;
            }

            string message =
                  "Punktnodefilen blev gemt som en MusicXml fil\r\n"
                + "Tryk 'Ja' for at gå til MusicXml filen i Windows Stifinder\r\n"
                + "Tryk 'Nej' for at fortsætte";
            string caption = applicationName;
            DialogResult messageBoxResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);

            // Open Explorer in the output directory to let the user inspect the result with for instance MuseScore or IBOS MusicXmlReader
            if (DialogResult.Yes == messageBoxResult)
            {
                Utilities.RunExeWithDirArgument(Utilities.ExplorerExe, Path.GetDirectoryName(fileNameSelectedByUser));
            }
            return true;
        }




        /// <summary>
        /// Exports the interpretation of the currnt MusicBraille inputfile as a simple text file.
        /// </summary>
        /// <param name="decoderItems"></param>
        private bool ExportToFileAsText(List<DecoderItem> decoderItems)
        {
            // This handler may be called before the data structiures have been established so we need to check
            if (null == currentInterpretation) return OnNoMusicBrailleFileLoaded();
            if (null == decoderOutputFileHandler) return OnNoMusicBrailleFileLoaded();

            List<string> strings = new List<string>();
            foreach (DecoderItem decoderItem in decoderItems)
            {
                string s = Logger.DeveloperMode ?  decoderItem.XmlToString() : "" ; // Only add the XML interpretation (which shows the attempts to fix ambiguities) in Developer mode  
                strings.Add(decoderItem.ToString() + s);
            }

            if (Logger.DeveloperMode)
            {
                regressionTest.DecoderOutputRegressionTest(strings, decoderOutputFileHandler.FullOutputFileName);
            }

            // Write the decoded output as a text interpretation to a file
            decoderOutputFileHandler.SaveInterpretation(strings, decoderOutputFileHandler.FullOutputFileName);

            string fileName = decoderOutputFileHandler.FullMusicXmlFileName;
      
            // Open Explorer in the output directory.
            Utilities.RunExeWithDirArgument("Explorer", decoderOutputFileHandler.OutputDirectory);
            return true;
        }


        private enum UserWarningOptions { hide, overview, details};



        private DialogResult ShowUserWarnings(UserWarningOptions options, UserInfoFlagsEnum mask)
        { 
            if (options == UserWarningOptions.hide) return DialogResult.OK;
            List<string> userWarnings = model.DecoderHandler.GetLocalUserWarnings(mask);
            if (0 == userWarnings.Count) return DialogResult.OK;

            // Build a localized errorMessage
            string fileName = System.IO.Path.GetFileName(fullFileName);
            string text1 = (1 == userWarnings.Count) ? LocRes.Message_Warning : LocRes.Message_Warnings;
            string text2 = LocRes.Message_FoundInBrailleMusicFile;
            string errorMessage = string.Format("{0} {1} {2} '{3}'", userWarnings.Count, text1, text2, fileName);

            options = UserWarningOptions.overview; // Until we get a better localization of the error messages we stick to the overview. See Logfile for details
            if (options == UserWarningOptions.details)
            {
                // Use a costum Messagebox where wrapping can be controlled and where JAWS is better supported !       
                UserMessageListForm userMessageListForm = new UserMessageListForm(errorMessage, userWarnings);
                return userMessageListForm.ShowDialog();  // Use ShowDialog() instead of Show() to wait for userMessageListForm to receive input
            }
            else
            {
                // Use a standard MessageBox
                string caption = string.Format("{0} {1} {2}", userWarnings.Count, text1, text2);
                return MessageBox.Show(caption,fileName,MessageBoxButtons.OKCancel);             
            }
        }


        private enum ShowAsMusicXmlOptions { hide, onConsole }
        private void  ShowAsMusicXml(ShowAsMusicXmlOptions options)
        {
            switch (options)
            {
                case ShowAsMusicXmlOptions.onConsole: musicXmlDocument.Save(Console.Out); break;
                default: break;
            }
        }


        private void ClearUI()
        {
            listBoxOffsets.Items.Clear();
            listBoxOffsets.Refresh();
        }

        /// <summary>
        /// This will cause JAWS to speak the name text from the menuItem as the Name of the FileOpen dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private string GetOpenDialogName(object sender)
        {
            string senderName = sender.ToString();
            string result = senderName.Replace("&", "");
            return result;
        }

#if false
        private void usingNOTAProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SelectAndOpenFile(false, false, GetOpenDialogName(sender))) return;
            ClearUI();
            Decode(DecoderOptions.RegionalOptionsEnum.Danish);
        }

        private void usingBrailleOrchProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SelectAndOpenFile(false, false, GetOpenDialogName(sender))) return;
            ClearUI();
            Decode(DecoderOptions.RegionalOptionsEnum.English);
        }

        private void usingAutoselectedProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SelectAndOpenFile(false, false, GetOpenDialogName(sender))) return;
            ClearUI();
            Decode(DecoderOptions.RegionalOptionsEnum.AutoSelect);
        }
#endif
        private void listBoxDecodedAsText_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newIndex = listBoxOffsets.SelectedIndex;
            DecoderItem decoderItem = listBoxOffsets.Items[newIndex] as DecoderItem;
            if (null != decoderItem)
            {
                if (userSettingsHandler.MusicAsSound.Checked)
                {
                    model.DecoderHandler.Play(decoderItem.XmlRepresentation);
                }
            }
            OnSelectedDecodedLineChanged();
        }

        public void OnSelectedDecodedLineChanged()
        {
            textBoxBraille.Clear();
            textBoxBraille.Text = "";
            DecoderItem decoderItem = (listBoxOffsets.SelectedItem as DecoderItem);
            if ((null != decoderItem) & (null != decoderItem.TokenString))
            {
                textBoxBraille.Text = decoderItem.TokenString;
            }
            if ((string.IsNullOrEmpty(textBoxBraille.Text)))
            {
                // If the currently represented text is empty we use all Braille characters in the decoderItem instead.
                textBoxBraille.Text = GetAllBrailleCharacters(decoderItem.ToString());
            }
            else
            {
                // Otherwise we romove any possible Non-Braille characters (This will remove any LF CF FF and other control characters)
                textBoxBraille.Text = GetAllBrailleCharacters(textBoxBraille.Text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string GetAllBrailleCharacters(string s)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in s)
            {
                if (c <  0x2800 ) continue;
                if (c >= 0x2900 ) continue;
                result.Append(c);
            }
            return result.ToString();
        }

        
        private void tactileMusicXmlReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            // While running in the VS debugger  we attempt to look up an application in the same solution
                  
            string thisExePath = Utilities.GetExecutingDirectory();
            string thisDirName = "MusicBrailleReader";
            string thatDirName = "MusicXmlReader";
            string thatExePath = "";

            if (!thisExePath.Contains(thisDirName))
            {
                string s = string.Format("Error: Path {0}\r\ndoes not contain '{1}' ", thisExePath, thisDirName);
                MessageBox.Show(s);
                Logger.LogCF(": "+s);
                return;
            }

            thatExePath = thisExePath.Replace(thisDirName, thatDirName);
            string thatExeName = "Tactile MusicXmlReader.exe";
            string thatFullExePath = Path.Combine(thatExePath, thatExeName);

            if (!File.Exists(thatFullExePath))
            {
                string s = string.Format("Error: {0}\r\ndoes not exist", thatFullExePath);
                MessageBox.Show(s);
                Logger.LogCF(": "+s);
                return;
            }

            Run(thatFullExePath, latestMusicXmlFileGenerated);
        }


        const string MuseScoreExe = @"C:\Program Files\MuseScore 3\bin\MuseScore3.exe"; // As in ..MusicXmlReader\app.config
        private void museScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run(MuseScoreExe, latestMusicXmlFileGenerated);
        }

        private void Run(string exePath, string musicXmlPath)
        {
            // First handle unusual illegal values of musicXmlPath
            string shortName = Path.GetFileName(exePath);
            string commonWarning = string.Format("\r\nStarting {0} with no MusicXml file specified.",shortName);
            if (string.IsNullOrEmpty(latestMusicXmlFileGenerated))
            {
                // This happens if no MusicXml file has been genereated yet during the current session of the application
                string s = string.Format("Warning: No MusicXml file generated yet.{0}",commonWarning);
                MessageBox.Show(s);
                Logger.LogCF(": " + s);
                musicXmlPath = ""; // Avoid trouble when we run the application without a file specified
            }
            else
            {
                if (!File.Exists(latestMusicXmlFileGenerated))
                {
                    string s = string.Format("Warning: File does not exist:\r\n{0}{1}", latestMusicXmlFileGenerated,commonWarning);
                    MessageBox.Show(s);
                    Logger.LogCF(": " + s);
                    // Warn only, no need to return
                    musicXmlPath = ""; //  Avoid trouble when we run the application without a file specified
                }
            }

            string quotedMusicXmlFilePath = Utilities.Quote(musicXmlPath);
            if (!Utilities.RunExeWithArgument(exePath, quotedMusicXmlFilePath))
            {
                string s = string.Format("Error: Failed to run\r\n{0}", exePath, quotedMusicXmlFilePath);
                MessageBox.Show(s);
                Logger.LogCF(": " + s);
            }

        }

        private void logfileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Utilities.RunExeWithDirArgument(Utilities.ExplorerExe, Logger.LogFileDirectory))
            {
                string s = string.Format("Error: Failed to run\r\n{0}('{1}')", Utilities.ExplorerExe, Logger.LogFileDirectory);
                MessageBox.Show(s);
                Logger.LogCF(": " + s);
            }
        }

        const string IbosMusicXmlReaderExe = @"C:\Program Files (x86)\IBOS MusicXmlReader\IBOS MusicXmlReader.exe"; // Run the official, installed version
        private void iBOSMusicXmlReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run(IbosMusicXmlReaderExe, latestMusicXmlFileGenerated);
        }

        Thread testThread;
        static void DoTest(object o)
        {
           (o as RegressionTest).ExecuteAll();
        }

        /// <summary>
        /// Starts the test and returns immediately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regressionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBoxOffsets.Items.Clear();
            testThread = new Thread(new ParameterizedThreadStart(DoTest));
            testThread.Start(regressionTest);      
        }


        /// <summary>
        /// Open an explicitly named Music Braille. Only for test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openTestFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearUI();
            string baseDir = GetFileOpenInitialDirectory(false);
#if false
            string fullDir = Path.Combine(baseDir, "BrailleOrch");
            fullFileName = Path.Combine(fullDir, "Bor001.Beethoven - Für Elise.brf");
            fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII_Ex;
            currentInterpretation = Decode(DecoderOptions.RegionalOptionsEnum.English);
            ExportToFiles(musicXmlDocument);
#else
            string fullDir = Path.Combine(baseDir,currentTestDirectory);
            fullFileName = Path.Combine(fullDir, currentTestFileName); 
            fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252;
            currentInterpretation = GetInterpretation(DecoderOptions.RegionalOptionsEnum.Danish,UserWarningOptions.details);
#endif
        }

        private void logFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserWarnings.DumpGlobalUserWarnings(); // Include the latest warnings in the logfile before opening it
            string notePadExe = "Notepad.exe";
            if (!Utilities.RunExeWithFileArgument(notePadExe, Logger.LogFileFullName))
            {
                string s = string.Format("Error: Failed to run\r\n{0}('{1}')", notePadExe, Logger.LogFileFullName);
                MessageBox.Show(s);
                Logger.LogCF(": " + s);
            }
        }

        private void regressionReferenceLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
          Logger.LogCF("");
            // Do not place the output files under "\Temp" because such files will be deleted by the system, and we want to keep them for later reference.
            string appDataLocalDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string s = Path.Combine(appDataLocalDir, "MusicBrailleToMusicXmlCmd");
            if (!Directory.Exists(s)) Directory.CreateDirectory(s);
            string referencePath = Path.Combine(s, "Reference");
            StartExplorer(referencePath);
        }

        private void copyRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameTool renameTool = RenameTool.Create();
            string sourceDirectory = @"C:\Users\Jens\Dropbox\Root\MusicXml sample file archive\hsbm_12_som_music_xml";
            string appDataLocalDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string destDirectory = Path.Combine(appDataLocalDir,"Højskolesangbogen");
            DialogResult result = MessageBox.Show("Do you really want to¨copy and rename files from Højskolesangbogen ?", "", MessageBoxButtons.YesNo);
            if (DialogResult.Yes != result) return;
            // string destDirectory = Path.Combine(sourceDirectory, "FriendlyNames");
            renameTool.CopyRenameFiles(sourceDirectory,destDirectory);
            StartExplorer(destDirectory);
        }

        private void StartExplorer(string dir)
        {
            if (!Utilities.RunExeWithDirArgument(Utilities.ExplorerExe, dir))
            {
                string message = string.Format("Error: Failed to run\r\n{0}('{1}')", Utilities.ExplorerExe, dir);
                MessageBox.Show(message);
                Logger.LogCF1(": " + message);
            }
        }

        private void transscribeHøjskolesangbogenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string appDataLocalDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string sourceDirectory = Path.Combine(appDataLocalDir, "Højskolesangbogen");
            DialogResult result =   MessageBox.Show("Do you really want to transscribe Højskolesangbogen ?", "", MessageBoxButtons.YesNo);
            if (DialogResult.Yes != result) return;
            BatchTransscriber batchTransscriber = BatchTransscriber.Create(model);
            batchTransscriber.Transscribe(sourceDirectory);
            StartExplorer(sourceDirectory);
        }

        bool useRecentFile = true;

        // At this point of time (immediately after opening and interpreting the BrailleMusic file) all error information has been collected, 
        // but we do not want to reveal the error information only related to the MusicXml generation now. So we use the following mask:
        UserInfoFlagsEnum mask = UserInfoFlagsEnum.AllFlags & ~(UserInfoFlagsEnum.GenerationFailedToFixDuration | UserInfoFlagsEnum.GenerationUnSupportedVoiceList);

        private void openUsingNOTAProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnOpenClick(sender, DecoderOptions.RegionalOptionsEnum.Danish);
        }

        private void openUsingBrailleOrchProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnOpenClick(sender, DecoderOptions.RegionalOptionsEnum.English);
        }

        private void OnOpenClick(object sender, DecoderOptions.RegionalOptionsEnum regionalOptions)
        {
            OnOpen(GetOpenDialogName(sender), regionalOptions);
        }

        private void OnOpen(string dialogTitle, DecoderOptions.RegionalOptionsEnum regionalOptions)
        {
            try
            {
                if (!SelectAndOpenFile(false, useRecentFile, dialogTitle)) return;
                ClearUI();
                currentInterpretation = GetInterpretation(regionalOptions, UserWarningOptions.details);
                // throw new Exception("Only for debugging!"); // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                ShowUserWarnings(UserWarningOptions.details, mask); // Show some  warnings in a detailled format        
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                string message = ResourcesForMusicBrailleReaderMainForm.Message_AnUnexpectedErrorOccurred; 
                ModelBaseMessageBox.Show(message,ModelBaseMessageBoxButtons.OK,ModelBaseMessageBoxIcon.Error); // Use the static UserMssageBox class implemented in MusicXmlReaderBase class to demonstrate how to use that class from anywhere !
            }
        }



#if false
        #region TextBoxRawBraille6EventHandlers        

        private void textBoxRawBraille6_KeyDown(object sender, KeyEventArgs e)
        {
            musicBrailleEditor.OnTextBoxRawBraille6_KeyDown(sender, e); // Simple pass on
        }

        private void textBoxRawBraille6_KeyPress(object sender, KeyPressEventArgs e)
        {
            musicBrailleEditor.OnTextBoxRawBraille6_KeyPress(sender, e); // Simple pass on
        }

        private void textBoxRawBraille6_KeyUp(object sender, KeyEventArgs e)
        {
            musicBrailleEditor.OnTextBoxRawBraille6_KeyUp(sender,e); // Simple pass on
        }

        #endregion

        private void textBoxRawBraille6_TextChanged(object sender, EventArgs e)
        {
            musicBrailleEditor.OnTextBoxRawBraille6_TextChanged(sender, e);
        }
#endif

        private void exporterSomMusicXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToFileAsMusicXml(musicXmlDocument);
        }

        private void exporterSomTextToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            ExportToFileAsText(currentInterpretation);
        }

        private const string extensionJCF = ".JCF";

        private void OnJAWSDirectoryNotFound()
        {
            MessageBox.Show("Mappen for JAWS applikations-specifik konfigurationsfil findes ikke");
            string s = string.Format(": Directory for JAWS application-specific configurationfile for {0} does not exist", executingAssemblyShortName);
            Logger.LogCF(s);
        }

        private void jAWSSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This mechanism should be replaced with the mechanism in MusicXmlREader when the programs are merged.
            string expectedDirectory = model.ExternalToolsHandler.GetJawsDirectoryName(executingAssemblyShortName);
            if (string.IsNullOrEmpty(expectedDirectory))
            {
                OnJAWSDirectoryNotFound();
                return;
            }
            string configurationFileName = executingAssemblyShortName + extensionJCF;
            string fullFileName = Path.Combine(expectedDirectory, configurationFileName);
            if (!File.Exists(fullFileName))
            {
                string m = string.Format("JAWS applikations-specifik konfigurationsfil:\r\n'{0}'\r\n findes ikke", configurationFileName);
                MessageBox.Show(m);
                string s = string.Format(": JAWS application-specific configurationfile {0} is not found in {1}", configurationFileName, expectedDirectory);
                Logger.LogCF(s);
                return;
            }
            // At this point we can show the file in NotePad:
            Utilities.RunExeWithFileArgument(Utilities.NotepadExe, fullFileName);
        }

        private void jAWSSettingsDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This mechanism should be replaced with the mechanism in MusicXmlREader when the programs are merged.
            string expectedDirectory = model.ExternalToolsHandler.GetJawsDirectoryName(executingAssemblyShortName);
            if (string.IsNullOrEmpty(expectedDirectory))
            {
                OnJAWSDirectoryNotFound();
                return;
            }
            Utilities.RunExeWithDirArgument(Utilities.ExplorerExe, expectedDirectory);
        }

        private void jAWSSettingsUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string destinatinDirectory = model.ExternalToolsHandler.GetJawsDirectoryName(executingAssemblyShortName);
            if (string.IsNullOrEmpty(destinatinDirectory))
            {
                OnJAWSDirectoryNotFound();
                return;
            }

            bool ok = false;
            try
            {
                string configurationFileName = executingAssemblyShortName + extensionJCF;
                string configurationFileFullName = Path.Combine(destinatinDirectory, configurationFileName);
                if (File.Exists(configurationFileFullName))
                {
                    // Save a backup of the existing file
                    string savedFileShortName = executingAssemblyShortName + "." + DateTime.Now.Ticks.ToString() + extensionJCF;
                    string savedFileFullName = Path.Combine(destinatinDirectory, savedFileShortName);
                    File.Copy(configurationFileFullName, savedFileFullName);
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[OutputModes]");
                sb.AppendLine("POSITION=0|2|0|Positionsinformation");
                sb.AppendLine("CONTROL_TYPE = 0 | 2 | 2 | Kontroltype");
                sb.AppendLine("[FocusXT14]");
                sb.AppendLine("StructuredModeInclusionFlags=8189");
                string s = sb.ToString();
                File.WriteAllText(configurationFileFullName, s);
                ok = true;
            }
            catch (Exception ex)
            {
                Logger.LogCFE(ex);
            }
            MessageBox.Show(string.Format("Opdatering af JAWS konfigurationsfil {0}",ok ? "lykkedes" : "mislykkedes")); 
            Logger.LogCF(string.Format("Update of JAWS configuration file {0}", ok ? "succeeded" : "failed"));            
        }

        private void MusicBrailleReaderMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.LogCF("");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == creatingForm)
            {
                Logger.LogCF(": Closing application.");
                Application.Exit();
            }
            else
            {
                // We were started from the MainForm
                Logger.LogCF(": Closing form.");
                this.Close();
            } 
        }
    }
}
