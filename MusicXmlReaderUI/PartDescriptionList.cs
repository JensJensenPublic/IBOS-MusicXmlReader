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
                    currentPartId = (o as PartElement).PartId;
                    parts.Add(currentPart);
                }

                else if (o is NoteElement)
                {
                    currentPart.Add(o as NoteElement);
                }

                else if (o is HarmonyElement)
                {
                    currentPart.Add(o as HarmonyElement);
                }


                else if (o is BackupElement)
                {
                    currentPart.Add(o as BackupElement);
                }

                else if (o is ForwardElement)
                {
                    currentPart.Add(o as ForwardElement);
                }

                else if (o is MeasureElement)
                {
                    currentPart.Add(o as MeasureElement);
                }

                else if (o is SoundElement)
                {
                    currentPart.Add(o as SoundElement);
                }
                else if (o is ClefElement)
                {
                    currentPart.Add(o as ClefElement);
                }
                else if (o is KeyElement)
                {
                    currentPart.Add(o as KeyElement);
                }
                else if (o is TimeElement)
                {
                    currentPart.Add(o as TimeElement);
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
                    Model.Log(string.Format("PartDescriptionList: Unexpected object of type {0}: String='{1}'", typeAsString, o.ToString()));
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
