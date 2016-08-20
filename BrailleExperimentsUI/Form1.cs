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
            model = new BrailleExperimentsUIModel(this.ListBoxRight, this.textBoxBraille);
        }

        private void ListBoxRight_SelectedIndexChanged(object sender, EventArgs e)
        {               
            model.TextBrailleListBoxIndexChanged();
        }

        private void ListBoxRight_Leave(object sender, EventArgs e)
        {
            model.FocusLost();
        }

        private void textBoxBraille_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
