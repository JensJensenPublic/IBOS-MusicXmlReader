using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class MeasureFraction
    {
        // string className = "MeasureFraction";
        private int nominator;
        public int Nominator { get { return nominator; } }

        private int denominator;
        public int Denominator { get { return denominator; } }

        private Int64 newStartTime;
        private Int64 currentStartTime;
        private Int64 currentMeasurestartTime;
        private bool found = false;
        IntegerFraction result;
        //Int64 denominatorResult = -1;
        //Int64 nominatorResult = -1;
        //Int64 denominatorResult2 = -1; // Needed for expressing as a sum of 2 fractions
        //Int64 nominatorResult2 = -1; // Needed for expressing as a sum of 2 fractions
        //MeasureFraction previousMeasureFraction = null;

        const Int64 quarterNoteDuration = NoteElement.commonDivisions;
        const Int64 fullNoteDuration = 4 * quarterNoteDuration;
        private EventDescription eventDescription; // The EventDescription owning this MeasureFraction

        public Int64 CurrentStartTime { get { return currentStartTime; } }
        private string complexString = "";


        public override string ToString()
        {
            //Evaluate();

            if (found)
            {
                // The offset within the measure can be expressed as a simple fraction
                if (0 == result.nominator)
                {
                    return string.Format("{0}", result.nominator); // Ignore the denominator
                }
                else
                {
                    return string.Format("{0}/{1}", result.nominator, result.denominator); // Show the denominator
                }
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
                return string.Format("{0}", decimalValue);
            }
        }

        private struct IntegerFraction
        {
            public Int64 nominator;
            public Int64 denominator;
        }

        private bool EvaluateFraction(Int64 offset, List<Int64> denominators, ref IntegerFraction result)
        {
            bool fractionFound = false;
            foreach (Int64 denominator in denominators)
            {
                for (Int64 nominator = 1; (nominator <= denominator * 2); nominator++)
                {
                    Int64 product = denominator * offset;
                    Int64 divisor = nominator * fullNoteDuration;
                    if (0 == product % divisor)
                    {
                        result.denominator = denominator;
                        result.nominator = product / divisor;
                        fractionFound = true;
                        break; // Break out of for-loop
                    }
                }
                if (fractionFound)
                {
                    break; // break out or foreach-loop
                }
            }

            if (fractionFound)
            {
                string s = string.Format("({0})={1}/{2} ", offset, result.nominator, result.denominator);
                // Logger.LogCF(s);
            }

            return fractionFound;
        }


        /// <summary>
        /// This function may be called either "Just in time" as a part of ToString() or during initialization, depending of performance needs.
        /// </summary>
        public void Evaluate(ref MeasureFraction previousMeasureFraction)
        { 
            Int64 offset = newStartTime - currentMeasurestartTime;
            found = false;

            if (0 == offset)
            {
                result.nominator = 0;
                result.denominator = 1;
                found = true;
                return;
            }

            // Attempt to find integers N and D (for nominator and denominator) 
            // so that offset/fullNoteDuration can be expressed as an integer fraction N/D
            // This is done by finding N and D so  (D*offset) / (N*fulllNoteDuration) has a remainder of 0

            result = new IntegerFraction();
            List<Int64> denominators2 = new List<Int64> { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
            // Look for values such as
            // 1/2, 2/2, 3/2, 4/2
            // 1/4, 2/4, 3/4, 4/4, 5/4, 6/4, 7/4, 8/4
            // 1/8 .. 16/8
            // 1/16 .. 32/16
            // ...
            found = EvaluateFraction(offset, denominators2, ref result);

            if (!found)
            {
                if (null != previousMeasureFraction)
                {
                    // Attempt to express the offset to latest known beat as N/M
                    offset = newStartTime - previousMeasureFraction.newStartTime;
                    List<Int64> denominators3 = new List<Int64> { 1, 3, 6, 12, 24, 48, 96, 192, 384 };
                    //    // Look for values such as
                    //    // 1/3, 2/3, 3/3, 4/3
                    //    // 1/6, 2/6, 3/6, 4/6, 5/6, 6/6, 7/6, 8/6
                    //    // 1/12 .. 16/12
                    //    // 1/24 .. 32/48
                    //    // ...
                    found = EvaluateFraction(offset, denominators3, ref result);
                }
            }



            if (!found)
            {
                // Special code for the case where the offset can not be expressed as a simple fraction
                if (null != eventDescription.EndEventElements)
                {
                    int count = eventDescription.EndEventElements.Count;
                    if (count != 1)
                    {
                        //Logger.LogCFOnce(string.Format(": Count={0}", count));   
                    }

                    // Find the list of all notes that end at this event
                    EventDescription existingOwner = null;
                    foreach (EndEventElement endEventElement in eventDescription.EndEventElements)
                    {
                        if (endEventElement.StartElement is NoteElement)
                        {
                            NoteElement noteElement = endEventElement.StartElement as NoteElement;
                            EventDescription owner = noteElement.OwningEventDescription;
                            string s = string.Format(": {0} OwningEvent: Start={1} {2}", noteElement.ToString(),owner.StartTime.ToString(),owner.MeasureFractions.ToString());
                            // Logger.LogCF(s);
                            this.complexString = noteElement.OwningEventDescription.MeasureFractions.ToString() + "+" + noteElement.TupleDurationString() ;
                            //found = true;

                            // Check if the noteelements belong to different owners
                            if ((null != existingOwner) && (owner != existingOwner))
                            {
                                Logger.LogCFOnce(string.Format(": Count={0} Different owners found",count));
                            }
                            existingOwner = owner;
                        }
                    }
                }
            }

            if (found || !string.IsNullOrEmpty( this.complexString))
            {
                //        previousMeasureFraction = this;
            }
            else
            {
                Logger.LogCF(string.Format("(): MeasureFraction could not be determined for offset={0} q={1} q/3={2}", offset, quarterNoteDuration, quarterNoteDuration / 3));
            }





            // Can neither be expressed as N/(2^M) nor N/((2^M)*3)) 
            // Attempt to express it as a sum of the two above.

            //foreach (Int64 denominator in new List<Int64> { 1, 3, 6, 12, 24, 48, 96, 192, 384 })
            //{
            //    // Look for values such as
            //    // 1/3, 2/3, 3/3, 4/3
            //    // 1/6, 2/6, 3/6, 4/6, 5/6, 6/6, 7/6, 8/6
            //    // 1/12 .. 16/12
            //    // 1/24 .. 32/48
            //    // ...
            //    for (Int64 nominator = 1; (nominator <= denominator * 2); nominator++)
            //    {
            //        Int64 product = denominator * offset;
            //        Int64 divisor = nominator * fullNoteDuration;
            //        if (0 == product % divisor)
            //        {
            //            denominatorResult2 = denominator;
            //            nominatorResult2 = product / divisor;
            //            this.previousMeasureFraction = previousMeasureFraction;
            //            found = true;
            //            Logger.LogCF(string.Format(": {0}/{1} + {2}/{3}", previousMeasureFraction.nominatorResult, previousMeasureFraction.denominatorResult, nominatorResult2, denominatorResult2));
            //            break; // Break out of for-loop
            //        }
            //    }
            //    if (found)
            //    {
            //        break; // break out or foreach-loop
            //    }
            //}

#if false
            if (!found)
            {
                //if ((null != previousMeasureFraction) && (null != previousMeasureFraction.ads))
                //{
                //    Logger.LogCF(string.Format(": PreviousMeasureFraction.ads.Count={0}", previousMeasureFraction.ads.Count));
                //}

                // Create a list of all noteElements that end at this EventDescription
                IntegerFraction result = new IntegerFraction();
                List<EndEventElement> endEventElements = eventDescription.EndEventElements;
                foreach (EndEventElement endEventElement in endEventElements)
                {
                    if (endEventElement.StartElement is NoteElement)
                    {  

                        Int64 arithmeticDurationInCommonDivisions = (endEventElement.StartElement as NoteElement).ArithmeticDurationInCommonDivisions;
                        Logger.LogCF(string.Format(": ArithmeticDurationInCommonDivisions={0}", arithmeticDurationInCommonDivisions));
                        offset = arithmeticDurationInCommonDivisions;
                        found = EvaluateFraction(offset,ref result);
                    }
                    if (found)
                    {
                        if (null == previousMeasureFraction)
                        {
                            Logger.LogCF(string.Format(" : Found! PreviousMeasureFraction=null", previousMeasureFraction));
                        }
                        else
                        {
                            Logger.LogCF(string.Format(" : Found! Prev.Nominator={0} Prev.Denom={1}",
                                previousMeasureFraction.Nominator, previousMeasureFraction.Denominator));
                            string s = string.Format(" {0}/{1} + {2}/{3}", previousMeasureFraction.Nominator, previousMeasureFraction.Denominator, result.nominator, result.denominator);
                            Logger.LogCF(s);
                        }
                        break;
                    }
                }
          }
#endif


        }

        private MeasureFraction(EventDescription eventDescription, int nominator, int denominator)
        {
            this.nominator = nominator;
            this.denominator = denominator;
            this.eventDescription = eventDescription;
        }

        static public MeasureFraction Create(EventDescription eventDescription,int nominator)
        {
            return new MeasureFraction(eventDescription,nominator, 1);
        }

        private MeasureFraction(EventDescription eventDescription, Int64 currentStartTime, Int64 currentMeasureStartTime)
        {
            // string functionName = "MeasureFraction";
            Int64 startTime = eventDescription.StartTime;
            Int64 duration = startTime - currentStartTime;
            Int64 quarterNoteDuration = NoteElement.commonDivisions;
            Int64 fullNoteDuration = 4 * quarterNoteDuration;
            int nominator = 1;   // TODO
            int denominator = (int)(fullNoteDuration / duration);
            int remainder = (int)(fullNoteDuration % duration);
            if (0 != remainder)
            {
                // Logger.Log(string.Format("{0}.{1}({2},{3}): Denominator={4} Remainder={5}", className, functionName, newStartTime, currentStartTime, denominator, remainder));
            }
            this.nominator = nominator;
            this.denominator = denominator;
            // Variables for alternaticve calculation:
            this.newStartTime = startTime;
            this.currentStartTime = currentStartTime;      
            this.currentMeasurestartTime = currentMeasureStartTime;
            this.eventDescription = eventDescription;
        }


        static public MeasureFraction Create(EventDescription eventDescription, Int64 currentStartTime,Int64 currentMeasureStartTime)
        {
            return new MeasureFraction(eventDescription, currentStartTime, currentMeasureStartTime);
        }
    }
}
