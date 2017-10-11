using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MusicXmlReaderModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReader
{

    /// <summary>
    /// For decoupling user-commands from their (possibly localized) Keyboard-key representation textual representation.
    /// For instance
    /// UserCommandEnum.NextMeas from (Keys.Control | Keys.Right)  and "CONTROL-RIGHT"
    /// Call Init() during system initialisation to populate internal directories with values to use for looking up keycodes received from the keyboard
    /// Call LookUp() to convert input from the keyboard to a UserCommandEnum
    /// </summary>
    public class UserCommandDictionary
    {
        private static string className = "UserCommandDictionary";

        public enum UserCommandEnum
        {
            Unknown,
            NextEvent,
            PreviousEvent,
            NextMeasure,
            PreviousMeasure,
            NextDetail,
            PreviousDetail,
            GotoMeasure,
            TempoUp,
            TempoDown,
            Tempo,
            FirstInstrument,
            LastInstrument,
            FirstPart,
            LastPart,
            UpperChordNote,
            LowerChordNote,
            StartStop,
            GotoNoteList,
            StartPlaying,
            StopPlaying,
            Toggle, // Either PlayState or Checkbox
            TempoChange,
            Repeat,
            Goto,
            FilterNotesAll,
            FilterNotes,
            FilterNotesForMusic,
            FilterNotesForText,
            FilterNotesForBraille,
            FilterNotesForParts,
            FilterNotesToggle,
            FilterNotesOff,
            FilterNotesOn,
            DetailsUp,
            DetailsDown,
            TopPart,
            BottomPart,
            TopOfChord,
            ChordName
        };


        /// <summary>
        /// Returns the key combination for a textual representation of a user command.
        /// Used for populating commandDictionary with values partly controlled by the (localised) contents of ResourcesForUI
        /// </summary>
        /// <param name="licalizedInput"></param>
        /// <returns></returns>
        private static Keys GetKeys(string localizedInput)
        {
            string functionName = "GetKeys";
            char[] separators = new char[]{ '+' };
            string[] substrings = localizedInput.Split(separators);
            Keys result = Keys.None;
            switch (substrings.Length)
            {
                case 0: return Keys.None;
                case 1: // For instance P               
                    string s = substrings[0];
                    alphabeticKeyDictionary.TryGetValue(s, out result);
                    return result;
                case 2: // For instance CONTROL+P
                case 3: // For instance CONTROL+SHIFT+P
                case 4: // For instance CONTROL+SHIFT+ALT+P
                    alphabeticKeyDictionary.TryGetValue(substrings[substrings.Length - 1], out result); // The last substring is the alphabetic character
                    Keys specialKey = Keys.None;
                    foreach (string substring in substrings.Take(substrings.Length - 1))
                    {
                        specialKeyDictionary.TryGetValue(substring, out specialKey);
                        result |= specialKey; // Bitwise OR
                    }
                    return result; 
                default:
                    Logger.Log(string.Format("{0}.{1} Unexpected input= '{2}'", className, functionName, localizedInput));
                    return Keys.None;
            }
        }

#warning ToDo Del op i 3 eller flere dictionaries, 1 pr control så den samme Key kan være forskellige kommandoer i forskellige controller
        private static Dictionary<UserCommandEnum, System.Windows.Forms.Keys> commandDictionary; // Map from Command to key
        private static Dictionary<System.Windows.Forms.Keys, UserCommandEnum> noteListcommandDictionary;
        private static Dictionary<System.Windows.Forms.Keys, UserCommandEnum> noteFiltercommandDictionary;
        private static Dictionary<System.Windows.Forms.Keys, UserCommandEnum> detailsListcommandDictionary;
        // private static Dictionary<System.Windows.Forms.Keys, UserCommandEnum> ???commandDictionary;
        private static Dictionary<string, System.Windows.Forms.Keys> specialKeyDictionary;
        private static Dictionary<string, System.Windows.Forms.Keys> alphabeticKeyDictionary;

        public static void Init()
        {         
            // We recognize the following special keys
            specialKeyDictionary = new Dictionary<string, System.Windows.Forms.Keys>(3); // Never more than these 3 members
            specialKeyDictionary.Add("CONTROL", Keys.Control);  // May be localized if desired!
            specialKeyDictionary.Add("ALT", Keys.Alt);      // May be localized if desired!
            specialKeyDictionary.Add("SHIFT", Keys.Shift);    // May be localized if desired!

            // We recognize the following alphabetic keys
            alphabeticKeyDictionary = new Dictionary<string, System.Windows.Forms.Keys>(40);  // Never more than 40 members
            alphabeticKeyDictionary.Add("A", Keys.A);
            alphabeticKeyDictionary.Add("B", Keys.B);
            alphabeticKeyDictionary.Add("C", Keys.C);
            alphabeticKeyDictionary.Add("D", Keys.D);
            alphabeticKeyDictionary.Add("E", Keys.E);
            alphabeticKeyDictionary.Add("F", Keys.F);
            alphabeticKeyDictionary.Add("G", Keys.G);
            alphabeticKeyDictionary.Add("H", Keys.H);
            alphabeticKeyDictionary.Add("I", Keys.I);
            alphabeticKeyDictionary.Add("J", Keys.J);
            alphabeticKeyDictionary.Add("K", Keys.K);
            alphabeticKeyDictionary.Add("L", Keys.L);
            alphabeticKeyDictionary.Add("M", Keys.M);
            alphabeticKeyDictionary.Add("N", Keys.N);
            alphabeticKeyDictionary.Add("O", Keys.O);
            alphabeticKeyDictionary.Add("P", Keys.P);
            alphabeticKeyDictionary.Add("Q", Keys.Q);
            alphabeticKeyDictionary.Add("R", Keys.R);
            alphabeticKeyDictionary.Add("S", Keys.S);
            alphabeticKeyDictionary.Add("T", Keys.T);
            alphabeticKeyDictionary.Add("U", Keys.U);
            alphabeticKeyDictionary.Add("V", Keys.V);
            alphabeticKeyDictionary.Add("W", Keys.W);
            alphabeticKeyDictionary.Add("X", Keys.X);
            alphabeticKeyDictionary.Add("Y", Keys.Y);
            alphabeticKeyDictionary.Add("Z", Keys.Z);
            alphabeticKeyDictionary.Add("0", Keys.D0);
            alphabeticKeyDictionary.Add("1", Keys.D1);

            // We recognize the following commands from the user
            commandDictionary = new Dictionary<UserCommandEnum,System.Windows.Forms.Keys>(40);  // Never more than 40 members
            // For controlling the NoteList
            commandDictionary.Add(UserCommandEnum.PreviousEvent,        Keys.Left );                // No localization needed
            commandDictionary.Add(UserCommandEnum.NextEvent,            Keys.Right );               // No localization needed
            commandDictionary.Add(UserCommandEnum.PreviousMeasure,      Keys.Left  | Keys.Control);
            commandDictionary.Add(UserCommandEnum.NextMeasure,          Keys.Right | Keys.Control);
            commandDictionary.Add(UserCommandEnum.GotoNoteList,         GetKeys("CONTROL+L"));      // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.StartPlaying,         GetKeys("CONTROL+P"));      // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.StopPlaying,          GetKeys("CONTROL+SHIFT+P")); // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.Toggle,               Keys.Space);                // Either PlayState 
            commandDictionary.Add(UserCommandEnum.TempoChange,          GetKeys("CONTROL+T"));       // May be localized if desired! // Takes parameters 
            commandDictionary.Add(UserCommandEnum.Repeat,               GetKeys("CONTROL+R"));            // May be localized if desired! // Takes parameters  
            commandDictionary.Add(UserCommandEnum.Goto,                 GetKeys("CONTROL+G"));              // May be localized if desired! // Takes parameters 
            // For controlling the NoteFilter
            commandDictionary.Add(UserCommandEnum.FilterNotesAll,       GetKeys("ALT+A"));        // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.FilterNotes,          GetKeys("ALT+F"));           // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.FilterNotesForMusic,  GetKeys("ALT+M"));   // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.FilterNotesForText,   GetKeys("ALT+T"));    // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.FilterNotesForBraille,GetKeys("ALT+B")); // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.FilterNotesForParts,  GetKeys("ALT+S"));   // May be localized if desired!  // Stemmer / Parts
            commandDictionary.Add(UserCommandEnum.FilterNotesToggle,    Keys.Space      );  // Toggle checkbox
            commandDictionary.Add(UserCommandEnum.FilterNotesOff,       GetKeys("ALT+0"));        // May be localized if desired!
            commandDictionary.Add(UserCommandEnum.FilterNotesOn,        GetKeys("ALT+1"));         // May be localized if desired!

            // For controlling tempo
            commandDictionary.Add(UserCommandEnum.TempoUp,              Keys.Alt | Keys.PageUp);
            commandDictionary.Add(UserCommandEnum.TempoDown,            Keys.Alt | Keys.PageDown);

            // While viewing details
            commandDictionary.Add(UserCommandEnum.DetailsUp,            Keys.Up);
            commandDictionary.Add(UserCommandEnum.DetailsDown,          Keys.Down);

            // To start viewing details
            commandDictionary.Add(UserCommandEnum.TopOfChord,           Keys.Control | Keys.Up);
            commandDictionary.Add(UserCommandEnum.ChordName,            Keys.Control | Keys.Down);
            commandDictionary.Add(UserCommandEnum.FirstPart,            Keys.Control | Keys.Up);
            commandDictionary.Add(UserCommandEnum.LastPart,             Keys.Control | Keys.Down);
            commandDictionary.Add(UserCommandEnum.FirstInstrument,      GetKeys("CONTROL+I")); // Later: ResourcesForUI.UserCommandFirstInstrument 


            // The following commands are accepted by the noteList:
            noteListcommandDictionary = new Dictionary<System.Windows.Forms.Keys, UserCommandEnum>(); // The following commands are accepted by the noteList:
            Add(noteListcommandDictionary, UserCommandEnum.PreviousEvent);
            Add(noteListcommandDictionary, UserCommandEnum.NextEvent);


            // The following commands are accepted by the detailList:


            // The following commands are accepted by the filterTree:

        }

        private static void Add(Dictionary<System.Windows.Forms.Keys, UserCommandEnum> directory, UserCommandEnum command)
        {
            Keys keys = Keys.None;
            commandDictionary.TryGetValue(command, out keys);
            directory.Add(keys, command); 
        }


        public UserCommandEnum LookupInNoteList(Keys keys)
        {
            string functionName = "Lookup";
            UserCommandEnum result = UserCommandEnum.Unknown;
            noteListcommandDictionary.TryGetValue(keys,out result);
            Logger.Log(string.Format("{0}.{1}({2}) returned {3}", className, functionName, keys, result));
            return result;
        }

    }
}
