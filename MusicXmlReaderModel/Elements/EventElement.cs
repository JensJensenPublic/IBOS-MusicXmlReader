using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// This is a base class for all classes describing events (in time)
    /// These classes must all implement the StartTime member.
    /// </summary>
    public class EventElement : Element
    {
        protected int startTime;

        /// <summary>
        /// Unit is milliSeconds. Is 0 at start of part.
        /// </summary>
        public int StartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                startTime = value;
            }

        }
    }
}
