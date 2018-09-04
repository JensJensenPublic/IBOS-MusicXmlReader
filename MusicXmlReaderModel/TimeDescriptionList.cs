using System;
using System.Collections.Generic;

namespace MusicXmlReaderModel
{

    public class TimeDescriptionList:  IComparer<Element>
    {

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TimeDescriptionList()
        {
        }

        // The list of times ,each containing a list of elements        
        public List<Element> times;

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TimeDescriptionList(PartDescriptionList partDescriptionList, int divisions)
        {
            int numberOfParts = partDescriptionList.NumberOfParts;
            times = new List<Element>();

            foreach (List<Element> elementList in partDescriptionList.parts)
            {
                Int64 nextStartTime = 0; // Each part starts at time = 0 MilliSeconds
                Int64 previousStartTime = 0;
                Int64 maxNextStartTime = 0;
                foreach (Element e in elementList)
                {
                    if (e is NoteElement)
                    {
                        NoteElement noteElement = e as NoteElement;
                        if (noteElement.GraceNote)
                        {
                            // Grace notes are not implemented yet, but they must be explicitly ignored.
                            // TO DO: Implement grace notes.
                            string pitch = noteElement.Pitched ? noteElement.Step.ToString() + noteElement.Octave.ToString() : "(unpitched)";
                            //Logger.Log(string.Format("TimeDescriptionList constructor ignoring grace note {0} in measure {1} part {2}", pitch, noteElement.MeasureNumber, noteElement.PartId));
                            //Logger.LogOnce(string.Format("TimeDescriptionList constructor ignoring grace note"));
                        }
                        else
                        {
                            times.Add(noteElement);
                            if (noteElement.Chord)
                            {   // Start at the beginning of the previous note
                                noteElement.StartTime = previousStartTime;
                            }
                            else
                            {
                                // Default: Start after the previous note
                                noteElement.StartTime = nextStartTime;
                                previousStartTime = nextStartTime; // Needed if the following note has the "chord" elemenn
                                nextStartTime += noteElement.DurationInCommonDivisions;
                            }
                            // Create an EndEventElement to mark the end of this NoteElement                            
                            EndEventElement endEventElement = EndEventElement.Create(noteElement, noteElement.StartTime + noteElement.DurationInCommonDivisions);
                            times.Add(endEventElement);
                            maxNextStartTime = Math.Max(maxNextStartTime, nextStartTime);
                        }
                    }
                    else if (e is ForwardElement)
                    {
                        // Move the MusciXml program counter without playing anything
                        ForwardElement forwardElement = e as ForwardElement;
                        nextStartTime += forwardElement.DurationInCommonDivisions; /////////////////////////////// FIX THIS TO DO
                        maxNextStartTime = Math.Max(maxNextStartTime, nextStartTime);
                        //previousStartTime = nextStartTime; // ??????????????????????????????????????????
                    }

                    else if (e is BackupElement)
                    {
                        // Move the MusciXml program counter without playing anything
                        BackupElement backupElement = e as BackupElement;
                        bool wasMax = (nextStartTime == maxNextStartTime); 
                        nextStartTime -= backupElement.DurationInCommonDivisions; /////////////////////////////// FIX THIS TO DO
                        if (wasMax)
                        {
                            // maxNextStartTime has been set too high and must be moved back !
                            //Logger.LogCF(string.Format(".BackupElement: Reducing maxNextStartTime from {0} to {1}", maxNextStartTime, nextStartTime));
                            //Logger.LogCFOnce(string.Format(".BackupElement: Reducing maxNextStartTime"));
                            maxNextStartTime = nextStartTime;
                        }

                        //previousStartTime = nextStartTime; // ??????????????????????????????????????????
                    }

                    else if ((e is HarmonyElement)
                         ||  (e is MeasureElement)
                         ||  (e is SoundElement)
                         ||  (e is KeyElement)
                         ||  (e is ClefElement)
                         || (e is TimeElement)
                         || (e is RepeatElement)
                         || (e is BarlineElement)
                         || (e is InstrumentsElement)
                         || (e is AttributesElement)
                         || (e is DirectionElement)
                         || (e is MeasureStyleElement)

                         )

                    {
                        // All these elements are EventElements!
#warning ToDo Refactor !
                        if (e is MeasureElement)
                        {
                            // For investigation problem 391 :
                            // If a part contains more than 1 voice, some MusicXml files may contai voices that do not fill up all measures completely by rests.                 

                            int number = (e as MeasureElement).Number;
                            if (nextStartTime != maxNextStartTime)
                            {
                                // Model.MetaInformation                 
                                // Logger.LogCFOnce(string.Format(": Adjusts NextStartTime. CurrentEncoding={0}", Logger.CurrentEncoding)); // To verify that this only happens for Sibelius !
                                Logger.LogCF(string.Format(": Measure {0} adjusts NextStartTime from {1} to {2} CurrentEncoding={3}", number, nextStartTime, maxNextStartTime, Logger.CurrentEncoding));
                                nextStartTime = maxNextStartTime;
                            }
                        }
                        

                        EventElement eventElement = e as EventElement;
                        eventElement.StartTime = nextStartTime;
                        times.Add(eventElement);
                    }
    
                    else
                    {
                        // Ignore this element.
                        Logger.LogOnce(string.Format("TimeDescriptionList: Unexpected element of type {0} String='{1}'", e.GetType(), e.ToString()));
                    }

                }

                if (0 == (times.Count))
                {
                    return;
                }

                times.Sort(Compare);

            }

        }

        public static TimeDescriptionList Create(PartDescriptionList partDescriptionList, int divisions)
        {
            return new TimeDescriptionList(partDescriptionList, divisions);
        }

        public int Compare(Element x, Element y)
        {
            if ((x is EventElement) && (y is EventElement))
            {
                // We want to use 64 bit integer arithmetics for  all times, but Compare must return an int:
                Int64 dif = ((x as EventElement).StartTime - (y as EventElement).StartTime);
                if (dif < 0) return -1;
                if (dif > 0) return  1;
            }
            return 0;
        }
        
    }
}

