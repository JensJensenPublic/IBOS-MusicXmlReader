using System;
using System.Collections.Generic;

namespace MusicXmlReaderModel
{

    public class TimeDescriptionList:  IComparer<Element>
    {
        /// <summary>
        /// NextStartTime was computed as a sum of durations of NoteElements, BackupElements, ForwardElements etc and is not always accurate !
        /// NextMeasureBasedStartTime is based only on the starttime and duration of measureElements and is believed to be accurate !
        /// When these 2 values differ and NextMeasureBasedStartTime exists (i.e. not at the first 2 MeasureElements) we use NextMeasureBasedStartTime
        /// For investigation problem 391 :
        /// </summary>
        /// <param name="measureElement"></param>
        /// <param name="nextStartTime"></param>
        /// <returns></returns>
        private Int64 EvalNextStartTime(MeasureElement measureElement, Int64 nextStartTime)
        {
            int number = measureElement.Number;
            string partId = (null == measureElement.PartId) ? "" : measureElement.PartId;
            MeasureElement previous = measureElement.PreviousMeasureElement;
            if (previous == null)
            {
                // No previous measure to rely on we can only return the original nextStartTime
                // Logger.LogCF(string.Format(": Previous  == null:  Part={0} Mesaure={1}", partId, number));
                return nextStartTime;
            }

            if (null == previous.PreviousMeasureElement)
            {
                // The previous measure is the first measure so it may be incomplete (Danish "optakt")
                // Logger.LogCF(string.Format(": Prev.Prev == null: Part={0} Mesaure={1}", partId, number));
                return nextStartTime;
            }

            // Based on the starttime of the previous MeasureElement and the duration of it.
            // Note: The duration of the previous MeasureEmlement is NOT known when it was read from the file, but is known now !
            Int64 startTime = previous.StartTime;
            Int64 nextMeasureBasedStartTime = startTime + measureElement.MeasureDuration; // Based on previous MeasureElement in this part ONLY
            if (nextMeasureBasedStartTime != nextStartTime)
            {
                Utilities.Beep();
                Logger.LogCFOnce(string.Format(": StartTimes differ: Part={0} Measure={1}", partId, number ));
                Logger.LogCFOnce(string.Format(": File={0}", Logger.CurrentMusicXmlPath));

                Logger.LogCF(String.Format(": StartTimes differ: Part={0} Measure={1} NextStartTime={2} NextMeasureStartTime={3} PreviousStartTime={4} MeasureDuration={5} Adjusting NextStartTime to {6}",
                    partId, number, nextStartTime, nextMeasureBasedStartTime, startTime, measureElement.MeasureDuration, nextMeasureBasedStartTime));
            }
            return nextMeasureBasedStartTime;
        }


        private void Add(EventElement eventElement, Int64 startTime)
        {
            eventElement.StartTime = startTime;
            times.Add(eventElement);
        }



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

            foreach (PartDescription part in partDescriptionList.parts)
            {
                Int64 nextStartTime = 0; // Each part starts at time = 0 MilliSeconds
                Int64 previousStartTime = 0;
                foreach (Element e in part.Elements)
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
                        }
                    }
                    else if (e is ForwardElement)
                    {
                        // Move the MusciXml program counter without playing anything
                        ForwardElement forwardElement = e as ForwardElement;
                        nextStartTime += forwardElement.DurationInCommonDivisions; /////////////////////////////// FIX THIS TO DO
                    }

                    else if (e is BackupElement)
                    {
                        // Move the MusciXml program counter without playing anything
                        BackupElement backupElement = e as BackupElement;
                        nextStartTime -= backupElement.DurationInCommonDivisions; /////////////////////////////// FIX THIS TO DO
                    }

                    else if (e is MeasureElement)
                    {
                        nextStartTime = EvalNextStartTime(e as MeasureElement, nextStartTime);
                        Add((e as EventElement), nextStartTime);
                    }

                    else if (
                        (e is HarmonyElement)
                     || (e is SoundElement)
                     || (e is KeyElement)
                     || (e is ClefElement)
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
                        // All these elements are EventElements!
                        Add((e as EventElement), nextStartTime);
                    }
                    else
                    {
                        // Ignore this element.
                        Logger.LogCFOnce(string.Format(": Unexpected element of type {0} String='{1}'", e.GetType(), e.ToString()));
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

