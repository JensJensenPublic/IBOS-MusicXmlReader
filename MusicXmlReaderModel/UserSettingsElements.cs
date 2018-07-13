using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; // Only needed for Test()
using System.Xml; // Only needed for Test()


namespace MusicXmlReaderModel
{
    class UserSettingsElements
    {
        //************************************************************************************************************************************
        // For test Only: bulid a tree -------------------------------------------------------------------------------------------------------
        //************************************************************************************************************************************

        public readonly int Version = 0; // The version of the current implementation
        UserSettingsElement root = null; // Represents the root of the tree representing all User Settings. This data structure is optimized for converting to and from XML.


        public  void Init(PartlistElement partList)
        {
            Logger.LogCF(string.Format(".Entry"));
            try
            {
                root = UserSettingsElement.Create(UserSettingNames.UserSettings);

                // Start with some "Better safe than sorry" versioning information for handling possible backward / forward compatibility issues !
                root.AddChild(UserSettingsElement.Create("UserSettingsElementsVersion", this.Version)); // The version of the implementation of UserSettingsElement 
                root.AddChild(UserSettingsElement.Create("UserSettingElementVersion", UserSettingsElement.Version)); // The version of the implementation of UserSettingsElement 
                root.AddChild(UserSettingsElement.Create("ApplicationVersion", "0.0.0.0")); // Get the real version from somewhere !

                // Simple user settings
                root.AddChild(UserSettingsElement.Create("UserTempoFactor", 100));


                // The Filter tree contains 3 children: Sound, Speech and MusicBraille, each containtng 2 subtrees
                UserSettingsElement soundSettings = root.AddChild(UserSettingsElement.Create(UserSettingNames.Sound, true));
                UserSettingsElement speechSettings = root.AddChild(UserSettingsElement.Create(UserSettingNames.Speech, true));
                UserSettingsElement musicBrailleSettings = root.AddChild(UserSettingsElement.Create(UserSettingNames.MusicBraille, true));
                // SoundSettings contains 2 children "DetailsForSound" and "PartsForSound"
                UserSettingsElement detailsForSound = soundSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Details, true)); // 7?
                UserSettingsElement partsForSound = soundSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Parts, true));
                // SpeechSettings contains 2 children "DetailsForSpeech" and "PartsForSpeech"
                UserSettingsElement detailsForSpeech = speechSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Details, true)); // 9?
                UserSettingsElement partsForSpeech = speechSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Parts, true));
                // MusicBrailleSettings contains 2 children "DetailsForSound" and "PartsForSound"
                UserSettingsElement detailsForMusicBraille = musicBrailleSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Details, true));
                UserSettingsElement partsForMusicBraille = musicBrailleSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Parts, true));

                // Fill in and enable all parts in each of the 3 branches:
                for (int i = 0; (i < partList.NumberOfParts()); i++)
                {
                    ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                    string name = scorePartElement.partId;
                    partsForSound.AddChild(UserSettingsElement.Create(name, true));
                    partsForSpeech.AddChild(UserSettingsElement.Create(name, true));
                    partsForMusicBraille.AddChild(UserSettingsElement.Create(name, true));
                }

                // Fill in all details:

                // Details for Sound playing
                detailsForSound.AddChild(UserSettingsElementBool.Create(UserSettingNames.Harmonies, false));

                // Details for Speech
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.MeasureNumbers, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Harmonies, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Notes, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.NoteOctaves, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.NoteTypes, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.NoteAccidentals, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Notations, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Lyrics, true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.MetaInformation, false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Divisions, false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.HarmonyCodes, false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Lyrics, false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.EndEvents, false));

                // Detains for Music Braille
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.MeasureNumbers, true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.Harmonies, true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.Notes, true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.Notations, true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.MeasureNumbers, true));

                Logger.LogCF(string.Format(".Exit"));
            }
            catch (Exception e)
            {
                Logger.LogCF(string.Format(": Exception. Message = {0}", e.Message));

            }
        }

        public void Test()
        {
            Logger.LogCF(string.Format(".Entry"));

            string xml = ToXml(root);

            string xml1 = xml.Replace("utf-16", "utf-8"); // HACK !!


            string fileName = @"c:\temp\UserSettings.xml";
            System.IO.File.WriteAllText(fileName, xml1);


            try
            {
                UserSettingsElement fromXml = UserSettingsElement.CreateFromFile(fileName);


                int nElements = 0;
                if (root.IsEqualTo(fromXml, ref nElements))
                {
                    Logger.LogCF(string.Format("Usersettings read from file {0} were equal to original usersettings. Both contain {1} elements", fileName, nElements));
                    // Unequalities are logged at a lower level
                }
            }
            catch (Exception e)
            {

                Logger.LogCF(string.Format("Exception.Message= {0}", e.Message));
            }

            Logger.LogCF(string.Format(".Exit"));

        }





        /// <summary>
        /// Concerts a UserSettingElement to XML
        /// </summary>
        /// <param name="usersettingsElement"></param>
        /// <returns></returns>
        public string ToXml(UserSettingsElement usersettingsElement)
        {

            // Code inspired by https://www.dotnetperls.com/xmltextwriter

            // Use StringWriter as backing for XmlTextWriter.
            string result = "";
            try
            {
                // StringBuilder sb = new StringBuilder();
                StringWriter str = new StringWriter();

                XmlTextWriter xml = new XmlTextWriter(str);


                //XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                //xmlWriterSettings.Encoding = System.Text.Encoding.UTF8;
                //XmlWriter xml = XmlWriter.Create(str, xmlWriterSettings);

                //xml.WriteProcessingInstruction("xml", "version='1.0'"); // Force use of default utf-8 encoding 



                // using (XmlWriter xmlWriter = XmlWriter.Create(sb))
                {
                    // Version information etc
                    xml.WriteStartDocument();

                    xml.WriteWhitespace("\r\n");

                    usersettingsElement.ToXml(xml); // Calls recursion

                    xml.WriteEndDocument();

                    // Result is a string.
                    result = str.ToString();
                    Logger.LogCF(string.Format(" : Length={0}", result.Length));
                    Logger.LogCF(string.Format(":\r\n{0}\r\n", result));

                }

            }
            catch (Exception e)
            {
                Logger.LogCF(string.Format("Exception Message={0}", e.Message));
                return null;
            }
            return result;
        }


        ///// <summary>
        ///// Converts XML to a UserSettingsElement
        ///// </summary>
        ///// <param name="xml"></param>
        ///// <returns></returns>
        //public static UserSettingsElement ReadFromFile(string fileName)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    XmlTextReader reader = new XmlTextReader(fileName);
        //    reader.WhitespaceHandling = WhitespaceHandling.None;
        //    doc.Load(reader); // This single operation may last decades of seconds on a slow platform!!



        //}

        private UserSettingsElements()
        {
        }


        public static UserSettingsElements Create()
        {
            return new UserSettingsElements();
        }



    }

}

