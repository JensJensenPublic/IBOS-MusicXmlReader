using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderUI;
using System.Globalization;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    class UserSettingsHandler
    {

        private string className = "UserSettingsHandler";
        private TreeView treeView;

        private TreeNode musicAsSound;
        private TreeNode musicAsSoundVoices;
        private TreeNode musicAsSoundDetails;

        private TreeNode musicAsText;
        private TreeNode musicAsTextVoices;
        private TreeNode musicAsTextDetails;

        private TreeNode musicAsBraille;
        private TreeNode musicAsBrailleVoices;
        private TreeNode musicAsBrailleDetails;

        // The Level-0 nedes have the following fixed indices:
        private const int MusicNodeIndex = 0;
        private const int TextNodeIndex = 1;
        private const int BrailleNodeIndex = 2;

        // The level-1 nodes Parts and Details are inserted into the level-0 nodes at fixed indices:
        private const int partsNodeIndex = 0;
        private const int detailsNodeIndex = 1;


        private MainForm mainForm;
        //private UserSettings userSettings;
        //private PartlistElement partList; 
        private Model model;

        public TreeNode MusicAsSound
        {
            get
            {
                return musicAsSound;
            }
        }

        public TreeNode MusicAsText
        {
            get
            {
                return musicAsText;
            }
        }

        public TreeNode MusicAsBraille
        {
            get
            {
                return musicAsBraille;
            }            
        }


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
            string functionName = "TreeView_AfterCheck";
            if (null == model.UserSettings)
            {
                return;
            }
            int level = e.Node.Level;
            string name = e.Node.Name;
            string text = e.Node.Text;
            int i = e.Node.Index;
            if (level == 0)
            {
                if (e.Node.Equals(musicAsSound))
                {
                    model.UserSettings.MusicAsSound = e.Node.Checked;
                }
                else if (e.Node.Equals(musicAsText))
                {
                    model.UserSettings.MusicAsSpeech = e.Node.Checked;
                }
                else if (e.Node.Equals(musicAsBraille))
                {
                    model.UserSettings.MusicAsMusicBraille = e.Node.Checked;
                }
                else
                {
                    Logger.Log(string.Format("{0}.{1}: Unexpected Node at level 1: Text={2}", className, functionName, e.Node.Text));
                }

            }
            else if (level == 2)
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


        //public void CollapseAllParts()
        //{
        //    treeView.Nodes[0].Nodes[partsNodeIndex].Collapse(); // Collapse Music.Parts
        //    treeView.Nodes[1].Nodes[partsNodeIndex].Collapse(); // Collapse Text.Parts
        //    treeView.Nodes[2].Nodes[partsNodeIndex].Collapse(); // Collapse Braille.Parts
        //}

        //public void CollapseAllDetails()
        //{
        //    treeView.Nodes[0].Nodes[detailsNodeIndex].Collapse(); // Collapse Music.Details
        //    treeView.Nodes[1].Nodes[detailsNodeIndex].Collapse(); // Collapse Text.Details
        //    treeView.Nodes[2].Nodes[detailsNodeIndex].Collapse(); // Expand Braille.Details
        //}


        public void Init()
        {
            clearAll();

            //
            // Build up the fixed part of the tree, which does not depend on the actual MusicXmlfile
            //
     
            musicAsSound  = treeView.Nodes.Insert(MusicNodeIndex ,ResourcesForUI.TreeView_MusicAsSound);
            musicAsSoundVoices = musicAsSound.Nodes.Insert(partsNodeIndex,ResourcesForUI.TreeView_MusicAsSound_Parts);
            musicAsSoundDetails = musicAsSound.Nodes.Insert(detailsNodeIndex,ResourcesForUI.TreeView_MusicAsSound_Details);

            musicAsText = treeView.Nodes.Insert(TextNodeIndex, ResourcesForUI.TreeView_MusicAsSpeech);
            musicAsTextVoices = musicAsText.Nodes.Insert(partsNodeIndex,ResourcesForUI.TreeView_MusicAsSpeech_Parts);
            musicAsTextDetails = musicAsText.Nodes.Insert(detailsNodeIndex,ResourcesForUI.TreeView_MusicAsSpeech_Details);

            musicAsBraille = treeView.Nodes.Insert(BrailleNodeIndex ,ResourcesForUI.TreeView_MusicAsBraille);
            musicAsBrailleVoices = musicAsBraille.Nodes.Insert(partsNodeIndex,ResourcesForUI.TreeView_MusicAsBraille_Parts);
            musicAsBrailleDetails = musicAsBraille.Nodes.Insert(detailsNodeIndex,ResourcesForUI.TreeView_MusicAsSound_Details);  

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
            LoadParts(musicAsTextVoices, partList);
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
            LoadDetails(musicAsTextDetails,userSettings.readerSettingsNames, model.UserSettings.readerSettingsValues);
            LoadDetails(musicAsBrailleDetails, userSettings.musicBrailleSettingsNames, model.UserSettings.musicBrailleSettingsValues);
        }

        public void CheckSelectedNotes()
        {
            // As default check all nodes at level 0 and 1;
            // The notes at level 2 are checked according to the default values set up by the model.

            musicAsText.Checked = true;
            musicAsTextVoices.Checked = true;
            musicAsTextDetails.Checked = true;

            musicAsSound.Checked = true;
            musicAsSoundVoices.Checked = true;
            musicAsSoundDetails.Checked = true;

            musicAsBraille.Checked = true;
            musicAsBrailleVoices.Checked = true;
            musicAsBrailleDetails.Checked = true;
        }


        public void ExpandAllNodes()
        {
            treeView.ExpandAll();
        }

        #region editHandlers 
        // Handle clicks in the Edit menu by expanding and collapsing nodes in the treeView 

        public void ShowMusic()
        {
            treeView.Focus();
            musicAsSound.ExpandAll();
            musicAsText.Collapse(false);
            musicAsBraille.Collapse(false);
            treeView.SelectedNode = musicAsSound;
        }

        public void ShowText()
        {
            treeView.Focus();
            musicAsText.ExpandAll();
            musicAsSound.Collapse(false);
            musicAsBraille.Collapse(false);
            treeView.SelectedNode = musicAsText;
        }

        public void ShowBraille()
        {
            treeView.Focus();
            treeView.Nodes[BrailleNodeIndex].ExpandAll();
            musicAsSound.Collapse(false);
            musicAsText.Collapse(false);
            treeView.SelectedNode = musicAsBraille;
        }


        public void ShowParts()
        {
            treeView.Focus();
            treeView.ExpandAll();
            musicAsSound.Nodes[detailsNodeIndex].Collapse(); // Collapse Music.Details
            musicAsText.Nodes[detailsNodeIndex].Collapse(); // Collapse Text.Details
            musicAsBraille.Nodes[detailsNodeIndex].Collapse(); // Expand Braille.Details  
            // Select the "Parts" node under Music representation
            treeView.SelectedNode = musicAsSound.Nodes[UserSettingsHandler.partsNodeIndex];
        }

        public void ShowDetails()
        {
            treeView.Focus();
            treeView.ExpandAll();
            musicAsSound.Nodes[partsNodeIndex].Collapse(); // Collapse Music.Parts
            musicAsText.Nodes[partsNodeIndex].Collapse(); // Collapse Text.Parts
            musicAsBraille.Nodes[partsNodeIndex].Collapse(); // Collapse Braille.Parts
            // Select the "Details" node under Music representation
            treeView.SelectedNode = musicAsSound.Nodes[UserSettingsHandler.detailsNodeIndex];
        }

        public void ShowAllItems()
        {
            treeView.Focus();
            treeView.ExpandAll();
            treeView.SelectedNode = musicAsSound.Nodes[UserSettingsHandler.detailsNodeIndex];
        }

        #endregion

    }


}
