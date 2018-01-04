using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardTest
{
    public class KeyEventSequence
    {
        string className = "KeyEventSequence";

        private List<KeyEvent> list = new List<KeyEvent>();
        public List<KeyEvent> List { get { return list; } } // Containa all events

        private List<KeyEvent> uniqueEvents = new List<KeyEvent>();
        public List<KeyEvent> UniqueEvents { get { return uniqueEvents; } } // Contains only one instance of each event

        private List<int> keysDown = new List<int>();
        public List<int> KeysDown { get { return keysDown; } } // Contains all keys currently pressed

        public KeyEventSequence()
        {
            //list =
            //uniqueEvents 
            //keysDown = new List<Keys>();
        }

        public int Add(KeyEvent newEvent)
        {
            // Add to list of all events
            list.Add(newEvent);

            // Add to list of unique events
            KeyEvent existingKeyEvent = uniqueEvents.Find(newEvent.Equals);
            if (null == existingKeyEvent)
            {
                uniqueEvents.Add(newEvent); // Count is initialized to  1!!.
            }
            else
            {
                existingKeyEvent.Count++;
            }
                
            newEvent.AddToList(keysDown);

            // Add to list of keys down::
            // newEvent.AddToKeysDown(keysDown);

            return keysDown.Count;
        }

        public override string ToString()
        {
            string functionName = "ToString";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}= ", className, functionName);
            foreach (KeyEvent keyEvent in uniqueEvents)
            {
                string newString = keyEvent.ToString();
                sb.Append(newString);
            }
            return sb.ToString();
        }

        public void Clear()
        {
            list.Clear();
            uniqueEvents.Clear();
            keysDown.Clear();
        }

    }




}
