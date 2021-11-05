using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationAnalyzer
{
    class TranslationMatrix
    {
        private List<TranslationList> transtationLists = new List<TranslationList>();

        public static TranslationMatrix Create(List<TranslationList> translationLists, RessourcesEnum resource)
        {
            return new TranslationMatrix(translationLists, resource);
        }

        private TranslationMatrix(List<TranslationList> translationLists, RessourcesEnum resource)
        {
            foreach (TranslationList tl in translationLists)
            {
                if (tl.Resource == resource)
                {
                    this.transtationLists.Add(tl);
                }
            }
        }

        /// <summary>
        /// Return the common number of items for all translationLists
        /// Check that all translationLists contain the same number of tranalations.
        /// Throw an exception if not!
        /// </summary>
        /// <returns></returns>
        private int CommonNumberOfItems()
        {          
            int numberOfItems = 0;
            foreach (TranslationList tl in transtationLists)
            {
                if ((numberOfItems != 0) && (numberOfItems != tl.NumberOfItems))
                {
                    throw new Exception("Number of items vary");
                }
                numberOfItems = tl.NumberOfItems;
            }
            return numberOfItems;
        }

        private string FormatString(int length)
        {
            return string.Format("{0}{1}{2}", "{0,-", length, "}"); // Generate format-string based on the length parameter
        }

        private string Caption(string keyFormat, string valueFormat)
        {
            StringBuilder sbCaption = new StringBuilder();
            sbCaption.Append(string.Format(keyFormat, "Identificator"));
            foreach (TranslationList tl in transtationLists)
            {
                sbCaption.Append(string.Format(valueFormat, tl.Culture.ToString()));
            }
            return sbCaption.ToString();
        }


        public List<string> ToStrings()
        {
            List<string> result = new List<string>();
            int numberOfItems = CommonNumberOfItems();
            int keyLength = 55; // Use 55 character positions for the key-string, that is: The Localization method name
            int valueLength = 40; // Use 40 character positions for each value-string, that is: The Localization method value, looked up by the .Net system.
            string keyFormat = FormatString(keyLength);
            string valueFormat = FormatString(valueLength);
            result.Add(Caption(keyFormat, valueFormat));
            StringBuilder sb = new StringBuilder();
            Translation translation00 = transtationLists[0].GetTranslation(0);   
            sb.Append(translation00.MethodInfo.ToString());

            string methodInfo0 = "";
            int itemNumber;
            for (itemNumber = 0; itemNumber < numberOfItems; itemNumber++)
            {
                sb = new StringBuilder();
                for (int cultureNumber = 0; cultureNumber < transtationLists.Count; cultureNumber++)
                {   
                    Translation translation = transtationLists[cultureNumber].GetTranslation(itemNumber);
                    string methodInfo = translation.MethodInfo.ToString();
                    if (0 == cultureNumber)
                    {
                        // Only prepend methodinfo once!
                        methodInfo0 = methodInfo;
                        sb.Append(string.Format(keyFormat, Quote(methodInfo.Remove(0,18)))); // Remove the initial "System.String get_" which is the same for all translations
                    }
                    else
                    {
                        // But check that all cultures have same methodInfo
                        if (0 != string.Compare(methodInfo0, methodInfo))
                        {
                            throw new Exception("Assertion failed!");
                        }

                    }
                    //sb.Append("    " + translation.TransatedString);
                    string valueString = string.Format(valueFormat, Quote(translation.TransatedString));
                    int startPosition = sb.Length; // The start position of the valueString to be added
                    sb.Append(valueString);
                    if (valueString.Length > valueLength)
                    {                     
                        result.Add(sb.ToString()); // Add he string even if it is too long
                        sb.Clear();
                        sb.Append('.', startPosition + valueLength);  // Leave the StringBuilder with spaces ready for adding the next valueString
                    }
                }
                result.Add(sb.ToString());
            }
            result.Add(string.Format("Number of items={0}", itemNumber));
            return result;
        }

        private string Quote(string s)
        {
            return string.Format("'{0}'",s);
        }

}


}
