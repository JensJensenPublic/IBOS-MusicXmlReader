using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{
    class MusicXmlKeySignatureHandler
    {
        private int fifths = 0; // Dafaults to 0, that is: C major or A minor
        public int Fifths { get { return fifths; } }

        private void LogCF(string s)
        {
            Logger.LogCF1(s);
        }

        MusicXmlKeySignatureHandler()
        {
            fifths = 0;
        }

        private MusicXmlKeySignatureHandler(string signature)
        {
            LogCF(string.Format(": Signature = {0}", signature));
            fifths = 0;
            bool natural = !(('+' == signature[0]) || ('-' == signature[0])) ;
            bool ok = int.TryParse(signature, out fifths);
            if (ok)
            {
                if (natural)
                {
                    fifths = 0;
                }    
                return;
            }
            LogCF(string.Format(": Invalid input='{0}' Must be an integer", signature));
        }

        public static MusicXmlKeySignatureHandler Create()
        {
            return new MusicXmlKeySignatureHandler();
        }

        public static MusicXmlKeySignatureHandler Create(string signature)
        {
            return new MusicXmlKeySignatureHandler(signature);
        }
    }
}
