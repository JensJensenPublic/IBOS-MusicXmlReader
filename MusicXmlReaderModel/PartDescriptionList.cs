using System;
using System.Collections.Generic;

namespace MusicXmlReaderModel
{
    public class PartDescription
    {
        private string id = "";
        public string Id { get { return id; }  }
        private List<Element> elements;
        public List<Element> Elements { get { return elements; } }
        private PartDescription() { }
        private PartDescription(string id)
        {
            this.id = id;
            this.elements = new List<Element>();
        }
        public static PartDescription Create(string id)
        {
            return new PartDescription(id);
        }
    }


    public class PartDescriptionList
    {

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private PartDescriptionList()
        {
        }

        // The list of parts, each containing a list of elements 
        public List<PartDescription> parts;

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private PartDescriptionList(List<MusicXmlObject> xmlObjects, int numberOfParts)
        {
            parts = new List<PartDescription>(numberOfParts);
            PartDescription currentPart = null; 
            string currentPartId  = "";
            foreach (Object o in xmlObjects)
            {
                Element e = o as Element;
                if (e is PartElement)
                {
                    // Create the next part               
                    currentPartId = (e as PartElement).PartId;
                    currentPart = PartDescription.Create(currentPartId);
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
                || (e is BarlineElement)
                || (e is InstrumentsElement)
                || (e is AttributesElement)
                || (e is DirectionElement)
                || (e is MeasureStyleElement)
                 || (e is PrintElement)
                 || (e is StavesElement)
                )
                {
                    // All of these elements are related to events and timing and must be reflected in in the EventDescriptionList.
                    // So they are transferred through the following lists:
                    // List<MusicXmlObject> -> PartDescriptionList -> TimeDescriptionList -> EventDescriptionList
                    currentPart.Elements.Add(e);
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

                else if
                (   (o is TransposeElement)
  
                )
                {
                    // These types are explicitly ignored because they are related to the whole part, not to an event.
                }
                                
                else
                {
                    Type type = o.GetType();
                    string typeAsString = type.ToString();
                    Logger.LogCFOnce(string.Format(": Unexpected object of type {0}: String='{1}'", typeAsString, o.ToString()));
                }

            }

            // Only for inspection during debugging:
            foreach (PartDescription part in parts)
            {
                int numberOfElements = part.Elements.Count;
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
