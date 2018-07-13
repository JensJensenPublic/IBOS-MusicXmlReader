using System;
using System.Globalization;


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
        private UserSettingsElementBool[] partsToPlay; // Play the note values from these parts
        //private bool[] partsToRead; // Read the note values from these parts
        private UserSettingsElementBool[] partsToRead; // Read the note values from these parts
        private UserSettingsElementBool[] partsToBraille; // Generate MusicBraille for these parts

        public bool GetPartsToPlay(int index) { return partsToPlay[index].Value; }
        public void SetPartsToPlay(int index, bool b) { partsToPlay[index].Value = b; }
        public bool GetPartsToRead(int index) { return partsToRead[index].Value; }
        public void SetPartsToRead(int index, bool b) { partsToRead[index].Value = b; }
        public bool GetPartsToBraille(int index) { return partsToBraille[index].Value; }
        public void SetPartsToBraille(int index, bool b) { partsToBraille[index].Value = b; }

//        private UserSettingsWriter userSettingsWriter = UserSettingsWriter.Create();

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
        public enum ReaderSettingsEnum
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


        private readonly UserSetting[] readerSettings = new UserSetting[(int)ReaderSettingsEnum.NumberOfReaderSettings];
        public UserSetting[] ReaderSettings { get { return readerSettings; } }
        //private void InitReaderSetting(ReaderSettingsEnum setting, string name, bool value)
        //{
        //    readerSettings[(int)setting] = UserSetting.Create(name, value);
        //}

        private void InitReaderSetting(ReaderSettingsEnum setting, string name, UserSettingsElement userSettingsElement)
        {
            readerSettings[(int)setting] = UserSetting.Create(name, (userSettingsElement as UserSettingsElementBool));
        }

        private void InitReaderDetailsSettings()
        {
            UserSettingsElement readerSettingsRoot = userSettingsElements.GetUserSettingsElement(UserSettingNames.Speech);
            UserSettingsElementBool use = readerSettingsRoot.GetNamedElementBool(UserSettingNames.Details);
            InitReaderSetting(ReaderSettingsEnum.MeasureNumbers, ResourcesForModel.UserSettings_ReaderNames_MeasureNumbers, use.GetNamedElement(UserSettingNames.MeasureNumbers));  // "TaktNumre",
            InitReaderSetting(ReaderSettingsEnum.Harmonies, ResourcesForModel.UserSettings_ReaderNames_Harmonies, use.GetNamedElement(UserSettingNames.Harmonies));       // "Harmonier",
            InitReaderSetting(ReaderSettingsEnum.Notes, ResourcesForModel.UserSettings_ReaderNames_Notes, use.GetNamedElement(UserSettingNames.Notes));           // "Noder",
            InitReaderSetting(ReaderSettingsEnum.NoteOctaves, ResourcesForModel.UserSettings_ReaderNames_Octaves, use.GetNamedElement(UserSettingNames.NoteOctaves));         // "Oktaver",
            InitReaderSetting(ReaderSettingsEnum.NoteTypes, ResourcesForModel.UserSettings_ReaderNames_NoteValues, use.GetNamedElement(UserSettingNames.NoteTypes));      // "NodeVærdier",
            InitReaderSetting(ReaderSettingsEnum.NoteAccidentals, ResourcesForModel.UserSettings_ReaderNames_Accidentals, use.GetNamedElement(UserSettingNames.NoteAccidentals));    // "Læse fortegn"
            InitReaderSetting(ReaderSettingsEnum.Notations, ResourcesForModel.UserSettings_ReaderNames_Notations, use.GetNamedElement(UserSettingNames.Notations));       // "Notationer",
            InitReaderSetting(ReaderSettingsEnum.Lyrics, ResourcesForModel.UserSettings_ReaderNames_Lyrics, use.GetNamedElement(UserSettingNames.Lyrics));          // "Tekst"
            InitReaderSetting(ReaderSettingsEnum.MetaInformation, ResourcesForModel.UserSettings_ReaderNames_Metainformation, use.GetNamedElement(UserSettingNames.MetaInformation)); // "Meta-information",
            InitReaderSetting(ReaderSettingsEnum.Divisions, ResourcesForModel.UserSettings_ReaderNames_Divisions, use.GetNamedElement(UserSettingNames.Divisions));      // "Divisions",
            InitReaderSetting(ReaderSettingsEnum.HarmonyCodes, ResourcesForModel.UserSettings_ReaderNames_HarmonyCodes, use.GetNamedElement(UserSettingNames.HarmonyCodes));   // "HarmoniCodes",
            InitReaderSetting(ReaderSettingsEnum.EndEvents, ResourcesForModel.UserSettings_ReaderNames_EndEvents, use.GetNamedElement(UserSettingNames.EndEvents));      // "EndEvents"
        }

        public bool GetReaderSettings(ReaderSettingsEnum i)
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
        public enum PlayerSettingsEnum { Harmonies = 0, NumberOfPlayerSettings = 1 }

        private readonly UserSetting[] playerSettings = new UserSetting[(int)PlayerSettingsEnum.NumberOfPlayerSettings];
        public UserSetting[] PlayerSettings { get { return playerSettings; } }

        //private void InitPlayerSetting(PlayerSettingsEnum setting, string name, bool value)
        //{
        //    playerSettings[(int)setting] = UserSetting.Create(name, value);
        //}

        private void InitPlayerSetting(PlayerSettingsEnum setting, string name, UserSettingsElement UserSettingsElement)
        {
            playerSettings[(int)setting] = UserSetting.Create(name, (UserSettingsElement as UserSettingsElementBool));
        }

        private void InitPlayerDetailsSettings()
        {
#warning ToDo Implement and re-enable PlayerSettings.MeasureBeats

            UserSettingsElement playerSettingsRoot = userSettingsElements.GetUserSettingsElement(UserSettingNames.Sound);
            UserSettingsElementBool use = playerSettingsRoot.GetNamedElementBool(UserSettingNames.Details);
            // InitPlayerSetting(PlayerSettings.MeasureBeats, ResourcesForModel.UserSettings_PlayerNames_Beats, false);  // "Taktslag", 
            InitPlayerSetting(PlayerSettingsEnum.Harmonies, ResourcesForModel.UserSettings_PlayerNames_Harmonies, use.GetNamedElementBool(UserSettingNames.Harmonies));  // "Harmonier",
        }

        public bool GetPlayerSettings(PlayerSettingsEnum i)
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
        public enum MusicBrailleSettingsEnum { MeasureNumbers = 0, Harmonies = 1, Notes = 2, Notations = 3, NumberOfMusicBrailleSettings = 4 };

        private UserSetting[] musicBrailleSettings = new UserSetting[(int)MusicBrailleSettingsEnum.NumberOfMusicBrailleSettings];
        public UserSetting[] MusicBrailleSettings { get { return musicBrailleSettings; } }

        //private void InitMusicBrailleSetting(MusicBrailleSettingsEnum setting, string name, bool value)
        //{
        //    musicBrailleSettings[(int)setting] = UserSetting.Create(name, value);
        //}

        private void InitMusicBrailleSetting(MusicBrailleSettingsEnum setting, string name, UserSettingsElementBool userSettingsElement)
        {
            musicBrailleSettings[(int)setting] = UserSetting.Create(name, userSettingsElement);
        }

        private void InitMusicBrailleDetailsSettings()
        {
            UserSettingsElement musicBrailleSettingsRoot = userSettingsElements.GetUserSettingsElement(UserSettingNames.MusicBraille);
            UserSettingsElementBool use = musicBrailleSettingsRoot.GetNamedElementBool(UserSettingNames.Details);
            InitMusicBrailleSetting(MusicBrailleSettingsEnum.MeasureNumbers, ResourcesForModel.UserSettings_BrailleNames_MeasureNumbers, use.GetNamedElementBool(UserSettingNames.MeasureNumbers));//"TaktNumre",
            InitMusicBrailleSetting(MusicBrailleSettingsEnum.Harmonies, ResourcesForModel.UserSettings_BrailleNames_Harmonies, use.GetNamedElementBool(UserSettingNames.Harmonies)); //"Harmonier",
            InitMusicBrailleSetting(MusicBrailleSettingsEnum.Notes, ResourcesForModel.UserSettings_BrailleNames_Notes, use.GetNamedElementBool(UserSettingNames.Notes));  //"Noder",
            InitMusicBrailleSetting(MusicBrailleSettingsEnum.Notations, ResourcesForModel.UserSettings_BrailleNames_Notations, use.GetNamedElementBool(UserSettingNames.Notations));   //"Notationer"
        }

        public bool GetMusicBrailleSettings(MusicBrailleSettingsEnum i)
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
                SetPartsToRead(i,value);
                //partsToRead[i] = value;
            }

            // Generate Music Braille for all parts
            for (int i = 0; (i < partsToBraille.Length); i++)
            {
                SetPartsToBraille(i, value);
                //partsToBraille[i] = value;
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


        UserSettingsElements userSettingsElements;

        /// <summary>
        /// Private constructor, used by the Create() method
        /// </summary>
        /// <param name="node"></param>
        private UserSettings(PartlistElement partList)
        {
            this.partList = partList;
            int numberOfParts = partList.NumberOfParts();

            // Create a tree structure containing exactly the user settings that can be saved to and restored from the file systems.
            userSettingsElements = UserSettingsElements.Create();
            userSettingsElements.Init(partList); // Might as well be a part of the Create() method
            userSettingsElements.Test();


            //partsToPlay = new bool[numberOfParts];       // Must be done here because numberUfParts is not a constant.
            //partsToRead = new bool[numberOfParts];       // Must be done here because numberUfParts is not a constant.
            //partsToBraille = new bool[numberOfParts];    // Must be done here because numberUfParts is not a constant.

            InitReaderPartsSettings(partList);
            InitPlayerPartsSettings(partList);
            InitMusicBraillePartsSettings(partList);

            InitReaderDetailsSettings();
            InitPlayerDetailsSettings();
            InitMusicBrailleDetailsSettings();

            userTempo = 100; // Percentage of tempo indicated in score
            if (((int)ReaderSettingsEnum.NumberOfReaderSettings != readerSettings.Length)
            || ((int)PlayerSettingsEnum.NumberOfPlayerSettings != playerSettings.Length)
            || ((int)MusicBrailleSettingsEnum.NumberOfMusicBrailleSettings != musicBrailleSettings.Length))

            {
                throw (new Exception("UserSettings: Wrong size of arrays"));
            }

            //string test = userSettingsWriter.ToXml(this); // Used for initial test only !!
        }

        private void InitReaderPartsSettings(PartlistElement partList)
        {
            UserSettingsElement readerSettingsRoot = userSettingsElements.GetUserSettingsElement(UserSettingNames.Speech);
            UserSettingsElementBool use = readerSettingsRoot.GetNamedElementBool(UserSettingNames.Parts);
            partsToRead = new UserSettingsElementBool[partList.NumberOfParts()];
            int i;            
            for (i = 0; (i < partsToRead.Length); i++)
            {
                string name = partList.GetPartFromNumber(i).partName;
                string id = partList.GetPartFromNumber(i).partId;
                UserSettingsElementBool useb = use.GetNamedElementBool(id);
                if (null != useb)
                {
                    // This Part ID has an entry in the userSettings. 
                    partsToRead[i] = useb;
                }
                else
                {
                    // Last resort: Create a new UserSettingeElement, which will not be saved with the file
                    partsToRead[i] = (UserSettingsElementBool)UserSettingsElementBool.Create("", true);
                }
            }
        }

        private void InitPlayerPartsSettings(PartlistElement partList)
        {
            partsToPlay = new UserSettingsElementBool[partList.NumberOfParts()];
            int i;
            for (i = 0; (i < partsToPlay.Length); i++)
            {
                partsToPlay[i] = (UserSettingsElementBool)UserSettingsElementBool.Create("", true);
            }
        }
        

        private void InitMusicBraillePartsSettings(PartlistElement partList)
        {
            partsToBraille = new UserSettingsElementBool[partList.NumberOfParts()];
            int i;
            for (i = 0; (i < partsToBraille.Length); i++)
            {
                partsToBraille[i] = (UserSettingsElementBool)UserSettingsElementBool.Create("", true);
            }
        }



        public void Save()
        {
            userSettingsElements.Save();
            
        }

        public static UserSettings Create(PartlistElement partList)
        {
            return new UserSettings(partList);
        }
    }


}
