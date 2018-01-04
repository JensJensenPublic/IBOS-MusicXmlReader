using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTest
{
    public class PerkinsKeySequence
    {
        private List<List<int>> keys;
        public  List<List<int>> Keys    { get { return keys; } }
        private string stringRepresentation = "Not implemented yet";

        private PerkinsKeySequence(List<List<int>> keys)
        {
            this.keys = keys;
        }


        /// <summary>
        /// A simple PerkinsKeySequence consisting of a single keypress
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public PerkinsKeySequence Create(int key)
        {
            List<int> innerList = new List<int> { key };
            return new PerkinsKeySequence(new List<List<int>> { innerList });
        }


        /// <summary>
        /// A typical PerkinsKeySequence consisting of 2 of more keys pressed at the same time
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        static public PerkinsKeySequence Create(List<int> keys)
        {
            return new PerkinsKeySequence(new List<List<int>> { keys });
        }


        /// <summary>
        /// A typical PerkinsKeySequence consisting of 2 of more keys pressed at the same time followed by 2 of more keys pressed at the same time
        /// </summary>
        /// <param name="keys0"></param>
        /// <param name="keys1"></param>
        /// <returns></returns>
        static public PerkinsKeySequence Create(List<int> keys0, List<int> keys1)
        {
            return new PerkinsKeySequence(new List<List<int>> { keys0,keys1});
        }


        /// <summary>
        /// A complex PerkinsKeySequence consisting of a sequence of (a number of keys pressed at the same time)
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        static public PerkinsKeySequence Create(List<List<int>> keys)
        {
            return new PerkinsKeySequence(keys);
        }


        /// <summary>
        /// An empty PerkinsSequence. Can be used for marking "Unimplemented"
        /// </summary>
        /// <returns></returns>
        static public PerkinsKeySequence Create()
        {
            return new PerkinsKeySequence(new List<List<int>> {});
        }

        public override string ToString()
        {
            return stringRepresentation; // 
        }


        /// <summary>
        /// The stringrepresentation returned by ToString() depends on the keyboard
        /// because some of the keys (except for the 8 Perkins keys) have different names on different keyboards. 
        /// </summary>
        /// <param name="keyboard"></param>
        public void InitString(Keyboard keyboard)
        {
            if (0 == keys.Count) return;
            StringBuilder sb = new StringBuilder();
            string conc = ""; 
            foreach (List<int> keyList in keys)
            {
                sb.Append(conc);
                // keyList represents a number of keys pressed at the same time
                string conc1 = "";
                foreach (int perkinsKey in keyList)
                {
                    sb.Append(conc1);
                    //sb.Append(perkinsKey.ToString()); //  First implementation !!
                    sb.Append(keyboard.PerkinsKeyToString(perkinsKey)); // Only the Keyboard class knows how to represent the key as a string
                    conc1 = " ";
                } 
                conc = " efterfulgt af ";
            }
            stringRepresentation = sb.ToString();
        }

    }
}
