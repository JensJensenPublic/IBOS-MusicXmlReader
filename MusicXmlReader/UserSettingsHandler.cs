using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicXmlReader
{
    class UserSettingsHandler
    {

        private TreeView treeView;

        private TreeNode musicAsSound;
        private TreeNode musicAsSoundVoices;
        private TreeNode musicAsSoundDetails;

        private TreeNode musicAsSpeech;
        private TreeNode musicAsSpeechVoices;
        private TreeNode musicAsSpeechDetails;

        private TreeNode musicAsBraille;
        private TreeNode musicAsBrailleVoices;
        private TreeNode musicAsBrailleDetails;


        // Prevent construction
        private UserSettingsHandler()
        {
        }


        private UserSettingsHandler(TreeView treeView)
        {
            this.treeView = treeView;
        }


        public static UserSettingsHandler Create(TreeView treeView)
        {
            return new UserSettingsHandler(treeView);
        }

        public void clearAll()
        {
            treeView.Nodes.Clear();
        }


        public void Init()
        {
            clearAll();

            //
            // Build up the fixed part of the tree, which does not depend on the actual MusicXmlfile
            //

            musicAsSound  = treeView.Nodes.Add("Music sound");
            musicAsSoundVoices = musicAsSound.Nodes.Add("Voices");
            musicAsSoundDetails = musicAsSound.Nodes.Add("Details");

            musicAsSpeech = treeView.Nodes.Add("Music speech");
            musicAsSpeechVoices = musicAsSpeech.Nodes.Add("Voices");
            musicAsSpeechDetails = musicAsSpeech.Nodes.Add("Details");

            musicAsBraille = treeView.Nodes.Add("Music Braille");
            musicAsBrailleVoices = musicAsBraille.Nodes.Add("Voices");
            musicAsBrailleDetails = musicAsBraille.Nodes.Add("Details");  

        }

    }
}
