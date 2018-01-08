using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardTest
{
    public class KeySequenceList
    {
        private List<Keys> prolog;
        private Keys keys;

        /// <summary>
        /// Convenience method for creating a JAWS prolog
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        static private List<Keys> Jaws(Keys keys)
        {
            return new List<Keys> { Keys.Insert, keys };
        }


        /// <summary>
        /// These variables can be used for identifying the various JAWA commands.
        /// </summary>
        public static KeySequenceList JawsToggleSpeech = KeySequenceList.Create(Jaws(Keys.Space), Keys.S);
        public static KeySequenceList JAWSReadTitleLine = KeySequenceList.Create(Jaws(Keys.T), Keys.None);
        public static KeySequenceList JAWSReadStatusLine = KeySequenceList.Create(Jaws(Keys.PageDown), Keys.None);
        public static KeySequenceList JAWSReadMessage = KeySequenceList.Create(Jaws(Keys.B), Keys.None);



        public static KeySequenceList Create(List<Keys> prolog, Keys keys)
        {
            return new KeySequenceList(prolog, keys);
        }

        //public static KeySequenceList Create()
        //{
        //    return new KeySequenceList(null,Keys.None);
        //}

        private KeySequenceList(List<Keys> prolog, Keys keys)
        {
            this.prolog = prolog;
            this.keys = keys;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is KeySequenceList)) return false;
            KeySequenceList that = obj as KeySequenceList;
            if (this.keys != that.keys) return false;
            if (this.prolog.Count != that.prolog.Count) return false;
            foreach (Keys key in this.prolog)
            {
                if (!that.prolog.Contains(key)) return false;
            }
            foreach (Keys key in that.prolog)
            {
                if (!this.prolog.Contains(key)) return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (0 != prolog.Count)
            {
                string conc = "";
                foreach (Keys key in prolog)
                {
                    // sb.Append(key.ToString() + " ");
                    sb.Append(conc + Utilities.KeysToString(key));
                    conc = "+";
                 }  
            }

            if (keys != Keys.None)
            {
                sb.Append(" efterfulgt af ");
                sb.Append(Utilities.KeysToString(keys));
                //sb.Append(keys);
            }
            return sb.ToString();
        }


    }
}
