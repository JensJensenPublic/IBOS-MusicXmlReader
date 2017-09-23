using System;
using System.IO;
using System.Collections.Generic;
using NAudio.Midi;
using System.Xml;
using JSJ.ScreenReaderAPI;
//using MusicXmlReaderUI; // Interfaces
using JSJ.MusicSynthesis; // Avoid by making new class

namespace MusicXmlReaderModel
{
    /// <summary>
    /// The Model class is the central hub in the MusicXmlReaderModel project.
    /// It connects the other classes in the MusicXmlReaderModel project.
    /// It plays the "Model" role in the MVVM (Model, Viev, ViewModel) architecture used in the MusicXmlReader solution
    /// As written in C# it is easily ported to other OS arvhitechtures, such as iOS and Android, using the Xamarin development tool
    /// </summary>
    public class Model
    {
        static public string TheStaticXmlFileName = "";
        string className = "Model";
        string theMusicXmlFileName = "";
        string myMusicXmlDirectory = ""; // Typically "C:\Users\<user>\\Documents\IBOS Nodelæser"
        string myMusicXmlSampleDirectory = ""; //  Typically "C:\Users\<user>\\Documents\IBOS Nodelæser\Eksempler"
        bool is64Bit; // This program is compiled and for the following architechture: false:x86 true:x64 
        List<MusicXmlObject> allMusicXmlObjecsts; // Holds all information from the .xml file
        MidiOut midiOut;
        IObjectCollection objects;
        public MusicPlayer musicPlayer;
        public BrailleDisplayer brailleDisplayer;
        public TextDisplayer textDisplayer;
        public PartlistElement partList; // Contains the list of parts, describing all instruments used including their midi parameters
        int divisions; // Current number of divisions of a quarternode
        int currentMeasureNumber = 0; // Current measure number
        StatusInformation currentStatusInformation; // Contains information which is valid in a part of the score, such as Key, Beats, Tempo etc.
        int latestMeasureNumber = 0;
        int numberOfParts; // Number of parts
        string currentPartId = "";
        ScorePartElement currentScorePartElement = null;
        TimeElement currentTimeElement; // Contains the current TimeElement
        UserSettings userSettings;
        ScreenReaderAPI screenReaderAPI;
        DebugTools debugTools;
        ExternalToolsHandler externalToolsHandler;
        IDebugDisplayerClient iDebugDisplayerClient;
        ProgressWriter loaderProgressWriter = null;
        ProgressWriter conversionProgressWriter = null;

        string executingAssembly;
        string executingDirectory;

        public string ScreenReaderName
        {
            get { return (null == screenReaderAPI) ? "" : screenReaderAPI.ScreenReaderName; }
        }

        MetaInformation metaInformation = MetaInformation.Create(); // Holds filename, title, composer, arranger etc.

        /// <summary>
        /// Create to be used by UI-less applications
        /// </summary>
        /// <returns></returns>
        static public Model Create()
        {
            return new Model(null, null, null);
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
            ok = ok && (doc.ChildNodes[1].Name.Contains("score-partwise"));
            return ok;
        }

        /// <summary>
        /// Writes the text to the status line in the UI handling X-thread issues etc!
        /// </summary>
        /// <param name="s"></param>
        private void WriteStatusInformation(string s)
        {
            if (null == iDebugDisplayerClient)
            {
                return;
            }
            iDebugDisplayerClient.WriteStatusInformation(s); // Handle the UI-les case
        }


