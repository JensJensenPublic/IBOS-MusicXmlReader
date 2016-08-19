using System;
using System.Windows.Forms;

namespace BrailleExperimentsUI
{
    public partial class Form1 : Form
    {
        BrailleExperimentsUIModel model;

        public Form1()
        {
            InitializeComponent();
            model = new BrailleExperimentsUIModel(this.ListBoxLeft, this.ListBoxRight, this.textBoxBraille, this.checkBoxBrailleSelector);
        }


        private void ListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            model.MusicBrailleListBoxIndexChanged();
        }

        private void ListBoxRight_SelectedIndexChanged(object sender, EventArgs e)
        {               
            model.TextBrailleListBoxIndexChanged();
        }

        private void ListBoxLeft_Leave(object sender, EventArgs e)
        {
            model.FocusLost();
        }

        private void ListBoxRight_Leave(object sender, EventArgs e)
        {
            model.FocusLost();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            model.RenderAsMusicBrailleChanged(checkBoxBrailleSelector.Checked);
        }

        private void textBoxBraille_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
