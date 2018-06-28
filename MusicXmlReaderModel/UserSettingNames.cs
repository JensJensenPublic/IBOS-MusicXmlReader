using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// NOTE: For forward and backward compatibility reasons these names must NEVER BE CHANGED !
    /// </summary>
    public static class UserSettingNames
    {
        public const string UserSettings = "UserSettings"; // Name of the entire UserSettinds Element

        // The 3 Level 0 nodes in the Filter
        public const string Sound = "Sound";
        public const string Speech = "Speech";
        public const string MusicBraille = "MusicBraille";

        // The 2 Level 1 nodes for each of the 3 level 0 nodes
        public const string Parts = "Parts";
        public const string Details = "Details";

        // Level 2 nodes for Details    

        public const string MeasureNumbers = "MeasureNumbers";
        public const string Harmonies = "Harmonies";
        public const string Notes = "Notes";
        public const string NoteOctaves = "NoteOctaves";
        public const string NoteTypes = "NoteTypes";
        public const string NoteAccidentals = "NoteAccidentals";
        public const string Notations = "Notations";
        public const string Lyrics = "Lyrics";
        public const string MetaInformation = "MetaInformation";
        public const string Divisions = "Divisions";
        public const string HarmonyCodes = "HarmonyCodes";
        public const string EndEvents = "EndEvents";


        // Names used for transferring Type information
        public const string Type = "Type";
        public const string TypeVoid = "Void";
        public const string TypeBool = "Bool";
        public const string TypeInt = "Int";
        public const string TypeString = "String";


    }
}
