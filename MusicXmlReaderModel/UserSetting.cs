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

        private UserSetting(bool value, string name)
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
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public static UserSetting Create(bool value, string name)
        {
            return new UserSetting(value, name);
        }
    }
}
