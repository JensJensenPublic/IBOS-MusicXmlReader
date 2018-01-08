using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System;


// http://support.humanware.com/Site/Files/a/06cc20404d351cbaa2520fcd0d5adc3/1e6a68ebde8e47aad878d7a3e74743d4/EN%20Brailliant%20with%20Screen%20Readers_Rev08.htm#_Toc332634180

namespace KeyboardTest
{
    // Manufacturer: HumanWare
    // Distributor:  Instrulog 
    // Comment:

    public class KeyboardBraillant : Keyboard
    {
        private string deviceName = "Braillant";
        public override string DeviceName { get { return deviceName;} }
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // Note ModifierKeysEnum differs between various keyboards !!
        enum ModifierKeysEnum { FunctionKeys = 4, INSERT = 6, CTRL = 3, WINDOWS = 1, JAWS = 5, ALT = 2, SHIFT = 7 } // From the .pdf file above


        // Convenience definitions:
        const int SPACE = (int)DSKey.MELLEMRUM;
        const int CHORD = SPACE;

        const int CTRL =    (int)ModifierKeysEnum.CTRL;
        const int SHIFT =   (int)ModifierKeysEnum.SHIFT;
        const int ALT =     (int)ModifierKeysEnum.ALT;
        const int JAWS =    (int)ModifierKeysEnum.JAWS;
        const int WINDOWS = (int)ModifierKeysEnum.WINDOWS;
        const int INSERT  = (int)ModifierKeysEnum.INSERT;
        const int FUNCTION= (int)ModifierKeysEnum.FunctionKeys;

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
                // Single key presses representing simple letters a..z   
                case Keys.A: return PerkinsKeySequence.Create(new List<int> { 1 }); // For ComboBox only. "a" is used to represent all letters

                // Single key presses representing simple digits 0.9
                case Keys.D1: return PerkinsKeySequence.Create(new List<int> { 1, 8 }); // For ComboBox only. "1" is used to represent all letters

                // Double key presses representing conbinations of CONTROL or ALT with a simple letter. SHIFT is probably not needed !
                case Keys.Control | Keys.A: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, SPACE }, new List<int> { 1 }); // For general use. "a" is used to represent all letters
                case Keys.Alt | Keys.A: return PerkinsKeySequence.Create(new List<int> { ALT, 8, SPACE }, new List<int> { 1 }); // For general use. "a" is used to represent all letters
                                                                                                                                //testSteps.Add(TestStep.Create(Keys.Shift | Keys.A, new List<int> { SHIFT, 8, CHORD }, new List<int> { 1 })); // For general use. "a" is used to represent all letters
#if false

            // Double key presses representing conbinations of CONTROL SHIFT ALT with a simple digit. Probably not needed anyway !!!
            // testSteps.Add(TestStep.Create(Keys.Control  | Keys.D1, new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
            // testSteps.Add(TestStep.Create(Keys.Shift    | Keys.D1, new List<int> { SHIFT,8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
            // testSteps.Add(TestStep.Create(Keys.Alt      | Keys.D1, new List<int> { ALT,  8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters

            ////////////////////////////////////////////////////
            // Single key presses representing control functions
            ////////////////////////////////////////////////////
#endif
                case Keys.Space: return PerkinsKeySequence.Create(new List<int> { SPACE }); // For general use
                case Keys.Enter: return PerkinsKeySequence.Create(new List<int> { 8 }); // WORKS
                case Keys.Back: return PerkinsKeySequence.Create(new List<int> { 7 }); // WORKS
                case Keys.Escape: return PerkinsKeySequence.Create(new List<int> { 1, 5, SPACE }); // WORKS "Space E"
                case Keys.Tab: return PerkinsKeySequence.Create(new List<int> { 4, 6, SPACE }); // WORKS 
                case Keys.Home: return PerkinsKeySequence.Create(new List<int> { 2, 3, SPACE }); // WORKS
                case Keys.End: return PerkinsKeySequence.Create(new List<int> { 5, 6, SPACE }); // WORKS
                case Keys.PageUp: return PerkinsKeySequence.Create(new List<int> { 1, 2, SPACE }); // WORKS
                case Keys.PageDown: return PerkinsKeySequence.Create(new List<int> { 4, 5, SPACE }); // WORKS Reported as "Next"

                ////////////////////////////////////////////////////
                // Single key presses representing arrow-navigation
                ////////////////////////////////////////////////////
                case Keys.Right: return PerkinsKeySequence.Create(new List<int> { 6, SPACE }); // WORKS 
                case Keys.Left: return PerkinsKeySequence.Create(new List<int> { 3, SPACE }); // WORKS 
                case Keys.Up: return PerkinsKeySequence.Create(new List<int> { 1, SPACE }); // WORKS 
                case Keys.Down: return PerkinsKeySequence.Create(new List<int> { 4, SPACE }); // WORKS 

                ////////////////////////////////////////////////////
                // Double key presses representing arrow-navigation
                ////////////////////////////////////////////////////
                case Keys.Control | Keys.Right: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, SPACE }, new List<int> { 6, SPACE });   // WORKS Requires a sequence of chords!
                case Keys.Control | Keys.Left: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, SPACE }, new List<int> { 3, SPACE });   // WORKS Requires a sequence of chords!
                case Keys.Control | Keys.Up: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, SPACE }, new List<int> { 1, SPACE });   // WORKS Requires a sequence of chords!
                case Keys.Control | Keys.Down: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, SPACE }, new List<int> { 4, SPACE });   // WORKS Requires a sequence of chords!



                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Single key presses representing characters and reported as PACKET combined with the UNICODE value on KeyPressed
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                case Keys.Oemcomma: return PerkinsKeySequence.Create(new List<int> { 2 }); // For entering numric parameters. WORKS, Is reported as a PACKET as digits and alpha characters
                case Keys.OemPeriod: return PerkinsKeySequence.Create(new List<int> { 3 }); //  For entering numric parameters WORKS, Is reported as a PACKET as digits and alpha characters
                                                                                            // testSteps.Add(TestStep.Create(Keys.Multiply | Keys.Control , new List<int> { 3, 8, CHORD }, new List<int> { 3 }));
                                                                                            // Experiments show that even if {3,5} generates Keys.Multiply, { 3, 8, CHORD }, { 3, 5 } does NOT generate Keys.Mulitply | Keys.Control !! We probably need to avoid using CTRL+* !!



                default: return PerkinsKeySequence.Create(); // A PerkinsKEySequence without contents


            }
        }
        
        protected override void TestDeviceSpecificFunctions(bool all)
        { }


        protected void TestKeyCombinations(bool all)
        { }

        internal KeyboardBraillant()
        {

        }
    }
}
