namespace JAWSExperiments
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                listBox1.Items.Add(string.Format("Line {0}", i));
            }
        }
    }
}