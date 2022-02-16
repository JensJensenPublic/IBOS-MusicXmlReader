using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MusicXmlReaderModel;
using System.Collections.Generic;
using RawPrinterHelper;
using System.Xml;

namespace MusicXmlReader
{

    /// <summary>
    /// The only purpose of this .cs file is to textually extract some code from the enormous ManiForm.cs
    /// Contains simple support functions related to the MenuStrip and accessing private member variables of MainForm
    /// </summary>
    public partial class MainForm
    {
        /// <summary>
        /// As default JAWS does not render the Shortcut key, only the access key
        /// By setting the AccessibleName property we can overwrite the default to anything we wish:
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        string GenerateAccessibleName(string text, Keys keys)
        {
            if (Keys.None == keys) return Utilities.RemoveAmpersant(text);
            return Utilities.RemoveAmpersant(text) + " " + UiUtilities.KeysToString(keys); // Say the control char before the other character.
        }

#warning ToDO: Use UiAccessibilityModel and call menuItemHandler.GenerateAccessibileName
        void GenerateAccessibleName(ref ToolStripMenuItem menuItem)
        {
            menuItem.AccessibleName = GenerateAccessibleName(menuItem.Text, menuItem.ShortcutKeys);
        }

        private string GetFileOpenInitialDirectory(bool useRecentFile)
        {
            // When running the initial user session we want to use the files in the <user>\<Documents>\<IBOS MusicXmlReader> directory
            // Where <Documents> and <MusicXmlReader> both represent localized strings 
            // Otherwise we want to use the directory most recently used by the current user
            if (useRecentFile) return userPreferencesHandler.GetExistingBaseDirectory(userPreferencesHandler.MusicXmlFile, myMusicXmlDirectory);
            return myMusicXmlDirectory;
        }


        private string GetFileOpenInitialFileName(bool useRecentFile)
        {
            // When running the initial user session we want to use the files in the <user>\<Documents>\<IBOS MusicXmlReader> directory
            // Where <Documents> and <MusicXmlReader> both represent localized strings 
            // Otherwise we want to use the file most recently used by the current user
            if (useRecentFile) return userPreferencesHandler.GetExistingFile(userPreferencesHandler.MusicXmlFile, "");
            return "";
        }




