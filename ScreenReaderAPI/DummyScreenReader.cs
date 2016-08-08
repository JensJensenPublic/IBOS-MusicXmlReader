using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSJ.ScreenReaderAPI
{

    /// <summary>
    /// Created when no screenreader is running in order to keep application code simple.
    /// </summary>
    class DummyScreenReader : ScreenReaderAPI
    {

        static public DummyScreenReader Create()
        {
            return new DummyScreenReader();
        }

        // Prevent construction
        private DummyScreenReader()
        {
        }

        public override bool Speak(string s)
        {
            return true;
        }

        public override bool Braille(string s)
        {
            return true;
        }

        public override bool Silence()
        {
            return true;
        }

        public override string GetScreenReaderName()
        {
            return "";
        }

        public override string GetScreenReaderDllName()
        {
            return "";
        }
    }
}
