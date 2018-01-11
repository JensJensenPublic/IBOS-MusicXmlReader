using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace KeyboardTest
{


    public class KeyboardFocus14 : Keyboard
    {

        // https://www.freedomscientific.com/Content/Documents/Manuals/Focus/Focus14Blue/Focus14-Blue-Users-Guide.pdf

        // Manufacturer: Freedom Scientific
        // Distributor:  Instrulog
        // Comment:  

        private string deviceName = "Focus";
        public override string DeviceName { get { return deviceName; } }

        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        public enum ModifierKeysEnum { FunctionKeys = 1, INSERT = 2, CTRL = 3, WINDOWS = 4, JAWS = 5, ALT = 6, SHIFT = 7 } // From the .pdf file above

        // Convenience definitions:
        const int SPACE     = (int)DSKey.MELLEMRUM;
        const int CHORD     = SPACE;
        const int CTRL      = (int)ModifierKeysEnum.CTRL;
        const int SHIFT     = (int)ModifierKeysEnum.SHIFT;
        const int ALT       = (int)ModifierKeysEnum.ALT;
        const int JAWS      = (int)ModifierKeysEnum.JAWS;
        const int WINDOWS   = (int)ModifierKeysEnum.WINDOWS;
        const int INSERT    = (int)ModifierKeysEnum.INSERT;
        const int FUNCTION  = (int)ModifierKeysEnum.FunctionKeys;


    /// <summary>
    /// Convenience method for creating a JAWS prolog.
    /// The implementation of this method is keyboard specific !
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    private List<Keys> Jaws(Keys keys)
        {
            return new List<Keys> { Keys.Insert, keys };
        }

        private List<int> ModifiersToPerkins(Keys keys)
        {
            List<int> result = new List<int>();
            if (0 != (keys & Keys.Control)) result.Add(CTRL);
            if (0 != (keys & Keys.Shift)) result.Add(SHIFT);
            if (0 != (keys & Keys.Alt)) result.Add(ALT);
            if (result.Count != 0)
            {
                result.Add(8);
                result.Add(CHORD);
                return result;
            }
            return null;
        }



        /// <summary>
        /// Return the keyboard-specific Perkins representation of a special JAWS sequence
        /// #warning JAWS "Toggle speech on/off" can not be executed in this way because it is not possible to send the JAWS "Insert Space"
        /// because "Space" in 2 different ways at the same time !!! 
        /// </summary>
        /// <param name="jawsKeySequence"></param>
        /// <returns></returns>
        protected override PerkinsKeySequence GetPKS(KeySequenceList jawsKeySequence)
        {
            if (jawsKeySequence == KeySequenceList.JawsToggleSpeech)    return PerkinsKeySequence.Create(new List<int> { INSERT, 8, CHORD }, new List<int> { SPACE}, new List<int> { 2, 3, 4 });
            if (jawsKeySequence == KeySequenceList.JAWSReadStatusLine)  return PerkinsKeySequence.Create(new List<int> { INSERT, 8, CHORD }, new List<int> { 5, 6, 7, CHORD });
            if (jawsKeySequence == KeySequenceList.JAWSReadTitleLine)   return PerkinsKeySequence.Create(new List<int> { INSERT, 8, CHORD }, new List<int> { 2, 3, 4, 5 });
            if (jawsKeySequence == KeySequenceList.JAWSReadMessage)     return PerkinsKeySequence.Create(new List<int> { INSERT, 8, CHORD }, new List<int> { 1, 2 });
            // Add more definitions here
            return PerkinsKeySequence.Create();
        }


        /// <summary>
        /// Return the keyboard-specific representation of a Windows.Forms.Keys value
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected override PerkinsKeySequence GetPKS(Keys keys)
        {
            switch (keys)
            {
                case Keys.A: return PerkinsKeySequence.Create(1);                   // For ComboBox only. "a" is used to represent all letters
                case Keys.D1: return PerkinsKeySequence.Create(new List<int> { 1, 8 }); // For ComboBox only. "1" is used to represent all letters
                case Keys.Control | Keys.A: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1 }); // For general use. "a" is used to represent all letters
                case Keys.Control | Keys.G: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 2, 4, 5 }); // 
                case Keys.Control | Keys.R: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 2, 3, 5 }); //
                case Keys.Control | Keys.N: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 3, 4, 5 }); // 
                case Keys.Control | Keys.M: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 3, 4 }); // 

