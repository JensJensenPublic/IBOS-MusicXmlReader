using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// Allows the Decoder class to call methods supplied by its client
    /// </summary>
    public interface IDecoderClient
    {
        /// <summary>
        /// Show a messageBox
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="messageLines"></param>
        void ShowMessageBox(string caption, List<string> messageLines);
    }
}
