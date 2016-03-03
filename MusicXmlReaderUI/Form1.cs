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
            string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);
            string fileName = "Node.xml";
            textBoxMessage.Focus();
            textBoxMessage.Text = string.Format("Indlæser {0}",fileName);
            fileName = System.IO.Path.Combine(executingDirectory, fileName);
            XmlDocument doc = new XmlDocument();   
            XmlTextReader reader = new XmlTextReader(fileName);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            doc.Load(reader);
            model.Recurse(doc.ChildNodes);
            model.Init(); // Experimental code !!
            model.LoadListBox();
            model.LoadListBoxTimes();
            listBoxFiltered.Focus();
            listBoxFiltered.SelectedIndex = 0;
            model.LoadListBoxOfParts(checkedListBoxPartsToPlay);
            model.LoadListBoxOfParts(checkedListBoxPartsToRead);
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
            model.musicPlayer.PlaySpeedChanged(sender, e);
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
            model.LoadListBoxTimes();
        }
    }

}
