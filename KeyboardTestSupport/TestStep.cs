using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardTest
{
    public class TestStep
    {
        private TargetControl targetControl;
        public TargetControl TargetControl { get { return targetControl; } set { targetControl = value; } }
        private Keys keys;
        public  Keys Keys { get { return keys; } }
        private string caption; // A plain text to show with or without keys
        private KeySequenceList keySequenceList; // A sequence of standard keyboard Keys, for instance a JAWS sequence: {Kays.Insert , Keys.Space}  followed by Keys.S
        public KeySequenceList KeySequenceList { get { return keySequenceList; } }
        public Keys ExpectedKeys { get { return keys; } }

        public const int MELLEMRUM = 0;
        // public const int Add as needed
    

        private string ToString(int i)
        {
            DSKey dsKey = (DSKey)i;
            return dsKey.ToString();
        }


        /// <summary>
        /// Returns a string describing the Key representation of this TestStep. 
        /// Internally the Key representation is either contained in this.keys or in this.keySequenceList.
        /// </summary>
        /// <returns></returns>
        public string GetKeyRepresentation()
        {
            if (Keys.None != keys) return Utilities.KeysToString(keys);//  keys.ToString();

            if (null != keySequenceList) return keySequenceList.ToString(); 
            return "";
        }


        //public string GetKeyRepresentation()



        public  string ToString(Keyboard keyboard)
        {                
            StringBuilder result = new StringBuilder();

            if (null != caption)
            {
                result.Append(caption);
            }

            const string prolog = ""; //  " STANDARD ";

            // First append information about tke Qwerty key sequence to emulate:
            if (null != keySequenceList)
            {
                result.Append(prolog);
                result.Append(keySequenceList.ToString());
            }

            if (Keys.None != keys)
            {
                result.Append(prolog);
                result.Append(Utilities.KeysToString(keys));

            }

            result.Append(" PERKINS ");

            PerkinsKeySequence localPerkinsKeySequence = (Keys.None != keys) ? keyboard.GetPerkinsSequence(keys) : keyboard.GetPerkinsSequence(keySequenceList); 
            if (null != localPerkinsKeySequence)
            {
                result.Append(localPerkinsKeySequence.ToString());
            }


            return result.ToString();

        }


        public static TestStep Create(string caption)
        {
            return new TestStep(caption,TargetControl.None,Keys.None,null);
        }

        public static TestStep Create(TargetControl targetControl, Keys keys)
        {
            return new TestStep("", TargetControl.All, keys, null);
        }

        public static TestStep Create(Keys keys)
        {
            return new TestStep("",TargetControl.All, keys, null);
        }

        public static TestStep Create(KeySequenceList keySequenceList)
        {
            return new TestStep("", TargetControl.All, Keys.None, keySequenceList);
        }

        public static TestStep Create(string caption, KeySequenceList keySequenceList)
        {
            return new TestStep(caption, TargetControl.All, Keys.None, keySequenceList);
        }


        private TestStep(string caption, TargetControl targetControl, Keys keys, KeySequenceList keySequenceList)
        {
            this.caption = caption;
            this.targetControl = targetControl;
            this.keys = keys;
            this.keySequenceList = keySequenceList;
        }

        //public void Init(Keyboard keyboard)
        //{
        //    if (keys != Keys.None) this.perkinsKeySequence = keyboard.GetPerkinsSequence(keys);
        //    if ( null != keySequenceList) this.perkinsKeySequence = keyboard.GetPerkinsSequence(keySequenceList);
        //}


    }
}
