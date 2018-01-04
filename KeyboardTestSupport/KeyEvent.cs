using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace KeyboardTest
{
    public abstract class KeyEvent
    {
        protected Keys keys;
        public Keys Keys { get { return keys; } }
        public int NewKey { get { return ((int)keys) % 0x10000; } } // The low 16 bits contain the keyCode for the key itseff, NOT including SONTROL, SHIFT etc

        protected char character;
        public char Character { get { return character; } }
        public abstract override string ToString();
        private int count = 1;
        public int Count { get { return count;} set { count = value;} } // Number of instances of this Keyevent
        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType()) return false;
            KeyEvent theOther = (obj as KeyEvent);
            return (this.character == theOther.character) &&  (this.keys == theOther.keys);  // Explicitly do NOT compare .count!!
        }

        // Override GetHashCode to get rid of warning
        public override int GetHashCode()
        {
            return GetHashCode();
        }

        public abstract int AddToList(List<Keys> list);
        public abstract int AddToList(List<int> list);

        public string Source(string className, string functionName)
        {
            return ""; // Disables logging of source
            //return string.Format("{0}.{1}", className, functionName); // Enables logging of source
        }
    }

    public class KeyDownEvent : KeyEvent
    {
        string className = "KeyDownEvent";

        public KeyDownEvent(Keys keys)
        {
            this.keys = keys;
        }

        public override string ToString()
        {
            string functionName = "ToString";
            string source = Source(className, functionName);
            int value = (int) keys;
            string sCount = (1 == Count) ? "" : Count.ToString() + "*";
            return string.Format("{0}{1}KeyDown({2}=0x{3:X})  ",source,  sCount, keys.ToString(), value);
        }

        public override int AddToList(List<Keys> list)
        {
            if (!list.Contains(keys))
            {
                list.Add(keys);
            }
            return list.Count;
        }

        public override int AddToList(List<int> list)
        {
            // int i = ((int)this.keys) % 0x10000;
            if (!list.Contains(NewKey))
            {
                list.Add(NewKey);
            }
            return list.Count;
        }


    }

    public class KeyPressedEvent : KeyEvent
    {
        string className = "KeyPressedEvent";

        public KeyPressedEvent(char character)
        {
            this.character = character;
        }

        public override string ToString()
        {
            string functionName = "ToString";
            string source = Source(className, functionName);
            return string.Format("{0}KeyPressed('{1}' =0x{2:X})  ",source, character.ToString(), (int) character);
        }

        public override int AddToList(List<Keys> list)
        {
            return list.Count;
        }

        public override int AddToList(List<int> list)
        {
            return list.Count;
        }


    }


    public class KeyUpEvent : KeyEvent
    {
        string className = "KeyUpEvent";

        public KeyUpEvent(Keys keys)
        {
            this.keys = keys;
        }
        
        public override string ToString()
        {
            string functionName = "ToString";
            string source = Source(className, functionName);
            int value = (int) keys;
            return string.Format("{0} KeyUp({1}=0x{2:X})  ",source, keys.ToString(), value);
        }

        public override int AddToList(List<Keys> list)
        {
            list.Remove(keys);
            return list.Count;
        }

        public override int AddToList(List<int> list)
        {
            // int i = ((int)this.keys) % 0x10000;
            list.Remove(NewKey);
            return list.Count;
                 
        }


    }



}
