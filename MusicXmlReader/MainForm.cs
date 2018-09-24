using System;
using System.Windows.Forms;
using System.Drawing;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// The 3 interfaces are used for
    /// IBrailleDisplayerClient     Let the Model write MusicBraille patterns to the appropriate Textbox
    /// IObjectCollection   Let the Model access the main Listbox when auto-playing music
    /// IUtilityClient      Let the Model show MessageBoxes
    /// By using these interfaces we avoid that the Model needs to know anything abour Windows Forms!
    /// This makes it much easier to reuse the Model for othea applications and other platforms.
    /// </summary>
    public partial class MainForm : Form, IDebugDisplayerClient, IObjectCollection, IUtilityClient
    {
        string className = "MainForm";
        public static readonly Color FocusedColor = Color.White;         // Mainly for debugging. For released versions use Color.White !
        public static readonly Color NonFocusedColor = Color.WhiteSmoke; // Mainly for debugging. For released versions use Color.White !
        // bool consoleTrace = false;
        enum MusicPlayerStateEnum { unknown, stopped, running };
        string applicationName = "";  // Locakized application name. Will be re-initialized later using localization!
        string executingAssemblyFullPath  = ""; // The (unlocalized) name and location of the program, 
        string executingAssemblyShortName = ""; // The (unlocalized) short name of the program, used by for instance JAWS to name configuration file! 
        string myMusicXmlDirectory; // Default location for MusicXml files belonging to thos user. Will be populated with sample files!

        Model model;        // The Model containing all of the business logic.

        // Most of the UI code in the MainForm class is distributed into the following "Handler" classes, each handling a specific UI control
        UserSettingsHandler     userSettingsHandler; // Contains all settings that can be configured by the user
        ImportHandler           importHandler;
        DetailsHandler          detailsHandler;
        ParameterInputHandler   parameterInputHandler;
        ListBoxTimesHandler     listBoxTimesHandler;
        MessageHandler          messageHandler;


        public MainForm()
        {
            string functionName = "MainForm"; // Only for logging
            try
            {
                UiUtilities.Beep(); // To easily check if the Beep() function works as expected!
                executingAssemblyFullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                executingAssemblyShortName = System.IO.Path.GetFileNameWithoutExtension(executingAssemblyFullPath);
                InitializeComponent();
                Logger.Open(null); // null => Use the default logfile name
                Logger.Log(""); // An empty line to catch the eye
                Logger.Log(string.Format("{0}.{1} Starting: Date={2}", className, functionName, System.DateTime.Now.ToLongDateString()));
                Application.ApplicationExit += Application_ApplicationExit;
                UiUtilities.LogSystemInformation();

                // If the execution directory contains a file named "Language.txt" containing the string "en-US"
                // the application language will be changed to english evin if running on a danish PC!
                UiUtilities.LogGLobalisationInformation();
  
                // Do any UI localization before we create the model. In this way we avoid showing unlocalized texts if an error is reported by a messagebox.
                applicationName = ResourcesForUI.MainForm_ApplicationName;
                messageHandler = MessageHandler.Create(applicationName);
                textBoxScreenReader.Hide(); // This textbox gets Focus used during long-lasting operation and thus draws the Screenreaders attensio to itself, avoiding too much Speech !
                LocalizeMenuStrip(); // Overwrite all items in MenuStrip with localized texts
                textBoxStatusInformation.AccessibleName = ResourcesForUI.StatusLine_Accessible_Name; // Overwrite with localized text
 

                Utilities.UtilityClient = (this as IUtilityClient); //Decide how to show error messages and warnings 
                model = Model.Create((this as IObjectCollection), (this as IDebugDisplayerClient), applicationName);
                importHandler = ImportHandler.Create(model,this,applicationName);
                parameterInputHandler = ParameterInputHandler.Create(model,this);
           
                this.Text = applicationName;
                WriteStatusInformation(model.ScreenReaderName);

                // XCopy MusicXml samples from the "MusicXml samples" directory in the installation files to myMusicXmlDirectory during first activation ! 
                myMusicXmlDirectory = model.InitMusicXmlFiles(applicationName, ResourcesForUI.DirectoryNames_Samples, ResourcesForUI.DirectoryNames_Downloads);

                // Initially none of the 3 main components has focus.
                listBoxTimes.BackColor = NonFocusedColor;
                listBoxDetails.BackColor = NonFocusedColor;
                userSettingsTreeView.BackColor = NonFocusedColor;
                textBoxText.Hide(); // Not for the end user !!Could be reenabled during debugging of Braille Music handling               

                // Create remaining handlers. Some of the need references to others
                detailsHandler = DetailsHandler.Create(listBoxTimes, listBoxDetails, model, this as IDebugDisplayerClient);
                listBoxTimesHandler = ListBoxTimesHandler.Create(model, detailsHandler, listBoxTimes);

                // Create a handler for the user settinge, in this case modelled as a treeview.
                userSettingsHandler = UserSettingsHandler.Create(userSettingsTreeView, model, listBoxTimesHandler);
                userSettingsHandler.Init(); // Builds up the fixed part of the treeview
                userSettingsHandler.Reset();
                // Create a handler for handling all Keyboard shortcuts
                // shortCutHandler = ShortcutHandler.Create(this, model);

                // Experiments.LogRightAlignedMenus(this);

                LoadIcon();

                // throw (new Exception("For test only")); // Insert this line to test the Last Resort handler below
            }
            catch (Exception e)
            {
                // Last resort handler: An unhandled exception occured.
                // The best we can do is to show a warning in a messagebox and log the exception message          
                Logger.Log(String.Format("{0}.{1} Exception occurred: {2}",className,functionName,e.Message));
                ShowWarning((int)ModelMessageEnum.UnspecifiedInitializationError,"","");
            }
        }

  

        private void LoadIcon()
        {
            const string functionName = "LoadIcon";
            string iconFile = "";
            try
            {
                iconFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(executingAssemblyFullPath), "icon.ico");
                this.Icon = new System.Drawing.Icon(iconFile);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} Could not load icon file {2} ->", className, functionName, iconFile));
                Logger.Log(string.Format("{0}", e.Message));
            }

        }

        
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            model.OnApplicationExit(); // Let the Model clean up its resources etc 
        }

