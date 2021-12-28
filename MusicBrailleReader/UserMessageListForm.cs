using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MusicBrailleReader
{

    /// <summary>
    /// Simple replacement for Forms.MessageBox allowing better control of formatting etc.
    /// </summary>
    public partial class UserMessageListForm : Form
    {
        public UserMessageListForm(string caption, List<string> messages)
        {
            InitializeComponent();
            this.Text = caption;
            int maxMessageLength = 0;
            foreach (string message in messages)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    this.listBox.Items.Add(message);
                    maxMessageLength = Math.Max(maxMessageLength, message.Length);
                }
            }
            this.Width = 1000;
            this.Height = 300;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
