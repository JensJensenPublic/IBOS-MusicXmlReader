using System;
using System.IO;
using System.Windows.Forms;
using MusicXmlReaderModel;

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
            if (Keys.None == keys) return text;
            return Utilities.RemoveAmpersant(text) + " " + UiUtilities.KeysToString(keys); // Say the control char before the other character.
        }

        void GenerateAccessibleName(ref ToolStripMenuItem menuItem)
        {
            menuItem.AccessibleName = GenerateAccessibleName(menuItem.Text, menuItem.ShortcutKeys);
        }

        void LocalizeMenuStrip()
        {

            //MenuStrip.Text = "??";
            // Children of MenuStrip
            filesToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files;
            editToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Edit;
            viewToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_View;
            toolsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools;
            archivesToolStripMenuItem.Text = ResourcesForUI.ToolsStripMenuItem_Archives;
            helpToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help;

            // Children (and grandchildren) of  fileToolStripMenuItem
            openMusicXmlFileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile;
            openMusicXmlFileToolStripMenuItem.ShortcutKeys = ShortcutHandler.openMusicXmlFile;
            GenerateAccessibleName(ref openMusicXmlFileToolStripMenuItem);
            importDownloadsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ImportDownloads;
            importNewestDownloadsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ImportNewestDownloads;
            importNewSampleFilesToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ImportNewestSamples;
            exitToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_Exit;
            exitToolStripMenuItem.ShortcutKeys = ShortcutHandler.exitApplication;
            this.exportMusicBrailleToFileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ExportMusicBrailleToFile;
            // this.exportMusicBrailleToFileToolStripMenuItem.ShortcutKeys = ShortcutHandler....;
            this.brfUnicodeToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_brf_Unicode;
            //this.brfUnicodeToolStripMenuItem.ShowShortcutKeys = ShortcutHandler....;
            this.brfASCIIToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_brf_ASCII;
            //this.brfASCIIToolStripMenuItem.ShortcutKeys = ShortcutHandler....;
            GenerateAccessibleName(ref exitToolStripMenuItem);

            // Children of editToolStripMenuItem, referring to the Treeview
            // Texts:  NOTE! Use the same texts as used in the treeview to which these items refer!!
            allItemsToolStripMenuItem.Text = ResourcesForUI.TreeView_All_Items;
            filterItemsToolStripMenuItem.Text = ResourcesForUI.TreeWiew_Items;
            musicRepresentationToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSound;
            textRepresentationToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSpeech;
            brailleRepresentationToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsBraille;
            partsToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSound_Parts;
            detailsToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSound_Details;
            uncheckAllToolStripMenuItem.Text = ResourcesForUI.TreeView_UncheckAll;
            checkAllToolStripMenuItem.Text = ResourcesForUI.TreeView_CheckAll;

            // Children of editToolStripMenuItem referring to the ParameterInputForm
            repeatToolStripMenuItem.Text = ResourcesForUI.ParameterInputForm_Repeat;
            goToToolStripMenuItem.Text = ResourcesForUI.ParameterInputForm_GoTo;
            ofNominalTempoToolStripMenuItem.Text = ResourcesForUI.ParameterInputForm_PctOfNominalTempo;


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
            museScoreToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_MuseScore;
            sibeliusToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Sibelius;
            logfileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Logfile;
            openXMLFileLocationToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_OpenXmlFileLocation;
            openLogFileLocationToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Logfile_Location;
            inspectAsXMLToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml;
            viewAsInterpretedXMLToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_ViewAsInterpretedXml;
            jAWSSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_JAWS_Settings;
            userSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_User_Settings;

            // Children of  helpToolStripMenuItem
            this.aboutIBOSMusicXmlReaderToolStripMenuItem.Text = string.Format("{0} {1}", ResourcesForUI.ToolStripMenuItem_Help_About, applicationName);
            this.keyboardShortcutsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_Shortcuts;
            this.linkToNewestSoftwareToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdate;
            this.usersManualToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_ShowUsersManual;

            // CheckShortCuts();
        }


        private string GetFileOpenInitialDirectory()
        {
            string functionName = "GetOpenFileInitialDirectory";
            string result = myMusicXmlDirectory;  // When running a user session we want to use the files in the <user>\Documents\IBOS NNodelæser directory 
            //if (model.InitialDirectory.Contains("Visual Studio"))
            //{
            //    result = model.InitialDirectory;   // When running a debug session we want to use the files in the debug\bin directory
            //}
            Logger.Log(string.Format("{0}.{1} returns {2}", className, functionName, result));
            return result;
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
            openFileDialog.Filter = string.Format("{0}|*.xml;*.musicxml;*.mxl", ResourcesForUI.OpenFileDialog_Filter); // Only present .xml files and .mxl files
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

            // Save User settings for currently loaded file (if any) immediately before clearing the UI:

            model.SaveUserSettings();

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

            switch (extension)
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

            this.Text = UiUtilities.GetTitleInfo(applicationName,model);

            model.SetUserTempo(100); // Play at 100% of tempo specified in MusicXml file

            return true;
        }




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
                UiUtilities.Beep();
                Logger.Log(string.Format("{0}.{1} Exception. Message={2}", className, functionName, exception.Message));
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

        //private void uncheckOthersToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    userSettingsHandler.UpdateCheckBoxes(UserSettingsHandler.CheckboxOperation.Uncheck, UserSettingsHandler.CheckboxRelation.SameParent);
        //    // userSettingsHandler.UpdateOtherCheckboxes(UserSettingsHandler.CheckboxOperation.Uncheck);
        //}

        //private void checkOthersToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    userSettingsHandler.UpdateCheckBoxes(UserSettingsHandler.CheckboxOperation.Check, UserSettingsHandler.CheckboxRelation.SameParent);
        //    //userSettingsHandler.UpdateOtherCheckboxes(UserSettingsHandler.CheckboxOperation.Check);     
        //}


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


