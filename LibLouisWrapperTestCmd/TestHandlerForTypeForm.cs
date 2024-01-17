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

            TypeformEnum[] plainText = new TypeformEnum[] { TypeformEnum.plain_text };
            TypeformEnum[] italicText = new TypeformEnum[] { TypeformEnum.italic};
            TypeformEnum[] underlinedText = new TypeformEnum[] { TypeformEnum.underline };
            TypeformEnum[] boldText = new TypeformEnum[] { TypeformEnum.bold };
            testResult.Result &= libLouisWrapper.TranslateStringTFE("x", out plainDots, plainText);
            testResult.Result &= libLouisWrapper.TranslateStringTFE("x", out italicDots, italicText);
            testResult.Result &= libLouisWrapper.TranslateStringTFE("x", out underlinedDots, underlinedText);
            testResult.Result &= libLouisWrapper.TranslateStringTFE("x", out boldDots, boldText);

            return testResult;            
        }

    }
}
