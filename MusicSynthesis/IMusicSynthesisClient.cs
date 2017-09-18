using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSJ.MusicSynthesis
{

    /// <summary>
    /// For routing Log-messages etc back th the client
    /// </summary>
    public interface IMusicSynthesisClient
    {
        void Log(string s);
        void LogOnce(string s);
    }

}
