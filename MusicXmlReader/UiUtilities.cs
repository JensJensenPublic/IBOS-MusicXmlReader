using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicXmlReader
{
    /// <summary>
    /// Class for static utility functions, only used by the UI and using dependencies of System.Windows.Forms;
    /// </summary>
    public static class UiUtilities
    {

        /// <summary>
        /// Converts a Windows.Forms.Keys to a string.
        /// Faces the problem that the standard Keys.ToString() converts digits to "D0" to "D9" not "0" to "9"
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string KeysToString(Keys keys)
        {
            if (Keys.None == keys) return "";
#warning ToDo move to ResourcesForUi ?
#warning ToDo Also use from localization of menues to generate AccessibleNAme 
            string control = ResourcesForHelp.Shortcut_Key_Control;
            string alt = ResourcesForHelp.shortcut_Key_alt;
            string shift = ResourcesForHelp.shortcut_Key_shift;
            string controlString = (0 != (keys & Keys.Control)) ? control + "+" : "";
            string altString = (0 != (keys & Keys.Alt)) ? alt + "+" : "";
            string shiftString = (0 != (keys & Keys.Shift)) ? shift + "+" : "";
            Keys simpleKey = keys & ~(Keys.Control | Keys.Alt | Keys.Shift);
            string charString = simpleKey.ToString(); // Will generate "D0" to "D9" for the digits !
            if ((simpleKey >= Keys.D0) && (simpleKey <= Keys.D9))
            {
                char c = (char)('0' + (char)(simpleKey - Keys.D0));
                charString = c.ToString();
            }
            return controlString + altString + shiftString + charString;
        }

    }
}
