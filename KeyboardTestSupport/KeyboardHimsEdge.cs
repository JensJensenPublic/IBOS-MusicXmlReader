using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace KeyboardTest
{
    // http://mielke.cc/brltty/doc/KeyBindings/brl-hm-edge.html

    public class KeyboardHimsEdge : Keyboard
    {

        // Manufacturer: Hims
        // Distributor:  Instrulog
        // Comment:  

        private string deviceName = "Hims Edge";
        public override string DeviceName { get { return deviceName; } }

        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        enum ModifierKeysEnum { FunctionKeys = 1, INSERT = 2, CTRL = 3, WINDOWS = 4, JAWS = 5, ALT = 6, SHIFT = 7 } // From the .pdf file above

        // Convenience definitions:
        const int SPACE = (int)DSKey.MELLEMRUM;
        const int CHORD = SPACE;
        const int CTRL = (int)DSKey.EdgeCTRL;
        const int SHIFT = (int)DSKey.EdgeSHIFT;
        const int ALT = (int)DSKey.EdgeALT;
        const int JAWS = (int)DSKey.EdgeINSERT;
        const int WINDOWS = (int)DSKey.EdgeWINDOWS;
        const int INSERT = (int)DSKey.EdgeINSERT;


        /// <summary>
        /// Convenience method for creating a JAWS prolog
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private List<Keys> Jaws(Keys keys)
        {
            return new List<Keys> { Keys.Insert, keys };
        }

        protected override PerkinsKeySequence GetPKS(KeySequenceList jawsKeySequence)
        {
            if (jawsKeySequence == KeySequenceList.JawsToggleSpeech) return PerkinsKeySequence.Create();
            if (jawsKeySequence == KeySequenceList.JAWSReadStatusLine) return PerkinsKeySequence.Create();
            if (jawsKeySequence == KeySequenceList.JAWSReadTitleLine) return PerkinsKeySequence.Create();
            if (jawsKeySequence == KeySequenceList.JAWSReadMessage) return PerkinsKeySequence.Create();
            // Add more definitions here
            return PerkinsKeySequence.Create();
        }

        protected override PerkinsKeySequence GetPKS(Keys keys)
        {
            switch (keys)
            {
                case Keys.A: return PerkinsKeySequence.Create(new List<int> { 1 }); // For ComboBox only. "a" is used to represent all letters


                // Single key presses representing simple digits 0.9
                case Keys.D1: return PerkinsKeySequence.Create(new List<int> { 1, 8 }); // For ComboBox only. "1" is used to represent all letters

                // Double key presses representing conbinations of CONTROL or ALT with a simple letter. SHIFT is probably not needed !
                case Keys.Control | Keys.A: return PerkinsKeySequence.Create(new List<int> { CTRL, 1 });    // For general use. "a" is used to represent all letters
                case Keys.Control | Keys.G: return PerkinsKeySequence.Create();
                case Keys.Control | Keys.R: return PerkinsKeySequence.Create();
                case Keys.Control | Keys.N: return PerkinsKeySequence.Create();
                case Keys.Control | Keys.M: return PerkinsKeySequence.Create();

                case Keys.Alt | Keys.A: return PerkinsKeySequence.Create(new List<int> { ALT, 1 });         // For general use. "a" is used to represent all letters
                                                                                                                                //testSteps.Add(TestStep.Create(Keys.Shift | Keys.A, new List<int> { SHIFT, 8, CHORD }, new List<int> { 1 })); // For general use. "a" is used to represent all letters
                                                                                                                                // Shortcut-keys, used by Windows or JAWS:
                case Keys.Control | Keys.O: return PerkinsKeySequence.Create(); // Windows: Open FileOpen dialogue                                                                                                     
         //       case Keys.Alt | Keys.F4: return PerkinsKeySequence.Create(new List<int> { ALT, FUNCTION, 8, CHORD }, new List<int> { 1, 4, 5 }); // Windows: Close program ( {1,4,5} = "D" = "4" )                                                                                                                 
                case Keys.Alt: return PerkinsKeySequence.Create(); // Windows: Open Menu line ( {1,3,4} = "M" ) // FAILS
                case Keys.Tab: return PerkinsKeySequence.Create(); // Windows: Next Control in current form // WORKS

                // Note: Insert is not represented as a simple flag as CONTROL, ALT and DELETE, so we need to express combinations with Insert with another key as 2 Keys !
                //       This is accomplished by the Jaws() convenience method.

                // Double key presses representing conbinations of CONTROL SHIFT ALT with a simple digit. Probably not needed anyway !!!
                // testSteps.Add(TestStep.Create(Keys.Control  | Keys.D1, new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
                // testSteps.Add(TestStep.Create(Keys.Shift    | Keys.D1, new List<int> { SHIFT,8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
                // testSteps.Add(TestStep.Create(Keys.Alt      | Keys.D1, new List<int> { ALT,  8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters

                ////////////////////////////////////////////////////
                // Single key presses representing control functions
                ////////////////////////////////////////////////////

                case Keys.Space: return PerkinsKeySequence.Create(new List<int> { SPACE }); 
                case Keys.Enter: return PerkinsKeySequence.Create(new List<int> { 8 });                     
                case Keys.Back: return PerkinsKeySequence.Create(new List<int> { 7 });                      
                case Keys.Escape: return PerkinsKeySequence.Create(new List<int> { 1, 5, CHORD }); //         
                case Keys.Tab | Keys.Shift: return PerkinsKeySequence.Create();
                case Keys.Home: return PerkinsKeySequence.Create();
                case Keys.End: return PerkinsKeySequence.Create();
                case Keys.PageUp: return PerkinsKeySequence.Create();
                case Keys.PageDown: return PerkinsKeySequence.Create(); 
                ////////////////////////////////////////////////////
                // Single key presses representing arrow-navigation
                ////////////////////////////////////////////////////

                case Keys.Right: return PerkinsKeySequence.Create(new List<int> { 6, CHORD }); // WORKS
                case Keys.Left: return PerkinsKeySequence.Create(new List<int> { 3, CHORD }); // WORKS
                case Keys.Up: return PerkinsKeySequence.Create(new List<int> { 1, CHORD }); // WORKS
                case Keys.Down: return PerkinsKeySequence.Create(new List<int> { 4, CHORD }); // WORKS
                ////////////////////////////////////////////////////
                // Double key presses representing arrow-navigation
                ////////////////////////////////////////////////////
                case Keys.Control | Keys.Right: return PerkinsKeySequence.Create(new List<int> { 5, CHORD }); // WORKS
                case Keys.Control | Keys.Left: return PerkinsKeySequence.Create(new List<int> { 2, CHORD }); // WORKS 
                case Keys.Control | Keys.Up: return PerkinsKeySequence.Create(); // WORKS Requires a sequence of chords!
                case Keys.Control | Keys.Down: return PerkinsKeySequence.Create(); // WORKS Requires a sequence of chords!

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Single key presses representing characters and reported as PACKET combined with the UNICODE value on KeyPressed
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case Keys.Oemcomma: return PerkinsKeySequence.Create(new List<int> { 2 }); // For entering numric parameters. WORKS, Is reported as a PACKET
                case Keys.OemPeriod: return PerkinsKeySequence.Create(new List<int> { 3 }); //  For entering numric parameters WORKS, Is reported as a PACKET          
                // testSteps.Add(TestStep.Create(Keys.Multiply | Keys.Control , new List<int> { 3, 8, CHORD }, new List<int> { 3 }));
                // Experiments show that even if {3,5} generates Keys.Multiply, { 3, 8, CHORD }, { 3, 5 } does NOT generate Keys.Mulitply | Keys.Control !! We probably need to avoid using CTRL+* !!

                default: return PerkinsKeySequence.Create(); // A PerkinsKEySequence without contents
            }
        }

        internal KeyboardHimsEdge()
        {
        }

    }
}
