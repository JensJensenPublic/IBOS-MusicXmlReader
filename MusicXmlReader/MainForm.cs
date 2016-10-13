using System;
using System.Windows.Forms;
using System.Globalization;
using MusicXmlReaderUI;

namespace MusicXmlReader
{
        /// <summary>
    /// The 3 interfaces are used for
    /// IWritableString     Let the Model write MusicBraille patterns to the appropriate Textbox
    /// IObjectCollection   Let tne Model access the main Listbox
    /// IMessageShower      Let the Model show MessageBoxes
    /// By using these interfaces we avoid that the Model needs to know anything abour Windows Forms!
    /// This makes it much easier to reuse the Model for othea applications and other platforms.
    /// </summary>
    public partial class MainForm : Form, IWritableString, IObjectCollection, IMessageShower
    {
        string ApplicationName = "IBOS Nodelæser";  // Application name. Fits into a Freedom Scientific Focus 14 Braille dirplay!
        Model model;        // The Model contails all of the business logic.        
        bool autoReload;    // Used to optimize performance when changing large parts of the UI within short time
        UserSettingsHandler userSettingsHandler;

        public MainForm()
        {
            InitializeComponent();
            Logger.Open("MusicXmlReaderUI.log");
            LogSystemInformation();

#if false
            // Used for testing localisation
            System.Threading.Thread thisThread;
            thisThread = System.Threading.Thread.CurrentThread;
            thisThread.CurrentUICulture = new CultureInfo("en-US"); 
#endif

            LogGLobalisationInformation();
            Utilities.MessageShower = (this as IMessageShower); //Decide how to show error messages and warnings 
            model = Model.Create((this as IObjectCollection), (this as IWritableString), ApplicationName);
            this.Text = ApplicationName;

            // Create a handler for the user settinge, in this case modelled as a treeview.
            userSettingsHandler = UserSettingsHandler.Create(this,this.userSettingsTreeView,model);
            userSettingsHandler.Init(); // Buyilds up the fixed part of the treeview

        }



        #region supportcode


        public static void LogSystemInformation()
        {
            Logger.Log(string.Format("ComputerName={0} UserName={1} UserDomainName={2}",
                SystemInformation.ComputerName, SystemInformation.UserName, SystemInformation.UserDomainName));
            Logger.Log(string.Format("OSVersion={0} ProcessorCount={1} Is64BitOperatingSystem={2} Is64BitProcess={3}",
            System.Environment.OSVersion, System.Environment.ProcessorCount, System.Environment.Is64BitOperatingSystem, System.Environment.Is64BitProcess));
        }


        /// <summary>
        /// Log information and implement a temporary mechanism for overwriting the locale on the machine
        /// by placing a simple textfile in the executing directory
        /// </summary>
        public static void LogGLobalisationInformation()
        {
            try
            {
                string currentCultureName = CultureInfo.CurrentUICulture.Name;
                Logger.Log(string.Format("CultureInfo.CurrentUICulture.Name={0}", currentCultureName));
                string LanguageFileName = (System.IO.Path.Combine(System.Environment.CurrentDirectory, "Language.txt"));
                if (System.IO.File.Exists(LanguageFileName))
                {
                    string newCultureName = System.IO.File.ReadAllText(LanguageFileName);   
                    Logger.Log(string.Format("Changing UICulture for UI thread to {0}", newCultureName));
                    System.Threading.Thread thisThread = System.Threading.Thread.CurrentThread;
                    thisThread.CurrentUICulture = new CultureInfo(newCultureName);
                    Logger.Log(string.Format("thisThread.CurrentUICulture={0}", thisThread.CurrentUICulture.Name));
                }

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("LogGLobalisationInformation threw an exception. Message=}0}", e.Message));
            }

        }

        /// <summary>
        /// Assume that a string contains Braille if it is not empty and the first char is a Braille char
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool isBraille(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            char c = s[0];
            return ((0x2800 <= c) && (c <= 0x28ff));
        }

        #region IWritableString
        // Implement IWritableString
        public void SetString(string s)
        {
            if (isBraille(s))
            {
                textBoxBraille.Text = s;
            }
            else
            {
                textBoxText.Text = s;
            }
        }
        
        #endregion

        #region  IObjectCollection

        public int GetNumberOfObjects()
        {
            return listBoxTimes.Items.Count;
        }

        public object GetObjectAtIndex(int index)
        {
            return listBoxTimes.Items[index];
        }