        /// <summary>
        /// Stops on any error and returns false
        /// </summary>
        /// <param name="fullXmlFileName"></param>
        /// <returns></returns>
        public bool LoadMusicXmlFile(string fullXmlFileName)
        {
            string functionName = "LoadMusicXmlFile";
            bool ok = true;
            string xmlFileName = ""; // The MusicXml file currently handled 
            // ProgressWriter progressWriter = null;
            try
            {
                string defaultFileName = "Node.xml";
                if (string.IsNullOrEmpty(fullXmlFileName))
                {
                    // Use a default value             
                    fullXmlFileName = System.IO.Path.Combine(executingDirectory, defaultFileName);
                }


                xmlFileName = System.IO.Path.GetFileName(fullXmlFileName); // Report a filename even if an exception is thrown during conversion !


                if (".mxl" == Path.GetExtension(fullXmlFileName))
                {
                    // This is a compressed MusicXml file in the .mxl format
                    string mxlFileName = System.IO.Path.GetFileName(fullXmlFileName);
                    string progressConverting = string.Format("{0} {1} {2}", ResourcesForModel.Progress_Converting, mxlFileName,ResourcesForModel.Progress_FromMxlToXml);
                    conversionProgressWriter = ProgressWriter.Create(1000, iDebugDisplayerClient, progressConverting); 
                    fullXmlFileName = Utilities.MxlToXml(fullXmlFileName, executingDirectory);
                    conversionProgressWriter.Stop();
                    // ToDo Error handling
                }

                xmlFileName = System.IO.Path.GetFileName(fullXmlFileName);
                TheStaticXmlFileName = xmlFileName ; // Make the fikename globally available without a reference to a Model instance.

                XmlDocument doc = new XmlDocument();
                XmlTextReader reader = new XmlTextReader(fullXmlFileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                string progressLoading = string.Format("{0} {1}", ResourcesForModel.Progress_LoadingFile, xmlFileName);
                loaderProgressWriter = ProgressWriter.Create(1000, iDebugDisplayerClient, progressLoading); 
                doc.Load(reader); // This single operation may last decades of seconds on a slow platform!!
                loaderProgressWriter.Stop();
                ok = ok && CheckMusicXmlSyntax(doc);
                // ok = false; //For test only
                if (ok)
                {
                    string status = "";
                    metaInformation = MetaInformation.Create();
                    metaInformation.FileName = MetaInfoItem.Create(ResourcesForModel.MetaInfoText_FileName, Path.GetFileName(fullXmlFileName));
                    Logger.Log(string.Format("{0}.{1}: Loaded >>>>>>>>>> '{2}' <<<<<<<<<<", className, functionName, Path.GetFileName(fullXmlFileName)));
                    Logger.Log(string.Format("{0}.{1}: From   '{2}'", className, functionName,Path.GetDirectoryName(fullXmlFileName)));
                    allMusicXmlObjecsts = new List<MusicXmlObject>(); // Create the list holding all MusicXml elements read from file
                    status = string.Format("{0} {1}", ResourcesForModel.Status_Interpreting, xmlFileName);
                    WriteStatusInformation(status);
                    MidiPitchedChannelMap.Reset();
                    Recurse(doc.ChildNodes);                          // Build  the list holding all MusicXml elements read from file
                    Logger.Log(string.Format("{0}.{1}: Parsed '{2}'", className, functionName,xmlFileName));
                    status = string.Format("{0} {1}", ResourcesForModel.Status_BuildingDataStructuresFor, xmlFileName);
                    WriteStatusInformation(status); 
                    Init();  // Initialize the basic Model data structures.
                    musicPlayer.ResetInstrumentMapping(); // Initialize the MusicPlayer data structures
                    theMusicXmlFileName = fullXmlFileName;
                    status = string.Format("{0} {1}", xmlFileName,ResourcesForModel.Status_WasSuccessfullyLoaded);
                    WriteStatusInformation(status);
                }
                else
                {
                    string status = string.Format("{0} '{1}'.   {2}", ResourcesForModel.Status_FailedToLoad, xmlFileName, ResourcesForModel.Status_ItIsNotAValidMusicXmlFile);
                    WriteStatusInformation(status); 
                    Logger.Log(string.Format("{0}.{1}: Failed to load '{2}' because it not a valid MusicXml file", className, functionName, xmlFileName));
                    theMusicXmlFileName = "";
                }
                //throw (new Exception("For test only")); // For test only
            }
            catch (System.Exception e)
            {
                Logger.Log(string.Format("{0}.{1}: Failed to load '{2}' Exception.Message='{3}'", className, functionName, xmlFileName, e.Message));

                if (null != loaderProgressWriter)
                {
                    loaderProgressWriter.Stop(); // Be sure to stop any running progresswriter
                }
                if (null != conversionProgressWriter)
                {
                    conversionProgressWriter.Stop(); // Be sure to stop any running progresswriter
                }
                string status = string.Format("{0} '{1}'.  {2}", ResourcesForModel.Status_FailedToLoad, xmlFileName, e.Message);
                WriteStatusInformation(status);

                ok = false;
            }

            return ok;
        }


        public List<string> ImportNewestDownloads()
        {
            string functionName = "ImportNewestDownloads";
            List<string> result = new List<string>();
            try
            {
                bool defaultUser = false;
                string downloadPath = KnownFolders.GetPath(KnownFolder.Downloads, defaultUser); // Get the path to the current user.
                string destinationPath = myMusicXmlDirectory;
                Logger.Log(string.Format("{0}.{1}: DownloadPath={2} DestinationPath={3}", className, functionName, downloadPath, destinationPath));
                string[] files =  Directory.GetFiles(downloadPath);
                //Logger.Log(string.Format("{0}.{1}: Files={2}", className, functionName, files.GetLength(0)));
                List<string> musicXmlFiles = new List<string>();
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file);
                    if ((".xml" == extension) || (".mxl" == extension))
                    {
                        musicXmlFiles.Add(file);
                    }
                }
                //Logger.Log(string.Format("{0}.{1}: MusicXml files={2}", className, functionName, musicXmlFiles.Count));
                List<string> todaysMusicXmlFiles = new List<string>();
                DateTime toDay = DateTime.Now.Date;
                foreach (string file in musicXmlFiles)
                {

                    FileInfo fi = new FileInfo(file);
                    DateTime fileDate = fi.LastWriteTimeUtc.Date;
                    if ((fileDate.Year == toDay.Year) && (fileDate.Month == toDay.Month) && (fileDate.Day == toDay.Day))
                    {
                        todaysMusicXmlFiles.Add(file);

                    }
                }
                Logger.Log(string.Format("{0}.{1}: Files={2} MusicXml={3} Today={4}", 
                    className, functionName, files.GetLength(0), musicXmlFiles.Count, todaysMusicXmlFiles.Count));

                // Copy files

                foreach (string file in todaysMusicXmlFiles)
                {
                    string shortFileName = Path.GetFileName(file);
                    string destFileName = Path.Combine(destinationPath, shortFileName);
                    if (File.Exists(Path.Combine(destinationPath, shortFileName)))
                    {
                        Logger.Log(string.Format("{0}.{1}: Skipping {2} because it has already been imported", className, functionName, file));
                    }
                    else
                    {
                        try
                        {
                            File.Copy(file, destFileName, false); // False <==> Do not overwrite existing
                            Logger.Log(string.Format("{0}.{1}: Copied {2}", className, functionName, file));
                            result.Add(Path.GetFileName(file));
                        }
                        catch (Exception e)
                        {
                            Logger.Log(string.Format("{0}.{1}: Failed to copy {2} Exception.Message={3}", className, functionName,file, e.Message));
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} Failed. Exception.Message={2}", className, functionName, e.Message)); 
            }
            return result;
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
                Logger.Log("The application is exiting.");
                Logger.Log("");
            }
            catch (Exception)
            {
                // Ignore any errors at this point!
            }
        }

