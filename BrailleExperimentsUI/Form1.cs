using System;
using System.Windows.Forms;

namespace BrailleExperimentsUI
{
    public partial class Form1 : Form
    {
        BrailleExperimentsUIModel model;

        private void ListBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            model.IndexChanged(sender, e);
        }

        private void ListBoxRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            model.IndexChanged(sender, e);
        }

        public Form1()
        {
            InitializeComponent();
            model = new BrailleExperimentsUIModel(this.ListBoxLeft,this.ListBoxRight);
        }
    }
}
