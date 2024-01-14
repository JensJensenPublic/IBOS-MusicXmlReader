using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibLouisWrapperTestCmd
{
    class TestHandlerForDanish : TestHandler
    {
        internal static TestHandlerForDanish Create(string testInputDir)
        {
            return new TestHandlerForDanish(testInputDir);
        }

        TestHandlerForDanish(string testInputDir) : base("da-dk-g26.ctb",testInputDir) //  Danish table for 6 dots grade 2 forward and backward translation (2022)
        {}

        internal override bool ExecuteTests()
        {
            if (!CheckWrapper()) return false;

            //string text = "The quick brown fox jumps over the lazy dog";
            string danishCharacters = "abcdefghijklmnopqrstuvwxyzæøå";

            for (int i = 0; ((testResult) && (i < 1)); i++)
            {
                testResult &= CharsToDotsToCharsTest(danishCharacters.ToLower());     // Seems NOT to handle Capital letters !
                testResult &= StringToDotsToStringTest(danishCharacters);               // Seems to handle Capital letters !
                //testResult &= StringToDotsToStringTFETest(danishCharacters);             // Seems to handle Capital letters !       Disabled because it seems to cause strange errors        
            }

            // Run explicitly named testfiles
            testResult &= RunTestFile(Path.Combine(testInputDir, "Danish.txt"));
            testResult &= RunTestFile(Path.Combine(testInputDir, "DanishGraphics.txt")); // https://blind.dk/punktskrift-2022    Den danske punktskrift 2022    "÷" will fail       
            testResult &= RunTestFile(Path.Combine(testInputDir, "SpecialCharacters.txt"));
            testResult &= RunTestFile(Path.Combine(testInputDir, "EscapeSequences.txt"));


            OnEndOfTestFiles("Danish");

            return testResult;
        }
    }
}
