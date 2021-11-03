using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Simple class for generating names and directories used when testing the Decoder
    /// </summary>
    public class DecoderOutputFileHandler
    {
        string outputDirectory;
        string outputFileName;
        string musicXmlFileName;
        string fullOutputFileName;
        string fullMusicXmlFileName;
        string fullReferenceOutputFileName; // For regression test
        string fullReferenceMusicXmlFileName; // For regression test

        public string OutputDirectory { get { return outputDirectory; } }
        public string OutputFileName { get { return outputFileName; } }
        public string MusicXmlFileName { get { return musicXmlFileName; } }
        public string FullOutputFileName { get { return fullOutputFileName; } }
        public string FullMusicXmlFileName { get { return fullMusicXmlFileName; } }
        public string FullReferenceOutputFileName { get { return fullReferenceOutputFileName; } }
        public string FullReferenceMusicXmlFileName { get { return fullReferenceMusicXmlFileName; } }


        public void SaveInterpretation(List<string> interpretation, string fullOutputFileName)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(fullOutputFileName, FileMode.Create)))
            {
                foreach (string line in interpretation)
                    try
                    {
                        sw.WriteLine(line);
                    }
                    catch (Exception e)
                    {
                        Logger.LogCFE(e);
                    }
            }
        }

        private string GetDecoderOutputDirectory()
        {
            return GetDecoderOutputDirectory(false);
        }

        private string GetDecoderOutputDirectory(bool referenceDir)
        {
            // Do not place the output files under "\Temp" because such files will be deleted by the system, and we want to keep them for later reference.
            string appDataLocalDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string s = Path.Combine(appDataLocalDir, "MusicBrailleToMusicXmlCmd"); 
            if (!Directory.Exists(s)) Directory.CreateDirectory(s);
            string result = Path.Combine(s, referenceDir ? "Reference" : System.DateTime.Now.ToShortDateString());
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetDecoderOutputFileName(string inputFileName)
        {
            string newExtension = "Decode" + Path.GetExtension(inputFileName);
            string inputFileShortName = Path.GetFileName(inputFileName);
            string result = Path.ChangeExtension(inputFileShortName, newExtension);
            return result;
        }

        private string GetMusicXmlOutputFileName(string inputFileName)
        {
            string newExtension = "musicxml"; // Intensionally no Capital letters
            string inputFileShortName = Path.GetFileName(inputFileName);
            string result = Path.ChangeExtension(inputFileShortName, newExtension);
            return result;
        }


        private DecoderOutputFileHandler(string inputFileName)
        {
            outputDirectory = GetDecoderOutputDirectory();
            outputFileName = GetDecoderOutputFileName(inputFileName);
            musicXmlFileName = GetMusicXmlOutputFileName(inputFileName);
            fullOutputFileName = Path.Combine(outputDirectory, outputFileName) + ".txt";
            fullMusicXmlFileName = Path.Combine(outputDirectory, musicXmlFileName);
            fullReferenceOutputFileName = Path.Combine(GetDecoderOutputDirectory(true), outputFileName) + ".txt"; // For regression test
            fullReferenceMusicXmlFileName = Path.Combine(GetDecoderOutputDirectory(true), musicXmlFileName); // For regression test
        }
        static public DecoderOutputFileHandler Create(string inputFileName)
        {
            return new DecoderOutputFileHandler(inputFileName);
        }

    }
}

