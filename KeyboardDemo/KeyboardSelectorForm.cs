using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyboardTest;

namespace KeyboardDemo
{
//    public enum KeyboardNameEnum { Unknown, Focus14, Braillant, Edge, BrailleNoteTouch }


    /// <summary>
    /// Simple form for letting the user selest the Perkins keyboard (using the standard keyboard)
    /// </summary>
    public partial class KeyboardSelectorForm : Form
    {
        private IKeyboardSelectorClient client;
 //       static public KeyboardNameEnum DefaultKeyboard = KeyboardNameEnum.BrailleNoteTouch;
//        static public KeyboardNameEnum DefaultKeyboard = KeyboardNameEnum.Focus14;

        public KeyboardSelectorForm(KeyboardNameEnum keyboardName, IKeyboardSelectorClient client)
        {
            InitializeComponent();
            UserInit(keyboardName, client);
        }

        private KeyboardNameEnum keyboardName;// = KeyboardSelectorForm.DefaultKeyboard; // Chosen as default

        private void UserInit(KeyboardNameEnum keyboardName,IKeyboardSelectorClient client)
        {
            this.keyboardName = keyboardName;
            this.client = client;
            comboBox1.Text = client.ReadKeyboardName().ToString();
            comboBox1.Items.Add(KeyboardNameEnum.Focus14);
            comboBox1.Items.Add(KeyboardNameEnum.Braillant);
            comboBox1.Items.Add(KeyboardNameEnum.HimsEdge);
            comboBox1.Items.Add(KeyboardNameEnum.BrailleNoteTouch);
        }


        /// <summary>
        /// Write user's selection back to client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            keyboardName = (KeyboardNameEnum) comboBox1.SelectedItem;
            client.WriteKeyboardName(keyboardName);
            Console.WriteLine(string.Format("Keyboard={0}", keyboardName));        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.WriteKeyboardName(keyboardName);
            Console.WriteLine(string.Format("Keyboard changed to {0}", keyboardName));
            this.Close();
        }
    }
}
