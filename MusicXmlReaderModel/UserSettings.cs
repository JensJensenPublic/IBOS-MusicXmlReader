using System;
using System.Globalization;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Holds all user defined settings, such as the set of parts to play and read.
    /// Also allows for changing settings for development purposes.
    /// </summary>
    public class UserSettings
    {

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
            Notations =5,
            Lyrics = 6,
            MetaInformation = 7,
            Divisions =8,
            HarmonyCodes =9,
            EndEvents =10,
            NumberOfReaderSettings =11
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

        public enum PlayerSettings { MeasureBeats = 0, Harmonies = 1,NumberOfPlayerSettings=2}
        public readonly string[] playerSettingsNames = new string[(int)PlayerSettings.NumberOfPlayerSettings];
        public bool[] playerSettingsValues = new bool[(int)PlayerSettings.NumberOfPlayerSettings];

        private void InitPlayerSetting(PlayerSettings setting, string name, bool value)
        {
            playerSettingsNames[(int)setting] = name;
            playerSettingsValues[(int)setting] = value;
        }

        private void InitPlayerSettings()
        {
            InitPlayerSetting(PlayerSettings.MeasureBeats, ResourcesForModel.UserSettings_PlayerNames_Beats, false);  // "Taktslag",
            InitPlayerSetting(PlayerSettings.Harmonies, ResourcesForModel.UserSettings_PlayerNames_Harmonies, false);  // "Harmonier",
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
        public int UserTempo
        {
            get
            {
                return userTempo;
            }

            set
            {
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
            // Then enable allparst: 
            for (int i = 0; (i < partsToRead.Length); i++)
            {
                partsToRead[i] = value;
            }
        }

        /// <summary>
        /// Enable or disable all settings related to MssicBraille
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
        private UserSettings(int numberOfParts)
        {
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
        }

        public static UserSettings Create(int numberOfParts)
        {
            return new UserSettings(numberOfParts);
        }

    }
}
