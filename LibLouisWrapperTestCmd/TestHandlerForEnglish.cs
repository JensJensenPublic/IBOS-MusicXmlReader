using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibLouisWrapperTestCmd
{
    internal class TestHandlerForEnglish : TestHandler
    {
        internal static TestHandlerForEnglish Create(string testInputDir)
        {
            return new TestHandlerForEnglish(testInputDir);
        }

        TestHandlerForEnglish(string testInputDir) : base("en-ueb-g2.ctb",testInputDir)
        {}

        internal override bool ExecuteTests()
        {
            if (!CheckWrapper()) return false;

            string englishCharacters = "abcdefghijklmnopqrstuvwxyz"; // No æøå
            for (int i = 0; ((testResult) && (i < 1)); i++)
            {
                testResult &= CharsToDotsToCharsTest(englishCharacters.ToLower());     // Seems NOT to handle Capital letters !
                testResult &= StringToDotsToStringTest(englishCharacters);               // Seems to handle Capital letters !
//              testResult &= StringToDotsToStringTFETest(englishCharacters);             // Seems to handle Capital letters !         Disabled because it seems to cause strange errors           
            }

            testResult &= RunTestFile(Path.Combine(testInputDir, "EscapeSequences.txt"));
            testResult &= RunTestFile(Path.Combine(testInputDir, "SpecialCharacters.txt")); // ";" will fail !
                                                                                               //englishResult &= RunTestFile(Path.Combine(testInputDir, "EnglishExperiment.txt"));

            testResult &= RunTestFile(Path.Combine(testInputDir, "English.txt"));
            //englishResult &= RunTestFile(Path.Combine(testInputDir, "EnglishWithoutTabs.txt")); 

            OnEndOfTestFiles("English");
            return testResult;
            
        }

    }
}
