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
        const string UserSettings = "UserSettings"; // Name of the entire UserSettinds Element

        // The 3 Level 0 nodes in the Filter
        const string Sound = "Sound";
        const string Speech = "Speech";
        const string MusicBraille = "MusicBraille";

        // The 2 Level 1 nodes for each of the 3 level 0 nodes
        const string Parts = "Parts";
        const string Details = "Details";

        // Level 2 nodes for Details    

        const string MeasureNumbers = "MeasureNumbers";
        const string Harmonies = "Harmonies";
        const string Notes = "Notes";
        const string NoteOctaves = "NoteOctaves";
        const string NoteTypes = "NoteTypes";
        const string NoteAccidentals = "NoteAccidentals";
        const string Notations = "Notations";
        const string Lyrics = "Lyrics";
        const string MetaInformation = "MetaInformation";
        const string Divisions = "Divisions";
        const string HarmonyCodes = "HarmonyCodes";
        const string EndEvents = "HarmonyCodes";

    }
}
