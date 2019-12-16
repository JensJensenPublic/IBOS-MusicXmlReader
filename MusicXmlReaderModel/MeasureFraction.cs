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
        private IntegerFraction binaryFractionPart;     // The part of the measurePosition, that can be expressed as N/2^M N and M integers
        private IntegerFraction baseBinaryFraction;     // The base part to be used together with tupleFractions
        private List<IntegerFraction> tupleFractions;   // The part of the measurePosition, that can be expressed as a sum of Q/P where Q is an integer and P is a prime 3,5,7,9,11 etc 
        private EventDescription eventDescription;      // The EventDescription owning this MeasureFraction
        private string stringRepresentation = "";       // Initialized by the Evaluate() function during initialization and returned later by tehe ToString() function


        /// <summary>
        /// Returns the string representation of this object.
        /// This requires that the Evaluate() function was called durting initialization !
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (null == stringRepresentation) Logger.LogCFOnce("; stringRepresentation is null");
            return stringRepresentation;
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

        private bool NotesFound()
        {
            // Warn about events containing only EndEvents and no Note Elements
            int nEndEvents = eventDescription.EndEventElements.Count;
            int nNotes = eventDescription.NoteCount;
            if ((nEndEvents > 0) && (0 == nNotes))
            {
                // If the MusicXml file is not excact with respect to integer arithmetics, this may cause empty lines and strange MeasureFractions !!
                Logger.LogCFOnce(string.Format(": Found {0} EndEvents and {1} NoteElements. This will cause an empty line in the NoteList!", nEndEvents, nNotes));
                stringRepresentation = "+??";
                return false;
            }
            return true;
        }

        private void AddOwner(List<TestItem> allOwners, EventDescription eventDescription, NoteElement noteElement) //  For debugging only !
        {
            bool found = false;
            foreach (TestItem testItem in allOwners)
            {
                if (testItem.OwningEvent == eventDescription)
                {
                    found = true;
                    break;
                }

                if (!found)
                {
                    allOwners.Add(new TestItem(eventDescription, noteElement));
                }
            }
        }
        

        private void Format(string baseString, List<IntegerFraction> tupleFractions)
        {
            IntegerFraction firstElement = tupleFractions.ElementAt(0);
            firstElement.Normalize();
            long number = 0;
            foreach (IntegerFraction binaryFraction in tupleFractions)
            {
                binaryFraction.Normalize();
                if (binaryFraction.IsEqualTo(firstElement))
                {
                    number++;
                }
            }

            if (number == tupleFractions.Count)
            {
                // The tuple contains a number of identical fractions, so we can express it in a very shorthand form
                IntegerFraction integerFraction = new IntegerFraction(number, firstElement.Denominator);
                this.stringRepresentation = baseString + " + " + integerFraction.ToString();
            }
            else
            {
                // The tuple contains fractions that are not all identical. We must list them all !
                StringBuilder sb = new StringBuilder(baseString);
                foreach (IntegerFraction binaryFraction in tupleFractions)
                {
                    sb.Append("+");
                    sb.Append(binaryFraction.ToString());
                }
                this.stringRepresentation = sb.ToString();
            }

        }





