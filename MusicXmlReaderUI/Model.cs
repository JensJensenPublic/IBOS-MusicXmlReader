using System;
using System.IO;
using System.Collections.Generic;
using NAudio.Midi;
using System.Windows.Forms;
using System.Xml;
using DavyKager; // Tolk

namespace MusicXmlReaderUI
{
    class Model
    {
        public readonly string ApplicationName = "IBOS Musiklæser";
        string theMusicXmlFileName = "";
        List<MusicXmlObject> allMusicXmlObjecsts; // Holds all information from the .xml file
        MidiOut midiOut;
        ListBox listBoxParts; // Lists elements grouped per part
        ListBox listBoxPoly; // Lists elements grouped per time
        public MusicPlayer musicPlayer;
        public BrailleDisplayer brailleDisplayer;
        public PartlistElement partList; // Contains the list of parts, describing all instruments used including their midi parameters
        int divisions; // Current number of divisions of a quarternode
        int tempo;     // Current tempo in beats pr minute
        int currentMeasureNumber = 0 ; // Current measure number
        int latestMeasureNumber = 0;
        int numberOfParts; // Number of parts
        //int currentPartitionNumber = -1;
        string currentPartId = "";
        //int currentPartNumber = 0;  // Will be saved with each note
        //int currentMidiChannel = 1; // Will be saved with each note
        ScorePartElement currentScorePartElement = null;
        UserSettings userSettings;
 
        string executingAssembly;
        string executingDirectory;
        
        List<string> metaInfoStrings = new List<string>(); // Selected meta info from the current file, such as Title and Composer


        private bool CheckFileExistance(string fileName, string methodName, bool dir)
        {
            if (dir)
            {
                if (!System.IO.Directory.Exists(fileName))
                {
                    Model.Log(string.Format("{0} Directory {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", fileName));
                    MessageBox.Show(string.Format("Mappen {0} findes ikke", fileName));
                    return false;
                }
            }
            else
            {
                if (!System.IO.File.Exists(fileName))
                {
                    Model.Log(string.Format("{0} File {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", fileName));
                    MessageBox.Show(string.Format("Filen {0} findes ikke", fileName));
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Attempts to start an external program using a single filename as argument
        /// Errors are reportes through messageboxes and Model.Log()
        /// </summary>
        /// <param name="exeFileName">Name of program to start, with or without full path</param>
        /// <param name="argFileName">Name of file to use as argument when starting the program</param>
        /// <returns>true <==> succaee</returns>
        private bool RunExeWithFileArgument(string exeFileName, string argFileName)
        {
            return RunExeWithFileArgument(exeFileName, argFileName, false);
        }

        private bool RunExeWithDirArgument(string exeFileName, string argFileName)
        {
            return RunExeWithFileArgument(exeFileName, argFileName, true);
        }


        private bool RunExeWithFileArgument(string exeFileName, string argFileName,bool dir)
        {
            string methodName = "RunExeWithFileArgument";
            // Check arguments
            string exePathName = Path.GetDirectoryName(exeFileName);
            if ((!string.IsNullOrEmpty(exePathName)) && (!CheckFileExistance(exeFileName, methodName,false))) return false;
            if ((!string.IsNullOrEmpty(argFileName)) && (!CheckFileExistance(argFileName, methodName,dir))) return false;
            // Create process startinfo. Enclose all filenames and pathnames in "" in order to handle possible space characters!
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();     
            pProcess.StartInfo.FileName = string.Format("\"{0}\"", exeFileName);
            pProcess.StartInfo.WorkingDirectory = string.IsNullOrEmpty(exePathName) ? null : string.Format("\"{0}\"", exePathName);
            pProcess.StartInfo.Arguments = string.Format("\"{0}\"", argFileName); 
            pProcess.StartInfo.UseShellExecute = true; // Allows the system to search for the executable using PATH
            pProcess.StartInfo.RedirectStandardOutput = false;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            try
            {
                pProcess.Start();
            }
            catch (Exception e)
            {
                Model.Log(string.Format("ReadFileByExecutable: Exception thrown while starting {0}: {1}", pProcess.StartInfo.FileName, e.Message));
                MessageBox.Show(string.Format("Kunne ikke starte programmet \r\n'{0}'\r\nmed filen\r\n'{1}'",exeFileName,argFileName));
                return false;
            }
            return true;
        }

        #region LogFile
        public static string LogFileName = "MusicXmlReader.Log";
        public static void Log(string s)
        {
            try
            {
                System.DateTime now = System.DateTime.Now;
                string time = string.Format("{0}.{1,03}", now.ToLongTimeString(), now.Millisecond.ToString()); // Always use 3 digits for milliseconds
                System.IO.File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), LogFileName), time + " " + s + "\r\n");
            }
            catch (Exception e)
            {
                // What to do here ??
            }
        }


