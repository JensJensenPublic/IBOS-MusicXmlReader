using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace KeyboardTest
{
    public class TestDefinitions
    {

        static public TestDefinitions Create()
        {
            return new TestDefinitions();
        }

 

        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
  

        /// <summary>
        /// Reflects all key combinations needed by the IBOS MusicXmlREader program.
        /// Must reflect the definitions in 
        /// C:\Users\Jens\Dropbox\Visual Studio 2015\Solutions\Tactile MusicXmlReader\MusicXmlReader\ShortcutHandler.cs
        /// 
        /// The actual Prkins Key sequences used during the test are often device-specific and are implemented in the deviceSpecific GetPKS(Keys keys) method!
        /// 
        /// NOTE:   Passing this test will not guarantee that the keyboard can be used for input to all Windows Forms controls
        ///         because different controls may react differently!
        /// 
        /// </summary>
        /// <param name="all"></param>
        public List<TestStep> TestMusicXmlReaderKeys(bool all)
        {
            string functionName = MethodInfo.GetCurrentMethod().Name.ToString();
            //AddCaption(functionName);
            List<TestStep> localSteps = new List<TestStep>();

            localSteps.Add(TestStep.Create(functionName));

            // Single key presses representing simple letters a..z   
            localSteps.Add(TestStep.Create(TargetControl.ComboBox, Keys.A));
            if (!all) return localSteps;

            // Single key presses representing simple digits 0.9
            localSteps.Add(TestStep.Create(TargetControl.ComboBox, Keys.D1)); // "1" is used to represent all letters

            // Double key presses representing conbinations of CONTROL or ALT with a simple letter. SHIFT is probably not needed !
            localSteps.Add(TestStep.Create(Keys.Control | Keys.A)); // For general use. "a" is used to represent all letters
            localSteps.Add(TestStep.Create(Keys.Control | Keys.G));
            localSteps.Add(TestStep.Create(Keys.Control | Keys.R));
            localSteps.Add(TestStep.Create(Keys.Control | Keys.N));
            //            localSteps.Add(TestStep.Create(Keys.Control | Keys.K, new List<int> { CTRL, 8, CHORD }, new List<int> {1,3 })); // 
            localSteps.Add(TestStep.Create(Keys.Control | Keys.M));



#warning Find out what happens when ALT-A is pressed on the FOCUS keyboard.
            localSteps.Add(TestStep.Create(Keys.Alt | Keys.A));// For general use. "ALT-a" is used to represent all letters Something strange happens here!!!
            localSteps.Add(TestStep.Create(Keys.Alt | Keys.F)); // For general use. "ALT-f" opens the "&Filer" menu
            localSteps.Add(TestStep.Create(Keys.Alt | Keys.R)); // For general use. "ALT-r" opens the "&Rediger" menu
            //testSteps.Add(TestStep.Create(Keys.Shift | Keys.A, new List<int> { SHIFT, 8, CHORD }, new List<int> { 1 })); // For general use. "a" is used to represent all letters

            // Shortcut-keys, used by Windows or JAWS:
            localSteps.Add(TestStep.Create(Keys.Control | Keys.O)); // Windows: Open FileOpen dialogue
            localSteps.Add(TestStep.Create(Keys.Alt | Keys.F4)); // Windows: Close program ( {1,4,5} = "D" = "4" )
#warning ALT alone seems to mean something lige "Next JAWS state" when JAWS is running, but "Open Menu line" from standard keyboard when JAWS is not running
            localSteps.Add(TestStep.Create(Keys.Alt)); // Windows: Open Menu line ( {1,3,4} = "M" ) // FAILS
            localSteps.Add(TestStep.Create(Keys.Tab));// Windows: Next Control in current form // WORKS

            // Note: Insert is not represented as a simple flag as CONTROL, ALT and DELETE, so we need to express combinations with Insert with another key as 2 Keys !
            //       This is accomplished by the Jaws() convenience method.
#warning JAWS "Toggle speech on/off" can not be executed in this way because it is not possible to send the JAWS "Insert Space" because "Space" in 2 different ways at the same time !!! 
            localSteps.Add(TestStep.Create("JAWS Slå tale fra/til ", KeySequenceList.JawsToggleSpeech)); // FAILS !JAWS: Toggle speech On/Off // FAILS
            localSteps.Add(TestStep.Create("JAWS Oplæs titellinie ", KeySequenceList.JAWSReadTitleLine)); // WORKS!  JAWS: Read Title line ( {2,3,4,5} = T)
            localSteps.Add(TestStep.Create("JAWS Oplæs statuslinie", KeySequenceList.JAWSReadStatusLine)); // WORKS!  JAWS: Read STATUS line ( {5,6,7,CHORD} =PageDown )
            localSteps.Add(TestStep.Create("JAWS Oplæs meddelelse ", KeySequenceList.JAWSReadMessage)); // WORKS! JAWS: Read Message  ( {1,2} = B) 


            // Double key presses representing conbinations of CONTROL SHIFT ALT with a simple digit. Probably not needed anyway !!!
            // testSteps.Add(TestStep.Create(Keys.Control  | Keys.D1, new List<int> { CTRL, 8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
            // testSteps.Add(TestStep.Create(Keys.Shift    | Keys.D1, new List<int> { SHIFT,8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters
            // testSteps.Add(TestStep.Create(Keys.Alt      | Keys.D1, new List<int> { ALT,  8, CHORD }, new List<int> { 1, 8 })); // For general use. "1" is used to represent all letters

            ////////////////////////////////////////////////////
            // Single key presses representing control functions
            ////////////////////////////////////////////////////

            localSteps.Add(TestStep.Create(Keys.Space));
            localSteps.Add(TestStep.Create(Keys.Enter));
            localSteps.Add(TestStep.Create(Keys.Back));
            localSteps.Add(TestStep.Create(Keys.Escape));
            localSteps.Add(TestStep.Create(Keys.Tab));
            localSteps.Add(TestStep.Create(Keys.Home));
            localSteps.Add(TestStep.Create(Keys.End));
            localSteps.Add(TestStep.Create(Keys.PageUp));
            localSteps.Add(TestStep.Create(Keys.PageDown));
            ////////////////////////////////////////////////////
            // Single key presses representing arrow-navigation
            ////////////////////////////////////////////////////
            localSteps.Add(TestStep.Create(Keys.Right));
            localSteps.Add(TestStep.Create(Keys.Left));
            localSteps.Add(TestStep.Create(Keys.Up));
            localSteps.Add(TestStep.Create(Keys.Down));
            ////////////////////////////////////////////////////
            // Double key presses representing arrow-navigation
            ////////////////////////////////////////////////////
            localSteps.Add(TestStep.Create(Keys.Control | Keys.Right));
            localSteps.Add(TestStep.Create(Keys.Control | Keys.Left));
            localSteps.Add(TestStep.Create(Keys.Control | Keys.Up));
            localSteps.Add(TestStep.Create(Keys.Control | Keys.Down));


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Single key presses representing characters and reported as PACKET combined with the UNICODE value on KeyPressed
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            localSteps.Add(TestStep.Create(Keys.Oemcomma)); // For entering numric parameters. WORKS, Is reported as a PACKET
            // testSteps.Add(TestStep.Create(Keys.Oemcomma,  new List<int> { 6 })); // For entering numric parameters(Accordnig to .pdf) FAILS
            localSteps.Add(TestStep.Create(Keys.OemPeriod)); //  For entering numric parameters WORKS, Is reported as a PACKET
            //testSteps.Add(TestStep.Create(Keys.OemPeriod, new List<int> { 4,6 })); // , For entering numric parameters (Accordnig to .pdf) FAIL
            // testSteps.Add(TestStep.Create(Keys.Multiply | Keys.Control , new List<int> { 3, 8, CHORD }, new List<int> { 3 }));
            // Experiments show that even if {3,5} generates Keys.Multiply, { 3, 8, CHORD }, { 3, 5 } does NOT generate Keys.Mulitply | Keys.Control !! We probably need to avoid using CTRL+* !!

            return localSteps;       
        }




        //****************************************************************************************
        // TestSimpleLetters, TestSimpleDigits and TestPunctuationMarks define the sets of Keys
        // That mus be tested for Letters, Digits and PunctuationMArks respectively.
        //****************************************************************************************


        public List<TestStep> TestSimpleLetters(bool all)
        {
            string functionName = "TestSimpleLetters";
            List<TestStep> localSteps = new List<TestStep>();
            localSteps.Add(TestStep.Create(functionName));
            localSteps.Add(TestStep.Create(Keys.A));// , new List<int>() { 1 }));  // Generate "KeyDown Packet Value=231" , "KeyPress Dec=97 Hex=61 CHAR16=a" "KeyUp Packet Value=231"
            if (!all) return localSteps;
            localSteps.Add(TestStep.Create(Keys.B));//, new List<int>() { 1, 2 }));
            localSteps.Add(TestStep.Create(Keys.C));//, new List<int>() { 1, 4 }));
            localSteps.Add(TestStep.Create(Keys.D));//, new List<int>() { 1, 4, 5 }));
            localSteps.Add(TestStep.Create(Keys.E));//, new List<int>() { 1, 5 }));
            localSteps.Add(TestStep.Create(Keys.F));//, new List<int>() { 1, 2, 4 }));
            localSteps.Add(TestStep.Create(Keys.G));//, new List<int>() { 1, 2, 4, 5 }));
            localSteps.Add(TestStep.Create(Keys.H));//, new List<int>() { 1, 2, 5 }));
            localSteps.Add(TestStep.Create(Keys.I));//, new List<int>() { 2, 4 }));
            localSteps.Add(TestStep.Create(Keys.J));//, new List<int>() { 2, 4, 5 }));
            localSteps.Add(TestStep.Create(Keys.K));//, new List<int>() { 1, 3 }));
            localSteps.Add(TestStep.Create(Keys.L));//, new List<int>() { 1, 2, 3 }));
            localSteps.Add(TestStep.Create(Keys.M));//, new List<int>() { 1, 3, 4 }));
            localSteps.Add(TestStep.Create(Keys.N));//, new List<int>() { 1, 3, 4, 5 }));
            localSteps.Add(TestStep.Create(Keys.O));//, new List<int>() { 1, 3, 5 }));
            localSteps.Add(TestStep.Create(Keys.P));//, new List<int>() { 1, 2, 3, 4 }));
            localSteps.Add(TestStep.Create(Keys.Q));//, new List<int>() { 1, 2, 3, 4, 5 }));
            localSteps.Add(TestStep.Create(Keys.R));//, new List<int>() { 1, 2, 3, 5 }));
            localSteps.Add(TestStep.Create(Keys.S));//, new List<int>() { 2, 3, 4 }));
            localSteps.Add(TestStep.Create(Keys.T));//, new List<int>() { 2, 3, 4, 5 }));
            localSteps.Add(TestStep.Create(Keys.U));//, new List<int>() { 1, 3, 6 }));
            localSteps.Add(TestStep.Create(Keys.V));//, new List<int>() { 1, 2, 3, 6 }));
            localSteps.Add(TestStep.Create(Keys.W));//, new List<int>() { 2, 4, 5, 6 }));
            localSteps.Add(TestStep.Create(Keys.X));//, new List<int>() { 1, 3, 4, 6 }));
            localSteps.Add(TestStep.Create(Keys.Y));//, new List<int>() { 1, 3, 4, 5, 6 }));
            localSteps.Add(TestStep.Create(Keys.Z));//, new List<int>() { 1, 3, 5, 6 }));
            return localSteps;
        }


        public List<TestStep> TestSimpleDigits(bool all)
        {
            List<TestStep> localSteps = new List<TestStep>();
            localSteps.Add(TestStep.Create(Keys.D1));//, "1", ciffer + "1", new List<int>() { 1, digit })); // Generate "KeyDown Packet Value=231" , "KeyPress Dec=49 Hex=31 CHAR16=1" "KeyUp Packet Value=231"
            if (!all) return localSteps;
            localSteps.Add(TestStep.Create(Keys.D2));//, "2", ciffer + "2", new List<int>() { 1, 2, digit }));
            localSteps.Add(TestStep.Create(Keys.D3));//, "3", ciffer + "3", new List<int>() { 1, 4, digit }));
            localSteps.Add(TestStep.Create(Keys.D4));//, "4", ciffer + "4", new List<int>() { 1, 4, 5, digit }));
            localSteps.Add(TestStep.Create(Keys.D5));//, "5", ciffer + "5", new List<int>() { 1, 5, digit }));
            localSteps.Add(TestStep.Create(Keys.D6));//, "6", ciffer + "6", new List<int>() { 1, 2, 4, digit }));
            localSteps.Add(TestStep.Create(Keys.D7));//, "7", ciffer + "7", new List<int>() { 1, 2, 4, 5, digit }));
            localSteps.Add(TestStep.Create(Keys.D8));//, "8", ciffer + "8", new List<int>() { 1, 2, 5, digit }));
            localSteps.Add(TestStep.Create(Keys.D9));//, "9", ciffer + "9", new List<int>() { 2, 4, digit }));
            localSteps.Add(TestStep.Create(Keys.D0));//, "0", ciffer + "0", new List<int>() { 2, 4, 5, digit }));
            return localSteps;
        }


        /// <summary>
        /// Under development
        /// </summary>
        /// <param name="all"></param>
        public List<TestStep> TestPunctuationMarks(bool all)
        {
#warning ToDo Add items.
            // Common punctuation marks
            List<TestStep> localSteps = new List<TestStep>();
            localSteps.Add(TestStep.Create(Keys.Oemcomma));// "Komma", new List<int>() { 2 }));
            if (!all) return localSteps;
            //testSteps.Add(TestStep.Create(Keys.OemSemicolon));//, "Semikolon", new List<int>() { 2, 3 }));
            //testSteps.Add(TestStep.Create(Keys.OEm", "Kolon", new List<int>() { 2, 5 }));
            localSteps.Add(TestStep.Create(Keys.OemPeriod));// , "Punktum", new List<int>() { 2, 5, 6 }));
            //testSteps.Add(TestStep.Create(Keys.E, "Udråbstegn", new List<int>() { 2, 3, 5 }));
            localSteps.Add(TestStep.Create(Keys.OemOpenBrackets)); // (", "Parentes", new List<int>() { 2, 3, 5, 6 }));
            localSteps.Add(TestStep.Create(Keys.OemCloseBrackets));//, "Parentes", new List<int>() { 2, 3, 5, 6 })); // Check this !
            localSteps.Add(TestStep.Create(Keys.OemQuestion));//, "Spørgsmålstegn", new List<int>() { 2, 3, 6 }));
            //testSteps.Add(TestStep.Create("\"", "Dobbelt anførselstegn", new List<int>() { 2, 3, 6 }));
            localSteps.Add(TestStep.Create(Keys.Multiply));// ; ; ; ;, "Stjerne", new List<int>() { 3, 5 }, new List<int>() { 3, 5 }));
            //testSteps.Add(TestStep.Create(Keys, "Skråstreg", new List<int>() { 3, 5, 6 }));
            localSteps.Add(TestStep.Create(Keys.OemBackslash)); //,// "BackSlash", new List<int>() { 3, 5, 6 }));
            localSteps.Add(TestStep.Create(Keys.Oemtilde));// , "Enkelt anførselstegn", new List<int>() { 3 }));
            localSteps.Add(TestStep.Create(Keys.OemMinus));//, "Bindestreg", new List<int>() { 3, 6 }));
            localSteps.Add(TestStep.Create(Keys.Oemplus));//, "Bindestreg", new List<int>() { 3, 6 }));
            return localSteps;
        }

    }
}
