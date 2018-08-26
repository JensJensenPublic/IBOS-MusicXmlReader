using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public partial class MeasureFraction
    {
        private static readonly Int64 quarterNoteDuration = NoteElement.commonDivisions;
        private static readonly Int64 fullNoteDuration = 4 * quarterNoteDuration;


        private Int64 newStartTime;
        private Int64 currentMeasurestartTime;
        private IntegerFraction binaryFractionPart; // Contains the part of the measurePosition, that can be expressed as N/2^M N and M integers
        private List<IntegerFraction> tupleFractions;  // Contains the part of the measurePosition, that can be expressed as a sum of Q/P where Q is an integer and P is a prime 3,5,7,9,11 etc 



        private EventDescription eventDescription; // The EventDescription owning this MeasureFraction

        private string complexString = "";


        public override string ToString()
        {
            if (null != binaryFractionPart)
            {
                // The offset within the measure can be expressed as a simple fraction
                return "+ " + binaryFractionPart.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(complexString))
                {
                    // The offset within the measure can NOT be expressed as a simple fraction, but as a more complex structure
                    // Is composed of several values
                    return complexString;
                }

                // As a last resort represent the value as a decimnal fraction
                Int64 offset = newStartTime - currentMeasurestartTime;
                float decimalValue = ((float)offset / (float)fullNoteDuration);
                return string.Format("{0:0.000}", decimalValue);
            }
        }


        private IntegerFraction EvaluateFraction(Int64 offset, List<Int64> denominators)
        {
            IntegerFraction result = null;
            foreach (Int64 denominator in denominators)
            {
                for (Int64 nominator = 1; (nominator <= denominator * 2); nominator++)
                {
                    Int64 product = denominator * offset;
                    Int64 divisor = nominator * fullNoteDuration;
                    if (0 == product % divisor)
                    {
                        result = new IntegerFraction(product / divisor, denominator);
                        break; // Break out of for-loop
                    }
                }
                if (null != result)
                {
                    break; // break out or foreach-loop
                }
            }

            if (null != result)
            {
                string s = string.Format("({0})={1}/{2} ", offset, result.Nominator, result.Denominator);
                // Logger.LogCF(s);
            }

            return result;
        }



        /// <summary>
        /// This function must be called during initialization!
        /// </summary>
        public void Evaluate()
        {
            Int64 offset = newStartTime - currentMeasurestartTime;

            if (0 == offset)
            {
                binaryFractionPart = new IntegerFraction(0, 1);
                return;
            }

            // Attempt to find integers N and D (for nominator and denominator) 
            // so that offset/fullNoteDuration can be expressed as an integer fraction N/D
            // This is done by finding N and D so  (D*offset) / (N*fulllNoteDuration) has a remainder of 0


            // First look for simple values such as
            // 1/2, 2/2, 3/2, 4/2
            // 1/4, 2/4, 3/4, 4/4, 5/4, 6/4, 7/4, 8/4
            // 1/8 .. 16/8
            // 1/16 .. 32/16
            // ...
            List<Int64> denominators = new List<Int64> { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
            binaryFractionPart = EvaluateFraction(offset, denominators);

            // If the value can not be expressed as a simple binary fraction attempt to express it as a sum of binary fraction and a tuplet:
            if (null == binaryFractionPart)
            {
                // Special code for the case where the offset can not be expressed as a simple fraction
                if (null != eventDescription.EndEventElements)
                {
                    int count = eventDescription.EndEventElements.Count;
                    if (count != 1)
                    {
                        // Logger.LogCFOnce(string.Format(": {0} owners", count));   
                    }

                    // Warn about events containing only EndEvents
                    int nEndEvents = eventDescription.EndEventElements.Count;
                    int nNotes = eventDescription.NoteCount;     
                    if ((nEndEvents > 0) && (0 == nNotes))
                    {
                        // If the MusicXml file is not excact with respect to integer arithmetics, this may cause empty lines and strange MeasureFractions !!
                        Logger.LogCFOnce(string.Format(": Found {0} EndEvents and {1} NoteElements. This will cause an empty line in the NoteList!", nEndEvents, nNotes));
                        complexString = "+??";
                        return;
                    }

                    // Find the list of all notes that end at this event
                    List<TestItem> allOwners = new List<TestItem>(); // Only used for code analyzis, not visible to the user !
                    foreach (EndEventElement endEventElement in eventDescription.EndEventElements)
                    {
                        this.tupleFractions = new List<IntegerFraction>();
                        // NOTE: This is NOT strictly correct: This code will use only the last eneEventElement encountered, but for the moment this will do !
                        if (endEventElement.StartElement is NoteElement)
                        {
                            NoteElement previousNoteElement = endEventElement.StartElement as NoteElement;
                            if (previousNoteElement.NoteDuration == NoteTypeEnum.unknown) //   PrintObjectAttributeValue)
                            {
                                Logger.LogCFOnce(string.Format(": PreviousNoteElement.NoteDuration = {0}", previousNoteElement.NoteDuration.ToString()));
                                break; // Relies on reporting of a decimal fraction as a last resort. 
                            }
                            EventDescription previousEventDescription = previousNoteElement.OwningEventDescription;
                            // Logger.LogCFOnce( string.Format(": {0} OwningEvent: Start={1} {2}", noteElement.ToString(),owner.StartTime.ToString(),owner.MeasureFraction.ToString()));

                            IntegerFraction baseBinaryFractionPart = previousEventDescription.MeasureFraction.binaryFractionPart; // Use the same binaryfraction
                            if (null != baseBinaryFractionPart)
                            {
                                this.tupleFractions.Add(baseBinaryFractionPart); // Use the latest known binary fraction as a base
                            }
                            this.tupleFractions.AddRange(previousEventDescription.MeasureFraction.tupleFractions); // Copy the tuple fractions of previous EventDescription
                            this.tupleFractions.Add(previousNoteElement.TupleDuration()); // Add tupleduration of previous NoteElement
                            StringBuilder sb = new StringBuilder();
                            foreach (IntegerFraction binaryFraction in tupleFractions)
                            {
                                sb.Append("+");
                                binaryFraction.Normalize();
                                sb.Append(binaryFraction.ToString());
                            }
                            this.complexString = sb.ToString();

                            bool found = false;
                            foreach (TestItem testItem in allOwners)
                            {
                                if (testItem.OwningEvent == previousEventDescription)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                allOwners.Add(new TestItem(previousEventDescription, previousNoteElement));
                            }
                        }
                    }

                    Log(allOwners, eventDescription);
                }
            }

            //if ((result != null) || !string.IsNullOrEmpty( this.complexString))
            //{
            //    //        previousMeasureFraction = this;
            //}
            //else

            if ((null == binaryFractionPart) && (string.IsNullOrEmpty(this.complexString)))

            {
                Logger.LogCF(string.Format("(): MeasureFraction could not be determined for offset={0} q={1} q/3={2}", offset, quarterNoteDuration, quarterNoteDuration / 3));
            }
        }


        private MeasureFraction(EventDescription eventDescription, Int64 currentStartTime, Int64 currentMeasureStartTime)
        {
            // string functionName = "MeasureFraction";

            Int64 startTime = eventDescription.StartTime;
            Int64 duration = startTime - currentStartTime;
            Int64 quarterNoteDuration = NoteElement.commonDivisions;
            Int64 fullNoteDuration = 4 * quarterNoteDuration;
            // Variables for alternaticve calculation:
            this.newStartTime = startTime;
            this.currentMeasurestartTime = currentMeasureStartTime;
            this.eventDescription = eventDescription;
            this.tupleFractions = new List<IntegerFraction>();
        }
        
        static public MeasureFraction Create(EventDescription eventDescription, Int64 currentStartTime, Int64 currentMeasureStartTime)
        {
            return new MeasureFraction(eventDescription, currentStartTime, currentMeasureStartTime);
        }

    }
}