        private void SaveUserMusicXmlFileInfo()
        {
            try
            {
                //userPreferencesHandler.MusicXmlDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                userPreferencesHandler.MusicXmlFile = openFileDialog.FileName;
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
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
        private bool SelectAndOpenMusicXmlFile(bool useDefaultSettings, bool useRecentFile)
        {
            openFileDialog.Reset(); // Prevent survival of strange settings from latest usage of this reused OpenFileDialog
            openFileDialog.Title = ResourcesForUI.OpenFileDialog_Title;  // Just a neutral name "Open"
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.xml;*.musicxml;*.mxl", ResourcesForUI.OpenFileDialog_Filter); // Only present .xml files and .mxl files
                                                                                                                       //            openFileDialog.Filter = string.Format("{0}|*.xml|{0}|*.mxl", ResourcesForUI.OpenFileDialog_Filter,ResourcesForUI.OpenFileDialog_Filter_mxl); // Only present .xml files and .mxl files
            openFileDialog.InitialDirectory = GetFileOpenInitialDirectory(useRecentFile);
            openFileDialog.FileName = GetFileOpenInitialFileName(useRecentFile);

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
                return false; // Let the user press ESC without warning him
            }

            // model.Silence();   // Stop Screenreader talking about the OpenfileDialog, we just left !!

            // Save User settings for currently loaded file (if any) immediately before clearing the UI:

            model.SaveUserSettings(); // Save score-specific settings for the PREVIOUS file
            this.SaveUserMusicXmlFileInfo(); // Save the full path to the file just selected by user.

            // Clear all UI BEFORE starting the time consuming Load operation:
            ClearUI();

            // Start filling the UI with information about the NEW file
            //            textBoxNormalText.Focus(); // Move focus to the (empty) textBoxNormalText to prevent JAWS form talking too much !!

            string fullFileName = openFileDialog.FileName;
            string shortFileName = System.IO.Path.GetFileName(fullFileName);
            string extension = System.IO.Path.GetExtension(fullFileName);
            string xmlFileName = "";
            string message = string.Format("{0} '{1}'", ResourcesForUI.TextBox_Messages_Reading_File, shortFileName);
            WriteStatusInformation(message); // Still write messages to the Status line, but without without Focus on the StatusLine
            //textBoxScreenReader.Text = ResourcesForUI.TextBox_Messages_Reading_File; // As textBoxNormalText has Focus, this will be read by the ScreenReader !
            textBoxScreenReader.Text = shortFileName; // As textBoxNormalText has Focus, this will be read by the ScreenReader !
            textBoxScreenReader.Refresh();

            switch (extension.ToLower()) // Accept extensions such as MusicXml or XML, as does MuseScore
            {
                case ".mxl": // Attempt to convert from .mxl to .xml first
                    xmlFileName = model.MxlToXml(openFileDialog.FileName);
                    if (string.IsNullOrEmpty(xmlFileName))
                    {
                        message = string.Format("{0} '{1}' {2}", ResourcesForUI.TextBox_Messages_FailedToConvert_File, shortFileName, ResourcesForUI.TextBox_Messages_ToMusicXml);
                        WriteStatusInformation(message);
                        textBoxStatusInformation.Focus();
                        UiUtilities.Beep();
                        MessageBox.Show(message, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    break;
                case ".xml": // Continue
                case ".musicxml": // Continue
                    xmlFileName = fullFileName;
                    break;
                default: // Report unsupported fileformat
                    message = string.Format("{0} '{1}'", ResourcesForUI.TextBox_Messages_UnsupportedFileFormat, shortFileName);
                    WriteStatusInformation(message);
                    textBoxStatusInformation.Focus();
                    UiUtilities.Beep();
                    MessageBox.Show(message, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
            }
            return this.LoadMusicXmlFile(xmlFileName, shortFileName, useDefaultSettings);
        }

        
        /// <summary>
        /// Load a MusicXml file from disk and handle all UI initialisation
        /// </summary>
        /// <param name="xmlFileName">Full filename of the file to load from disk</param>
        /// <param name="shortFileName">Filename without path of the file to load</param>
        /// <param name="useDefaultSettings">If set the Model will attempt to reuse user settings from previous session</param>
        /// <returns>true iff success</returns>
        private bool LoadMusicXmlFile(string xmlFileName, string shortFileName, bool useDefaultSettings)
        {
            Logger.ClearStatistics();  // Clear statistics to be collected while loading, parsing and rendering the MusicXml file:
            Logger.CurrentMusicXmlPath = xmlFileName; // Allow for easy logging of the full file name from anywhere in the code

            // Now follows the time-consuming operation, where the Model loads and interpretes a new MusicXml file.
            if (!model.LoadMusicXmlFile(xmlFileName, useDefaultSettings)) // Load the selected .xml file into the Model and build all internal data structures.
            {
                // Simple error handling
                string message = string.Format("{0} '{1}'", ResourcesForUI.TextBox_Messages_FailedToRead_File, shortFileName); // Short filename for UI
                WriteStatusInformation(message);
                ShowWarning((int)ModelMessageEnum.FailedToReadMusicXmlFile, shortFileName, "");
                MessageBox.Show(message, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Log(string.Format("Failed to read {0}", openFileDialog.FileName)); // Full filename for UI
                Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() until now
                this.Text = applicationName; // Remove any exixting filename from the title bar
                return false;
            }

            listBoxTimesHandler.AutoReload = false; // While loading the listbox all changes are  made by user and must be ignored

            // The initial values of the user settings are determined by the model.
            // These settings must be reflected in the UI:

            userSettingsHandler.LoadLevel0And1Nodes(model.UserSettings);

            // Load the Checkboxes controlling the user settings per part
            userSettingsHandler.LoadParts(model.partList);
            // Load the Checkboxes controlled by a fixed number of settings statically defined in the Model.
            userSettingsHandler.LoadDetails(model.UserSettings);
            // Finally expand the tree
            userSettingsHandler.ExpandAllNodes();
            //this.userSettingsTreeView.ExpandAll();

            // Use the status line for meta information ontil overwritten by real status information
            WriteStatusInformation(UiUtilities.GetStatusFromMetaInformation(model));

            // Transfer from model to ListboxTimes
            // model.Silence();
            foreach (EventDescription eventDescription in model.EventDescriptionList.Events)
            {
#if false
                // NOTE: Removing empty lines has undesired consequences whwn the user removes all speech and MusicBraille output!
                //       So for the time being we prefer showing the empty linse !!
                string s = eventDescription.ToString();
                if (string.IsNullOrWhiteSpace(s))
                {
                    //string warning = string.Format("Empty string skipped in measure{0}", eventDescription.StatusInformation.CurrentMeasureElement.ToString());
                    //Logger.LogCF(string.Format(": {0}",warning));
                    // string asterixes = "*************************************************";
                    // listBoxTimes.Items.Add(string.Format("{0} {1} {0}",asterixes,warning));  // May be used for debugging to show the blank lines
                }
                else
                {
                    listBoxTimes.Items.Add(eventDescription);
                }
#else
                listBoxTimes.Items.Add(eventDescription);
#endif
            }

            Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() during parsing, interpreting and rendering the file
            this.Text = UiUtilities.GetTitleInfo(applicationName, model);
            model.SetUserTempo(100); // Play at 100% of tempo specified in MusicXml file
            return true;
        }


        //////////////





        /// <summary>
        /// Same as openMusicXmlFileToolStripMenuItem_Click(), but delete the UserSettings file before opening !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openMusicXmlFileUsingDefaultSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const bool deleteUserSettings = true;
            OpenMusicXmlFile(deleteUserSettings, false);
        }

        /// <summary>
        /// Same as openMusicXmlFileToolStripMenuItem_Click(), but opens in the directory for the most recently opened MusicXml file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openRecentMusicXmlFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const bool useRecentFile = true;
            OpenMusicXmlFile(false, useRecentFile);
        }

        /// <summary>
        /// The default method for letting the user select and open a MusicXml file
        /// The initial directory is always the base directory for the IBOS MusicXmlReader user files:
        /// "C:\Users\(user)\Documents\(IBOS MusicXmlReader)"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openMusicXmlFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMusicXmlFile(false, false);
        }

        private void OpenMusicXmlFile(bool useDefaultSettings, bool useRecentFile)
        {
            // model.Silence();

            try
            {
                // throw new Exception("Test");
                textBoxScreenReader.Text = "";
                textBoxScreenReader.Refresh();
                textBoxScreenReader.Show();
                textBoxScreenReader.Focus(); // Move focus to the (empty) textBoxScreenreader to prevent JAWS form talking too much !! 

                // Show a standard Select File dialog to allow the user to select and open a MusicXml file
                SelectAndOpenMusicXmlFile(useDefaultSettings, useRecentFile);

                if (listBoxTimes.Items.Count > 0)
                {
                    // Note: This is a HACK, which forces the JIT compiler to handle the code for scrolling outside the initially visible part of the listbox.
                    // This is needed to prevent unstable rhythm during autoplaying when the cursor leaves the initially visible part of the listbox
                    // and thus forces the first scroll operation.
#if true
                    // Not needed if we kan avoid the "if InvoceRequired mechanism above (Line 459)
                    Logger.LogCF(string.Format(": Changing SelectedIndex to {0} and back to 0", listBoxTimes.Items.Count - 1));
                    listBoxTimes.SelectedIndex = (listBoxTimes.Items.Count - 1);
#endif
                    listBoxTimes.SelectedIndex = 0;
                }

            }
            catch (Exception exception)
            {
                // Be sure to restore the UI state !
                UiUtilities.Beep();
                Logger.LogCFE(exception);
            }
            // At last move focus (from the NormalText textbox) to the mail Listbox to make the Screenreader do its job
            listBoxTimesHandler.Focus();
            textBoxScreenReader.Hide();
            listBoxTimesHandler.AutoReload = true;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = ResourcesForUI.TextBox_Messages_TheProgramIsExiting;
            WriteStatusInformation(message);
            // Remaining actions are taken in Application_ApplicationExit.
            // In this way the Model will always be shut down no matter why the application exits.
            Application.Exit();
        }

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
            Form helpForm = new HelpForm(organisationDependencies.ApplicationName, developerMode);
            helpForm.Show();
            // MessageBox.Show(ShortcutHelp.Create().ToString(), caption);
        }

        #endregion

        #region settings


        private void generelSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralSettingsForms generalSettingsForms = new GeneralSettingsForms(applicationName, userPreferencesHandler);
            DialogResult dialogResult = generalSettingsForms.ShowDialog();
            Logger.LogCF(string.Format(": GeneralSettingsForms returned {0}", dialogResult.ToString()));
            if (DialogResult.OK == dialogResult)
            {
                generalSettingsForms.SaveSettings();
            }
            generalSettingsForms.Dispose();
        }


