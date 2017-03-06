using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReader
{
    class ShortcutHelp
    {

        private ShortcutHelp()
        {
        }

        private string Plus(string s1)
        {
            return s1;
        }

        private string Plus(string s1, string s2)
        {
            return s1 + "+" + s2;
        }
        

        private string Plus(string s1, string s2, string s3)
        {
            return s1 + "+" + s2 + "+" + s3;
        }

        public override string ToString()
        {     
            // The following texts must be localized!   
            const string control = "CONTROL";
            const string space = "SPACE";
            const string escape = "ESCAPE";
            const string enter = "ENTER";
            const string insert = "INSERT";
            const string alt = "ALT";
            const string tab = "TAB";
            const string followedBy = "followed by";
            const string pageDown  = "PAGEDOWN";


            StringBuilder sb = new StringBuilder();

            const string format = "{0} : {1}";
            sb.Append(string.Format(format, Plus(control, "O"),  "KLKL"));
            sb.Append(string.Format(format, Plus(alt, "F4"),     "KLKL"));
            sb.Append(string.Format(format, Plus(alt),           "KLKL"));
            sb.Append(string.Format(format, Plus(insert,space) + " " + followedBy + " s","KLKL"));
            sb.Append(string.Format(format, Combine(insert, pagedown), "KLKL"));

            for (int i = 0; (i < 50); i++)
            {
                sb.Append(string.Format("{0}\r", i));
            }
            return sb.ToString();
        }


        public static ShortcutHelp Create()
        {
            return new ShortcutHelp();
        }
    }
}
