
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardTest
{
    // Manufacturer: HEMS
    // Distributor:  LVI
    // Comment: Very common, but no documentation is available !!

    class KeyboardU2 : Keyboard
    {

        private string deviceName = "HIMS U2";
        public override string DeviceName { get { return deviceName; } }

        protected override PerkinsKeySequence GetPKS(KeySequenceList jawsKeySequence)
        {
            return null;
        }

        protected override PerkinsKeySequence GetPKS(Keys keys)
        {
            return null;
        }


        protected override void TestDeviceSpecificFunctions(bool all)
        { }

        protected void TestKeyCombinations(bool all)
        { }

        internal KeyboardU2()
        {
        }

    }
}
