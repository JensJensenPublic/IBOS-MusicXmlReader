using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{



    /// <summary>
    /// For interpreting pairs of dot2356 as a pair of parentesis
    /// </summary>
    class ParentesisHandler
    {

        private TextItem leftParentesis; // For locating pairs of parentesis dot2356 ..... dot2356

        public void Clear()
        {
            leftParentesis = null;
        } 
        
        public void OnCharacter(TextItem textItem)
        {
            if (0 != string.Compare("/", textItem.Text)) return;
            OnDot2356(textItem);
        }

        public void OnDenominator(TextItem textItem)
        {
            if (0 != string.Compare("/7", textItem.Text)) return;
            {
                // The symbol for "/7" is identical to the distributed symbol for parentesis 
                OnDot2356(textItem);
            }
        }

        private void OnDot2356(TextItem textItem)
        {
            if (null == leftParentesis)
            {
                leftParentesis = textItem;
            }
            else
            {
                leftParentesis.Text = "(";
                textItem.Text = ")";
                leftParentesis = null;
            }
        }


private ParentesisHandler()
        {
        }

        public static ParentesisHandler Create()
        {
            return new ParentesisHandler();
        }
    }
}