/// <summary>
/// This function must be called during initialization!
/// It takes no parameters, but works on several member variables:.
/// It may update the following member variables
///  this. stringrepresentation
///  this.binaryfractionPart
///  this.baseBinaryFraction 
///  this.tupleFractions
/// </summary>
public void Evaluate()
        {
            Int64 offset = newStartTime - currentMeasurestartTime;


            bool showBeatNumber = true; // TEmporarily all switchin back to old implementation !

            string beatStringAndNumber = null;
            if (showBeatNumber)
            {
                System.Int64 beatDuration = this.eventDescription.StatusInformation.CurrentTimeElement.BeatDuration;
                System.Int64 beatNumber = (offset / beatDuration) + 1; // In music the index base is 1 !
                string beatString = ResourcesForModel.NoteElement_Beat_Text;
                beatStringAndNumber = string.Format("{0} {1} ", beatString, beatNumber); // Assure same formatting in all cases
                offset = offset % beatDuration;
                if (offset == 0)
                {
                    // The position is on the beat
                    binaryFractionPart = new IntegerFraction(0, 1);
                    baseBinaryFraction = binaryFractionPart;
                    stringRepresentation = beatStringAndNumber;
                    return;
                }
            }
            else
            {
                if (0 == offset)
                {
                    binaryFractionPart = new IntegerFraction(0, 1);
                    baseBinaryFraction = binaryFractionPart;
                    stringRepresentation = "+ " + binaryFractionPart.ToString();
                    return;
                }
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
            baseBinaryFraction = binaryFractionPart;
            if (null != binaryFractionPart)
            {
                if (showBeatNumber)
                {
                    // The position can be expressed as a MeasureNumber plus a BeatNumber plus a simple fraction
                    stringRepresentation = string.Format("{0} + {1}", beatStringAndNumber, binaryFractionPart.ToString());
                }
                else
                {
                    stringRepresentation = "+ " + binaryFractionPart.ToString();
                }
                return; 
            }


            // If the value can not be expressed as a simple binary fraction attempt to express it as a sum of basebinaryfraction and a tuplet:
      
            if (null != eventDescription.EndEventElements)
            {
                //Logger.LogCF(string.Format(": Offset={0} can not be expressed as a simple binary fraction", offset));
                int count = eventDescription.EndEventElements.Count;
                // if (count != 1)  Logger.LogCFOnce(string.Format(": {0} owners", count));   

                if (! NotesFound()) return; // Return if no notes end at this eventDescription

                // Find the list of all notes that end at this event
                List<TestItem> allOwners = new List<TestItem>(); // Only used for code analyzis, not visible to the user !
                foreach (EndEventElement endEventElement in eventDescription.EndEventElements)
                {
                    this.tupleFractions = new List<IntegerFraction>();
                    // NOTE: This is NOT strictly correct: This code will use only the last endEventElement encountered, but for the moment this will do !
                    if (endEventElement.StartElement is NoteElement)
                    {
                        NoteElement previousNoteElement = endEventElement.StartElement as NoteElement;
                        if (previousNoteElement.NoteDuration == NoteTypeEnum.unknown) //   PrintObjectAttributeValue)
                        {
                            string commonFormat = "(): PreviousNoteElement.NoteDuration='{0}'  {1}";
                            if (previousNoteElement.PrintObjectAttributeValue)
                            {
                                Logger.LogCFOnce(string.Format(commonFormat, previousNoteElement.NoteDuration.ToString(),""));
                            }
                            else
                            {
                                Logger.LogCFOnce(string.Format(commonFormat , previousNoteElement.NoteDuration.ToString()," for invisible NoteElement. This is OK"));
                            }
                            break; // Relies on reporting of a decimal fraction as a last resort. 
                        }
                        EventDescription previousEventDescription = previousNoteElement.OwningEventDescription;
                        // Logger.LogCFOnce( string.Format(": {0} OwningEvent: Start={1} {2}", noteElement.ToString(),owner.StartTime.ToString(),owner.MeasureFraction.ToString()));


                        baseBinaryFraction = previousEventDescription.MeasureFraction.baseBinaryFraction; // Use the same baseBinaryfraction as the previous

                        this.tupleFractions.AddRange(previousEventDescription.MeasureFraction.tupleFractions); // Copy the tuple fractions of previous EventDescription
                        this.tupleFractions.Add(previousNoteElement.TupleDuration()); // Add tupleduration of previous NoteElement
                        string baseString = "";
                        if (showBeatNumber)
                        {
                            baseString = beatStringAndNumber;
                            if (null != baseBinaryFraction)
                            {
                                baseString = baseString + "+ " + baseBinaryFraction.ToString(); // Use the latest known binary fraction as a base
                            }
                        }
                        else
                        {
                            if (null != baseBinaryFraction)
                            {
                                baseString = "+ " + baseBinaryFraction.ToString(); // Use the latest known binary fraction as a base
                            }
                        }

                        Format(baseString,tupleFractions);

                        AddOwner(allOwners, previousEventDescription, previousNoteElement); //  For debugging only !

                        string logMessage = "The position could NOT be expressed as a MeasureNumber plus a BeatNumber plus a simple fraction";
                        // Logger.LogCF(string.Format(": {0} in '{1}' this.stringRepresentation='{2}' ", logMessage, Logger.CurrentMusicXmlFile, this.stringRepresentation));
                        // Logger.LogCFOnce(string.Format(": {0} in '{1}' ", logMessage , Logger.CurrentMusicXmlFile));

                    }
                }
                Log(allOwners, eventDescription); // For debugging only !!
            }

            if (!string.IsNullOrEmpty(this.stringRepresentation))
            {
                //Logger.LogCF(string.Format(": Stringrepresentation='{0}'", this.stringRepresentation));
                return;
            }

            // As a last resort represent the value as a decimnal fraction
            float decimalValue = ((float)offset / (float)fullNoteDuration);
            if (showBeatNumber)
            {
                stringRepresentation = string.Format("{0} + {1:0.000}", beatStringAndNumber, decimalValue);
            }
            else
            {
                stringRepresentation = string.Format("+ {0:0.000}", decimalValue);
            }
            string message = "Using last resort Decimal representation";
            Logger.LogCF(string.Format(": {0} {1} Stringrepresentation",message, this.stringRepresentation));
            Logger.LogCFOnce(string.Format(": {0} in '{1}' ",message, Logger.CurrentMusicXmlFile));
            // Use the following line for debugging only ! (Performance issus)
            bool visible = this.eventDescription.ContainsVisibleNotes;
            if (visible)
            {
                Logger.LogCF(string.Format("(): MeasureFraction could not be determined for offset={0} q={1} q/3={2} Using decimal value={3}", offset, quarterNoteDuration, quarterNoteDuration / 3, stringRepresentation));
            }
            {
                Logger.LogCFOnce("(): MeasureFraction could not be determined for invisible event. Using decimal value instead. This is OK"); 
            }
            //Logger.LogCFOnce(string.Format("(): MeasureFraction could not be determined."));
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

