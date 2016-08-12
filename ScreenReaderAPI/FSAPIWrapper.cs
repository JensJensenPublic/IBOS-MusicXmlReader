using System;
using System.Runtime.InteropServices;

// C:\ProgramData\Freedom Scientific\JAWS\17.0\Scripts\hjconst.jsh Contains the following constants for use with the BrailleLine function
//  brlShowFirstRange = 0, ; show the first display worth of data
//  brlShowSameRange = 1, ; attempt to align to same relative offset in data as prior show
//  brlShowLastRange=2, ; show the last range of the data
//  brlShowRangeContainingCursor = 3, ; show the range containing the cursor acording to the autoPanMode.

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

        protected override bool SpeakImplementation(string s)
        {
            // return JFWSayString(s);
            // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
            string sayStringFunction = string.Format("SayString(\"{0}\")",s);
            return JFWRunFunction(sayStringFunction);           
        }

        protected override bool BrailleImplementation(string s)
        {
            bool result;
            string brailleStringFunction = string.Format("BrailleString(\"{0}\")",s);
            result = JFWRunFunction(brailleStringFunction);
            if (result)
            {
                // Experimental code!
                //result = JFWRunFunction("BrailleLine(0)");
            }
            return result;
        }

        protected override bool SilenceImplementation()
        {
            return JFWStopSpeech();
        }

        protected override string GetScreenReaderNameImplementation()
        {
            return "JAWS";
        }

        protected override string GetScreenReaderDllNameImplementation()
        {
            return "fsapi.dll";
        }

        protected override ScreenReader GetScreenReader()
        {
            return ScreenReader.JAWS;
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
