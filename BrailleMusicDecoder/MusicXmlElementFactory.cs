using System;
using System.Xml;
using BrailleMusicDecoder.MusicXmlElements;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{
    public enum IntervalDirectionEnum { up, down }

    public class MusicXmlElementFactory
    {
        internal MusicXmlDocument doc; // The MusicXmlDocument used for creating and organizing all MusicXmlElement objects 

        // To be placed directly under the note element
        internal XmlNode TimeModificationElement(int actualNotes, int normalNotes)
        {
            // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-time-modification.htm
            XmlNode result = doc.CreateElement("time-modification"); // An Xml element named "time-modification"
            result.AppendChild(NameValuePair("actual-notes", actualNotes.ToString()));
            result.AppendChild(NameValuePair("normal-notes", normalNotes.ToString()));
            return result;
        }

        // To be placed under the Notations element under the note element
        internal XmlNode TupletBracket(int number, string bracketType)
        {
            // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-tuplet.htm
            XmlNode result = doc.CreateElement("tuplet");
            result.Attributes.Append(NameValueAttribute("bracket", "yes"));
            result.Attributes.Append(NameValueAttribute("number", number.ToString()));
            result.Attributes.Append(NameValueAttribute("placement", "above"));
            result.Attributes.Append(NameValueAttribute("type", bracketType));
            return result;
        }

        internal XmlNode AttributesElement(int divisions,int beats, int beatType)
        {
            XmlNode result = doc.CreateElement("attributes"); // This is an Xml Element named "Attributes" 
            result.AppendChild(NameValuePair("divisions", divisions.ToString()));
            result.AppendChild(KeyElement(0)); // Use a default value for C / Am for the case that the Braille file does not define the Key
            result.AppendChild(TimeElement(beats,beatType)); // Use 4/4 as a default until overwritten by a real value from the Music Braille file
            result.AppendChild(doc.CreateElement("clef"));// Pure placeholder
            return result;
        }

        internal XmlNode EmptyAttributesElement()
        {
            XmlNode result = doc.CreateElement("attributes"); // This is an Xml Element named "Attributes" 
            return result;
        }

        internal XmlNode TieElement(string startStopAttribute)
        {
            XmlNode result = EmptyElement("tie");
            result.Attributes.Append(NameValueAttribute("type", startStopAttribute));
            return result;
        }

        internal XmlNode TiedElement(string startStopAttribute)
        {
            XmlNode result = EmptyElement("tied");
            result.Attributes.Append(NameValueAttribute("type", startStopAttribute));
            return result;
        }
        
        internal XmlNode SlurElement(string startStopAttribute)
        {
            XmlNode result = EmptyElement("slur");
            result.Attributes.Append(NameValueAttribute("type", startStopAttribute));
            return result;
        }

        #region barline

        internal enum BarStyleEnum { normal, lightLight, lightHeavy, heavyLight, dotted, dashed, defaultValue = normal } // Default is "normal"
        internal enum BarLocationEnum { right, left, middle, defaultValue = right } // Default is "right"

        private XmlNode BarStyleElement(string barStyle)
        {
            XmlNode result  = NameValuePair("bar-style", barStyle);
            return result;
        }

        // Only called from the outside ! Assures that the default location "right" is always explicitly shown.
        internal XmlNode BarLineElement(BarStyleEnum barStyleEnum)
        {
            return BarLineElement(barStyleEnum, BarLocationEnum.right);
        }

        // Called  from the "One size fits all"
        private XmlNode BarLineElementImplementation(BarStyleEnum barStyleEnum)
        {
            XmlNode result = doc.CreateElement("barline");
            switch (barStyleEnum)
            {
                case BarStyleEnum.lightLight: result.AppendChild(BarStyleElement("light-light")); break;
                case BarStyleEnum.lightHeavy: result.AppendChild(BarStyleElement("light-heavy")); break;
                case BarStyleEnum.heavyLight: result.AppendChild(BarStyleElement("heavy-light")); break;
                case BarStyleEnum.dotted: result.AppendChild(BarStyleElement("dotted")); break;
                case BarStyleEnum.dashed: result.AppendChild(BarStyleElement("dashed")); break;
                case BarStyleEnum.normal: break; // No childelement needed for "normal"
                default: throw new Exception("");
            }
            return result;
        }
        

        // The One size fits all!
        internal XmlNode BarLineElement(BarStyleEnum barStyleEnum,BarLocationEnum barLocation)
        {
            XmlNode result = BarLineElementImplementation(barStyleEnum);
            const string location = "location";
            switch (barLocation)
            {
                case BarLocationEnum.left: result.Attributes.Append(result.Attributes.Append(NameValueAttribute(location, "left"))); break;
                case BarLocationEnum.middle: result.Attributes.Append(result.Attributes.Append(NameValueAttribute(location, "middle"))); break;
                case BarLocationEnum.right: result.Attributes.Append(result.Attributes.Append(NameValueAttribute(location, "right"))); break;
                default: throw new Exception("");
            }
            return result;
        }
        #endregion

        internal XmlNode WorkChildElement(string text)
        {
            XmlNode result = NameValuePair("work-title", text);
            return result;
        }

        internal MusicXmlMeasureElement MeasureElement(int measureNumber)
        {
            MusicXmlMeasureElement result = doc.CreateMusicXmlMeasureElement("measure",this);
            XmlAttribute number = doc.CreateAttribute("number");
            result.Attributes.Append(number);
            number.Value = measureNumber.ToString();
            return result;
        }

        internal XmlNode PrintElement()
        {
            XmlNode result = doc.CreateElement("print");
            result.AppendChild(SystemLayoutElement());
            return result;
        }

        private XmlNode SystemLayoutElement()
        {
            XmlNode result = doc.CreateElement("system-layout");
            result.AppendChild(NameValuePair("top-system-distance", "1000"));
            return result;
        }

        internal XmlNode Element(string name)
        {
            XmlNode result = doc.CreateElement(name);
            return result;
        }


        internal MusicXmlNoteElement ChordNoteElement(MusicXmlNoteElement oldNote, InputSubCategoryEnum interval, IntervalDirectionEnum direction, MusicXmlBuilderAccidentalHandler accidentalHandler)
        {
            MusicXmlNoteElement result = doc.CreateMusicXmlNoteElement(string.Copy(oldNote.Name),this); // Create an empty clone of the note
            result.InnerXml = string.Copy(oldNote.InnerXml); // Copy the contents
            result.InsertBefore(Element("chord"), result.FirstChild); // Mark the clone as a "chord"
            result.ModifyPitch(interval, direction, accidentalHandler); // Modify pitch of the new note
            result.RemoveNamedChild("tie"); // Do NOT clone  ties
            result.RemoveGrandChildren("notations"); // Do NOT clone contents of notations (Such as "tied")
            return result;
        }

        internal MusicXmlHarmonyElement HarmonyNoteElement(MusicXmlHarmonyElement oldNote)
        {
            MusicXmlHarmonyElement result = doc.CreateMusicXmlHarmonyElement(string.Copy(oldNote.Name),this); // Create an empty clone of the note
            result.InnerXml = string.Copy(oldNote.InnerXml);     
            return result;
        }

        internal XmlNode PitchElement(string step, int octave, int alter)
        {
            XmlNode result = doc.CreateElement("pitch");
            result.AppendChild(NameValuePair("step", step.ToString()));
            if (0 != alter)
            {
                result.AppendChild(NameValuePair("alter", alter.ToString())); // Only append Alter element if needed
            }
            result.AppendChild(NameValuePair("octave", octave.ToString()));
            return result;
        }
        
        

        internal MusicXmlNoteElement NoteElement(XmlNode pitchElement, int duration, string type, XmlNode timeModification, int voice, string stem, XmlNode tupletBracket)
        {
            MusicXmlNoteElement result = doc.CreateMusicXmlNoteElement("note",this); 
            result.AppendChild(pitchElement);
            result.AppendChild(NameValuePair("duration", duration.ToString()));
            //if (null != currentTie)
            //{
            //    // The latest note started a tie, which muat be stopped now
            //    result.AppendChild(TieElement("stop"));
            //    currentTie = null;
            //}
            result.AppendChild(NameValuePair("voice", voice.ToString()));
            result.AppendChild(NameValuePair("type", type));
            if (null != timeModification)
            {
                result.AppendChild(timeModification);
            }
            result.AppendChild(NameValuePair("stem", stem));
            XmlNode notations = EmptyElement("notations");
            if (null != tupletBracket)
            {
                notations.AppendChild(tupletBracket);
            }
            result.AppendChild(notations);
            //if (null != currentTied)
            //{
            //    // The latest note started a tied, which must be stopped now
            //    notations.AppendChild(TiedElement("stop"));
            //    currentTied = null;
            //}
            //if (null != currentAccidental)
            //{
            //    // Accidentals are placed BEFORE the Note in Music Braille so check if any accidental is waiting to be used !
            //    XmlNode typeNode = result.SelectSingleNode("type");
            //    result.InsertAfter(currentAccidental, typeNode);
            //    currentAccidental = null;
            //}


            return result;

        }

        internal string YesNoAttribute(bool yes)
        {
            return yes ? "yes" : "no";
        }


        internal XmlNode GraceElement(bool slash)
        {
            XmlNode result = EmptyElement("grace");
            result.Attributes.Append(NameValueAttribute("slash", YesNoAttribute(slash)));
            return result; 
        }


        internal XmlNode EmptyElement(string name)
        {
            return doc.CreateElement(name);
        }


        internal MusicXmlNoteElement RestElement(int duration, string type, int voice)
        {
            MusicXmlNoteElement result = doc.CreateMusicXmlNoteElement("note",this);
            MusicXmlNoteElement rest = doc.CreateMusicXmlNoteElement("rest",this);
            result.AppendChild(rest); // Append a rest instead of a pitch !
            result.AppendChild(NameValuePair("duration", duration.ToString()));
            result.AppendChild(NameValuePair("voice", voice.ToString()));
            result.AppendChild(NameValuePair("type", type));
            XmlNode notations = EmptyElement("notations");
            result.AppendChild(notations);
            return result;
        }

        internal XmlNode BackUpElement(int duration)
        {
            XmlNode result = doc.CreateElement("backup");
            result.AppendChild(NameValuePair("duration", duration.ToString()));
            return result;
        }

        internal XmlNode ForwardElement(int duration)
        {
            XmlNode result = doc.CreateElement("forward");
            result.AppendChild(NameValuePair("duration", duration.ToString()));
            return result;
        }


        internal XmlNode IdentificationElement()
        {
            XmlNode result = doc.CreateElement("identification");
            result.AppendChild(EncodingElement());
            return result;

        }

        internal XmlNode DefaultsElement()
        {
            XmlNode result = Element("defaults");
            result.AppendChild(ScalingElement());
            result.AppendChild(PageLayoutElement());
            result.AppendChild(WordFontElement());
            result.AppendChild(LyricFontElement());
            return result;
        }

        /// <summary>
        /// Values inspired by files generated bu MuseScore
        /// </summary>
        /// <returns></returns>
        internal XmlNode ScalingElement()
        {
            XmlNode result = doc.CreateElement("scaling");
            result.AppendChild(NameValuePair("millimeters", "7.05556"));
            result.AppendChild(NameValuePair("tenths", "40"));
            return result;
        }


        /// <summary>
        /// Values inspired by files generated bu MuseScore
        /// </summary>
        /// <returns></returns>
        internal XmlNode PageLayoutElement()
        {
            XmlNode result = doc.CreateElement("page-layout");
            result.AppendChild(NameValuePair("page-height", "1683.78"));
            result.AppendChild(NameValuePair("page-width", "1190.55"));
            result.AppendChild(PageMarginElement("even"));
            result.AppendChild(PageMarginElement("odd"));
            return result;
        }

        /// <summary>
        /// Values inspired by files generated bu MuseScore
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        internal XmlNode PageMarginElement(string side)
        {
            XmlNode result = doc.CreateElement("page-margins");
            result.Attributes.Append(NameValueAttribute("type", side));
            result.AppendChild(NameValuePair("left-margin", "56.6929"));
            result.AppendChild(NameValuePair("right-margin", "56.6929"));
            result.AppendChild(NameValuePair("top-margin", "56.6929"));
            result.AppendChild(NameValuePair("bottom-margin", "113.386"));
            return result;
        }

        /// <summary>
        /// Values inspired by files generated bu MuseScore
        /// </summary>
        /// <param name="creditString"></param>
        /// <returns></returns>
        internal XmlNode CreditElement(string creditString)
        {

            string s = creditString.Replace("\n", ""); // Get rid of all '\n'                             
            string[] allTextLines = s.Split('\r');
            bool firstLine = true;

            XmlNode result = doc.CreateElement("credit");
            result.Attributes.Append(NameValueAttribute("page", "1"));
            foreach (String textLine in allTextLines)
            {
                XmlNode creditWords = NameValuePair("credit-words", textLine);
                if (firstLine)
                {
                    // These values are only nededd for the first credit-word:
                    creditWords.Attributes.Append(NameValueAttribute("default-x", "56.6929"));
                    creditWords.Attributes.Append(NameValueAttribute("default-y", "1627.09"));
                    creditWords.Attributes.Append(NameValueAttribute("justify", "left"));
                    creditWords.Attributes.Append(NameValueAttribute("valign", "top"));
                    creditWords.Attributes.Append(NameValueAttribute("font-size", "6"));  // Or even 6 
                    firstLine = false;
                }
                result.AppendChild(creditWords);
            }
            return result;
        }


        internal XmlNode EncodingElement()
        {
            XmlNode result = doc.CreateElement("encoding");

            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            result.AppendChild(NameValuePair("software", executingAssembly.FullName));       
            DateTime date =System.DateTime.Now.Date;
            string encodingDate = string.Format("{0}-{1:D02}-{2:D02}", date.Year, date.Month, date.Day); // We need the "D" because the input values are strings, not integers.
            result.AppendChild(NameValuePair("encoding-date", encodingDate));
            result.AppendChild(SupportsElement("accidental", "yes"));
            result.AppendChild(SupportsElement("stem", "yes"));
            // More to follow if needed.
            return result;
        }




        internal XmlNode SupportsElement(string elementName, string type)
        {
            XmlNode result = doc.CreateElement("supports");
            result.Attributes.Append(NameValueAttribute("element", elementName));
            result.Attributes.Append(NameValueAttribute("type", type));
            return result;
        }

        //internal XmlNode DefaultsElement()
        //{
        //    XmlNode result = doc.CreateElement("defaults");
        //    result.AppendChild(WordFontElement());
        //    result.AppendChild(LyricFontElement());
        //    return result;
        //}

        private XmlNode WordFontElement()
        {
            XmlNode result = doc.CreateElement("word-font");
            result.Attributes.Append(NameValueAttribute("font-family", "MuseJazz"));
            result.Attributes.Append(NameValueAttribute("font-size", "8"));
            return result;
        }

        private XmlNode LyricFontElement()
        {
            XmlNode result = doc.CreateElement("lyric-font");
            result.Attributes.Append(NameValueAttribute("font-family", "MuseJazz"));
            result.Attributes.Append(NameValueAttribute("font-size", "8"));
            return result;
        }

 
        internal XmlNode DivisionsElement(int divisions)
        {
            XmlNode result = NameValuePair("divisions", divisions.ToString());
            return result;
        }

        internal XmlNode KeyElement(int fifths)
        {
            XmlNode result = doc.CreateElement("key");
            result.AppendChild(NameValuePair("fifths", fifths.ToString()));
            return result;
        }


        internal XmlNode DirectionElement()
        {
            XmlNode result = doc.CreateElement("direction");
            return result;
        }


        internal XmlNode SegnoDirectionElement() // SoundDirectionElement(string directionTypeName, string soundAttributeName, string soundAttributeValue)
        {
            return SoundDirectionElement("segno", "segno", "segno");
           // < direction >
           //  < direction - type >
           //   < segno />
           //  </ direction - type >
           //  < sound segno = "segno" /> 
           // </ direction >
        }


        /// <summary>
        /// Generates the MusicXml representation of the musical "Del Segno al fine" symbol
        /// </summary>
        /// <returns></returns>
        internal XmlNode DalSegnoAlFineDirectionElement()
        {
            XmlNode directionElement = DirectionElement();
            XmlNode directionTypeElement = DirectionTypeElement();
            XmlNode directionType = this.WordsElement("d.s. al fine");
            directionElement.AppendChild(directionTypeElement);
            directionTypeElement.AppendChild(directionType);
            return directionElement;
            // <direction>
            //    <direction-type>
            //      <words>d.s. al fine</words>
            //    </direction-type>
            // </direction>
        }

        /// <summary>
        /// Generates the MusicXml representation of the musical "Fine" symbol
        /// </summary>
        /// <returns></returns>
        internal XmlNode FineDirectionElement()
        {
            XmlNode directionElement = DirectionElement();
            XmlNode directionTypeElement = DirectionTypeElement();
            XmlNode directionType = this.WordsElement("Fine");
            directionElement.AppendChild(directionTypeElement);
            directionTypeElement.AppendChild(directionType);
            return directionElement;
            // < direction >
            //   < direction - type >
            //     < words > Fine </ words >
            //   </ direction - type >
            // </ direction >
        }

        /// <summary>
        /// Generates the MusicXml representation of the "Segna" musical symbol. 
        /// // This includes a graphical representation and as well as a soundElement for controlling the generation of sound
        /// </summary>
        /// <param name="directionTypeName"></param>
        /// <param name="soundAttributeName"></param>
        /// <param name="soundAttributeValue"></param>
        /// <returns></returns>
        private XmlNode SoundDirectionElement(string directionTypeName, string soundAttributeName, string soundAttributeValue)
        {
            // First create all the nodes
            XmlNode directionElement = DirectionElement();
            XmlNode directionTypeElement = DirectionTypeElement();
            XmlNode directionType = EmptyElement(directionTypeName);
            XmlNode soundElement = SoundElement();
            XmlAttribute soundAttribute = NameValueAttribute(soundAttributeName, soundAttributeValue);
            // Link them together:
            directionElement.AppendChild(directionTypeElement);
            directionTypeElement.AppendChild(directionType);
            directionElement.AppendChild(soundElement);
            soundElement.Attributes.Append(soundAttribute);
            // Now it should look something like this:
            // < direction >
            //  < direction - type >
            //    < segno />
            //  </ direction - type >
            //  < sound segno = "segno" />
            //</ direction >
            return directionElement;
        }




        internal XmlNode SoundElement()
        {
            XmlNode result = doc.CreateElement("sound");
            return result;
        }

        internal XmlNode DirectionElement(string s)
        {
            XmlNode direction = DirectionElement();
            XmlNode directionType = DirectionTypeElement();
            XmlNode wordElement = WordsElement(s);
            direction.AppendChild(directionType);
            directionType.AppendChild(wordElement);
            return direction;
        }

        internal XmlNode DirectionTypeElement()
        {
            XmlNode result = doc.CreateElement("direction-type");
            return result;
        }

        internal XmlNode DynamicsElement(string text)
        {
            XmlNode result = doc.CreateElement("dynamics");
            result.AppendChild(doc.CreateElement(text));
            return result;
        }



        internal XmlNode RepeatElement(string direction)
        {
            XmlNode result = doc.CreateElement("repeat");
            result.Attributes.Append(NameValueAttribute("direction", direction));
            return result;
        }

        internal XmlNode EndingElement(string number, string type)
        {
            XmlNode result = doc.CreateElement("ending");
            if (string.IsNullOrEmpty(number)) return result;
            result.Attributes.Append(NameValueAttribute("number", number));
            result.Attributes.Append(NameValueAttribute("type", type));
            result.Attributes.Append(NameValueAttribute("print-object", "yes"));
            return result;
        }


        internal XmlNode WordsElement(string text)
        {
            return NameValuePair("words", text);
        }

        internal XmlNode WedgeElement(string wedgeType)
        {
            XmlNode result = doc.CreateElement("wedge");
            result.Attributes.Append(NameValueAttribute("type", wedgeType));
            return result;
        }



        internal XmlNode TimeElement(int beats, int beatType)
        {
            XmlNode result = doc.CreateElement("time");
            result.AppendChild(NameValuePair("beats", beats.ToString()));
            result.AppendChild(NameValuePair("beat-type", beatType.ToString()));
            return result;
        }

        internal XmlNode ForwardElement(string value)
        {
            XmlNode result = doc.CreateElement("forward");
            result.AppendChild(NameValuePair("duration", value));
            return result;
        }

        private string ShowNull(string s)
        {
            if (null == s) return "null";
            return s;
        }

        /// <summary>
        /// Sets the value of xmlNode.childName to newValue and returns tho old value
        /// </summary>
        /// <param name="xmlNode">The XmlNode to operate on</param>
        /// <param name="childName">The name of the child to operate on</param>
        /// <param name="newValue">The new value of the child</param>
        /// <returns>The old value of the child</returns>
        internal string SetChildValue(XmlNode xmlNode, string childName, string newValue)
        {
            if (null == xmlNode)
            {
                Logger.LogCF(string.Format(" called with xmlNode=null chileName={0} newValue={1}", ShowNull(childName), ShowNull(newValue)));
                return ""; // Or better null ?
            }
            XmlNode oldChild = xmlNode.SelectSingleNode(childName);
            string result = oldChild.Value;
            XmlNode newChild = NameValuePair(childName, newValue);
            xmlNode.ReplaceChild(newChild, oldChild);
            return result;
        }

        internal string GetChildValue(XmlNode xmlNode, string childName)
        {
            XmlNode child = xmlNode.SelectSingleNode(childName);
            string result = child.InnerText;
            return result;
        }



#if false
        /// <summary>
        /// Retrun a string for use in MusicXml
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        private string GetAccidental(string inputValue)
        {
            switch (inputValue)
            {
#warning: Fix this junk-code for instance by using 3 enums for sharp, flat and natural
                case "sharp":
                case "Kryds":
                    return "sharp";
                case "flat":
                case "B":  return "flat";
                case "natural":
                case "Opløsning":
                    return "natural";
                default: return null; //
            }
        }
#endif

        private XmlNode AccidentalElement(string text, bool courtesy)
        {
            XmlNode result = NameValuePair("accidental", text);
#warning todo find out of the following condition is needed
            if (courtesy)
            {
                result.Attributes.Append(NameValueAttribute("cautionary", courtesy ? "yes" : "no"));
            }
            return result;
        }


        internal XmlNode AccidentalElement(string inputValue, InputSubCategoryEnum inputSubCategory)
        {
            switch (inputSubCategory)
            {
                // case InputSubCategoryEnum.None: return AccidentalElement(GetAccidental(inputValue), false); // The old and obsolete way of doing things, based on the "inputvalue"
                case InputSubCategoryEnum.AccidentalFlat: return AccidentalElement("flat", false);    // The new way of doing things, based on "inputSubCategory"
                case InputSubCategoryEnum.AccidentalCourtesyFlat: return AccidentalElement("flat", true);    // The new way of doing things, based on "inputSubCategory"
                case InputSubCategoryEnum.AccidentalSharp: return AccidentalElement("sharp", false);  // The new way of doing things, based on "inputSubCategory"
                case InputSubCategoryEnum.AccidentalCourtesySharp: return AccidentalElement("sharp", true);  // The new way of doing things, based on "inputSubCategory"
                case InputSubCategoryEnum.AccidentalNatural: return AccidentalElement("natural", false);     // The new way of doing things, based on "inputSubCategory"
                case InputSubCategoryEnum.AccidentalCourtesyNatural: return AccidentalElement("natural", true);     // The new way of doing things, based on "inputSubCategory"

                default:
#warning todo Log something and return null ??
                    throw new System.NotSupportedException(string.Format("AccidentalElement({0},{1})", inputValue, inputSubCategory.ToString()));
            }
        }


        internal XmlNode AttributesElement()
        {
            XmlNode result = doc.CreateElement("attributes");
            return result;
        }


        internal XmlNode ClefElement(InputCategoryEnum inputCategory, string inputValue, InputSubCategoryEnum inputSubCategory)
        {
            switch (inputSubCategory)
            {
                case InputSubCategoryEnum.ClefF: return ClefElement("F", 4, 0);
                case InputSubCategoryEnum.ClefG: return ClefElement("G", 2, 0);
                case InputSubCategoryEnum.HandRight: return ClefElement("G", 2, 0);
                case InputSubCategoryEnum.HandLeft: return ClefElement("F", 4, 0);
                case InputSubCategoryEnum.HandPedal: return ClefElement("F", 4, 0);
            }
            Logger.LogCF(string.Format("ClefElement: Illegal inputSubCategory={0}", inputSubCategory.ToString()));
            return null;
        }


        internal MusicXmlHarmonyElement HarmonyElement(string root, string alter)
        {
            MusicXmlHarmonyElement result = doc.CreateMusicXmlHarmonyElement("harmony",this);
            XmlNode harmonyRoot = doc.CreateElement("root");
            harmonyRoot.AppendChild(NameValuePair("root-step", root)); // Derived from the call parameter
            if (!string.IsNullOrEmpty(alter))
            {
                harmonyRoot.AppendChild(NameValuePair("root-alter", alter)); 
            }           
            result.AppendChild(harmonyRoot);
            result.AppendChild(NameValuePair("kind", "major")); // Just a place holder. May be changed later.
            return result;
        }

        internal XmlNode BassElement(string bass, string alter)
        {
            XmlNode result = doc.CreateElement("bass");
            result.AppendChild(NameValuePair("bass-step", bass));  // Derived from the call parameter
            if (!string.IsNullOrEmpty(alter))
            {
                result.AppendChild(NameValuePair("bass-alter", alter)); 
            }
            return result;
        }

        internal XmlNode DegreeElement(int degree, int alter, string degreeType)
        {
            XmlNode result = doc.CreateElement("degree");
            result.AppendChild(NameValuePair("degree-value", degree.ToString()));
            result.AppendChild(NameValuePair("degree-alter", alter.ToString()));
            result.AppendChild(NameValuePair("degree-type", degreeType.ToString()));
            return result;
        }

        internal XmlNode ClefElement(string sign, int line, int clefOctaveChange)
        {
            XmlNode result = doc.CreateElement("clef");
            result.AppendChild(NameValuePair("sign", sign));
            result.AppendChild(NameValuePair("line", line.ToString()));
            result.AppendChild(NameValuePair("clef-octave-change", clefOctaveChange.ToString()));
            return result;
        }

        // Simple helper
        internal XmlNode NameValuePair(string name, string value)
        {
            XmlNode result = doc.CreateElement(name);
            result.InnerText = value;
            return result;
        }

        internal XmlAttribute NameValueAttribute(string name, string value)
        {
            XmlAttribute result = doc.CreateAttribute(name);
            result.Value = value;
            return result;
        }



        internal MusicXmlElementFactory(MusicXmlDocument doc)
        {
            this.doc = doc; 
        }

        internal MusicXmlElementFactory()
        { }

        public static MusicXmlElementFactory Create(MusicXmlDocument doc)
        {
            return new MusicXmlElementFactory(doc);
        }

    }
}
