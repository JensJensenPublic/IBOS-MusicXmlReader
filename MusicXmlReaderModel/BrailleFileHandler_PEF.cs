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
        XmlNode bodyElement; // The element where the real information is placed

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
            XmlAttribute versionAttribute = doc.CreateAttribute("version");
            versionAttribute.Value = "2008-1";
            pefNode.Attributes.Append(versionAttribute);
            XmlAttribute xmlnsAttribute = doc.CreateAttribute("xmlns");
            xmlnsAttribute.Value = "http://www.daisy.org/ns/2008/pef";
            pefNode.Attributes.Append(xmlnsAttribute);
            doc.AppendChild(pefNode);

            // We still need to build the following lines, exemplified by the 390120.def sample file: 
            // < format xmlns = "http://purl.org/dc/elements/1.1/" > application / x - pef + xml </ format > 
            // < date xmlns = "http://purl.org/dc/elements/1.1/" > 2011 - 09 - 29 </ date >  
            // < title xmlns = "http://purl.org/dc/elements/1.1/" > Imudico's melodibog 28, for el-orgel, klaver og guitar med becifring og akkord-diagrammer (udeladt i punktudgaven)</title>
            // < identifier xmlns = "http://purl.org/dc/elements/1.1/" > 390120 </ identifier >

            // The Head element
            XmlNode headElement = doc.CreateElement("head");
            pefNode.AppendChild(headElement);
            XmlNode metaElement = doc.CreateElement("meta");
            headElement.AppendChild(metaElement);

            // The body element containing the dynamic information:
            bodyElement = doc.CreateElement("body");
            pefNode.AppendChild(bodyElement);

            string temp = doc.OuterXml; // For inspection during debugging

            doc.Save(@"C:\temp\temp\sample.pef");

            // No initialisation of conversion tables are needed here !
        }



        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            // The bodyElement is a member variable, initialized during construction
            bool result = false;
            
            // Build the "bodyElement" from the unicodeBraille representation 

            //
            //
            //

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
