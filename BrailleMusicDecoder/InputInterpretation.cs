namespace BrailleMusicDecoder
{

    /// <summary>
    /// Represents a possible interpretation of a sequence of BrailleMusic characters 
    /// </summary>
    class InputInterpretation
    {
        private IntegerList token; 
        public int TokenLength { get { return token.List.Count; } }
        private InputCategoryEnum inputCategory; 
        private string value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">The sequence of BrailleMusic values, that this instance represents</param>
        /// <param name="inputCategory">The category of value that this instance represents, for instance a musical note, a digit or a letter</param>
        /// <param name="value">A string representation of the instance, within the inputCategory</param>
        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value)
        {
            this.token = token;
            this.inputCategory = inputCategory;
            this.value = value;
        }


        /// <summary>
        /// A string representation of the instance, within the inputCategory
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// A string representation of this instance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0,-5} {1}={2} ", token.ToUnicodeString(), inputCategory.ToString(), value);
        }


        // The category of value that this instance represents, for instance a musical note, a digit or a letter
        internal InputCategoryEnum InputCategory
        {
            get
            {
                return inputCategory;
            }
        }

    }
}
