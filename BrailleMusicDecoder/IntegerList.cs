using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// A simple class for representing a sequence of Braille6 values coded as integers in [0..63]
    /// </summary>
    class IntegerList
    {
        private List<int> list = new List<int>();
        public List<int> List { get { return list; } }
        public int Count { get { return list.Count; } }
        public IntegerList() { }
        public IntegerList(int i)
        {
            this.list.Add(i);
        }
        public IntegerList(int i0, int i1)
        {
            this.list.Add(i0);
            this.list.Add(i1);
        }

        public void Add(int i)
        {
            list.Add(i);
        }

        public bool StartsWith(IntegerList sequence)
        {
            if (this.list.Count < sequence.Count) return false;
            for (int i = 0; (i < sequence.Count); i++)
            {
                if (this.list[i] != sequence.list[i]) return false;
            }
            return true;
        }
        
        public string ToUnicodeString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in this.list)
            {
                sb.Append(TokenReader.ToUnicodeChar(i));
            }
            return sb.ToString();
        }
    }


}
