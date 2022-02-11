//#define TestMode 
#undef TestMode
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Contains basic definitions used for all derived classes: BrailleBuilder, BrailleBuilderEx and BrailleBuilderForText
    /// This Base class does not know anything about MusicBraille and is used ac base class for MusicBraille as well as normal TextBraille !
    /// It relies on linking BrailleBuilders instead of always appending braille and text immediately
    /// This eases formatting into Braille documents for embossers and notetakers with limited linewidth and formheight
    /// </summary>
    public abstract class BrailleBuilderBase
    {
        // Valuse for explicitly defining dot patterns in terms of hex byte-values
        protected const byte noDots = 0;
        protected const byte dot1 = 0x01;
        protected const byte dot2 = 0x02;
        protected const byte dot3 = 0x04;
        protected const byte dot4 = 0x08;
        protected const byte dot5 = 0x10;
        protected const byte dot6 = 0x20;
        protected const byte dot7 = 0x40;
        protected const byte dot8 = 0x80;

        // Values representing single digits, used by all derived classes
        protected static readonly byte Number = dot3 + dot4 + dot5 + dot6; // Marks the start of numeric coding
        protected static readonly byte cipher0 = dot2 + dot4 + dot5; // 
        protected static readonly byte cipher1 = dot1; // 
        protected static readonly byte cipher2 = dot1 + dot2; // 
        protected static readonly byte cipher3 = dot1 + dot4; // 
        protected static readonly byte cipher4 = dot1 + dot4 + dot5; // 
        protected static readonly byte cipher5 = dot1 + dot5; // 
        protected static readonly byte cipher6 = dot1 + dot2 + dot4; // 
        protected static readonly byte cipher7 = dot1 + dot2 + dot4 + dot5; // 
        protected static readonly byte cipher8 = dot1 + dot2 + dot5; // 
        protected static readonly byte cipher9 = dot2 + dot5; // 

        public const Int64 NoTimeStamp = -1;
        // The basic variables used for building up Braille strings
        private List<byte> braille = new List<byte>(); // Contains the raw Braille 6-Bit patterns
        public List<byte> Braille { get { return braille; } }
        private StringBuilder text = new StringBuilder(); // Contains a (homemade) textrepresentation of the 6-bit patterns. Used by developers during debugging
        private Int64 timeStamp = NoTimeStamp;    // Contains the timestamp of the EventDescription which was used for generating this BrailleBuilder. -1 if not related to an EventDescription.  
        private List<BrailleBuilderBase> children = new List<BrailleBuilderBase>(); // Contains the BrailleBuilders used for building this BrailleBuilder

        public long TimeStamp { get { return timeStamp; } }

        // General parameter for logging, debugging and statistics 
        static public bool verbose = true;

        /// <summary>
        /// For debugging only: Returns the Braille value formatted as HEX values
        /// </summary>
        /// <returns></returns>
        public string BrailleToHexString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in braille)
            {
                sb.Append(string.Format("0x{0:x} ", b));
            }
            return sb.ToString();
        }

        /// <summary>
        /// For debugging only: Returns the Braille value formatted as a Unicode string
        /// </summary>
        /// <returns></returns>
        public string BrailleToUnicode()
        {
            return ToUnicodeString(braille); // Rely on the private implementation.
        }


        public StringBuilder Text
        {
            get
            {
                return text;
            }
        }

        public void AppendText(string s)
        {
            this.text.Append(s);
        }

        /// <summary>
        /// Kept private in order to prevent  new List<byte>(n) with n as a size instead of a single list element!
        /// </summary>
        /// <param name="bytes"></param>
        private void Append(List<byte> bytes)
        {
            this.braille.AddRange(bytes);
        }

        public void Append(byte[] bytes)
        {
            this.braille.AddRange(new List<byte>(bytes));
        }

        public void Append(byte b)
        {
            this.braille.Add(b);
        }

        public void Append(byte b, string text)
        {
            this.braille.Add(b);
            this.text.Append(text);
        }

        public void Append(byte b, char character)
        {
            this.braille.Add(b);
            this.text.Append(character);
        }

        public void Append(byte[] bytes, string text)
        {
            this.braille.AddRange(new List<byte>(bytes));
            this.text.Append(text);
        }

        /// <summary>
        /// Kept private in order to prevent  new List<byte>(n) with n as a size instead of a single list element!
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="text"></param>
        protected void Append(List<byte> bytes, string text)
        {
            this.braille.AddRange(bytes);
            this.text.Append(text);
        }


        /// <summary>
        /// Adds the BrailleBuilder by linking it into this instead of concatenating Braille and Text
        /// This allows for concatenating Braille and Text at a later time when the dimensions of the embosser or notataker is known
        /// </summary>
        /// <param name="bb"></param>
        public void Append(BrailleBuilderBase bb)
        {
            Concatenate(bb);  // Empty function unless in TestMode ///
            this.children.Add(bb);
        }


        /// <summary>
        /// Simple convenience method for converting from simple "dot" representation to Unicode
        /// </summary>
        /// <param name="braille"></param>
        /// <returns></returns>
        private string ToUnicodeString(List<byte> braille)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in braille) { sb.Append((char)(BrailleDisplayer.UnicodeBrailleBase + (char)b)); };
            return sb.ToString();
        }

        /// <summary>
        /// Simple convenience method for converting from simple "dot" representation to Unicode
        /// </summary>
        /// <param name="braille"></param>
        /// <returns></returns>
        public string ToUnicodeString(byte[] braille)
        {
            return ToUnicodeString(new List<byte>(braille));
        }


        /// <summary>
        /// Returns the Unicode representation of this BrailleBuilder
        /// </summary>
        /// <returns></returns>
        public string ToBrailleString()
        {
            string linked = ToLinkedBrailleString();
            if (IsTestMode) TestBraille(linked);
            return linked;
        }

        /// <summary>
        /// Returns the Equvivalent text representation of this BrailleBuilder
        /// </summary>
        /// <returns></returns>
        public string ToEquvivalentTextRepresentation()
        {
            string linked = this.ToLinkedEquvivalentTextRepresentation();
            if (IsTestMode) TestEquvivalentText(linked); 
            return linked;
        }

        /// <summary>
        /// Returns the contents as a string of Unicode characters in the interval 0x2800 ..0x28ff
        /// </summary>
        /// <returns></returns>
        private string ToLinkedBrailleString()
        {
            // The new implementation relying on late concatenatin
            StringBuilder sb = new StringBuilder();
            sb.Append(ToUnicodeString(this.braille));
            foreach (BrailleBuilderBase child in children)
            {
                sb.Append(child.ToLinkedBrailleString());
            }
            string result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Returns the logically equvivalent text representation of the MusicBraille string returned by ToBrailleString()
        /// </summary>
        /// <returns></returns>
        private string ToLinkedEquvivalentTextRepresentation()
        {
            // The new implementation relying on late concatenatin
            StringBuilder sb = new StringBuilder();
            sb.Append(this.text);
            foreach (BrailleBuilderBase child in children)
            {
                sb.Append(child.ToLinkedEquvivalentTextRepresentation());
            }
            string result = sb.ToString();
            return result;
        }

        #region Constructors
        protected BrailleBuilderBase(Int64 timeStamp)
        {
            this.timeStamp = timeStamp;
        }

        protected BrailleBuilderBase()
        {
            this.timeStamp = NoTimeStamp;
        }
        #endregion 

        //*************************************************************************************************
        #region TectCode for testing  "linking"(the new implementation) against "concatenation"(the old implementation)
        //*************************************************************************************************
#if (TestMode)
        private StringBuilder addedText = new StringBuilder(); // The text of  values appended form other BrailleBuilders
        private List<byte> addedBraille = new List<byte>();  // The Braille of  values appended form other BrailleBuilders

        // Counters, only used for debugging and test
        static private int errors = 0;
        static private int successes = 0;

        private void Concatenate(BrailleBuilder bb)
        {
            // The old implementation
            addedText.Append(bb.text);
            addedText.Append(bb.addedText);
            this.addedBraille.AddRange(bb.braille);
            this.addedBraille.AddRange(bb.addedBraille);
        }

        private void TestBraille(string linked)
        {
            Test(linked, this.ToConcatenatedBrailleString(), "Braille");
        }

        private void TestEquvivalentText(string linked)
        {
            Test(linked, this.ToConcatenatedEquvivalentTextRepresentation(), "EquText");
        }

        private void Test(string linked, string concatenated, string comment)
        {
            // While debugging check that the 2 implementations deliver identical results:
            if (0 != string.Compare(linked, concatenated))
            {
                string message = ": Linked and Concatenated differ. {0}'{1}'";
                Logger.LogCFOnce(string.Format(": {0} Linked.Length='{1}' Concatenated.Length = '{2}'", comment, linked.Length, concatenated.Length));
                Logger.LogCFOnce(string.Format(message, "Linked      =", linked));
                Logger.LogCFOnce(string.Format(message, "Concatenated=", concatenated));
                errors++;
            }
            else
            {
                // Simple way to show that TestMode is on
                successes++;
                if (successes == 1)
                {
                    // Logger.LogCF(string.Format(": Successes={0} Errors={1}", successes, errors));
                }
                if (0 == (successes % 1000))
                {
                    // Logger.LogCF(string.Format(": Successes={0} Errors={1}", successes, errors));
                }
            }
        }

        private string ToConcatenatedBrailleString()
        {
            // The original implementaion relying on early concatenation
            string result = ToUnicodeString(braille) + ToUnicodeString(addedBraille);
            return result;
        }

        private string ToConcatenatedEquvivalentTextRepresentation()
        {
            // The original implementaion relying on early concatenation
            string result = text.ToString() + addedText.ToString();
            return result;
        }

        private const bool IsTestMode = true;
#else
        // Empty functions used as placeholders for functions only used in TestMode
        private void Concatenate(BrailleBuilderBase bb) { }
        private void TestBraille(string linked){}
        private void TestEquvivalentText(string linked){}
        private  bool IsTestMode = false;
#endif
        #endregion

    }

}
