using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    class MusicXmlParserException : Exception
    {
        /// <summary>
        /// Rethrow this exception when a specific error condition has beed detected during parsing of MuxicXml
        /// </summary>
        /// <param name="s"></param>
        public MusicXmlParserException(string s) : base(s)
        { }

        /// <summary>
        /// Avoid creation
        /// </summary>
        private MusicXmlParserException()
        { }

    }
}
