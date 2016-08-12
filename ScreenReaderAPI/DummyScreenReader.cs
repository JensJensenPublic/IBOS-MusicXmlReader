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

        protected override bool SpeakImplementation(string s)
        {
            return true;
        }

        protected override bool BrailleImplementation(string s)
        {
            return true;
        }

        protected override bool SilenceImplementation()
        {
            return true;
        }

        protected override string GetScreenReaderNameImplementation()
        {
            return "DummyScreenReader";
        }

        protected override string GetScreenReaderDllNameImplementation()
        {
            return "";
        }

        protected override ScreenReader GetScreenReader()
        {
            return ScreenReader.Dummy;
        }

    }
}
