using System;
using System.Xml;
using System.Text;

namespace BrailleMusicDecoder.MusicXmlElements
{
    /// <summary>
    /// Conveniense methods for manipulation af MusicXml NoteElements
    /// </summary>
    public class MusicXmlNoteElement : MusicXmlElement
    {
        /// <summary>
        /// Convenience method for modifying the octave number within a NoteElement
        /// To be used when adding chord notes marked with a specific octave number
        /// </summary>
        /// <param name="newOctaveNumber"></param>
        public void ModifyOctaveNumber(int newOctaveNumber)
        { 
            XmlNode pitchNode = this.SelectSingleNode("pitch");
            if (null == pitchNode)
            {
                LogCF(string.Format(": No PitchNode found. NewOctaveNumber={0}", newOctaveNumber));
                return;
            }
            XmlNode octaveNode = pitchNode.SelectSingleNode("octave");
            string octaveString = octaveNode.InnerText;
            int existingOctaveNumber = int.Parse(octaveString);
            if (newOctaveNumber != existingOctaveNumber)
            {
                LogCF(string.Format(": Modifying OctaveNumber from {0} to {1}", existingOctaveNumber, newOctaveNumber));
                octaveNode.InnerText = newOctaveNumber.ToString();
            }
        }

        public void RemoveNamedChild(string childName)
        {
            XmlNode childNode = this.SelectSingleNode(childName);
            if (null != childNode)
            {
                this.RemoveChild(childNode);
            }
        }

        public void RemoveGrandChildren(string childName)
        {
            XmlNode childNode = this.SelectSingleNode(childName);
            if (null != childNode)
            {
                childNode.RemoveAll();
                childNode.Attributes.RemoveAll();
            }
        }



        /// <summary>
        /// Convenience method for modifying the pitchElement within a NoteElement
        /// </summary>
        /// <param name="interval">The interval as specified in the MusicBraille file</param>
        /// <param name="direction">The directio of the interval, typically down for right hand and up for left hand </param>
        public void ModifyPitch(InputSubCategoryEnum interval, IntervalDirectionEnum direction, MusicXmlBuilderAccidentalHandler accidentalHandler)
        {
            // Find the relevalt nodes
            XmlNode pitchNode = this.SelectSingleNode("pitch");
            if (null == pitchNode)
            {
                LogCF(string.Format(": No PitchNode found. Interval={0} Direction={1}", interval, direction));
                return;
            }
            XmlNode stepNode = pitchNode.SelectSingleNode("step");
            XmlNode octaveNode = pitchNode.SelectSingleNode("octave");
            // Read the relavant values
            string stepString = stepNode.InnerText;
            string octaveString = octaveNode.InnerText;
            int octaveInt = int.Parse(octaveString);
            int fullStepInt = ToInt(stepString);
            int intervalSize = ToInt(interval);
            // Compute the new values
            int delta = (direction == IntervalDirectionEnum.up) ? intervalSize : -intervalSize;
            int newFullStep = Modulus7(fullStepInt + delta, ref octaveInt); // Get a number [0..6]

            accidentalHandler.Set(newFullStep);
            int alterValue = accidentalHandler.GetAlter(newFullStep);

            // Write the new values back
            stepNode.InnerText = ToString(newFullStep);
            octaveNode.InnerText = octaveInt.ToString();

            // Handle Alter
            XmlNode alterNode = pitchNode.SelectSingleNode("alter");
            if (null != alterNode)
            {
                // Use the accidentalHandler to compute the new accidental !!
                // The original had an alternode
                if (alterValue == 0)
                {
                    pitchNode.RemoveChild(alterNode);
                }
                else
                {
                    alterNode.InnerText = alterValue.ToString(); // To be implemented
                }
            }
            else
            {
                // The original han no alternode
                if (alterValue != 0)
                {
                    // Add an AlterNode
                    XmlNode newAlterNode = elementFactory.NameValuePair("alter", alterValue.ToString());
                    XmlNode theStepNode = pitchNode.SelectSingleNode("step");
                    pitchNode.InsertAfter(newAlterNode,theStepNode); 
                }
            }
        }

        #region debugtools
        private string GetDebugInnerText(string childName)
        {
            XmlNodeList childNodes = this.SelectNodes(childName);
            switch (childNodes.Count)
            {
                case 0:  return string.Format("'{0}': UNDEFINED", childName);
                case 1:  return string.Format("'{0}'='{1}'", childName, childNodes[0].InnerText);
                default: return string.Format("'{0} ({1} OCCURANCES)", childName,childNodes.Count); 
            }
        }

