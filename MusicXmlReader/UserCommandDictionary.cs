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
        private string className = "UserCommandDictionary";

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
            Up,
            Down,
            TopPart,
            BottomPart
        };


        /// <summary>
        /// Returns the key combination for a textual representation of a user command.
        /// Used for populating commandDictionary with values partly controlled by the (localised) contents of ResourcesForUI
        /// </summary>
        /// <param name="licalizedInput"></param>
        /// <returns></returns>
        private Keys GetKeys(string localizedInput)
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
                    Logger.Log(string.Format("{0}.{1} Unexpected input= '{2}'", className, functionName, localizedInput);
                    return Keys.None;
            }
        }


        private static Dictionary<System.Windows.Forms.Keys, UserCommandEnum> commandDictionary;
        private static Dictionary<string, System.Windows.Forms.Keys> specialKeyDictionary;
        private static Dictionary<string, System.Windows.Forms.Keys> alphabeticKeyDictionary;

        public void Init()
        {         
            // We recognize the following special keys
            specialKeyDictionary = new Dictionary<string, System.Windows.Forms.Keys>();
            specialKeyDictionary.Add("CONTROL", Keys.Control);  // May be localized if desired!
            specialKeyDictionary.Add("ALT", Keys.Control);      // May be localized if desired!
            specialKeyDictionary.Add("SHIFT", Keys.Control);    // May be localized if desired!

            // We recognize the following alphabetic keys
            alphabeticKeyDictionary = new Dictionary<string, System.Windows.Forms.Keys>();
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

            // We recognize the following commands from the user
            commandDictionary = new Dictionary<System.Windows.Forms.Keys, UserCommandEnum>();
            // For controlling the NoteList
            commandDictionary.Add(Keys.Left, UserCommandEnum.PreviousEvent);    // No localization needed
            commandDictionary.Add(Keys.Right, UserCommandEnum.NextEvent);       // No localization needed
            commandDictionary.Add(Keys.Left  | Keys.Control, UserCommandEnum.PreviousMeasure);
            commandDictionary.Add(Keys.Right | Keys.Control, UserCommandEnum.NextMeasure);
            commandDictionary.Add(GetKeys("CONTROL+L"), UserCommandEnum.GotoNoteList);      // May be localized if desired!
            commandDictionary.Add(GetKeys("CONTROL+P"), UserCommandEnum.StartPlaying);      // May be localized if desired!
            commandDictionary.Add(GetKeys("CONTROL+SHIFT+P"), UserCommandEnum.StopPlaying); // May be localized if desired!
            commandDictionary.Add(Keys.Space, UserCommandEnum.Toggle); // Either PlayState or Checkboxvalue
            commandDictionary.Add(GetKeys("CONTROL+T"), UserCommandEnum.TempoChange);       // May be localized if desired! // Takes parameters 
            commandDictionary.Add(GetKeys("CONTROL+R"), UserCommandEnum.Repeat);            // May be localized if desired! // Takes parameters  
            commandDictionary.Add(GetKeys("CONTROL+G"), UserCommandEnum.Goto);              // May be localized if desired! // Takes parameters 
            // For controlling the NoteFilter
            commandDictionary.Add(GetKeys("ALT+A"), UserCommandEnum.FilterNotesAll);        // May be localized if desired!
            commandDictionary.Add(GetKeys("ALT+F"), UserCommandEnum.FilterNotes);           // May be localized if desired!
            commandDictionary.Add(GetKeys("ALT+M"), UserCommandEnum.FilterNotesForMusic);   // May be localized if desired!
            commandDictionary.Add(GetKeys("ALT+T"), UserCommandEnum.FilterNotesForText);    // May be localized if desired!
            commandDictionary.Add(GetKeys("ALT+B"), UserCommandEnum.FilterNotesForBraille); // May be localized if desired!
            commandDictionary.Add(GetKeys("ALT+S"), UserCommandEnum.FilterNotesForParts);   // May be localized if desired!  // Stemmer / Parts
            commandDictionary.Add(Keys.Space,       UserCommandEnum.FilterNotesToggle);  
            commandDictionary.Add(GetKeys("ALT+0"), UserCommandEnum.FilterNotesOff);        // May be localized if desired!
            commandDictionary.Add(GetKeys("ALT+1"), UserCommandEnum.FilterNotesOn);         // May be localized if desired!

            // For controlling tempo
            commandDictionary.Add(Keys.Alt | Keys.PageUp, UserCommandEnum.TempoUp);
            commandDictionary.Add(Keys.Alt | Keys.PageDown, UserCommandEnum.TempoDown);

            // To start viewing details or viewing details
            commandDictionary.Add(Keys.Up, UserCommandEnum.Up);
            commandDictionary.Add(Keys.Down, UserCommandEnum.Down);
            commandDictionary.Add(Keys.Control | Keys.Up, UserCommandEnum.TopPart);
            commandDictionary.Add(Keys.Control | Keys.Down, UserCommandEnum.BottomPart);
            commandDictionary.Add(GetKeys("CONTROL+I"), UserCommandEnum.FirstInstrument); // Later: ResourcesForUI.UserCommandFirstInstrument 
        }

        public UserCommandEnum Lookup(Keys keys)
        {
            string functionName = "Lookup";
            UserCommandEnum result = UserCommandEnum.Unknown;
            commandDictionary.TryGetValue(keys,out result);
            Logger.Log(string.Format("{0}.{1}({2}) returned {3}", className, functionName, keys, result));
            return result;
        }

    }
}
