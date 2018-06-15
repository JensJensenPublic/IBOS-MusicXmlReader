using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class UserSetting
    {
        private bool value;
        private string name;

        // Prevent construction
        private UserSetting()
        { }

        private UserSetting(string name, bool value)
        {
            this.value = value;
            this.name = name;

        }

        public bool Value
        {
            get
            {
                return value;
            }
            set
            {
               this.value  = value; 
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public static UserSetting Create(string name, bool value)
        {
            return new UserSetting(name, value);
        }
    }
}
