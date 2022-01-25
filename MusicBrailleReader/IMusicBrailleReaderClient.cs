using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrailleReader
{
    public interface IMusicBrailleReaderClient
    {
        void HideForm();
        void ShowForm();
        void EnableForm(bool b);
        string GetLatestBrailleMusicPath();
        void SetLatestBrailleMusicPath(string s);
        string GetMyMusicXmlDirectory();
    }
}
