using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using MusicXmlReaderModel;
using System.IO;


namespace MusicXmlReaderUI
{
    /// <summary>
    /// Simple testprogram for checking the parsing of all .xml filer in a directory tree.
    /// For each valid MusicXml file found in the tree 2 .txt files are generated:
    ///  A file containing the Music Braille representation
    ///  A file containing the Normal Text representation.
    /// Both files are generated with all UserSettings enabled. 
    /// After generating these files all logging information and statistics generated during the operation is output to a .log file
    /// </summary>
    partial class Program
    {
        static Model model;
        static string executingDirectory = ""; // Needed in the call to  Utilities.MxlToXml()
        const string failed = "**********failed**********";
        static int staffsExported = 0; // Counter of successfully exported staffs
        static int staffsOverWritten = 0; // Number ofstaffs that were overwritten because the same MusicXml file was handled more than once from different locations (This is OK)
        static int filesExported = 0;  // Counter of successfully exported files 
        static List<string> allFileNames = new List<string>();

        static bool IsAlreadyHandled(string shortFileName)
        {
            if (allFileNames.Contains(shortFileName))
            {
                string format = ": Skipping file {0}because it has been already handled once";
                Logger.LogCFOnce(string.Format(format,"")); // Quiet version
                //Logger.LogCFOnce(string.Format(format, "'" + shortFileName + "' ")); // Verbose version
                return true; // Skip this file. We have already handled it from another directory
            }
            allFileNames.Add(shortFileName);
            return false;
        }

        static void Recurse(string dir, ref int successes, ref int failures)
        {
            string[] files = System.IO.Directory.GetFiles(dir);
   
            foreach (string file in files)
            {
                string shortFileName = Path.GetFileName(file);
                Logger.CurrentMusicXmlFile = shortFileName;
                Logger.CurrentMusicXmlPath = file;
 
                string extension = System.IO.Path.GetExtension(file);
                // Logger.PostString = " in " + System.IO.Path.GetFileName(file); // Will report the file name with the error logged !
                bool ok;
                switch (extension)
                {
        
                    case ".mxl":
                        if (IsAlreadyHandled(shortFileName)) break; // No need to handle the same file twice during same test
                        Logger.ClearStatistics(); // Start counting diagnostic messages for this file
                        string xmlFileName = Utilities.MxlToXml(file, executingDirectory);
                        ok = !string.IsNullOrEmpty(xmlFileName);
                        string message = string.Format("Utilities.MxlToXml({0}) {1}", file, ok ? "succeeded" : failed);  
                        string shortMessage = string.Format("Utilities.MxlToXml {0}", ok ? "succeeded" : failed);
                        Logger.Log(message);
                        Logger.LogOnce(ok ? shortMessage : message); // Only log filename if an error occurred
                        Logger.DumpStatistics(); // Dump count of diagnostic messages for this file
                        break;

                    case ".xml":
                        if (IsAlreadyHandled(shortFileName)) break; // No need to handle the same file twice during same test
                        Logger.ClearStatistics(); // Start counting unimplemented elements and attributes for this file
                        ok = model.LoadMusicXmlFile(file,false); // Loads and parses the file
                        Console.WriteLine(string.Format("Model.LoadMusicXmlFile({0}) {1}", file, ok ? "succeeded" : failed));
                        GenerateMusicBrailleFilesForCurrentMusicXmlFile(ok, model.EventDescriptionList, file);
                        Logger.DumpStatistics(); // Dump count of unimplemented elements and attributes for this file
                        if (ok)
                        {
                            successes++;
                        }
                        else
                        {
                            failures++;
                        }
                        break;
                    default:
                        break;

                }

            }
            string[] subDirs = System.IO.Directory.GetDirectories(dir);
            foreach(string subDir in subDirs)
            {
                Recurse(subDir,ref successes, ref failures);
            }
        }
 
        /// <summary>
        /// Generates MusicBraille files for the currently loaded MusicXml files.
        /// If the MusicXml file contains more than one part, one or more MusicBraille file are generated for each part.
        /// If a part contains more than one staff, a musicXml file is generated for each staff of this part
        /// </summary>
        /// <param name="ok">The result of previously opening and parsing the file</param>
        /// <param name="events">The musical events described by the file</param>
        /// <param name="fileFullName">The name of the MusicXml file</param>
        static private void GenerateMusicBrailleFilesForCurrentMusicXmlFile(bool ok, EventDescriptionList events,string fileFullName)
        {
            if (!ok) return;
            string fileShortName = System.IO.Path.GetFileName(fileFullName);
            string directory = System.IO.Path.Combine(Logger.LogFileDirectory, "TextRepresentation");
            string musicBrailleFileName = System.IO.Path.Combine(directory, fileShortName + ".MusicBraille.txt");
            string normalTextFileName    = System.IO.Path.Combine(directory, fileShortName + ".NormalText.txt");

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }


            if (System.IO.File.Exists(musicBrailleFileName))
            {
                System.IO.File.Delete(musicBrailleFileName);    // Get rid of existing MusicBraille file
            }

            if (System.IO.File.Exists(normalTextFileName))
            {
                System.IO.File.Delete(normalTextFileName);    // Get rid of existing BlackText file
            }

