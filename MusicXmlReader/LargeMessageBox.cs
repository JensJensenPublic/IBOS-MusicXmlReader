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
    public partial class LargeMessageBox : Form
    {
        public LargeMessageBox()
        {
            InitializeComponent();
        }

        public LargeMessageBox(string caption, string message)
        {
            InitializeComponent();
            this.AutoSize = true;
            this.Text = caption; 
            this.label1.Text = message; 
        }
    }
}
