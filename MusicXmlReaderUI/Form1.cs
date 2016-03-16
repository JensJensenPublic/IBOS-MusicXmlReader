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
            if (!showTimes)  listBoxTimes.Hide();
            model = new Model(listBoxFiltered,listBoxTimes);        
            listBoxFiltered.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);          
        }

   
        private  void SelectedIndexChanged(object sender, System.EventArgs e)
        {
            object o = listBoxFiltered.Items[listBoxFiltered.SelectedIndex];
            model.musicPlayer.SelectedIndexChanged(listBoxFiltered.SelectedIndex,o);
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
            model.LoadMusicXmlFile(); // Load the .xml file into the Model and build all internal data structures.

            textBoxMessage.Focus();
            textBoxMessage.Text = string.Format("Indlæser {0}",model.FullXmlFileName); 
 
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
            if (autoReload) model.LoadListBoxTimes();
        }

        private void checkBoxShowHarmonies_CheckedChanged(object sender, EventArgs e)
        {
            model.SetShowHarmonies(checkBoxShowHarmonies.Checked);
            if (autoReload) model.LoadListBoxTimes();
        }


    }

}
