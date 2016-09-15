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
                    DumpEvents(ok,model.EventDescriptionList);                         
                    Logger.DumpStatistics(); // Dump count of unimplemented elements and attributes for this file
                }

            }
            string[] subDirs = System.IO.Directory.GetDirectories(dir);
            foreach(string subDir in subDirs)
            {
                Recurse(subDir);
            }

        }

        static void DumpEvents(bool ok, EventDescriptionList events)
        {
            if (!ok) return;
            model.UserSettings.SetMusicBrailleSettings((int)UserSettings.MusicBrailleSettings.Harmonies, true);
            model.UserSettings.SetMusicBrailleSettings((int)UserSettings.MusicBrailleSettings.MeasureNumbers, true);
            model.UserSettings.SetMusicBrailleSettings((int)UserSettings.MusicBrailleSettings.Notations, true);
            model.UserSettings.SetMusicBrailleSettings((int)UserSettings.MusicBrailleSettings.Notes, true);
            model.UserSettings.partsToRead[0] = true;
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
                        StringBuilder sbUnicode = new StringBuilder("Unicode=");
                        foreach (byte b in brailleBytes)
                        {
                            sbRaw.Append(string.Format(" {0:X2}", b));
                            char unicodeChar = (char)(0x2800 + (int)b);
                            sbUnicode.Append(unicodeChar);
                        }
                        sbRaw.Append(" ) ");
                        sbUnicode.Append("");
                        Logger.Log(sbRaw.ToString() + sbUnicode.ToString());
                    }
                }

            }
            Console.WriteLine(string.Format("The file contains {0} events", events));
        }

        
        static void Main(string[] args)
        {
            Logger.LogFileName = "MusicXmlReaderCmd.log";
            model = Model.Create();
            Console.WriteLine(string.Format("Model.Create {0}", (model != null) ? "succeeded" : "failed"));
            if (null == model) return;

            string testFileDirName = @"C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\MusicXmlReaderUI\bin\Debug\MusicXml samples";

            // Recurse through all directories and load all musicXml files found

            Recurse(testFileDirName);

            model.ReadLogFile();            // Open Notepad with the Logfile
            model.OpenLogFileLocation();    // Open File Explorer in the directory holding the LogFile
            Console.ReadLine();
        }
    }
}
