using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace MusicXmlReader
{

    /// <summary>
    /// Contains some (hopefully) obsolete code, attempting to investigate a documented Windows Forms error.
    /// </summary>
    public static class Experiments
    {

        public static void LogRightAlignedMenus(MainForm mainForm)
        {
#if true
            // Investigate The Left/Right problem (Error 296)
            // https://stackoverflow.com/questions/16305454/getting-the-left-and-right-arrow-keys-to-select-the-previous-next-menu-instead-o
            // https://connect.microsoft.com/VisualStudio/feedback/details/786382/menustrip-control-issues-with-rightalignedmenus
            // https://connect.microsoft.com/VisualStudio/feedback/details/796965/menustrip-right-left-arrow-keys-work-reversely-when-a-submenu-is-open-dropped-down
            Logger.Log(string.Format("->SystemInformation.RightAlignedMenus={0}", System.Windows.Forms.SystemInformation.RightAlignedMenus.ToString()));
            Logger.Log(string.Format("->MainMenu.RightToLeft={0}", mainForm.RightToLeft.ToString()));
            Logger.Log(string.Format("->MainMenu.RightToLeftLayout={0}", mainForm.RightToLeftLayout.ToString()));
#warning Remove experiments !!
#endif
            //MenuStripWorkAround(); // Does not solve the problem !! !!
#if false
                if (SystemInformation.RightAlignedMenus)
                {
#warning Find a real solution instead of this terrible hack !
                    Beep();
                    MessageBox.Show(ResourcesForUI.Message_RightAlignedMenus);
                }
#endif 
        }



#if false
        private void MenuStripWorkAround()
        {
            // NO! this does not solve the problem ! It changes the appearance of the dropdowns, but not the behaviour !!!
            // https://connect.microsoft.com/VisualStudio/feedback/details/786382/menustrip-control-issues-with-rightalignedmenus
            // https://connect.microsoft.com/VisualStudio/feedback/details/796965/menustrip-right-left-arrow-keys-work-reversely-when-a-submenu-is-open-dropped-down
            //Begin workaround
            const string functionName = "MenuStripWorkAround";

            // if (SystemInformation.RightAlignedMenus)
            {
                MainMenuStrip.RightToLeft = RightToLeft.Yes;
                foreach (ToolStripMenuItem toolStripMenuItem in this.MenuStrip.Items)
                {                 
                    toolStripMenuItem.RightToLeft = RightToLeft.Yes;
                    Logger.Log(string.Format("{0}.{1} Setting RightToLeft to 'Yes' for {2} ", className, functionName, toolStripMenuItem.Name));
                    foreach (object o in toolStripMenuItem.DropDownItems)
                    {
                        if (o is ToolStripDropDownItem)
                        {
                            ToolStripDropDownItem toolStripDropDownItem = o as ToolStripDropDownItem;
                            Logger.Log(string.Format("{0}.{1} Setting RightToLeft to 'No' for {2} ", className, functionName, toolStripDropDownItem.Name));
                            toolStripDropDownItem.RightToLeft = RightToLeft.No;
                            
                        }
                    } 
                }
            }
        }
#endif








    }
}
