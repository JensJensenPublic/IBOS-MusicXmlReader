using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MusicXmlReaderUI
{
    /// <summary>
    /// Simple testprogram for checking the parsing of all .xml filer in a directory tree.
    /// Generates output to the .log file
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
        /// 
        /// </summary>
        /// <param name="ok">The result of previously opening and parsing the file</param>
        /// <param name="events">The musical events described by the file</param>
        /// <param name="fileFullName">The name of the file</param>
        static private void DumpEvents(bool ok, EventDescriptionList events,string fileFullName)
        {
            if (!ok) return;
            string fileShortName = System.IO.Path.GetFileName(fileFullName);
            string musicBrailleFileName= System.IO.Path.Combine(Logger.LogFileDirectory, fileShortName + ".MusicBraille.txt");

         
            if (System.IO.File.Exists(musicBrailleFileName))
            {
                System.IO.File.Delete(musicBrailleFileName);    // Get rid of existing file 
            }

            // Build the full text first instead of opening and closing the file for each chunk!
            StringBuilder fullString = new StringBuilder(fileShortName + "\r\n"); // Write the name of the .xml file first   

            // Set up for generating MusicBraille
            model.UserSettings.SetAllMusicBrailleSettings(true);


            // Convert the parsed file to MusicBraille
            for (int i = 0; (i < events.Events.Count); i++)
            {
                object o = model.EventDescriptionList.Events[i];
                if (o is EventDescription)
                {
                    List<byte> brailleBytes = (o as EventDescription).ToBraille();
                    if (0 != brailleBytes.Count)
                    {
                        StringBuilder sbRaw = new StringBuilder("Hex=(");
                        //StringBuilder sbUnicode = new StringBuilder("Unicode=");
                        StringBuilder sbUnicode = new StringBuilder();
                        foreach (byte b in brailleBytes)
                        {
                            sbRaw.Append(string.Format(" {0:X2}", b));
                            char unicodeChar = (char)(0x2800 + (int)b);
                            sbUnicode.Append(unicodeChar);
                        }
                        sbRaw.Append(" ) ");
                        sbUnicode.Append("");
                        //string line = sbRaw.ToString() + sbUnicode.ToString(); // Show the hex representation + the Music Braille representation
                        string line = sbUnicode.ToString(); // Show the Music Braille representation only.
                        fullString.Append(line + "\r\n");
                    }
                }

            }
            // Finally write the whole file contents at once:
            System.IO.File.AppendAllText(musicBrailleFileName, fullString.ToString());
            Console.WriteLine(string.Format("The file contains {0} events", events));
        }

        
        static void Main(string[] args)
        {
            Logger.Open("MusicXmlReaderCmd.log");
            model = Model.Create();
            Console.WriteLine(string.Format("Model.Create {0}", (model != null) ? "succeeded" : "failed"));
            if (null == model) return;

            string testFileDirName = @"C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\MusicXmlReaderUI\bin\Debug\MusicXml samples";

            // Recurse through all directories and load all musicXml files found

            Recurse(testFileDirName);

            model.ReadLogFile();            // Open Notepad with the Logfile
            model.OpenLogFileLocation();    // Open File Explorer in the directory holding the LogFile
            //Console.ReadLine();
        }
    }
}
