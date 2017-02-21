using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Class for holding a simple name/value pair of meta information
    /// </summary>
    public class MetaInfoItem
    {
        private string name;
        private string value;

        // Prevent construction
        private MetaInfoItem()
        {}

        private MetaInfoItem(string name, string value)
        {
            this.name = name;
            this.value = value;            
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }
        }

        public static MetaInfoItem Create(string name, string value)
        {
            return new MetaInfoItem(name, value);
        }

        public static MetaInfoItem Create()
        {
            return new MetaInfoItem("","");
        }
    }


    /// <summary>
    /// Class for holding all meta information , such as file name, title, composer etc
    /// </summary>
    public class MetaInformation
    {
        // Conveniency: Init everything to empty strings!
        private MetaInfoItem fileName = MetaInfoItem.Create();
        private MetaInfoItem work = MetaInfoItem.Create();
        private MetaInfoItem movementTitle = MetaInfoItem.Create();
        private MetaInfoItem movementNumber = MetaInfoItem.Create();

        public MetaInfoItem FileName
        {
            get
            {
                return fileName;
            }
        }

        public MetaInfoItem Work
        {
            get
            {
                return work;
            }
        }

        public MetaInfoItem MovementTitle
        {
            get
            {
                return movementTitle;
            }
        }

        public MetaInfoItem MovementNumber
        {
            get
            {
                return movementNumber;
            }
        }


        // Prevent construction
        private MetaInformation()
        { }


        public static MetaInformation Create()
        {
            return new MetaInformation();
        }

    }
}
