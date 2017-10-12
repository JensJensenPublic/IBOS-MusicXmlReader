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



        private void Show(ListBox listBox, string line)
        {
            listBox.Items.Add(line);
            listBox.Refresh();
            if (listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1; // Force JAWS to read the last line
            }
        }



        private void Show(string text, KeyEventArgs e)
        {
            string line = string.Format("{0}{1}{2}{3} {4} Value={5}",
                text,                           // 0
                e.Alt ? " ALT" : "",            // 1
                e.Control ? " CONTROL" : "",    // 2
                e.Shift ? " SHIFT" : "",        // 3
                e.KeyCode,                       // 4
                e.KeyValue                      // 5
                );
            Show(listBoxForKeyDown, line);
        }

        private void Show(ListBox listBox, string text, KeyPressEventArgs e)
        {
            int intValue = (int)e.KeyChar;
            Keys keysValue = (Keys)e.KeyChar;
            Char char16 = (Char)intValue;
               string line = string.Format("{0}  Dec={1} Hex={2:X} CHAR16='{3}'",
                text, //0
                intValue, //1
                intValue, //2
                char16) // 3
                ;
            Show(listBox, line);
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (0 == keysDown)
            {
                // When the first key in a sequence is pressed we clear the screen
                listBoxForKeyDown.Items.Clear();
                listBoxForKeyDown.Refresh();
                listBoxForKeyPress.Items.Clear();
                listBoxForKeyPress.Refresh();
            }
            keysDown++;
            Show("KeyDown", e);
        }

        #region keyHandlers
        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(sender, e);
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            Show(listBoxForKeyPress, "KeyPress", e);
        }


        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            keysDown--;
        }

        private void listBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(sender, e);
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(sender, e); 
        }


        private void listBoxForKeyPress_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(sender, e);
        }

        private void listBoxForKeyPress_KeyPress(object sender, KeyPressEventArgs e)
        {
            Show(listBoxForKeyPress, "KeyPress", e);
        }

        private void listBoxForKeyPress_KeyUp(object sender, KeyEventArgs e)
        {
            keysDown--;
        }
        #endregion keyHandlers

        #region IndexChangedHandlers
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            return;
        }

        private void listBoxForKeyPress_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion IndexChangedHandlers

        #region clickHandlers
        private void listBox_Click(object sender, EventArgs e)
        {
            keysDown = 0;
            listBoxForKeyDown.Items.Clear();
            listBoxForKeyDown.Refresh();
        }

        private void listBoxForKeyPress_Click(object sender, EventArgs e)
        {
            listBoxForKeyPress.Items.Clear();
            listBoxForKeyPress.Refresh();
        }
        #endregion clickHandlers

    }
}
