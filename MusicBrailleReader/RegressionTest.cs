using System;
using System.Windows.Forms; // MessageBox
using System.Collections.Generic;
using System.Xml;
using System.IO; // File
using MusicXmlReaderModel; // Logger
using MusicBrailleDecoderTest;


namespace MusicBrailleReader
{
    class RegressionTest
    {
        // Exclude some substrings from the string representation
        const DecoderOptions.FormatOptionsEnum excludedDecoderOptione =
              DecoderOptions.FormatOptionsEnum.indexAndLength
            | DecoderOptions.FormatOptionsEnum.rawValues
            | DecoderOptions.FormatOptionsEnum.apostrophesInValue
            | DecoderOptions.FormatOptionsEnum.lineNumber
            | DecoderOptions.FormatOptionsEnum.rawBrailleLines
            | DecoderOptions.FormatOptionsEnum.extraString
            | DecoderOptions.FormatOptionsEnum.pageNumber
            | DecoderOptions.FormatOptionsEnum.value
            | DecoderOptions.FormatOptionsEnum.none; // No value, only for ease of adding and removing lines

        // Exclude the names of some categories (The information is fully contained in friendlyString)
        const DecoderOptions.CategoryEnum hiddenCategoryNames =
              DecoderOptions.CategoryEnum.Note
            | DecoderOptions.CategoryEnum.Articulation
//            | DecoderOptions.CategoryEnum.EmbeddedTextRepresentation
            | DecoderOptions.CategoryEnum.Hand; // No "Note" or "Hand" in front of the note

        const DecoderOptions.CategoryEnum hiddenCategories =
              DecoderOptions.CategoryEnum.Character // Exclude inputDescriptions containing single characters. Handled by AccumulatedText
            | DecoderOptions.CategoryEnum.Digit;  // Exclude inputDescriptions containing single digits.  Handled by AccumulatedText

        const bool developerMode = true;


