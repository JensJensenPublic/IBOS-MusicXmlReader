using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using MusicXmlReaderModel; // Namespace not dll!

namespace MusicBrailleReader
{
    class RenameTool
    {
        private List<string> warnings = new List<string>();

        private bool CheckFile(string fullFileName)
        {
            string extension = Path.GetExtension(fullFileName);
            if (0 != String.Compare(".musicxml", extension.ToLower())) return false;
            if (!File.Exists(fullFileName)) return false;
            return true; ;
        }


        private string GetTitle(string sourceFilename)
        {
            if (!CheckFile(sourceFilename)) return "";
            if (sourceFilename.Contains("!")) return ""; // Illegal char in filename
            XmlDocument doc = new XmlDocument();
            doc.Load(sourceFilename);
            XmlNode scoreNode = doc.SelectSingleNode("score-partwise");
            XmlNode creditNode = scoreNode.SelectSingleNode("credit");
            string title = creditNode.FirstChild.InnerText;
            string legalTitle = ReplaceInvalidCharacters(title);
            //string legalTitle = title; // ReplaceInvalidCharacters(title);
            if (0 == string.Compare(legalTitle, title))
            {
                Logger.LogCF(string.Format(": Title='{0}'", legalTitle));
            }
            else
            {
                Logger.LogCF(string.Format(": Title='{0}' contains illegal characters for a filename. Using FileName='{1}'", title, legalTitle));

                warnings.Add(string.Format("{0,-70} --> {1}", Quote(title), Quote(legalTitle)));
            }
            return legalTitle;
        }

        private string Quote(string s)
        {
            return string.Format("'{0}'", s);
        }

        private string ReplaceInvalidCharacters(string s)
        {
            string result = string.Join("_", s.Split(Path.GetInvalidFileNameChars()));
            return result;
        }

        private string CopyRenameFile(string sourceFileName, string destDirName,string title, bool overWrite)
        {
            if (!CheckFile(sourceFileName)) return "";
            string extension = Path.GetExtension(sourceFileName);
            string destShortFileName = Path.GetFileNameWithoutExtension(sourceFileName) + "." + title +  extension; // Insert the title in front the extension.
            string destFileName = Path.Combine(destDirName, destShortFileName);
            File.Copy(sourceFileName, destFileName,overWrite);
            return destFileName;
        }


        public bool CopyRenameFiles(string sourceDir, string destDir)
        {
            int successes = 0;
            int errors = 0;
            string currentSourceFileName = ""; // For error reporting
            try
            {
                if (!Directory.Exists(sourceDir))
                {
                    Logger.LogCF(string.Format(": SourceDir not found {0}", sourceDir));
                    return false;
                }

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                if (!Directory.Exists(destDir))
                {
                    Logger.LogCF(string.Format(": DestDir could not be created {0}", destDir));
                    return false;
                }

                Logger.LogCF(string.Format(": Source Directory={0}", sourceDir));
                Logger.LogCF(string.Format(": Dest.  Directory={0}", destDir));

              
                foreach (string sourceFileName in Directory.GetFiles(sourceDir))
                {
                    currentSourceFileName = sourceFileName;
                    string title = "";
                    try
                    {
                        title = GetTitle(sourceFileName);
                        string destFileName = CopyRenameFile(sourceFileName, destDir, title, true); // true to overwrite existing file
                        Logger.LogCF(string.Format(": Created '{0}'", Path.GetFileName(destFileName)));
                        RemoveMovementTitle(destFileName);
                        successes++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogCF(string.Format(": Failed to extract title for {0}", sourceFileName));
                        errors++;
                    }

                }
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                Logger.LogCF(string.Format(": CurrentSourceFileName ={0}", currentSourceFileName));
                return false;
            }

            Logger.LogCF(string.Format(": Created {0} files. Failed to create {1} files",successes, errors));
            if (0 != warnings.Count)
            {
                Logger.LogCF(string.Format("{0} titles were changed to avoid illegal characters in filename:", warnings.Count));
                foreach (string warning in warnings)
                {
                    Logger.Log(warning);
                }

            }

            return true;
        }

        private bool RemoveMovementTitle(string fileName)
        {    
            string[] lines = File.ReadAllLines(fileName);
            int lineToRemove = -1;
            for (int i = 0; (i< lines.Length); i++)
            {
                if (lines[i].Contains("movement-title"))
                {
                    lineToRemove = i;
                }
            }
            if (-1 != lineToRemove)
            {
                lines[lineToRemove] = "";
                File.WriteAllLines(fileName, lines);
                return true;
            }
            return false;
        }



        private RenameTool()
        {}

        public static RenameTool Create()
        {
            return new RenameTool();
        }

    }
}
