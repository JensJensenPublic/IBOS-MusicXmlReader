using System;
using System.Xml;

namespace MusicXmlReaderModel
{

    public class ForwardElement : Element
    {


        int duration;
        int divisions;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ForwardElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ForwardElement(XmlNode node, int divisions)
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


        public Int64 DurationInCommonDivisions
        {
            get
            {
                return duration * NoteElement.commonDivisions / divisions;
            }
        }


        public static  ForwardElement Create(XmlNode node, int divisions)
        {
            return new ForwardElement(node, divisions);
        }

        public override string ToString()
        {
            return string.Format("Forward {0} -------------------------------->", DurationInCommonDivisions);

        }
    }
}


