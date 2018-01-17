using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    public partial class ParameterInputForm : Form
    {
        private string className = "ParameterInputForm";
        private ParameterDescription parameterDescription;
        public ParameterDescription ParameterDescription
        {
            get
            {
                return parameterDescription;
            }
            set
            {
                parameterDescription = value;
                this.Name = parameterDescription.Name; // parameterDescription.Name must contain the localized name of the form !!!
            }
        }

        public string ComboBoxInput { get { return comboBox1.Text; } }
        public ParameterInputForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Handle input by looking at the KeyPress event.
        /// Perkins-keyboards typically do not generate usable KeyDown events !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // const string functionName = "comboBox1_KeyPress";
            const char RETURN = (char)13;
            const char ESC = (char)27;
            switch (e.KeyChar)
            {
                case RETURN:
                    this.DialogResult = DialogResult.OK;
                    e.Handled = true;   // Prevent "Ding" when closing form with valid value                 
                    this.Close();
                    break;
                case ESC:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                default:
                    break;
            }

            // Check the syntax of the resulting input and report any error by a beep
            string input = comboBox1.Text + e.KeyChar.ToString();
            bool ok = parameterDescription.CheckSyntax(input);
            if (!ok)
            {
                System.Media.SystemSounds.Beep.Play();
                // Logger.Log(string.Format("{0}.{1}: Name={2} Input={3} Syntax error!", className, functionName, Name, input));
            }
        }
    }
}
