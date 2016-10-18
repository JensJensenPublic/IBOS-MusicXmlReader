using System.Collections.Generic;
using System.Management;


// http://stackoverflow.com/questions/3331043/get-list-of-connected-usb-devices
// JSJ: Shows a number of USB devices, but nothing changes when the FOCUS 14 device is removed


namespace MusicXmlReaderModel
{
    static class DeviceInfo
    {
        static public void LogDeviceInfo()
        {
            var usbDevices = GetUSBDevices();

            foreach (var usbDevice in usbDevices)
            {
                Logger.Log(string.Format("Device ID: {0}, PNP Device ID: {1}, Description: {2}",
                    usbDevice.DeviceID, usbDevice.PnpDeviceID, usbDevice.Description));
            }
              
        }

        static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            //string searchString1 = @"Select * From Win32_USBHub"; // Does not show Braille device
            //string searchString2 = @"Select * From Win32_USBControllerDevice"; //Crashes
            //string searchString3 = @"Select * From Win32_PnPEntity"; // Device ID: USB\VID_0F4E&PID_0114\0123456, PNP Device ID: USB\VID_0F4E&PID_0114\0123456, Description: Focus 3 Braille Display USB
            string searchString4 = @"SELECT * FROM Win32_PnPEntity where DeviceID Like ""USB%"""; // As 3 but shows only USB devices

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(searchString4))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new USBDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description")
                ));
            }

            collection.Dispose();
            return devices;
        }
    }

    class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
    }
}

