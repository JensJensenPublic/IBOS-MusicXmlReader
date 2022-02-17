using System;
using System.IO;
using System.Collections.Generic;
using NAudio.Midi;
using System.Xml;
using JSJ.ScreenReaderAPI;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// The Model class is the central hub in the MusicXmlReaderModel project.
    /// It connects the other classes in the MusicXmlReaderModel project.
    /// It plays the "Model" role in the MVVM (Model, Viev, ViewModel) architecture used in the MusicXmlReader solution
    /// As written in C# it is easily ported to other OS architechtures, such as iOS and Android, using the Xamarin development tool
    /// </summary>
    public class Model
    {

        static public string TheStaticXmlFileName = "";
        string className = "Model";
        string theMusicXmlFileName = "";
        string theUserSettingsFileName = "";
        string myMusicXmlDirectory = ""; // Typically "C:\Users\<user>\\Documents\IBOS Nodelæser"
        string myMusicXmlSampleDirectory = ""; //  Typically "C:\Users\<user>\\Documents\IBOS Nodelæser\Eksempler"
        string myMusicXmlDownloadDirectory = ""; //  Typically "C:\Users\<user>\\Documents\IBOS Nodelæser\Overførsler"
        bool is64Bit; // This program is compiled and for the following architechture: false:x86 true:x64 
        private List<MusicXmlObject> allMusicXmlObjecsts; // Holds all information from the .xml file
        // The following  lists contain references into allMusicXmlObjecsts where the decoded information is kept! 
        private PartDescriptionList partDescriptionList;
        private TimeDescriptionList timeDescriptionList;
        private EventDescriptionList eventDescriptionList;
        MidiOut midiOut;
        IObjectCollection objects;
        public MusicPlayer musicPlayer;
        public BrailleDisplayer brailleDisplayer;
        public TextDisplayer textDisplayer;
        public PartlistElement partList; // Contains the list of parts, describing all instruments used including their midi parameters
        int divisions = 24; // Current number of divisions of a quarternode. Just a default value
        MusicXmlInterpreter musicXmlInterpreter;
        StatusInformation currentStatusInformation; // Contains information which is valid in a part of the score, such as Key, Beats, Tempo etc.
        int numberOfParts; // Number of parts
        UserSettings userSettings;
        UserPreferences userPreferences = UserPreferences.Create();
        ScreenReaderAPI screenReaderAPI;
        DebugTools debugTools;
        ExternalToolsHandler externalToolsHandler;
        DecoderHandler decoderHandler;
        public DecoderHandler DecoderHandler { get { return decoderHandler; } }
        IDebugDisplayerClient iDebugDisplayerClient;
        ProgressWriter loaderProgressWriter = null;
        ProgressWriter conversionProgressWriter = null;
        DefaultsElement defaults; // Score-wide defaults for scaling, layout and appearance. Exactly one DefaultElement is expected per score.
        public DefaultsElement Defaults { get { return defaults; } }
        //private LoggerProxy loggerProxy;
        MetaInformation metaInformation = null; // Holds filename, title, composer, arranger etc. related to a specific score

        string executingAssembly;
        string executingDirectory;


        #region Configuration
        // The following configuration values are found in App.Config for the main .Exe program

        bool experimentalCode = false;
        public bool ExperimentalCode { get { return experimentalCode; } set { experimentalCode = value; } } // Generally available develomment feature to control Model behaviour from UI

        private bool handleGraphics = false; // Optimize for speed on slow devices by setting to "false"
        public bool HandleGraphics { get { return handleGraphics; } set { handleGraphics = value; } }

        private bool useExternal7Zip = false;
        public bool UseExternal7Zip { get { return useExternal7Zip; } set { useExternal7Zip = value; } }
        #endregion Configuration

        //private string latestBrailleFileSaveDirectory = ""; // For starting in the right directory when using "Tool"->"Interpret file as Music Braille"
        //public string LatestBrailleFileSaveDirectory { get { return latestBrailleFileSaveDirectory; }  set { latestBrailleFileSaveDirectory = value; } } 

        public string ScreenReaderName
        {
            get { return (null == screenReaderAPI) ? "" : screenReaderAPI.ScreenReaderName; }
        }



        /// <summary>
        /// Create to be used by UI-less applications
        /// </summary>
        /// <returns></returns>
        static public Model Create()
        {
            return new Model(null, null, null,null);
        }

        static public Model Create(IObjectCollection objects, IDebugDisplayerClient ws, string menuCaption,IDecoderUiClient decoderUiClient)
        {
            return new Model(objects, ws, menuCaption,decoderUiClient);
        }


        /// <summary>
        /// Quick and dirty check to reject obvious unusable Xml files
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool CheckMusicXmlSyntax(XmlDocument doc)
        {
            bool ok = true;
            //ok = ok && LogDocumentSyntaxError("For testing error handling",false); // Forces an error
            ok = ok && LogDocumentSyntaxError("Document has no child nodes", doc.HasChildNodes);
            ok = ok && LogDocumentSyntaxError("Document has too few childNodes", (doc.ChildNodes.Count >= 3));
            ok = ok && LogDocumentSyntaxError("Document is not formatted as score-partwise", IsScorePartwise(doc));
            return ok;
        }


        private bool LogDocumentSyntaxError(string text, bool ok)
        {
            if (ok) return true;
            Logger.LogCF(string.Format(":>>>>>{0}<<<<<", text));
            return false;
        }

        /// <
        /// summary>
        /// The "score-partwise" information may be found in the name of any childNode, not only node 1.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool IsScorePartwise(XmlDocument doc)
        {
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.Name.Contains("score-partwise"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calls the "Silence" method in th ScreenReaderAPI which maps to different methods depending in the
        /// current screenreader.
        /// For JAWS the native Method "JfwApiJFWStopSpeech()" is called
        /// </summary>
        public void Silence()
        {
            string functionName = "Silence";

            try
            {
                //throw new Exception("test");
                if (null != screenReaderAPI)
                {
                    screenReaderAPI.Silence();   // Stop Screenreader talking about the OpenfileDialog, we just left !!
                }
                else
                {
                    Logger.LogOnce(string.Format("{0}.{1}: screenReaderAPI=null", className, functionName));
                }
            }
            catch (Exception e)
            {
                Logger.LogOnce(string.Format("{0}.{1}: Exception.Message={2}", className, functionName, e.Message));
            }
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


        public string MxlToXml(string fullXmlFileName)
        {
            return MxlToXml(fullXmlFileName, null);
        }

        /// <summary>
        /// Pack the call to Utilities.Utilities.MxlToXml into a ProgressReporter.
        /// </summary>
        /// <param name="fullXmlFileName"></param>
        /// <returns></returns>
        public string MxlToXml(string fullXmlFileName, string destinationDirectory)
        {
            string functionName = "ConvertFromMxlToXml";
            string extension = Path.GetExtension(fullXmlFileName);
            if (".mxl" != extension)
            {
                Logger.Log(string.Format("{0}.{1} was called with unexpected file extension:{2}", className, functionName, extension));
                return "";
            }
            // This is a compressed MusicXml file in the .mxl format
            string mxlFileName = System.IO.Path.GetFileName(fullXmlFileName);
            string progressConverting = string.Format("{0} {1} {2}", ResourcesForModel.Progress_Converting, mxlFileName, ResourcesForModel.Progress_FromMxlToXml);
            conversionProgressWriter = ProgressWriter.Create(1000, iDebugDisplayerClient, progressConverting);
            fullXmlFileName = Utilities.MxlToXml(fullXmlFileName, destinationDirectory, this.useExternal7Zip ? MxlDecompressionMethod.External7ZipExe : MxlDecompressionMethod.SystemIOCompressionZipFile);
            conversionProgressWriter.Stop();
            return fullXmlFileName;
        }

        private bool UseDefaultUserSettings()
        {
            bool result = true;
            try
            {
                if (File.Exists(theUserSettingsFileName))
                {
                    File.Delete(theUserSettingsFileName);
                }
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                result = false;
            }
            Logger.LogCF(string.Format("({0}) returns {1}", theUserSettingsFileName, result));
            return result;
        }


        /// <summary>
        /// Stops on any error and returns false
        /// </summary>
        /// <param name="fullXmlFileName"></param>
        /// <returns></returns>
        public bool LoadMusicXmlFile(string fullXmlFileName, bool useDefaultSettings)
        {
            string functionName = "LoadMusicXmlFile";
            theMusicXmlFileName = fullXmlFileName;
            theUserSettingsFileName = theMusicXmlFileName + ".IBOS_MusicXmlReader";
            if (useDefaultSettings)
            {
                UseDefaultUserSettings();
            }
            bool ok = true;
            string xmlFileName = ""; // The MusicXml file currently handled 
            // ProgressWriter progressWriter = null;
            XmlTextReader reader = null;
            try
            {
                string defaultFileName = "Node.xml";
                if (string.IsNullOrEmpty(fullXmlFileName))
                {
                    // Use a default value             
                    fullXmlFileName = System.IO.Path.Combine(executingDirectory, defaultFileName);
                }

                xmlFileName = System.IO.Path.GetFileName(fullXmlFileName); // Report a filename even if an exception is thrown during conversion !
                TheStaticXmlFileName = xmlFileName; // Make the filename globally available without a reference to a Model instance.

                XmlDocument doc = new XmlDocument();
                reader = new XmlTextReader(fullXmlFileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                string progressLoading = string.Format("{0} {1}", ResourcesForModel.Progress_LoadingFile, xmlFileName);
                loaderProgressWriter = ProgressWriter.Create(1000, iDebugDisplayerClient, progressLoading);
                try
                {
                    Logger.DocLoadDelay.Start(); // Only for statistict !
                    doc.Load(reader); // This single operation may last decades of seconds on a slow platform!!
                    Logger.DocLoadDelay.Stop();
                }
                catch (Exception e)
                {
                    string s = string.Format(": XmlDocument.Load() failed. File={0} Message={1}", fullXmlFileName, e.Message);
                    Logger.LogCFOnce(s);
                    // We might add an extra stacktrace here for debuggin purposes.
                    ok = false;
                }
                loaderProgressWriter.Stop();
                ok = ok && CheckMusicXmlSyntax(doc);
                // ok = false; //For test only
                if (ok)
                {
                    //                    defaults = null; // 
                    string status = "";
                    metaInformation = MetaInformation.Create();
                    metaInformation.FileName = MetaInfoItem.Create(ResourcesForModel.MetaInfoText_FileName, Path.GetFileName(fullXmlFileName));
                    Logger.Log(string.Format("{0}.{1}: Loaded >>>>>>>>>> '{2}' <<<<<<<<<<", className, functionName, Path.GetFileName(fullXmlFileName)));
                    Logger.Log(string.Format("{0}.{1}: From   '{2}'", className, functionName, Path.GetDirectoryName(fullXmlFileName)));
                    allMusicXmlObjecsts = new List<MusicXmlObject>(); // Create the list holding all MusicXml elements read from file
                    status = string.Format("{0} {1}", ResourcesForModel.Status_Interpreting, xmlFileName);
                    WriteStatusInformation(status);
                    MidiPitchedChannelMap.Reset();
                    Logger.DocParseDelay.Start(); // Only for statistics !

                    // Interpret the document as a MusicXml document
                    musicXmlInterpreter = MusicXmlInterpreter.Create(allMusicXmlObjecsts, metaInformation, divisions);
                    musicXmlInterpreter.HandleGraphics = handleGraphics; // Decide if graphic information should be handled

                    musicXmlInterpreter.Recurse(doc.ChildNodes);   // Build  the list holding all MusicXml elements read from file

                    // Use the result from musicXmlInterpreter.Recurse for initializing some structures:  
                    defaults = musicXmlInterpreter.Defaults;
                    this.partList = musicXmlInterpreter.PartList;
                    this.numberOfParts = partList.NumberOfParts();
                    userSettings = UserSettings.Create(partList, theUserSettingsFileName);
                    userSettings.defaultStringFormat = (ScreenReaderAPI.ScreenReaderType.NVDA == screenReaderAPI.GetScreenReaderType()) ? "{1}" : "{0} {1}";

                    Logger.DocParseDelay.Stop(); // Only for statistict !
                    Logger.Log(string.Format("{0}.{1}: Parsed '{2}'", className, functionName, xmlFileName));
                    status = string.Format("{0} {1}", ResourcesForModel.Status_BuildingDataStructuresFor, xmlFileName);
                    WriteStatusInformation(status);
                    Logger.StructureInitDelay.Start(); // Only for statistics !
                    Init();  // Initialize the basic Model data structures.
                    Logger.StructureInitDelay.Stop(); // Only for statistics !
                    musicPlayer.ResetInstrumentMapping(); // Initialize the MusicPlayer data structures
                    status = string.Format("{0} {1}", xmlFileName, ResourcesForModel.Status_WasSuccessfullyLoaded);
                    WriteStatusInformation(status);
                }
                else
                {
                    string status = string.Format("{0} '{1}'.   {2}", ResourcesForModel.Status_FailedToLoad, xmlFileName, ResourcesForModel.Status_ItIsNotAValidMusicXmlFile);
                    WriteStatusInformation(status);
                    Logger.Log(string.Format("{0}.{1}: Failed to load '{2}' because it not a valid MusicXml file", className, functionName, xmlFileName));
                    theMusicXmlFileName = "";
                }
                // throw (new Exception("For test only")); // For test only
            }
            catch (System.Exception e)
            {
                // If we get here we could not load the file
                // An exception Exception with type UserHandledException has already been reported at the source. Other exceptions must be reported using a stack trace.
                Logger.LogCFOnce(string.Format(": Failed to parse '{0}' Exception.Message='{1}'", xmlFileName, e.Message)); // Log Application-specific information.

                if (e is MusicXmlParserException)
                {
                    // The original error condition has been handled and logged locally. No need to pollute the LogFile with stacktrace information
                    Logger.LogCFOnce(string.Format(": Caught rethrown MusicXmlParserException. Message={0}", e.Message));
                }
                else
                {
                    // This Exception has not been handled locally
                    Logger.LogCFE(e); // Log Exception-specific information, including a full stack trace
                }

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
            if (null != reader)
            {
                reader.Close(); // Need to close the reader in order to let other processes access the .xml file (For instance when reopening a .mxl file) !!!!
            }
            return ok;
        }


        /// <summary>
        /// Common mechanism for importing files independent of the selection method:
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="destinationPath"></param>
        private List<string> ImportFiles(List<string> fileNames, string destinationPath)
        {
            string functionName = "ImportFiles";
            // Be sure the destination path exists
            List<string> result = new List<string>();
            TryCreateDirectory(destinationPath);
            foreach (string file in fileNames)
            {
                string shortFileName = Path.GetFileName(file);
                string destFileName = Path.Combine(destinationPath, shortFileName);
                string extension = Path.GetExtension(shortFileName);
                //if (File.Exists(Path.Combine(destinationPath, shortFileName)))
                //{
                //    Logger.Log(string.Format("{0}.{1}: Skipping {2} because it has already been imported", className, functionName, file));
                //    break;
                //}
                string verb = ""; // Only used for logging
                try
                {
                    switch (extension)
                    {
                        case ".xml": // Copy the file
                            verb = "copy";
                            File.Copy(file, destFileName, false); // False <==> Do not overwrite existing
                            Logger.Log(string.Format("{0}.{1}: Copied {2}", className, functionName, file));
                            result.Add(Path.GetFileName(file));
                            break;
                        case ".mxl": // Convert from .xlm to .xml
                            verb = "convert";
                            string xmlFileName;
                            xmlFileName = MxlToXml(file, destinationPath);
                            if (!string.IsNullOrEmpty(xmlFileName))
                            {
                                Logger.Log(string.Format("{0}.{1}: Converted {2} to xml", className, functionName, file));
                                result.Add(Path.GetFileName(file));
                            }
                            else
                            {
                                Logger.Log(string.Format("{0}.{1}: Failed to convert {2} to xml", className, functionName, file));
                            }
                            break;
                        default:
                            verb = "skip";
                            Logger.Log(string.Format("{0}.{1}: Skipping import of {2} because it has an unsupported extension: {3} ", className, functionName, file, extension));
                            break;
                    } // switch
                } // try
                catch (Exception e)
                {
                    Logger.Log(string.Format("{0}.{1}: Failed to {2} {3} Exception.Message={4}", className, functionName, verb, file, e.Message));
                }
            } // foraech
            return result;
        }


        public List<string> ImportSelectedDownloads(List<string> fileNames)
        {
            // string functionName = "ImportSelectedDownloads";
            return ImportFiles(fileNames, myMusicXmlDownloadDirectory);
        }





        public List<string> SelectNewestDownloads()
        {
            string functionName = "SelectNewestDownloads";
            List<string> result = new List<string>();
            try
            {
                bool defaultUser = false;
                string downloadPath = KnownFolders.GetPath(KnownFolder.Downloads, defaultUser); // Get the path to the current user.
                string destinationPath = myMusicXmlDirectory;
                Logger.Log(string.Format("{0}.{1}: DownloadPath={2} DestinationPath={3}", className, functionName, downloadPath, destinationPath));
                string[] files = Directory.GetFiles(downloadPath);
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

                result = todaysMusicXmlFiles;

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
                    className, functionName, deviceNumber, mc.ProductName, mc.Technology, mc.ProductId, mc.Notes));
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
        private Model(IObjectCollection objects, IDebugDisplayerClient iDebugDisplayerClient, string caption,IDecoderUiClient decoderUiClient)
        {
            string methodName = "Model";

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            executingDirectory = Utilities.GetExecutingDirectory();
            executingAssembly = Utilities.GetExecutingAssembly();
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
   

            // Utilities.CheckScreenReader(screenReaderAPI.ScreenReaderName, caption); // Check for DummyScreenReader

            midiOut = CreateMidiOut(0);
            musicPlayer = new MusicPlayer(objects, midiOut);
            decoderHandler = DecoderHandler.Create(musicPlayer,decoderUiClient);
            int displaySize = 40;
            brailleDisplayer = BrailleDisplayer.Create(iDebugDisplayerClient, displaySize, screenReaderAPI); // TODO Get the real displaysize from somewhere
            textDisplayer = TextDisplayer.Create(iDebugDisplayerClient);
            Logger.Log(string.Format("Model: Assuming size of physical Braille display = {0}", displaySize));
            this.objects = objects;
            this.iDebugDisplayerClient = iDebugDisplayerClient;
            //            this.loggerProxy = new LoggerProxy(); // Used to establish a callback path from BrailleMusicDecoder and other sub-dlls

            Logger.LogSystemParameters();
            Logger.LogDebuggerAttachment();
        }



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

        public UserPreferences UserPreferences
        {
            get { return userPreferences; }
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

        public string TheUserSettingsFileName
        {
            get
            {
                return theUserSettingsFileName;
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


        // Use this methode to apply special options during test
        private NoteElementComperator.SpecialOptionsEnum GetSortOptions()
        {
            NoteElementComperator.SpecialOptionsEnum result = 0; // All defaults
            //result |= NoteElementComperator.SpecialOptionsEnum.NoSort; // Do not sort at all
            //result |= NoteElementComperator.SpecialOptionsEnum.IgnorePartNumbers; // Ignore part numbers while sorting
            //result |= NoteElementComperator.SpecialOptionsEnum.UseStaffNumbers;   // Use staff numbers when sorting
            //result |= NoteElementComperator.SpecialOptionsEnum.HighestPitchLast; //  Show highest pitch last when sorting
            //result |= NoteElementComperator.SpecialOptionsEnum.LogDifferences;   // Log differences while sorting
            return result;
        }


        public void Init()
        {
            partDescriptionList = PartDescriptionList.Create(allMusicXmlObjecsts, numberOfParts);
            timeDescriptionList = TimeDescriptionList.Create(partDescriptionList, divisions);
            currentStatusInformation = StatusInformation.Create(MusicPlayer.defaultMusicPlayerTempo);
            eventDescriptionList = EventDescriptionList.Create(timeDescriptionList, numberOfParts, userSettings, currentStatusInformation);
            eventDescriptionList.Sort(NoteElementComperator.Create(userSettings, GetSortOptions())); // NOTE: The userSettings parameter is primarily used for debugging purposes
            eventDescriptionList.InitMeasureFractions(); // NOTE! New code for handling fractions of measures !!
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
            if (null == UserSettings) return;
            UserSettings.UserTempo = tempo;
            musicPlayer.SetUserTempo(tempo); // Change the actual tempo
            currentStatusInformation.CurrentTempoModification = tempo; // Show the new tempo in the status line
        }

        public void ChangeUserTempo(int change)
        {
            if (null == UserSettings) return;
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
            this.userSettings.SetParts(UserSettings.Category.Sound, partNumber, value);
            musicPlayer.UserSettings = this.userSettings;
        }

        public void SetPartsToRead(int partNumber, bool value)
        {
            this.userSettings.SetParts(UserSettings.Category.Speech, partNumber, value);
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
                this.SaveUserSettings();
                musicPlayer.StopPlaying();
                musicPlayer.DamperThreadStop();
                musicPlayer.UiProxyThreadStop();
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
        public string InitMusicXmlFiles(string applicationName, string sampleDirName, string downloadDirName)
        {
            string sourceDirName = InitialDirectory;
            string functionName = "InitMusicXmlFiles";
            string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // C:\Users\<Username>\Documents       
            myMusicXmlDirectory = Path.Combine(documentPath, applicationName);              //   C:\Users\<Username>\Documents\IBOS Nodelæser
            myMusicXmlSampleDirectory = Path.Combine(myMusicXmlDirectory, sampleDirName);   //   C:\Users\<Username>\Documents\IBOS Nodelæser\Eksempler
            myMusicXmlDownloadDirectory = Path.Combine(myMusicXmlDirectory, downloadDirName);   //   C:\Users\<Username>\Documents\IBOS Nodelæser\Overførsler
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
                    Directory.CreateDirectory(myMusicXmlDownloadDirectory);  // C:\Users\<Username>\Documents\IBOS MusicXmlReader\Overførsler
                    Logger.Log(string.Format("{0}.{1}: Calling DirectoryCopy(Source,Dest) where", className, functionName));
                    Logger.Log(string.Format(" Source='{0}'", sourceDirName));
                    Logger.Log(string.Format(" Dest=  '{0}'", myMusicXmlSampleDirectory));
                    Utilities.DirectoryCopy(sourceDirName, myMusicXmlSampleDirectory, true, ref nFiles, ref nDirs, fileNames);  // true to copy subdirs
                    Logger.Log(string.Format("{0}.{1}: DirectoryCopy() successfully copied {2} files in {3} directories", className, functionName, nFiles, nDirs));
                    // throw (new Exception("For test only"));
                }
                catch (Exception e)
                {
                    Logger.Log(string.Format("{0}.{1}: Exception during DirectoryCopy(): Message='{2}'", className, functionName, e.Message));
                }
            }

            return myMusicXmlDirectory;
        }

        private void TryCreateDirectory(string path)
        {
            string functionName = "TryCreateDirectory";
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    Logger.Log(string.Format("{0}.{1} Failed: Exception.Message='{2}'", className, functionName, e.Message));
                }
            }
        }







        /// <summary>
        /// Imports all new sample files and directories.
        /// Assumes that member variables defining all paths have already been set up by InitMusicXmlFiles()
        /// </summary>
        public List<string> ImportNewSampleFiles(ref int nFiles, ref int nDirs)
        {
            string functionName = "ImportNewSampleFiles";
            string sourceDirName = InitialDirectory;
            TryCreateDirectory(myMusicXmlSampleDirectory); // MAy have been deleted by the user
            List<string> result = new List<string>();
            Logger.Log(string.Format("{0}.{1}: Calling DirectoryCopy(Source,Dest) where", className, functionName));
            Logger.Log(string.Format(" Source='{0}'", sourceDirName));
            Logger.Log(string.Format(" Dest=  '{0}'", myMusicXmlSampleDirectory));
            Utilities.DirectoryCopy(sourceDirName, myMusicXmlSampleDirectory, true, ref nFiles, ref nDirs, result);
            Logger.Log(string.Format("{0}.{1}: DirectoryCopy() successfully copied {2} files in {3} directories", className, functionName, nFiles, nDirs));
            return result;
        }

        private DetailsPlayer detailsPlayer = null;

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
                noDescriptions[0] = DetailsDescription.Create(ResourcesForModel.DetailsDescription_NoChordFound);
                return noDescriptions;
            }
        }

        public DetailsDescription[] GetCurrentStatusDetails(EventDescription eventDescription)
        {
            if (null != eventDescription.StatusInformation)
            {
                detailsPlayer = DetailsPlayer.Create(eventDescription.StatusInformation);
                return detailsPlayer.DetailsDescriptionArray;
            }
            else
            {
                // Return message to the user that no chord was found.
                DetailsDescription[] noDescriptions = new DetailsDescription[1];
                //noDescriptions[0] = DetailsDescription.Create(ResourcesForModel.DetailsDescription_NoStatusFound);
                noDescriptions[0] = DetailsDescription.Create(ResourcesForModel.DetailsDescription_NoChordFound);  // TODO Fix
                return noDescriptions;
            }
        }

        //public List<DetailsDescription> GetSingleNoteDetailsList(DetailsDescription currentDetails)
        //{
        //    detailsPlayer = DetailsPlayer.Create(musicPlayer);
        //    List<DetailsDescription> result = new List<DetailsDescription>();
        //    foreach (NoteElement note in currentDetails.Notes)
        //    {
        //        // listBoxDetails.Items.Add(note.ToDetailsString());
        //        result.Add(DetailsDescription.Create(note.ToDetailsString(), note)); // Hold the note itself and its string representation
        //    }
        //    return result;
        //}


        public DetailsDescription[] GetSingleNoteDetails(DetailsDescription currentDetails)
        {
            //bool showHand = true;
            detailsPlayer = DetailsPlayer.Create(musicPlayer);
            foreach (NoteElement note in (currentDetails as NoteListDetailsDescription).Notes)
            {
                string musicBraille = "";
                // Start new code for showing MusicBraille in singleNoteDetails
                BrailleBuilderForMusic bb = BrailleBuilderForMusic.Create(0);
                bb.AddNote(note, null);
                Logger.LogCF(string.Format(": {0} {1}", bb.ToBrailleString(), bb.Text));
                // Note: These lines will later be sorted before they are displayed (using DetailsPlayer.Compare), so we can not expect the same sequence in the Log!! 
                musicBraille = bb.ToBrailleString() + " ";
                // End new code                
                string leftRightHand = note.GetLeftRightString();
                string noteString = musicBraille + leftRightHand + note.ToDetailsString();
                detailsPlayer.DetailsDescriptionList.Add(DetailsDescription.Create(noteString, note)); // Hold the note itself and its string representation
            }
            return detailsPlayer.DetailsDescriptionArray; ;
        }

        public DetailsDescription[] GetCurrentEventDetails(EventDescription eventDescription, bool noteLevel)
        {

            if (null != eventDescription)
            {
                detailsPlayer = DetailsPlayer.Create(eventDescription, partList, userSettings, musicPlayer, noteLevel);
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
                detailsPlayer = DetailsPlayer.Create(partList, musicPlayer);
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


        public DetailsDescription[] GetBrailleFileDetails()
        {
            // Create a Detailsplayer designed for the purpose
            detailsPlayer = DetailsPlayer.Create();
            // Get the contents
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_ASCII, 0, 0); // Use formating for Notataker device 
            //BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode);


            // In order to emulate the "real" behaviour for generating and writing a file with MusicBraille information 
            // we go to the full proces of writing the file to the filesystem and reading it back !
            // Explicitly Write the file to the Log , NOT to the user directory !
            string testFileDirectory = Logger.LogFileDirectory;
            string testFileName = theMusicXmlFileName + brailleFileHandler.GetExtension();
            string testFileFullName = Path.Combine(testFileDirectory, Path.GetFileName(testFileName));
            List<string> formattedStrings = this.eventDescriptionList.Format(this.userSettings);
            string formattedString = BrailleUtilities.Format(formattedStrings, brailleFileHandler.CharsPerLine, brailleFileHandler.LinesPerForm);
            brailleFileHandler.WriteToFile(formattedString, testFileFullName, true);                        // Write the file to the Logger Directory
            string brailleFileAsUnicode = brailleFileHandler.ReadFromFile(testFileFullName);                //  Read the file back

            // Format the contents for the Detail window using the formatting information embedded in the file 

            string[] forms = brailleFileAsUnicode.Split((char)012); // Split into a number of forms
            // Fill in the detailsplayer with the contents
            int formNumber = 0;
            foreach (string form in forms)
            {
                if (0 != formNumber)
                {
                    // Add a FormFeed before all foems except the first one.
                    detailsPlayer.DetailsDescriptionList.Add(DetailsDescription.Create(ResourcesForModel.DetailsDescription_MusicBraille_FormFeed));
                }
                formNumber++;
                string[] lines = form.Split((char)010); // Split each form into a number of lines
                foreach (string line in lines)
                {
                    string lineAsText = Utilities.BrailleToDotNumbers(line);
                    detailsPlayer.DetailsDescriptionList.Add(DetailsDescription.Create(line + " " + lineAsText));
                }
            }
            return detailsPlayer.DetailsDescriptionArray;
        }


        /// <summary>
        /// Move selection to the start of the next or previous measure
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="move"> +1: Next Measure,  -1: Previous Measure</param>
        public void SelectMeasure(int selectedIndex, int move)
        {
            string functionName = "SelectMeasure";
            const int noMeasure = -1; // Marks that this event is not at the start of a measure
            // Logger.Log(string.Format("{0}.{1}(SelectedIndex={2},Move={3})",className,functionName, selectedIndex,move));
            try
            {
                // Find the first event containing a measure nmuber in either backwards or forwards direction
                for (int i = selectedIndex; ((i + move) >= 0) && ((i + move) < this.objects.GetNumberOfObjects());)
                {
                    i += move;
                    EventDescription nextEvent = (this.objects.GetObjectAtIndex(i) as EventDescription);
                    if (null != nextEvent) // We may want to enter a string instead of an EventDescription!
                    {
                        if (nextEvent.MeasureNumber != noMeasure)
                        {
                            // Thie event is the first event in this measure.
                            objects.SetSelectedIndex(i);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1}(SelectedIndex={2},Move={3}) failed. Message={4}", className, functionName, selectedIndex, move, e.Message));

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


        /// <summary>
        /// A Saves the User settings for the currently loaded MusicXml file.
        /// Must be called in the following situations:
        /// 1) When explicitly required by the user through a user control.
        /// 2) When loading a new MusicXml file
        /// 3) When the program is exiting
        /// </summary>
        public void SaveUserSettings()
        {
            if (null == userSettings)
            {
                Logger.LogCF(": No UserSettings found");
                return;
            }
            string xml = userSettings.ToXml();

            // Save the file in the same directory as the MUSICXML file.
            System.IO.File.WriteAllText(theUserSettingsFileName, xml);
        }

        public enum BrailleStyleEnum
        {
            Unknown,
            IBOS,       // The simple file style used by IBOS MusicXmlReader Version 3.0 which only exports the same Braille Music representation as shown in the UI
            BANA2015    // The official style specified by BANA in 2015, including Intervalnotation and InAccord representation
                        // Add more styles as needed, probably sub-formats to BANA2015 "Bar over Bar" etc
        };

        public enum BrailleDeviceEnum
        {
            Embosser,   // Danish: "Punktprinter"
            NoteTaker,  // Danish: "Notatapparat"
        }

        /// <summary>
        /// Converts the current eventdescriptionList to Unicode string representation using the current User Settings
        /// </summary>
        /// <param name="brailleFileHandler"></param>
        /// <returns></returns>
        public StaffList GetBrailleRepresentation(int charsPerLine, int linesPerForm, BrailleStyleEnum brailleStyle)
        {
            //Logger.LogCF(string.Format(": {0} Can be changed in App.Config", ExperimentalCode ? "Experimental code!" : "Version 3.0 code"));
            Logger.LogCF(string.Format("BrailleRepresentation = '{0}", brailleStyle.ToString()));
            switch (brailleStyle)
            {
                case BrailleStyleEnum.BANA2015:
                    {
                        // Generate a list of timestamped BrailleBuilders, each representing BrailleMusic for an event
                        // The timestamps can (in later implementatations be used to generate synchronized BrailleMusic representations for 2 or more parst/staffs.
                        StaffList globalStaffList = StaffList.Create(partDescriptionList, metaInformation); // This timeconsuming operation is only executed when needed
                        globalStaffList.AddMetaInformationDetails(partList); // Fill in Meta information such as Part names 

                        // Hide staffs not enabled in UserSettings !

                        foreach (Staff staff in globalStaffList.Staffs)
                        {
                            staff.Enabled = userSettings.GetParts(UserSettings.Category.MusicBraille, staff.PartNumber);
                        }

                        // Fill in with all music information
                        if (!globalStaffList.Init(this.eventDescriptionList, this.userSettings))
                        {
                            return null; // To signal failure
                        }

                        // StaffList brailleMusicRepresentation = brailleFileHandler.FormatEx(this.eventDescriptionList,globalStaffList, this.userSettings);

                        // Convert to a BrailleMusic string by ignoting the timestamps.
                        globalStaffList.Unpack(charsPerLine, linesPerForm);
                        globalStaffList.Merge(charsPerLine, linesPerForm); // For debugging and development
                        return globalStaffList;
                    }
                case BrailleStyleEnum.IBOS:
                    {
                        List<string> formattedStrings = this.eventDescriptionList.Format(this.userSettings);
                        string formattedString = BrailleUtilities.Format(formattedStrings, charsPerLine, linesPerForm);
                        StaffList result = StaffList.Create(formattedString, metaInformation, "IBOS"); // For backward compatibility with  version 3.0)
                        result.AllStaffs.AddRange(result.Staffs);
                        return result;
                    }
                default:
                    Logger.LogCF(string.Format("Unsupported BrailleRepresentation '{0}", brailleStyle.ToString()));
                    return null; // What else to do ?
            }
        }


        /// <summary>
        /// Simple local convenience method
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="brailleFileHandler"></param>
        private bool GenerateTestPattern(string directoryName, BrailleFileHandler brailleFileHandler)
        {
            bool result = false;
            try
            {
                string fileName = brailleFileHandler.GenerateTestpattern(directoryName);
                Utilities.CloneFile(fileName, "bin"); // Generate a copy of the file, but with the ".bin" extension (For inspection of binary contents)
                result = brailleFileHandler.IsValidBrailleMusic(fileName);
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }

            if (!result)
            {
                Logger.LogCF(string.Format(": Failed to generate a valid testpattern for {0}", brailleFileHandler.GetFileFormat()));
            }
            return result;
        }

        /// <summary>
        /// Generate a simple test patterns consisting of the 64 possible Braille glyphs and write it to 
        /// the directory used for Log files
        /// Finally open an explorer in that directory:
        /// </summary>
        public bool GenerateMusicBrailleTestpattern()
        {
            bool result = false;
            string directoryName = Path.Combine(Logger.LogFileDirectory, "TestPatterns");
            if (!Directory.Exists(directoryName))
            {
                Logger.LogCF(string.Format(": Created {0}", directoryName));
                Directory.CreateDirectory(directoryName);
            }

            int chars = userPreferences.CharsPerLine;
            int lines = userPreferences.LinesPerForm;

            bool b1 = GenerateTestPattern(directoryName, BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_ASCII, chars, lines));
            bool b2 = GenerateTestPattern(directoryName, BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode, chars, lines));
            bool b3 = GenerateTestPattern(directoryName, BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252, chars, lines));
            bool b4 = GenerateTestPattern(directoryName, BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode_utf8, chars, lines));
            bool b5 = GenerateTestPattern(directoryName, BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode_utf16, chars, lines));
            bool b6 = GenerateTestPattern(directoryName, BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode_utf32, chars, lines));

            // Open an explorer to inspect the log filed
            ExternalToolsHandler.OpenLogFileLocation();

            result = (b1 && b2 && b3 && b4 && b5 && b6);
            return result;
        }

        /// <summary>
        /// Returns the list of parts currently used for generation of Music Braille
        /// This is important for the first version of "Export to Braille"
        /// </summary>
        /// <returns></returns>
        private List<string> GetEnabledMusicBrailleParts()
        {
            List<string> result = new List<string>();
            if (this.userSettings.MusicAsMusicBraille)
            {
                // Generation of Music Braille is enabled. Count the number of parts enabled.
                for (int i = 0; (i < numberOfParts); i++)
                {
                    if ((userSettings.GetParts(UserSettings.Category.MusicBraille, i)))
                    {
                        // Use the name if it exists, otherwise the id))
                        string name = partList.GetPartFromNumber(i).PartName;
                        string id = partList.GetPartFromNumber(i).partId;
                        string s = string.IsNullOrEmpty(name) ? id : name;
                        result.Add(s);
                    }
                }
            }
            Logger.LogCF(string.Format("returns {0}", result.Count));
            return result;
        }

        public int NumberOfEnabledMusicBrailleParts
        {
            get { return GetEnabledMusicBrailleParts().Count; }
        }


        /// <summary>
        /// Returns a string describing the contents of the currently genereted MusicBraille file:
        /// (Upper rules take precedence)
        /// If exactly one part is enabled the name of the part is returned
        /// If all parts are enabled a localized string "tutti" is returned
        /// If some, but not all parts are enabled a localized string "multi" is returned
        /// An empty string is returned
        /// </summary>
        public string MusicBrailleFilenameAttribute
        {
            get
            {
                List<string> enabledParts = GetEnabledMusicBrailleParts();
                if (1 == enabledParts.Count) return enabledParts[0];           // Exactly one part
                if (numberOfParts == enabledParts.Count) return ResourcesForModel.MusicBrailleFilenameAttribute_Tutti;      // All parts
                if (enabledParts.Count > 0) return ResourcesForModel.MusicBrailleFilenameAttribute_Multi;                   // Some but not all
                return "";
            }
        }


        public string AnalyzeLocalization()
        {
            string baseDirectory = Path.Combine(Logger.MusicXmlReaderTempDirectory, "LocalizationAnalyzer");
            LocalizationAnalyzer.Analyzer analyzer = LocalizationAnalyzer.Analyzer.Create(baseDirectory, LocalizationAnalyzer.Analyzer.noOptions); // NoOptions: Do not use Console
            analyzer.Execute();
            return baseDirectory;
        }

    }

}


