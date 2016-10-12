using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderUI;

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
            this.treeView.AfterCheck += TreeView_AfterCheck;
        }

        private void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            int level = e.Node.Level;
            string name = e.Node.Name;
            string text = e.Node.Text;
            // Find out what to do !
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


        private void LoadParts(TreeNode treeNode,PartlistElement partList)
        {
            treeNode.Nodes.Clear();
            for (int i = 0; (i < partList.NumberOfParts()); i++)
            {
                ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                TreeNode node = treeNode.Nodes.Add(string.Format("{0} {1}", scorePartElement.partId, scorePartElement.partName));
                //checkedListBox.SetItemChecked(i, true);
            }
            //checkedLi.CheckOnClick = true;      

        }


        /// <summary>
        /// Builds the subtres of the UserSettings tree which depend on the partlist of the currently selected MusicXml file
        /// Each part is represented by a node in each of the following 3 subtrees
        /// </summary>
        /// <param name="partList"></param>
        public void LoadParts(PartlistElement partList)
        {
            LoadParts(musicAsSoundVoices, partList);
            LoadParts(musicAsSpeechVoices, partList);
            LoadParts(musicAsBrailleVoices, partList);
        }


        private void LoadDetails(TreeNode treeNode, string[] names, bool[] values)
        {
            treeNode.Nodes.Clear();
            for (int i = 0; (i < names.Length); i++)
            {
                TreeNode node = treeNode.Nodes.Add(names[i]);
                node.Checked =  values[i];
            }
        }

        /// <summary>
        /// Loads the subtree of the UserSettings tree which depends on a statically fixed set
        /// of settings, definde my the Model.
        /// The set of nodes in the 3 trees will typically differ.
        /// </summary>
        /// <param name="userSettings"></param>
        public void LoadDetails(UserSettings userSettings)
        {
            LoadDetails(musicAsSoundDetails, userSettings.playerSettingsNames, userSettings.playerSettingsValues);
            LoadDetails(musicAsSpeechDetails,userSettings.readerSettingsNames, userSettings.readerSettingsValues);
            LoadDetails(musicAsBrailleDetails, userSettings.musicBrailleSettingsNames, userSettings.musicBrailleSettingsValues);
        }
        
    }
}
