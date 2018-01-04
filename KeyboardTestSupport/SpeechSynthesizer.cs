using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.ScreenReaderAPI;

namespace KeyboardTest
{
    public class SpeechSynthesizer : IScreenReaderAPILogger
    {
        private ScreenReaderAPI screenReaderAPI;

        // Implement IScreenReaderAPILogger
        public bool LogEvent(string s)
        {
            Console.WriteLine(s);
            return true;
        }
        public bool TraceLine(string s)
        {
            Console.WriteLine(s);
            return true;
        }

        public bool TraceChar(char c)
        {
            Console.WriteLine(c.ToString());
            return true;
        }


        private SpeechSynthesizer()
        {
            screenReaderAPI = ScreenReaderAPI.Create(false, this); // fsapi.dll

        }

        public void Speak(string s)
        {
            screenReaderAPI.Speak(s);
        }


        public static SpeechSynthesizer Create()
        {
            return new SpeechSynthesizer();
        }
    }
}
