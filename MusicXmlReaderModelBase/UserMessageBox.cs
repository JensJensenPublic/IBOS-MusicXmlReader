using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Simple static class for allowing anu part of the application, even way down in the business logic to show a UserMessage, for instance in case of a caught exception
    /// MAy easily be extended by parameters such as icons and buttons and even a return parameter py parafrasing the parameters fro the Windows Forms MEssageBox.
    /// </summary>
    public static class UserMessageBox
    {
        static private IUserMessageBox staticUser = null; 


        /// <summary>
        /// Initialisation to be called from the application mainform.
        /// This allows the busines logic of the application to show a MessageBox without referencing Windows.Forms
        /// </summary>
        /// <param name="user"></param>
        public static void  Init(IUserMessageBox user)
        {
            staticUser = user;
        }

        /// <summary>
        /// To be called from any method during execution in order to show a MessageBox in the 
        /// </summary>
        /// <param name="message"></param>
        public static void Show(string message)
        {
            if (null == staticUser) return;
            staticUser.ShowUserMessageBox(message);
        }

    }
}