#endregion // tools ***********************************************************************


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
            // string url = "http://www.ibos.dk/hjaelpemidler/ibos-nodelaeser.html";
            string url = ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdateLink;
            model.ExternalToolsHandler.OpenUrl(url);
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



        /// <summary>
        /// Second-level item. Use .brf (Unicode)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void brfUnicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportMusicBrailleToFile(BrailleFileHandler.FileFormat.BRF_Unicode);
        }

        /// <summary>
        /// Second-level item. Use .brf (ASCII)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void brfASCIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportMusicBrailleToFile(BrailleFileHandler.FileFormat.BRF_ASCII);
        }

        /// <summary>
        /// First-level item. Use .brf (Unicode) as default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportMusicBrailleToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportMusicBrailleToFile(BrailleFileHandler.FileFormat.BRF_Unicode);
        }


        /// <summary>
        /// Common handling of all fileformats
        /// </summary>
        /// <param name="fileFormat"></param>
        private void exportMusicBrailleToFile(BrailleFileHandler.FileFormat fileFormat)
        {
            if (null == model.EventDescriptionList)
            { 
                Logger.LogCF(": No MusicXml file is currently loaded!");
                UiUtilities.Beep();
                return;  
            }
            parameterInputHandler.exportMusicBrailleToFileToolStripMenuItem_Click(); // Prompt the user for formatting parameters
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileFormat, model.UserPreferences.CharsPerLine, model.UserPreferences.LinesPerForm);
            string brailleRepresentation = model.GetBrailleRepresentation(brailleFileHandler);
            if (null == brailleRepresentation)
            {
                Logger.LogCF(string.Format(": Failed to convert to Braille"));
                return;
            }
            
            // Conversion succeeded. Prompt use for filename  

            string extension = brailleFileHandler.GetExtension(); // Currently always ".brf" Maybe later ".pef" ?
            string fileFormatName = brailleFileHandler.GetFileFormat(); // Currently "BRF_Unicode" or "BRF_ASCII"
            saveBrailleFileDialog.InitialDirectory = Path.GetDirectoryName(model.TheMusicXmlFileName);
            saveBrailleFileDialog.FileName = Path.GetFileNameWithoutExtension(model.TheMusicXmlFileName) + "." +fileFormatName;
            saveBrailleFileDialog.DefaultExt = extension;
#warning ToDo Localize Filer
            // saveBrailleFileDialog.Filter = string.Format("{0}|*.brf", "Braille filer");
            saveBrailleFileDialog.Filter = string.Format("{0}|.brf", "Braille filer");
            DialogResult dialogResult = saveBrailleFileDialog.ShowDialog();

            if (DialogResult.OK == dialogResult)
            {
                bool ok = brailleFileHandler.WriteToFile(brailleRepresentation, saveBrailleFileDialog.FileName, true); // Taking in account width and height
                Logger.LogCF(string.Format(": Export to '{0}' {1}", saveBrailleFileDialog.FileName, ok ? "succeded" : "failed"));
            }
            else
            {
                Logger.LogCF(string.Format(": SaveDialog returned {0}", dialogResult.ToString()));
            }

        }

        private void brailleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailsHandler.ShowGlobalDetails(DetailsHandler.DetailsEnum.BrailleFile, DetailsHandler.DetailsDirection.FromTop);
        }




        private void usersManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowUsersManual();
        }


        private void generateMusicBrailleTestpatternToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.GenerateMusicBrailleTestpattern();
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
