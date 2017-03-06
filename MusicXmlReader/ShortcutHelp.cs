using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReader
{
    class ShortcutHelp
    {
        StringBuilder sb;

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

        private string Combine(string s1, string s2)
        {
            return string.Format("{0} : {1}", s1, s2);
        }

        private void AddLine(string line)
        {
            sb.Append(line + "\r");
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
            const string shift = "SHIFT";


            sb = new StringBuilder();


            // Følgende standard Windows / JAWS tastatur genveje kan anvendes generelt
            AddLine("-----------------------");
            AddLine(Combine(Plus(control, "O"),  "KLKL"));
            AddLine(Combine(Plus(alt, "F4"),     "KLKL"));
            AddLine(Combine(alt, "KLKL"));
            AddLine(Combine(tab, "KLKL"));
            AddLine(Combine(Plus(insert,space) + " " + followedBy + " s","KLKL"));
            AddLine(Combine(Plus(insert,pageDown), "KLKL"));
            AddLine(Combine(Plus(insert, "T"), "KLKL"));

            // Følgende specielle tastaturgenveje og kommandoer kan anvendes i forbindelse med Nodelisten
            AddLine("-----------------------");
            AddLine(Combine(Plus(control, "L"), "KLKL"));
            AddLine(Combine(Plus(control, "P"), "KLKL"));
            AddLine(Combine(Plus(control, shift, "T"), "KLKL"));
            AddLine(Combine(Plus(space), "KLKL"));
            AddLine(Combine("Tn " + enter, "KLKL"));
            AddLine(Combine("Rn,m " + enter, "KLKL"));
            AddLine(Combine("Gn " + enter, "KLKL"));
            AddLine(Combine("ESC", "KLKL"));

            // Følgende specielle tastaturgenveje kan bruges i forbindelse med redigering af nodefilteret:
            AddLine("-----------------------");
            AddLine(Combine(Plus(control, "A"), "KLKL"));
            AddLine(Combine(Plus(control, "F"), "KLKL"));
            AddLine(Combine(Plus(control, "M"), "KLKL"));
            AddLine(Combine(Plus(control, "T"), "KLKL"));
            AddLine(Combine(Plus(control, "B"), "KLKL"));
            AddLine(Combine(Plus(control, "S"), "KLKL"));
            AddLine(Combine(space, "KLKL"));
            AddLine(Combine(Plus(control, "A"), "KLKL"));
            AddLine(Combine(Plus(control, "0"), "KLKL"));
            AddLine(Combine(Plus(control, "1"), "KLKL"));

            return sb.ToString();
        }


        public static ShortcutHelp Create()
        {
            return new ShortcutHelp();
        }
    }
}
