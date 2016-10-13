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


        private MainForm mainForm;
        //private UserSettings userSettings;
        //private PartlistElement partList; 
        private Model model;


        // Prevent construction
        private UserSettingsHandler()
        {
        }


        private UserSettingsHandler(MainForm mainForm, TreeView treeView,Model model)
        {
            this.mainForm = mainForm;
            this.treeView = treeView;
            this.treeView.AfterCheck += TreeView_AfterCheck;
            this.model = model;       
        }


        /// <summary>
        /// This method is called whenever the value of a checkbox is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            int level = e.Node.Level;
            string name = e.Node.Name;
            string text = e.Node.Text;
            int i = e.Node.Index;
            if ((level == 2) && (null != model.UserSettings))
            {
                switch (e.Node.Parent.Index)
                {
                    case 0:  // Voices
                        switch (e.Node.Parent.Parent.Index)
                        {
                            case 0: model.UserSettings.partsToPlay[i] = e.Node.Checked; break;
                            case 1: model.UserSettings.partsToRead[i] = e.Node.Checked; break;
                            case 2: model.UserSettings.partsToBraille[i] = e.Node.Checked; break;
                            default: break;       
                        }
                        break;
                    case 1: // Details
                        switch (e.Node.Parent.Parent.Index) 
                        {
                            case 0: model.UserSettings.playerSettingsValues[i] = e.Node.Checked; break;
                            case 1: model.UserSettings.readerSettingsValues[i] = e.Node.Checked; break;
                            case 2: model.UserSettings.musicBrailleSettingsValues[i] = e.Node.Checked; break;
                            default: break;
                        } break;
                    default: return;
                }
            }
            // Transfer the settings to the MusicPlayer
            model.musicPlayer.UserSettings = model.UserSettings;
            mainForm.ConditionalLoadListBoxTimes();

        }

        public static UserSettingsHandler Create(MainForm mainForm,TreeView treeView,Model model)
        {
            return new UserSettingsHandler(mainForm,treeView,model);
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
                node.Checked = true; // As default enable all parts
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
            this.
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
            //this.userSettings = userSettings;
            LoadDetails(musicAsSoundDetails, userSettings.playerSettingsNames, model.UserSettings.playerSettingsValues);
            LoadDetails(musicAsSpeechDetails,userSettings.readerSettingsNames, model.UserSettings.readerSettingsValues);
            LoadDetails(musicAsBrailleDetails, userSettings.musicBrailleSettingsNames, model.UserSettings.musicBrailleSettingsValues);
        }

        public void CheckSelectedNotes()
        {
            // As default check all nodes at level 0 and 1;
            // The notes at level 2 are checked according to the default values set up by the model.

            musicAsSpeech.Checked = true;
            musicAsSpeechVoices.Checked = true;
            musicAsSpeechDetails.Checked = true;

            musicAsSound.Checked = true;
            musicAsSoundVoices.Checked = true;
            musicAsSoundDetails.Checked = true;

            musicAsBraille.Checked = true;
            musicAsBrailleVoices.Checked = true;
            musicAsBrailleDetails.Checked = true;
        }


        public void ExpandSelectedNodes()
        {
            musicAsSound.Collapse(false); // Collapse the node showing the Sound representation and all its children
            musicAsSpeech.ExpandAll();
            musicAsBraille.ExpandAll();

            //treeView.ExpandAll();
        }


    }
}
