using System.Xml;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// This Element is NOT created directly from an element in the MusicXml file, 
    /// but is an artificial marker used to represent the end of another Element,
    /// such as a NoteElement or a HarmonyElement
    /// </summary>
    public class EndEventElement : EventElement
    {

        /// <summary>
        /// The EventElement to which this EndElement is related
        /// </summary>
        EventElement startElement;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private EndEventElement()
        {
        }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="startElement">The EventElement to which this EndElement is related</param>
        /// <param name="time">The time for this event</param>
        private EndEventElement(EventElement startElement, int time)
        {
            this.startElement = startElement;
            this.startTime = time;
        }

        public EventElement StartElement
        {
            get
            {
                return startElement;
            }
        }

        public static EndEventElement Create(EventElement startElement, int time)
        {
            return new EndEventElement(startElement,time);
        }

        public override string ToString()
        {
            return string.Format("EndEventElement");
        }
    }
}

