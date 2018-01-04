using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTest
{
    public interface ITestStepReporter
    {
        void Report(string s);
        void Clear();
    }
}
