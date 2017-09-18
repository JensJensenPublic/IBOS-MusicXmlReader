using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSJ.MusicSynthesis
{

    public class MusicSynthesisLogger
    {
        private static IMusicSynthesisClient theClient;

        internal static void Log(string s)
        {
            if (null != theClient)
            {
                theClient.Log(s);
            }
        }

        internal static void LogOnce(string s)
        {
            if (null != theClient)
            {
                theClient.LogOnce(s);
            }
        }

        public static void Init(IMusicSynthesisClient client)
        {
            theClient = client;
        }
    }
}