        private Model()
        {
            // Prevent creation 
        }


        /// <summary>
        /// Convenience method for creating an NAudio MidiOut device and logging its capabilities
        /// Catshes any exception and logs it.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <returns></returns>
        private MidiOut CreateMidiOut(int deviceNumber)
        {
            const string functionName = "CreateMidiOut";
            MidiOut midiOut = null;
            try
            {
                // throw new Exception("For test only");
                midiOut = new MidiOut(deviceNumber);
                MidiOutCapabilities mc = MidiOut.DeviceInfo(deviceNumber);
                Logger.Log(string.Format("{0}.{1}: Created MidiOut({2}) for ProductName='{3}' Technology='{4}' ProductId={5} Notes={6} ",
                    className, functionName, deviceNumber, mc.ProductName,  mc.Technology, mc.ProductId, mc.Notes ));
                Logger.Log(string.Format("{0}.{1}: Supports: AllChannels={2} MidiStreamOut={3} PatchCatching={4} SeparateLeftAndRightVolume={5} VolumeControl={6})",
                    className, functionName, mc.SupportsAllChannels, mc.SupportsMidiStreamOut, mc.SupportsPatchCaching, mc.SupportsSeparateLeftAndRightVolume, mc.SupportsVolumeControl));
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1}: Creation of MidiOut({2}) failed. Exception.Message='{3}'", className, functionName, deviceNumber, e.Message));
                Utilities.ShowWarning(ModelMessageEnum.UnspecifiedInitializationError, string.Format("MidiOut({0})=null", deviceNumber), "");
                // Do NOT rethrow, as this will prevent the creation of the Model and make the Logfile unavailable !!        
            }
            return midiOut;
        }



