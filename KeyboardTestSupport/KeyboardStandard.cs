using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardTest
{
    public class KeyboardStandard : Keyboard
    {

        private string deviceName = "Standard";
        public override string DeviceName { get { return deviceName; } }

        protected override PerkinsKeySequence GetPKS(KeySequenceList jawsKeySequence)
        {
            return null;
        }

        protected override PerkinsKeySequence GetPKS(Keys keys)
        {
            return null;
        }


        internal KeyboardStandard() : base()
        {
        }

    }
}
