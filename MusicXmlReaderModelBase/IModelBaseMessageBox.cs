using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public interface IModelBaseMessageBox
    {
        void ShowUserMessageBox(string Message, ModelBaseMessageBoxButtons buttons, ModelBaseMessageBoxIcon icon);
    }
}
