using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace KeyboardTest
{

    [FlagsAttribute]
    public  enum TargetControl
    {
        None = 0,
        Form = 1,
        ComboBox = 2,
        ListBox= 4,
        TreeView = 8,
        All = None | Form | ComboBox| ListBox| TreeView // Add any new values here !!
    }


    /// <summary>
    /// Implements functionality and definitions common for all Perkins Keyboards, including a the set of standard Key combinations
    /// that any Perkins keyboard must implement in order to be used for controlling the IBOS MusicXmlReader program.
    /// </summary>
    public abstract class Keyboard
    {
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private TestDefinitions testDefinitions;
        protected List<TestStep> testSteps;

        // http://8dotbraille.com/
        // https://www.freedomscientific.com/Content/Documents/Manuals/Focus/Focus14Blue/Focus-14-Blue-Online-Users-Guide.htm

        public abstract string DeviceName { get; } 

        public override string ToString()
        {
            return DeviceName;
        }


        /// <summary>
        /// Needed to avoid warning CA2214
        /// </summary>
        protected void InitTests()
        {
            testDefinitions = TestDefinitions.Create();
            string functionName = "InitTests";
            testSteps = new List<TestStep>();    
            testSteps.Add(TestStep.Create(string.Format("{0}.{1}: Keyboard={2}. Venstre-click for at teste næste tegn", className, functionName,this.ToString())));
            testSteps.AddRange(testDefinitions.TestMusicXmlReaderKeys(true));
            //testSteps.AddRange(testDefinitions.TestPunctuationMarks(true));
            //testSteps.AddRange(testDefinitions.TestSimpleLetters(true));
            ////testSteps.AddRange(testDefinitions.TestMusicXmlReaderOptionalKeys(true));
            //testSteps.AddRange(testDefinitions.TestSimpleDigits(true));
            ////stSteps.AddRange(testDefinitions.TestDeviceSpecificFunctions(true));
            ////testSteps.AddRange(testDefinitions.TestKeyCombinations(true));
            //testSteps.AddRange(testDefinitions.TestPunctuationMarks(false));
        }


        //        protected abstract void TestMusicXmlReaderKeys(bool all);

  
        
        //***************************************************************************************************
        // LetterToPerkins, DigitToPerkins and PunctuationMarksToToPerkins
        // all return the standard Perkins representation of the input parameter
        //***************************************************************************************************

        protected List<int> LetterToPerkins(Keys keys)
        {
            //string functionName = "LetterToPerkins";
            switch (keys)
            {
                case Keys.A: return new List<int>() { 1 };
                case Keys.B: return new List<int>() { 1, 2 };
                case Keys.C: return new List<int>() { 1, 4 };
                case Keys.D: return new List<int>() { 1, 4, 5 };
                case Keys.E: return new List<int>() { 1, 5 };
                case Keys.F: return new List<int>() { 1, 2, 4 };
                case Keys.G: return new List<int>() { 1, 2, 4, 5 };
                case Keys.H: return new List<int>() { 1, 2, 5 };
                case Keys.I: return new List<int>() { 2, 4 };
                case Keys.J: return new List<int>() { 2, 4, 5 };
                case Keys.K: return new List<int>() { 1, 3 };
                case Keys.L: return new List<int>() { 1, 2, 3 };
                case Keys.M: return new List<int>() { 1, 3, 4 };
                case Keys.N: return new List<int>() { 1, 3, 4, 5 };
                case Keys.O: return new List<int>() { 1, 3, 5 };
                case Keys.P: return new List<int>() { 1, 2, 3, 4 };
                case Keys.Q: return new List<int>() { 1, 2, 3, 4, 5 };
                case Keys.R: return new List<int>() { 1, 2, 3, 5 };
                case Keys.S: return new List<int>() { 2, 3, 4 };
                case Keys.T: return new List<int>() { 2, 3, 4, 5 };
                case Keys.U: return new List<int>() { 1, 3, 6 };
                case Keys.V: return new List<int>() { 1, 2, 3, 6 };
                case Keys.W: return new List<int>() { 2, 4, 5, 6 };
                case Keys.X: return new List<int>() { 1, 3, 4, 6 };
                case Keys.Y: return new List<int>() { 1, 3, 4, 5, 6 };
                case Keys.Z: return new List<int>() { 1, 3, 5, 6 };
                default: return null;
            }
        }

        protected List<int> DigitToPerkins(Keys keys)
        {
            //string functionName = "DigitToPerkins";
            switch (keys)
            {
                case Keys.D1: return new List<int>() { 1, 8 };
                case Keys.D2: return new List<int>() { 1, 2, 8 };
                case Keys.D3: return new List<int>() { 1, 4, 8 };
                case Keys.D4: return new List<int>() { 1, 4, 5, 8 };
                case Keys.D5: return new List<int>() { 1, 5, 8 };
                case Keys.D6: return new List<int>() { 1, 2, 4, 8 };
                case Keys.D7: return new List<int>() { 1, 2, 4, 5, 8 };
                case Keys.D8: return new List<int>() { 1, 2, 5, 8 };
                case Keys.D9: return new List<int>() { 1, 2, 4, 8 };
                case Keys.D0: return new List<int>() { 2, 4, 5, 8 };
                default: return null;
            }
        }


        /// <summary>
        /// Under development !!!
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected List<int> PunctuationMarksToToPerkins(Keys keys)
        {
#warning ToDo Add items.
            switch (keys)
            {
                //// Common punctuation marks
                ////  § 6. Sætningstegn m.v
                case Keys.Oemcomma: return new List<int>() { 2 };
                //case ;"));//, "Semikolon", new List<int>() { 2, 3 }));
                //case :"));//, "Kolon", new List<int>() { 2, 5 }));
                case Keys.OemPeriod: return new List<int>() { 3 };
                case Keys.OemQuestion: return new List<int>() { 2, 6 }; // Packet WORKS                    
                //case !"));//, "Udråbstegn", new List<int>() { 2, 3, 5 })); // Packet WORKS
                //case \""));//, "Dobbelt anførselstegn", new List<int>() { 2, 3, 5, 6 })); // Packet WORKS
                case Keys.OemCloseBrackets: return new List<int>() { 3, 5, 6 };
                case Keys.OemOpenBrackets: return new List<int>() { 5 };
                //case ]"));//, "Kantet parentes slut", new List<int>() { 5 }, new List<int>() { 3, 5, 6 }));// FAILS
                ////testSteps.Add(TestStep.Create(@"'"));//, "Apostrof", new List<int>() { 4 })); // WORKS
                //case -"));//, "Tankestreg", new List<int>() { 3, 6 }, new List<int>() { 3, 6 })); // WORKS
                //case -"));//, "Bindestreg", new List<int>() { 3, 6 })); // 
                ////case -", "Prikker", new List<int>() { 3}, new List<int>() { 3 }, new List<int>() { 3 })); //  Overload not implemented yet

                ////// § 8. Andre tegn
                //case ="));//, "Lighedstegn", new List<int>() { 2, 3, 5, 6 }));   // FAILS (Reports anførselstegn)
                //case ="));//, "Lighedstegn", new List<int>() { 2, 3, 5, 6, 8 }));   // WORKS 
                ////case ", "Gentagelsestegn", new List<int>() {  3, 5 }));   // WORKS 
                //case *"));//, "Stjerne", new List<int>() { 3, 5 }));   // Fails: "ig" 
                //case *"));//, "Stjerne", new List<int>() { 6 }, new List<int>() { 3, 5 }));  // WORKS
                //case §"));//, "Paragraftegn", new List<int>() { 5,7,8 }));   // WORKS  
                //case /"));//, "Skråstreg", new List<int>() { 3, 4, 8 })); // WORKS 
                //case &"));//, "Ampersand", new List<int>() { 1, 2, 3, 4, 6, 8 })); // WORKS

                ////// § 10 Regnetegn
                //case +"));//, "Plus", new List<int>() { 2, 3, 5 })); // Reported as "Udråbstegn"
                //case -"));//, "Minus", new List<int>() { 3, 6 })); //  Reported as "Enkeltstreg"
                ////case ", "Gangetegn", new List<int>() { })); // 
                //case :"));//, "Divisionstegn", new List<int>() { 2, 5, 6 })); // FAILS
                //case ="));//, "Lighedstegn", new List<int>() { 2, 3, 5, 6 }));   // Reported as "Anførselstegn" 
                //case %"));//, "Procenttegn", new List<int>() { 2, 4, 5 }, new List<int>() { 3, 5, 6 })); // FAILS Reports "JAWS Markør"
                ////case ", "Promilletegn", new List<int>() {2,4,5 }, new List<int>() { 3,5,6 }, new List<int>() { 3,5,6 })); // 
                //case ^"));//, "Potenstegn", new List<int>() { 3, 4, 6 })); // FAILS
                ////// testSteps.Add(TestStep.Create("?", "Gradtegn", new List<int>() { 4, CHORD }, new List<int>() { 3, 5, 6 })); // WORKS "CHORD" may be FOCUS4 specific ?
                //case /"));//, "Brøktegn", new List<int>() { 3, 4 })); // FAILS        

                default: return null;
            }
        }




        /// <summary>
        /// Thia translation may later turn out to be dependent translation !
        /// May be overridden if neded !
        /// </summary>
        /// <param name="perkinsKey"></param>
        /// <returns></returns>
        public virtual string PerkinsKeyToString(int perkinsKey)
        {
            if ((1 <= perkinsKey) && (perkinsKey <= 8))
            {
                return perkinsKey.ToString(); // Render as "1" to "8"
            }

             foreach (int i in Enum.GetValues(typeof(DSKey)))
            {
                if (perkinsKey == i) return ((DSKey)i).ToString(); // REturn the namof the enum value
            }

            return "Ukendt PerkinsKey";
        }

        
        protected abstract PerkinsKeySequence GetPKS(KeySequenceList jawsKeySequence);
        protected abstract PerkinsKeySequence GetPKS(Keys keys);


        public PerkinsKeySequence GetPerkinsSequence(KeySequenceList jawsKeySequence)
        {
            PerkinsKeySequence pks = GetPKS(jawsKeySequence);
            pks.InitString(this); // Depends on the pks and 'this'
            return pks;
        }

        public PerkinsKeySequence GetPerkinsSequence(Keys keys)
        {
            PerkinsKeySequence pks = GetPKS(keys); // Depends on 'this' and the keys to represent
            pks.InitString(this); // Depends on the pks and 'this'
            return pks;
        }

        protected virtual void TestDeviceSpecificFunctions(bool all)
        { }

        protected virtual void TestMusicXmlReaderOptionalKeys(bool all)
        { }


        protected Keyboard()
        {
        }
        
        public List<TestStep> TestSteps { get { return testSteps; } }

 
        public static KeyboardFocus14 CreateFocus14Keyboard()
        {            
            KeyboardFocus14 k = new KeyboardFocus14();
            k.InitTests();
            return k;
        } 

        public static KeyboardStandard CreateStandardKeyboard()
        {
            KeyboardStandard k = new KeyboardStandard();
            k.InitTests();
            return k;
        }

        public static KeyboardBraillant CreateBraillantKeyboard()
        {
            KeyboardBraillant k = new KeyboardBraillant();
            k.InitTests();
            return k;
        }


        public static KeyboardBrailleNoteTouch CreateBrailleNoteTouchKeyboard()
        {
            KeyboardBrailleNoteTouch k = new KeyboardBrailleNoteTouch();
            k.InitTests();
            return k;
        }

        public static KeyboardHimsEdge CreateKeyboardHimsEdgeKeyboard()
        {
            KeyboardHimsEdge k = new KeyboardHimsEdge();
            k.InitTests();
            return k;
        }

        public static KeyboardU2 CreateKeyboardHimsU2Keyboard()
        {
            KeyboardU2 k = new KeyboardU2();
            k.InitTests();
            return k;
        }
        

        private void Warn(Keys keys)
        {
            Console.WriteLine(string.Format("Unspecified value of TargetControl for {0}", keys.ToString()));
        }  

    }
}
