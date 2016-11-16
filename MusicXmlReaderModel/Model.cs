 using System;
using System.IO;
using System.Collections.Generic;
using NAudio.Midi;
using System.Xml;
using JSJ.ScreenReaderAPI;
using MusicXmlReaderUI; // Interfaces

namespace MusicXmlReaderModel
{
    public class Model
    {
        string className = "Model";
        string theMusicXmlFileName = "";
        bool is64Bit; // This program is compiled and for the following architechture: false:x86 true:x64 
        List<MusicXmlObject> allMusicXmlObjecsts; // Holds all information from the .xml file
        MidiOut midiOut;
        //ListBox listBoxPoly; // Lists elements grouped per time
        IObjectCollection objects; 
        public MusicPlayer musicPlayer;
        public BrailleDisplayer brailleDisplayer;
        public TextDisplayer textDisplayer;
        public PartlistElement partList; // Contains the list of parts, describing all instruments used including their midi parameters
        int divisions; // Current number of divisions of a quarternode
        int tempo;     // Current tempo in beats pr minute
        int currentMeasureNumber = 0; // Current measure number
        int latestMeasureNumber = 0;
        int numberOfParts; // Number of parts
        //int currentPartitionNumber = -1;
        string currentPartId = "";
        //int currentPartNumber = 0;  // Will be saved with each note
        //int currentMidiChannel = 1; // Will be saved with each note
        ScorePartElement currentScorePartElement = null;
        TimeElement currentTimeElement; // Contains the current TimeElement
        UserSettings userSettings;
        ScreenReaderAPI screenReaderAPI;
        DebugTools debugTools;
        ExterrnalToolsHandler externalToolsHandler;


        string executingAssembly;
        string executingDirectory;

        List<string> metaInfoStrings = new List<string>(); // Selected meta info from the current file, such as Title and Composer


        /// <summary>
        /// Create to be used by UI-less applications
        /// </summary>
        /// <returns></returns>
        static public Model Create()
        {
            return new Model(null,null,null);
        }

        static public Model Create(IObjectCollection objects, IDebugDisplayerClient ws, string menuCaption)
        {
            return new Model(objects, ws, menuCaption);
        }
        

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
        /// Stops on any error and returns false
        /// </summary>
        /// <param name="fullXmlFileName"></param>
        /// <returns></returns>
        public bool LoadMusicXmlFile(string fullXmlFileName)
        {
            bool ok = true;
            string xmlFileName = ""; // The MusicXml file currently handled       
            try
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
                    metaInfoStrings.Add(string.Format("{0}: {1}", ResourcesForModel.MetaInfoText_FileName, Path.GetFileName(fullXmlFileName))); // Guarentees that some meta information exists
                    Logger.Log(string.Format("Loaded '{0}'", Path.GetFileName(fullXmlFileName)));
                    Logger.Log(string.Format("From   '{0}'", Path.GetDirectoryName(fullXmlFileName)));
                    allMusicXmlObjecsts = new List<MusicXmlObject>(); // Create the list holding all MusicXml elements read from file
                    Recurse(doc.ChildNodes);                          // Build  the list holding all MusicXml elements read from file
                    Logger.Log(string.Format("Parsed {0}", xmlFileName));
                    Init(); // Experimental code !!                   // TO DO move rest of this {} into Init !
                    Logger.Log(string.Format("Initialized all components"));
                    theMusicXmlFileName = fullXmlFileName;
                }
                else
                {
                    Logger.Log(string.Format("Failed to load {0} because it not a valid MusicXml file",xmlFileName));
                    theMusicXmlFileName = "";
                }
            }
            catch (System.Exception e)
            {
                Logger.Log(string.Format("Failed to load {0} ({1})", xmlFileName, e.Message));
                ok = false;
            }

