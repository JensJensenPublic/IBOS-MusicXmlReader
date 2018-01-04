using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardTest
{
    // Manufacturer: 
    // Distributor: 
    // Comment:     Lars Petersens device

    class KeyboardPronto : Keyboard
    {
        string deviceName = "Pronto";


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

        internal KeyboardPronto()
        {
        }

    }
}
