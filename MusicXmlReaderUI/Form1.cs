using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderUI
{
    public partial class Form1 : Form
    {
        bool showTimes = true;
        Model model;
        bool autoReload;

        // string fullFileName = @"C:\temp\MusicXML\La Mer.xml"; // The sample XML file to read from

        public Form1()
        {
            InitializeComponent();
            if (!showTimes) listBoxTimes.Hide();
            model = new Model(listBoxFiltered, listBoxTimes);      
            listBoxFiltered.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
        }


        private void SelectedIndexChanged(object sender, System.EventArgs e)
        {
            object o = listBoxFiltered.Items[listBoxFiltered.SelectedIndex];
            model.musicPlayer.SelectedIndexChanged(listBoxFiltered.SelectedIndex, o);
        }

        /// <summary>
        /// Simple way to assure that the usersettings reflect the UI
        /// More lines must be added when addinge more controls tothe UI
        /// </summary>
        /// <param name="userSettings"></param>
        private void InitUserSettings(UserSettings userSettings)
        {
            //userSettings.readDivisions = checkBoxShowStartTime.Checked;
            //userSettings.playHarmonies = checkBoxPlayHarmonies.Checked;
            //userSettings.readHarmonies = checkBoxShowHarmonies.Checked;
            //userSettings.readHarmonyCodes = checkBoxOplæsBecifringskoder.Checked;
            //userSettings.readMeasureNumbers = checkBoxReadMeasureNumbers.Checked;
            //userSettings.readEndEvents = checkBoxReadEndEvents.Checked;
            //userSettings.readNotes = checkBoxReadPitch.Checked;
            //userSettings.readNoteOctaves = checkBoxReadOctave.Checked;
            //userSettings.readNoteTypes = checkBoxReadDuration.Checked;
        }
                
        //private void Recurse(XmlNodeList childrenNodes)
        //{  
        //    foreach (XmlNode childNode in childrenNodes)
        //    {

        //        if (showRaw)
        //        {
        //            // The Unfiltered data
        //            if (listBoxRaw.Items.Count >= 1000) return;
        //            switch (childNode.NodeType)
        //            {
        //                case XmlNodeType.Element:
        //                    listBoxRaw.Items.Add(childNode.NodeType + " " + childNode.Name);         
        //                    break;
        //            }
        //        }

        //        // The filtered data

        //        bool doRecursion = true;
        //        switch (childNode.NodeType)
        //        {
        //            case XmlNodeType.Element:
        //                doRecursion = model.WriteElement(childNode);
        //                break;
        //            case XmlNodeType.Comment:
        //                listBoxFiltered.Items.Add(childNode.InnerText);
        //                break;
        //        }
        //        if (doRecursion)
        //        {
        //            Recurse(childNode.ChildNodes);
        //        }
        //    }     
        //}


        private void SelectAndOpenMusicXmlFile(object sender, EventArgs e)
        {
            openFileDialog.FileName = "Node.xml"; // Use this sample file as a default
            openFileDialog.Filter = "MusicXml filer|*.xml"; // Only present .xml files
            openFileDialog.InitialDirectory = model.InitialDirectory;
            openFileDialog.ShowDialog();

            textBoxMessage.Focus();
            textBoxMessage.Text = string.Format("Indlæser {0}", openFileDialog.FileName);

            if (!model.LoadMusicXmlFile(openFileDialog.FileName)) // Load the selected .xml file into the Model and build all internal data structures.
            {
                // Simple error handling
                textBoxMessage.Text = string.Format("Kunne ikke indlæse {0}", openFileDialog.FileName);
                return;
            }
            

            // Reflect the UI values of the UserSettings to the model
            InitUserSettings(model.UserSettings);

            // Load the Checked Listboxes controlling the user settings per part

            model.LoadListBoxOfParts(checkedListBoxPartsToPlay);
            model.LoadListBoxOfParts(checkedListBoxPartsToRead);
            model.LoadListBoxOfParts(checkedListBoxPartsToReadLyrics);
            model.LoadListBoxOfParts(checkedListBoxParts); // The over all checked listbox

            // Load the Checked listboxes controlling the global user settings
            for (int i = 0; (i < (int)UserSettings.ReaderSettings.NumberOfReaderSettings); i++)
            {
                checkedListBoxReaderSettings.Items.Add(model.UserSettings.ReaderSettingsNames[i], model.UserSettings.ReaderSettingsValues[i]);
            }

            for (int i = 0; (i < (int)UserSettings.PlayerSettings.NumberOfPlayerSettings); i++)
            {
                checkedListBoxPlayerSettings.Items.Add(model.UserSettings.PlayerSettingsNames[i], model.UserSettings.PlayerSettingsValues[i]);
            }

            autoReload = true; // From now on all changes are  made by user and must be handled
      
            // Load the ListBox showing the filtered values 
            //listBoxFiltered.Focus();
            //listBoxFiltered.SelectedIndex = 0;

            // Let the Model do the hard work of transforming to e timed representation.
            model.LoadListBoxTimes();

            // Focus on the listbox representing the time representation
            listBoxTimes.Focus();
            //listBoxTimes.SelectedIndex = 0;
        }

        #region Buttons
        private void Play_Click(object sender, EventArgs e)
        {
            model.StartPlayingMono();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            model.musicPlayer.StopPlaying();
        }

        private void butonPlayPoly_Click(object sender, EventArgs e)
        {
            model.StartPlayingPoly();
        }


        #endregion //Buttons

        private void numericUpDownPlaySpeed_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = sender as NumericUpDown;
            float value = (float)numericUpDown.Value;
            model.PlaySpeedChanged(100F / value);
        }


        private void listBoxTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            object o = listBoxTimes.Items[listBoxTimes.SelectedIndex];
            model.musicPlayer.SelectedIndexChanged(listBoxTimes.SelectedIndex, o);

        }

        #region Checked Listboxes


        private void checkedListBoxParts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //model.SetParts(e.Index, (CheckState.Checked == e.NewValue));
            // Copy the new value into all of the 3 listboxes
            checkedListBoxPartsToPlay.SetItemChecked(e.Index, (CheckState.Checked == e.NewValue));
            checkedListBoxPartsToRead.SetItemChecked(e.Index, (CheckState.Checked == e.NewValue));
            checkedListBoxPartsToReadLyrics.SetItemChecked(e.Index, (CheckState.Checked == e.NewValue));
        }


        /// <summary>
        /// Occurs whenever the state of any of the checkboxes changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxPartsToPlay_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            model.SetPartsToPlay(e.Index, (CheckState.Checked == e.NewValue));
            // No reload needed for "Play" options
        }

        /// <summary>
        /// Occurs whenever the state of any of the checkboxes changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBoxPartsToRead_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            model.SetPartsToRead(e.Index, (CheckState.Checked == e.NewValue));
            // This has changed the way ToString() works the notes are drawn in listBoxTimes, so it must be redrawn
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkedListBoxPartsToReadLyrics_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            model.SetPartsToReadLyrics(e.Index, (CheckState.Checked == e.NewValue));
            // This has changed the way ToString() works the notes are drawn in listBoxTimes, so it must be redrawn
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkedListBoxReaderSettings_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            model.UserSettings.ReaderSettingsValues[e.Index] = (CheckState.Checked == e.NewValue);
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkedListBoxPlayerSettings_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            model.UserSettings.PlayerSettingsValues[e.Index] = (CheckState.Checked == e.NewValue);
            if (autoReload) model.LoadListBoxTimes();
        }



        #endregion // Checked Listboxes

        #region CheckBoxes
        #endregion // CheckBoxes

        /// <summary>
        /// Control checkbox for one spscific part
        /// </summary>
        /// <param name="checkedListBox"></param>
        /// <param name="number"></param>
        /// <param name="checkState"></param>
        private void SetCheckBox(CheckedListBox checkedListBox, int number, CheckState checkState)
        {
            int index = number - 1;  // Voices are numbered from 1 and up
            if ((index >= 0) && (index < checkedListBox.Items.Count))
            {
                checkedListBox.SetItemCheckState(index, checkState);
            }
        }

        /// <summary>
        /// Control checkboxes for all parts
        /// </summary>
        /// <param name="checkedListBox"></param>
        /// <param name="checkState"></param>
        private void SetCheckBoxes(CheckedListBox checkedListBox, CheckState checkState)
        {
            for (int part = 1; (part <= checkedListBox.Items.Count); part++)
            {
                SetCheckBox(checkedListBox, part, checkState);
            }
        }

        /// <summary>
        /// Use special keyboard keys instead of clicking the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            CheckState checkState = e.Shift ? CheckState.Unchecked : CheckState.Checked;
            
            bool check = !e.Shift;
            bool control = e.Control;
            bool alt = e.Alt;
            int number = 0;
            bool isNumberKey = false;
            bool isHandled = false;
            switch (e.KeyCode)
            {
                case Keys.D0: // Set or clear all voices:
                    if (e.Control) SetCheckBoxes(checkedListBoxPartsToPlay,checkState);
                    if (e.Alt) SetCheckBoxes(checkedListBoxPartsToRead, checkState);
                    break;

                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9: number = e.KeyCode - Keys.D0; isNumberKey = true; break; // Set or clear one voice
                case Keys.B: // "Becifringer" TO DO Handle localisation issue here!
                    //if (e.Control) checkBoxShowHarmonies.Checked = check;
                    //if (e.Alt) checkBoxPlayHarmonies.Checked = check;
                    isHandled = true;
                    break;
                case Keys.T: // "Takter" TO DO Handle localisation issue here!
                    //if (e.Control) checkBoxReadMeasureNumbers.Checked = check;
                    //if (e.Alt)     checkBoxPlayMeasureNumbers.Checked = check;
                    isHandled = true;
                    break;
                default: break;
            }
            if (isNumberKey) 
            {
                if ((e.Control) && !(e.Alt)) SetCheckBox(checkedListBoxPartsToPlay, number, checkState);
                if ((e.Alt) && !(e.Control)) SetCheckBox(checkedListBoxPartsToRead, number, checkState);
                if ((e.Alt) && (e.Control)) SetCheckBox(checkedListBoxPartsToReadLyrics, number, checkState);
                isHandled = true;
            }

            e.Handled = isHandled; // Do not pass this key on
            //string item = (checkedListBoxPartsToPlay.Items[1]).ToString();
            //bool b = checkedListBoxPartsToPlay.GetItemChecked(1);
            //checkedListBoxPartsToPlay.SetItemCheckState(index, checkState);
        }


        private void buttonReadLogFile_Click(object sender, EventArgs e)
        {
            model.ReadLogFile();
        }

        private void buttonReadMusicXmlFile_Click(object sender, EventArgs e)
        {
            model.ReadMusicXmlFile();
        }

        private void buttonReadInterpretation_Click(object sender, EventArgs e)
        {
            model.ReadInterpretation();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            model.StartMuseScore();
        }


        #region Menu handlers

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show a standard Select File dialog to allow the user to select and open a MusicXml file
            SelectAndOpenMusicXmlFile(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TO DO: Call common code to stop MusicPlayer and exit.
        }

        private void logFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ReadLogFile();
        }

        private void museScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.StartMuseScore();
        }

        private void xmlFilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ReadMusicXmlFile();
        }

        private void fortolketXMLFilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.ReadInterpretation();
        }

        #endregion


        /// <summary>
        /// Common convenience method for focusing on a checked listbox and selecting the first item (if any)
        /// </summary>
        /// <param name="checkedListBox"></param>
        private void FocusAndSelect(CheckedListBox checkedListBox)
        {
            checkedListBox.Focus();       
            //checkedListBox.Select();
            if (checkedListBox.Items.Count > 0)
            {
                checkedListBox.SetSelected(0, true);
                //checkedListBox.SelectedItem = 0;
            }        
            checkedListBox.Refresh();

        }

        private void stemmerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FocusAndSelect(checkedListBoxParts);    
        }

        private void stemmerSomSpillesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FocusAndSelect(checkedListBoxPartsToPlay);
        }

        private void stemmerSomOplæsesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FocusAndSelect(checkedListBoxPartsToRead);
        }

        private void stemmerMedLyrikoplæsningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FocusAndSelect(checkedListBoxPartsToReadLyrics); 
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void logFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.OpenLogFileLocation();
        }

        private void musicXmlFileLocationtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.OpenMusicXmlFileLocation();
        }

        private void checkedListBoxParts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }

}
