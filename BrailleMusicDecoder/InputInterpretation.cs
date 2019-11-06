using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    class InputInterpretation
    {
        private IntegerList token;
        public IntegerList Token { get { return token; } }
        public int TokenLength { get { return token.List.Count; } }
        private InputCategoryEnum inputCategory;
        private string value;
        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value)
        {
            this.token = token;
            this.inputCategory = inputCategory;
            this.value = value;
        }


        public string Value
        {
            get
            {
                return value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0,-5} {1}={2} ", token.ToUnicodeString(), inputCategory.ToString(), value);
        }


        internal InputCategoryEnum InputCategory
        {
            get
            {
                return inputCategory;
            }
        }


    }
}
