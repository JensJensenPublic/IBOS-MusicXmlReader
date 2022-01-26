using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrailleReader
{
    public interface IMusicBrailleReaderClient
    {
        string GetLatestBrailleMusicPath();
        void SetLatestBrailleMusicPath(string s);
        string GetMyMusicXmlDirectory();
    }
}