        private void ShowBrailleMusicSettingsDialog(BrailleMusicSettingsForm.DeviceTypeEnum deviceTypeEnum)
        {
            BrailleMusicSettingsForm brailleMusicSettingsForm = new BrailleMusicSettingsForm(deviceTypeEnum, applicationName, userPreferencesHandler);
            DialogResult result = brailleMusicSettingsForm.ShowDialog();
            Logger.LogCF(string.Format(": BrailleMusicSettingsForm returned {0}", result.ToString()));
            if (DialogResult.OK == result)
            {
                brailleMusicSettingsForm.SaveSettings();
            }
            brailleMusicSettingsForm.Dispose();
        }

        private void embosserSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowBrailleMusicSettingsDialog(BrailleMusicSettingsForm.DeviceTypeEnum.Embosser);
        }

        private void highSpeedEmbosserSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowBrailleMusicSettingsDialog(BrailleMusicSettingsForm.DeviceTypeEnum.HighSpeedEmbosser);
        }

        private void notetakerSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowBrailleMusicSettingsDialog(BrailleMusicSettingsForm.DeviceTypeEnum.NoteTaker);
        }

        private void musicBrailleSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowBrailleMusicSettingsDialog(BrailleMusicSettingsForm.DeviceTypeEnum.GeneralDevice);
        }

        private void resetAllUserSettingsToDefaultValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = ResourcesForUI.Message_Settings_DoYouReallyWantToReset;
            DialogResult dialogResult = MessageBox.Show(message, applicationName, MessageBoxButtons.OKCancel);
            if (DialogResult.OK == dialogResult)
            {
                Logger.LogCF(": Resetting all user settings to default values");
                userPreferencesHandler.Reset();
            }
        }


        #endregion settings

        #region tools

        private string selectExe(string link, string appConfig, string hardCoded)
        {
            if (!string.IsNullOrEmpty(link)) return link;               // First priority:  The user has placed a link as a shortcut on the desktop
            if (!string.IsNullOrEmpty(appConfig)) return appConfig;     // Second priority: The value from the app.config file
            return hardCoded;                                           // Fallback:        A hardcoded value which the user can not change.
        }



        /// <summary>
        /// Executes the executable with the currently open MusicXml File as argument.
        /// If the executable is not found an error message is issued, specifying where the user can change the location of the executable
        /// </summary>
        /// <param name="executable"></param>
        /// <param name="settingName"></param>
        private void ExecuteOnCurrentMusicXmlFile(string executable, string settingName)
        {
            if (File.Exists(executable))
            {
                Utilities.RunExeWithFileArgument(executable, model.TheMusicXmlFileName);
            }
            else
            {
                messageHandler.ShowMessage(GetExternalProgramNotFoundMessage(executable, ResourcesForUI.ToolStripMenuItem_Settings_General, settingName));
            }
        }

        private void museScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExecuteOnCurrentMusicXmlFile(this.userPreferencesHandler.MuseScoreExe, ResourcesForSettings.General_MuseScoreLocation);
        }

        private void sibeliusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExecuteOnCurrentMusicXmlFile(this.userPreferencesHandler.SibeliusExe, ResourcesForSettings.General_SibeliusLocation);
        }

        private void startCapellaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExecuteOnCurrentMusicXmlFile(this.userPreferencesHandler.CapellaExe, ResourcesForSettings.General_CapellaLocation);
        }

        private void startFinaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExecuteOnCurrentMusicXmlFile(this.userPreferencesHandler.FinaleExe, ResourcesForSettings.General_FinaleLocation);
        }

        private void startPhotoScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.userPreferencesHandler.PhotoScoreExe))
            {
                Utilities.RunExeWithArgument(this.userPreferencesHandler.PhotoScoreExe, "");
            }
        }

        private void startBrailleMusicEditor2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.userPreferencesHandler.BrailleMusicEditor2Exe))
            {
                Utilities.RunExeWithArgument(this.userPreferencesHandler.BrailleMusicEditor2Exe, "");
            }
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
            model.ExternalToolsHandler.ReadMusicXmlFile("iexplore.exe", model.TheMusicXmlFileName);
        }

        private void musicXmlFileAsRawXMLUsingGoogleChromeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadMusicXmlFile("chrome.exe", model.TheMusicXmlFileName);
        }

        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadUserSettingsXmlFile(model.TheUserSettingsFileName);
        }

        private void viewAsInterpretedXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadInterpretation(model.AllMusicXmlObjecsts, model.TheMusicXmlFileName);
        }


        private void jAWSSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.ReadJawsSettingsFile(executingAssemblyShortName);
        }

        private void generateGraphicInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ExternalToolsHandler.GenerateGraphicInformation(model.TheMusicXmlFileName, model.Defaults, model.EventDescriptionList);
        }
        private void octoBraille1252ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectBrailleMusicFile(BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252);
        }

        private void aSCIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectBrailleMusicFile(BrailleFileHandler.FileEncoding.BRF_ASCII);
        }

        private void unicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectBrailleMusicFile(BrailleFileHandler.FileEncoding.BRF_Unicode);
        }

        private bool isValidBrailleMusic(string fileName, BrailleFileHandler.FileEncoding encoding)
        {
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(encoding, 0, 0);
            return brailleFileHandler.IsValidBrailleMusic(fileName);
        }

        private void resetUserSettingsToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.userPreferencesHandler.Reset();
        }

        private void userPreferencesLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We must call GetExecutingAssembly() from the assembly containing the .exe file, otherwise we just get tha name of a the .dll executing the call!
            string appFullName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            model.ExternalToolsHandler.OpenConfigurationFileLocation(appFullName);
        }


        private MusicBrailleReader.MusicBrailleReaderMainForm musicBrailleReaderMainform;

        private void musicBrailleReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            musicBrailleReaderMainform = MusicBrailleReader.MusicBrailleReaderMainForm.Create(this as MusicBrailleReader.IMusicBrailleReaderClient, model);
            musicBrailleReaderMainform.ShowDialog(); // Using ShowDialog() instead of Show() will freeze MainForm until musicBrailleReaderMainform closes.
        }

 

        private bool AppendLine(System.Text.StringBuilder sb, string fileName, BrailleFileHandler.FileEncoding encoding)
        {
            bool b = isValidBrailleMusic(fileName, encoding);
            string s = string.Format("{0} : {1} \r\n", encoding, b);
            sb.Append(s);
            return b;
        }

        private void autodetectedEncodingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Reset(); // Prevent survival of strange settings from latest usage of this reused OpenFileDialog
            openFileDialog.Title = ResourcesForUI.OpenFileDialog_Title;  // Just a neutral name "Open"
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.*", ""); // All file types                                                                                                             
            openFileDialog.InitialDirectory = Logger.LogFileDirectory; //
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ShowDialog();

            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return; // Let the user press ESC without warning him
            }

            string fileName = openFileDialog.FileName;
            Logger.LogCF(string.Format("Filename='{0}'", fileName));

            System.Text.StringBuilder sb = new System.Text.StringBuilder(string.Format("File='{0}'\r\n\r\n", Path.GetFileName(fileName)));

            bool isOctoBraille_1252 = AppendLine(sb, fileName, BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252);
            bool isASCII = AppendLine(sb, fileName, BrailleFileHandler.FileEncoding.BRF_ASCII);
            bool isUnicode = AppendLine(sb, fileName, BrailleFileHandler.FileEncoding.BRF_Unicode); // Use Windows default (UTF-8)
            bool isUnicodeUtf8 = AppendLine(sb, fileName, BrailleFileHandler.FileEncoding.BRF_Unicode_utf8); // Explicitly use UTF-8
            bool isUnicodeUtf16 = AppendLine(sb, fileName, BrailleFileHandler.FileEncoding.BRF_Unicode_utf16);
            bool isUnicodeUtf32 = AppendLine(sb, fileName, BrailleFileHandler.FileEncoding.BRF_Unicode_utf32);

            string message = sb.ToString();
            MessageBox.Show(message, applicationName, MessageBoxButtons.OK);
        }



        private bool SelectBrailleMusicFile(BrailleFileHandler.FileEncoding fileEncoding)
        {
            openFileDialog.Reset(); // Prevent survival of strange settings from latest usage of this reused OpenFileDialog
            openFileDialog.Title = ResourcesForUI.OpenFileDialog_Title;  // Just a neutral name "Open"
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.*", ""); // All file types                                                                                                             
                                                                  //            openFileDialog.InitialDirectory = Directory.Exists(model.LatestBrailleFileSaveDirectory) ? model.LatestBrailleFileSaveDirectory : Logger.LogFileDirectory; //
            openFileDialog.InitialDirectory = userPreferencesHandler.GetExistingDirectory(userPreferencesHandler.BrailleMusicDirectory, Logger.LogFileDirectory);
            openFileDialog.FileName = ""; // As we typically produce several Braille Music files at a time it has no meaning to select one of them
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ShowDialog();

            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return false; // Let the user press ESC without warning him
            }

            string inputFileName = openFileDialog.FileName;
            Logger.LogCF(string.Format("Filename='{0}'  Format={1}", inputFileName, fileEncoding));

            Int64 initialLogCount = Logger.GlobalCount;
            XmlDocument musicXmlDocument;
            bool developerMode = true;
            DecoderOptions decoderOptions = DecoderOptions.Create(DecoderOptions.RegionalOptionsEnum.Danish, developerMode);  // For now use Danish contractions
            List<DecoderItem> interpretation = model.DecoderHandler.InterpretBrailleMusicFile(inputFileName, fileEncoding, out musicXmlDocument, decoderOptions);
            Int64 finalLogCount = Logger.GlobalCount;
            if (null == interpretation)
            {
                Logger.LogCF(string.Format("Failed to interpret '{0}' as {1}", inputFileName, fileEncoding));
                return false;
            }

            Logger.LogCF(string.Format(": Successfully interpreted '{0}' as {1}", inputFileName, fileEncoding));
            Logger.LogCF(string.Format(": Returned {0} lines of information. Generated {1} loglines", interpretation.Count, finalLogCount - initialLogCount));