#if false
        // For development onlY!
        private List<string> shortcutStrings;
        private List<Keys> shortCutKeys;

        private string Add(string text)
        {
            shortcutStrings.Add(text);
            int index = text.IndexOf("&");
            if (text.Length >= index + 1)
            {
                // The character following the & will be interpreted as a shortcut combined with the ALT key
                char ch = text.ToUpper()[index+1];
                Keys keys = Keys.Alt | (Keys.A + ch - 'A');
                Add(keys);
            }
            return text;
        }

        private Keys Add(Keys keys)
        {
            string functionName = "Add";
            if (shortCutKeys.Contains(keys))
            {
                Logger.Log(String.Format("{0}.{1}: Duplicate key = {2}",className,functionName, keys.ToString()));
            }
            else
            {
                shortCutKeys.Add(keys);
            }
           
            return keys;
        }
#endif






#region IDebugDisplayerClient

        public void WriteNormalTextString(string s)
        {
            textBoxNormalText.Text = s;
        }


        public void WriteBrailleString(string s)
        {
            textBoxBraille.Text = s;
        }

        public void WriteTextString(string s)
        {
            textBoxText.Text = s;
        }

        private long currentSequenceNumber = 0; // For avoiding overwriting relavant status information witn obsolete status information
        public void WriteStatusInformation(string s)
        {
            WriteStatusInformation(s, ++currentSequenceNumber);
        }

        private string latestStatusInformation = "";

        delegate void WriteStatusInformationCallback(string s, long sequenceNumber);
        private void WriteStatusInformation(string s, long sequenceNumber)
        {
            string functionName = "WriteStatusInformation";
            if (textBoxStatusInformation.InvokeRequired)
            {
                Logger.Log(string.Format("{0}.{1}.IR({2})", className, functionName, s));
                WriteStatusInformationCallback d = new WriteStatusInformationCallback(WriteStatusInformation);
                textBoxStatusInformation.Invoke(d, s, sequenceNumber);
            }
            else
            {
                // Logger.Log(string.Format("{0}.{1}.INR({2},{3} Current={4})", className, functionName, s,sequenceNumber,currentSequenceNumber));
                if (sequenceNumber == currentSequenceNumber)
                {
                    // Logger.Log(string.Format("{0}.{1}.INR: Showed Status:{2}", className, functionName, s));
                    textBoxStatusInformation.Text = s;
                    textBoxStatusInformation.Refresh();
                    latestStatusInformation = s;
                }
                else
                {
                    // Explicitly skip obsolete status information
                    Logger.Log(string.Format("{0}.{1}.INR: Ignored Status:{2}", className, functionName, s));
                }
            }
        }

