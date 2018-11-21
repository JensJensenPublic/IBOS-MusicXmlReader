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

        public static void Beep()
        {
            Utilities.Beep(); // Implementation moved to Utilities
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

        public static void LogControlPositions(Control.ControlCollection controls)
        {
            string functionName = "LogControlPositions";
            Control lowestControl = null;
            foreach (Control control in controls)
            {
                //Log(control, functionName + ".Loop");
                if ((null == lowestControl) || (GetBottom(control) > GetBottom(lowestControl)))
                {
                    lowestControl = control;
                }
                //LogControlPositions(control.Controls);
            }

            if (null != lowestControl)
            {
                Log(lowestControl, functionName + ".Result");
            }

        }

        static void Log(Control control, string functionName)
        {
            Logger.Log(string.Format("{0}.{1} Name={2} Text={3} Bottom={4}", className, functionName, control.Name, control.Text,GetBottom(control)));
        }

        static int GetBottom(Control control)
        {
            int top = control.Location.Y;
            int height = control.Height;
            return top + height;
        }


        public static void LogSystemInformation()
        {
            Logger.Log(string.Format("Executing Assembly='{0}'", System.Reflection.Assembly.GetExecutingAssembly()));
            Logger.Log(string.Format("ComputerName={0} UserName={1} UserDomainName={2}",
                SystemInformation.ComputerName, SystemInformation.UserName, SystemInformation.UserDomainName));
            Logger.Log(string.Format("OSVersion={0} ProcessorCount={1} Is64BitOperatingSystem={2} Is64BitProcess={3}",
            System.Environment.OSVersion, System.Environment.ProcessorCount, System.Environment.Is64BitOperatingSystem, System.Environment.Is64BitProcess));
        }


        /// <summary>
        /// Log information and implement a temporary mechanism for overwriting the locale on the machine
        /// by placing a simple textfile in the executing directory
        /// </summary>
        public static void LogGLobalisationInformation()
        {
            try
            {
                string currentCultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;
                Logger.Log(string.Format("CultureInfo.CurrentUICulture.Name={0} ResourceFile={1}", currentCultureName, ResourcesForUI.ResourceFileName));
                string LanguageFileName = (System.IO.Path.Combine(System.Environment.CurrentDirectory, "Language.txt"));
                if (System.IO.File.Exists(LanguageFileName))
                {
                    string newCultureName = System.IO.File.ReadAllText(LanguageFileName);
                    Logger.Log(string.Format("Changing UICulture for UI thread to {0}", newCultureName));
                    System.Threading.Thread thisThread = System.Threading.Thread.CurrentThread;
                    thisThread.CurrentUICulture = new System.Globalization.CultureInfo(newCultureName);
                    Logger.Log(string.Format("thisThread.CurrentUICulture={0}", thisThread.CurrentUICulture.Name));
                }

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("LogGLobalisationInformation threw an exception. Message={0}", e.Message));
            }

        }

        /// <summary>
        /// Assume that a string contains Braille if it is not empty and the first char is a Braille char
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool isBraille(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            char c = s[0];
            return ((0x2800 <= c) && (c <= 0x28ff));
        }

        /// <summary>
        /// Defines the contents of the title-line
        /// </summary>
        /// <returns></returns>
        public static string GetTitleInfo(string applicationName,Model model)
        {
            string result = string.Format("{0}  {1}  {2}"
                                            , applicationName // 0
                                            , model.MetaInformation.FileName // 1
                                            , model.MetaInformation.MovementTitle // 2
                                            );
            return result;
        }

        /// <summary>
        /// Defines the (initial) contents of the status line
        /// </summary>
        /// <returns></returns>
        public static string GetStatusFromMetaInformation(Model model)
        {
            string result = string.Format("{0}   {1}   {2}   {3}   {4}   {5}   {6}   {7}"
                                            , "" // 0 No need to repeat the application name here !
                                            , model.MetaInformation.FileName // 1
                                            , model.MetaInformation.MovementTitle // 2
                                            , model.MetaInformation.MovementNumber // 3
                                            , model.MetaInformation.Work // 4
                                            , model.MetaInformation.Source // 5 
                                            , model.MetaInformation.Creator // 6
                                            , model.MetaInformation.Encoding // 7
                                         );
            return result;
        }



    }
}
