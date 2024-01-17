using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibLouisWrapper.Wrapper;
using static System.Net.Mime.MediaTypeNames;

namespace LibLouisWrapperTestCmd
{
    internal class TestHandlerForTypeForm : TestHandler
    {
        internal static TestHandlerForTypeForm Create(string testInputDir)
        {
            return new TestHandlerForTypeForm(testInputDir);
        }

        TestHandlerForTypeForm(string testInputDir) : base("en-ueb-g2.ctb",testInputDir)
        {}

        private bool TranslateStringTFE(string text, out string dots, TypeformEnum[] tfs)
        { 
            bool result = libLouisWrapper.TranslateStringTFE(text, out dots, tfs);
            StringBuilder sb = new StringBuilder();
            foreach (TypeformEnum ft in tfs)
            {
                sb.Append(ft.ToString() + " ");
            }
            Log(string.Format("TranslateStringTFE({0},{1}) returned Dots={2}", text, sb.ToString(), dots));
            return result;
        }


        internal override TestResult ExecuteTests()
        {
            if (!CheckWrapper())
            {
                testResult.Result = false;
                return testResult;
            }

            string plainDots;
            string italicDots;
            string underlinedDots;

            string boldDots;
            string text = "a"; 

            TypeformEnum[] plainText = new TypeformEnum[] { TypeformEnum.plain_text };
            TypeformEnum[] italicText = new TypeformEnum[] { TypeformEnum.italic};
            TypeformEnum[] underlinedText = new TypeformEnum[] { TypeformEnum.underline };
            TypeformEnum[] boldText = new TypeformEnum[] { TypeformEnum.bold };
            testResult.Result &= TranslateStringTFE(text, out plainDots, plainText);        
            testResult.Result &= TranslateStringTFE(text, out italicDots, italicText);
            testResult.Result &= TranslateStringTFE(text, out underlinedDots, underlinedText);
            testResult.Result &= TranslateStringTFE(text, out boldDots, boldText);

            return testResult;            
        }

    }
}
