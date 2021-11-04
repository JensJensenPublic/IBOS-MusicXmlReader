using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// Utility class for allowing reference to individual pieces of text during conversion.
    /// This allows (for instance) for handling parentesis structures by saving a reference to the left parentesis while looking for the right parentesis.
    /// Will also allow (for instance) for saving the original MusicBraille sequence which was converted to the current text sequence (should the need arise).
    /// </summary>
    internal class TextItem
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public TextItem(string text)
        {
            this.text = text;
        }
    }
}