        private string GetDebugAttributes(string childName, string attributeName)
        {
            XmlNodeList childNodes = this.SelectNodes(childName);
            switch (childNodes.Count)
            {
                case 0: return string.Format("'{0}'=UNDEFINED", childName);
                case 1: return string.Format("'{0}'.Attributes('{1}')={2}", childName, attributeName, childNodes[0].Attributes.GetNamedItem(attributeName).Value);
                default:
                    // More than one childelement with the given childName exists. Accumulate the value of the specified attribute across the children.
                    StringBuilder attributeValues = new StringBuilder();
                    foreach (XmlNode childNode in childNodes)
                    {
                        string value = childNode.Attributes.GetNamedItem(attributeName).Value;
                        attributeValues.Append(value + " ");
                    }
                    return string.Format("'{0}' ({1} OCCURANCES).Attributes('{2}')={3} ", childName, childNodes.Count,attributeName, attributeValues.ToString());
            }
        }

        
        private string GetDebugMeasureNumberString()
        {
            XmlNode parentNode = this.ParentNode;
            XmlNode parentNodeNumberAttribute =  parentNode.Attributes.GetNamedItem("number");
            string measureNumber = parentNodeNumberAttribute.Value;
            return string.Format("Measure={0} ", measureNumber);
        }

        private string ToDebugString()
        {
            string measure = this.GetDebugMeasureNumberString();
            string pitch = this.GetDebugInnerText("pitch");
            string duration = this.GetDebugInnerText("duration");
            string tie = this.GetDebugAttributes("tie","type");
            string notations = this.GetDebugInnerText("notations");
            string result =  measure + " " + pitch + " " + duration + " " + tie + " "  + notations;
            return result;
        }

        #endregion

        public void AddTieAndTied(MusicXmlElementFactory e, string startOrStop)
        {
            string atEntry = ".Entry: " + this.ToDebugString();
            LogCF(atEntry);

            // The sound is represented by the "tie" element, placed directly under the note
            // Find a "previousElement" to place the tie Element after.
            // Most notes contain a durationelement, but grace notes do NOT.
            // The following code assumes that either a durationElement or a pitchElement is always there!
            XmlNode previousElement = SelectSingleNode("duration"); 
            if (null == previousElement)
            {
               previousElement = SelectSingleNode("pitch"); 
            }
            XmlNode tie = e.TieElement(startOrStop);
            InsertAfter(tie, previousElement);
            // The graphics is represented by the "tied" element placed within the note's notations element
            XmlNode tied = e.TiedElement(startOrStop);
            XmlNode notations = SelectSingleNode("notations");
            notations.AppendChild(tied);

            string atExit = ".Exit:  " + this.ToDebugString();
            LogCF(atExit);
        }


        public void AddSlur(MusicXmlElementFactory e, string startOrStop)
        {  
            // The graphics is represented by the "slur" element placed within the note's notations element
            XmlNode tied = e.SlurElement(startOrStop);
            XmlNode notations = SelectSingleNode("notations");
            notations.AppendChild(tied);
        }


        // Implement some simple fullstep arithmetics for handling interval notation

        private int Modulus7(int fullStep, ref int octave)
        {
            int result = fullStep % 7;
            int carry = fullStep / 7;
            octave += carry;
            if (result >= 0) return result;
            octave--;
            return result + 7;
        }

        private int ToInt(string step)
        {
            switch (step)
            {
                case "C": return 0;
                case "D": return 1;
                case "E": return 2;
                case "F": return 3;
                case "G": return 4;
                case "A": return 5;
                case "B": return 6;
            }
            throw new Exception(string.Format("Invalid step={0}", step));
        }
        
        private string ToString(int step)
        {
            switch (step)
            {
                case 0: return "C";
                case 1: return "D";
                case 2: return "E";
                case 3: return "F";
                case 4: return "G";
                case 5: return "A";
                case 6: return "B";
            }
            throw new Exception(string.Format("Invalid step={0}", step));
        }

        private int ToInt(InputSubCategoryEnum interval)
        {
            switch (interval)
            {
                case InputSubCategoryEnum.IntervalSecond: return 1; // C to D 
                case InputSubCategoryEnum.IntervalThird: return 2;  // C to E
                case InputSubCategoryEnum.IntervalFourth: return 3; // C to F
                case InputSubCategoryEnum.IntervalFifth: return 4; // C to G
                case InputSubCategoryEnum.IntervalSixth: return 5; // C to A
                case InputSubCategoryEnum.IntervalSeventh: return 6; // C to B
                case InputSubCategoryEnum.IntervalOctave: return 7; // C to C          
            }
            throw new Exception(string.Format("Invalid interval={0}", interval.ToString()));
        }


        private MusicXmlElementFactory elementFactory;
        public MusicXmlElementFactory ElementFactory { get { return elementFactory; } set { elementFactory = value; } }


        // Should only be called from MusicXmlDocument.CreateMusicXmlNoteElement  when a document is loaded from a file
        internal MusicXmlNoteElement(string s0, string s1, string s2, XmlDocument doc) : base(s0, s1, s2, doc)
        { }

        // Called when building a MusicXmlDocument, for instance from the MusicXmlBuilder class.
        internal MusicXmlNoteElement(string name, XmlDocument doc) : base("",name,"", doc)
        { }


        //private MusicXmlNoteElement(XmlNode xmlNode) : base(xmlNode)
        //{
        //}

        //public static new MusicXmlNoteElement Create(XmlNode xmlNode)
        //{
        //    return new MusicXmlNoteElement(xmlNode);
        //}

    }
}
