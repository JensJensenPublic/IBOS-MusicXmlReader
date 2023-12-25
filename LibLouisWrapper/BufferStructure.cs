using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibLouisWrapper
{
    /// <summary>
    /// Simple convenience class for building buffers used for calling LibLouis functions
    /// </summary>
    internal class BufferStructure
    {

        private Encoding encoding;
        private byte[] inputBuffer;
        public byte[] InputBuffer { get { return inputBuffer; } }
        private byte[] outputBuffer;
        public byte[] OutputBuffer { get { return outputBuffer; } }
        private int outputBufferSize;
        public int OutputBufferSize { get { return outputBufferSize; } }

        public bool GetTranslation(out string translation)
        {      
            translation = null;
            if (null == outputBuffer) return false;
            string s = encoding.GetString(outputBuffer);  // Decode
            translation = s.TrimEnd(new char[] { '\0' }); // Remove all trailing null characters
            return true;                                        
        }

        private BufferStructure(string input, Encoding encoding, int sizeFactor, int minimumSize)
        {
            this.encoding = encoding;   
            inputBuffer = encoding.GetBytes(input); // Encode the input string    
            outputBufferSize = Math.Max(inputBuffer.Length * sizeFactor, 4096);
            outputBuffer = new byte[outputBufferSize];
        }
        /// <summary>
        /// Prevent construction
        /// </summary>
        private BufferStructure() { }


        public static BufferStructure Create(string input, Encoding encoding)
        {
            return new BufferStructure(input, encoding, 2, 4096);
        }


    }
}
