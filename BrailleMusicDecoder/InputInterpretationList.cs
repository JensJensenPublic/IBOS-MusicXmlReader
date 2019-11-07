using System.Collections.Generic;
using System.Text;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// A simple class for holding all the possible interpretations of a sequence of BrailleMusic characters
    /// </summary>
    class InputInterpretationList
    {
        private IntegerList rawValues;
        public IntegerList RawValues { get { return rawValues; } } // The raw input values used for generating this InputValueList
        private List<InputInterpretation> inputInterpretations = new List<InputInterpretation>();
        public List<InputInterpretation> InputInterpretations { get { return inputInterpretations; } }

        public InputInterpretationList(IntegerList rawValues)
        {
            this.rawValues = rawValues;
        }


        public void Add(InputInterpretation inputValue)
        {
            if (null == inputValue.Value) return;
            inputInterpretations.Add(inputValue);
        }

        public void Add(int token, InputCategoryEnum category, string value)
        {
            Add(new InputInterpretation(new IntegerList(token), category, value));
        }

        public void Add(int token, InputCategoryEnum category)
        {
            Add(new InputInterpretation(new IntegerList(token), category, ""));
        }

        public void Add(IntegerList token, InputCategoryEnum category, string value)
        {
            Add(new InputInterpretation(token, category, value));
        }

        /// <summary>
        /// Returns the values of this instance, filtered by the allowedInputCategories bitmask, thus allowing 0, 1 or several values to pass through
        /// </summary>
        /// <param name="allowedInputCategories"></param>
        /// <returns></returns>
        public InputInterpretationList Filter(InputCategoryEnum allowedInputCategories)
        {
            InputInterpretationList result = new InputInterpretationList(this.rawValues);
            foreach (InputInterpretation inputValue in this.inputInterpretations)
            {
                if (0 != (inputValue.InputCategory & allowedInputCategories))
                {
                    result.Add(inputValue);
                }
            }
            return result;
        }

        public bool Contains(InputCategoryEnum category)
        {
            foreach (InputInterpretation inputValue in this.inputInterpretations)
            {
                if (0 != (inputValue.InputCategory & category))
                {
                    return true;
                }
            }
            return false;
        }


        public int Count { get { return inputInterpretations.Count; } }
        public override string ToString()
        {
            string warning = "";
            if (inputInterpretations.Count != 1)
            {
                StringBuilder sbWarning = new StringBuilder();
                sbWarning.Append(string.Format("{0} ->Warning: {1} interpretations found", rawValues.ToUnicodeString(), inputInterpretations.Count));
                foreach (InputInterpretation inputValue in inputInterpretations)
                {
                    sbWarning.Append(inputValue.ToString());
                }
                warning = sbWarning.ToString();
            }

            StringBuilder sb = new StringBuilder();
            foreach (InputInterpretation inputValue in inputInterpretations)
            {
                sb.Append(inputValue.ToString());
            }

            // Allow 20 characters for the decoded stting itself before showing the warning.
            return string.Format("{0,-20} {1}", sb.ToString(), warning);
        }
    }

}
