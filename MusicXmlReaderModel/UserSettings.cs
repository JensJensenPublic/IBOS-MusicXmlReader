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



        // Arrays for controlling individual parts
        public bool[] partsToPlay; // Play the note values from these partitions
        public bool[] partsToRead; // Read the note values from these partitions
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

        public enum ReaderSettings
        {
            MeasureNumbers =0 ,
            Harmonies =1,
            Notes =2,
            NoteOctaves =3,
            NoteTypes =4,
            NoteAccidentals =5,
            Notations =6,
            Lyrics = 7,
            MetaInformation = 8,
            Divisions =9,
            HarmonyCodes =10,
            EndEvents =11,
            NumberOfReaderSettings =12
        };
        

        public readonly string[] readerSettingsNames  = new string[(int)ReaderSettings.NumberOfReaderSettings];
        public bool[] readerSettingsValues = new bool[(int)ReaderSettings.NumberOfReaderSettings];

        private void InitReaderSetting(ReaderSettings setting , string name, bool value )
        {
            readerSettingsNames[(int) setting] = name;
            readerSettingsValues[(int)setting] = value;
        }
        
        private void InitReaderSettings()
        {
            InitReaderSetting(ReaderSettings.MeasureNumbers, ResourcesForModel.UserSettings_ReaderNames_MeasureNumbers, true);  // "TaktNumre",
            InitReaderSetting(ReaderSettings.Harmonies,      ResourcesForModel.UserSettings_ReaderNames_Harmonies, true);       // "Harmonier",
            InitReaderSetting(ReaderSettings.Notes,          ResourcesForModel.UserSettings_ReaderNames_Notes, true);           // "Noder",
            InitReaderSetting(ReaderSettings.NoteOctaves,    ResourcesForModel.UserSettings_ReaderNames_Octaves, true);         // "Oktaver",
            InitReaderSetting(ReaderSettings.NoteTypes,      ResourcesForModel.UserSettings_ReaderNames_NoteValues, true);      // "NodeVærdier",
            InitReaderSetting(ReaderSettings.NoteAccidentals,ResourcesForModel.UserSettings_ReaderNames_Accidentals, false);    // "Læse fortegn"
            InitReaderSetting(ReaderSettings.Notations,      ResourcesForModel.UserSettings_ReaderNames_Notations, true);       // "Notationer",
            InitReaderSetting(ReaderSettings.Lyrics,         ResourcesForModel.UserSettings_ReaderNames_Lyrics, true);          // "Tekst"
            InitReaderSetting(ReaderSettings.MetaInformation,ResourcesForModel.UserSettings_ReaderNames_Metainformation, true); // "Meta-information",
            InitReaderSetting(ReaderSettings.Divisions,      ResourcesForModel.UserSettings_ReaderNames_Divisions, false);      // "Divisions",
            InitReaderSetting(ReaderSettings.HarmonyCodes,   ResourcesForModel.UserSettings_ReaderNames_HarmonyCodes, false);   // "HarmoniCodes",
            InitReaderSetting(ReaderSettings.EndEvents,      ResourcesForModel.UserSettings_ReaderNames_EndEvents, false);      // "EndEvents"
        }
        
        public bool GetReaderSettings(ReaderSettings i)
        {
            return readerSettingsValues[(int)i];
        }
        public void SetReaderSettings(int i, bool b)
        {
            readerSettingsValues[(int)i] = b;
        }

        //*****************************************************************************************
        // Global Player Settings (for all parts)
        //*****************************************************************************************

//        public enum PlayerSettings { MeasureBeats = 0, Harmonies = 1, NumberOfPlayerSettings = 2 }
        public enum PlayerSettings { Harmonies = 0, NumberOfPlayerSettings = 1 }
        public readonly string[] playerSettingsNames = new string[(int)PlayerSettings.NumberOfPlayerSettings];
        public bool[] playerSettingsValues = new bool[(int)PlayerSettings.NumberOfPlayerSettings];

        private void InitPlayerSetting(PlayerSettings setting, string name, bool value)
        {
            playerSettingsNames[(int)setting] = name;
            playerSettingsValues[(int)setting] = value;
        }

        private void InitPlayerSettings()
        {
#warning ToDo Implement and re-enable PlayerSettings.MeasureBeats
            // InitPlayerSetting(PlayerSettings.MeasureBeats, ResourcesForModel.UserSettings_PlayerNames_Beats, false);  // "Taktslag", 
            InitPlayerSetting(PlayerSettings.Harmonies, ResourcesForModel.UserSettings_PlayerNames_Harmonies, true);  // "Harmonier",
        }

        public bool GetPlayerSettings(PlayerSettings i)
        {
            return playerSettingsValues[(int)i];
        }
        public void SetPlayerSettings(int i, bool b)
        {
            playerSettingsValues[(int)i] = b;
        }

        //*****************************************************************************************
        // Global Music Braille Settings settings (for all parts)
        //*****************************************************************************************

        public enum MusicBrailleSettings { MeasureNumbers = 0, Harmonies = 1, Notes = 2,  Notations = 3,  NumberOfMusicBrailleSettings = 4 };
        public readonly string[] musicBrailleSettingsNames = new string[(int)MusicBrailleSettings.NumberOfMusicBrailleSettings];
        public bool[] musicBrailleSettingsValues = new bool[(int)MusicBrailleSettings.NumberOfMusicBrailleSettings];

        private void InitMusicBrailleSetting(MusicBrailleSettings setting, string name, bool value)
        {
            musicBrailleSettingsNames[(int)setting] = name;
            musicBrailleSettingsValues[(int)setting] = value;
        }

        private void InitMusicBrailleSettings()
        {
            InitMusicBrailleSetting(MusicBrailleSettings.MeasureNumbers, ResourcesForModel.UserSettings_BrailleNames_MeasureNumbers,false);//"TaktNumre",
            InitMusicBrailleSetting(MusicBrailleSettings.Harmonies, ResourcesForModel.UserSettings_BrailleNames_Harmonies,false); //"Harmonier",
            InitMusicBrailleSetting(MusicBrailleSettings.Notes, ResourcesForModel.UserSettings_BrailleNames_Notes,true);  //"Noder",
            InitMusicBrailleSetting(MusicBrailleSettings.Notations, ResourcesForModel.UserSettings_BrailleNames_Notations,true);   //"Notationer"
        }
        
        public bool GetMusicBrailleSettings(MusicBrailleSettings i)
        {
            return musicBrailleSettingsValues[(int)i];
        }
        public void SetMusicBrailleSettings(int i, bool b)
        {
            musicBrailleSettingsValues[(int)i] = b;
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
            for (int i = 0; (i < musicBrailleSettingsValues.Length); i++)
            {
                SetMusicBrailleSettings(i, value);
            }

        
        }

        public void SetAllNormalTextSettings(bool value)
        {
            // First handle all Normal text specific settings
            for (int i = 0; (i < readerSettingsValues.Length); i++)
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
            if (((int)ReaderSettings.NumberOfReaderSettings != readerSettingsNames.Length)
            || ((int)ReaderSettings.NumberOfReaderSettings  != readerSettingsValues.Length)
            || ((int)PlayerSettings.NumberOfPlayerSettings  != playerSettingsNames.Length)
            || ((int)PlayerSettings.NumberOfPlayerSettings  != playerSettingsValues.Length))
            {
                throw (new Exception("UserSettings: Wrong size of arrays"));
            }

            // string test = userSettingsWriter.ToXml(this); // Used for initial test only !!
        }

        public static UserSettings Create(PartlistElement partList)
        {
            return new UserSettings(partList);
        }

    }
}