            // Build the full text first instead of opening and closing the file for each chunk!
            StringBuilder fullMusicBrailleString = new StringBuilder(fileShortName + "\r\n"); // Write the name of the .xml file first   
            StringBuilder fullNormalTextString =    new StringBuilder(fileShortName + "\r\n"); // Write the name of the .xml file first   

            // Set up for generating every posible output
            model.UserSettings.SetAllPartsSettings(true);        // Select all parts 
            model.UserSettings.SetAllMusicBrailleSettings(true); // Select all Music Braille Settings (For each part selected above)
            model.UserSettings.SetAllNormalTextSettings(true);   // Select all Normal Text settings   (For each part selected above)

            //model.ExperimentalCode = true;
            Logger. BrailleMusicGenerationDelay.Start();
            GenerateBrailleMusicFiles(); // New
            Logger.BrailleMusicGenerationDelay.Stop();

#if false

            // Obsolete code now replaced by GenerateBrailleMusicFiles() !!

            // Convert the parsed file to MusicBraille and Normal text
            for (int i = 0; (i < events.Events.Count); i++)
            {
                object o = model.EventDescriptionList.Events[i];
                if (o is EventDescription)
                {
                    EventDescription eventDescription = o as EventDescription;
                    AddAsMusicBraille(eventDescription, fullMusicBrailleString);
                    AddAsNormalText(eventDescription, fullNormalTextString);
                }

            }   

            
            // Finally write the whole files at once:
            System.IO.File.AppendAllText(musicBrailleFileName, fullMusicBrailleString.ToString());
            System.IO.File.AppendAllText(normalTextFileName, fullNormalTextString.ToString());
            Console.WriteLine(string.Format("The file contains {0} events", events));
#endif
        }
        static void Main(string[] args)
        {
            Logger.Open("MusicXmlReaderCmd.log");
            string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);
#if false
            // Used for testing localisation
            System.Threading.Thread thisThread;
            thisThread = System.Threading.Thread.CurrentThread;
            thisThread.CurrentUICulture = new CultureInfo("en-US"); // Use this culture instead of the default culture for this machine.
#endif
            BrailleBuilder.verbose = false; // Reduce amount of logging
            model = Model.Create();
            Console.WriteLine(string.Format("Model.Create {0}", (model != null) ? "succeeded" : "failed"));
            if (null == model) return;

            string testFileDirName = @"C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\MusicXmlReader\bin\Debug\MusicXml samples"; // Released sample files
            string archiveDirName  = @"C:\Users\Jens\Dropbox\Root\MusicXml sample file archive"; // All sample files

            // Recurse through all directories and load all musicXml files found
            int successes = 0;
            int failures = 0;
            staffsExported = 0;
            filesExported = 0;
           

#warning ToDo Find a way to get this path from elsewhere

            //string userBaseDirectory = @"C:\Users\Jens\AppData\Local\Temp\MusicXmlReader\Unicode (UTF-8)";
            //string developerBaseDirectory = @"C:\Users\Jens\AppData\Local\Temp\MusicXmlReader\Unicode (UTF-8)\Developer";

            //CleanBaseDirectory(userBaseDirectory); // The "real" .brf files
            //CleanBaseDirectory(developerBaseDirectory); // The files containing Braille and Developer Interpretation

            Logger.CleanDirectoriesDelay.Start();
            foreach (BrailleFileHandler.FileEncoding fileEncoding in fileEncodingsToTest)
            {
                string userBaseDirectory = GetBaseDirectory(fileEncoding);
                string developerBaseDirectory = GetDeveloperBaseDirectory(fileEncoding);
                CleanBaseDirectory(userBaseDirectory); // The "real" Braille files
                CleanBaseDirectory(developerBaseDirectory); // The files containing Braille and Developer Interpretation
            }
            Logger.CleanDirectoriesDelay.Stop();

            // Logger.LogDelays(); // Use during debugging to check formatting rapidly! 

            Logger.ExecutionDelay.Start();
            Recurse(testFileDirName, ref successes,ref failures);
            Recurse(archiveDirName, ref successes, ref failures);
            Logger.ExecutionDelay.Stop();

            Logger.CheckDirectoriesDelay.Start();
            foreach (BrailleFileHandler.FileEncoding fileEncoding in fileEncodingsToTest)
            {
                string userBaseDirectory = GetBaseDirectory(fileEncoding);
                string developerBaseDirectory = GetDeveloperBaseDirectory(fileEncoding);
                CheckAgainstReference(userBaseDirectory); // The "real" Braille files
                CheckAgainstReference(developerBaseDirectory); // The files containing Braille and Developer Interpretation
            }
            Logger.CheckDirectoriesDelay.Stop();



            Logger.DumpGlobalStatistics();  // Statistics summed over all MusicXml files.
            Logger.Log(string.Format("{0} succeses, {1} failures {2} LogLines", successes, failures, Logger.NumberOfLogLines));

            Logger.LogCF(string.Format(": Exported BrailleMusic for {0} MusicXml files containing {1} staffs in {2}", filesExported, staffsExported, Logger.ExecutionDelay.ToString()));
            Logger.LogCF(string.Format(": {0} staffs were overwritten.", staffsOverWritten));

            Logger.LogDelays();

            model.ExternalToolsHandler.ReadLogFile();            // Open Notepad with the Logfile
            model.ExternalToolsHandler.OpenLogFileLocation();    // Open File Explorer in the directory holding the LogFile

       
            Console.ReadLine();
        }
    }
}
