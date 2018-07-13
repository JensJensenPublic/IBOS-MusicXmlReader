using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class UserSetting
    {
//        private bool value;
        private string name;
        private UserSettingsElement userSettingsElement;

        // Prevent construction
        private UserSetting()
        { }

        //private UserSetting(string name, bool value)
        //{
        //    this.value = value;
        //    this.name = name;

        //}

        private UserSetting(string name, UserSettingsElement userSettingsElement)
        {
            this.userSettingsElement = userSettingsElement;
//            this.value = true;
            this.name = name;

        }



        public bool Value
        {
            get
            {
                if (null != userSettingsElement) 
                {
                    return (userSettingsElement as UserSettingsElementBool).Value;
                }
                Logger.LogCF("");
                return false;
            }
            set
            {
                if (null != userSettingsElement)
                {
                    (userSettingsElement as UserSettingsElementBool).Value = value;
                    return;
                }
                Logger.LogCF("");
//                this.value  = value; 
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        //public static UserSetting Create(string name, bool value)
        //{
        //    return new UserSetting(name, value);
        //}

        public static UserSetting Create(string name, UserSettingsElementBool userSettingsElement)
        {
            return new UserSetting(name, userSettingsElement);
        }
    }
}
