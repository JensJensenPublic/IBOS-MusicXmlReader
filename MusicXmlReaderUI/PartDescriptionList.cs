using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MusicXmlReaderUI
{

    public class PartDescriptionList
    {

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private PartDescriptionList()
        {
        }

        // The list of partitions,each containing a list of elements        
        public List<List<Element>> parts;

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private PartDescriptionList(List<MusicXmlObject> xmlObjects, int numberOfParts)
        {
            parts = new List<List<Element>>(numberOfParts);
            List<Element> currentPart = null; 
            string currentPartId  = "";
            foreach (Object o in xmlObjects)
            {
                Element e = o as Element;
                if (e is PartElement)
                {
                    // Create the next partition
                    currentPart = new List<Element>();
                    currentPartId = (e as PartElement).PartId;
                    parts.Add(currentPart);
                }

                else if 
                (
                (e is NoteElement)  
                || (e is HarmonyElement)
                || (e is BackupElement)      // Er ikke et EventElement
                || (e is ForwardElement)     // Er ikke et EventElement
                || (e is MeasureElement)
                || (e is SoundElement)
                || (e is ClefElement)
                || (e is KeyElement)
                || (e is TimeElement)
                || (e is RepeatElement)
                )
                {
                    // All of these elements are related to events and timing and must be reflected in in the EventDescriptionList.
                    // So they are transferred through the following lists:
                    // List<MusicXmlObject> -> PartDescriptionList -> TimeDescriptionList -> EventDescriptionList
                    currentPart.Add(e);
                }

                else if
                (   (o is ScorePartwiseElement)
                ||  (o is SimpleTextElement)
                ||  (o is PartlistElement)
                ||  (o is DivisionsElement) 
                ||  (o is CreatorElement)
                )
                {
                    // These types are explicitly ignored because they are related to the whole score, not to an event.
                }

                else
                {
                    Type type = o.GetType();
                    string typeAsString = type.ToString();
                    Logger.Log(string.Format("PartDescriptionList: Unexpected object of type {0}: String='{1}'", typeAsString, o.ToString()));
                }

            }

            // Only for inspection during debugging:
            foreach (List<Element> elements in parts)
            {
                int numberOfElements = elements.Count;
            }
            
        }

        public static PartDescriptionList Create(List<MusicXmlObject> xmlObjects, int numberOfParts)
        {
            return new PartDescriptionList(xmlObjects, numberOfParts);
        }


        public int NumberOfParts
        {
            get
            {
                return parts.Count;
            }
        }
    }
}
