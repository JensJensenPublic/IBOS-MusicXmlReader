using System.Text;

namespace JAWSExperiments
{
    public partial class Form1 : Form
    {
        string longText; 

        public Form1()
        {
            InitializeComponent();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                sb.Append("0123456789");
            }
            longText = sb.ToString();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10000; i++)
            {
                listBox1.Items.Add(string.Format("Line {0} {1}", i,longText.Substring(0,500)));
            }
        }
    }
}