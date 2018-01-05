using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace KeyboardTest
{

    public enum KeyboardNameEnum { Unknown, Focus14, Braillant, HimsEdge, BrailleNoteTouch }

    public class Utilities
    {
        /// <summary>
        /// Converts a Windows.Forms.Keys to a string.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string KeysToString(Keys keys)
        {
            if (Keys.None == keys) return SimpleKeyToString(keys);
            string control = "CONTROL";
            string alt = "ALT";
            string shift = "SHIFT";
            string controlString = (0 != (keys & Keys.Control)) ? control : "";
            string altString = (0 != (keys & Keys.Alt)) ? alt : "";
            string shiftString = (0 != (keys & Keys.Shift)) ? shift : "";
            string charString = SimpleKeyToString(keys);
            //if ((simpleKey >= Keys.D0) && (simpleKey <= Keys.D9))
            //{
            //    char c = (char)('0' + (char)(simpleKey - Keys.D0));
            //    charString = c.ToString();
            //}
            return Concatenate (controlString , altString , shiftString , charString);
        }

        public static string SimpleKeyToString(Keys keys)
        {
            Keys simpleKey = keys & ~(Keys.Control | Keys.Alt | Keys.Shift);
            string none = "";
            string space = "MELLEMRUM";
            switch (simpleKey)
            {
                case Keys.None:     return none;                    // Needs special handling
                case Keys.Space:    return space;                   // Needs special handling
                default:            return simpleKey.ToString();    // For the time being just use the standard ToString()
            }
        }

        public static string KeyListToString(List<Keys> keyList)
        {
            if (null == keyList) return "";
            string concatenator = "";
            StringBuilder sb = new StringBuilder();
            foreach (Keys keys in keyList)
            {
                sb.Append(concatenator);
                sb.Append(KeysToString(keys));
                concatenator = "+";
            }
            return sb.ToString();
        }


        public static string ConcatenateByString(string a, string b, string concatenator)
        {
            if (string.IsNullOrEmpty(a)) return b;
            if (string.IsNullOrEmpty(b)) return a;
            return a + concatenator + b;
        }


        public static string ConcatenateByPlus(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b;
            if (string.IsNullOrEmpty(b)) return a;
            return a + "+" + b;
        }

        public static string Concatenate(string a, string b, string c, string d)
        {
            return ConcatenateByPlus(ConcatenateByPlus(a,b), ConcatenateByPlus(c,d));
        }
        

        public static void LogJawsConfiguration()
        {
            string defaultConfigFile = @"C: \Users\Jens\AppData\Roaming\Freedom Scientific\JAWS\17.0\Settings\dan\default.jcf";
            string text = "JAWS konfigurationsfil";
            try
            {
                if (File.Exists(defaultConfigFile))
                {
                    //FileStream configurationStream = File.OpenRead(defaultConfigFile);
                    StreamReader configurationStream = File.OpenText(defaultConfigFile);
                    //File.
        
                    Console.WriteLine(String.Format("Start på {0} ({1})", text, defaultConfigFile));
                    string line = "";
                    while (null != line)
                    {
                        line = configurationStream.ReadLine();
                        if (null != line)
                        {
                            Console.WriteLine(string.Format("  {0}", line));
                        }
                    }
                    Console.WriteLine(String.Format("Slut på {0}", text));
                }
                else
                {
                    Console.WriteLine(String.Format("JAWS konfigurationsfil eksisterer ikke {0}", defaultConfigFile));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Fejl under læsning af {0} {1} {2}",text, defaultConfigFile,e.Message));
            }

        }

        public static KeyboardNameEnum LogUsbDevices()
        {
            List<PlatformDependencies.USBDeviceInfo> usbDevices = PlatformDependencies.DeviceInfo.GetUSBDevices();
            Console.WriteLine(string.Format("Found {0} devices:", usbDevices.Count));
            foreach (var usbDevice in usbDevices)
            {
                Console.WriteLine(string.Format("Device ID: {0}, PNP Device ID: {1}, Description: {2}",
                    usbDevice.DeviceID, usbDevice.PnpDeviceID, usbDevice.Description));
            }

            foreach (var usbDevice in usbDevices)
            {
                string s = usbDevice.Description.ToUpper();
                if (s.ToUpper().Contains("FOCUS")) return KeyboardNameEnum.Focus14;
                if (s.ToUpper().Contains("HIMS")) return KeyboardNameEnum.HimsEdge;
                if (s.ToUpper().Contains("BRAILLENOTE TOUCH")) return KeyboardNameEnum.BrailleNoteTouch;
                //..
            }
            return KeyboardNameEnum.Unknown;
        }



        // Sample lines from above:
        // Device ID: USB\VID_0F4E&PID_0114\0123456, PNP Device ID: USB\VID_0F4E&PID_0114\0123456, Description: Focus 3 Braille Display USB
        // Device ID: USB\VID_045E&PID_930B\5&26E377A5&0&3, PNP Device ID: USB\VID_045E&PID_930B\5&26E377A5&0&3, Description: HIMS USB Driver


    }
    }
