using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{
    class JfwApiWrapper : ScreenReaderAPI
    {
   
        static public JfwApiWrapper Create()
        {
            bool found = false;
            try
            {
                // JAWS does not support an explicit "TestIfRunning", so we must use an other command.   
                // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
                string sayStringFunction = string.Format("SayString(\"{0}\")", "");
                found = JFWRunFunction(sayStringFunction);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("JFWRunFunction threw an exception.Message: {0}", e.Message));
            }
            return found ? new JfwApiWrapper() : null;
        }

        // Prevent construction
        private JfwApiWrapper()
        {
 
        }

        protected override bool SpeakImplementation(string s)
        {
            // return JFWSayString(s);
            // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
            string sayStringFunction = string.Format("SayString(\"{0}\")", s);
            return JFWRunFunction(sayStringFunction);
        }

        protected override bool BrailleImplementation(string s)
        {
            string brailleStringFunction = string.Format("BrailleString(\"{0}\")", s);
            return JFWRunFunction(brailleStringFunction);
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
            return "jfwapi.dll";
        }


        [DllImport("jfwapi.dll", CharSet = CharSet.Ansi)]
        private static extern bool JFWStopSpeech();           

        [DllImport("jfwapi.dll", CharSet = CharSet.Unicode)] 
        private static extern bool JFWSayString(String text);

        [DllImport("jfwapi.dll", CharSet = CharSet.Unicode)] 
        private static extern bool JFWRunFunction(String text);
                
    }
}
