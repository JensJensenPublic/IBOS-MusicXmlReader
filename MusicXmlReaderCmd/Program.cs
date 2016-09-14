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
                    bool ok = model.LoadMusicXmlFile(file);
                    Console.WriteLine(string.Format("Model.LoadMusicXmlFile({0}) {1}", file, ok ? "succeeded" : "failed"));
                    int events = model.EventDescriptionList.Events.Count;
                    Console.WriteLine(string.Format("The file contains {0} events", events));
                }

            }
            string[] subDirs = System.IO.Directory.GetDirectories(dir);
            foreach(string subDir in subDirs)
            {
                Recurse(subDir);
            }

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
