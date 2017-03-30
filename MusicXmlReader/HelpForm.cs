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
    /// Simple form for showing help information for keyboard shortcuts.
    /// A MessageBox will not do the job because it is not possible to navigate a MessageBox via JAWS.
    /// </summary>
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            UserInit();
        }

        private void UserInit()
        { 
            this.Text = ResourcesForUI.MainForm_ApplicationName + ":   " + ResourcesForHelp.Shortcut_MenuCaption;
            ShortcutHelp shortcutHelp = ShortcutHelp.Create();
            List<string> strings = shortcutHelp.ToStrings();
            foreach (string s in strings)
            {
                listBoxHelp.Items.Add(s);
            }

        }

        private void HelpForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void listBoxHelp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
