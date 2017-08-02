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

        static public DummyScreenReader Create(IScreenReaderAPILogger logger)
        {
            return new DummyScreenReader(logger);
        }

        // Prevent construction
        private DummyScreenReader()
        {
        }

   
        private DummyScreenReader(IScreenReaderAPILogger logger) : base(logger)
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

        protected override ScreenReaderType GetScreenReaderTypeImplementation()
        {
            return ScreenReaderType.Dummy;
        }

    }
}
