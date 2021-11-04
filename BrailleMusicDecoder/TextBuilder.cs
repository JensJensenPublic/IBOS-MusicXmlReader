using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Almost equivalent to StringBuilder, except that each item is a class and thus can be referenced individually
    /// This is used for instance for handling parentesis represented by a pair of dot2356 symbols.
    /// </summary>
    class TextBuilder
    {
        private List<TextItem> textItems;

        public void Append(string text)
        {
            textItems.Add(new TextItem(text));
        }

        public void Append(TextItem textItem)
        {
            textItems.Add(textItem);
        }

        public void Clear()
        {
            textItems.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TextItem textItem in textItems)
            {
                sb.Append(textItem.Text);
            }
            return sb.ToString();
        }

        private TextBuilder()
        {
            textItems = new List<TextItem>();
        }

        public static TextBuilder Create()
        {
            return new TextBuilder();
        }
    }
}