            return ok;
        }
 
        /// <summary>
        /// To use a console in a Windows Forms application change:
        /// Project Properties -> Application -> Output Type -> Console Application
        /// Original value was "Windows Application"
        /// </summary>
        private void InitTestConsole(bool use)
        {        
            if (use)
            {
                try
                {
                    Console.SetWindowPosition(0, 0);
                    Console.SetWindowSize(140, 20); // Seems to be a good compromize
                }
                catch (Exception)
                {
                    // Quietly stop using the console if it is not there!
                    use = false;
                }
            }
            Logger.UseConsole = use;
        }

        void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                musicPlayer.StopPlaying();
                Logger.Log("The application is exiting");
            }
            catch (Exception)
            {
                // Ignore any errorsat this point!
            } 
        }

        private Model()
        {
            // Prevent creation 
        }

        /// <summary>
        /// Constructor to be used by UI-based applications
        /// </summary>
        private Model(IObjectCollection objects, IDebugDisplayerClient iDebugDisplayerClient, string caption)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);
            is64Bit = IntPtr.Size == 8;
            InitTestConsole(true); // Please see the Log methode for details!
            Logger.Log(""); // An empty line
            Logger.Log(string.Format("Date={0}:", System.DateTime.Now.ToLongDateString()));
            Logger.Log(string.Format("{0} started in '{1}'", System.IO.Path.GetFileName(executingAssembly), executingDirectory));
            //Logger.LogSystemInformation();
            Utilities.CheckDlls(executingDirectory, caption, is64Bit);


            // Create an API to JAWS or NVDA depending on which screenreader is currently running
            debugTools = DebugTools.Create(); // Used for logging and tracing from screenReaderAPI.
            screenReaderAPI = ScreenReaderAPI.Create(is64Bit,debugTools);
            externalToolsHandler = ExterrnalToolsHandler.Create();
            Utilities.CheckScreenReader(screenReaderAPI.ScreenReaderName, caption); // Check for DummyScreenReader

            midiOut = new MidiOut(0);
            musicPlayer = new MusicPlayer(objects,midiOut);
            int displaySize = 14;
            brailleDisplayer = BrailleDisplayer.Create(iDebugDisplayerClient, displaySize, screenReaderAPI); // TODO Get the real displaysize from somewhere
            textDisplayer = TextDisplayer.Create(iDebugDisplayerClient);
            Logger.Log(string.Format("Model: Assuming size of physical Braille display = {0}", displaySize));

            //musicPlayer.ChangeInstrument(20); // Church Organ

            this.objects = objects;

            //Model.Log(string.Format("listBoxPoly.AccessibleDefaultActionDescription={0}", listBoxPoly.AccessibleDefaultActionDescription));
            //Model.Log(string.Format("listBoxPoly.AccessibilityObject.ToString()={0}", listBoxPoly.AccessibilityObject.ToString())); 
            

            // Log some global system parameters.
            Logger.LogSystemParameters();
            // Log availability of NVDA interface.


            //LogNvdaInterface(); // Will be replaced by ScreenReaderAPI.Create() !

            //DeviceInfo.LogDeviceInfo();
        }

        /// <summary>
        /// Handels the syntax analysis of an XML node representing a MusicXML element while reading the MusicXML file.
        /// </summary>
        /// <param name="node">An XML node representing a MusicXML element.</param>
        /// <returns>True <==> Further recursion is required.</returns>
        private bool WriteElement(XmlNode node)
        {
            string functionName = "WriteElement";
            bool continueRecursion = true;
            switch (node.Name)
            {
                case "note":
                    if (this.currentMeasureNumber != this.latestMeasureNumber)
                    {
                        this.latestMeasureNumber = this.currentMeasureNumber;
                    }
                    NoteElement note = NoteElement.Create(node, this.divisions, this.currentMeasureNumber, this.currentScorePartElement, this.currentTimeElement); // New version
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
                    userSettings.defaultStringFormat = (ScreenReaderAPI.ScreenReaderType.NVDA == screenReaderAPI.GetScreenReaderType()) ? "{1}" : "{0} {1}";                          
                    // allMusicXmlObjecsts.AddRange(   (partitionList.ToStrings());
                    continueRecursion = false;
                    break;
                case "measure":
                    MeasureElement measureElement = MeasureElement.Create(node);
                    allMusicXmlObjecsts.Add(measureElement); // Avoid the "Ikke VAlgt" error message from screenreader
                    this.currentMeasureNumber = measureElement.Number; 
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
                    SimpleTextElement movementTitle = SimpleTextElement.Create(node, "Opus");
                    allMusicXmlObjecsts.Add(movementTitle);
                    metaInfoStrings.Add(movementTitle.ToString());
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
                    allMusicXmlObjecsts.Add(DirectionElement.Create(node));
                    continueRecursion = false;
                    break;
                case "transpose":
                    TransposeElement transposeElement = TransposeElement.Create(node);
                    allMusicXmlObjecsts.Add(transposeElement);
                    this.currentScorePartElement.TransposeElement = transposeElement;
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
                    currentTimeElement = TimeElement.Create(node);
                    allMusicXmlObjecsts.Add(currentTimeElement);
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
                case "attributes": // Maybe attributes are always found under MeasureElement ??
                    //Logger.LogOnce(string.Format("{0}.{1} Unimplemented element: Name={2} Parent.Name={3}",
                    //    className, functionName, node.Name,node.ParentNode.Name));
                    allMusicXmlObjecsts.Add(AttributesElement.Create(node));       // Ignore until we need them 
                    // WE CONTINUE RECURSION below the attributes element, which may contain a lot of other relevant elements:
                    // footnote, level, divisions, key, time, staves, part-symbol,instruments, clef, staff-details, transpose, directive,measure-style
                    // For the time being there is no need to structure these elements into the Attribute Element !          
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

                case "barline":
                    allMusicXmlObjecsts.Add(BarlineElement.Create(node));
                    continueRecursion = false;
                    //Model.Log(string.Format("Model.WriteElement: Unimplemented element 'repeat' Part={0} Measure={1}", currentPartId, currentMeasureNumber));
                    break;

                case "instruments":
                    allMusicXmlObjecsts.Add(InstrumentsElement.Create(node));
                    continueRecursion = false;
                    break;

                case "source":
                    SimpleTextElement source = SimpleTextElement.Create(node, "Source");
                    allMusicXmlObjecsts.Add(source);
                    metaInfoStrings.Add(source.ToString());
                    continueRecursion = false;
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
                case "bar-style":
                case "staff-size":
                case "staff-lines":
                case "staff-tuning":
                case "staff-octave":
                    break; // Explicitly ignoring graphic information!
                case "tuning-octave":
                case "tuning-step":
                case "capo":
                    break; // Also ignore these until they are needed!
                default:
                    Logger.LogOnce(string.Format("{0}.{1} Unimplemented Element. Name={2}",className,functionName,node.Name));
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

        /// <summary>
        ///  This program is compiled and for the following architechture: false:x86 true:x64 
        /// </summary>
        public bool Is64Bit
        {
            get
            {
                return is64Bit;
            }
        }

        public EventDescriptionList EventDescriptionList
        {
            get
            {
                return eventDescriptionList;
            }
            
        }

        public List<string> MetaInfoStrings
        {
            get
            {
                if (userSettings.GetReaderSettings(UserSettings.ReaderSettings.MetaInformation))
                {
                    return metaInfoStrings;
                }
                else
                {
                    return new List<string>(); // Return an empty string
                }
            }

        }

        public ExterrnalToolsHandler ExternalToolsHandler
        {
            get
            {
                return externalToolsHandler;
            }

        }

        public string TheMusicXmlFileName
        {
            get
            {
                return theMusicXmlFileName;
            }
        }

        public List<MusicXmlObject> AllMusicXmlObjecsts
        {
            get
            {
                return allMusicXmlObjecsts;
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

        public bool ToggleStartStopPlaying(int startIndex)
        {
            return musicPlayer.ToggleStartStopPlaying(numberOfParts, startIndex);           
        }


 
        public bool StartPlayingPoly(int startIndex)
        {
            // We need an index to start at and at least 1 part to play ! 
            if (-1 == startIndex) 
            {
                if (0 == numberOfParts)
                {
                    return false;
                }
                else
                {
                    startIndex = 0; // Start at the beginning
                }
            }
            //musicPlayer.Reset(numberOfParts,startIndex);
            musicPlayer.StartPlaying(numberOfParts,startIndex);
            return true;
        }

        public void StopPlaying()
        {
            musicPlayer.StopPlaying();
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

        public void PlaySpeedChanged(float newValue)
        {
            userSettings.userSlowDown = newValue;
        }


        public void StopRefreshingBrailleDevice()
        {
            brailleDisplayer.StopRefreshing();
        }

        public void StartRefreshingBrailleDevice()
        {
            brailleDisplayer.StartRefreshing();
        }


        /// <summary>
        /// Define an interval to be repeated.
        /// </summary>
        /// <param name="firstMeasure"></param>
        /// <param name="lastMeasure"></param>
        public bool StartRepeating(int firstMeasure, int lastMeasure)
        {
            //musicPlayer.Reset(numberOfParts,-1);
            return musicPlayer.StartRepeating(numberOfParts,firstMeasure, lastMeasure);
        }

        public bool TogglePlaying(int startIndex)
        {
            return musicPlayer.ToggleStartStopPlaying(numberOfParts,startIndex);
        }

        public void StopRepeating()
        {
            musicPlayer.StopRepeating();
        }

        /// <summary>
        /// This method must be called by the application before exit
        /// in order to allow the Model to clean up and release any resources etc.
        /// </summary>
        public void OnApplicationExit()
        {
            string functionName = "OnApplicationExit";
            try
            {
                musicPlayer.StopPlaying();
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} Exception.Message={2}", className, functionName, e.Message));
            }

            System.Threading.Thread.Sleep(500); // Allow the player to stop

            // Finally tell force the closing of all Midi channels:
            try
            {
                // TODO Call MIDI "AllNotesOff"

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} Exception.Message={2}", className, functionName, e.Message));
            }

            System.Threading.Thread.Sleep(100); // Allow the Midi system to stop all sounds

            Logger.Log(string.Format("{0}.{1} succeeded.", className, functionName)); 
        }
    }
}
