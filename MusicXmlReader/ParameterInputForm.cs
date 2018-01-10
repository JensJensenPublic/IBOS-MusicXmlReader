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
    public partial class ParameterInputForm : Form
    {
        private ParameterDescription parameterDescription;
        public ParameterDescription ParameterDescription { get { return parameterDescription; } set { parameterDescription = value; } }
        public string ComboBoxInput { get { return comboBox1.Text; } }
        public ParameterInputForm()
        {
            InitializeComponent();
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                Console.WriteLine((int) e.KeyChar);
                if (13 == (int)e.KeyChar)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                bool ok = parameterDescription.CheckSyntax(comboBox1.Text + e.KeyChar.ToString());
                if (!ok)
                {
                    System.Media.SystemSounds.Beep.Play();
                    Console.WriteLine("{0} Syntax error!", Name);
                }

            }
        }
    }
}
