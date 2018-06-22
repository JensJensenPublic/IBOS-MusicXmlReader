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
        protected int id; // MetaData representing the internal interpretation of this element
        public int Id { get { return id;} }

        public UserSettingsElement SetChild(UserSettingsElement userSettingsElement)
        {
            int index = userSettingsElement.id;
            if ((index < 0) || (index >= children.Length))
            {
                Logger.LogCF(string.Format(": XmlName={0} Index={1} is out of bounds [{2} to {3}[", this.xmlName, index, 0, children.Length));
                return null;
            }
            else
            {
                children[userSettingsElement.id] = userSettingsElement;
                return userSettingsElement;
            }
        }

        protected UserSettingsElement[] children;
        // The actual value of the element is represented in a derived class

        protected UserSettingsElement()
        {
        }

        protected UserSettingsElement(string xmlName, int id, int nChildren)
        {
            this.xmlName = xmlName;
            this.id = id;
            this.children = new UserSettingsElement[nChildren];
        }


        protected void ToXml(XmlWriter xml, string value)
        {

            //xml.WriteWhitespace("\r\n"); // Every new element starts at a new line
            xml.WriteStartElement(this.xmlName);
            //xml.WriteWhitespace("\r\n"); // Every new value starts at a new line
            xml.WriteAttributeString("id", this.id.ToString());
            xml.WriteValue(value);
            bool firstChild = true;           
            foreach (UserSettingsElement childElement in this.children)
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



        //public abstract void ToXml(XmlTextWriter xml); // Each subclass must know how to convert its own value to a string
        public abstract void ToXml(XmlWriter xml); // Each subclass must know how to convert its own value to a string


    }


    public class UserSettingsElementBool : UserSettingsElement
    {
        private bool value; // The actual value, typically represented by a checkbox in the User Interface

        public override void ToXml(XmlWriter xml)
        {
            base.ToXml(xml, this.value.ToString()); // Convert to string before calling the base class !
        }


        private UserSettingsElementBool() : base() { }// Prevent construction
        private UserSettingsElementBool(string xmlName, int id, int nChildren, bool value) : base(xmlName, id, nChildren)
        {
            this.value = value;
        }

        public static UserSettingsElementBool Create(string xmlName, int id, int nChildren, bool value)
        {
            return new UserSettingsElementBool(xmlName, id, nChildren, value);
        }

  

        public static UserSettingsElementBool Create(XmlNode xmlNode)
        {
            int id = -1;
            foreach (XmlAttribute attribute in xmlNode.Attributes)
            {
                if (attribute.Name == "id") id = int.Parse(attribute.Value);
            }

            bool value = true;

            UserSettingsElementBool result = UserSettingsElementBool.Create(xmlNode.Name, id, xmlNode.ChildNodes.Count, value);
            Logger.LogCF(string.Format(": Name={0} id={1} value={2} nChildren={3}", xmlNode.Name, id, value, xmlNode.ChildNodes.Count));
            //foreach (XmlNode childNode in xmlNode.ChildNodes)
            for (int i = 0; (i< xmlNode.ChildNodes.Count); i++)
            {
                XmlNode child = xmlNode.ChildNodes[i];
                if (child.NodeType == XmlNodeType.Element)
                {
                    result.SetChild(UserSettingsElementBool.Create(child));
                }
                if (child.NodeType == XmlNodeType.Text)
                {
                    if (!bool.TryParse(child.Value, out value))
                    {
                        value = true ;
                        Logger.LogCF(string.Format(": Defaulting to true"));
                    }
                }
            }

            return result;            
        }

    }

    public class UserSettingsElementVoid : UserSettingsElement
    {
        public override void ToXml(XmlWriter xml)
        {
            base.ToXml(xml,""); // This type has no value !
        }


        private UserSettingsElementVoid() : base() { }// Prevent construction
        private UserSettingsElementVoid(string xmlName, int id, int nChildren) : base(xmlName, id, nChildren)
        {
        }

        private UserSettingsElementVoid(string fileName) 
        {
            XmlDocument doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(fileName);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            doc.Load(reader); // This single operation may last decades of seconds on a slow platform!!
            foreach (XmlNode node in doc.ChildNodes)
            {
                Logger.LogCF(string.Format(": Node.Name={0}", node.Name));
                switch (node.Name)                {               
                    case "UserSettings":
                        UserSettingsElement userSettingsElement = UserSettingsElementBool.Create(node);

                        break;
                    default: break;
                }
            }

        }

        public static UserSettingsElementVoid Create(string xmlName, int id, int nChildren)
        {
            return new UserSettingsElementVoid(xmlName, id, nChildren);
        }

        public static UserSettingsElementVoid Create(string filename)
        {
            Logger.LogCF(".Entry");
            UserSettingsElementVoid result = new UserSettingsElementVoid(filename);
            Logger.LogCF(string.Format(".Exit"));
            return result;
        }


    

    }


}
