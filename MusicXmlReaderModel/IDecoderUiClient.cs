using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Allows the DecoderHandler class to call methods supplied by its (UI) client
    /// </summary>
    public interface IDecoderUiClient
    {
        /// <summary>
        /// Show a messageBox
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="messageLines"></param>
        void ShowMessageBox(string caption, List<string> messageLines);
    }  
}
