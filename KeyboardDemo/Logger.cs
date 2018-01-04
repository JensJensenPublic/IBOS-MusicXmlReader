using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KeyboardTest;

namespace KeyboardDemo
{
    static class Logger
    {
        static private ITestStepReporter testStepreporter;
        static public ITestStepReporter TestStepreporter { set {  testStepreporter = value; } }

        static public void LogKeyDown(string sender, KeyEventArgs e, bool formKeyPreview)
        {
            Log(sender, false, e);
            UpdateDown(e,formKeyPreview);
        }

        static public void LogKeyUp(string sender, KeyEventArgs e, bool formKeyPreview)
        {
            Log(sender, true, e);
            UpdateUp(e,formKeyPreview);
        }


        private static void WriteLine(string s)
        {
            if (null == testStepreporter)
            {
                Console.WriteLine(s);
            }
            else
            {
                testStepreporter.Report(s);
            }
        }


        static private void Log(string sender, bool up, KeyEventArgs e)
        {

            string s = string.Format("{0}: {1} KeyData={2}", sender, up ? "KeyUp   " : "KeyDown ", Utilities.KeysToString(e.KeyData));
            WriteLine(s);
        }


        /// <summary>
        /// Primitive translation of some control characters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static private string CharToString(char c)
        {
            if (c > 0x20) return c.ToString();
            switch ((int) c)
            {
                //case 0x01: return "STRTTOFHEADING";
                case 0x08: return "BACKSPACE";
                case 0x20: return "SPACE";
                case 0x0A: return "LINEFEED";
                case 0x0D: return "RETURN";
                case 0x18: return "CANCEL";
                case 0x1B: return "ESC";
                default: return string.Format("{0:X}", (int)c);
            }
        }



        static public void Log(string sender, KeyPressEventArgs e)
        {

            char c = (char)e.KeyChar;
            string cString = CharToString(c);
            string s = string.Format("{0}: {1} KeyChar='{2}' Value={3} =0x{3:X} ", sender, "KeyPress", cString,(int)c);
            WriteLine(s);
        }


        static public void Log(string sender, TreeViewEventArgs e)
        {
            string s = string.Format("{0}: {1}", sender, "AfterClick");
            WriteLine(s);
        }

        static public void Log(string s)
        {
            WriteLine(s);
        }



        static private List<Keys> keysDown = new List<Keys>();
        static private List<Keys> formPreviewKeysDown = new List<Keys>();
        static private string delimiter = "------------------------------------";

        static private void UpdateUp( KeyEventArgs e,bool formKeyPreview)
        {
            // e.KeyCode represents the value of the key just pressed, not including any previous "Control" "Shift" etc
            List<Keys> keyList = formKeyPreview ? formPreviewKeysDown : keysDown;
            keyList.Remove(e.KeyCode);
            if ((0 == keysDown.Count) && (0 == formPreviewKeysDown.Count))
            {
                WriteLine(delimiter);
            }
        }

        static private void UpdateDown(KeyEventArgs e,bool formKeyPreview)
        {
            List<Keys> keyList = formKeyPreview ? formPreviewKeysDown : keysDown;
            if (!keyList.Contains(e.KeyCode))
            {
                keyList.Add(e.KeyCode);
            }
        }


}
}
