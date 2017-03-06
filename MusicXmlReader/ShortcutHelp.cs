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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
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
