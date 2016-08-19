using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleExperimentsUI
{
    /// <summary>
    /// Takes the role of MusicXmlReaderUI.EventDesctiopion in the experimental environment
    /// Contains the information related to a line in the ListBox music representation as ToString()
    /// Contains the musicBraille part as ToMusicBraille()
    /// Contains the text part as ToTExtBraille()
    /// </summary>
    class BrailleExperimentsLineDescription
    {
        private string format;
        private string musicBrailleString;
        private string textBrailleString;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private BrailleExperimentsLineDescription()
        {
        }

         private BrailleExperimentsLineDescription(string musicBrailleString, string textBrailleString, string format)
        {
            this.musicBrailleString = musicBrailleString;
            this.textBrailleString = textBrailleString;
            this.format = format;
        }

        public static BrailleExperimentsLineDescription Create(string musicBrailleString, string textBrailleString, string format)
        {
            return new BrailleExperimentsLineDescription(musicBrailleString, textBrailleString, format);
        }

        public static BrailleExperimentsLineDescription Create(string musicBrailleString, string textBrailleString)
        {
            return new BrailleExperimentsLineDescription(musicBrailleString, textBrailleString,"{0} {1}"); // Default, used in the JAWS care
        }

        public override string ToString()
        {
            return string.Format(format, musicBrailleString, textBrailleString);
        }

        public string ToMusicBrailleString()
        {
            return musicBrailleString;
        }

        public string ToTextBrailleString()
        {
            return textBrailleString;
        }
    }
}
