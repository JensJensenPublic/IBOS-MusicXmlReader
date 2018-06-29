using System;
using System.Globalization;
using System.IO; // Only needed for Test()
using System.Xml; // Only needed for Test()


// Note for .Net mechanisms for persisting  User Settings see: https://msdn.microsoft.com/en-us/library/ms171565(v=vs.100).aspx

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Holds all user defined settings, such as the set of parts to play and read.
    /// Also allows for changing settings for development purposes.
    /// </summary>
    public class UserSettings
    {

        PartlistElement partList;
        public PartlistElement PartList { get { return partList; } }


        // All of these settings are just for exchanging simple information.
        // No need to make the coad less readable by making them private etc:
        public string defaultStringFormat = "{0} {1}";

        // Top nodes
        public bool MusicAsSound = true;
        public bool MusicAsSpeech = true;
        public bool MusicAsMusicBraille = true;



        // Arrays for controlling individual parts. NOTE: The names of the parts are defined by the current MusicXML file !
        public bool[] partsToPlay; // Play the note values from these parts
        public bool[] partsToRead; // Read the note values from these parts
        public bool[] partsToBraille; // Generate MusicBraille for these parts  

        private UserSettingsWriter userSettingsWriter = UserSettingsWriter.Create();

        // For controlling other user properties
        // readMeasureNumbers;
        // playMeasureBeats;     // Not implemented yet.
        // readHarmonies;        // After localisation  
        // playHarmonies;
        // readNotes;            // Common for all selected voices. Example: Cis
        // readNoteOctaves;      // Common for all selected voices. Example: 4
        // readNoteTypes;        // Common for all selected voices. Example: Eight
        // For controlling other DEVELOPER properties
        // readDivisions;      
        // readHarmonyCodes;     // As found in the MusicXml file
        // readEndEvents;


        //*********************************************************
        // Global Reader Settings settings (for all parts)
        //*********************************************************

        /// <summary>
        /// NOTE! For compatibility reasons: DO NOT REMOVE MEMBERS FROM THIS ENUM !!!
        /// </summary>
        public enum ReaderSettings
        {
            MeasureNumbers = 0,
            Harmonies = 1,
            Notes = 2,
            NoteOctaves = 3,
            NoteTypes = 4,
            NoteAccidentals = 5,
            Notations = 6,
            Lyrics = 7,
            MetaInformation = 8,
            Divisions = 9,
            HarmonyCodes = 10,
            EndEvents = 11,
            NumberOfReaderSettings = 12
        };


        public readonly UserSetting[] readerSettings = new UserSetting[(int)ReaderSettings.NumberOfReaderSettings];
        private void InitReaderSetting(ReaderSettings setting, string name, bool value)
        {
            readerSettings[(int)setting] = UserSetting.Create(name, value);
        }

        private void InitReaderSettings()
        {
            InitReaderSetting(ReaderSettings.MeasureNumbers, ResourcesForModel.UserSettings_ReaderNames_MeasureNumbers, true);  // "TaktNumre",
            InitReaderSetting(ReaderSettings.Harmonies, ResourcesForModel.UserSettings_ReaderNames_Harmonies, true);       // "Harmonier",
            InitReaderSetting(ReaderSettings.Notes, ResourcesForModel.UserSettings_ReaderNames_Notes, true);           // "Noder",
            InitReaderSetting(ReaderSettings.NoteOctaves, ResourcesForModel.UserSettings_ReaderNames_Octaves, true);         // "Oktaver",
            InitReaderSetting(ReaderSettings.NoteTypes, ResourcesForModel.UserSettings_ReaderNames_NoteValues, true);      // "NodeVærdier",
            InitReaderSetting(ReaderSettings.NoteAccidentals, ResourcesForModel.UserSettings_ReaderNames_Accidentals, false);    // "Læse fortegn"
            InitReaderSetting(ReaderSettings.Notations, ResourcesForModel.UserSettings_ReaderNames_Notations, true);       // "Notationer",
            InitReaderSetting(ReaderSettings.Lyrics, ResourcesForModel.UserSettings_ReaderNames_Lyrics, true);          // "Tekst"
            InitReaderSetting(ReaderSettings.MetaInformation, ResourcesForModel.UserSettings_ReaderNames_Metainformation, true); // "Meta-information",
            InitReaderSetting(ReaderSettings.Divisions, ResourcesForModel.UserSettings_ReaderNames_Divisions, false);      // "Divisions",
            InitReaderSetting(ReaderSettings.HarmonyCodes, ResourcesForModel.UserSettings_ReaderNames_HarmonyCodes, false);   // "HarmoniCodes",
            InitReaderSetting(ReaderSettings.EndEvents, ResourcesForModel.UserSettings_ReaderNames_EndEvents, false);      // "EndEvents"
        }

        public bool GetReaderSettings(ReaderSettings i)
        {
            return readerSettings[(int)i].Value;
        }
        public void SetReaderSettings(int i, bool b)
        {
            readerSettings[(int)i].Value = b;
        }

        //*****************************************************************************************
        // Global Player Settings (for all parts)
        //*****************************************************************************************


        /// <summary>
        /// NOTE! For compatibility reasons: DO NOT REMOVE MEMBERS FROM THIS ENUM !!!
        /// </summary>
        public enum PlayerSettings { Harmonies = 0, NumberOfPlayerSettings = 1 }

        public readonly UserSetting[] playerSettings = new UserSetting[(int)PlayerSettings.NumberOfPlayerSettings];

        private void InitPlayerSetting(PlayerSettings setting, string name, bool value)
        {
            playerSettings[(int)setting] = UserSetting.Create(name, value);
        }

        private void InitPlayerSettings()
        {
#warning ToDo Implement and re-enable PlayerSettings.MeasureBeats
            // InitPlayerSetting(PlayerSettings.MeasureBeats, ResourcesForModel.UserSettings_PlayerNames_Beats, false);  // "Taktslag", 
            InitPlayerSetting(PlayerSettings.Harmonies, ResourcesForModel.UserSettings_PlayerNames_Harmonies, true);  // "Harmonier",
        }

        public bool GetPlayerSettings(PlayerSettings i)
        {
            return playerSettings[(int)i].Value;
        }
        public void SetPlayerSettings(int i, bool b)
        {
            playerSettings[(int)i].Value = b;
        }

        //*****************************************************************************************
        // Global Music Braille Settings settings (for all parts)
        //*****************************************************************************************


        /// <summary>
        ///  NOTE! For compatibility reasons: DO NOT REMOVE MEMBERS FROM THIS ENUM !!!
        /// </summary>
        public enum MusicBrailleSettings { MeasureNumbers = 0, Harmonies = 1, Notes = 2, Notations = 3, NumberOfMusicBrailleSettings = 4 };

        public UserSetting[] musicBrailleSettings = new UserSetting[(int)MusicBrailleSettings.NumberOfMusicBrailleSettings];
        private void InitMusicBrailleSetting(MusicBrailleSettings setting, string name, bool value)
        {
            musicBrailleSettings[(int)setting] = UserSetting.Create(name, value);
        }

        private void InitMusicBrailleSettings()
        {
            InitMusicBrailleSetting(MusicBrailleSettings.MeasureNumbers, ResourcesForModel.UserSettings_BrailleNames_MeasureNumbers, false);//"TaktNumre",
            InitMusicBrailleSetting(MusicBrailleSettings.Harmonies, ResourcesForModel.UserSettings_BrailleNames_Harmonies, false); //"Harmonier",
            InitMusicBrailleSetting(MusicBrailleSettings.Notes, ResourcesForModel.UserSettings_BrailleNames_Notes, true);  //"Noder",
            InitMusicBrailleSetting(MusicBrailleSettings.Notations, ResourcesForModel.UserSettings_BrailleNames_Notations, true);   //"Notationer"
        }

        public bool GetMusicBrailleSettings(MusicBrailleSettings i)
        {
            return musicBrailleSettings[(int)i].Value;
        }
        public void SetMusicBrailleSettings(int i, bool b)
        {
            musicBrailleSettings[(int)i].Value = b;
        }

        //*****************************************************************************************
        // Non-boolean user settings
        //*****************************************************************************************  

        // Tempo settings
        private int userTempo = 100; // Percentage of tempo indicated in score
        private int minUserTempo = 1;
        private int maxUserTempo = 1000;
        public int UserTempo
        {
            get
            {
                return userTempo;
            }

            set
            {
                if (value < minUserTempo)
                {
                    value = minUserTempo;
                }
                if (value > maxUserTempo)
                {
                    value = maxUserTempo;
                }
                userTempo = value;
            }
        }


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private UserSettings()
        {
        }


        /// <summary>
        /// Enable or disable all parts
        /// Primarily used for test
        /// </summary>
        /// <param name="value"></param>
        public void SetAllPartsSettings(bool value)
        {
            // Generate Text for all parts 
            for (int i = 0; (i < partsToRead.Length); i++)
            {
                partsToRead[i] = value;
            }

            // Generate Music Braille for all parts
            for (int i = 0; (i < partsToBraille.Length); i++)
            {
                partsToBraille[i] = value;
            }

        }

        /// <summary>
        /// Enable or disable all settings related to MusicBraille
        /// Primarily used for test
        /// </summary>
        public void SetAllMusicBrailleSettings(bool value)
        {
            // First handle all MusicBraille specific settings
            for (int i = 0; (i < musicBrailleSettings.Length); i++)
            {
                SetMusicBrailleSettings(i, value);
            }


        }

        public void SetAllNormalTextSettings(bool value)
        {
            // First handle all Normal text specific settings
            for (int i = 0; (i < readerSettings.Length); i++)
            {
                SetReaderSettings(i, value);
            }
        }


        /// <summary>
        /// Private constructor, used by the Create() method
        /// </summary>
        /// <param name="node"></param>
        private UserSettings(PartlistElement partList)
        {    
            this.partList = partList;
            int numberOfParts = partList.NumberOfParts();
            partsToPlay = new bool[numberOfParts];       // Must be done here because numberUfParts is not a constant.
            partsToRead = new bool[numberOfParts];       // Must be done here because numberUfParts is not a constant.
            partsToBraille = new bool[numberOfParts];    // Must be done here because numberUfParts is not a constant.
            InitReaderSettings();
            InitPlayerSettings();
            InitMusicBrailleSettings();

            userTempo = 100; // Percentage of tempo indicated in score
            if (((int)ReaderSettings.NumberOfReaderSettings != readerSettings.Length)
            || ((int)PlayerSettings.NumberOfPlayerSettings != playerSettings.Length)
            || ((int)MusicBrailleSettings.NumberOfMusicBrailleSettings != musicBrailleSettings.Length))

            {
                throw (new Exception("UserSettings: Wrong size of arrays"));
            }

            //string test = userSettingsWriter.ToXml(this); // Used for initial test only !!

            Test(partList);
        }

        public static UserSettings Create(PartlistElement partList)
        {
            return new UserSettings(partList);
        }

        //************************************************************************************************************************************
        // For test Only: bulid a tree -------------------------------------------------------------------------------------------------------
        //************************************************************************************************************************************

        public void Test(PartlistElement partList)
        {
            Logger.LogCF(string.Format(".Entry"));
            try
            {
                UserSettingsElement userSettings = UserSettingsElement.Create(UserSettingNames.UserSettings);

                userSettings.AddChild(UserSettingsElement.Create("UserTempoFactor", 100));

                // usesSettings contains 3 children: Sound, Speech and MusicBraille, each containtng 2 subtrees
                UserSettingsElement soundSettings = userSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Sound, true));
                UserSettingsElement speechSettings = userSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Speech, true));
                UserSettingsElement musicBrailleSettings = userSettings.AddChild(UserSettingsElement.Create(UserSettingNames.MusicBraille, true));
                // SoundSettings contains 2 children "DetailsForSound" and "PartsForSound"
                UserSettingsElement detailsForSound = soundSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Details, true)); // 7?
                UserSettingsElement partsForSound = soundSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Parts, true));
                // SpeechSettings contains 2 children "DetailsForSpeech" and "PartsForSpeech"
                UserSettingsElement detailsForSpeech = speechSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Details,  true)); // 9?
                UserSettingsElement partsForSpeech = speechSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Parts,  true));
                // MusicBrailleSettings contains 2 children "DetailsForSound" and "PartsForSound"
                UserSettingsElement detailsForMusicBraille = musicBrailleSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Details, true));
                UserSettingsElement partsForMusicBraille = musicBrailleSettings.AddChild(UserSettingsElement.Create(UserSettingNames.Parts,  true));

                // Fill in and enable all parts in each of the 3 branches:
                for (int i = 0; (i < partList.NumberOfParts()); i++)
                {
                    ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                    string name = scorePartElement.partId;
                    partsForSound.AddChild(UserSettingsElement.Create(name,  true));
                    partsForSpeech.AddChild(UserSettingsElement.Create(name,   true));
                    partsForMusicBraille.AddChild(UserSettingsElement.Create(name,  true));
                }

                // Fill in all details:

                // Details for Sound playing
                detailsForSound.AddChild(UserSettingsElementBool.Create(UserSettingNames.Harmonies,   false));

                // Details for Speech
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.MeasureNumbers,   true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Harmonies,  true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Notes,   true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.NoteOctaves,   true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.NoteTypes,   true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.NoteAccidentals,  true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Notations,   true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Lyrics,   true));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.MetaInformation,   false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Divisions,   false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.HarmonyCodes,   false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.Lyrics, false));
                detailsForSpeech.AddChild(UserSettingsElement.Create(UserSettingNames.EndEvents,   false));

                // Detains for Music Braille
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.MeasureNumbers,   true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.Harmonies,  true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.Notes,   true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.Notations,  true));
                detailsForMusicBraille.AddChild(UserSettingsElement.Create(UserSettingNames.MeasureNumbers,   true));


                string xml = UserSettings.ToXml(userSettings);

                string xml1 = xml.Replace("utf-16", "utf-8"); // HACK !!


                string fileName = @"c:\temp\UserSettings.xml";
                System.IO.File.WriteAllText(fileName, xml1);


                try
                {
                    UserSettingsElement fromXml = UserSettingsElement.CreateFromFile(fileName);


                    int nElements = 0;
                    if (userSettings.IsEqualTo(fromXml,ref nElements))
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
            catch (Exception e)
            {
                Logger.LogCF(string.Format(": Exception. Message = {0}", e.Message));
            }
        }


        /// <summary>
        /// Concerts a UserSettingElement to XML
        /// </summary>
        /// <param name="usersettingsElement"></param>
        /// <returns></returns>
        public static string ToXml(UserSettingsElement usersettingsElement)
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





    }

}
