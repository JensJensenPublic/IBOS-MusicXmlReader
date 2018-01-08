
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardTest
{
    // Manufacturer: HEMS
    // Distributor:  LVI
    // Comment: Very common, but no documentation is available !!

    public class KeyboardU2 : Keyboard
    {

        private string deviceName = "HIMS U2";
        public override string DeviceName { get { return deviceName; } }


        protected override PerkinsKeySequence GetPKS(Keys keys)
        {
            return PerkinsKeySequence.Create();
        }

        protected override PerkinsKeySequence GetPKS(KeySequenceList jawsKeySequence)
        {
            return PerkinsKeySequence.Create();
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
