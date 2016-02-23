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
        bool showRaw = false;
        Model model;

        string fileName = @"C:\temp\MusicXML\La Mer.xml"; // The sample XML file to read from

        public Form1()
        {
            InitializeComponent();
            if (!showRaw)  listBoxRaw.Hide();
            model = new Model(listBoxFiltered);        
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
            fileName = System.IO.Path.Combine(executingDirectory, "Node.xml");
            XmlDocument doc = new XmlDocument();   
            XmlTextReader reader = new XmlTextReader(fileName);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            doc.Load(reader);
            model.Recurse(doc.ChildNodes);
            model.LoadListBox();   
        }

        private void Play_Click(object sender, EventArgs e)
        {
            model.musicPlayer.StartPlaying();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            model.musicPlayer.StopPlaying();
        }
    }
}
