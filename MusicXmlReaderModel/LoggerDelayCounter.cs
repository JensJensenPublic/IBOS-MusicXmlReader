using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    // Note: this class has been moved to the MusicXmlReaderModelBase project (namespace = MusicXmlReaderModel) in order to avoid circular references!
#if false
    public class LoggerDelayCounter
    {
        TimeSpan executionTime;
        public TimeSpan ExecutionTime { get { return executionTime; } }
        DateTime startTime;

        public void Start()
        {
            startTime = DateTime.Now;
        }

        public void Stop()
        {
            DateTime now = DateTime.Now;
            executionTime = executionTime.Add(now - startTime);    
        }

        public override string ToString()
        {
            return executionTime.ToString();
        }

        public string ToMs()
        {
            return string.Format("{0}", ((int)executionTime.TotalMilliseconds));
        }

        public string ToSeconds()
        {
            return string.Format("{0}", ((int)executionTime.TotalSeconds));
        }

        private LoggerDelayCounter()
        {
            executionTime = new TimeSpan(0);
        }

        public static LoggerDelayCounter Create()
        {
            return new LoggerDelayCounter();
        }

    }
#endif
}