#warning todo!

            // Generate an output file file with:
            // 1) The original contents shown in dots as well as in numbers
            // 2) The result of the decoding operation

            // Get a list of files for generating the output

            DecoderOutputFileHandler decoderOutputFileHandler = DecoderOutputFileHandler.Create(inputFileName);

            List<string> strings = new List<string>();
            foreach (DecoderItem decoderItem in interpretation)
            {
                strings.Add(decoderItem.ToString());
            }
            // Write the decoded output as a text interpretation to a file
            decoderOutputFileHandler.SaveInterpretation(strings, decoderOutputFileHandler.FullOutputFileName);

            // Save the MusicXml file
            musicXmlDocument.Save(Console.Out); // To the console
            musicXmlDocument.Save(decoderOutputFileHandler.FullMusicXmlFileName); // To a file

            // Open the output file in NotePad
            //Utilities.RunExeWithFileArgument("NotePad", fullOutputFileName);

            // Open Explorer in the output directory.
            Utilities.RunExeWithDirArgument("Explorer", decoderOutputFileHandler.OutputDirectory);



            // Show in the UI or save as Unicode textfile including the interpretation of dot numbers. Show in NotePad!!

            return true;


        }


        private void analyzeLocalizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string baseDirectory = model.AnalyzeLocalization();
            Utilities.RunExeWithArgument("Explorer", baseDirectory);
        }

        #endregion // tools 

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

        #endregion // archives

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
            string url = organisationDependencies.LinkToNewestSoftware;
            if (string.IsNullOrWhiteSpace(url))
            {
                string text = "No link to new software found";
                Logger.LogCF(string.Format(": url={0}", url));
                MessageBox.Show(text);
#warning ToDo Show a LOCALIZED messagebox             
            }
            else
            {
                model.ExternalToolsHandler.OpenUrl(url);
            }
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Update all nodes in the tree with the same name
            userSettingsHandler.UpdateCheckBoxes(UserSettingsHandler.CheckboxOperation.Uncheck, UserSettingsHandler.CheckboxRelation.SameName);
        }

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Update all nodes in the tree with the same name
            userSettingsHandler.UpdateCheckBoxes(UserSettingsHandler.CheckboxOperation.Check, UserSettingsHandler.CheckboxRelation.SameName);
        }

        private void instrumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailsHandler.ShowGlobalDetails(DetailsHandler.DetailsEnum.Instruments, DetailsHandler.DetailsDirection.FromTop);
        }


        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.SaveUserSettings();
        }

        #region Export of Music Braille

        // Private definitions used for export to Braille Music
        private const Model.BrailleStyleEnum IbosStyle = Model.BrailleStyleEnum.IBOS;
        private const Model.BrailleStyleEnum BanaStyle = Model.BrailleStyleEnum.BANA2015;

        private string FormatExportMessage(BrailleDevice bD, string delimiter)
        {
            string result = string.Format("{0}BrailleDeviceType={1}{0}FileEncoding={2}{0}PageWidth={3}{0}PageHeight={4}{0}PageLayout={5}",
                              delimiter, bD.GetType().ToString(), bD.BrailleFileFormat, bD.PageWidth, bD.PageHeight, bD.BraillePageLayout);
            return result;
        }

        private string FormatExportMessage(string deviceType, string fileFormat, int pageWidth, int pageHeight, string pageLayout, string delimiter)
        {
            string result = string.Format("{0}BrailleDeviceType={1}{0}FileEncoding={2}{0}PageWidth={3}{0}PageHeight={4}{0}PageLayout={5}",
                              delimiter, deviceType, fileFormat, pageWidth, pageHeight, pageLayout);
            return result;
        }


        /// <summary>
        /// Common handler for generating Braille Music files from parameters taken from one of the 3 existing profiles.
        /// Called directly from 
        /// 1) MenuStripHandler.toNotetakerToolStripMenuItem_Click
        /// 2) MenuStripHandler.toEmbosserToolStripMenuItem_Click(object sender, EventArgs e)
        /// 3) MenuStripHandler.toIbosGenericDeviceToolStripMenuItem_Click
        /// All values needed are extracted from the "brailleDevice" parameter !
        /// </summary>
        /// <param name="brailleDevice"></param>
        private void Export(BrailleDevice brailleDevice)
        {
            Logger.LogCF(string.Format(": {0}", FormatExportMessage(brailleDevice, " "))); // In Log use SPACE as delimiter.
            if (developerMode)
            {
                MessageBox.Show("ExportMusicBrailleToFile()\r\n" + FormatExportMessage(brailleDevice, "\r\n")); // In Messagebox use CR LF as delimiter
            }

            textBoxEmpty.Show(); // Attract JAWS attension to make it stop talking
            textBoxEmpty.Focus();
            try
            {
                // throw new Exception(); // For test only
                BrailleMusicHandlerResult result = brailleMusicExportHandler.ExportMusicBrailleToFile(brailleDevice.BrailleFileFormat, brailleDevice.PageWidth, brailleDevice.PageHeight, brailleDevice.BraillePageLayout, brailleDevice.DeviceName);
                if (BrailleMusicHandlerResult.OkShowBrailleMusicReader == result)
                {
                    // Everything succeeded and the user accepted to open the BrailleREader
                    musicBrailleReaderMainform = MusicBrailleReader.MusicBrailleReaderMainForm.Create(this as MusicBrailleReader.IMusicBrailleReaderClient, model);
                    musicBrailleReaderMainform.OpenLatestDirectory(); // Emulate that the user used the File menu to start an OPenDialog
                    musicBrailleReaderMainform.ShowDialog(); // Using ShowDialog() instead of Show() will freeze MainForm until musicBrailleReaderMainform closes.
                }
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                messageHandler.ShowMessage(ResourcesForUI.Message_OperationFailed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            textBoxEmpty.Hide(); // Revert the operation that temporarily attracted JAWS attension
        }
          

        // After selecting GenericDevice all parameters: Encoding, pagewidth and pageheight are automatically taken from Settings->NoteTaker
        #region NoteTaker
        /// <summary>
        /// After selecting "Notetaker"
        /// Simple implementation for exporting to notetaker. This implementation needs no further input from the user !
        /// The Braille mapping is based on the current language (See mapping below)
        /// The formatting parameters are set to 0,0 (No formatting)        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toNotetakerToolStripMenuItem_Click(object sender, EventArgs e)
        {
#warning TODO Think about the old convension og using length and width = 0 to signal that no foematting is needed (The IBOS Layout case)
            BrailleDevice noteTaker = userPreferencesHandler.noteTaker;  // Just a shorthand
            if (noteTaker.BraillePageLayout == Model.BrailleStyleEnum.IBOS)
            {
                // This is the case where we handle backwards compatibility by setting PAgeWidth and PAgeHeight to 0
                string message = "Backwards compatible NoteTaker";
                Logger.LogCF(string.Format(": {0}", FormatExportMessage(message, noteTaker.BrailleFileFormat.ToString(), 0, 0, noteTaker.BraillePageLayout.ToString(), " ")));
                if (developerMode)
                {
                    MessageBox.Show("ExportMusicBrailleToFile()\r\n" + FormatExportMessage(message, noteTaker.BrailleFileFormat.ToString(), 0, 0, noteTaker.BraillePageLayout.ToString(), "\r\n"));
                }
                // This is tho old, original export of "IBOS style", only suitable for notetaker. Ignore result.
                BrailleMusicHandlerResult result = brailleMusicExportHandler.ExportMusicBrailleToFile(noteTaker.BrailleFileFormat, 0, 0, noteTaker.BraillePageLayout, null);
            }
            else
            {
                // This is the default case
                Export(noteTaker);
            }
        }

        #endregion NoteTaker

        // After selecting Embosser all parameters: Encoding, pagewidth and pageheight are automatically taken from Settings->Embosser
        #region Embosser
        /// <summary>
        /// After selecting "Embosser"
        /// Simple implementation for exporting to embosser. This implementation needs no further input from the user !
        /// The Braille mapping is based on the current language (See mapping below)
        /// The formatting parameters are set to (40,20) (Seems to be the format used by NOTA)    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toEmbosserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export(userPreferencesHandler.embosser);
        }
        #endregion Embosser

        #region highspeedembosser
        private void toHighSpeedEmbosserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export(userPreferencesHandler.highSpeedEmbosser);
        }
        #endregion

        // After selecting GenericDevice all parameters: Encoding, pagewidth and pageheight are automatically taken from Settings->Generic Braille Device
        #region GenericDevice
        // After selecting IBOS, GEneric device
        private void toIbosGenericDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export(userPreferencesHandler.genericBrailleDevice);
        }

        #endregion generic device

        // After selecting "Optional format" the user must manually specify all parameters: Encoding(ASCII, OctoBraille, Unicode} , pagewidth and pageheight
        #region OptionalFormat
        // After selecting "IBOS", "Any Format" , "Braille1252"
        private void txtOctoBraille1252ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IbosExport(BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252);
        }

        // After selecting "IBOS", "Any Format" , "ASCII"
        private void brfASCIIToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IbosExport(BrailleFileHandler.FileEncoding.BRF_ASCII);
        }

        // After selecting "IBOS" "Any Format" , "Unicode"
        private void brfUnicodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IbosExport(BrailleFileHandler.FileEncoding.BRF_Unicode);
        }


        /// <summary>
        /// Export in "IBOS" style. Only used for backward compatability with version 3.
        /// JSJ 2022.02.04: Refactored in order to resemble the Export() method used for export using a predefined Braille device profile
        /// In this way we can use the same "textBoxEmpty" trick as in Export() for making JAWS stop talking about irrelevant issues. 
        /// </summary>
        /// <param name="fileEncoding"></param>
        private void IbosExport(BrailleFileHandler.FileEncoding fileEncoding)
        {
            textBoxEmpty.Show(); // Attract JAWS attension to make it stop talking
            textBoxEmpty.Focus();
            try
            {
     
                BrailleMusicHandlerResult result =  brailleMusicExportHandler.ExportMusicBrailleToFile(fileEncoding, IbosStyle);
                // Do exactly as in the BANA case, implemented in Export().
                // (We might decide to handle the IDOS case otherwise decause the output is not well suited for the IBOS MusicBrailleReader.)
                if (BrailleMusicHandlerResult.OkShowBrailleMusicReader == result)
                {
                    // Everything succeeded and the user accepted to open the BrailleREader
                    musicBrailleReaderMainform = MusicBrailleReader.MusicBrailleReaderMainForm.Create(this as MusicBrailleReader.IMusicBrailleReaderClient, model);
                    musicBrailleReaderMainform.ShowDialog(); // Using ShowDialog() instead of Show() will freeze MainForm until musicBrailleReaderMainform closes.
                }

            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            textBoxEmpty.Hide(); // Return JAWS atension to where it was 
        }

        #endregion OptionalFormat

        private void inOptionalFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.LogCF("Unused");
        }

        private void inBana2015OptionalFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.LogCF("Unused");
        }
   
        // This handler allows for using the current contents of the current profiles as part of the menu texts
        private void exportMusicBrailleToolStripMenuItem_Paint(object sender, PaintEventArgs e)
        {
            this.LocalizeToolsMenuItems(userPreferencesHandler);
        } 

        #endregion // Export of Music Braille

        /// <summary>
        /// Generally usable method for building a message for a messagebox when an external program is not found
        /// </summary>
        /// <param name="executableName">The path of the executable which is not found</param>
        /// <param name="level1String">The 1. level location under S2ttings for this path</param>
        /// <param name="level2String">The 2. level location under S2ttings for this path</param>
        /// <returns></returns>
        private string GetExternalProgramNotFoundMessage(string executableName, string level1String, string level2String)
        {
            string externalProgramNotFound = ResourcesForUI.Message_File_ExternalProgramNotFound; //   "External program not found";
            string pleaseEnterValidPathIn = ResourcesForUI.Message_File_PleaseEnterValidPathIn; // "Please enter a valid path in";
            string settings = ResourcesForUI.ToolStripMenuItem_Settings; //  "Settings";
            StringBuilder result = new StringBuilder();
            result.AppendLine(string.Format("{0}:", externalProgramNotFound));
            result.AppendLine();
            result.AppendLine(string.Format("'{0}'", executableName));
            result.AppendLine();
            result.AppendLine(string.Format("{0}:", pleaseEnterValidPathIn));
            result.AppendLine();
            result.AppendLine(string.Format("'{0}'-->", Utilities.RemoveAmpersant(settings)));
            result.AppendLine(string.Format("  '{0}' -->", Utilities.RemoveAmpersant(level1String)));
            result.AppendLine(string.Format("    '{0}'", Utilities.RemoveAmpersant(level2String)));
            return result.ToString();

        }


        #region Print

        private void printMusicBrailleUsingIBPrintToolStripMenuItem_Click(object sender, EventArgs e) { } // Obsolete. Replaced by:
        private void usingExternalProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string executable = AppConfigHandler.GetValue(AppConfigHandler.KeyEnum.IBPrintExe);      //  Typically @"C:\Program Files (x86)\Index Braille\IbPrint\IbPrint.exe";
            string executable = userPreferencesHandler.genericBrailleDevice.ApplicationLocation;
            string executableName = userPreferencesHandler.genericBrailleDevice.ApplicationLocation;
            if (!File.Exists(executable))
            {
                messageHandler.ShowMessage(GetExternalProgramNotFoundMessage(executable, ResourcesForUI.ToolStripMenuItem_Settings_MusicBraille, ResourcesForSettings.Braille_ApplicationLocation));
                return;
            }
            //            string directory = Directory.Exists(model.LatestBrailleFileSaveDirectory) ? model.LatestBrailleFileSaveDirectory : "";
            string directory = userPreferencesHandler.GetExistingDirectory(userPreferencesHandler.BrailleMusicDirectory, "");
            Logger.LogCF(string.Format(": Executable='{0}'     Directory='{1}'", executable, directory));
