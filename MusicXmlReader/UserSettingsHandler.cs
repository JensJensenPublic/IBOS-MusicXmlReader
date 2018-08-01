using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// using MusicXmlReaderUI;
using System.Globalization;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    public class UserSettingsHandler
    {
        public enum CheckboxOperation { Unknown, Check, Uncheck, ToggleAndCopy };
        public enum CheckboxRelation  { Unknown, SameName, SameParent };

        private string className = "UserSettingsHandler";
        private bool consoleTrace = false;
        private TreeView treeView;
        private ListBoxTimesHandler listBoxTimesHandler;

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


        private UserSettingsHandler(TreeView treeView,Model model, ListBoxTimesHandler listBoxTimesHandler)
        {  
            this.treeView = treeView;
            this.listBoxTimesHandler = listBoxTimesHandler;
            this.treeView.AfterCheck += TreeView_AfterCheck;
            this.model = model;
            this.treeView.AccessibleName = ResourcesForUI.TreeView_Accessible_Name;
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(TreeView_KeyDown);
            this.treeView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TreeView_KeyPress);
            this.treeView.KeyUp += new System.Windows.Forms.KeyEventHandler(TreeView_KeyUp);
            this.treeView.Enter += TreeView_Enter;
            this.treeView.Leave += TreeView_Leave;
        }

        private void TreeView_Leave(object sender, EventArgs e)
        {
            this.treeView.BackColor = MainForm.NonFocusedColor;
        }

        private void TreeView_Enter(object sender, EventArgs e)
        {
            this.treeView.BackColor = MainForm.FocusedColor;
        }

        public void Reset()
        {
            treeView.CollapseAll();
            treeView.Refresh();
        }

        private void TreeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (consoleTrace) Console.WriteLine("userSettingsTreeView_KeyPress"); 
        }

        private void TreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (consoleTrace) Console.WriteLine("userSettingsTreeView_KeyUp");
        }


        private bool UpdateNotesWithSameName(bool newValue, int level)
        {
            // We only handle level 2 nodes
            if (2 != level) return false;

            // Locate and check/uncheck all nodes with same parent-name and same node-name
            string level1Name = treeView.SelectedNode.Parent.Name;
            string level2Text = treeView.SelectedNode.Text;
            foreach (TreeNode level0Node in treeView.Nodes)
            {
                foreach (TreeNode level1Node in level0Node.Nodes)
                {
                    if (0 == string.Compare(level1Name, level1Node.Name))
                    {
                        foreach (TreeNode level2Node in level1Node.Nodes)
                        {
                            if (0 == string.Compare(level2Text, level2Node.Text))
                            {
                                level2Node.Checked = newValue;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool UpdateNotesWithSameParent(bool newValue, int level)
        {      
            TreeNodeCollection nodes;
            switch (level) // We only handle level 0 nodes and 2 nodes
            {
                case 0: nodes = treeView.Nodes; break; // Level 0 nodes have no parent !
                case 2: nodes = treeView.SelectedNode.Parent.Nodes; break;
                default: return false;
            }

            foreach (TreeNode node in nodes)
            {
                if (node != treeView.SelectedNode)
                {
                    node.Checked = newValue;
                }
            }
            return true;
        }


        /// <summary>
        /// Candle chsckboxes, distributed in the tree
        /// </summary>
        /// <param name="checkboxOperation"></param>
        /// <returns></returns>
        public bool UpdateCheckBoxes(CheckboxOperation checkboxOperation, CheckboxRelation checkboxRelation)
        {
            string functionName = "UpdateCheckBoxes";
            Logger.Log(string.Format("{0}.{1}({2},{3})", className, functionName, checkboxOperation, checkboxRelation));
            bool result = false;
            bool newValue;
            // We only handle the shortcuts specified in shortCutHandler
            switch (checkboxOperation)
            {
                case CheckboxOperation.Check: newValue = true; break;
                case CheckboxOperation.Uncheck: newValue = false; break;
                case CheckboxOperation.ToggleAndCopy: newValue = !treeView.SelectedNode.Checked; result = true; break;
                default: return false;
            }

            // This only has meaning if a node is selected !
            if (null == treeView.SelectedNode) return result;


            bool saveAutoReload = listBoxTimesHandler.AutoReload;
            listBoxTimesHandler.AutoReload = false; // Avoid loading the listbox for each and every change

            switch (checkboxRelation)
            {
                case CheckboxRelation.SameName: UpdateNotesWithSameName(newValue, treeView.SelectedNode.Level); break;
                case CheckboxRelation.SameParent: UpdateNotesWithSameParent(newValue, treeView.SelectedNode.Level); break;
                default: break; // TODO fix this case
            }

            this.treeView.Refresh(); // Refresh the treeView before we start refreshing the listbox  (which may take some time !)

#warning Maybe we should not refresh the listbox until it gets focus ??

            listBoxTimesHandler.AutoReload = saveAutoReload; // Restore
            listBoxTimesHandler.ConditionalLoad(); // Reload once instead of multiple times
            return result;
        }


        ///// <summary>
        ///// Update all other checkboxes on this lecel in this subtree
        ///// For instance in order to turn all other voices off or on
        ///// </summary>
        ///// <param name="checkboxOperation"></param>
        //public void UpdateOtherCheckboxes(CheckboxOperation checkboxOperation)
        //{
        //    string functionName = "UpdateOtherCheckboxes";
        //    //Logger.Log(string.Format("{0}.{1}({2}) Not implemented yet !!", className, functionName, checkboxOperation));
        //    //UiUtilities.Beep();
        //    UpdateCheckBoxes(checkboxOperation, CheckboxRelation.SameParent);
        //}


        /// <summary>
        /// Occurs when a key is pressed while treeView has focus         
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TreeView_KeyDown(object sender, KeyEventArgs e)
        {

            if (consoleTrace) Console.WriteLine("userSettingsTreeView_KeyDown");
            if (e.KeyData == ShortcutHandler.listBoxFocus)
            {
                listBoxTimesHandler.Focus(); // Easy way to move the focus to the main listbox
                e.SuppressKeyPress = true;
                return;
            }


            if (null == treeView.SelectedNode)
            {
                e.SuppressKeyPress = true;
                return;
            }


            switch (e.KeyData)
            {
                case ShortcutHandler.uncheckOthers: e.Handled = UpdateCheckBoxes(CheckboxOperation.Uncheck, CheckboxRelation.SameParent); e.SuppressKeyPress = true; break;
                case ShortcutHandler.checkOthers:   e.Handled = UpdateCheckBoxes(CheckboxOperation.Check, CheckboxRelation.SameParent); e.SuppressKeyPress = true; break;
                // For the time being the cneckAll and uncheckAll commands are called through the menuline, which is not formally correct, 
                // because they should only be active when the Treeview has focus. They may be activated by the 2 lines below !
                //case ShortcutHandler.uncheckAll: e.Handled = UpdateCheckBoxes(CheckboxOperation.Uncheck, CheckboxRelation.SameParent); e.SuppressKeyPress = true; break;
                //case ShortcutHandler.checkAll: e.Handled = UpdateCheckBoxes(CheckboxOperation.Check, CheckboxRelation.SameParent); e.SuppressKeyPress = true; break;
                default: break;
            }

            // Otherwise let the treeview itself handle it

        }




        /// <summary>
        /// This method is called whenever the value of a checkbox is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            string functionName = "TreeView_AfterCheck";
            if (consoleTrace) Console.WriteLine(functionName);
            TreeNode level0Node = null;
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
                level0Node = e.Node;
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
                level0Node = e.Node.Parent.Parent;
                switch (e.Node.Parent.Index)
                {
                    case 0:  // Voices
                        switch (e.Node.Parent.Parent.Index)
                        {
                            case 0: model.UserSettings.SetPartsToPlay(i, e.Node.Checked); break;
                            case 1: model.UserSettings.SetPartsToRead(i, e.Node.Checked); break;
                            case 2: model.UserSettings.SetPartsToBraille(i,e.Node.Checked); break;
                            default: break;       
                        }
                        break;
                    case 1: // Details
                        switch (e.Node.Parent.Parent.Index) 
                        {
                            case 0: model.UserSettings.SetPlayerSettings(i,e.Node.Checked); break;
                            case 1: model.UserSettings.SetReaderSettings(i,e.Node.Checked); break;
                            case 2: model.UserSettings.SetMusicBrailleSettings(i,e.Node.Checked); break;
                            default: break;
                        } break;
                    default: return;
                }
            }
            // Transfer the settings to the MusicPlayer
            model.musicPlayer.UserSettings = model.UserSettings;

            if (!((null != level0Node) && (0 == level0Node.Index)))
            {
                // Skip the update of the Listbox if this was a change of a sound parameter, which is not reflected there.
                listBoxTimesHandler.ConditionalLoad();
            }

        }

        public static UserSettingsHandler Create(TreeView treeView,Model model, ListBoxTimesHandler listBoxTimesHandler)
        {
            return new UserSettingsHandler(treeView,model,listBoxTimesHandler);
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

        private string NoAmp(string s)
        {
            return Utilities.RemoveAmpersant(s);
        }



        /// <summary>
        /// Overwrite relwvant node names with localized texts
        /// </summary>
        public void Init()
        {
            clearAll();

            //
            // Build up the fixed part of the tree, which does not depend on the actual MusicXmlfile
            // The .Name in level 1 nodes is used for horisontal navigation between nodes with similar semantics.
            // The losalized textstrings may contain "&" because they are also used for assigning localized menu shortcuts !! All occurrences of "&" are removed !! 
            //
            string f = " " + ResourcesForUI.Treeview_for + " ";

            musicAsSound  = treeView.Nodes.Insert(MusicNodeIndex ,NoAmp(ResourcesForUI.TreeView_MusicAsSound));
            musicAsSoundVoices = musicAsSound.Nodes.Insert(partsNodeIndex,NoAmp(ResourcesForUI.TreeView_MusicAsSound_Parts + f + ResourcesForUI.TreeView_MusicAsSound));
            musicAsSoundVoices.Name = NoAmp(ResourcesForUI.TreeView_MusicAsSound_Parts);
            musicAsSoundDetails = musicAsSound.Nodes.Insert(detailsNodeIndex,NoAmp(ResourcesForUI.TreeView_MusicAsSound_Details + f + ResourcesForUI.TreeView_MusicAsSound));
            musicAsSoundDetails.Name = NoAmp(ResourcesForUI.TreeView_MusicAsSound_Details);

            musicAsText = treeView.Nodes.Insert(TextNodeIndex, NoAmp(ResourcesForUI.TreeView_MusicAsSpeech));
            musicAsTextVoices = musicAsText.Nodes.Insert(partsNodeIndex,NoAmp(ResourcesForUI.TreeView_MusicAsSpeech_Parts + f + ResourcesForUI.TreeView_MusicAsSpeech));
            musicAsTextVoices.Name = NoAmp(ResourcesForUI.TreeView_MusicAsSpeech_Parts);
            musicAsTextDetails = musicAsText.Nodes.Insert(detailsNodeIndex,NoAmp(ResourcesForUI.TreeView_MusicAsSpeech_Details + f + ResourcesForUI.TreeView_MusicAsSpeech));
            musicAsTextDetails.Name = NoAmp(ResourcesForUI.TreeView_MusicAsSpeech_Details);

            musicAsBraille = treeView.Nodes.Insert(BrailleNodeIndex ,NoAmp(ResourcesForUI.TreeView_MusicAsBraille));
            musicAsBrailleVoices = musicAsBraille.Nodes.Insert(partsNodeIndex,NoAmp(ResourcesForUI.TreeView_MusicAsBraille_Parts + f + ResourcesForUI.TreeView_MusicAsBraille));
            musicAsBrailleVoices.Name = NoAmp(ResourcesForUI.TreeView_MusicAsBraille_Parts);
            musicAsBrailleDetails = musicAsBraille.Nodes.Insert(detailsNodeIndex,NoAmp(ResourcesForUI.TreeView_MusicAsSound_Details + f + ResourcesForUI.TreeView_MusicAsBraille));
            musicAsBrailleDetails.Name = NoAmp(ResourcesForUI.TreeView_MusicAsSound_Details);
        }

        /// <summary>
        /// Initialize a treeNode with children representing the parts defined in the List of Parts.
        /// The initial value of each child is loadeed from model.UserSettings, which again contains values found in the ".xml.IBOS" settings file for the MusicXml file
        /// </summary>
        /// <param name="treeNode">The TreeNode to initialize</param>
        /// <param name="partList">The List of parts (previously loaded from the MusicXml file)</param>
        /// <param name="category">The UserSettings category : { Speech, Sound or MusicBraille}</param>
        private void LoadParts(TreeNode treeNode,PartlistElement partList,UserSettings.Category category)
        {
            treeNode.Nodes.Clear();
            for (int i = 0; (i < partList.NumberOfParts()); i++)
            {
                ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                TreeNode node = treeNode.Nodes.Add(string.Format("{0} {1}", scorePartElement.partId, scorePartElement.partName));
                bool b = model.UserSettings.GetParts(category,i);                 
                node.Checked = b;
            } 
        }


        /// <summary>
        /// Builds the subtres of the UserSettings tree which depend on the partlist of the currently selected MusicXml file
        /// Each part is represented by a node in each of the following 3 subtrees
        /// </summary>
        /// <param name="partList"></param>
        public void LoadParts(PartlistElement partList)
        {
            LoadParts(musicAsSoundVoices, partList,UserSettings.Category.Sound);
            LoadParts(musicAsTextVoices, partList,UserSettings.Category.Speech);
            LoadParts(musicAsBrailleVoices, partList,UserSettings.Category.MusicBraille);
        }

        private void LoadDetails(TreeNode treeNode, UserSetting[] settings)
        {
            LoadDetails(treeNode, settings, int.MaxValue);
        }

        private void LoadDetails(TreeNode treeNode, UserSetting[] settings, int lastNodeToLoad)
        {
            treeNode.Nodes.Clear();
            for (int i = 0; (i < settings.Length) && (i <= lastNodeToLoad); i++)
            {
                TreeNode node = treeNode.Nodes.Add(settings[i].Name);
                node.Checked = settings[i].Value;
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
            const int lastTextDetail =    7; // "7" is the "Lyrics" node. Do not load last notes for release versions! They are for real hardcore debugging only!
            //const int lastTextDetail = 11; // 11: Load all nodes: For real hardcore debugging only !!!!!!!
            LoadDetails(musicAsSoundDetails, userSettings.PlayerSettings);
            LoadDetails(musicAsTextDetails, userSettings.ReaderSettings, lastTextDetail);  
            LoadDetails(musicAsBrailleDetails, userSettings.MusicBrailleSettings);
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

        public void ShowFilterItems(bool expandAndSelect)
        {
            treeView.Focus();
            if (expandAndSelect)
            {
                treeView.ExpandAll();
                treeView.SelectedNode = musicAsSound.Nodes[UserSettingsHandler.detailsNodeIndex];
            }
        }




        #endregion

    }


}
