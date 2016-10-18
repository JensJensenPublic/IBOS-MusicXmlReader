using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MusicXmlReaderUI;


namespace MusicXmlReaderModel
{

    public class BackupElement : Element
    {


        int duration;
        int divisions;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private BackupElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private BackupElement(XmlNode node,int divisions)
        {
            this.divisions = divisions;
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "duration":
                        duration = int.Parse(n.InnerText); break;
                }
            }
        }

        public int Duration
        {
            get
            {
                return duration;
            }
        }


        public int DurationInCommonDivisions
        {
            get
            {
                return duration * NoteElement.commonDivisions / divisions;
            }
        }


        public static BackupElement Create(XmlNode node,int divisions)
        {
            return new BackupElement(node,divisions);
        }

        public override string ToString()
        {
            return string.Format("Backup {0} <--------------------------------", DurationInCommonDivisions);

        }
    }
}


