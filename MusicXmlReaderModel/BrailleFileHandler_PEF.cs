using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace MusicXmlReaderModel
{
    class BrailleFileHandler_PEF : BrailleFileHandler
    {
        XmlDocument doc; // For building the PEF file as an XmlDocument
        XmlNode metaElement; // The element where the metainformation is placed by the "WriteToFile method"
        XmlNode currentSectionElement; // The element where the dynamic information is placed by the "WriteToFile method"

        int maxRows = 0; // Statistics only. Max number of rows for a page
        int maxCols = 0; // Statistics only. MAx number of coloumns for a line


        public override int GetCodePage()
        {
             return 65001;
        }


        /// <summary>
        /// Simple mechanism for checking BrailleMusic encoded using UTF-8
        /// Use real Windows classes such as StreamReader with Encoding parameter for the actual decoding!
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override bool IsValidBrailleMusic(string fileName)
        {
            byte[] bytesReadFromFile = ReadAsBinary(fileName);
            return BrailleFileUnicodeTester.Create().Test(bytesReadFromFile, base.controlCharacters);
        }

 

        public override string GetExtension()
        {
            return ".pef";
        }

        public override string GetFileFormat()
        {
            return "PEF";
        }

        private XmlAttribute CreateAttribute(string name, string value)
        {
            XmlAttribute attribute = doc.CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }

        private void AddPage(List<XmlNode> rows)
        {
            if (0 == rows.Count) return;
            // Create a new page and fill in with rows
            XmlNode pageElement = doc.CreateElement("page");
            foreach (XmlNode row in rows)
            {
                pageElement.AppendChild(row);
            }
            maxRows = Math.Max(maxRows, rows.Count);
            rows.Clear();
            // Append the new page to the section
            currentSectionElement.AppendChild(pageElement);
        }

        XmlNode CreateMetaChild(string name, string innerText)
        {
            const string xmlnsName = "xmlns";
            const string xmlnsValue = "http://purl.org/dc/elements/1.1/";
            XmlNode result = doc.CreateElement(name);
            result.Attributes.Append(CreateAttribute(xmlnsName, xmlnsValue));
            result.InnerText = innerText;
            return result;
        }


        internal BrailleFileHandler_PEF(int charsPerLine, int linesPerForm)
        {
            this.charsPerLine = charsPerLine;
            this.linesPerForm = linesPerForm;

            // The following code is inspired by C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\BrailleMusicDecoder\MusicXmlBuilder.cs
            doc = new XmlDocument();
            // <? xml version = "1.0" encoding = "utf-8" ?>
            string encodingName = "UTF-8";
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", encodingName, null);
            doc.AppendChild(docNode);

            // <pef version="2008-1" xmlns="http://www.daisy.org/ns/2008/pef">
            XmlNode pefNode = doc.CreateElement("pef");
            pefNode.Attributes.Append(CreateAttribute("version","2008-1"));
            pefNode.Attributes.Append(CreateAttribute("xmlns", "http://www.daisy.org/ns/2008/pef"));
            doc.AppendChild(pefNode);

            // The pefNode contains the Head element
            XmlNode headElement = doc.CreateElement("head");
            pefNode.AppendChild(headElement);
            // The headElement contains the metaElement. The metaElement will be filled in later by the "WriteToFile method" because it needs the filename.
            metaElement = doc.CreateElement("meta");  
            headElement.AppendChild(metaElement);

            // The pefNode also contains the body element
            XmlNode bodyElement = doc.CreateElement("body");
            pefNode.AppendChild(bodyElement);

            // The body element contains the Volume Elenemt:     <volume cols="42" rows="25" duplex="true" rowgap="0">
            XmlNode volumeElement = doc.CreateElement("volume");
            volumeElement.Attributes.Append(CreateAttribute("cols",charsPerLine.ToString()));    
            volumeElement.Attributes.Append(CreateAttribute("rows",linesPerForm.ToString()));
            volumeElement.Attributes.Append(CreateAttribute("duplex","true"));
            volumeElement.Attributes.Append(CreateAttribute("rowgap","0"));
            bodyElement.AppendChild(volumeElement);

            // The Volume element contains the Section Element.
            // The Section Element is a member variable currentSectionEmelemt because it holds the dynamic information to be filled in by the WriteToFile method
            currentSectionElement = doc.CreateElement("section");
            volumeElement.AppendChild(currentSectionElement);

            // No initialisation of conversion tables are needed here !
        }
        
        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            currentSectionElement.RemoveAll(); // Only keep the static part of the contents, remove all previously generated pages and rows.
            bool result = false;

            // Add childNotes to metaElement
            DateTime now = DateTime.Now;
            string date = string.Format("{0}-{1}-{2}", now.Year, now.Month, now.Day);
            metaElement.RemoveAll();
            metaElement.AppendChild(CreateMetaChild("format", "application / x - pef + xml"));
            metaElement.AppendChild(CreateMetaChild("date", date));
            metaElement.AppendChild(CreateMetaChild("title", "TITLE")); // ToDo: Find out what to put here and when and how.
            metaElement.AppendChild(CreateMetaChild("identifier", Path.GetFileNameWithoutExtension(fullFileName)));

            // Statistic counters during debugging:
            int nBraille = 0;
            int nLF = 0;
            int nCR = 0;
            int nFF = 0;
            int nOther = 0;

            StringBuilder currentRowContents = new StringBuilder(this.charsPerLine);
            List<XmlNode> currentPageContents = new List<XmlNode>(this.linesPerForm);
            XmlNode currentRowElement;
       
            // Add the contents of unicodeBraille as a number of pages, each containing a number of rows
            foreach (char c in unicodeBraille)
            {
                if ((0x2800 <= c) && (c <= 0x28ff))
                {
                    currentRowContents.Append(c);
                    nBraille++;
                }
                else
                {
                    switch ((int)c)
                    {
                        case CarriageReturn:
                            currentRowElement = doc.CreateElement("row");
                            string currentRowString = currentRowContents.ToString();
                            currentRowElement.InnerText = currentRowString;               
                            currentPageContents.Add(currentRowElement);
                            maxCols = Math.Max(maxCols, currentRowContents.Length);
                            currentRowContents.Clear();
                            nCR++;
                            break; 
                        case LineFeed: nLF++;  break;
                        case FormFeed: nFF++; AddPage(currentPageContents); break;
                        default: nOther++; Logger.LogCF(string.Format(": Unexpected character= '{0}'", c));break;
                    }
                }  
            }

            // Add the last page, even if no FormFeed is found
            AddPage(currentPageContents);

            string s = string.Format(": nBraille={0} nCR={1} nLF={2} nFF={3} nOther={4} maxCols={5} maxRows={6}", nBraille, nCR, nLF, nFF, nOther, maxCols, maxRows);
            Logger.LogCF(s);   

            try
            {
                doc.Save(fullFileName);
                result = true;
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            
            return result;
        }

        /// <summary>
        /// PRimarily for debugging
        /// </summary>
        /// <param name="doc"></param>
        private void LogMetaInformation(XmlDocument doc)
        {
            try
            {
                // Extract the interesting nodes:
                XmlNode pefNode = doc.SelectSingleNode("pef");
                XmlNode headNode = pefNode.SelectSingleNode("head");
                XmlNode metaNode = headNode.SelectSingleNode("meta");
                Logger.LogCF(string.Format(": 'head' contains:"));
                foreach (XmlNode node in metaNode.ChildNodes)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (XmlNode attribute in node.Attributes)
                    {
                        sb.Append(attribute.InnerText);
                    }
                    Logger.LogCF(string.Format(": Name='{0}' Attributes='{1}' InnerXml='{2}' ", node.Name, sb.ToString(), node.InnerXml));
                }
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }

        }





        /// <summary>
        ///  Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public override string ReadFromFile(string fullFileName)
        {
            string result = null;
            try
            {
                // Get rid of namespaces by loading the .pef file in this way.
                // Inspired by https://stackoverflow.com/questions/17161317/xml-document-selectsinglenode-returns-null/33231117
                XmlTextReader xmlReader = new XmlTextReader(fullFileName);
                xmlReader.Namespaces = false;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlReader);
                Logger.LogCF(string.Format(": Loaded '{0}' containing {1} nodes:", Path.GetFileName(fullFileName), doc.ChildNodes.Count));
                Logger.LogCF(string.Format(": doc.ChildNodes[0]='{0}'", doc.ChildNodes[0].OuterXml));

                LogMetaInformation(xmlDocument); // Primarily for debugging

                //// Extract the interesting nodes containing the musical information:
                XmlNode pefNode = xmlDocument.SelectSingleNode("pef"); 
                XmlNode bodyNode = pefNode.SelectSingleNode("body");
                XmlNode volumeNode = bodyNode.SelectSingleNode("volume");
                XmlNode sectionNode = volumeNode.SelectSingleNode("section");
                XmlNodeList pageNodes = sectionNode.SelectNodes("page");
                Logger.LogCF(string.Format(": Document contains {0} pages", pageNodes.Count));
                int pageNumber = 1; // For debugging only
                StringBuilder rawMusicBraille = new StringBuilder(); // Here we collect the MusicBraille to be sent to the MusicBraille Decoder 
                foreach (XmlNode pageNode in pageNodes)
                {
                    XmlNodeList rowNodes = pageNode.SelectNodes("row");
                    StringBuilder symbolCount = new StringBuilder(" ");                           
                    foreach (XmlNode row in rowNodes)
                    {
                        string rowAsString = row.InnerText.ToString();
                        symbolCount.Append(rowAsString.Length + " ");
                        rawMusicBraille.Append(rowAsString + "\r\n");
                    }
                    Logger.LogCF(string.Format(": Page {0,2} contains {1,2} rows with ({2,2}) symbols", pageNumber++, rowNodes.Count, symbolCount.ToString()));
                    rawMusicBraille.Append((char) FormFeed);
                }   
                result = rawMusicBraille.ToString();           
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }  
            return result;
        }
    }


}