#warning Find out what happens when ALT-A is pressed on the FOCUS keyboard.
                case Keys.Alt | Keys.A: return PerkinsKeySequence.Create(new List<int> { ALT, 8, CHORD }, new List<int> { 1 });          // For general use. "ALT-a" is used to represent all letters Something strange happens here!!!
                case Keys.Alt | Keys.F: return PerkinsKeySequence.Create(new List<int> { ALT, 8, CHORD }, new List<int> { 1, 2, 4 });    // For general use. "ALT-f" opens the "&Filer" menu
                case Keys.Alt | Keys.R: return PerkinsKeySequence.Create(new List<int> { ALT, 8, CHORD }, new List<int> { 1, 2, 3, 5 }); // For general use. "ALT-r" opens the "&Rediger" menu

                // Shortcut-keys, used by Windows or JAWS:
                case Keys.Control | Keys.O: return PerkinsKeySequence.Create(new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 3, 5 }); // Windows: Open FileOpen dialogue
                case Keys.Alt | Keys.F4:    return PerkinsKeySequence.Create(new List<int> { ALT, FUNCTION, 8, CHORD }, new List<int> { 1, 4, 5 }); // Windows: Close program ( {1,4,5} = "D" = "4" )
#warning ALT alone seems to mean something lige "Next JAWS state" when JAWS is running, but "Open Menu line" from standard keyboard when JAWS is not running
                case Keys.Alt:              return PerkinsKeySequence.Create(new List<int> { 1, 3, 4, CHORD }); // Windows: Open Menu line ( {1,3,4} = "M" ) // FAILS
                //case Keys.Tab:              return PerkinsKeySequence.Create(new List<int> { 4, 5, CHORD }); // Windows: Next Control in current form // WORKS

                ////////////////////////////////////////////////////
                // Single key presses representing control functions
                ////////////////////////////////////////////////////

                case Keys.Space:    return PerkinsKeySequence.Create(new List<int> { SPACE }); // For general use
                case Keys.Enter:    return PerkinsKeySequence.Create(new List<int> { 8 }); // WORKS
                case Keys.Back:     return PerkinsKeySequence.Create(new List<int> { 7 }); // WORKS
                case Keys.Escape:   return PerkinsKeySequence.Create(new List<int> { 1, 3, 5, 6, CHORD }); // WORKS
                case Keys.Tab:      return PerkinsKeySequence.Create(new List<int> { 4, 5, CHORD }); // WORKS ON UP
                case Keys.Home:     return PerkinsKeySequence.Create(new List<int> { 1, 3, CHORD }); // WORKS
                case Keys.End:      return PerkinsKeySequence.Create(new List<int> { 4, 6, CHORD }); // WORKS
                case Keys.PageUp:   return PerkinsKeySequence.Create(new List<int> { 2, 3, 7, CHORD }); // WORKS
                case Keys.PageDown: return PerkinsKeySequence.Create(new List<int> { 5, 6, 7, CHORD }); // WORKS Reported as "Next"
                ////////////////////////////////////////////////////
                // Single key presses representing arrow-navigation
                ////////////////////////////////////////////////////
                case Keys.Right:    return PerkinsKeySequence.Create(new List<int> { 6, CHORD }); // WORKS
                case Keys.Left:     return PerkinsKeySequence.Create(new List<int> { 3, CHORD }); // WORKS
                case Keys.Up:       return PerkinsKeySequence.Create(new List<int> { 1, CHORD }); // WORKS
                case Keys.Down:     return PerkinsKeySequence.Create(new List<int> { 4, CHORD }); // WORKS
                ////////////////////////////////////////////////////
                // Double key presses representing arrow-navigation
                ////////////////////////////////////////////////////
                case Keys.Control | Keys.Right: return PerkinsKeySequence.Create(new List<int> { 5, CHORD }); // WORKS
                case Keys.Control | Keys.Left:  return PerkinsKeySequence.Create(new List<int> { 2, CHORD }); // WORKS 
                case Keys.Control | Keys.Up:    return PerkinsKeySequence.Create(new List<int> { 3, 8, CHORD }, new List<int> { 1, CHORD }); // WORKS Requires a sequence of chords!
                case Keys.Control | Keys.Down:  return PerkinsKeySequence.Create(new List<int> { 3, 8, CHORD }, new List<int> { 4, CHORD }); // WORKS Requires a sequence of chords!
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Single key presses representing characters and reported as PACKET combined with the UNICODE value on KeyPressed
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                case Keys.Oemcomma: return PerkinsKeySequence.Create(new List<int> { 2 }); // For entering numric parameters. WORKS, Is reported as a PACKET
                // testSteps.Add(TestStep.Create(Keys.Oemcomma,  new List<int> { 6 })); // For entering numric parameters(Accordnig to .pdf) FAILS
                case Keys.OemPeriod: return PerkinsKeySequence.Create(new List<int> { 3 }); //  For entering numric parameters WORKS, Is reported as a PACKET
                //testSteps.Add(TestStep.Create(Keys.OemPeriod, new List<int> { 4,6 })); // , For entering numric parameters (Accordnig to .pdf) FAIL
                // testSteps.Add(TestStep.Create(Keys.Multiply | Keys.Control , new List<int> { 3, 8, CHORD }, new List<int> { 3 }));
                // Experiments show that even if {3,5} generates Keys.Multiply, { 3, 8, CHORD }, { 3, 5 } does NOT generate Keys.Mulitply | Keys.Control !! We probably need to avoid using CTRL+* !!

                default: return PerkinsKeySequence.Create(); // A PerkinsKEySequence without contents
            }
        }


        /// <summary>
        /// NOTE:
        /// The Numeric *,+ and - can be used for manipulating a treeview.
        /// But although the definitions below seem to generate the right Key values for *,+,- 
        /// they have no effect on the treeview in the IBOS MusicXmlReader application.
        /// Probably because the real keyboard generates KeyDown(Multiply) and KeyDown(Multiply) 
        /// while the Focus14 on  generates KeyDown(Packet) and KeyDown(Packet)
        /// Both keyboards generate identical information on KeyPress()*
        /// 
        /// Fortunately the Treeview can be manipulated in a similar way by the Navigation buttone Up, Down,LEft, Right !!
        /// 
        /// </summary>
        /// <param name="all"></param>
        protected  override void TestMusicXmlReaderOptionalKeys(bool all)
        {
            string functionName = MethodInfo.GetCurrentMethod().Name.ToString();
            //AddCaption(functionName);

            List<TestStep> localSteps = new List<TestStep>();

            // For manipulating treeviews
            localSteps.Add(TestStep.Create(Keys.Multiply));//,   new List<int> { 3, 5 }));    // Standard Braille WORKS
            // localSteps.Add(TestStep.Create(Keys.Multiply,new List<int> { (int) DSKey.RightShiftkey12b, 5 }));   // Focus 14 doc: Rightshift, 3 FAILS
            if (!all) return;
            localSteps.Add(TestStep.Create(Keys.Add));//,        new List<int> { 2, 3, 5, 8 })); // Standard Braille is {2,3,5} WORKS–
            // localSteps.Add(TestStep.Create(Keys.Add,     new List<int> { (int)DSKey.RightShiftkey12b, 8 }));   // Focus 14 doc: Rightshift, 8 FAILS
            // Focus 14 doc: Right shift 8
            // localSteps.Add(TestStep.Create(Keys.Subtract,new List<int> { 2, 5 }));    // Standard Braille FAILS
            localSteps.Add(TestStep.Create(Keys.Subtract));//,   new List<int> { 3, 6 ,8 }));    // Experimwnts ! Generates Hex= 2D as Numeric Subtract!
            // Focus 14 doc: ??
            //CheckImplementation(className, functionName, localSteps, IbosMusicXmlReaderKeys.optionalKeys, IbosMusicXmlReaderKeys.keySequences);
            TestSteps.AddRange(localSteps);
        }









        /// <summary>
        /// This function can be regarded as a repository of device specific functions that are not needed by teh IBOS MusicXmlReader for the moment,
        /// but may required later.
        /// </summary>
        /// <param name="all"></param>
        protected override void TestDeviceSpecificFunctions(bool all)
        {
            // Table of Multi  Control Functions
            // *********************************
            // Function                    Command
            // Enable Auto Advance Mode    LEFT SELECT + RIGHT SELECT
            // Decrease Auto Advance Speed LEFT SELECT
            // Increase Auto Advance Speed RIGHT SELECT
            // Right Mouse Click           PANNING BUTTON+CURSOR ROUTER BUTTON
            // Control + Left Mouse Click  ROUTER BUTTON CHORD
            // Page Down                   LEFT or RIGHT SELECT + ROCKER DOWN
            // Page Up                     LEFT or RIGHTSELECT + ROCKER UP
            // Top of File                 LEFT PANNING BUTTON + SELECT BUTTON
            // Bottom of File              RIGHT PANNING BUTTON + SELECT BUTTON
            // End                         PANNING BUTTON + ROCKER BAR DOWN
            // Home                        PANNING BUTTON + ROCKER BAR UP
            // Next Line                   ROCKER BAR DOWN
            // Prior Line                  ROCKER BAR UP
            // Pan Left                    LEFT PANNING BUTTON
            // Pan Right                   RIGHT PANNING BUTTON
            // Select Text                 LEFT SHIFT + CURSOR ROUTER BUTTON
            // Select Block                RIGHT SHIFT + CURSOR ROUTER BUTTON (at beginning of block; repeat at end of block)
            // Toggle NAV Rockers On / Off LEFT or RIGHT MODEBUTTON CHORD

            //int CHORD = (int)DSKey.MELLEMRUM;
            //int RIGHTSHIFT = (int)DSKey.RightShiftkey12b;
            TestSteps.Add(TestStep.Create(Keys.Home));//, new List<int>() { (int)DSKey.LeftPanningButton11a, (int)DSKey.LeftRockerBar10aUp }));
            if (!all) return;
            /*
            testSteps.Add(TestStep.Create(Keys.Home, new List<int>() { (int)DSKey.RightPanningButton11b, (int)DSKey.RightRockerBar10bUp }));  // WORKS
            testSteps.Add(TestStep.Create(Keys.End, new List<int>() { (int)DSKey.LeftPanningButton11a, (int)DSKey.LeftRockerBar10aDown }));  // WORKS
            testSteps.Add(TestStep.Create(Keys.End, new List<int>() { (int)DSKey.RightPanningButton11b, (int)DSKey.RightRockerBar10bDown })); // WORKS
            testSteps.Add(TestStep.Create(Keys.Down, new List<int>() { (int)DSKey.LeftRockerBar10aDown }));  // WORKS
            testSteps.Add(TestStep.Create(Keys.Down, new List<int>() { (int)DSKey.RightRockerBar10bDown })); // FAILS
            testSteps.Add(TestStep.Create(Keys.Up, new List<int>() { (int)DSKey.LeftRockerBar10aUp }));    // WORKS
            testSteps.Add(TestStep.Create(Keys.Up, new List<int>() { (int)DSKey.RightRockerBar10bUp }));   // FAILS
            // Text Selection commands
            const int CHORD = (int)DSKey.SPACE; // Shorthand for the "CHORD" key on the Focus14 device.
            testSteps.Add(TestStep.Create(Keys.Shift | Keys.Left, new List<int> { 3, 7, CHORD })); // Select Prior Chaarcter  
            testSteps.Add(TestStep.Create(Keys.Shift | Keys.Right, new List<int> { 6, 7, CHORD })); // Select Prior Chaarcter  
            testSteps.Add(TestStep.Create(Keys.Shift | Keys.Up, new List<int> { 1, 7, CHORD })); // 
            testSteps.Add(TestStep.Create(Keys.Shift | Keys.Down, new List<int> { 4, 7, CHORD })); // 
            testSteps.Add(TestStep.Create(Keys.Control | Keys.Shift | Keys.Left, new List<int> { 2, 7, CHORD })); //  
            testSteps.Add(TestStep.Create(Keys.Control | Keys.Shift | Keys.Right, new List<int> { 5, 7, CHORD })); //     

            testSteps.Add(TestStep.Create(Keys.Shift | Keys.Up, new List<int> { (int)DSKey.LeftShiftKey12a, (int)DSKey.LeftRockerBar10aUp }));   // WORKS
            testSteps.Add(TestStep.Create(Keys.Shift | Keys.Down, new List<int> { (int)DSKey.LeftShiftKey12a, (int)DSKey.LeftRockerBar10aDown })); // WORKS
            testSteps.Add(TestStep.Create(Keys.Control | Keys.Up, new List<int> { (int)DSKey.RightShiftkey12b, (int)DSKey.LeftRockerBar10aUp }));   // WORKS
            testSteps.Add(TestStep.Create(Keys.Control | Keys.Down, new List<int> { (int)DSKey.RightShiftkey12b, (int)DSKey.LeftRockerBar10aDown })); // WORKS

            //testSteps.Add(TestStep.Create(, new List<int> { })); //  
            //testSteps.Add(TestStep.Create(, new List<int> { })); //  
            //testSteps.Add(TestStep.Create(, new List<int> { })); //  
            //testSteps.Add(TestStep.Create(, new List<int> { })); //  
            //testSteps.Add(TestStep.Create(, new List<int> { })); //  
            //testSteps.Add(TestStep.Create(, new List<int> { })); //  
            //testSteps.Add(TestStep.Create(, new List<int> { })); //  

            */

            TestSteps.Add(TestStep.Create(Keys.Multiply));//, new List<int> { 3, 5 })); // WORKS
            TestSteps.Add(TestStep.Create(Keys.Multiply));// | Keys.Control, new List<int> { 3, 8, CHORD }, new List<int> { 3, 5 })); // FAILS!!!


            // Special Keys
            //************
            // Use these keystrokes to simulate certain keys that are not available on the
            // Focus braille keyboard. These keys can be combined with the modifier keys
            // described previously.Punctuation and other symbols will be entered using their
            // contracted braille equivalents if Contracted Braille Translation is set to Input
            // and Output. For your convenience, both keystrokes and braille dot patterns are
            // provided.If no dot pattern equivalent is available, a dash appears in the table cell.
            //*************
            //
            //  ESC RIGHTSHIFT+DOT 1 or Z CHORD or RIGHTHIFT+DOT 1 or DOTS 1-3-5-6  CHORD
            TestSteps.Add(TestStep.Create(Keys.Escape));//, new List<int> { 1, 3, 5, 6, CHORD })); // WORKS
            // ALT RIGHTSHIFT + DOT 2
            TestSteps.Add(TestStep.Create(Keys.Alt));//, new List<int> { RIGHTSHIFT, 2 })); // WORKS
            // APPLICATION Key RIGHTSHIFT+DOT 2 CHORD
            TestSteps.Add(TestStep.Create(Keys.Apps));//, new List<int> { RIGHTSHIFT, 2, CHORD })); // "SYSTEM"
            // NUM PAD ASTERISK RIGHTSHIFT + DOT 3
            TestSteps.Add(TestStep.Create(Keys.Multiply));//, new List<int> { RIGHTSHIFT, 3 })); // FAILS
            // WINDOWS Key RIGHT SHIFT+DOT 4
            TestSteps.Add(TestStep.Create(Keys.LWin));//, new List<int> { RIGHTSHIFT, 4 })); // WORKS, Opens Windows dialog
            // NUM PAD SLASH RIGHTSHIFT+DOT 7
            TestSteps.Add(TestStep.Create(Keys.Divide));//, new List<int> { RIGHTSHIFT, 7 })); // fails
            // CAPS LOCK RIGHT SHIFT+DOT 7 CHORD 
            TestSteps.Add(TestStep.Create(Keys.CapsLock));//, new List<int> { RIGHTSHIFT, 7, CHORD })); // WORKS
            // NUM PAD PLUS RIGHTSHIFT + DOT 8
            TestSteps.Add(TestStep.Create(Keys.Add));//, new List<int> { RIGHTSHIFT, 8 })); // FAILS
            //BACKSPACE DOT 7 -
            TestSteps.Add(TestStep.Create(Keys.Back));//, new List<int> { RIGHTSHIFT, 7 })); // FAILS
                                                      //ENTER DOT 8
            TestSteps.Add(TestStep.Create(Keys.Enter));//, new List<int> { 8 })); // WORKS
                                                       //CTRL + BACKSPACE DOTS 1 - 2 - 3 - 4 - 5 - 6 - 7 CHORD
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.Back));//, new List<int> { 1,2,3,4,5,6,7,CHORD }));  // WORKS
            // TAB DOTS 4 - 5 CHORD
            TestSteps.Add(TestStep.Create(Keys.Tab));//, new List<int> { 4,5,CHORD })); // TAB
            // SHIFT+TAB DOTS 1-2 CHORD
            TestSteps.Add(TestStep.Create(Keys.Shift | Keys.Tab));//, new List<int> { 1,2,CHORD})); // WORKS
            // HOME DOTS 1 - 3  CHORD
            TestSteps.Add(TestStep.Create(Keys.Home));//, new List<int> { 1, 3, CHORD })); // WORKS
            // END DOTS 4 - 6 CHORD
            TestSteps.Add(TestStep.Create(Keys.End));//, new List<int> { 4, 6, CHORD })); // WORKS
            // PAGE UP LEFT SELECT+ROCKER BAR UP or RIGHT SELECT + ROCKER BAR UP  or DOTS 2 - 3 - 7 CHORD
            TestSteps.Add(TestStep.Create(Keys.PageUp));//, new List<int> { 2,3,7, CHORD })); // WORKS
            // PAGE DOWN LEFT SELECT+ROCKER BAR DOWN or RIGHT SELECT + ROCKER BAR DOWN or DOTS 5 - 6 - 7  CHORD
            TestSteps.Add(TestStep.Create(Keys.PageDown));//, new List<int> { 5,6,7, CHORD })); // WORKS
            // DELETE FOR CHORD DOTS 1 - 2 - 3 - 4 - 5 - 6 CHORD
            TestSteps.Add(TestStep.Create(Keys.Delete));//, new List<int> { 1, 2, 3,4,5,6,CHORD })); // WORKS (Numeric Delete or Command.Delete) 
            // EQUALS DOTS 1 - 2 - 3 - 4 - 5 - 6 -
            // TestSteps.Add(TestStep.Create(Keys.Equal, new List<int> { 1, 2, 3, 4, 5, 6 })); // FAILS
            // RIGHT BRACKET DOTS 1 - 2 - 4 - 5 - 6 - 7 -
            //TestSteps.Add(TestStep.Create(Keys.R, new List<int> { 1, 2, 4, 5, 6, 7 })); //,ÏÏ
            // LEFT BRACKET DOTS 2 - 4 - 6 - 7 -
            // ??
            //BACKSLASH DOTS 1 - 2 - 5 - 6 - 7 -
            // ?
            //SLASH DOTS 3 - 4 -
            TestSteps.Add(TestStep.Create(Keys.Divide));//, new List<int> { 3,4})); //
                                                        // LEFT PARENTHESIS DOTS 1 - 2 - 3 - 5 - 6 -
                                                        // ?
                                                        // RIGHT PARENTHESIS DOTS 2 - 3 - 4 - 5 - 6 -
                                                        // ?
                                                        // APOSTROPHE DOT 3 -
                                                        // ?
                                                        // DASH DOTS 3 - 6 -
                                                        // ?
                                                        // GRÀVE DOT 4 -
                                                        // ?
                                                        // PERIOD DOTS 4 - 6 -
                                                        // ?
                                                        // QUESTION MARK DOTS 1 - 4 - 5 - 6 -
                                                        // ? 
                                                        // EXCLAMATION MARK DOTS 2 - 3 - 4 - 6 -
                                                        // ?             
                                                        // SEMICOLON DOTS 5 - 6 -
                                                        // ?
                                                        //COMMA DOT 6 -
                                                        // ?


            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Control keys to be used in general 2-key combination. The control key must be followed by an other key 
            // For instance to generate Keys.Control | Keys.A press {3,8,CHORD} followed by {1}
            /////////////////////////////////////////////////////////////////////////////////////////////////////////


            //TestSteps.Add(TestStep.Create(Keys.,      new List<int> { FUNCTION,8,CHORD })); // "1" WORKS To be used with the following key!
            //TestSteps.Add(TestStep.Create(Keys.Insert,new List<int> { INSERT,8, CHORD }));  // "2" Do not test, it will affect JAWS operation  
            TestSteps.Add(TestStep.Create(Keys.Control));//, new List<int> { CTRL, 8, CHORD }));    // "3" WORKS To be used with the following key!  WORKS
            //TestSteps.Add(TestStep.Create(Keys.LWin,  new List<int> { WINDOWS,8, CHORD })); // "4" WORKS To be used with the following key! 
            //TestSteps.Add(TestStep.Create(Keys.JAWS,  new List<int> { JAWS, 8,CHORD }));    // "5" Do not test, it will affect JAWS operation                                                                                                  // "5" = JAWS 
            TestSteps.Add(TestStep.Create(Keys.Alt));//, new List<int> { ALT, 8, CHORD }));    // "6" To be used with the following key!        WORKS
            TestSteps.Add(TestStep.Create(Keys.Shift));//, new List<int> { SHIFT, 8, CHORD }));  // "7" WORKS To be used with the following key!  WORKS



            //////////////////////
            // 2-key combination 
            //////////////////////

            TestSteps.Add(TestStep.Create(Keys.Add | Keys.Shift));//, new List<int> { })); // 
            TestSteps.Add(TestStep.Create(Keys.Shift | Keys.Tab));//, new List<int> { 1, 2, CHORD })); // WORKS ON UP
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.PageUp));//, new List<int> { 2, 3, 7, CHORD })); // WORKS
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.PageDown));//, new List<int> { 5, 6, 7, CHORD })); // WORKS
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.Multiply));//, new List<int> { })); // 
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.Tab));//, new List<int> { 5, 6, CHORD })); // WORKS


            //////////////////////
            // 3-key combination 
            //////////////////////

            TestSteps.Add(TestStep.Create(Keys.Control | Keys.Shift | Keys.Up));//, new List<int> { CTRL, SHIFT, 8, CHORD }, new List<int> { 1, CHORD })); // WORKS
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.Shift | Keys.Down));//, new List<int> { CTRL, SHIFT, 8, CHORD }, new List<int> { 4, CHORD })); //  WORKS
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.Shift | Keys.P));//, new List<int> { CTRL, SHIFT, 8, CHORD }, new List<int> { 1, 2, 3, 4 })); // Stop Playing WORKS

            //////////////////////
            // 0-key combination 
            //////////////////////

            TestSteps.Add(TestStep.Create(Keys.None));//, new List<int> { 7, CHORD })); // For completeness only !!
            TestSteps.Add(TestStep.Create(Keys.None));//, new List<int> { 8, CHORD })); // For completeness only !!


            //////////////////////
            // Sequences 
            //////////////////////

            TestSteps.Add(TestStep.Create(Keys.Alt | Keys.F4));//, new List<int> { (int)ModifierKeysEnum.FunctionKeys, (int)ModifierKeysEnum.ALT, 8, CHORD }, new List<int> { 1, 4, 5 })); // (Exit program)




        }

        /*
             


         
              
         



                Text Selection Commands
                ************************
                Use these keystrokes to perform various text selection commands. Both
                keystrokes and braille dot patterns are provided in the table. If no dot pattern
                equivalent is available, a dash appears in the table cell.
                Description Keystroke DOTS Pattern
                Select Prior
                Character
                DOTS 3-7 CHORD -
                32
                Description Keystroke DOTS Pattern
                Select Next
                Character
                DOTS 6-7 CHORD -
                Select Prior Word DOTS 2-7 CHORD -
                Select Next Word DOTS 5-7 CHORD -
                Select Prior Line DOTS 1-7 CHORD -
                Select Next Line DOTS 4-7 CHORD -
                Select Prior Screen LEFT SHIFT+K LEFT
                SHIFT+DOTS 1-3
                Select Next Screen LEFT SHIFT+DOTS 4-6 -
                Select from Start of
                Line
                K+DOT 7 CHORD DOTS 1-3-7
                CHORD
                Select to End of
                Line
                DOTS 4-6-7 CHORD --
                Select from Top L+DOTS 7 CHORD DOTS 1-2-3-7
                CHORD
                Select to Bottom DOTS 4-5-6-7 CHORD -
                Move To Beginning
                Of Line
                ROCKER BAR UP+PANNING
                BUTTON
                -
                Move To End Of
                Line
                ROCKER BAR
                DOWN+PANNING BUTTON 

                */


        private enum ControlKeyEnum {INSERT=2,CONTROL=3,WINDOWS=4,JAWS=5,ALT=6,SHIFT=7 }; // FOCUS 14 specific values!


        protected void TestKeyCombinations(bool all)
        {
            //const int CHORD = (int) DSKey.MELLEMRUM; // Shorthand for the "CHORD" key on the Focus14 device.
            TestSteps.Add(TestStep.Create(Keys.Control | Keys.A));//, new List<int>() { 8, (int)ControlKeyEnum.CONTROL, CHORD }, new List<int>() { 1 })); // WORKS
            if (!all) return;
            TestSteps.Add(TestStep.Create(Keys.Shift   | Keys.A));//, new List<int>() { 8, (int)ControlKeyEnum.SHIFT, CHORD },   new List<int>() { 1 })); // WORKS
            TestSteps.Add(TestStep.Create(Keys.Alt     | Keys.A));//, new List<int>() { 8, (int)ControlKeyEnum.ALT, CHORD },     new List<int>() { 1 })); // WORKS
            //TestSteps.Add(TestStep.Create(Keys.LWin    | Keys.A, new List<int>() { 8, (int)ControlKeyEnum.WINDOWS, CHORD }, new List<int>() { 1 })); // FAILS: Trigs "Ny Meddelelse"
            //TestSteps.Add(TestStep.Create(Keys.Insert  | Keys.A, new List<int>() { 8, (int)ControlKeyEnum.INSERT, CHORD },  new List<int>() { 1 }));  // FAILS: "Subtract", Starts strange loop- sequence !!
        }
 
        internal KeyboardFocus14() : base()
        {
        }

    }
}
