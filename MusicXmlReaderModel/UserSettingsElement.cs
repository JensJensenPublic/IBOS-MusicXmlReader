using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Represents an XML element used for holding user settings information
    /// </summary>
    public abstract class UserSettingsElement
    {
        protected string xmlName; // Unlocalized name used for identification when serialized
        public string XmlName { get { return xmlName; } }

        private Type dataType; // The datatype of the element represented
  
        public UserSettingsElement AddChild(UserSettingsElement userSettingsElement)
        {
            childList.Add(userSettingsElement);
            return userSettingsElement;
        }

        protected List<UserSettingsElement> childList;
        // The actual value of the element is represented in a derived class

        protected UserSettingsElement()
        {
        }

        protected UserSettingsElement(string xmlName,Type type)
        {
            this.xmlName = xmlName;
            this.dataType = type;
            this.childList = new List<UserSettingsElement>();
        }
        
        protected void ToXml(XmlWriter xml, string value)
        {
            //xml.WriteWhitespace("\r\n"); // Every new element starts at a new line
            xml.WriteStartElement(this.xmlName);
            //xml.WriteWhitespace("\r\n"); // Every new value starts at a new line
            xml.WriteAttributeString("Type", this.dataType.ToString());
            xml.WriteValue(value);
            bool firstChild = true;
            foreach (UserSettingsElement childElement in this.childList)
            {
                if (firstChild)
                {
                    xml.WriteWhitespace("\r\n"); // Insert new line before first child
                    firstChild = false;
                }
                if (null != childElement)
                {
                    childElement.ToXml(xml);
                }
            }

            xml.WriteEndElement();
            xml.WriteWhitespace("\r\n"); // Every new element ends at a new line
        }


        /// <summary>
        /// For checking serialization
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public  bool IsEqualTo(UserSettingsElement that)
        {

            Logger.LogCF(string.Format(": Comparing this.xmlName={0} to that.xmlName={1}", this.xmlName, that.xmlName));

            if (0 != string.Compare(this.xmlName, that.xmlName))
            {
                Logger.LogCF(string.Format(": this.xmlName={0} that.xmlName={1}", this.xmlName, that.xmlName));
                return false;
            }
 
            if (this.childList.Count() != that.childList.Count())
            {
                Logger.LogCF(string.Format(": this.childList.Count={0} that.childList.Count={1}", this.childList.Count(), that.childList.Count()));
                return false;
            }

            for (int i = 0; (i < this.childList.Count()); i++)
            {
                if (!this.childList[i].IsEqualTo(that.childList[i]))
                {
                    return false;
                }
            }

   

            return true;

        }

        public static UserSettingsElement Create(string xmlName)
        {
            return new UserSettingsElementVoid(xmlName,"");
        }

        public static UserSettingsElement Create(string xmlName, bool value)
        {
            return new UserSettingsElementBool(xmlName, value);
        }

        public static UserSettingsElement Create(string xmlName, int value)
        {
            return new UserSettingsElementInt(xmlName, value);
        }

        public static UserSettingsElement Create(string xmlName,string dummy )
        {
            return new UserSettingsElementVoid(xmlName,"");
        }


        /// <summary>
        /// Build a tree of UserSettingElements corresponding to the tree represented by xmlNode
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static UserSettingsElement CreateFromNode(XmlNode xmlNode)
        {

            UserSettingsElement result = null;
            string name =  xmlNode.Attributes[0].Name.ToString();
            string dataType = xmlNode.Attributes[0].Value.ToString();
            switch (dataType)
            {
               // case "System.Boolean": result = new UserSettingsElementBool(xmlNode.Name, (null == xmlNode.Value) ? true : false); break; // Hack !!
                case "System.Boolean": result = new UserSettingsElementBool(xmlNode.Name, xmlNode.Value); break; 
                case "System.String": result = new UserSettingsElementVoid(xmlNode.Name, ""); break;
                case "System.Int32": result = new UserSettingsElementInt(xmlNode.Name, xmlNode.Value); break;
                default: Logger.LogCF(string.Format("Unsupported datatype={0}", dataType)); break;
                //case "void": result = new UserSettingsElementVoid(xmlNode.Name,""); break;

            }

            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                // Logger.LogCF(string.Format("Child.Name={0}", child.Name));
                if (child.NodeType == XmlNodeType.Element)
                {
                    result.AddChild(UserSettingsElement.CreateFromNode(child));
                }
                if (child.NodeType == XmlNodeType.Text)
                {
                    //if (!bool.TryParse(child.Value, out result.value))
                    //{
                    //    result.value = true;
                    //    Logger.LogCF(string.Format(": Defaulting to true"));
                    //}
                }
            }
            //Logger.LogCF(string.Format(": Name={0}  value={1} ", xmlNode.Name, result.value));
            return result;
        }

        /// <summary>
        /// Creates  a UserSettingsElement representing a full user settings tree  from an xmlfile
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static UserSettingsElement CreateFromFile(string fileName)
        {
            Logger.LogCF(".Entry");
            UserSettingsElement result = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader reader = new XmlTextReader(fileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                doc.Load(reader); // This single operation may last decades of seconds on a slow platform!!
                foreach (XmlNode node in doc.ChildNodes)
                {
                    Logger.LogCF(string.Format(": Node.Name={0}", node.Name));
                    switch (node.Name)
                    {
                        case UserSettingNames.UserSettings: result = UserSettingsElement.CreateFromNode(node); break;
                        default: break; //  Logger.LogCF(string.Format(": node.Name={0}", node.Name)); break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogCF(string.Format(": Exception.Message={0}", e.Message));
                return null;
            }

            Logger.LogCF(string.Format(".Exit"));
            return result;
        }







        //public abstract void ToXml(XmlTextWriter xml); // Each subclass must know how to convert its own value to a string
        public abstract void ToXml(XmlWriter xml); // Each subclass must know how to convert its own value to a string


    }


    //*****************************************************************************************************************
    // Classes derived from UserSettingsElement
    //*****************************************************************************************************************


    public class UserSettingsElementBool : UserSettingsElement
    {
        private bool value; // The actual value, typically represented by a checkbox in the User Interface

        public override void ToXml(XmlWriter xml)
        {
            base.ToXml(xml, this.value.ToString()); // Convert to string before calling the base class !
        }
        private UserSettingsElementBool() : base() { }// Prevent construction
        public UserSettingsElementBool(string xmlName, bool value) : base(xmlName, value.GetType())
        {
            this.value = value;
        }



        public UserSettingsElementBool(string xmlName, string value): base (xmlName,bb.GetType()) // We can not call GetType() on value !
        {
            if (!Boolean.TryParse(value, out this.value))
            {
                this.value = true; // Dafault to true
            }
        }

        static bool bb = false; // Only used for calling bb.GetType //
    }




    public class UserSettingsElementVoid : UserSettingsElement
    {
        public override void ToXml(XmlWriter xml)
        {
            base.ToXml(xml,"VOID"); // This type has no value !
        }        
        private UserSettingsElementVoid() : base() { }// Prevent construction
        public UserSettingsElementVoid(string xmlName, string dummy) : base(xmlName,dummy.GetType()) 
        {
            // No value to set !
        }
    }


    public class UserSettingsElementInt : UserSettingsElement
    {
        private int value; // The actual value, 
        public override void ToXml(XmlWriter xml)
        {
            base.ToXml(xml, this.value.ToString()); // Convert to string before calling the base class !
        }
        private UserSettingsElementInt() : base() { }// Prevent construction
        public UserSettingsElementInt(string xmlName, int value) : base(xmlName, value.GetType())
        {
            this.value = value;
        }
        public UserSettingsElementInt(string xmlName, string value) : base(xmlName, value.GetType())
        {
            this.value = int.Parse(value);
        }

    }

}