        delegate void SetSelectedIndexCallback(int index);
        public void SetSelectedIndex(int index)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBoxTimes.InvokeRequired)
            {
                SetSelectedIndexCallback d = new SetSelectedIndexCallback(SetSelectedIndex);
                listBoxTimes.Invoke(d, new object[] { index });
            }
            else
            {
                listBoxTimes.Focus(); // Maybe not needed. How can we force the Screeen-reader to read the selected line? 
                listBoxTimes.SelectedIndex = index;
                // System.Threading.Thread.Sleep(100); // HACK Pause the UI thread and let the Screenreader get a chance
            }
        }
        #endregion

        #region IMessageShower 
        // Decide how to show error messages and warnings          
        public void ShowMessage(string text)
        {
            MessageBox.Show(text);
        }

        public void ShowWarning(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion






        /// <summary>
        /// Open a standard File Dialog allowing the user select a MusicXml file.
        /// Clear statistic counters describing the operations on the file selected.
        /// Let the Model load the file selected and report any errors detected to the user
        /// Update the UserSettingsTreeview used for filtering depending on the contents of the MusicXml file loaded
        /// Load the contents of the MusicXml file into the main ListBox.
        /// Dump statistic counters describing the operations on the file selected.
        /// </summary>
        /// <param name="sender"> Not used</param>
        /// <param name="e">Not used</param>
        private void SelectAndOpenMusicXmlFile(object sender, EventArgs e)
        {
            openFileDialog.FileName = "Node.xml"; // Use this sample file as a default
            openFileDialog.Filter = "MusicXml filer|*.xml"; // Only present .xml files
            openFileDialog.InitialDirectory = model.InitialDirectory;
            openFileDialog.ShowDialog();

            textBoxMessage.Focus();
            textBoxMessage.Text = string.Format("Indlæser {0}", openFileDialog.FileName);

            Logger.ClearStatistics();  // Clear statistics to be collected while loading, parsing and rendering the MusicXml file:

            if (!model.LoadMusicXmlFile(openFileDialog.FileName)) // Load the selected .xml file into the Model and build all internal data structures.
            {
                // Simple error handling
                textBoxMessage.Text = string.Format("Kunne ikke indlæse {0}", openFileDialog.FileName);
                Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() until now
                return;
            }

            // Clear the contents of the listbox showing the timed events (important when loading a new file)
            listBoxTimes.Items.Clear();
            listBoxTimes.Refresh();


            autoReload = false; // While loading the listbox all changes are  made by user and must be ignored

            // The initial values of the user settings are determined by the model.
            // These settings must be reflected in the UI:
            // Load the Checkboxes controlling the user settings per part
            userSettingsHandler.LoadParts(model.partList);
            // Load the Checkboxes controlled by a fixed number of settings statically defined in the Model.
            userSettingsHandler.LoadDetails(model.UserSettings);
            // Finally expand the tree
            userSettingsHandler.ExpandSelectedNodes();
            //this.userSettingsTreeView.ExpandAll();
            userSettingsHandler.CheckSelectedNotes();

            // Let the Model do the hard work of transforming to e timed representation.
            LoadListBoxTimes();

            Logger.DumpStatistics(); // Dump all statistics collected by LogOnce() during parsing, interpreting and rendering the file

            autoReload = true; // From now on all changes are  made by user and must be handled

            // Focus on the listbox representing the time representation
            listBoxTimes.Focus();
            //listBoxTimes.SelectedIndex = 0;
        }


        public void ConditionalLoadListBoxTimes()
        {
            if (autoReload)
            {
                LoadListBoxTimes();
            }
        }
        

        /// <summary>
        /// Load the main listbox with information fetched from the Model
        /// First all metainformation (Composer, Author, etc)
        /// Then all the events describing the music sheet itself
        /// </summary>
        private void LoadListBoxTimes()
        {
            listBoxTimes.Items.Clear();
            foreach (string s in model.MetaInfoStrings)
            {
                listBoxTimes.Items.Add(s);
            }
            foreach (EventDescription eventDescription in model.EventDescriptionList.Events)
            {
                listBoxTimes.Items.Add(eventDescription);
            }
        }

        #endregion


        private void openMusicXmlFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show a standard Select File dialog to allow the user to select and open a MusicXml file
            SelectAndOpenMusicXmlFile(sender, e);
        }



        #region ListBoxTimes

        //private void listBoxTimes_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int index = listBoxTimes.SelectedIndex;
        //    // Model.Trace(string.Format("ListBoxTimes_SelectedIndexChanged(i={0})", index));
        //    object o = listBoxTimes.Items[index];
        //    model.musicPlayer.SelectedIndexChanged(index, o);
        //    model.brailleDisplayer.SelectedIndexChanged(index, o);
        //}

        private void ListBoxTimes_GotFocus(object sender, EventArgs e)
        {
            int index = listBoxTimes.SelectedIndex;
            Logger.Trace(string.Format("ListBoxTimes_GotFocus(i={0})", index));
            // Even if we got focus we can not be sure that an item is selected!
            if (-1 != index)
            {
                // If an index is selected do as if Selected Index changed
                object o = listBoxTimes.Items[index];
                model.musicPlayer.SelectedIndexChanged(index, o);
                model.brailleDisplayer.SelectedIndexChanged(index, o);
            }
        }

        private void ListBoxTimes_LostFocus(object sender, EventArgs e)
        {
            Logger.Trace("ListBoxTimes_LostFocus");
            model.StopRefreshingBrailleDevice();
        }
        
        private void listBoxTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxTimes.SelectedIndex;
            // Model.Trace(string.Format("ListBoxTimes_SelectedIndexChanged(i={0})", index));
            object o = listBoxTimes.Items[index];
            model.musicPlayer.SelectedIndexChanged(index, o);
            model.brailleDisplayer.SelectedIndexChanged(index, o);
        }

        #endregion
    }
}