#endregion IDebugDisplayerClient

#region  IObjectCollection

        public int GetNumberOfObjects()
        {
            return listBoxTimesHandler.GetNumberOfObjects();
        }
        
        public object GetObjectAtIndex(int index)
        {
            return listBoxTimesHandler.GetObjectAtIndex(index);
        }

        public void SetSelectedIndex(int index)
        {
            listBoxTimesHandler.SetSelectedIndex(index);
        }
#endregion IObjectCollection

#region IMessageShower 
        // Decide how to show error messages and warnings          
        public void ShowMessage(int messageId,string parameter, string text)
        {
            messageHandler.ShowMessage(messageId, parameter, text);
        }
        

        /// <summary>
        /// Build and show a message consisting of
        /// Line 1: A Message followed by possible parameters. Example: "File not found Stardust.xml"
        /// Line 2: (optional) an extra message. Example: "Plaese see Log File.."
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="text"></param>
        public void ShowWarning(int messageId,string parameter,string text)
        {
            messageHandler.ShowWarning(messageId, parameter, text);
        }
#endregion IMessageShower


        /// <summary>
        ///  Clear the contents of the listbox showing the timed events (important when loading a new file)
        /// </summary>
        private void ClearUI()
        {
            listBoxTimesHandler.Reset(); // No empty line inserted
            listBoxDetails.Items.Clear();
            listBoxDetails.Refresh();
            listBoxDetails.AutoSize = false;
            textBoxBraille.Clear();
            textBoxBraille.Refresh();
            textBoxText.Clear();
            textBoxText.Refresh();
            textBoxStatusInformation.Clear();
            textBoxStatusInformation.Refresh();
            userSettingsHandler.Reset();
            textBoxNormalText.Clear();
            textBoxNormalText.Refresh();
            this.Text = applicationName; // Remove the name of the previously loaded MusicXml file from the title line
        }

        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // The form is closing
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                // The reason is that the user pressed ALT+F4
                if (DialogResult.Yes != MessageBox.Show(ResourcesForUI.Message_DoYouWantToExitTheProgram, applicationName, MessageBoxButtons.YesNo))
                {
                    e.Cancel = true;
                }
            }
            //altF4Pressed = false;
        }
        
        /// <summary>
        /// Seems to be needed to compensate for an error in the communication between WINDOWS and JAWS:
        /// After a form Resize the INSERT+PAGEDOWN JAWS shortcut no longer reads the control positioned at the bottom of the form, in this case the Status Line.
        /// Instead some other undefined information is read, appa rantly depending on the contents of the other controls on the form.
        /// The HACK below seems to compensate for this error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            string functionName = "MainForm_SizeChanged";
            Logger.Log(string.Format("{0}.{1} Form size changed to Width={2} Height={3} Restoring StatusInformation to '{4}'", className, functionName,this.Width,this.Height, latestStatusInformation));
            this.textBoxStatusInformation.Text = latestStatusInformation; // Restore prevopus contents after resize !!
            this.Refresh();
        }


    }
}
