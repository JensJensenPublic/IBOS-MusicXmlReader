using MusicXmlReaderModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicXmlReader
{
    /// <summary>
    /// Simple form for asking asking an initial YES/NO question with a "DontShowAgain" message
    /// </summary>
    public partial class MessageForm : Form
    {
        /// <summary>
        /// The value of the "Don't show again" checkbox when the form was closed.
        /// </summary>
        public bool DontShowAgain { get { return checkBoxDontShowAgain.Checked; } }

        private MessageForm()
        { }

        public MessageForm(string caption,string question)
        {
            InitializeComponent();         
            this.checkBoxDontShowAgain.Text = ResourcesForUI.CheckBox_Caption_DoNotShowThisAgain;
            this.buttonNo.Text = ResourcesForUI.Button_Text_No;
            this.buttonYes.Text = ResourcesForUI.Button_Text_Yes;
            this.Text = caption;
            this.labelQuestion.Text = question;         
            this.labelQuestion.Focus();
            this.labelQuestion.Select();
        }

        private void checkBoxDontShowAgain_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log(string.Format(": CheckBoxDontShowAgain.Checked={0}", checkBoxDontShowAgain.Checked));
        }
    }
}
