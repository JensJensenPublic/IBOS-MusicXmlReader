using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderUI
{
    //class NotationElement
    //{

    //    private string tiedType = "";

    //    /// <summary>
    //    /// To force the use of the Create() method
    //    /// </summary>
    //    private NotationElement()
    //    { }

        

    //    /// <summary>
    //    /// Private constructor, used by the Crate() method
    //    /// </summary>
    //    /// <param name="node"></param>
    //    private NotationElement(XmlNode node)
    //    {

    //        // Dig out elements
    //        foreach (XmlNode n in node.ChildNodes)
    //        {
    //            switch (n.Name)
    //            {
    //                case "tied":
    //                    foreach (XmlAttribute a in n.Attributes)
    //                    {
    //                        switch (a.Name)
    //                        {
    //                            case "type": tiedType = a.Value;
    //                                break;
    //                        }
    //                    }
    //                    break; 
    //            }
    //        }   
    //    }

    //    public string TiedType
    //    {
    //        get
    //        {
    //            return tiedType;
    //        } 
    //    }

    //    public static NotationElement Create(XmlNode node)
    //    {
    //        return new NotationElement(node);
    //    }

    //    public override string ToString() 
    //    {
    //        return string.Format("Notation.tiedType {0}", tiedType);
    //    }
    //}

}

