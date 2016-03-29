using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderUI
{
    /// <summary>
    /// Holds all user defined settings, such as the set of parts to play and read.
    /// Also allows for changing settings for development purposes.
    /// </summary>
    public class UserSettings
    {

        // All of these settings are just for exchanging simple information.
        // No need to make the coad less readable by making them private etc:

        // Arrays for controlling individual parts
        public bool[] partsToPlay; // Play the note values from these partitions
        public bool[] partsToRead; // Read the note values from these partitions
        public bool[] partsToReadLyrics; // Read the lyrics from these partitions

        // For controlling other user properties
        public bool readMeasureNumbers;
        public bool playMeasureBeats; // Not implemented yet.
        public bool readHarmonies;    // After localisation  
        public bool playHarmonies;
        public float userSlowDown;

        // For controlling other DEVELOPER properties
        public bool readDivisions;      
        public bool readHarmonyCodes; // As found in the MusicXml file
        public bool readEndEvents;






        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private UserSettings()
        {
        }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private UserSettings(int numberOfParts)
        {
            partsToPlay = new bool[numberOfParts];
            partsToRead = new bool[numberOfParts];
            partsToReadLyrics = new bool[numberOfParts];
            userSlowDown = 1.0F;
        }

        public static UserSettings Create(int numberOfParts)
        {
            return new UserSettings(numberOfParts);
        }

    }
}
