using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Simple convenisnce class for keeping track otf the current position within the Music Braille sheet
    /// </summary>
    internal class DecoderSpacePositionHandler
    {
        private const int initialCount = 1; // We use 1-indexing here !
        bool showPageNumber;
        string separator1 = "";
        bool showLineNumber;
        string separator2 = "";
        bool showSpaceNumber;


        private int spaceNumber = initialCount;
        public int SpaceNumber { get { return spaceNumber; } }
        private int lineNumber = initialCount;
        public int LineNumber { get { return lineNumber; } }
        private int formNumber = initialCount;
        public int FormNumber { get { return formNumber; } }

        internal string OnNewInput(InputInterpretation input)
        {
            if (null == input)
            {
                spaceNumber++; // By default just continut to the next space
                return ToString();
            }

            switch (input.Category)
            {
                case InputCategoryEnum.ControlCharCRLF:
                    lineNumber++;
                    spaceNumber = initialCount;
                    break;

                case InputCategoryEnum.ControlCharCRLFNumber:
                    lineNumber++;
                    spaceNumber = initialCount;
                    break;
#if false
                case InputCategoryEnum.ControlCharacterFF:
                    formNumber++;
                    lineNumber = initialCount;
                    spaceNumber = initialCount;
                    break;
#endif
                default:
                spaceNumber+= input.TokenLength;
                break;  
            }
            return ToString();
        }

        public override string ToString()
        {
            string result = string.Format("{0}{1}{2,02}{3}{4}",
                showPageNumber ? formNumber.ToString() : "", // 0
                separator1, // 1
                showLineNumber ? lineNumber.ToString() : "", // 2
                separator2, // 3
                showSpaceNumber ?  spaceNumber.ToString() : ""); //4
            return result;
        }

        DecoderSpacePositionHandler(StringFormatOptions stringFormatOptions)
        {
            showPageNumber = 0 != (stringFormatOptions & StringFormatOptions.pageNumber);
            showLineNumber = 0 != (stringFormatOptions & StringFormatOptions.lineNumber);
            showSpaceNumber = 0 != (stringFormatOptions & StringFormatOptions.spaceNumber);
            separator1 = (showPageNumber && showLineNumber) ? "." : "";
            separator2 = (showLineNumber && showSpaceNumber) ? "." : "";
        }


        internal static DecoderSpacePositionHandler Create(StringFormatOptions stringFormatOptions)
        {
            return new DecoderSpacePositionHandler(stringFormatOptions);
        }
    }
}
