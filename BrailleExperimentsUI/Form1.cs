using System;
using System.Windows.Forms;
using JSJ.ScreenReaderAPI;

namespace BrailleExperimentsUI
{

    

    public partial class Form1 : Form, IScreenReaderAPILogger
    {

        public bool LogEvent(string s)
        {
            Console.WriteLine(s);
            return false;
        }

        ScreenReaderAPI screenReaderAPI;

        public Form1()
        {
            InitializeComponent();

            screenReaderAPI = ScreenReaderAPI.Create(false, this as IScreenReaderAPILogger);

            ListBoxLeft.Items.Add("a");
            ListBoxLeft.Items.Add("b");
            ListBoxLeft.Items.Add("c");
            ListBoxRight.Items.Add("d");
            ListBoxRight.Items.Add("e");
            ListBoxRight.Items.Add("f");
        }

        private void ListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Stop the Speak and Braille automatically generated behind the scene
            screenReaderAPI.Silence();  
            // Get the contents of the selected item   
            ListBox listBox = (sender as ListBox);
            string s = listBox.Items[listBox.SelectedIndex].ToString();
            // Say something different in order to check if the original text was heard !
            screenReaderAPI.Speak("HEJ");    
            // Braille it
            screenReaderAPI.Braille(s,true);
        }
    }
}
