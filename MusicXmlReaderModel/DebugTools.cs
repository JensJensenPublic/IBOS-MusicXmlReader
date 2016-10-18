using System;
using JSJ.ScreenReaderAPI;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
{
    class DebugTools : IScreenReaderAPILogger
    {
        public bool LogEvent(string s)
        {
            Logger.Log(s);
            return true;
        }

        public bool TraceLine(string s)
        {
            Logger.Trace(s);
            return true;
        }

        public bool TraceChar(char c)
        {
            Logger.Trace(c.ToString());
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
