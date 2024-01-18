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
        //**************************************************************************************
        //
        // https://www.pathstoliteracy.org/ueb-lesson-4-typeform-indicators-used-ueb/
        //
        //                 Italics    Underline   Bold
        // Single letter   46,23      456,23      45,23
        // Word            46,2       456,2       45,2  
        // Passage begin   46,2356    456,2356    45,2356
        // Passege end     46,3       456,3       45,3
        //
        //**************************************************************************************


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
            Log(string.Format(": TranslateStringTFE({0},[ {1}]) returned Dots[{2}]={3}", text, sb.ToString(),dots.Length, dots));
            return result;
        }


        internal override TestResult ExecuteTests()
        {
            Log("+");
            if (!CheckWrapper())
            {
                testResult.Result = false;
                return testResult;
            }

            string plainDots;
            string italicDots;
            string underlinedDots;
            string boldDots;
            string dots;

            TypeformEnum[] plainChar = new TypeformEnum[] { TypeformEnum.plain_text };
            TypeformEnum[] italicChar = new TypeformEnum[] { TypeformEnum.italic};
            TypeformEnum[] underlinedChar = new TypeformEnum[] { TypeformEnum.underline };
            TypeformEnum[] boldChar = new TypeformEnum[] { TypeformEnum.bold };
            TypeformEnum[] mixedWord = new TypeformEnum[] { TypeformEnum.plain_text, TypeformEnum.italic, TypeformEnum.underline, TypeformEnum.bold };
            TypeformEnum[] plainWord = new TypeformEnum[] { TypeformEnum.plain_text, TypeformEnum.plain_text, TypeformEnum.plain_text, TypeformEnum.plain_text };
            TypeformEnum[] italicWord = new TypeformEnum[] { TypeformEnum.italic, TypeformEnum.italic, TypeformEnum.italic, TypeformEnum.italic };
            TypeformEnum[] underlinedWord = new TypeformEnum[] { TypeformEnum.underline, TypeformEnum.underline, TypeformEnum.underline, TypeformEnum.underline };
            TypeformEnum[] boldWord = new TypeformEnum[] { TypeformEnum.bold, TypeformEnum.bold, TypeformEnum.bold, TypeformEnum.bold };

            Log(": A single character with 5 different fonttypes");
            testResult.Result &= TranslateStringTFE("a", out plainDots, plainChar);        
            testResult.Result &= TranslateStringTFE("a", out italicDots, italicChar);
            testResult.Result &= TranslateStringTFE("a", out underlinedDots, underlinedChar);
            testResult.Result &= TranslateStringTFE("a", out boldDots, boldChar);

            Log(": A 4-letter word with 4 different fonttypes");
            testResult.Result &= TranslateStringTFE("aaaa", out dots, mixedWord);
            Log("A 4-letter word with 4 identical fonttypes");
            testResult.Result &= TranslateStringTFE("aaaa", out dots, plainWord);
            testResult.Result &= TranslateStringTFE("aaaa", out dots, italicWord);
            testResult.Result &= TranslateStringTFE("aaaa", out dots, underlinedWord);
            testResult.Result &= TranslateStringTFE("aaaa", out dots, boldWord);

            Log("-");
            return testResult;            
        }

    }
}
