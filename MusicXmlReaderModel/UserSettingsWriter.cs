using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class UserSettingsWriter
    {
        private void AddElement(XmlTextWriter xml,string name, bool value)
        {
            AddElementWithAttribute(xml, name, value, null, null);
        }

        private void AddElementWithAttribute(XmlTextWriter xml, string id, bool value,string attributeName, string attributeValue)
        {
            xml.WriteWhitespace("\r\n");
            xml.WriteStartElement(id);
            if (!(string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue)))
            {
                xml.WriteAttributeString(attributeName, attributeValue);
            }
            xml.WriteValue(value);
            xml.WriteEndElement();
        }

        private void AddParts(XmlTextWriter xml, PartlistElement parts, bool[] values)
        {
            int nNames = parts.NumberOfParts();
            int nValues = values.Length;
            if (nNames != nValues) Logger.LogCF(string.Format(": names.Length={0} values.Length={1}", nNames, nValues));
            for (int i = 0; (i < nNames); i++)
            {
                ScorePartElement scorePartElement = parts.GetPartFromNumber(i);
                string name = scorePartElement.partId;
                bool value = values[i];
                string attributeName = "name";
                string attributeValue = scorePartElement.partName;
                //AddElement(xml, name, value);
                AddElementWithAttribute(xml, name, value, attributeName, attributeValue);
            }
        }

        private void AddDetails(XmlTextWriter xml, string[] names, bool[] values)
        {
            int nNames  = names.Length;
            int nValues = values.Length;
            if (nNames != nValues) Logger.LogCF(string.Format(": names.Length={0} values.Length={1}", nNames, nValues));
            for (int i = 0; i < names.Length; i++) // length !!
            {
                string name = names[i];
                bool value = values[i];
                AddElement(xml, name, value);
            }
        }

        private void AddMainBranch(XmlTextWriter xml, string caption, bool value, PartlistElement partList, bool[] parts, string[] names, bool[] values)
        {
            xml.WriteWhitespace("\r\n");
            xml.WriteStartElement(caption);
            xml.WriteValue(value);
            // Parts for Music As Speech:
            AddParts(xml, partList, parts);
            AddDetails(xml, names, values);
            xml.WriteEndElement();
        }


        public string ToXml(UserSettings uS)
        {
            // Code inspired by https://www.dotnetperls.com/xmltextwriter

            // Use StringWriter as backing for XmlTextWriter.
            string result = "";
            try
            {
                // StringBuilder sb = new StringBuilder();
                StringWriter str = new StringWriter();
                XmlTextWriter xml = new XmlTextWriter(str);
                // using (XmlWriter xmlWriter = XmlWriter.Create(sb))
                {
                    // Version information etc
                    xml.WriteStartDocument();

                    // The root element containing all other elements
                    xml.WriteWhitespace("\r\n");
                    xml.WriteStartElement("UserSettings");

                    AddMainBranch(xml, "MusicAsSound", uS.MusicAsSound, uS.PartList, uS.partsToPlay, uS.playerSettingsNames, uS.playerSettingsValues);
                    AddMainBranch(xml, "MusicAsSpeech", uS.MusicAsSpeech, uS.PartList, uS.partsToRead, uS.readerSettingsNames, uS.readerSettingsValues);
                    AddMainBranch(xml, "MusicAsMusicBraille", uS.MusicAsMusicBraille, uS.PartList, uS.partsToBraille, uS.musicBrailleSettingsNames, uS.musicBrailleSettingsValues);

                    //// Music As Sound / "Musik afspilning"

                    //xml.WriteWhitespace("\r\n");
                    //xml.WriteStartElement("MusicAsSound");
                    //xml.WriteValue(uS.MusicAsSound);
                    //// Parts for Music As Speech:
                    //AddParts(xml, uS.PartList, uS.partsToPlay);
                    //AddDetails(xml, uS.playerSettingsNames, uS.playerSettingsValues);
                    //xml.WriteEndElement();
                    
                    //// Music As Speech / "Tekst visning"
                    //xml.WriteWhitespace("\r\n");
                    //xml.WriteStartElement("MusicAsSpeech");
                    //xml.WriteValue(uS.MusicAsSpeech);
                    //// Parts for Music As Speech:
                    //AddParts(xml, uS.PartList, uS.partsToRead);
                    //AddDetails(xml, uS.readerSettingsNames, uS.readerSettingsValues);
                    //xml.WriteEndElement();


                    //// Music As MusicBraille / "MusicBraille visning"
                    //xml.WriteWhitespace("\r\n");
                    //xml.WriteStartElement("MusicAsMusicBraille");
                    //xml.WriteValue(uS.MusicAsMusicBraille);
                    //// Parts for Music As Speech:
                    //AddParts(xml, uS.PartList, uS.partsToBraille);
                    //AddDetails(xml, uS.musicBrailleSettingsNames, uS.musicBrailleSettingsValues);
                    //xml.WriteEndElement();




                    xml.WriteWhitespace("\r\n");
                    xml.WriteEndElement(); // "UserSettings"
                    xml.WriteEndDocument();

                    // Result is a string.
                    result = str.ToString();
                    Console.WriteLine("Length: {0}", result.Length);
                    Console.WriteLine("Result: {0}", result);
                }

            }
            catch (Exception e)
            {
                Logger.LogCF(string.Format(": Exception.Message = {0}", e.Message));
                return "";
            }
            Logger.LogCF(string.Format(":\r\n{0}\r\n",result));
            return result;
        }
        




        private UserSettingsWriter()
        { }

        public static UserSettingsWriter Create()
        {
            return new UserSettingsWriter();
        }
    }
}
