using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel; // Namespace, not reference!

namespace BrailleMusicDecoder
{
    public enum RegionalOptionsEnum { UnKnown = 0, AutoSelect = 1, English=2, Danish=3 }

    /// <summary>
    /// For implementing regional differences such as contractions.
    /// This does NOT include Windows "localization" which is handled in the default Windows way using resource files and language codes!
    /// </summary>
    public abstract class RegionalOptions
    {

        protected const int noDots = TokenReader.noDots;
        protected const int dot1 = TokenReader.dot1;
        protected const int dot2 = TokenReader.dot2;
        protected const int dot3 = TokenReader.dot3;
        protected const int dot4 = TokenReader.dot4;
        protected const int dot5 = TokenReader.dot5;
        protected const int dot6 = TokenReader.dot6;

        public abstract string ExpandContraction(int dots);
        public abstract int Contract(string s);
        

        public static RegionalOptions Create(RegionalOptionsEnum regionalOptionsEnum)
        {
            switch (regionalOptionsEnum)
            {
                case RegionalOptionsEnum.Danish: return RegionalOptionsDanish.Create();
                case RegionalOptionsEnum.English: return RegionalOptionsEnglish.Create();
            }
            // Establish a default
            return RegionalOptionsDanish.Create();
        }
        
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/English_Braille
    /// https://www.teachingvisuallyimpaired.com/uploads/1/4/1/2/14122361/ueb_braille_chart.pdf
    /// </summary>
    public class RegionalOptionsEnglish : RegionalOptions
    {
        public override string ExpandContraction(int dots)
        {
            switch (dots)
            {
                case dot1 | dot2 | dot5 | dot6: return "ou";
                case dot1 | dot2 | dot3 | dot4 | dot6: return "and";
                case dot1 | dot2 | dot4 | dot6: return "ed";
                case dot3 | dot5: return "in";
                case dot2 | dot6: return "en"; // USed for "?" in Danish
                case dot2 | dot3 | dot4 | dot6: return "the";
                case dot3 | dot5 | dot6: return "by";
                //                case dot3 | dot4 | dot6: return "ING"; // Only while terminating a string. To be implemented!
                case dot1 | dot2 | dot3 | dot4 | dot5 | dot6: return "for";
                case dot1 | dot2 | dot3 | dot5 | dot6: return "of";
                case dot1 | dot2 | dot4 | dot5 | dot6: return "er";
                case dot1 | dot4 | dot5 | dot6: return "this";
                case dot3 | dot4 | dot5: return "ar"; // Replaces Danish "Æ" 
                case dot1 | dot6 : return "ch"; // Replaces Danish "Å" 
                case dot2 | dot4 | dot6: return "ow"; // Replaces Danish "Ø"
                case dot4 | dot6: return ".";
                case dot5 | dot6: return "."; // "Final letter abbreviation"
                case dot5: return "."; // "Abbreviation mark"
                case dot3 | dot4 | dot6: return "ing";  
            }
            return null;
        }

        public override int Contract(string s)
        {
            Logger.LogCF(" Not implemented yet");
            return -1;
        }

        private RegionalOptionsEnglish()
        { }

        public static RegionalOptionsEnglish Create()
        {
            return new RegionalOptionsEnglish();
        }

    }

    public class RegionalOptionsDanish : RegionalOptions
    {
        public override string ExpandContraction(int dots)
        {
            switch (dots)
            {
                case dot3 | dot4 | dot5: return "æ"; // Danish "Æ", not a real contraction, but a special letter 
                case dot1 | dot6: return "å"; //  Danish "Å" , not a real contraction, but a special letter
                case dot2 | dot4 | dot6: return "ø"; //  Danish "Ø" , not a real contraction, but a special letter
                case TokenReader.dot26: return "?"; // In Danish dot26 is used as "?" In English it is used as a contraction for "en"
            }
            return null;
        }
        
        public override int Contract(string s)
        {
            Logger.LogCF(" Not implemented yet");
            return -1;
        }

        private RegionalOptionsDanish()
        { }
        


        public static RegionalOptionsDanish Create()
        {
            return new RegionalOptionsDanish();
        }

    }



}
