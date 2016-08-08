using System;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{
    class FSAPIWrapper : ScreenReaderAPI
    {

        static public FSAPIWrapper Create()
        {
            bool found = false;
            try
            {
                // JAWS does not support an explicit "TestIfRunning", so we must use an other command.   
                // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
                string sayStringFunction = string.Format("SayString(\"{0}\")", "");
                found = JFWRunFunction(sayStringFunction);
            }
            catch (Exception)
            {
            }
            return found ? new FSAPIWrapper() : null;
        }

        // Prevent construction
        private FSAPIWrapper()
        {
        }

        public override bool Speak(string s)
        {
            // return JFWSayString(s);
            // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
            string sayStringFunction = string.Format("SayString(\"{0}\")",s);
            return JFWRunFunction(sayStringFunction);           
        }

        public override bool Braille(string s)
        {
            latestMessage = s;
            string brailleStringFunction = string.Format("BrailleString(\"{0}\")",s);
            return JFWRunFunction(brailleStringFunction);
        }

        public override bool Silence()
        {
            return JFWStopSpeech();
        }

        public override string GetScreenReaderName()
        {
            return "JAWS";
        }

        public override string GetScreenReaderDllName()
        {
            return "fsapi.dll";
        }

        [DllImport("fsapi.dll", CharSet = CharSet.Ansi)]
        public static extern bool JFWStopSpeech();

        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]            // Says the string, then crashes with an unbalanced stack
        public static extern bool JFWSayString(String text);

        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]       
        public static extern bool JFWRunFunction(String text);

        //[DllImport("fsapi.dll", CharSet = CharSet.Unicode)]   
        //public static extern bool JFWRunFunction(String function, String param1); // Probably not needed !

    }
}
