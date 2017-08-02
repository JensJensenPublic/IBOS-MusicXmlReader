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

        static private string className = "JfwApiWrapper";
        static public JfwApiWrapper Create(IScreenReaderAPILogger logger)
        {
            string methodName = "Create";
            bool found = false;
            try
            {
                // JAWS does not support an explicit "TestIfRunning", so we must use an other command.   
                // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
                string sayStringFunction = string.Format("SayString(\"{0}\")", "");
                found = NativeMethods.JfwApiJFWRunFunction(sayStringFunction);
            }
            catch (Exception e)
            {
                LogException(logger, className, methodName, "JFWRunFunction", e.Message);
            }
            return found ? new JfwApiWrapper(logger) : null;
        }

        // Prevent construction
        private JfwApiWrapper()
        {
        }

        private JfwApiWrapper(IScreenReaderAPILogger logger) : base(logger)
        {
        }

        protected override bool SpeakImplementation(string s)
        {
            // return JFWSayString(s);
            // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
            string sayStringFunction = string.Format("SayString(\"{0}\")", s);
            return NativeMethods.JfwApiJFWRunFunction(sayStringFunction);
        }

        protected override bool BrailleImplementation(string s)
        {
            string brailleStringFunction = string.Format("BrailleString(\"{0}\")", s);
            return NativeMethods.JfwApiJFWRunFunction(brailleStringFunction);
        }

        protected override bool SilenceImplementation()
        {
            return NativeMethods.JfwApiJFWStopSpeech();
        }

        protected override string GetScreenReaderNameImplementation()
        {
            return "JAWS";
        }

        protected override string GetScreenReaderDllNameImplementation()
        {
            return "jfwapi.dll";
        }

        protected override ScreenReaderType GetScreenReaderTypeImplementation()
        {
            return ScreenReaderType.JAWS;
        }
                
    }
}
