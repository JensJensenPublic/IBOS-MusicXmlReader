using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Contains code only used for analysis
    /// </summary>
    partial class MeasureFraction
    {

        /// <summary>
        /// For Analyzis only. No value for end user !!
        /// </summary>
        /// <param name="allOwners"></param>
        private void Log(List<TestItem> allOwners, EventDescription eventDescriptionBeingAnalyzed)
        {
            List<string> logs = new List<string>(); // Holds the log lines until we decide to show them
            List<IntegerFraction> fractionSums = new List<IntegerFraction>(); // Holds the sums in order to decide to show the logs.
            StringBuilder fractionSumString = new StringBuilder();
            if (allOwners.Count > 1)
            {
                Logger.LogCFOnce(string.Format(": {0} different owners for startevent", allOwners.Count)); // For MusicXmlReaderCmd
                Logger.LogCF(string.Format(": {0} different owners for startevents ending at {1}", allOwners.Count, eventDescriptionBeingAnalyzed.StartTime));
                foreach (TestItem testItem in allOwners)
                {
                    NoteElement ne = testItem.NoteElement;
                    TimeModificationElement tme = ne.TimeModificationElement;
                    int measureNumber = testItem.OwningEvent.StatusInformation.CurrentMeasureElement.Number;
                    string startTime = testItem.OwningEvent.StartTime.ToString();
                    string startEvent = string.Format("StartEvent({0} {1}+{2})", startTime, measureNumber, testItem.OwningEvent.MeasureFraction.ToString());
                    IntegerFraction fractionSum = GetFractionSum(testItem);

                    string noteElementString = string.Format(" NoteElement( '{0}' {1}{2}{3}{4} Voice={5} Actual={6} Normal={7} {8} )",
                        ne.ScorePartElement.PartName.TrimStart(' '), // 0
                        ne.PrintObjectAttributeValue ? "" : "NotPrinted", // 1
                        ne.GraceNote ? " GraceNote" : "", // 2
                        ne.CueNote ? "CueNote" : "", // 3
                        GetPitchOrPause(ne), // 4
                        ne.Voice, // 5                        
                        tme.ActualNotes, // 6
                        tme.NormalNotes,  // 7
                        ne.NoteDuration); // 8
                    logs.Add(string.Format(": {0,-50} {1} Fractions={2} FractionSum={3}", startEvent, noteElementString, GetFractions(testItem), fractionSum.ToString())); // StartEvent: Length=40, left align)) 

                    // Create a list of all sums round
                    bool found = false;
                    foreach (IntegerFraction sum in fractionSums)
                    {
                        if (sum.Equals(GetFractionSum(testItem)))
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        fractionSums.Add(fractionSum);
                        fractionSumString.Append(string.Format("({0}) ", fractionSum.ToString()));
                    }


                    // Logger.Log(string.Format(" Fractions={0}", GetFractions(testItem)));
                }

                // If this list contains more than one value we probably have an error:
                int count = fractionSums.Count;
                if (count != 1)
                {
                    Logger.LogCFOnce(string.Format(": Different owners with {0} different sums found: {1}", count, fractionSumString.ToString()));
                    foreach (string s in logs)
                    {
                        Logger.Log(s);
                    }
                }
                else
                {
                    Logger.LogCFOnce(string.Format(": Different owners found, but sums are identical ({0}). This is OK!", fractionSums[0].ToString()));
                }

            }
        }

        List<IntegerFraction> GetFractionList(TestItem testItem)
        {
            // Build a list of all IntegerFractions 
            List<IntegerFraction> testList = new List<IntegerFraction>();
            if (null != testItem.OwningEvent.MeasureFraction.binaryFractionPart)
            {
                testList.Add(testItem.OwningEvent.MeasureFraction.binaryFractionPart);
            }
            testList.AddRange(testItem.OwningEvent.MeasureFraction.tupleFractions);
            testList.Add(testItem.NoteElement.TupleDuration());
            return testList;

        }


        /// <summary>
        /// Convenience method for analysis
        /// </summary>
        /// <param name="testItem"></param>
        /// <returns></returns>
        string GetFractions(TestItem testItem)
        {
            // Build a list of all IntegerFractions 
            List<IntegerFraction> testList = GetFractionList(testItem);
            StringBuilder sb = new StringBuilder();
            // Log the list in a single line
            string conc = "";
            foreach (IntegerFraction integerFraction in testList)
            {
                IntegerFraction bf = testItem.OwningEvent.MeasureFraction.binaryFractionPart;
                if (null != bf)
                {
                    sb.Append(string.Format("{0}/{1}", bf.Nominator, bf.Denominator));
                    conc = " + ";
                }

                if (integerFraction is TupletFraction)
                {
                    TupletFraction tf = integerFraction as TupletFraction;
                    string duration = string.Format("*1/{0}", tf.Duration);
                    sb.Append(string.Format("{0}{1}/{2}{3}", conc, tf.Normal, tf.Actual, duration));
                }
                else
                {
                    sb.Append(string.Format("{0}{1}/{2}", conc, integerFraction.Nominator, integerFraction.Denominator));
                }
                conc = " + ";
            }
            return sb.ToString();
        }


        IntegerFraction GetFractionSum(TestItem testItem)
        {
            // Build a list of all IntegerFractions 
            List<IntegerFraction> testList = GetFractionList(testItem);
            IntegerFraction result = new IntegerFraction();
            foreach (IntegerFraction integerFraction in testList)
            {
                result.Add(integerFraction);
                result.Normalize();
            }

            return result; // ToDo Implement !
        }


        /// <summary>
        /// Convenience method
        /// </summary>
        /// <param name="ne"></param>
        /// <returns></returns>
        string GetPitchOrPause(NoteElement ne)
        {
            if (ne.IsPause)
            {
                return "Pause";
            }
            else
            {
                if (ne.UnPitched)
                {
                    return string.Format(" Unpitched({0}{1})", ne.UnpitchedElement.DisplayStep, ne.UnpitchedElement.DisplayOctave);
                }
                else
                {
                    return string.Format(" {0}{1}", ne.PitchValue, ne.Octave);
                }
            }
        }

    }



    /// <summary>
    /// Only ised for test and analysis
    /// </summary>
    public class TestItem
    {
        private EventDescription owningEvent;
        public EventDescription OwningEvent { get { return owningEvent; } }
        private NoteElement noteElement;
        public NoteElement NoteElement { get { return noteElement; } }


        public TestItem(EventDescription owningEvent, NoteElement noteElement)
        {
            this.owningEvent = owningEvent;
            this.noteElement = noteElement;
        }
    }


}

