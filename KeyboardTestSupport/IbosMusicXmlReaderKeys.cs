using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardTest
{

    /// <summary>
    /// This class represents all Key combinations presently needed by the IBOS MusicXmlReader application
    /// </summary>
    public static class IbosMusicXmlReaderKeys
    {
        public static List<Keys> keys = new List<Keys>
        {
            // "A"  represent ALL letters a..z
            Keys.A,                  // Generel input, for instance file names
            Keys.A | Keys.Control,   // Keyboard shortcut, not assigned
            Keys.G | Keys.Control,   // Keyboard shortcut Rediger->Goto 
            Keys.R | Keys.Control,   // Keyboard shortcut Rediger->Repeat 
            Keys.N | Keys.Control,   // Keyboard shortcut Rediger->% af &Nominal tempo
//            Keys.K | Keys.Control,   // Keyboard shortcut Rediger->Keyboard
            Keys.M | Keys.Control,   // Keyboard shortcut Rediger->MessageBox              
            Keys.A | Keys.Alt,       // Keyboard shortcuts
            Keys.F | Keys.Alt,       // Open "&Files" menu
            Keys.R | Keys.Alt,       // Open "&Rediger" menu
            // "1"  represent ALL digits 0 .. 9 
            Keys.D1,                 // Generel input. Input of GOTO, REPEAT and TEMPO parameters

            // Keys shortcut keys, used by Windows or JAWS /MEntioned by D.G. In "BrailleNote Touch genvejstaster til IBOS Nodelæser"
            Keys.O | Keys.Control,          // Open FileOpen Dialog
            Keys.F4 | Keys.Alt,             // Terminate program
            Keys.Alt,                       // Go to Menu-line
            Keys.Tab,                       // Toggle among controls in current form
 
            // Single key presses representing control functions 
            Keys.Space,
            Keys.Enter,
            Keys.Back,
            Keys.Escape,
            Keys.Tab,
            Keys.Home,
            Keys.End,
            Keys.PageUp,
            Keys.PageDown,
            // Single key presses representing arrow-navigation
            ////////////////////////////////////////////////////
            Keys.Right,
            Keys.Left,
            Keys.Up,
            Keys.Down,
            // Double key presses representing arrow-navigation
            ////////////////////////////////////////////////////
            Keys.Control | Keys.Right,
            Keys.Control | Keys.Left,
            Keys.Control | Keys.Up,
            Keys.Control | Keys.Down,
            // Single key presses representing special characters 
            Keys.Oemcomma, // For possible numeric input
            Keys.OemPeriod, // For possible numeric input
            // Keys.Control | Keys.Multiply // Needed as shortcut to "TEMPO" because CTRL-T is used for Filter.Text! Replace by CTRL+N !!!
        };


        public static List<Keys> optionalKeys = new List<Keys>
        {            
            //Windows Explorer tree control
            Keys.Multiply,  // Numeric Keypad*: Expands everything under the current selection
            Keys.Add,       // Numeric Keypad +: Expands the current selection
            Keys.Subtract   // Numeric Keypad -: Collapses the current selection.
        };


        public static List<KeySequenceList> keySequences = new List<KeySequenceList>
            {
               KeySequenceList.Create(  new List<Keys> { Keys.Insert, Keys.Space  }, Keys.S),       // Toggle JAWS speech on/off
               KeySequenceList.Create(  new List<Keys> { Keys.Insert, Keys.PageDown  }, Keys.None), // Read Status Line 
               KeySequenceList.Create(  new List<Keys> { Keys.Insert, Keys.T  }, Keys.None),        // Read Title Line
               KeySequenceList.Create(  new List<Keys> { Keys.Insert, Keys.B  }, Keys.None)         // Read MessageBox
            };
    }
}
