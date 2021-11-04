using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder.MusicXmlHandlers
{
    /// <summary>
    /// Sectionheaders are typically placed in front of the section they belong to.
    /// This mechanism is used to show the sectionHeader on the first measure in the sectio to which it belongs.
    /// Explicitly avoid Read with side effects !!
    /// </summary> 
    public class SectionHeaderHandler
    {
        private string latestSectionHeader; // Always contains latest sectionheader found in any part
        private bool newSectionHeader;

        /// <summary>
        ///  Allow for separate parts of the code only to obtain a new sectionheader once (if so wanted)
        /// </summary>
        /// <param name="once">If set a value will only be returned to the firat caller</param>
        /// <returns></returns>
        public string GetSectionHeader(bool once)
        {
            if (once)
            {
                string result = newSectionHeader ? latestSectionHeader : null;
                newSectionHeader = false;
                return result;
            }
            return latestSectionHeader;
        }
        
        public void OnSectionHeader(string s)
        {
            latestSectionHeader = s;
            newSectionHeader = true;
        }

        private SectionHeaderHandler()
        { }


        public static SectionHeaderHandler Create()
        {
            return new SectionHeaderHandler();
        }


    }
}
