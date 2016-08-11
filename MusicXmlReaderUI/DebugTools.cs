using System;
using JSJ.ScreenReaderAPI;

namespace MusicXmlReaderUI
{
    class DebugTools : IScreenReaderAPILogger
    {
        public bool LogEvent(string s)
        {
            Model.Log(s);
            return true;
        }

        public bool TraceLine(string s)
        {
            Model.Trace(s);
            return true;
        }

        public bool TraceChar(char c)
        {
            Model.Trace(c.ToString());
            return true;
        }

        public static DebugTools Create()
        {
            return new DebugTools();
        }

        // Prevent construction
        private DebugTools()
        { }

    }
}
