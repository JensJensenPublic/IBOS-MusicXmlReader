using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    // Reuse the similar definitions from namespace System.Windows.Forms

    public enum ModelBaseMessageBoxIcon
    {
        None = 0,
        Hand = 16,
        Stop = 16,
        Error = 16,
        Question = 32,
        Exclamation = 48,
        Warning = 48,
        Asterisk = 64,
        Information = 64
    }

    public enum ModelBaseMessageBoxButtons
    {
        OK = 0,
        OKCancel = 1,
        AbortRetryIgnore = 2,
        YesNoCancel = 3,
        YesNo = 4,
        RetryCancel = 5
    }



    /// <summary>
    /// Simple static class for allowing anu part of the application, even way down in the business logic to show a UserMessage, for instance in case of a caught exception
    /// MAy easily be extended by parameters such as icons and buttons and even a return parameter py parafrasing the parameters fro the Windows Forms MEssageBox.
    /// </summary>
    public static class ModelBaseMessageBox
    {
 

  



        static private IModelBaseMessageBox staticUser = null; 


        /// <summary>
        /// Initialisation to be called from the application mainform.
        /// This allows the busines logic of the application to show a MessageBox without referencing Windows.Forms
        /// </summary>
        /// <param name="user"></param>
        public static void  Init(IModelBaseMessageBox user)
        {
            staticUser = user;
        }

        /// <summary>
        /// To be called from any method during execution in order to show a MessageBox in the 
        /// </summary>
        /// <param name="message"></param>
        public static void Show(string message, ModelBaseMessageBoxButtons buttons, ModelBaseMessageBoxIcon icon)
        {
            if (null == staticUser) return;
            staticUser.ShowUserMessageBox(message,buttons, icon);
        }

    }
}
