using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace NvdaControllerClientSample
{

#if x86
     [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
#else
    [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode)]
#endif
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }
}