        public string ExecuteAll()
        {
            this.iRegressionTestClient.OnNewLine("Regression test started");
            MusicBrailleTestFiles testFiles = MusicBrailleTestFiles.Create();
            string referenceDir = "";
            foreach (MusicBrailleTestFiles.TestFileEnum f in Enum.GetValues(typeof(MusicBrailleTestFiles.TestFileEnum)))
            {
                if (MusicBrailleTestFiles.TestFileEnum.Unknown == f) continue;
//                if (MusicBrailleTestFiles.TestFileEnum.FourPianoBluesLeoSmith != f) continue;
                string shortFileName;
                MusicBrailleTestFiles.FileSourceEnum sourceEnum;
                testFiles.SelectTestFile(f,out shortFileName,out sourceEnum);
                BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.Unknown;
                DecoderOptions.RegionalOptionsEnum decoderOptionsEnum = DecoderOptions.RegionalOptionsEnum.AutoSelect;
                string fullFileName = testFiles.GetFullFileName(sourceEnum, shortFileName, ref fileEncoding,ref decoderOptionsEnum);

                string directoryName = Path.GetDirectoryName(fullFileName);
                {
                    if (!Directory.Exists(directoryName))
                    {
                        Logger.Log(string.Format(": Directory not found {0}", directoryName));
                    }
                }

                if (!File.Exists(fullFileName))
                {
                    Logger.Log(string.Format(": File not found {0}",fullFileName));
                }
                DecoderOptions decoderOptions = DecoderOptions.Create(decoderOptionsEnum, developerMode);
                decoderOptions.ExcludeSubStrings(excludedDecoderOptione); // Exclude some substrings from the string representation
                decoderOptions.ExcludeCategoryNames(hiddenCategoryNames); // Exclude the names of some categories (The information is fully contained in friendlyString)
                decoderOptions.ExcludeCategories(hiddenCategories); // Exclude inputDescriptions cintaining single characters     

                XmlDocument musicXmlDocument;

                //DecoderOptions.FormatOptionsEnum decoderFormatOptions = DecoderOptions.FormatOptionsEnum.defaultOptions; // & (~excludedDecoderOptione);
                //Model.DecoderFormatOptions decoderFormatOptions = Model.DecoderFormatOptions.categories | Model.DecoderFormatOptions.indexAndLength | Model.DecoderFormatOptions.pageLinePos; 
                List<DecoderItem> interpretation = iRegressionTestClient.InterpretBrailleMusicFile(fullFileName, fileEncoding, out musicXmlDocument, decoderOptions);
                // if (showXmlOnConsole) musicXmlDocument.Save(Console.Out); // Disable to speet up
                DecoderOutputFileHandler decoderOutputFileHandler = DecoderOutputFileHandler.Create(fullFileName);

                // All results for this MusicBraille file are now available.
                // Check if a reference dir exists
                referenceDir = Path.GetDirectoryName(decoderOutputFileHandler.FullReferenceOutputFileName);
                if (!Directory.Exists(referenceDir))
                {
                    Logger.LogCF(string.Format(": Reference directory not found: {0}", referenceDir));
                    Directory.CreateDirectory(referenceDir);
                    if (Directory.Exists(referenceDir))
                    {
                        Logger.LogCF(string.Format(": Reference directory created: {0}", referenceDir));
                    }
                }

                List<string> strings = new List<string>();
                foreach (DecoderItem decoderItem in interpretation)
                {
                    string s = decoderItem.XmlToString();
                    strings.Add(decoderItem.ToString() + s);
                }

                // Check if the 2 Regression test reference files exist and create them if not
                const string messageFormat  = "Regression test {0} for {1} files from {2}";
                const string notFoundFormat = "Reference file not found, creating a new: {0}";
                string referenceDecoderFile = decoderOutputFileHandler.FullReferenceOutputFileName;
                string s1 = "";
                if (!File.Exists(referenceDecoderFile))
                {
                    s1 = string.Format(notFoundFormat, referenceDecoderFile);
                    // Write the decoded output as a text interpretation to a file
                    decoderOutputFileHandler.SaveInterpretation(strings, referenceDecoderFile);
                }
                else
                {
                    bool ok = DecoderOutputRegressionTest(strings, referenceDecoderFile);
                    s1 = string.Format(messageFormat, SucceededOrFailed(ok), "Decoder ", fullFileName);
                    if (!ok)
                    {
                        decoderOutputFileHandler.SaveInterpretation(strings, referenceDecoderFile + ".DIFFERS.txt");
                    }
                }
                Logger.LogCF(": " + s1);
                this.iRegressionTestClient.OnNewLine(s1);

                string referenceMusicXmlFile = decoderOutputFileHandler.FullReferenceMusicXmlFileName;
                string s2 = "";
                if (!File.Exists(referenceMusicXmlFile))
                {
                    s2 = string.Format(notFoundFormat, referenceMusicXmlFile);
                    musicXmlDocument.Save(referenceMusicXmlFile); // To a file
                }
                else
                {
                    bool ok = MusicXmlRegressionTest(referenceMusicXmlFile, musicXmlDocument); // To a file
                    s2 = string.Format(messageFormat, SucceededOrFailed(ok), "MusicXml",fullFileName);
                    if (!ok)
                    {
                        // Save the new contents to a file for manual comparition !
                        musicXmlDocument.Save(referenceMusicXmlFile + ".DIFFERS.musicxml");
                    }
                }
                Logger.LogCF(": " + s2);
                this.iRegressionTestClient.OnNewLine(s2);
            }
            this.iRegressionTestClient.OnNewLine("Regression test compleeted");
            this.iRegressionTestClient.OnTermination(true,referenceDir);
            return referenceDir; // Path to the reference directory
        }


        private string SucceededOrFailed(bool b)
        {
            return b ? "succeeded" : "**FAILED**"; // Same length
        }



        public bool DecoderOutputRegressionTest(List<string> newContents, string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }
            string shortFileName = Path.GetFileName(fileName); 
            string[] oldContents = File.ReadAllLines(fileName);
            string oldContentsAsString = oldContents.ToString();
            int oldLength = oldContents.Length;
            int newLength = newContents.Count;
            if (newLength != oldLength)
            {
                MessageBox.Show(string.Format("{0}\r\n Number of decoded lines differ: Reference={1} New={2}", shortFileName, oldLength, newLength));
                return false;
            }
            for (int i = 0; (i < oldLength); i++)
            {
                string sOld = oldContents[i];
                string sNew = newContents[i];
                if (0 != string.Compare(sOld, sNew))
                {
                    MessageBox.Show(string.Format("{0}\r\n Decoded lines differ at index {1} (Reference/New) :\r\n'{2}'\r\n'{3}'", shortFileName, i, sOld, sNew));
                    return false;
                }
            }
            return true;

        }


