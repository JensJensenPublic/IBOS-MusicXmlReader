using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardTest
{
    // Manufacturer:    HEMS
    // Distributor:     LVI
    // Comment:         Replacement for U2

    class KeyboardPolaris : Keyboard
    {
        private string deviceName = "Polaris";
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

        internal KeyboardPolaris()
        {
        }
        
    }
}
