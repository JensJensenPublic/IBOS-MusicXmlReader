using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;


namespace BrailleMusicDecoder
{
    class TextVersalHandler
    {
        private bool versalSymbol;

        public void ClearSymbol()
        {
            versalSymbol = false;
        }

        public string Format(string s)
        { 
            if (!versalSymbol) return s;
            versalSymbol = false;
            if (string.IsNullOrEmpty(s)) return s;
            string s0 = s.Substring(0,1);
            string s0Upper = s0.ToUpper();
            if (1 == s.Length) return s0Upper;
            return s0Upper + s.Remove(0, 1);
        }


        public void Format(TextItem textItem)
        {
            if (!versalSymbol) return;
            textItem.Text = Format(textItem.Text);
        }


        public void OnTextVersal(InputSubCategoryEnum subCategory)
        {
            switch (subCategory)
            {
                case InputSubCategoryEnum.TextVersalSymbol: versalSymbol = true; break;
                case InputSubCategoryEnum.TextVersalWord: versalSymbol = true; break;
                case InputSubCategoryEnum.TextVersalPassage: versalSymbol = true; break;
                default: Logger.LogCF(string.Format(": Unexpected value of subcategory={0}", subCategory)); break;
            }
        }

        private TextVersalHandler()
        { }

        static public TextVersalHandler Create()
        {
            return new TextVersalHandler();
        }
    }
}
