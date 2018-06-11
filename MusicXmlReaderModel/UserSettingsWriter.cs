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
            xml.WriteStartElement(name);
            xml.WriteValue(value);
            xml.WriteEndElement();
            xml.WriteWhitespace("\r\n");
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
                    // Root.
                    xml.WriteStartDocument();
                    xml.WriteStartElement("UserSettings");

                    // Music As Speech

                    xml.WriteStartElement("MusicAsSpeech");
                    xml.WriteValue(userSettings.MusicAsSpeech);

                    //xml.WriteWhitespace("\n");

                    // Parts:

                    for (int i = 0; (i < userSettings.partsToRead.Length); i++)
                    {
                        ScorePartElement scorePartElement = userSettings.PartList.GetPartFromNumber(i);
                        //string name = "PartIndex" + i.ToString();   
                        //string name = scorePartElement.partName;
                        //string name = scorePartElement.partId; // Alternatively
                        //string name = scorePartElement.ToString();
                        //string name = scorePartElement.partName;
                        string name = scorePartElement.partId + " " + scorePartElement.partName;
                        bool value = userSettings.partsToRead[i];
                        AddElement(xml, name, value);         
                    }
                           

                    // Details:

                    int length = userSettings.readerSettingsNames.Length;
                    for (int i = 0; i < length; i++) // length !!
                    {
                        string name = userSettings.readerSettingsNames[i];
                        bool value = userSettings.readerSettingsValues[i];
                        AddElement(xml, name, value);
                    }
                    xml.WriteEndElement(); // MusicAsSpeech


                    // Music As Sound 

                    xml.WriteStartElement("MusicAsSound");
                    xml.WriteValue(userSettings.MusicAsSound);
                    xml.WriteWhitespace("\n");

                    for (int i = 0; i < userSettings.playerSettingsNames.Length; i++)
                    {
                        string name = userSettings.playerSettingsNames[i];
                        bool value = userSettings.playerSettingsValues[i];
                        xml.WriteStartElement(name);
                        xml.WriteValue(value);
                        xml.WriteWhitespace("\n");
                        xml.WriteEndElement();

                    }


                    xml.WriteEndElement(); // MusicASsSound

                    // MusicAsMusicBraille

                    xml.WriteStartElement("MusicAsMusicBraille");
                    xml.WriteValue(userSettings.MusicAsMusicBraille);
                    xml.WriteWhitespace("\n");

                    for (int i = 0; i < userSettings.musicBrailleSettingsNames.Length; i++)
                    {
                        string name = userSettings.musicBrailleSettingsNames[i];
                        bool value = userSettings.musicBrailleSettingsValues[i];
                        xml.WriteStartElement(name);
                        xml.WriteValue(value);
                        xml.WriteWhitespace("\n");
                        xml.WriteEndElement();

                    }
                    xml.WriteEndElement(); //MusicAsMusicBraille


                    xml.WriteEndElement();
                    xml.WriteEndDocument();

                    //ToXml(userSettings.MusicAsSound);

                    //xmlWriter.WriteStartDocument();//  .WriteElementString()
                    //xmlWriter.WriteStartElement("MusicAsSound");
                    //xmlWriter.WriteWhitespace("\n");
                    //xmlWriter.WriteValue(userSettings.MusicAsSound);
                    //xmlWriter.WriteEndElement();


                    //xml.userSettings.MusicAsSound;
                    ////userSettings.MusicAsSpeech;
                    ////userSettings.MusicAsMusicBraille;

                    //// Loop over Tuples.
                    //foreach (var element in array)
                    //{
                    //    // Write Employee data.
                    //    xml.WriteStartElement("Employee");

                    //    xml.WriteElementString("ID", element.Item1.ToString());
                    //    xml.WriteElementString("First", element.Item2);
                    //    xml.WriteWhitespace("\n  ");
                    //    xml.WriteElementString("Last", element.Item3);
                    //    xml.WriteElementString("Salary", element.Item4.ToString());

                    //    xml.WriteEndElement();
                    //    xml.WriteWhitespace("\n");
                    //}

                    //// End.
                    //xml.WriteEndElement();
                    //xml.WriteEndDocument();

                    //                xmlWriter.WriteEndDocument();

                    // Result is a string.
                    result = str.ToString();
                    Console.WriteLine("Length: {0}", result.Length);
                    Console.WriteLine("Result: {0}", result);
                }
            }
            catch (Exception e)
            {
                Logger.LogCF(string.Format(": Exception.Message = {0}",e.Message));
            }
            Logger.LogCF(result);
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
