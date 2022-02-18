using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Class for holding a simple name/value pair of meta information
    /// The class is heavily inspired by the definitions found at 
    /// 
    /// http://braillespecs.github.io/pef/pef-specification.html
    /// 
    /// This document (amongst other information) contains the following list of the 15 Dublin Core elements: 
    ///
    ///dc:format(mandatory)
    ///dc:identifier(mandatory)
    ///dc:title(optional)
    ///dc:creator(zero or more)
    ///dc:subject(zero or more)
    ///dc:description(optional)
    ///dc:publisher(zero or more)
    ///dc:contributor(zero or more)
    ///dc:date(optional)
    ///dc:type(zero or more)
    ///dc:source(zero or more)
    ///dc:language(zero or more)
    ///dc:relation(zero or more)
    ///dc:coverage(zero or more)
    ///dc:rights(zero or more)
    ///
    /// These elements are collected from several parts of the application.
    /// Some of them primarily depend on the score currently being processed, while others are common for all scores.
    /// 
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

        public override string ToString()
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            string result = string.Format("{0}:{1}", name, value);
            return result;
        }
    }

    public class DublinCore
    {
        // The following 15 items are found in the PEF specification
        // Conveniency: Init everything to empty strings!
        private MetaInfoItem format = MetaInfoItem.Create();     // Found in NOTA sample files with constant value "application/x-pef+xml"
        private MetaInfoItem identifier = MetaInfoItem.Create(); // Found in NOTA sample files with sample value "390120"
        private MetaInfoItem title = MetaInfoItem.Create();      // Found in NOTA sample files with sample value "Imudico's melodibog 28, for el-orgel, klaver og guitar med becifring og akkord-diagrammer (udeladt i punktudgaven)"
        private MetaInfoItem creator = MetaInfoItem.Create();
        private MetaInfoItem subject = MetaInfoItem.Create();
        private MetaInfoItem description = MetaInfoItem.Create();
        private MetaInfoItem publisher = MetaInfoItem.Create();
        private MetaInfoItem contributor = MetaInfoItem.Create();
        private MetaInfoItem date = MetaInfoItem.Create();       // Found in NOTA sample files with sample value "2021-10-05"
        private MetaInfoItem type = MetaInfoItem.Create();
        private MetaInfoItem source = MetaInfoItem.Create();
        private MetaInfoItem language = MetaInfoItem.Create();
        private MetaInfoItem relation = MetaInfoItem.Create();
        private MetaInfoItem coverage = MetaInfoItem.Create();
        private MetaInfoItem rights = MetaInfoItem.Create();

        // Accessors

        /// <summary>
        /// From the MusicXml "creator" element
        /// </summary>
        public MetaInfoItem Creator { get { return creator; } set { creator = value; } }
        /// <summary>
        /// From the MusocXml "source" element
        /// </summary>
        public MetaInfoItem Source { get { return source; } set { source = value; } }

        private DublinCore()
        { }

        public static DublinCore Create()
        {
            return new DublinCore();
        }

    }




    /// <summary>
    /// Class for holding all meta information , such as file name, title, composer etc
    /// This class is intended for collecting the information from various sources and then passing it
    /// as a parameter to the BrailleFileHandler
    /// </summary>
    public class MetaInformation
    {
        // Conveniency: Init everything to empty strings!

        private DublinCore dublincore = DublinCore.Create(); // Holds exactly the 15 items defined by DublinCore
        public DublinCore DublinCore { get { return dublincore; } }

        // The following items are found in the MusicXml file (See MusicXmlInterpretor.cs) but are not part of the official pef definition
        private MetaInfoItem fileName = MetaInfoItem.Create();
        private MetaInfoItem work = MetaInfoItem.Create();
        private MetaInfoItem movementTitle = MetaInfoItem.Create();
        private MetaInfoItem movementNumber = MetaInfoItem.Create();
        private MetaInfoItem encoding = MetaInfoItem.Create();

        // Accessors:

        public MetaInfoItem FileName { get { return fileName; } set { fileName = value; } }
        /// <summary>
        /// From the MusicXml "work" element
        /// </summary>
        public MetaInfoItem Work { get { return work; } set { work = value; } }
        /// <summary>
        /// From the MusicXml "movement-title" element
        /// </summary>
        public MetaInfoItem MovementTitle { get { return movementTitle; } set { movementTitle = value; } }
        /// <summary>
        /// From the MusicXml "movement-number" element
        /// </summary>
        public MetaInfoItem MovementNumber { get { return movementNumber; } set { movementNumber = value; } }
        /// <summary>
        /// From the MusicXml "encoding-description" element
        /// </summary>
        public MetaInfoItem Encoding { get { return encoding; } set { encoding = value; Logger.CurrentEncoding = encoding.ToString(); } }
   

        // Prevent construction
        private MetaInformation()
        { }


        public static MetaInformation Create()
        {
            return new MetaInformation();
        }

    }
}
