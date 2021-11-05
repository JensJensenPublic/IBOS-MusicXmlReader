using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;
using System.Xml;
using System.IO;

namespace MusicBrailleDecoderTest
{
    /// <summary>
    /// Simple Command-line testprogram for development and test of automated conversion from Music Braille to MusicXml.
    /// Builds entirely on functionality implemented in the MusicXmlReaderModel!
    /// By making the testprogram a part of the main solution debugging and development is made much easier
    /// </summary>
    class Program
    {



        static void Main(string[] args)
        {
            string shortFileName;
            MusicBrailleTestFiles.FileSourceEnum fileSourceEnum;
            MusicBrailleTestFiles musicBrailleTestFiles = MusicBrailleTestFiles.Create();

            // Select file source file here! 
            // musicBrailleTestFiles.SelectTestFile(TestFile.UlandsVise);
            // musicBrailleTestFiles.SelectTestFile(TestFile.FourPianoBluesLeoSmith);
            // musicBrailleTestFiles.SelectTestFile(TestFile.MariaGennemTorneGår);
            musicBrailleTestFiles.SelectTestFile(MusicBrailleTestFiles.TestFileEnum.FourPianoBluesAndorFoldes,out shortFileName,out fileSourceEnum);
            Logger.Open("MusicBrailleToMusicXmlCmd.log");       
            Model model = Model.Create();
            BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.Unknown;
            DecoderOptions.RegionalOptionsEnum regionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.AutoSelect;
            string fullFileName = musicBrailleTestFiles.GetFullFileName(fileSourceEnum, shortFileName, ref fileEncoding, ref regionalOptionsEnum);
            bool developerMode = true;
            XmlDocument musicXmlDocument = null;
            bool showXmlOnConsole = false;
            DecoderOptions decoderOptions = DecoderOptions.Create(regionalOptionsEnum, developerMode);
            List<DecoderItem> interpretation = model.DecoderHandler.InterpretBrailleMusicFile(fullFileName, fileEncoding,out musicXmlDocument, decoderOptions);  
            if (showXmlOnConsole) musicXmlDocument.Save(Console.Out); // Disable to speet up
            DecoderOutputFileHandler decoderOutputFileHandler = DecoderOutputFileHandler.Create(fullFileName);

            // Write the decoded output as a text interpretation to a file
            List<string> strings = new List<string>();
            foreach (DecoderItem decoderItem in interpretation)
            {
                strings.Add(decoderItem.ToString());
            }
            decoderOutputFileHandler.SaveInterpretation(strings, decoderOutputFileHandler.FullOutputFileName);

            // Save the MusicXml file
            if (showXmlOnConsole) musicXmlDocument.Save(Console.Out); // To the console. Disable to speed up debugging !
            musicXmlDocument.Save(decoderOutputFileHandler.FullMusicXmlFileName); // To a file

            // Open the output file in NotePad
            //Utilities.RunExeWithFileArgument("NotePad", fullOutputFileName);

            // Open Explorer in the output directory.
            Utilities.RunExeWithDirArgument("Explorer", decoderOutputFileHandler.OutputDirectory);


            return;
        }
    }
}
