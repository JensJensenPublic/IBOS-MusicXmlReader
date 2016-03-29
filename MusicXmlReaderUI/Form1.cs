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
            buttonStart.Select();
            if (!showTimes) listBoxTimes.Hide();
            model = new Model(listBoxFiltered, listBoxTimes);
            listBoxFiltered.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
        }


        private void SelectedIndexChanged(object sender, System.EventArgs e)
        {
            object o = listBoxFiltered.Items[listBoxFiltered.SelectedIndex];
            model.musicPlayer.SelectedIndexChanged(listBoxFiltered.SelectedIndex, o);
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


        private void buttonStartUsingDOM_Click(object sender, EventArgs e)
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

            // Load the Checked Listboxes controlling the user settings
            model.LoadListBoxOfParts(checkedListBoxPartsToPlay);
            model.LoadListBoxOfParts(checkedListBoxPartsToRead);
            autoReload = true;

            // Load the ListBox showing the filtered values 
            listBoxFiltered.Focus();
            listBoxFiltered.SelectedIndex = 0;

            // Let the Model do the hard work of transforming to e timed representation.
            model.LoadListBoxTimes();
        }

        private void Play_Click(object sender, EventArgs e)
        {
            model.StartPlayingMono();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            model.musicPlayer.StopPlaying();
        }

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

        private void butonPlayPoly_Click(object sender, EventArgs e)
        {
            model.StartPlayingPoly();
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


        private void checkBoxShowStartTime_CheckedChanged(object sender, EventArgs e)
        {
            model.SetShowStartTime(checkBoxShowStartTime.Checked);
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkBoxPlayHarmonies_CheckedChanged(object sender, EventArgs e)
        {
            model.SetPlayHarmonies(checkBoxPlayHarmonies.Checked);
            // No reload needed for "Play" options
        }

        private void checkBoxShowHarmonies_CheckedChanged(object sender, EventArgs e)
        {
            model.SetShowHarmonies(checkBoxShowHarmonies.Checked);
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkBoxOplæsBecifringskoder_CheckedChanged(object sender, EventArgs e)
        {
            model.SetShowHarmonyCodes(checkBoxOplæsBecifringskoder.Checked);
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkBoxReadMeasureNumbers_CheckedChanged(object sender, EventArgs e)
        {
            model.SetReadMeasureNumbers(checkBoxReadMeasureNumbers.Checked);
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkBoxReadEndEvents_CheckedChanged(object sender, EventArgs e)
        {
            model.SetReadEndEvents(checkBoxReadEndEvents.Checked);
            if (autoReload) model.LoadListBoxTimes();
        }



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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            CheckState checkState = e.Shift ? CheckState.Unchecked : CheckState.Checked;
            
            bool check = !e.Shift;
            bool control = e.Control;
            bool alt = e.Alt;
            int number = 0;
            bool isNumberKey = false;
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
                    if (e.Control) checkBoxShowHarmonies.Checked = check;
                    if (e.Alt) checkBoxPlayHarmonies.Checked = check;
                    break;
                case Keys.T: // "Takter" TO DO Handle localisation issue here!
                    if (e.Control) checkBoxReadMeasureNumbers.Checked = check;
                    if (e.Alt)     checkBoxPlayMeasureNumbers.Checked = check;
                    break;
                default: break;
            }
            if (isNumberKey) 
            {
                if (e.Control) SetCheckBox(checkedListBoxPartsToPlay, number, checkState);
                if (e.Alt) SetCheckBox(checkedListBoxPartsToRead, number, checkState);
            }
            
            e.Handled = true;
            //string item = (checkedListBoxPartsToPlay.Items[1]).ToString();
            //bool b = checkedListBoxPartsToPlay.GetItemChecked(1);
            //checkedListBoxPartsToPlay.SetItemCheckState(index, checkState);
        }

    }

}
