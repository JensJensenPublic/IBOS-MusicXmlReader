using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyboardTest;

namespace KeyboardDemo
{
    public interface IKeyboardSelectorClient
    {
        void WriteKeyboardName(KeyboardNameEnum keyboardName);
        KeyboardNameEnum ReadKeyboardName();
    }
}
