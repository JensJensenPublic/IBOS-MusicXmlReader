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
    public class IntegerList
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in list)
            {
                sb.Append((char)(0x2800 + i));
            }
            return sb.ToString();
        }

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
        public IntegerList(int i0, int i1, int i2)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
        }
        public IntegerList(int i0, int i1, int i2, int i3)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
        }
        public IntegerList(int i0, int i1, int i2, int i3, int i4)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
        }

        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
        }


        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5, int i6)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
            this.list.Add(i6);
        }

        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int i7)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
            this.list.Add(i6);
            this.list.Add(i7);
        }

        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
            this.list.Add(i6);
            this.list.Add(i7);
            this.list.Add(i8);
        }




#warning todo Generic version using List<int> as parameter !


        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8, int i9)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
            this.list.Add(i6);
            this.list.Add(i7);
            this.list.Add(i8);
            this.list.Add(i9);

        }
#if false
        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8, int i9, int i10)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
            this.list.Add(i6);
            this.list.Add(i7);
            this.list.Add(i8);
            this.list.Add(i9);
            this.list.Add(i10);
        }

        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8, int i9, int i10, int i11)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
            this.list.Add(i6);
            this.list.Add(i7);
            this.list.Add(i8);
            this.list.Add(i9);
            this.list.Add(i10);
            this.list.Add(i11);
        }

        public IntegerList(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8, int i9, int i10, int i11, int i12)
        {
            this.list.Add(i0);
            this.list.Add(i1);
            this.list.Add(i2);
            this.list.Add(i3);
            this.list.Add(i4);
            this.list.Add(i5);
            this.list.Add(i6);
            this.list.Add(i7);
            this.list.Add(i8);
            this.list.Add(i9);
            this.list.Add(i10);
            this.list.Add(i11);
            this.list.Add(i12);
        }
#endif
        public IntegerList(List<int> list)
        {
            this.list = list;
        }




        public IntegerList(IntegerList that, int count)
        {
            for (int i = 0; (i < count); i++)
            {
                this.List.Add(that.List[i]);
            }
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


        public bool Contains( List<int> sequence, int startPosition)
        {
            if (this.List.Count < startPosition + sequence.Count) return false;
            for (int i = 0; (i < sequence.Count); i++)
            {
                if (this.list[startPosition + i] != sequence[i]) return false;
            }
            return true;
        }


        public bool Equals(IntegerList that)
        {
            if (this.List.Count != that.List.Count) return false;
            return this.StartsWith(that);
        }
        
        public string ToUnicodeString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in this.list)
            {
                sb.Append(TokenReaderUtilities.ToUnicodeChar(i));
            }
            return sb.ToString();
        }
    }


}
