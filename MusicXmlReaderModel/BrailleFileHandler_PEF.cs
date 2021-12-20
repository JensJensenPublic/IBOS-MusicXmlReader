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
        XmlNode currentSectionElement; // The element where the dynamic information is placed

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

            // We still need to build the following lines, exemplified by the 390120.def sample file: 
            // < format xmlns = "http://purl.org/dc/elements/1.1/" > application / x - pef + xml </ format > 
            // < date xmlns = "http://purl.org/dc/elements/1.1/" > 2011 - 09 - 29 </ date >  
            // < title xmlns = "http://purl.org/dc/elements/1.1/" > Imudico's melodibog 28, for el-orgel, klaver og guitar med becifring og akkord-diagrammer (udeladt i punktudgaven)</title>
            // < identifier xmlns = "http://purl.org/dc/elements/1.1/" > 390120 </ identifier >

            // The pefNode contains the Head element
            XmlNode headElement = doc.CreateElement("head");
            pefNode.AppendChild(headElement);
            // The headElement contains the meta element
            XmlNode metaElement = doc.CreateElement("meta");
            headElement.AppendChild(metaElement);

            // The pefNode also contains the body element
            XmlNode bodyElement = doc.CreateElement("body");
            pefNode.AppendChild(bodyElement);

            // The body element contains the Volume Elenemt:     <volume cols="42" rows="25" duplex="true" rowgap="0">
            XmlNode volumeElement = doc.CreateElement("volume");
            volumeElement.Attributes.Append(CreateAttribute("cols","42"));    
            volumeElement.Attributes.Append(CreateAttribute("rows","25"));
            volumeElement.Attributes.Append(CreateAttribute("duplex","true"));
            volumeElement.Attributes.Append(CreateAttribute("rowgap","0"));
            bodyElement.AppendChild(volumeElement);

            // The Volume element contains the Section Element.
            // The Section Element is a member variable currentSectionEmelemt because it holds the dynamic information to be filled in by the WriteToFile method
            currentSectionElement = doc.CreateElement("section");
            volumeElement.AppendChild(currentSectionElement);

            string temp = doc.OuterXml; // For inspection during debugging

            doc.Save(@"C:\temp\temp\sample.pef");

            // No initialisation of conversion tables are needed here !
        }
        
        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            // The bodyElement is a member variable, initialized during construction
            bool result = false;

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


            //using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create))) // As UTF8 parameter, but starts without  EF BB BF Works with IbPrint(65001)
            //{
            //    try
            //    {
            //        //sw.Write(unicodeBraille);

            //        //sw.Write(doc.OuterXml);
            //        result = true;
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.LogCFE(e);
            //    }
            //}


            return result;
        }

        /// <summary>
        ///  Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public override string ReadFromFile(string fullFileName)
        {
            string result = null;
            //try
            //{
            //    result = System.IO.File.ReadAllText(fullFileName);
            //    Logger.LogCF(string.Format(": read {0} characters from {1}", result.Length, fullFileName));
            //}
            //catch (Exception e)
            //{
            //    result = null;
            //    Logger.LogCFE(e);
            //}
            return result;
        }
    }


}
