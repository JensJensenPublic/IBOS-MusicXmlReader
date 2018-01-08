using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTest
{

    /// <summary>
    /// Enumerates all Device-specific keys for all supported device types.
    /// Some keys are found on all device types, some on a subset of device types and some on a single device only.
    /// </summary>
    public enum DSKey
    {
        MELLEMRUM = 0,      // Probably available on all devices
        DOT1 = 1,
        DOT2 = 2,
        DOT3 = 3,
        DOT4 = 4,
        DOT5 = 5,
        DOT6 = 6,
        DOT7 = 7,
        DOT8 = 8,
        // FOCUS 14 Blue
        // https://www.freedomscientific.com/Content/Documents/Manuals/Focus/Focus14Blue/Focus14-Blue-Users-Guide.pdf
        // Buttons on the front from left to right
        //-----------------------------------------
        LeftSelectorButton9a =   9,
        LeftRockerBar10aDown = 10,
        LeftRockerBar10aUp  = 11,
        LeftPanningButton11a =  12,
        LeftShiftKey12a  =      13, // used in conjunction with the SPACEBAR, braille keys, and other controls to enter commands)
        RightShiftkey12b =      14, //(used in conjunction with the SPACEBAR, braille keys, and other controls to enter commands)
        RightPanningButton11b = 15 ,
        RightRockerBar10bDown = 16,
        RightRockerBar10bUp   = 17,
        RightSelectorButton9b = 18,
        // HIMS EDGE
        // The 8 black buttons, numbered from left to right
        EdgeESC = 19,
        EdgeTAB = 20,
        EdgeCTRL = 21,
        EdgeALT = 22,
        EdgeSHIFT = 23,
        EdgeINSERT = 24,
        EdgeWINDOWS = 25,
        EdgeAPPLICATION = 26,

    };

    public class DeviceSpecificKeys
    {
        public static string ToString(int i)
        {
            DSKey dsKey = (DSKey)i;
            switch (dsKey)
            {
                case DSKey.MELLEMRUM: return "MELLEMRUM";
                case DSKey.DOT1: return "1";
                case DSKey.DOT2: return "2";
                case DSKey.DOT3: return "3";
                case DSKey.DOT4: return "4";
                case DSKey.DOT5: return "5";
                case DSKey.DOT6: return "6";
                case DSKey.DOT7: return "7";
                case DSKey.DOT8: return "8";
                default: return dsKey.ToString();
            }  

        }
    }
}
