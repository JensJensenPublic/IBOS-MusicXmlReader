using System;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// This is a base class for all classes describing events (in time)
    /// These classes must all implement the StartTime member.
    /// </summary>
    public class EventElement : Element
    {
        protected Int64 startTime;
        protected EventDescription owningEventDescription;

        /// <summary>
        /// Unit is milliSeconds. Is 0 at start of part.
        /// </summary>
        public Int64 StartTime
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

        public EventDescription OwningEventDescription
        {
            get
            {
                return owningEventDescription;
            }

            set
            {
                owningEventDescription = value;
            }

        }



    }
}
