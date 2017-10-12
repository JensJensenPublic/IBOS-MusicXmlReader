using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardTest
{
    public partial class KeyboardTestForm : Form
    {
        private int keysDown = 0;

        public KeyboardTestForm()
        {
            InitializeComponent();
        }

        //private void KeyboardTestForm_KeyDown(object sender, KeyEventArgs e)
        //{
        //    listBox.Items.Add("KeyDown");
        //    return;
        //}

        //private void KeyboardTestForm_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    listBox.Items.Add("KeyPress");
        //    return;
        //}

        //private void KeyboardTestForm_KeyUp(object sender, KeyEventArgs e)
        //{
        //    listBox.Items.Add("KeyUp");
        //    return;
        //}

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //private void Show(string text)
        //{
        //    listBox.Items.Add(text);
        //}



        private void Show(string line)
        {
            listBox.Items.Add(line);
            listBox.Refresh();
            if (listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1; // Force JAWS to read the last line
            }
        }



        private void Show(string text,KeyEventArgs e)
        {
            string line = string.Format("{0}{1}{2}{3} {4} ",
                text,                           // 0
                e.Alt ? " ALT" : "",            // 1
                e.Control ? " CONTROL" : "",    // 2
                e.Shift ? " SHIFT" : "",        // 3
                e.KeyCode                       // 4
                );
            Show(line);
        }

        private void Show(string text,KeyPressEventArgs e)
        {
            string line = string.Format("{0} Char={1}",
                text, //0
                e.KeyChar);
            Show(line);   
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (0 == keysDown)
            {
                // When the first key in a sequence is pressed we clear the screen
                listBox.Items.Clear();
                listBox.Refresh();
            }
            keysDown++;
            Show("KeyDown",e);
        }

        private void listBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Show("KeyPress",e);
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            keysDown--;
            //Show("KeyUp  ",e);
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            return;
        }

        private void listBox_Click(object sender, EventArgs e)
        {
            keysDown = 0;
            listBox.Items.Clear();
            listBox.Refresh();
        }
    }
}
