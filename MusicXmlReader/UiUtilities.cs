using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// Class for static utility functions, only used by the UI and using dependencies of System.Windows.Forms;
    /// </summary>
    public static class UiUtilities
    {
        static string className = "UiUtilities";

        // Tempo can be changed between 10% and 1000% of the tempo specified in the MusicXml File
        public const int TempoFactorMinimum = 10;
        public const int TempoFactorMaximum = 1000;

        /// <summary>
        /// Assure common implementation of Beep();
        /// </summary>
        public static void Beep()
        {
            System.Media.SystemSounds.Beep.Play();
        }


        private const Keys controlMask =  (Keys.Control | Keys.Alt | Keys.Shift);

        private static Keys GetSimpleKeys(Keys keys)
        {
            return keys & ~controlMask;
        }

        private static Keys GetControlKeys(Keys keys)
        {
            return keys & controlMask;
        }

        public static string Plus(string s1, string s2)
        {
            string plus = (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) ? "" : "+";
            return s1 + plus + s2;
        }
        
        public static string Plus(string s1, string s2, string s3)
        {
            //return s1 + "+" + s2 + "+" + s3;
            return Plus(Plus(s1, s2), s3);
        }
        
        public static string LocalizeControlKey(Keys keys)
        {
            Keys controlKeys = GetControlKeys(keys);
            string control = ResourcesForHelp.Shortcut_Key_Control;
            string alt = ResourcesForHelp.shortcut_Key_alt;
            string shift = ResourcesForHelp.shortcut_Key_shift;
            string controlString = (0 != (keys & Keys.Control)) ? control : "";
            string altString = (0 != (keys & Keys.Alt)) ? alt : "";
            string shiftString = (0 != (keys & Keys.Shift)) ? shift : "";
            return Plus(controlString,altString,shiftString);
        }

        public static string LocalizeSimpleKey(Keys keys)
        {
            Keys simpleKeys = GetSimpleKeys(keys);
            switch (simpleKeys)
            {
                case Keys.Space:    return ResourcesForHelp.Shortcut_Key_space;
                case Keys.Escape:   return ResourcesForHelp.Shortcut_Key_escape;
                case Keys.Enter:    return ResourcesForHelp.Shortcut_Key_enter;
                case Keys.Insert:   return ResourcesForHelp.Shortcut_Key_insert;
                case Keys.Tab:      return ResourcesForHelp.shortcut_Key_tab;
                case Keys.PageUp:   return ResourcesForHelp.Shortcut_Key_pageUp;
                case Keys.PageDown: return ResourcesForHelp.Shortcut_Key_pageDown;
                case Keys.Left:     return ResourcesForHelp.Shortcut_ArrowLeft;
                case Keys.Right:    return ResourcesForHelp.Shortcut_ArrowRight;
                case Keys.Up:       return ResourcesForHelp.Shortcut_ArrowUp;
                case Keys.Down:     return ResourcesForHelp.Shortcut_ArrowDown;
                case Keys.Home:     return ResourcesForHelp.Shortcut_Home;
                case Keys.End:      return ResourcesForHelp.Shortcut_End;
                case Keys.Multiply: return ResourcesForHelp.Shortcut_Asterisk;
                case Keys.None:     return "";
                default:
                    return simpleKeys.ToString();
            }
        }



        /// <summary>
        /// Converts a Windows.Forms.Keys to a string.
        /// Faces the problem that the standard Keys.ToString() converts digits to "D0" to "D9" not "0" to "9"
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string KeysToString(Keys keys)
        {
            if (Keys.None == keys) return "";

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

        /// <summary>
        /// Generate audible beep when user attends to move outside listbox
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="move"></param>
        public static void WarnAtEnd(ListBox listBox, int move)
        {
            string functionName = "WarnAtEnd";
            try
            {
                int newindex = listBox.SelectedIndex + move;
                int firstIndex = 0;
                int lastIndex = listBox.Items.Count - 1;
                if (((newindex < firstIndex) || (newindex > lastIndex)) && (listBox.SelectedIndex != -1)) // Only warn when selected.
                {
                    System.Media.SystemSounds.Beep.Play();
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} Exception.Message={2}", className, functionName, e.Message));
            }
        }


        public static void Hand()
        {
            System.Media.SystemSounds.Hand.Play();
        }


    }
}
