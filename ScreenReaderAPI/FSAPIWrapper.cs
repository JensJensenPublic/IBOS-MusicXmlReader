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

        static private string className = "FSAPIWrapper";
        static public FSAPIWrapper Create(IScreenReaderAPILogger logger)
        {
            bool found = false;
            string methodName = "Create";
            try
            {
                // throw new Exception("For testing logging of exceptions only !");
                // JAWS does not support an explicit "TestIfRunning", so we must use an other command.   
                // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
                string sayStringFunction = string.Format("SayString(\"{0}\")", "");
                found = NativeMethods.FsApiJFWRunFunction(sayStringFunction);
            }
            catch (Exception e)
            {
                LogException(logger, className, methodName, "JFWRunFunction", e.Message);
            }
            return found ? new FSAPIWrapper(logger) : null;
        }

        // Prevent construction
        private FSAPIWrapper()
        {
        }
        

        // Prevent construction
        protected FSAPIWrapper(IScreenReaderAPILogger logger) : base(logger)
        {
        }

        protected override bool SpeakImplementation(string s)
        {
            // return JFWSayString(s);
            // We might use JFWSayString here, but the debugger reposts that it unbalances the stack, so we use the general JFWRunFunction
            string sayStringFunction = string.Format("SayString(\"{0}\")",s);
            return NativeMethods.FsApiJFWRunFunction(sayStringFunction);           
        }

        protected override bool BrailleImplementation(string s)
        {
            bool result;
            string brailleStringFunction = string.Format("BrailleString(\"{0}\")",s);
            result = NativeMethods.FsApiJFWRunFunction(brailleStringFunction);
            if (result)
            {
                // Experimental code!
                //result = JFWRunFunction("BrailleLine(0)");
            }
            return result;
        }

        protected override bool SilenceImplementation()
        {
            return NativeMethods.FsApiJFWStopSpeech();
        }

        protected override string GetScreenReaderNameImplementation()
        {
            return "JAWS";
        }

        protected override string GetScreenReaderDllNameImplementation()
        {
            return "fsapi.dll";
        }

        protected override ScreenReaderType GetScreenReaderTypeImplementation()
        {
            return ScreenReaderType.JAWS;
        }
        
  
    }
}
