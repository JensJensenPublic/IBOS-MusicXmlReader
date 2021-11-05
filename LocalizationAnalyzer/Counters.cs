using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationAnalyzer
{
    class Counters
    {
        public int localizationMethods = 0;
        public int otherMethods = 0;
        public int otherMembers = 0;
        public int runtimeProtertyInfo = 0;
        public int rtFieldInfo = 0;
        private Counters() { }
        static public Counters Create()
        {
            return new Counters();
        }

        public string LogFormat()
        {
            return string.Format("Found LocalizationMethods={0}  OtherMethods={1}  RuntimeProtertyInfo={2} rtFieldInfo={3} OtherMembers={4}", localizationMethods, otherMethods, runtimeProtertyInfo, rtFieldInfo, otherMembers);
        }

    }
}