        public bool MusicXmlRegressionTest(string fileName, XmlDocument newDocument)
        {
            // Check the contents of the MusicXml file against the contents of the previous one if it exists:
            if (File.Exists(fileName))
            {
                string shortFileName = Path.GetFileName(fileName);
                // We need to temporarily save and reload the new document , otherwise the comparition may fail in some cases!
                string tempDir = Logger.MusicXmlReaderTempDirectory;
                string tempFile = Path.Combine(tempDir, "temp.MusicXml");
                newDocument.Save(tempFile);
                XmlDocument reloadedNewDocument = new XmlDocument();
                reloadedNewDocument.Load(tempFile);
                XmlDocument refDocument = new XmlDocument();
                refDocument.Load(fileName);
                string refContents = refDocument.OuterXml;
                string newContents = reloadedNewDocument.OuterXml;
                int oldLength = refContents.Length;
                int newLength = newContents.Length;
                string message; // Used for error reporting
                string messageSource = "MusicXml files";
                if (0 == (string.Compare(refContents, newContents)))
                {
                    Logger.LogCF(String.Format(": {0}  {1} are identical", shortFileName,messageSource));
                    return true;
                }
                else
                {
                    // Check different number of lines
                    string[] refLines = refContents.Split();
                    string[] newLines = newContents.Split();
                    if (refLines.Length != newLines.Length)
                    {
                        message= string.Format("{0}\r\n Number of lines differ: \r\nReference={1} New={2}", shortFileName,refLines.Length, newLines.Length);
                        return ReportError(messageSource,message);
                    }

                    // Find number of different lines
                    int nLines = refLines.Length;
                    int nDifLines = 0;
                    int firstDifLine = 0;
                    int lastDifLine = 0;
                    for (int n = 0; (n < nLines); n++)
                    {
                        if (0 != string.Compare(refLines[n], newLines[n]))
                        {
                            // Lines differ.
                            if (0 == nDifLines) firstDifLine = n; // Note the line number of the first line
                            nDifLines++;
                            lastDifLine = n;
                        }
                    }
                    if (1 == nDifLines)
                    {
                        // Only one line differs. Report the contents of that line
                        string refLine = refLines[firstDifLine];
                        string newLine = newLines[firstDifLine];
                        const string encodingDate = "encoding-date";
                        if ((refLine.Contains(encodingDate)) && (newLine.Contains(encodingDate))) return true; // The only differing line contains  the encodingdate. Ignore.
                        {
                            message = string.Format("{0}\r\n Only line number number {1} differs: \r\nReference: \r\n{2} \r\nNew: \r\n{3}", shortFileName, firstDifLine + 1, refLine, newLine);
                            return ReportError(messageSource, message);
                        }
                    }

                    // Several lines differ. Report
                    message = string.Format("{0}\r\n Several lines differ \r\nLines={1} Difs={2} in Line interval=[{3}..{4}]", shortFileName, nLines, nDifLines, firstDifLine, lastDifLine);
                    return ReportError(messageSource,message);
                }
            }
            return false;
        }

        private bool ReportError(string source, string message)
        {
            string text = string.Format("{0}:\r\n{1}", source, message);
            MessageBox.Show(text);
            string log = text.Replace("\r\n", ""); // Remove cf,lf
            Logger.LogCF(": " + log);
            return false;
        }

        IRegressionTestClient iRegressionTestClient;

        private RegressionTest(IRegressionTestClient iRegressionTestClient)
        {
            this.iRegressionTestClient = iRegressionTestClient;
        }

        public static RegressionTest Create(IRegressionTestClient iRegressionTestClient)
        {
            return new RegressionTest(iRegressionTestClient);
        }



    }
}