        /// <summary>
        /// Constructor to be used by UI-based applications
        /// </summary>
        private Model(IObjectCollection objects, IDebugDisplayerClient iDebugDisplayerClient, string caption)
        {
            string methodName = "Model";

#if false
            // http://stackoverflow.com/questions/915210/how-can-i-get-the-path-of-the-current-users-application-data-folder
            string s = "";
            s = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);      // C:\Users\Jens\AppData\Local
            s = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);           // C:\Users\Jens\AppData\Roaming
            s = Environment.GetFolderPath(Environment.SpecialFolder.System);                    // C:\WINDOWS\system32
            s = System.Environment.SystemDirectory;                                             // C:\WINDOWS\System32
            s = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // C:\Users\Jens\Documents
            s = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Recent);      // C:\Users\Jens\AppData\Roaming\Microsoft\Windows\Recent
            s = System.IO.Path.GetPathRoot(System.Environment.SystemDirectory);                 // C:\
            s = KnownFolders.GetPath(KnownFolder.Downloads, false);   
#endif

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);
            is64Bit = IntPtr.Size == 8;
            InitTestConsole(true); // Please see the Log methode for details!
            Logger.Log(string.Format("{0}.{1}: Date={2}", className, methodName, System.DateTime.Now.ToLongDateString()));
            Logger.Log(string.Format("{0} started in '{1}'", System.IO.Path.GetFileName(executingAssembly), executingDirectory));
            //Logger.LogSystemInformation();
            Utilities.CheckDlls(executingDirectory, caption, is64Bit);


            // Create an API to JAWS or NVDA depending on which screenreader is currently running
            debugTools = DebugTools.Create(); // Used for logging and tracing from screenReaderAPI.
            screenReaderAPI = ScreenReaderAPI.Create(is64Bit, debugTools);
            externalToolsHandler = ExternalToolsHandler.Create();
            Utilities.CheckScreenReader(screenReaderAPI.ScreenReaderName, caption); // Check for DummyScreenReader

            midiOut = CreateMidiOut(0);
            musicPlayer = new MusicPlayer(objects, midiOut);
            int displaySize = 40;
            brailleDisplayer = BrailleDisplayer.Create(iDebugDisplayerClient, displaySize, screenReaderAPI); // TODO Get the real displaysize from somewhere
            textDisplayer = TextDisplayer.Create(iDebugDisplayerClient);
            Logger.Log(string.Format("Model: Assuming size of physical Braille display = {0}", displaySize));
            this.objects = objects;
            this.iDebugDisplayerClient = iDebugDisplayerClient;
            Logger.LogSystemParameters();
            Logger.LogDebugerAttachment();
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
                    // Intialization of instruments has been moved to MusicPlayer (where it belongs)
                    break;
                case "work":
                    SimpleTextElement workElement = SimpleTextElement.Create(node, "Titel"); // TODO: Localize
                    allMusicXmlObjecsts.Add(workElement);
                    metaInformation.Work = MetaInfoItem.Create(workElement.Name, workElement.Text);
                    continueRecursion = false;
                    break;
                case "movement-title":
                    SimpleTextElement movementTitle = SimpleTextElement.Create(node, "Opus"); // TODO: Localize
                    allMusicXmlObjecsts.Add(movementTitle);
                    metaInformation.MovementTitle = MetaInfoItem.Create(movementTitle.Name, movementTitle.Text);
                    continueRecursion = false;
                    break;
                case "movement-number":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Nummer"));
                    continueRecursion = false;
                    break;
                case "identification":
                    break;
                case "creator":
                    CreatorElement creatorElement = CreatorElement.Create(node);
                    allMusicXmlObjecsts.Add(creatorElement);
                    metaInformation.Creator = MetaInfoItem.Create(creatorElement.Name, creatorElement.Value);
                    continueRecursion = false;
                    break;
                case "rights":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Rettigheder"));
                    continueRecursion = false;
                    break;
                case "encoding":
                    // Is described in "software", "encoding-date", "encoder", "encoding-description"
                    //allMusicXmlObjecsts.Add(SimpleTextElement.Create(node)); 
                            
                    break;
                case "software":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Software"));
                    continueRecursion = false;
                    break;
                case "encoding-date":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Arrangements dato"));
                    continueRecursion = false;
                    break;
                case "encoder":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Arrangement"));
                    continueRecursion = false;
                    break;
                case "encoding-description":
                    SimpleTextElement encodingDescriptionElement = SimpleTextElement.Create(node, "Kodnings-beskrivelse");
                    allMusicXmlObjecsts.Add(encodingDescriptionElement);
                    Logger.LogOnce(string.Format("{0}.{1}: Encoding='{2}'",className,functionName,node.InnerText));
                    metaInformation.Encoding = MetaInfoItem.Create(encodingDescriptionElement.Name, encodingDescriptionElement.Text);
                    continueRecursion = false;
                    break;
                case "direction":
                    DirectionElement directionElement = DirectionElement.Create(node);
                    allMusicXmlObjecsts.Add(directionElement);
                    if (null != directionElement.SoundElement)
                    {
                        // DirectionElement may contain a soundelement
                        allMusicXmlObjecsts.Add(directionElement.SoundElement);
                    }
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
                    this.divisions = divisionsElement.Divisions;
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
                case "measure-repeat":
                    allMusicXmlObjecsts.Add(MeasureRepeatElement.Create(node));
                    continueRecursion = false;
                    break;
                case "measure-style":
                    allMusicXmlObjecsts.Add(MeasureStyleElement.Create(node));
                    continueRecursion = false;
                    break;
                case "backup":
                    allMusicXmlObjecsts.Add(BackupElement.Create(node, divisions));
                    continueRecursion = false;
                    break;
                case "forward":
                    allMusicXmlObjecsts.Add(ForwardElement.Create(node, divisions));
                    continueRecursion = false;
                    break;
                case "repeat":
                    // TODO Implement !!
                    allMusicXmlObjecsts.Add(RepeatElement.Create(node));
                    break;
                case "barline":
                    allMusicXmlObjecsts.Add(BarlineElement.Create(node));
                    continueRecursion = false;
                    break;
                case "instruments":
                    allMusicXmlObjecsts.Add(InstrumentsElement.Create(node));
                    continueRecursion = false;
                    break;
                case "source":
                    SimpleTextElement source = SimpleTextElement.Create(node, "Source");
                    allMusicXmlObjecsts.Add(source);
                    metaInformation.Source = MetaInfoItem.Create(source.Name, source.Text);
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
                    Logger.LogOnce(string.Format("{0}.{1} Unimplemented Element. Name={2}", className, functionName, node.Name));
                    allMusicXmlObjecsts.Add(UnimplementedElement.Create(node));
                    break;
            }
            return continueRecursion;
        }

        public void Recurse(XmlNodeList childrenNodes)
        {
            foreach (XmlNode childNode in childrenNodes)
            {
                bool doRecursion = true;
                switch (childNode.NodeType)
                {
                    case XmlNodeType.Element:
                        doRecursion = WriteElement(childNode);
                        break;
                    case XmlNodeType.Comment:
                        break;
                }
                if (doRecursion)
                {
                    Recurse(childNode.ChildNodes);
                }

            }
        }

        // The following 2 lists contain references into allMusicXmlObjecsts where the decoded information is kept! 
        private PartDescriptionList partDescriptionList;
        private TimeDescriptionList timeDescriptionList;
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

        public ExternalToolsHandler ExternalToolsHandler
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

        public MetaInformation MetaInformation
        {
            get
            {
                return metaInformation;
            }
        }

        public void Init()
        {
            partDescriptionList = PartDescriptionList.Create(allMusicXmlObjecsts, numberOfParts);
            divisions = 24; // TODO compute!
            timeDescriptionList = TimeDescriptionList.Create(partDescriptionList, divisions);
            currentStatusInformation = StatusInformation.Create(MusicPlayer.defaultMusicPlayerTempo);
            eventDescriptionList = EventDescriptionList.Create(timeDescriptionList, numberOfParts, userSettings, currentStatusInformation);
        }


        /// <summary>
        /// Converts a measureNumber to an index in the current EventDescriptionList
        /// Returns true iff the conversion was possible
        /// Primarily used by the UI, not needed by the Model code itself!
        /// </summary>
        /// <param name="measure"></param>
        /// <returns></returns>
        public bool MeasureToIndex(int measureNumberToFind, ref int index)
        {
            string functionName = "MeasureToIndex";
            if (null == eventDescriptionList) return false;
            try
            {
                for (int i = 0; (i < eventDescriptionList.Events.Count); i++)
                {
                    // Find the first match, representing the first event in the measure.
                    int m = eventDescriptionList.Events[i].MeasureNumber;
                    if (m == measureNumberToFind)
                    {
                        index = i;
                        Logger.Log(string.Format("{0}.{1} Found measure number {2} at index {3}", className, functionName, measureNumberToFind, index));
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} Exception thrown while searching measure number {2} Message={3}", className, functionName, measureNumberToFind, e.Message));
            }
            Logger.Log(string.Format("{0}.{1} Failed to find measure number {2}", className, functionName, measureNumberToFind));
            return false;
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
            if (!musicPlayer.StartPlaying(numberOfParts, startIndex))
            {
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.NotAllowedWhilePlaying, "", "");
            }
            return true;
        }

        public void SetUserTempo(int tempo)
        {
            UserSettings.UserTempo = tempo;
            musicPlayer.SetUserTempo(tempo); // Change the actual tempo
            currentStatusInformation.CurrentTempoModification = tempo; // Show the new tempo in the status line
        }

        public void ChangeUserTempo(int change)
        {
            UserSettings.UserTempo += change;
            musicPlayer.SetUserTempo(UserSettings.UserTempo); // Change the actual tempo
            currentStatusInformation.CurrentTempoModification = UserSettings.UserTempo; // Show the new tempo in the status line
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
            musicPlayer.UserSettings = this.userSettings;
        }

        public void SetPartsToRead(int partNumber, bool value)
        {
            this.userSettings.partsToRead[partNumber] = value;
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
        public void StartRepeating(int firstMeasure, int lastMeasure)
        {
            if (!musicPlayer.StartRepeating(numberOfParts, firstMeasure, lastMeasure))
            {
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.NotAllowedWhilePlaying, "", "");
            }
        }

        public bool TogglePlaying(int startIndex)
        {
            return musicPlayer.ToggleStartStopPlaying(numberOfParts, startIndex);
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
                musicPlayer.DamperThreadStop();
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

            if (null != screenReaderAPI)
            {
                screenReaderAPI.OnApplicationExit();
            } 

            
            Logger.Log(string.Format("{0}.{1} succeeded.", className, functionName));
        }


        /// <summary>
        /// During first activation copy sample files from the installation, typically
        /// From C:\Program Files (x86)\...
        /// To   C:\Users\"user"\Documents\"LocalizedApplicationName"
        /// Where "user" is current Windows username
        /// and   "LocalizedApplicationName" could be "IBOS Nodelæser" or "IBOS MusicXmlReader" or any other localized application name 
        /// </summary>
        /// <param name="applicationName">Localized application name</param>
        /// <param name="sampleDirName">Location of sample files, distributed with the installation files</param>
        /// <returns></returns>
        public string InitMusicXmlFiles(string applicationName, string sampleDirName)
        {
            string sourceDirName = InitialDirectory;
            string functionName = "InitMusicXmlFiles";
            string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // C:\Users\<Username>\Documents       
            myMusicXmlDirectory = Path.Combine(documentPath, applicationName);              //   C:\Users\<Username>\Documents\IBOS Nodelæser
            myMusicXmlSampleDirectory = Path.Combine(myMusicXmlDirectory, sampleDirName);   //   C:\Users\<Username>\Documents\IBOS Nodelæser\Eksempler
            int nFiles = 0;
            int nDirs = 0;
            List<string> fileNames = new List<string>();
            if (!Directory.Exists(myMusicXmlDirectory))
            {
                try
                {
                    // Create the destination directory:
                    Directory.CreateDirectory(myMusicXmlDirectory);  // C:\Users\<Username>\Documents\IBOS MusicXmlReader            
                    Directory.CreateDirectory(myMusicXmlSampleDirectory);  // C:\Users\<Username>\Documents\IBOS MusicXmlReader\Eksempler
                    Logger.Log(string.Format("{0}.{1}: Calling DirectoryCopy(Source,Dest) where", className, functionName));
                    Logger.Log(string.Format(" Source='{0}'", sourceDirName));
                    Logger.Log(string.Format(" Dest=  '{0}'", myMusicXmlSampleDirectory));
                    Utilities.DirectoryCopy(sourceDirName, myMusicXmlSampleDirectory, true, ref nFiles, ref nDirs,fileNames);  // true to copy subdirs
                    Logger.Log(string.Format("{0}.{1}: DirectoryCopy() successfully copied {2} files in {3} directories", className, functionName, nFiles, nDirs));
                    // throw (new Exception("For test only"));
                }
                catch (Exception e)
                {
                    Logger.Log(string.Format("{0}.{1}: Exception during DirectoryCopy(): Message='{2}'",
                        className, functionName, e.Message));
                }
            }
            return myMusicXmlDirectory;
        }


        /// <summary>
        /// Imports all new sample files and directories.
        /// Assumes that member variables defining all paths have already been set up by InitMusicXmlFiles()
        /// </summary>
        public List<string> ImportNewSampleFiles()
        {
            string functionName = "ImportNewSampleFiles";
            string sourceDirName = InitialDirectory;
            List<string> result = new List<string>();
            int nDirs = 0;
            int nFiles = 0;
            Logger.Log(string.Format("{0}.{1}: Calling DirectoryCopy(Source,Dest) where", className, functionName));
            Logger.Log(string.Format(" Source='{0}'", sourceDirName));
            Logger.Log(string.Format(" Dest=  '{0}'", myMusicXmlSampleDirectory));
            Utilities.DirectoryCopy(sourceDirName, myMusicXmlSampleDirectory, true, ref nFiles, ref nDirs,result);
            Logger.Log(string.Format("{0}.{1}: DirectoryCopy() successfully copied {2} files in {3} directories", className, functionName, nFiles, nDirs));
            return result;
        }

        private DetailsPlayer detailsPlayer= null; 
 
        public DetailsDescription[] GetCurrentHarmonyDetails(EventDescription eventDescription)
        {
            if (null != eventDescription.HarmonyElement)
            {
                detailsPlayer = DetailsPlayer.Create(eventDescription.HarmonyElement, musicPlayer);
                return detailsPlayer.DetailsDescriptionArray;
            }
            else
            {
                // Return message to the user that no chord was found.
                DetailsDescription[] noDescriptions = new DetailsDescription[1];
                noDescriptions[0] =  DetailsDescription.Create(ResourcesForModel.DetailsDescription_NoChordFound);
                return noDescriptions;
            }
        }


        public DetailsDescription[] GetCurrentEventDetails(EventDescription eventDescription)
        {

            if (null != eventDescription)
            {
                detailsPlayer = DetailsPlayer.Create(eventDescription,partList,userSettings,musicPlayer);
                return detailsPlayer.DetailsDescriptionArray;
            }
            else
            {
                return new DetailsDescription[0]; // No Event. Return an empty array.
            }

        }

        public DetailsDescription[] GetAllPartDetails()
        {
            if (null != partList)
            {
                detailsPlayer = DetailsPlayer.Create(partList);
                string[] details = partList.ToUserFriendlyStrings();
                foreach (string detail in details)
                {
                    detailsPlayer.DetailsDescriptionList.Add(DetailsDescription.Create(detail));
                }
                return detailsPlayer.DetailsDescriptionArray;
            }
            else
            {
                return new DetailsDescription[0]; // No partlist found. Return an empty array.
            }
        }


        /// <summary>
        /// Move selection to the start of the next or previous measure
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="move"></param>
        public void SelectMeasure(int selectedIndex,int move)
        {
            string functionName = "SelectMeasure";
            Logger.Log(string.Format("{0}.{1}(SelectedIndex={2},Move={3})",className,functionName, selectedIndex,move));
            try
            {
                EventDescription currentEvent = (this.objects.GetObjectAtIndex(selectedIndex) as EventDescription);
                int currentMeasureNumber = currentEvent.MeasureNumber;
                int newMeasureNumber = currentMeasureNumber + move;
                for (int i = selectedIndex; ((i+move) >= 0) && ((i + move) < this.objects.GetNumberOfObjects());)
                {
                    i += move;
                    EventDescription nextEvent = (this.objects.GetObjectAtIndex(i) as EventDescription);
                    if (nextEvent.MeasureNumber == newMeasureNumber)
                    {
                        if ((i >= 0) && (i < this.objects.GetNumberOfObjects()))
                        {
                            objects.SetSelectedIndex(i);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1}(SelectedIndex={2},Move={3}) failed. Message={4}", className, functionName, selectedIndex, move,e.Message));

            }
        }


        public void SelectedDetailsIndexChanged(DetailsDescription detailsDescription)
        {
            if (null != detailsPlayer)
            {
                detailsPlayer.SelectedDetailsIndexChanged(detailsDescription);
            }
        }

        public void ListBoxDetailsLeave()
        {
            if (null != detailsPlayer)
            {
                detailsPlayer.ListBoxDetailsLeave();
            }
        }

    }


}
