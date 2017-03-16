using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using MusicXmlReaderModel;


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
    class Program
    {
        static Model model;

        static void Recurse(string dir)
        {
            string[] files = System.IO.Directory.GetFiles(dir);
            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file);
                if ( 0 == string.Compare(".xml",extension ))
                {
                    Logger.ClearStatistics(); // Start counting unimplemented elements and attributes for this file
                    bool ok = model.LoadMusicXmlFile(file); // Loads and parses the file
                    Console.WriteLine(string.Format("Model.LoadMusicXmlFile({0}) {1}", file, ok ? "succeeded" : "failed"));
                    DumpEvents(ok,model.EventDescriptionList,file);                         
                    Logger.DumpStatistics(); // Dump count of unimplemented elements and attributes for this file
                }

            }
            string[] subDirs = System.IO.Directory.GetDirectories(dir);
            foreach(string subDir in subDirs)
            {
                Recurse(subDir);
            }

        }

        /// <summary>
        /// Generate the Unicode Music Braille representation of an Eventdescription
        /// </summary>
        /// <param name="eventDescription">The EventDescription to convert</param>
        /// <param name="sb">The Stringbuilder to receive the result</param>
        static private void AddAsMusicBraille(EventDescription eventDescription, StringBuilder sb)
        {
            StringBuilder dummy = new StringBuilder(); // We need to put the text generated somewhere !
            BrailleBuilder bb = eventDescription.ToBraille();
            if (0 != bb.Braille.Count)
            {
                StringBuilder sbRaw = new StringBuilder("Hex=(");
                //StringBuilder sbUnicode = new StringBuilder("Unicode=");
                StringBuilder sbUnicode = new StringBuilder();
                foreach (byte b in bb.Braille)
                {
                    sbRaw.Append(string.Format(" {0:X2}", b));
                    char unicodeChar = (char)(0x2800 + (int)b);
                    sbUnicode.Append(unicodeChar);
                }
                sbRaw.Append(" ) ");
                sbUnicode.Append("");
                //string line = sbRaw.ToString() + sbUnicode.ToString(); // Show the hex representation + the Music Braille representation
                string line = sbUnicode.ToString(); // Show the Music Braille representation only.
                sb.Append(line + "\r\n");
            }
        }

        /// <summary>
        /// Generate the normal text representation of an Eventdescription
        /// </summary>
        /// <param name="eventDescription">The EventDescription to convert</param>
        /// <param name="sb">The Stringbuilder to receive the result</param>
        static private void AddAsNormalText(EventDescription eventDescription, StringBuilder sb)
        {
            sb.Append(eventDescription.ToNormalTextString() + "\r\n");
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ok">The result of previously opening and parsing the file</param>
        /// <param name="events">The musical events described by the file</param>
        /// <param name="fileFullName">The name of the file</param>
        static private void DumpEvents(bool ok, EventDescriptionList events,string fileFullName)
        {
            if (!ok) return;
            string fileShortName = System.IO.Path.GetFileName(fileFullName);
            string musicBrailleFileName = System.IO.Path.Combine(Logger.LogFileDirectory, fileShortName + ".MusicBraille.txt");
            string normalTextFileName    = System.IO.Path.Combine(Logger.LogFileDirectory, fileShortName + ".NormalText.txt");

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
        }

        
        static void Main(string[] args)
        {
            Logger.Open("MusicXmlReaderCmd.log");
#if false
            // Used for testing localisation
            System.Threading.Thread thisThread;
            thisThread = System.Threading.Thread.CurrentThread;
            thisThread.CurrentUICulture = new CultureInfo("en-US"); // Use this culture instead of the default culture for this machine.
#endif
            model = Model.Create();
            Console.WriteLine(string.Format("Model.Create {0}", (model != null) ? "succeeded" : "failed"));
            if (null == model) return;

            string testFileDirName = @"C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\MusicXmlReader\bin\Debug\MusicXml samples"; // Released sample files
            string archiveDirName  = @"C:\Users\Jens\Dropbox\Root\MusicXml sample file archive"; // All sample files

            // Recurse through all directories and load all musicXml files found

            Recurse(testFileDirName);

            Recurse(archiveDirName);


            Logger.DumpGlobalStatistics();  // Statistics summed over all MusicXml files.

            model.ExternalToolsHandler.ReadLogFile();            // Open Notepad with the Logfile
            model.ExternalToolsHandler.OpenLogFileLocation();    // Open File Explorer in the directory holding the LogFile

       
            Console.ReadLine();
        }
    }
}
