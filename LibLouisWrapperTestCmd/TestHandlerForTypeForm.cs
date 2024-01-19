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

        // Some simple shorthands to reduce amount of text:
        public const TypeformEnum Plain = TypeformEnum.plain_text;
        public const TypeformEnum Italic = TypeformEnum.italic;
        public const TypeformEnum Underline = TypeformEnum.underline;
        public const TypeformEnum Bold = TypeformEnum.bold;

        private new TypeformEnum[] repeatedTypeformEnum(TypeformEnum tfe, int count)
        {

            TypeformEnum[] result = new TypeformEnum[count];
            for (int i = 0; (i < count); i++)
            {
                result[i] = tfe;
            }
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

            Log(": A single LETTER with 5 different fonttypes");
            testResult.Result &= TranslateStringTFE("a", out plainDots, new TypeformEnum[] { Plain });        
            testResult.Result &= TranslateStringTFE("a", out italicDots, new TypeformEnum[] { Italic });
            testResult.Result &= TranslateStringTFE("a", out underlinedDots, new TypeformEnum[] { Underline });
            testResult.Result &= TranslateStringTFE("a", out boldDots, new TypeformEnum[] { Bold });

            Log(": A 4-letter WORD with 4 different fonttypes");
            testResult.Result &= TranslateStringTFE("aaaa", out dots, new TypeformEnum[] { Plain, Italic, Underline, Bold });
            Log("A 4-letter WORD with identical fonttypes");
            testResult.Result &= TranslateStringTFE("aaaa", out dots, new TypeformEnum[] { Plain, Plain, Plain, Plain });
            testResult.Result &= TranslateStringTFE("aaaa", out dots, new TypeformEnum[] { Italic, Italic, Italic, Italic });
            testResult.Result &= TranslateStringTFE("aaaa", out dots, new TypeformEnum[] { Underline, Underline, Underline, Underline });
            testResult.Result &= TranslateStringTFE("aaaa", out dots, new TypeformEnum[] { Bold, Bold, Bold, Bold });

            Log(": A 4-word PASSAGE with identical fonttypes");
            string passage = "aaaa aaaa aaaa aaaa";
            testResult.Result &= TranslateStringTFE(passage, out dots, repeatedTypeformEnum(Plain, passage.Length));
            testResult.Result &= TranslateStringTFE(passage, out dots, repeatedTypeformEnum(Italic, passage.Length));
            testResult.Result &= TranslateStringTFE(passage, out dots, repeatedTypeformEnum(Underline, passage.Length));
            testResult.Result &= TranslateStringTFE(passage, out dots, repeatedTypeformEnum(Bold, passage.Length));

            Log("-");
            return testResult;            
        }

    }
}
