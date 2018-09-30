using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// These parameters are not related specific scores (as for USerSettings) but to the generel bahaviour ogf the application.
    /// 
    /// </summary>
    public class UserPreferences
    {

        // For export of MusicBraille: 
        private int charsPerLine = 14;
        public  int CharsPerLine { get { return charsPerLine; } set { charsPerLine = value; } }
        private int linesPerForm = 20;
        public int LinesPerForm { get { return linesPerForm; } set { linesPerForm = value; } }

        private UserPreferences()
        {

        }

        static public UserPreferences Create()
        {
            return new UserPreferences();
        }
    }
}
