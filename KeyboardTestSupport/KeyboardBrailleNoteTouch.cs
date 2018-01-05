using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace KeyboardTest
{
    // Manufacturer: HumanWare
    // Distributor:  Instrulog
    // Comment:      The Android device


    // Device ID: USB\VID_1C71&PID_C00A\650400001164, PNP Device ID: USB\VID_1C71&PID_C00A\650400001164, Description: USB-inputenhed

    public class KeyboardBrailleNoteTouch : Keyboard
    {
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        string deviceName = "BrailleNote Touch";

        enum ModifierKeysEnum { FunctionKeys = 4, INSERT = 6, CTRL = 3, WINDOWS = 1, JAWS = 5, ALT = 2, SHIFT = 7 } // Copied from Braillant. Only a best guess !!

        // Convenience definitions:
        const int SPACE = (int)DSKey.MELLEMRUM;
        const int CHORD = SPACE;
        const int CTRL = (int)ModifierKeysEnum.CTRL;
        const int SHIFT = (int)ModifierKeysEnum.SHIFT;
        const int ALT = (int)ModifierKeysEnum.ALT;
        const int JAWS = (int)ModifierKeysEnum.JAWS;
        const int WINDOWS = (int)ModifierKeysEnum.WINDOWS;
        const int INSERT = (int)ModifierKeysEnum.INSERT;
        const int FUNCTION = (int)ModifierKeysEnum.FunctionKeys;

        protected override PerkinsKeySequence GetPKS(KeySequenceList jawsKeySequence)
        {
            if (jawsKeySequence == KeySequenceList.JawsToggleSpeech) return PerkinsKeySequence.Create(new List<int> { 1, 2, 4, CHORD }, new List<int> { 2, 3, 4 }); // Not tested !
            if (jawsKeySequence == KeySequenceList.JAWSReadStatusLine) return PerkinsKeySequence.Create(new List<int> { INSERT, 8, CHORD }, new List<int> { 5, 6, 7, CHORD }); // Not tested !
            if (jawsKeySequence == KeySequenceList.JAWSReadTitleLine) return PerkinsKeySequence.Create(new List<int> { INSERT, 8, CHORD }, new List<int> { 2, 3, 4, 5 }); // Not tested !
            if (jawsKeySequence == KeySequenceList.JAWSReadMessage) return PerkinsKeySequence.Create(new List<int> { INSERT, 8, CHORD }, new List<int> { 1, 2 }); // Not tested !
            // Add more definitions here
            return PerkinsKeySequence.Create();
        }

        protected override PerkinsKeySequence GetPKS(Keys keys)
        {
            switch (keys)
            {
                case Keys.A: return PerkinsKeySequence.Create(new List<int> { 1 }); // WORKS For ComboBox only. "a" is used to represent all letters

                // Single key presses representing simple digits 0.9
                case Keys.D1: return PerkinsKeySequence.Create(new List<int> { 1, 8 }); // WORKS For ComboBox only. "1" is used to represent all letters

                // Double key presses representing conbinations of CONTROL or ALT with a simple letter. SHIFT is probably not needed !
                case Keys.Control | Keys.A: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1 }); // WORKS For general use. "a" is used to represent all letters
                case Keys.Alt | Keys.A: return PerkinsKeySequence.Create(new List<int> { ALT, 8, CHORD }, new List<int> { 1 }); // WORKS For general use. "a" is used to represent all letters
                                                                                                                                 //testSteps.Add(TestStep.Create(Keys.Shift | Keys.A, new List<int> { SHIFT, 8, CHORD }, new List<int> { 1 })); // For general use. "a" is used to represent all letters

                // Shortcut-keys, used by Windows or JAWS:
                case Keys.Control | Keys.O: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 3, 5 }); // WORKS Windows: Open FileOpen dialogue
                case Keys.Alt | Keys.F4: return PerkinsKeySequence.Create(new List<int> { ALT, FUNCTION, 8, CHORD }, new List<int> { 1, 4, 5 }); // WORKS Windows: Close program ( {1,4,5} = "D" = "4" )
                //case Keys.Alt: return PerkinsKeySequence.Create(new List<int> { 1, 3, 4, CHORD }); // FAILS Windows: Open Menu line ( {1,3,4} = "M" )
                case Keys.Tab: return PerkinsKeySequence.Create(new List<int> { 4, 6, CHORD }); // WORKS Windows: Next Control in current form

                case Keys.Insert | Keys.Space:     return PerkinsKeySequence.Create(new List<int> { 1, 2, 4, CHORD }); // FAILS  JAWS: Toggle speech On/Off
                case Keys.Insert | Keys.T:     return PerkinsKeySequence.Create(new List<int> { 5, 8, CHORD }, new List<int> { 2, 3, 4, 5 }); // FAILS JAWS: Read Title line ( {2,3,4,5} = T) 
                case Keys.Insert | Keys.B:     return PerkinsKeySequence.Create(new List<int> { 5, 8, CHORD }, new List<int> { 1, 2 }); // FAILS JAWS: Read Message  ( {1,2} = B)


                // Double key presses representing conbinations of CONTROL SHIFT ALT with a simple digit. Probably not needed anyway !!!
                // testSteps.Add(TestStep.Create(Keys.Control  | Keys.D1, new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
                // testSteps.Add(TestStep.Create(Keys.Shift    | Keys.D1, new List<int> { SHIFT,8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
                // testSteps.Add(TestStep.Create(Keys.Alt      | Keys.D1, new List<int> { ALT,  8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters

                ////////////////////////////////////////////////////
                // Single key presses representing control functions
                ////////////////////////////////////////////////////

                case Keys.Space:    return PerkinsKeySequence.Create(new List<int> { SPACE }); // WORKS
                case Keys.Enter:    return PerkinsKeySequence.Create(new List<int> { 8 }); // WORKS
                case Keys.Back:     return PerkinsKeySequence.Create(new List<int> { 7 }); // WORKS
                case Keys.Escape:   return PerkinsKeySequence.Create(new List<int> { 1, 5, SPACE }); // WORKS
                // case Keys.Tab:   return PerkinsKeySequence.Create(new List<int> { 4, 6, SPACE }); // Defined above
                case Keys.Home:     return PerkinsKeySequence.Create(new List<int> { 2, 3, SPACE }); // WORKS
                case Keys.End:      return PerkinsKeySequence.Create(new List<int> { 5, 6, SPACE }); // WORKS
                case Keys.PageUp:   return PerkinsKeySequence.Create(new List<int> { 1, 2, SPACE }); // WORKS
                case Keys.PageDown: return PerkinsKeySequence.Create(new List<int> { 4, 5, SPACE }); // WORKS Reported as "Next"
                //////////////////////////////////////////////////////
                //// Single key presses representing arrow-navigation
                //////////////////////////////////////////////////////
                case Keys.Right:     return PerkinsKeySequence.Create(new List<int> { 6, CHORD } ); // WORKS;
                case Keys.Left:     return PerkinsKeySequence.Create(new List<int> { 3, CHORD }); // WORKS;
                case Keys.Up:     return PerkinsKeySequence.Create(new List<int> { 1, CHORD }); // WORKS;
                case Keys.Down:     return PerkinsKeySequence.Create(new List<int> { 4, CHORD }); // WORKS;
                //////////////////////////////////////////////////////
                //// Double key presses representing arrow-navigation
                //////////////////////////////////////////////////////
                case Keys.Control | Keys.Right: return PerkinsKeySequence.Create(new List<int> { 3, 8, CHORD }, new List<int> { 6, CHORD }); // WORKS
                case Keys.Control | Keys.Left: return PerkinsKeySequence.Create(new List<int> { 3, 8, CHORD }, new List<int> { 3, CHORD }); // WORKS
                case Keys.Control | Keys.Up:     return PerkinsKeySequence.Create(new List<int> { 3, 8, CHORD }, new List<int> { 1, CHORD }); // WORKS 
                case Keys.Control | Keys.Down:     return PerkinsKeySequence.Create(new List<int> { 3, 8, CHORD }, new List<int> { 4, CHORD }); // WORKS 


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Single key presses representing characters and reported as PACKET combined with the UNICODE value on KeyPressed
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                case Keys.Oemcomma: return PerkinsKeySequence.Create(new List<int> { 2 });  // WORKS For entering numric parameters. Is reported as a PACKET as digits and alpha characters
                case Keys.OemPeriod: return PerkinsKeySequence.Create(new List<int> { 3 }); // WORKS For entering numric parameters. Is reported as a PACKET as digits and alpha characters

                default: return PerkinsKeySequence.Create(); // A PerkinsKEySequence without contents
            }
        }
    

    protected override void TestDeviceSpecificFunctions(bool all)
        { }


        protected void TestKeyCombinations(bool all)
        { }

        internal KeyboardBrailleNoteTouch()
        {
        }

    }
}
