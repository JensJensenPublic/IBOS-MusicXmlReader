using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSJ.ScreenReaderAPI
{
    public interface IScreenReaderAPILogger
    {
        bool LogEvent(string s);
    }
}
