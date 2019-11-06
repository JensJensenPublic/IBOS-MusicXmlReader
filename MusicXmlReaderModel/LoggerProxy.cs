using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    class LoggerProxy : IBrailleMusicDecoderLogger
    {
        /// <summary>
        /// For interfacing to BrailleMusicDecoder
        /// </summary>
        /// <param name="s"></param>
        public void Log(string s)
        {
            Logger.Log(s);
        }

    }
}