#warning TODO find out how to make the UI version of IBPrint prefer model.LatestBrailleFileSaveDirectory instead of the latest directory used by the Add button
            Utilities.RunExeWithArgument(executable, "");
        }


        private void printMusicBrailleToolStripMenuItem_Click(object sender, EventArgs e) { } // Obsolete. Replaced by:
        private void viaWindowsPrintDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            embosserHandler.Emboss(userPreferencesHandler.GetExistingDirectory(userPreferencesHandler.BrailleMusicDirectory, myMusicXmlDirectory));
        }
        #endregion Print

        #region Copy
        // Opens Explorer.exe in the directory where the latest MusicBraille file was generated. Defaults to myMusicXmlDirectory.
        private void copyMusicBrailleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string program = Utilities.ExplorerExe;
            string directory = userPreferencesHandler.GetExistingDirectory(userPreferencesHandler.BrailleMusicDirectory, myMusicXmlDirectory);
            Logger.LogCF(string.Format(": Starting {0} in {1}", program, directory));
            Utilities.RunExeWithArgument(program, directory);
        }
        #endregion Copy

        // End new UI

        /// <summary>
        /// Handles Details for BrailleFiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void brailleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!brailleMusicExportHandler.ScoreIsLoaded()) return; // Beeps and logs.
            if (!brailleMusicExportHandler.ScoreIsSupported(IbosStyle)) return; // Shows warning dialog
            detailsHandler.ShowGlobalDetails(DetailsHandler.DetailsEnum.BrailleFile, DetailsHandler.DetailsDirection.FromTop);
        }

        private void usersManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUsersManual();
        }

        private void generateMusicBrailleTestpatternToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool acceptCancel = true; // Accept cancel as "Use default parameters"
            bool validParams = parameterInputHandler.GetMusicBrailleFormatParameters(acceptCancel); // Prompt the user for formatting parameters
            if (!validParams)
            {
                UiUtilities.Beep();
                return;
            }
            bool ok = model.GenerateMusicBrailleTestpattern();
            if (!ok)
            {
                UiUtilities.Beep();
                Logger.LogCF(": Failed to generate Braille Music testpatterns");
            }
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
                    && (localizedFilenameEstension == Path.GetExtension(filename)))
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


        #region Repeat,GoTo,NormalTempo

        private void repeatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parameterInputHandler.RepeatToolStripMenuItem_Click(); // Pass on
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index;
            if (parameterInputHandler.GoToToolStripMenuItem_Click(out index)) // Pass on
            {
                listBoxTimes.SelectedIndex = index;
            }
        }

        private void ofNominalTempoToolStripMenuItem_Click(object sender, EventArgs e) // Pass on
        {
            parameterInputHandler.OfNominalTempoToolStripMenuItem_Click();
        }

        #endregion

    }
}
