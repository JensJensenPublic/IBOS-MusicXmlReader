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
                if (o is PartElement)
                {
                    // Create the next partition
                    currentPart = new List<Element>();
                    currentPartId   = (o as PartElement).PartId;
                    parts.Add(currentPart);
                }

                if (o is NoteElement)
                {
                    currentPart.Add(o as NoteElement);
                }

                if (o is HarmonyElement)
                {
                    currentPart.Add(o as HarmonyElement);
                }


                if (o is BackupElement)
                {
                    currentPart.Add(o as BackupElement);
                }

                if (o is ForwardElement)
                {
                    currentPart.Add(o as ForwardElement);
                }

                if (o is MeasureElement)
                {
                    currentPart.Add(o as MeasureElement);
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
