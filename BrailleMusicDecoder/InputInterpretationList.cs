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
            if (null == inputValue.FriendlyValue) return;
            inputInterpretations.Add(inputValue);
        }

        public void Add(int token, InputCategoryEnum category, string value)
        {
            Add(new InputInterpretation(new IntegerList(token), category, value, InputSubCategoryEnum.None));
        }

        public void Add(int token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategory)
        {
            Add(new InputInterpretation(new IntegerList(token), category, value, inputSubCategory));
        }

        public void Add(int token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategory, InputSubSubCategoryEnum inputSubSubCategoryEnum)
        {
            Add(new InputInterpretation(new IntegerList(token), category, value, inputSubCategory, inputSubSubCategoryEnum));
        }


        public void Add(int token, InputCategoryEnum category)
        {
            Add(new InputInterpretation(new IntegerList(token), category, ""));
        }

        public void Add(int rawValue, int token, InputCategoryEnum category)
        {
            if (rawValue == token)
            {
                this.Add(token, category, "");
            }
        }

        public void Add(int rawValue, int token, InputCategoryEnum category,string value,InputSubCategoryEnum subCategory)
        {
            if (rawValue == token)
            {
                this.Add(token, category, value,subCategory);
            }
        }


        //private void Add(IntegerList token, InputCategoryEnum category, string value)
        //{
        //    Add(new InputInterpretation(token, category, value));
        //}

        //private void Add(IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum)
        //{
        //    Add(new InputInterpretation(token, category, value, inputSubCategoryEnum));
        //}

        //private void Add(IntegerList token, InputCategoryEnum category, string value, InputSubSubCategoryEnum inputSubSubCategory)
        //{
        //    Add(new InputInterpretation(token, category, value, InputSubCategoryEnum.None,inputSubSubCategory));
        //}

        //private void Add(IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum, string inputSubCategoryValue)
        //{
        //    Add(new InputInterpretation(token, category, value, inputSubCategoryEnum, inputSubCategoryValue));
        //}

        //private void Add(IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum, InputSubSubCategoryEnum inputSubSubCategory)
        //{
        //    Add(new InputInterpretation(token, category, value, inputSubCategoryEnum, inputSubSubCategory));
        //}        

        //private void Add(IntegerList token, InputCategoryEnum category, string value,List<string> values)
        //{
        //    Add(new InputInterpretation(token, category, value, values));
        //}

        //private void Add(IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum subCategory, List<string> values)
        //{
        //    Add(new InputInterpretation(token, category, value, subCategory, values));
        //}



        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category)
        {
            if (rawValues.StartsWith(token))
            {
                this.Add(new InputInterpretation(token, category, ""));
//                this.Add(token, category, "");
            }
        }


        /// <summary>
        /// For specifying an interpretation through a list of lists of values. For example for intrepretating a sequence of digits as a number
        /// </summary>
        /// <param name="rawValues">The raw values to attempt to interpret</param>
        /// <param name="values">The values to look fpr</param>
        /// <param name="category">The catgory to report if found</param>
        /// <param name="subCategory">The subcatgory to report if found</param>
        public void Add(IntegerList rawValues, List<MusicBrailleMappingList> values, InputCategoryEnum category, InputSubCategoryEnum subCategory)
        {
            if (rawValues.Count < values.Count) return; // At the end of the file rawValues may be of any small size > 0
            int length = values.Count; // Find the number of values pairs to compare
            IntegerList token = new IntegerList();  // Here we collect the input used for this interpretation
            StringBuilder sb = new StringBuilder(); // Here we collect the interpretation as a string
            List<string> parameters = new List<string>(); // Here we collect the information as a list of strings        
            for (int i = 0; i < length; i++)
            {
                int rawValue = rawValues.List[i];
                MusicBrailleMapping mapResult = values[i].Map(rawValue);
                if (null == mapResult) return; // Return without adding on the first rawValue not found
                string mapResultString = mapResult.ToString();
                sb.Append(mapResultString);
                parameters.Add(mapResultString);
                token.Add(rawValue);              
            }
            // If all values could be mapped we add the interpretation found:
            string friendlyText = sb.ToString();
            this.Add(rawValues, token, category, friendlyText, subCategory, parameters);
        }

        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value)
        {
            if (rawValues.StartsWith(token))
            {
                this.Add(new InputInterpretation(token, category, value));
//                this.Add(token, category, value);
            }
        }

        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum)
        {
            if (rawValues.StartsWith(token))
            {
                this.Add(new InputInterpretation(token, category, value, inputSubCategoryEnum));
//                this.Add(token, category, value, inputSubCategoryEnum);
            }
        }

      public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum, List<string> parameters)
        {
            if (rawValues.StartsWith(token))
            {
                Add(new InputInterpretation(token, category, value, inputSubCategoryEnum, parameters));
                //this.Add(token, category, value, inputSubCategoryEnum,parameters);
            }
        }

        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value, InputSubSubCategoryEnum inputSubSubCategoryEnum)
        {
            if (rawValues.StartsWith(token))
            {
                Add(new InputInterpretation(token, category, value, InputSubCategoryEnum.None, inputSubSubCategoryEnum));
                //this.Add(token, category, value, inputSubSubCategoryEnum);
            }
        }



        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum, string inputSubCategoryValue)
        {
            if (rawValues.StartsWith(token))
            {
                Add(new InputInterpretation(token, category, value, inputSubCategoryEnum, inputSubCategoryValue));
                //this.Add(token, category, value, inputSubCategoryEnum, inputSubCategoryValue);
            }
        }


        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum, InputSubSubCategoryEnum inputSubSubCategory)
        {
            if (rawValues.StartsWith(token))
            {
                Add(new InputInterpretation(token, category, value, inputSubCategoryEnum, inputSubSubCategory));
                //this.Add(token, category, value, inputSubCategoryEnum, inputSubSubCategory);
            }
        }


        /// <summary>
        /// The "One size fits all version"
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="token"></param>
        /// <param name="category"></param>
        /// <param name="value"></param>
        /// <param name="inputSubCategoryEnum"></param>
        /// <param name="inputSubCategoryValue"></param>
        /// <param name="inputSubSubCategory"></param>
        /// <param name="values"></param>
        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value, InputSubCategoryEnum inputSubCategoryEnum, string inputSubCategoryValue,InputSubSubCategoryEnum inputSubSubCategory, List<string> values)
        {
            if (rawValues.StartsWith(token))
            {
                Add(new InputInterpretation(token, category, value, inputSubCategoryEnum, inputSubCategoryValue,inputSubSubCategory,values));
                //this.Add(token, category, value, inputSubCategoryEnum, inputSubSubCategory);
            }
        }


        public void Add(IntegerList rawValues, IntegerList token, InputCategoryEnum category, string value, List<string> values )
        {
            if (rawValues.StartsWith(token))
            {
                Add(new InputInterpretation(token, category, value, values));
                //this.Add(token, category, value, values);
            }
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
                if (0 != (inputValue.Category & allowedInputCategories))
                {
//                    ulong iCat = (ulong)inputValue.Category; // For debugging
//                    ulong iAllowed = (ulong)allowedInputCategories; // For debugging
                    result.Add(inputValue);
                }
            }
            return result;
        }

        public InputInterpretationList Prioritize()
        {
            // We do not want to modify during foreach so we do a little extra work
            if (this.Count <= 1) return this;
            // Determine the max size
            int maxSize = 0;
            foreach (InputInterpretation inputInterpretation in this.inputInterpretations)
            {
                int size = inputInterpretation.TokenLength;
                if (size > maxSize) maxSize = size;
            }
            // Get a list of all items with size less than maxSize
            List<InputInterpretation> itemsToRemove = new List<InputInterpretation>();
            foreach (InputInterpretation inputInterpretation in this.inputInterpretations)
            {
                int size = inputInterpretation.TokenLength;
                if (size < maxSize) itemsToRemove.Add(inputInterpretation);
            }
            // Remove all lists to remove from the original list
            foreach (InputInterpretation itemToRemove in itemsToRemove)
            {
                this.inputInterpretations.Remove(itemToRemove);
            }
            return this;
        }

        public int Count { get { return inputInterpretations.Count; } }

        public override string ToString()
        {
            throw new System.Exception("");
        }


        public string ToString(Options rawOptions)
        {
            string warning = "";
            if (inputInterpretations.Count != 1)
            {
                StringBuilder sbWarning = new StringBuilder();
                sbWarning.Append(string.Format("{0} ->Warning: {1} interpretations found", rawValues.ToUnicodeString(), inputInterpretations.Count));
                foreach (InputInterpretation inputValue in inputInterpretations)
                {
                    sbWarning.Append(inputValue.ToString(rawOptions));
                }
                warning = sbWarning.ToString();
            }

            StringBuilder sb = new StringBuilder();
            foreach (InputInterpretation inputValue in inputInterpretations)
            {
                sb.Append(inputValue.ToString(rawOptions));
            }

            string values = (inputInterpretations.Count == 0) ? "NONE" : sb.ToString();

            // Allow 20 characters for the decoded stting itself before showing the warning.
            return string.Format("{0,-20} {1}", values, warning);
        }
    }

}
