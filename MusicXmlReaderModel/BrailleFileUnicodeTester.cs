using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://en.wikipedia.org/wiki/UTF-8
// https://en.wikipedia.org/wiki/UTF-16
// https://en.wikipedia.org/wiki/UTF-32


namespace MusicXmlReaderModel
{

    class BrailleFileUnicodeTesterImplicitUtf8 : BrailleFileUnicodeTesterUtf8
    {
        public BrailleFileUnicodeTesterImplicitUtf8() : base()
        {
            preamble = new byte[] { }; // Note that the expected preamble differs from when UTF-8 is explicitly specified!!
        }
    }

    class BrailleFileUnicodeTesterUtf8 : BrailleFileUnicodeTester
    {
        public BrailleFileUnicodeTesterUtf8() : base()
        {
            encoding = Encoding.UTF8; 
            maxNumberOfBytes = 3;
            preamble = new byte[] { 0xef, 0xbb, 0xbf }; // Note that the expected preamble differs from when UTF-8 is inplicitly specified!!
            singleByteLimit = 0x80; // A Unicode value less than 0x80 (such as a typical ASCII character) can be represented as a single byte.
        }

        /// <summary>
        /// Please see https://en.wikipedia.org/wiki/UTF-8
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected override Int64 GetIntValue(List<byte> bytes)
        {
            Int64 result = 0;
            bool ok = false;
            {
                // Please see https://en.wikipedia.org/wiki/UTF-8
                int length = bytes.Count;

                int b1, b2, b3, b4; // Values extracted from the bytes
                int m1, m2, m3, m4; // Masks extrcted from the bytes          
                switch (length)
                {
                    case 1:
                        b1 = bytes[0] % 0x80; // Take 7
                        result = b1;
                        m1 = bytes[0] / 0x80; // Take 1
                        ok = Report((m1 == 0), bytes);
                        break;
                    case 2:
                        b1 = bytes[0] % 0x20; // Take 5
                        b2 = bytes[1] % 0x40; // Take 6
                        result = b1 * 0x40 + b2;
                        m1 = bytes[0] / 0x20; // Take 3
                        m2 = bytes[1] / 0x40; // Take 2
                        ok = Report(((m1 == 0x6) && (m2 == 0x2)), bytes);
                        break;
                    case 3:
                        b1 = bytes[0] % 0x10; // Take 4
                        b2 = bytes[1] % 0x40; // Take 6   
                        b3 = bytes[2] % 0x40; // Take 6  
                        result = b1 * 0x1000 + b2 * 0x40 + b3; // 4+6+6 = 16 bits
                        m1 = bytes[0] / 0x10; // Take 4
                        m2 = bytes[1] / 0x40; // Take 2
                        m3 = bytes[2] / 0x40; // Take 2
                        ok = Report(((m1 == 0xe) && (m2 == 0x2) && (m3 == 0x2)), bytes);
                        break;
                    case 4:
                        b1 = bytes[0] % 0x08; // Take 3
                        b2 = bytes[1] % 0x40; // Take 6   
                        b3 = bytes[2] % 0x40; // Take 6  
                        b4 = bytes[3] % 0x40; // Take 6  
                        result = b1 * 0x40000 + b2 * 0x400 + b3 * 40 + b4; // 3+6+6+6 = 21 bits
                        m1 = bytes[0] / 0x08; // Take 3
                        m2 = bytes[1] / 0x40; // Take 2
                        m3 = bytes[2] / 0x40; // Take 2
                        m4 = bytes[3] / 0x40; // Take 2
                        ok = Report(((m1 == 0xe) && (m2 == 0x2) && (m3 == 0x2) && (m4 == 0x2)), bytes);
                        break;
                }
                return result;
            }
        }
    }


    /// <summary>
    /// https://en.wikipedia.org/wiki/UTF-16
    /// </summary>
    class BrailleFileUnicodeTesterUtf16 : BrailleFileUnicodeTester
    {
        public BrailleFileUnicodeTesterUtf16() : base()
        {
            encoding = Encoding.Unicode; // Means UTF-32
            maxNumberOfBytes = 2;
            preamble = new byte[] { 0xff, 0xfe };
        }

        protected override Int64 GetIntValue(List<byte> bytes)
        {
            return GetIntValueLittleEndian(bytes);
        }
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/UTF-32
    /// </summary>
    class BrailleFileUnicodeTesterUtf32 : BrailleFileUnicodeTester
    {
        public BrailleFileUnicodeTesterUtf32() : base()
        {
            encoding = Encoding.UTF32;
            maxNumberOfBytes = 4;
            preamble = new byte[] { 0xff, 0xfe, 0x00, 0x00 };
        }

        protected override Int64 GetIntValue(List<byte> bytes)
        {
            return GetIntValueLittleEndian(bytes);
        }
    }



