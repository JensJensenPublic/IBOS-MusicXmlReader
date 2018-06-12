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
            xml.WriteWhitespace("\r\n");
            xml.WriteStartElement(name);
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
                string name = scorePartElement.partId + " " + scorePartElement.partName;
                bool value = values[i];
                AddElement(xml, name, value);
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


        public string ToXml(UserSettings userSettings)
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

                    // Music As Sound / "Musik afspilning"

                    xml.WriteWhitespace("\r\n");
                    xml.WriteStartElement("MusicAsSound");
                    xml.WriteValue(userSettings.MusicAsSound);
                    // Parts for Music As Speech:
                    AddParts(xml, userSettings.PartList, userSettings.partsToPlay);
                    AddDetails(xml, userSettings.playerSettingsNames, userSettings.playerSettingsValues);
                    xml.WriteEndElement();
                    
                    // Music As Speech / "Tekst visning"
                    xml.WriteWhitespace("\r\n");
                    xml.WriteStartElement("MusicAsSpeech");
                    xml.WriteValue(userSettings.MusicAsSpeech);
                    // Parts for Music As Speech:
                    AddParts(xml, userSettings.PartList, userSettings.partsToRead);
                    AddDetails(xml, userSettings.readerSettingsNames, userSettings.readerSettingsValues);
                    xml.WriteEndElement();


                    // Music As MusicBraille / "MusicBraille visning"
                    xml.WriteWhitespace("\r\n");
                    xml.WriteStartElement("MusicAsMusicBraille");
                    xml.WriteValue(userSettings.MusicAsMusicBraille);
                    // Parts for Music As Speech:
                    AddParts(xml, userSettings.PartList, userSettings.partsToBraille);
                    AddDetails(xml, userSettings.musicBrailleSettingsNames, userSettings.musicBrailleSettingsValues);
                    xml.WriteEndElement();




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
