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

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            listBox.Items.Add("KeyDown");
            return;
        }


        private void listBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            listBox.Items.Add("KeyPress");
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            listBox.Items.Add("KeyUp");
        }


        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            return;
        }

    }
}
