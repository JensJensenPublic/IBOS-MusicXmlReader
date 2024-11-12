using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    internal class LyricsHandler
    {
          
        internal string GetLyrics(EventDescriptionList events)
        {
            Logger.LogCF("(events)+");
            if (events == null) throw new ArgumentNullException();
            // This is whre the code goes !!
            Logger.LogCF("(events)-");
            return "";
        }


        internal string GetLyrics(XmlDocument doc)
        {
            Logger.LogCF("(doc)+");
            if (doc == null) throw new ArgumentNullException();
            // This is whre the code goes !!
            Logger.LogCF("(doc)-");
            return "";
        }

        private LyricsHandler()
        {         
        }


        internal static LyricsHandler Create()
        {
            return new LyricsHandler();
        }

    }
}