    /// <summary>
    /// Class for testing that a given representation of BrailleMusic encoded Unicode in either UTF-8, UTF-16 or UTF-32 is correct
    /// This class is primarily build "for fun" and in order to better understand the UTF-x mechanisms.
    /// The standard Windows  mechanisms should be used for the real coding and encoding
    /// </summary>
    abstract class BrailleFileUnicodeTester
    {
        protected byte[] preamble = new byte[] { };
        protected int minValue = 0x2800 + 0x00; // No dots
        protected int maxValue = 0x2800 + 0x3f; // All 6 dots
        protected int maxNumberOfBytes = 0;
        protected int singleByteLimit = 0; // Allows for representing as a single byte in UTF-8 
        protected Encoding encoding;

        protected abstract Int64 GetIntValue(List<byte> bytes);

        public bool Test(byte[] bytesReadFromFile, byte[] controlCharacters)
        {
            int index = 0;
            int errors = 0;
            int validChars = 0;
            int controlChars = 0;
            for (int i = 0; (i < preamble.Length); i++)
            {
                byte bf = (bytesReadFromFile[index++]);
                byte bp = (preamble[i]);
                if (bf != bp)
                {
                    Logger.LogCF(string.Format(": Wrong format of preamble: ..."));
                    return false;
                }
            }

            while (index < bytesReadFromFile.Length)
            {
                bool isSingleByte;
                Int64 nextValue = GetBytes(bytesReadFromFile, ref index, out isSingleByte);
                if ((nextValue < 0x100) && (controlCharacters.Contains<byte>((byte)nextValue)))
                {
                    // A control character coded in more than one byte
                    controlChars++;
                    //Logger.LogCF(string.Format(": Control+: {0} 0x{1:x}", controlChars, nextValue));
                }
                else if
                ((minValue <= nextValue) && (nextValue <= maxValue)) // A valid Braille 6-dot value
                {
                    validChars++;
                    //Logger.LogCF(string.Format(": {0} 0x{1:x}", validChars, nextValue));
                }
                else
                {
                    if (0 == errors++) // Onli report the first error, but count them !
                    {
                        string message = string.Format("Value=0x{0:x} exceeds valid interval [0x{1:x}]..[0x{2:x}]", nextValue, minValue, maxValue);
                        Logger.LogCF(message);
                    }
                    break; // No meaning to continue after an error because the index is invalid
                }
            }

            bool allUsed = (index == bytesReadFromFile.Length);
            if (!allUsed)
            {
                Logger.LogCF(string.Format(": ERROR: Number of bytes read from file={0} is not equal to number of bytes decoded={1}", bytesReadFromFile.Length, index));
            }
            Logger.LogCF(string.Format(": Found {0} control characters, {1} other valid characters and {2} errors ", controlChars, validChars, errors));
            return (allUsed && (errors == 0));
        }

        /// <summary>
        /// Returns the value of the next character from ( an array of bytes in utf-8, utf-16 or utf-12 encoding)
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="maxNumberOfBytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected Int64 GetBytes(byte[] bytes, ref int index, out bool isSingleByte)
        {
            isSingleByte = false;
            int maxNumberOfBytesLimit = 4;
            if (maxNumberOfBytesLimit < this.maxNumberOfBytes)
            {
                string message = string.Format("numberOfBytes={0} execeds maxnumberOfBytes={1}", this.maxNumberOfBytes, maxNumberOfBytesLimit);
                Logger.LogCF(string.Format(": {0}", message));
                throw new ArgumentException(message);
            }
            List<byte> byteList = new List<byte>();
            for (int i = 0; (i < this.maxNumberOfBytes); i++)
            {
                byte value = bytes[index++];
                if ((i == 0) && (value < singleByteLimit))
                {
                    isSingleByte = true;
                    return value; // This is one of the 7-bit values encoded in the 7 low bits in a single byte 
                }
                byteList.Add(value);   // This is  of the values encoded in a number bytes
            }
            return GetIntValue(byteList);
        }

        protected  Int64 GetIntValueLittleEndian(List<byte> bytes)
        {
            Int64 result = 0;
            // Assume little endian representation
            for (int i = bytes.Count - 1; i >= 0; i--)
            {
                result = (result * 0x100) + bytes[i];
            }
            return result;
        }
        
        protected bool Report(bool ok, List<byte> bytes)
        {
            if (!ok)
            {
                StringBuilder sb = new StringBuilder();
                foreach (byte value in bytes)
                {
                    sb.Append(string.Format("0x{0:x} ", value));
                }
                string message = string.Format(": Illegal mask found in < {0}>", sb);
                Logger.LogCF(message);
            }
            return ok;
        }

        public static BrailleFileUnicodeTester Create()
        {
            return new BrailleFileUnicodeTesterImplicitUtf8(); //  UTF-8 is used as a default
        }

        public static BrailleFileUnicodeTester Create(Encoding encoding)
        {
            if (encoding == Encoding.UTF8)
            {
                return new BrailleFileUnicodeTesterUtf8();
            }
            else if (encoding == Encoding.Unicode) // UFT-16
            {
                return new BrailleFileUnicodeTesterUtf16();
            }
            else if (encoding == Encoding.UTF32)
            {
                return new BrailleFileUnicodeTesterUtf32();
            }
            else
            {
                string message = string.Format("Unsupported Encoding {0}", encoding.EncodingName);
                Logger.LogCF(string.Format(": {0}", message));
                return null;
            }
        }
    }
}

