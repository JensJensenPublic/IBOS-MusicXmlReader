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
        public bool[] partsToReadLyrics; // Read the lyrics from these partitions
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


        // Global Reader Settings settings (for all parts)
        public enum ReaderSettings
        {
            MeasureNumbers =0 ,
            Harmonies =1,
            Notes =2,
            NoteOctaves =3,
            NoteTypes =4,
            Notations =5,
            Lyrics = 6,
            Divisions =7,
            HarmonyCodes =8,
            EndEvents =9,
            NumberOfReaderSettings =10
        };
        public readonly string[] readerSettingsNames =
        {
            ResourcesForModel.UserSettings_ReaderNames_MeasureNumbers,  // "TaktNumre",
            ResourcesForModel.UserSettings_ReaderNames_Harmonies,       // "Harmonier",
            ResourcesForModel.UserSettings_ReaderNames_Notes,           // "Noder",
            ResourcesForModel.UserSettings_ReaderNames_Octaves,         // "Oktaver",
            ResourcesForModel.UserSettings_ReaderNames_NoteValues,      // "NodeVærdier",
            ResourcesForModel.UserSettings_ReaderNames_Notations,       // "Notationer",
            ResourcesForModel.UserSettings_ReaderNames_Lyrics,          // "Tekst"
            ResourcesForModel.UserSettings_ReaderNames_Divisions,       // "Divisions",
            ResourcesForModel.UserSettings_ReaderNames_HarmonyCodes,    // "HarmoniCodes",
            ResourcesForModel.UserSettings_ReaderNames_EndEvents        // "EndEvents"
        };

        public bool[]            readerSettingsValues=
        {
            true,
            true,
            true,
            true, 
            true,
            true,
            true,
            false,
            false,
            false
        };
 
        public bool GetReaderSettings(ReaderSettings i)
        {
            return readerSettingsValues[(int)i];
        }
        public void SetReaderSettings(int i, bool b)
        {
            readerSettingsValues[(int)i] = b;
        }

        
        // Global Player Settings (for all parts)
        public enum PlayerSettings { MeasureBeats = 0, Harmonies = 1,NumberOfPlayerSettings=2}
        public readonly string[] playerSettingsNames =
            {
            ResourcesForModel.UserSettings_PlayerNames_Beats,       //"TaktSlag",
            ResourcesForModel.UserSettings_PlayerNames_Harmonies    //"Harmonier"
        };
        public bool[] playerSettingsValues = { false, false };
  
        public bool GetPlayerSettings(PlayerSettings i)
        {
            return playerSettingsValues[(int)i];
        }
        public void SetPlayerSettings(int i, bool b)
        {
            playerSettingsValues[(int)i] = b;
        }

 
        // Global Music Braille Settings settings (for all parts)
        public enum MusicBrailleSettings { MeasureNumbers = 0, Harmonies = 1, Notes = 2,  Notations = 3,  NumberOfReaderSettings = 4 };
        public readonly string[] musicBrailleSettingsNames =
        {
            ResourcesForModel.UserSettings_BrailleNames_MeasureNumbers, //"TaktNumre",
            ResourcesForModel.UserSettings_BrailleNames_Harmonies,      //"Harmonier",
            ResourcesForModel.UserSettings_BrailleNames_Notes,          //"Noder",
            ResourcesForModel.UserSettings_BrailleNames_Notations       //"Notationer"
        };

        public bool[] musicBrailleSettingsValues = { false, false, true, true };

        public bool GetMusicBrailleSettings(MusicBrailleSettings i)
        {
            return musicBrailleSettingsValues[(int)i];
        }
        public void SetMusicBrailleSettings(int i, bool b)
        {
            musicBrailleSettingsValues[(int)i] = b;
        }
        

        // Non-boolean user settings
        public float userSlowDown;      // Percentage of the speed described in the MusicXml file

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
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private UserSettings(int numberOfParts)
        {
            partsToPlay = new bool[numberOfParts];       // Must be done here because numberUfParts is not a constant.
            partsToRead = new bool[numberOfParts];       // Must be done here because numberUfParts is not a constant.
            partsToBraille = new bool[numberOfParts];    // Must be done here because numberUfParts is not a constant.
            partsToReadLyrics = new bool[numberOfParts]; // Must be done here because numberUfParts is not a constant.
            userSlowDown = 1.0F;
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
