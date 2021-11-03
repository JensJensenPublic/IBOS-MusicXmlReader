using System.Collections.Generic;

namespace MusicXmlReaderModel
{
    class LoggerCounters

    {    // Two parallel lists contains the logs and the counts of logs.
        private List<string> strings = new List<string>();
        private List<int> counters = new List<int>();

        private LoggerCounters()
        { }

        static public LoggerCounters Create()
        {
            return new LoggerCounters();
        }


        /// <summary>
        /// Add a string to the the list of strings logged.
        /// If the string is already in the list:  Increment counter and return false
        /// If the string is not in the list add it and return true
        /// </summary>
        /// <param name="s"></param>
        /// <returns>true if the string was added</returns>
        public bool Add(string s)
        {
            bool found = false;
            for (int i = 0; ((i < strings.Count) && !found); i++)
            {
                if (0 == strings[i].CompareTo(s))
                {
                    // The new string is already in the list
                    (counters[i])++;
                    found = true;
                }
            }
            if (!found)
            {
                strings.Add(s);
                counters.Add(1); // Count this occurrance
            }
            return !found;
        }

        public void ClearStatistics()
        {
            strings.Clear();
            counters.Clear();
        }

        /// <summary>
        /// Dumps all strings used as argument to LogOnce with the number of times it has been called
        /// since last call to ClearStatistics()
        /// </summary>
        public List<string> GetStatistics(out int totalNumberOfEntries)
        {
            int sum = 0;
            List<string> result = new List<string>();
            for (int i = 0; (i < strings.Count); i++)
            {
                result.Add(string.Format("  {0}:{1}", strings[i], counters[i])); // Indent by 2 positions
                sum += counters[i];
            }
            totalNumberOfEntries = sum;
            return result;
        }

    }
}

