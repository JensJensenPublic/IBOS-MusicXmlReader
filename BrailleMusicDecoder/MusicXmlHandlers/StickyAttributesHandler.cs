using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// For handling several "sticky" attributes in the same way.
    /// A sticky attribute is  MusicBraillen attribute that when applied twice is actice until it is cancelled or till the end of the measure.
    /// For the time being we handle the following sticky attributes:
    /// Intervals: (All intervals)
    /// Articulations: (Staccato)
    /// But more are probably to come.
    /// </summary>
    class StickyAttributesHandler
    {
        private List<InputSubCategoryEnum> intervals = new List<InputSubCategoryEnum>();
        public  List<InputSubCategoryEnum> Intervals { get { return intervals; } }        
        private readonly List<InputSubCategoryEnum> allIntervals = new List<InputSubCategoryEnum>() // The list of all intervals to handle with the "sticky" option
        { InputSubCategoryEnum.IntervalFifth,InputSubCategoryEnum.IntervalFourth,InputSubCategoryEnum.IntervalOctave,
          InputSubCategoryEnum.IntervalSecond,InputSubCategoryEnum.IntervalSeventh,InputSubCategoryEnum.IntervalSixth,InputSubCategoryEnum.IntervalThird};

        private List<InputSubCategoryEnum> articulations = new List<InputSubCategoryEnum>();
        public List<InputSubCategoryEnum> Articulations { get { return articulations; } }
        private readonly List<InputSubCategoryEnum> allArticulations = new List<InputSubCategoryEnum>() // The list of all intervals to handle with the "sticky" option
        { InputSubCategoryEnum.ArticulationStaccato};


        public bool IsInterval(InputSubCategoryEnum value)
        {
            return (allIntervals.Contains(value)); 
        }

        public bool IsOccurance(InputSubSubCategoryEnum value)
        {
            bool result = (value == InputSubSubCategoryEnum.OccursOnce) || (value == InputSubSubCategoryEnum.OccursTwice);
            return result;
        }

        private void ClearAll()
        {
            intervals.Clear();
            articulations.Clear();
        }

        public void OnNewMeasure()
        {
            ClearAll();
        }

        public void OnInAccord()
        {
            ClearAll();
        }

        public bool OnArticulation(InputSubCategoryEnum articulation, InputSubSubCategoryEnum occurrances)
        {
            // Call common code with parameters specific for articulations:
            switch (occurrances)
            {
                case InputSubSubCategoryEnum.OccursOnce: return OnOnce(allArticulations, articulations, articulation);
                case InputSubSubCategoryEnum.OccursTwice: return OnTwice(allArticulations, articulations, articulation);
                case InputSubSubCategoryEnum.Unknown: return true; // Some articulations are not caregorized as once or twice yet
            }
            Logger.LogCF(string.Format(": Unexpected value={0}", occurrances));
            return false;
        }

 

        public bool OnInterval(InputSubCategoryEnum interval, InputSubSubCategoryEnum occurrances)
        {
            switch (occurrances)
            {
                case InputSubSubCategoryEnum.OccursOnce: return OnOnce(allIntervals, intervals, interval);
                case InputSubSubCategoryEnum.OccursTwice: return OnTwice(allIntervals, intervals, interval);
                case InputSubSubCategoryEnum.Unknown: return true; // Some articulations are not caregorized as once or twice yet
            }
            return false; // In all other cases the interval symbol causes the addition of a new chord note
        }


        // Common code for all InputCategories start


        /// <summary>
        /// Used during debugging to check that the values are from the same "domain": Intervals, Articulations etc.
        /// </summary>
        /// <param name="allValues"></param>
        /// <param name="currentValue"></param>
        private void CheckParams(List<InputSubCategoryEnum> allValues,  InputSubCategoryEnum currentValue)
        {
            if (allValues.Contains(currentValue)) return; ; // OK
            string message = string.Format("Unexpected parameter{0}", currentValue);
            Logger.LogCF(string.Format(": {0}", message));
            throw (new Exception(message));
        }

        private bool OnOnce(List<InputSubCategoryEnum> allValues, List<InputSubCategoryEnum> currentValues, InputSubCategoryEnum currentValue)
        {
            CheckParams(allValues, currentValue); // Primarily during debugging !
            if (0 != currentValues.Count)
            {
                // If "sticky" intervals are active the interval symbol occurred once. This acts as a terminator for the "sticky" state, but NOT as a a new interval ! 
                currentValues.Clear();
                return false;
            }
            // In all other cases the interval symbol causes the addition of a new chord note
            return true;
        }

        private bool OnTwice(List<InputSubCategoryEnum> allValues, List<InputSubCategoryEnum> currentValues, InputSubCategoryEnum currentValue)
        {
            CheckParams(allValues, currentValue); // Primarily during debugging !
            currentValues.Add(currentValue);
            //LogIntervals();
            return true;
            //return false; // By being added to currentvalues the attribute will be applied later, so it would be applied twice if also applier here and now !. 
        }
// Common code for all InputCategories end.


        private StickyAttributesHandler()
        { }

        public static StickyAttributesHandler Create()
        {
            return new StickyAttributesHandler();
        }
    }
}