        public void ReadLogFile()
        {
            RunExeWithFileArgument("notepad.exe", System.IO.Path.Combine(System.IO.Path.GetTempPath(), LogFileName));
        }

        public void OpenLogFileLocation()
        {
            string tempPath = System.IO.Path.GetTempPath();
            RunExeWithDirArgument("explorer.exe",tempPath);
        }

        public void OpenMusicXmlFileLocation()
        {
            string dir = Path.GetDirectoryName(theMusicXmlFileName);
            if (System.IO.Directory.Exists(dir))
            {
                RunExeWithDirArgument("explorer.exe", dir); 
            }
        }
        
        #endregion


        #region InterpretationFile
        public static string InterpretationFileName = "MusicReader.txt";
        public void ReadInterpretation()
        {
            string fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), InterpretationFileName);
            //System.IO.FileStream  fileStream = System.IO.File.OpenWrite(InterpretationFileName);

            // Create contents
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fileName);
            if (allMusicXmlObjecsts.Count > 0)
            {
                foreach (object o in allMusicXmlObjecsts)
                {
                    streamWriter.WriteLine(o.ToString());
                }

            }
            else
            {
                streamWriter.WriteLine("No MusicXml objects found");
            }

            streamWriter.Close();

            RunExeWithFileArgument("notepad.exe", System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName));

            //ReadTempFileByNotepad(fileName);
        }
        #endregion

        #region MusicXmlFile
        public void ReadMusicXmlFile()
        {
            if (System.IO.File.Exists(theMusicXmlFileName))
            {
                //ReadFileByNotepad(theMusicXmlFileName);
                //ReadFileByExecutable("iexplore.exe",theMusicXmlFileName);
                RunExeWithFileArgument("iexplore.exe", theMusicXmlFileName);

            }
        }
        #endregion


        #region MuseScore
        public void StartMuseScore()
        {
            if (System.IO.File.Exists(theMusicXmlFileName))
            {
                //string exeFileName = @"C:\Program Files(x86)\MuseScore 2\bin\MuseScore.exe";
                string exeFileName =  @"C:\Program Files (x86)\MuseScore 2\bin\MuseScore.exe";
                //ReadFileByMuseScore(exeFileName, theMusicXmlFileName);
                RunExeWithFileArgument(exeFileName, theMusicXmlFileName);

            }
        }
        #endregion

 

        /// <summary>
        /// Quick and dirty check to reject obvious unusable Xml files
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool CheckMusicXmlSyntax(XmlDocument doc)
        {
            bool ok = true;
            ok = ok && doc.HasChildNodes;
            ok = ok && (doc.ChildNodes.Count >= 3);
            //ok = ok && (doc.ChildNodes[0].InnerXml.Contains(""));
            ok = ok && (doc.ChildNodes[1].Name.Contains("score-partwise"));
            //ok = ok && (doc.ChildNodes[2].InnerXml.Contains("score-partwise"));
            return ok;
        }

        /// <summary>
        /// Log som interesting system parameters
        /// </summary>
        private void LogSystemParameters()
        {
            bool screenReaderRunning;
            int lastWin32Error;
            string name = "SystemParametersiInfo.GetScreenReader";
            bool ok = SystemParametersiInfo.GetScreenReader(out screenReaderRunning, out lastWin32Error);
            if (ok)
            {
                Log(string.Format("{0} reported {1}",name, screenReaderRunning));
            }
            else
            {
                Log(string.Format("{0} failed. LastWin32Error = {1}", name, lastWin32Error));
            }
        }

        /// <summary>
        /// Checks the functionality of NVDA nvdaControllerClient
        /// All entries in the logfile are in English
        /// All speach must be localized! TODO
        /// </summary>
        /// <returns>false on first failure, true if everything succeds</returns>
        private bool LogNvdaInterface()
        {
            try
            {
                // First check if the nvdaControllerClient32.dll is found in the execution directory. TODO
                string fileName = "nvdaControllerClient32.dll";
                string fullFileName = Path.Combine(Environment.CurrentDirectory, fileName);
                if (!File.Exists(fullFileName))
                {
                    Log(string.Format("{0} is not found. NVDA ScreenReader can not be controlled through NVDA ControllerClient", fullFileName));
                    return false;
                }
                NvdaControllerClientWrapper nvda = NvdaControllerClientWrapper.Create();

                uint errorCode;
                bool resRunning = nvda.TestIfRunning(out errorCode);
                Log(string.Format("NVDA ControllerServer for NVDA ControllerClient is{0}running.", (!resRunning) ? " NOT " : " "));
                if (!resRunning)
                {
                    if (errorCode != 1722) // 1722 is the expected error in this case : "RPC server is not available. 
                    {
                        Log(string.Format("NvdaControllerClientWrapper.nvdaController_testIfRunning() failed WINERROR={0}", resRunning));
                    }        
                    return false;
                }

                System.Threading.Thread.Sleep(2000); // Allow the previous speach to propagate through the system
                nvda.SpeakText("N V D A ControllerClient");
                System.Threading.Thread.Sleep(2000); // Allow the speach to propagate through the system
                nvda.TempBrailleMessage("Braille"); //  Do notstart frefreshing the display
                nvda.CancelSpeech();                
            }
            catch (Exception e)
            {
                Log(string.Format("LogNvdaInterface() threw an exception: {0}",e.Message));
                return false;
            }
            return true;

        }         


        /// <summary>
        /// Stops on any error and returns false
        /// </summary>
        /// <param name="fullXmlFileName"></param>
        /// <returns></returns>
        public bool LoadMusicXmlFile(string fullXmlFileName)
        {
            bool ok = true;
            string xmlFileName = ""; // The MusicXml file currently handled       
//            try
            {
                string defaultFileName = "Node.xml";
                if (string.IsNullOrEmpty(fullXmlFileName))
                {     
                    // Use a default value             
                    fullXmlFileName = System.IO.Path.Combine(executingDirectory, defaultFileName);
                }
                xmlFileName = System.IO.Path.GetFileName(fullXmlFileName);
                XmlDocument doc = new XmlDocument();
                XmlTextReader reader = new XmlTextReader(fullXmlFileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                doc.Load(reader);
                ok = ok && CheckMusicXmlSyntax(doc);
                if (ok)
                {
                    metaInfoStrings = new List<string>(); // Reset Meta Information
                    metaInfoStrings.Add(Path.GetFileName(fullXmlFileName)); // Guarentees that some meta information exists
                    Log(string.Format("Loaded {0}", fullXmlFileName));
                    allMusicXmlObjecsts = new List<MusicXmlObject>(); // Create the list holding all MusicXml elements read from file
                    Recurse(doc.ChildNodes);                          // Build  the list holding all MusicXml elements read from file
                    Log(string.Format("Parsed {0}", xmlFileName));
                    Init(); // Experimental code !!                   // TO DO move rest of this {} into Init !
                    Log(string.Format("Initialized all components"));
                    LoadListBox();
                    theMusicXmlFileName = fullXmlFileName;
                }
                else
                {
                    Log(string.Format("Failed to load {0} because it not a valid MusicXml file",xmlFileName));
                    theMusicXmlFileName = "";
                }
            }
            //catch (System.Exception e)
            //{
            //    Log(string.Format("Failed to load {0} ({1})",xmlFileName,e.Message));
            //    ok = false;
            //}               
                        return ok;
        }

        private bool CheckDll(string dllName, string directory)
        {
            if (!File.Exists(Path.Combine(directory, dllName)))
            {
                Log(string.Format("Missing support-dll: {0}", dllName));
                return false;
            }
            return true;
        }
        
        private bool CheckDlls()
        {
            // Report if any file is missing
            bool result = true;
            bool is64Bit = IntPtr.Size == 8;
            Log(string.Format("This program is compiled for is a {0} bit ", is64Bit ? "64" : "32"));
            if (is64Bit)
            {
                result &= CheckDll("tolk.dll", executingDirectory);
                result &= CheckDll("jfwapi.dll", executingDirectory);
                result &= CheckDll("nvdaControllerClient64.dll", executingDirectory);
            }
            else
            {
                result &= CheckDll("tolk.dll", executingDirectory);
                result &= CheckDll("jfwapi.dll", executingDirectory);
                result &= CheckDll("nvdaControllerClient32.dll", executingDirectory);
            }
            return result;
        }

        private bool LoadTolk()
        {
            bool result = false;
            try
            {
                Tolk.Load();
                bool isLoaded = Tolk.IsLoaded();
                Log(string.Format("Tolk.IsLoaded() returned {0}", isLoaded));
                string screenReader = Tolk.DetectScreenReader();
                if (null == screenReader)
                {
                    return false;
                }
                Log(string.Format("Tolk.DetectScreenReader found {0}", (screenReader == null) ? "No screenreader" : screenReader));
                if (Tolk.HasSpeech())
                {
                    Console.WriteLine("This screen reader driver supports speech");
                }
                if (Tolk.HasBraille())
                {
                    Console.WriteLine("This screen reader driver supports braille");
                }
                result = true;
            }
            catch (Exception e)
            {
                Log(string.Format("Tolk.Load threw and exception: {0}", e.Message));
            }
            return result;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public Model(ListBox listBox, ListBox listBoxPoly, TextBox textBoxMusicBraille)
        {
            executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);
            Log(""); // An empty line
            Log(string.Format("Date={0}:", System.DateTime.Now.ToLongDateString()));
            Log(string.Format("{0} started in '{1}'", System.IO.Path.GetFileName(executingAssembly), executingDirectory));

            if (!CheckDlls())
            {
                MessageBox.Show("Manglende programfil ! Se venligst Logfilen!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!LoadTolk())
            {
                string caption = "Kunne ikke forbinde til skærmlæser!";
                MessageBox.Show(  "Kunne ikke forbinde til skærmlæser!\r\n"
                                + "Understøttede skærmlæsere er 'JAWS' og 'NVDA'\r\n"
                                + "Se venligst logfilen (Værktøjer->Log fil)",
                                caption,MessageBoxButtons.OK,MessageBoxIcon.Error);
            }    
                        
            midiOut = new MidiOut(0);
            musicPlayer = new MusicPlayer(listBox, listBoxPoly,midiOut);
            int displaySize = 14;
            brailleDisplayer = BrailleDisplayer.Create(textBoxMusicBraille, displaySize); // TODO Get the real displaysize from somewhere
            Model.Log(string.Format("Model: Assuming size of physical Braille display = {0}", displaySize));

            //musicPlayer.ChangeInstrument(20); // Church Organ

            this.listBoxParts = listBox;
            this.listBoxPoly = listBoxPoly;

            //Model.Log(string.Format("listBoxPoly.AccessibleDefaultActionDescription={0}", listBoxPoly.AccessibleDefaultActionDescription));
            //Model.Log(string.Format("listBoxPoly.AccessibilityObject.ToString()={0}", listBoxPoly.AccessibilityObject.ToString())); 
            

            // Log some global system parameters.
            LogSystemParameters();
            // Log availability of NVDA interface.
            LogNvdaInterface();

            //DeviceInfo.LogDeviceInfo();
        }
 
        /// <summary>
        /// Handels the syntax analysis of an XML node representing a MusicXML element while reading the MusicXML file.
        /// </summary>
        /// <param name="node">An XML node representing a MusicXML element.</param>
        /// <returns>True <==> Further recursion is required.</returns>
        private bool WriteElement(XmlNode node)
        {
            bool continueRecursion = true;
            switch (node.Name)
            {
                case "note":
                    if (this.currentMeasureNumber != this.latestMeasureNumber)
                    {
                        this.latestMeasureNumber = this.currentMeasureNumber;
                    }
                    NoteElement note = NoteElement.Create(node, this.divisions, this.currentMeasureNumber, this.currentScorePartElement); // New version
                    allMusicXmlObjecsts.Add(note);
                    continueRecursion = false;
                    break;
                case "part-list":
                    // We also save the part-list in the model for later reference.
                    partList = PartlistElement.Create(node);
                    allMusicXmlObjecsts.Add(partList);
                    this.numberOfParts = partList.NumberOfParts();
                    // Now we know the number of parts.
                    userSettings = UserSettings.Create(this.numberOfParts);
                    // allMusicXmlObjecsts.AddRange(   (partitionList.ToStrings());
                    continueRecursion = false;
                    break;
                case "measure":
                    MeasureElement measureElement = MeasureElement.Create(node);
                    allMusicXmlObjecsts.Add(measureElement); // Avoid the "Ikke VAlgt" error message from screenreader
                    this.currentMeasureNumber = int.Parse(measureElement.Number);
                    break;
                case "score-part":
                    // Describes the meta-data related to a part.
                    // This includes "part-name", "score-instrument" and "midi-instrument".
                    // (The notes and pauses are described in "part")
                    ScorePartElement scorePartElement = ScorePartElement.Create(node);
                    allMusicXmlObjecsts.Add(scorePartElement);
                    continueRecursion = false;
                    break;
                case "part":
                    // Describes the notes (and pauses) of a part.
                    // (The mata-data is described in "score-part")               
                    PartElement partElement = PartElement.Create(node);
                    allMusicXmlObjecsts.Add(partElement);
                    // Save the current part Id
                    this.currentPartId = partElement.PartId;
                    // Look up the partition in the partList
                    this.currentScorePartElement = partList.GetPartFromId(partElement.PartId);
                    // Save the part number as the current part number. This will be saved with each note!  
                    //this.currentPartNumber = currentScorePartElement.partNumber;
                    // Save the midiChannel of the part. This will be saved with each note!
                    //this.currentMidiChannel = currentScorePartElement.midiInstrumentElement.MidiChannel; 

                    // Set up the the MusicPlayer to use the specified midi program for the specified midiChannel
                    if ((0 != this.currentScorePartElement.MidiChannel) && (0 != this.currentScorePartElement.MidiProgram))
                    {
                        musicPlayer.ChangeInstrument(this.currentScorePartElement.MidiChannel, this.currentScorePartElement.MidiProgram);
                    }

                    break;
                // We know the existance of the following elements, but for the time being we ignore them.
                case "work":
                    SimpleTextElement workElement = SimpleTextElement.Create(node, "Titel");
                    allMusicXmlObjecsts.Add(workElement);
                    metaInfoStrings.Add(workElement.ToString()); 
                    continueRecursion = false;
                    break;
                case "movement-title":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Opus"));
                    continueRecursion = false;
                    break;
                case "movement-number":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Nummer"));
                    continueRecursion = false;
                    break;
                case "identification":
                    // allMusicXmlObjecsts.Add(SimpleTextElement.Create(node,"Identifikation"));          
                    break;
                case "creator":
                    CreatorElement creatorElement = CreatorElement.Create(node);
                    allMusicXmlObjecsts.Add(creatorElement);
                    metaInfoStrings.Add(creatorElement.ToString());
                    continueRecursion = false;
                    break;
                case "rights":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node,"Rettigheder"));
                    continueRecursion = false;
                    break;
                case "encoding":
                    // Is described in "software", "encoding-date", "encoder", "encoding-description"
                    //allMusicXmlObjecsts.Add(SimpleTextElement.Create(node));         
                    break;
                case "software":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node,"Software"));
                    continueRecursion = false;
                    break;
                case "encoding-date":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node,"Arrangements dato"));
                    continueRecursion = false;
                    break;
                case "encoder":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node,"Arrangement"));
                    continueRecursion = false;
                    break;
                case "encoding-description":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node,"Kodnings-beskrivelse"));
                    continueRecursion = false;
                    break;
                case "direction":
                    continueRecursion = false;
                    break;
                case "transpose":
                    continueRecursion = false;
                    break;
                case "divisions":
                    DivisionsElement divisionsElement = DivisionsElement.Create(node);
                    allMusicXmlObjecsts.Add(divisionsElement);
                    this.divisions = divisionsElement.GetDivisions();
                    break;
                case "key":
                    allMusicXmlObjecsts.Add(KeyElement.Create(node));
                    continueRecursion = false;
                    break;
                case "harmony":
                    allMusicXmlObjecsts.Add(HarmonyElement.Create(node));
                    continueRecursion = false;
                    break;
                case "sound":
                    SoundElement soundElement = SoundElement.Create(node);
                    allMusicXmlObjecsts.Add(soundElement);
                    this.tempo = soundElement.GetTempo();
                    break;
                case "time":
                    allMusicXmlObjecsts.Add(TimeElement.Create(node));
                    continueRecursion = false;
                    break;
                case "clef":
                    allMusicXmlObjecsts.Add(ClefElement.Create(node));
                    continueRecursion = false;
                    break;
                case "print":
                    continueRecursion = false; // This is all graphics stuff!
                    break;
                case "defaults":
                    continueRecursion = false; // This is all graphics stuff!
                    break;
                case "score-partwise":
                    allMusicXmlObjecsts.Add(ScorePartwiseElement.Create(node));                 
                    break;
                case "attributes":
                     // Ignore until we need them
                     break;
                case "backup":
                    // Needed soon!
                    allMusicXmlObjecsts.Add(BackupElement.Create(node, divisions));
                    continueRecursion = false;
                    break;
                case "forward":
                    // Needed soon!
                    allMusicXmlObjecsts.Add(ForwardElement.Create(node, divisions));
                    continueRecursion = false;
                    break;

                case "repeat":
                    // TODO Implement !!
                    allMusicXmlObjecsts.Add(RepeatElement.Create(node));
                    //Model.Log(string.Format("Model.WriteElement: Unimplemented element 'repeat' Part={0} Measure={1}", currentPartId, currentMeasureNumber));
                     break;

                // The following elements are ignored for the time being, as they describe graphical properties only!
                case "offset":
                case "supports":
                case "staves":
                case "staff-details":
                case "scaling":
                case "millimeters":
                case "tenths":
                case "page-layout":
                case "page-height":
                case "page-width":
                case "page-margins":
                case "left-margin":
                case "top-margin":
                case "bottom-margin":
                case "system-layout":
                case "system-margins":
                case "top-system-distance":
                case "staff-layout":
                case "staff-distance":
                case "appearance":
                case "line-width":
                case "right-margin":
                case "system-distance":
                case "note-size":
                case "distance":
                case "music-font":
                case "word-font":
                case "credit":
                case "credit-type":
                case "credit-words":
                case "barline":
                case "bar-style":
                    break; // Explicitly ignoring graphic information!
                default:
                    allMusicXmlObjecsts.Add(UnimplementedElement.Create(node));
                    break;
            }
            return continueRecursion;
        }

        public void Recurse(XmlNodeList childrenNodes)
        {
            foreach (XmlNode childNode in childrenNodes)
            {

                //if (showRaw)
                //{
                //    // The Unfiltered data
                //    if (listBoxRaw.Items.Count >= 1000) return;
                //    switch (childNode.NodeType)
                //    {
                //        case XmlNodeType.Element:
                //            listBoxRaw.Items.Add(childNode.NodeType + " " + childNode.Name);
                //            break;
                //    }
                //}

                // The filtered data

                bool doRecursion = true;
                switch (childNode.NodeType)
                {
                    case XmlNodeType.Element:
                        doRecursion = WriteElement(childNode);
                        break;
                    case XmlNodeType.Comment:
                        // allMusicXmlObjecsts.Add(childNode.InnerText);
                        break;
                }
                if (doRecursion)
                {
                    Recurse(childNode.ChildNodes);
                }                          
                                    
            }
        }

        // The following 2 lists contain references into allMusicXmlObjecsts where the decoded information is kept! 
        private PartDescriptionList  partDescriptionList;
        private TimeDescriptionList  timeDescriptionList;
        private EventDescriptionList eventDescriptionList;


        public string InitialDirectory
        {
            get
            {
                // For now we expect to find the musicxml files here.
                return Path.Combine(executingDirectory, "MusicXml samples");
            }
        }

        public UserSettings UserSettings
        {
            get
            {
                return userSettings;
            }
        }

        //private MeasureDescriptionList measureDescriptionList;

        public void Init()
        {
            partDescriptionList = PartDescriptionList.Create(allMusicXmlObjecsts,numberOfParts);
            divisions = 24; // TODO compute!
            timeDescriptionList = TimeDescriptionList.Create(partDescriptionList, divisions);
            eventDescriptionList = EventDescriptionList.Create(timeDescriptionList, numberOfParts,userSettings);
            //measureDescriptionList = MeasureDescriptionList.Create(allMusicXmlObjecsts);
        }

        //*****************************************************************************************
        // Event handlers called directly from the GUI and distributing control to other objects.
        //*****************************************************************************************

        public void LoadListBox()
        {
            listBoxParts.Items.Clear();
            foreach (MusicXmlObject musicXmlObject in allMusicXmlObjecsts)
            {
                {
                    // Add ALL objects to make it possible to browse manually through them
                    // During auto-play only node items (including pauses) will be selected to allow for correct timing!
                    if (!(musicXmlObject is MeasureElement))
                    {
                        listBoxParts.Items.Add(musicXmlObject);
                    }
                }
            }
        }


        public void StartPlayingMono()
        {
            musicPlayer.Reset(numberOfParts);
            musicPlayer.StartPlayingMono();
        }

        public void StartPlayingPoly()
        {
            musicPlayer.Reset(numberOfParts);
            musicPlayer.StartPlayingPoly();
        }

        public void LoadListBoxTimes()
        {
            listBoxPoly.Items.Clear();
            eventDescriptionList.LoadListBox(listBoxPoly, this.metaInfoStrings);
            //timeDescriptionList.LoadListBox(listBoxTimes); 
        }

        public void LoadListBoxOfParts(CheckedListBox checkedListBox)
        {
            checkedListBox.Items.Clear();
            for (int i = 0; (i < numberOfParts); i++)
            {
                ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                checkedListBox.Items.Add(string.Format("{0} {1}", scorePartElement.partId, scorePartElement.partName));
                checkedListBox.SetItemChecked(i, true);
            }
            checkedListBox.CheckOnClick = true;
        }

        //public void LoadListBoxPartsToPlay(CheckedListBox checkedListBoxPartsToPlay)
        //{
        //    for (int i = 0; (i < numberOfParts); i++)
        //    {
        //        ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
        //        checkedListBoxPartsToPlay.Items.Add(string.Format("{0} {1}", scorePartElement.partId, scorePartElement.partName));
        //        checkedListBoxPartsToPlay.SetItemChecked(i, true);
        //    }
        //    checkedListBoxPartsToPlay.CheckOnClick = true;
        //}

        public void LoadListBoxPartsToRead(CheckedListBox checkedListBoxPartsToPlay)
        {
        }

        public void SetParts(int partNumber, bool value)
        {
            // For the time being handled in the UI
        }



        public void SetPartsToPlay(int partNumber, bool value)
        {
            this.userSettings.partsToPlay[partNumber] = value;
            //musicPlayer.PartsToPlay = this.userSettings.partsToPlay;
            musicPlayer.UserSettings = this.userSettings;
        }

        public void SetPartsToRead(int partNumber, bool value)
        {
            this.userSettings.partsToRead[partNumber] = value;
        }

        public void SetPartsToReadLyrics(int partNumber, bool value)
        {
            this.userSettings.partsToReadLyrics[partNumber] = value;
        }
        
        public void PlaySpeedChanged(float newValue)
        {
            userSettings.userSlowDown = newValue;
        }


        public void StopRefreshingBrailleDevice()
        {
            brailleDisplayer.StopRefreshing();
        }
    }
}
