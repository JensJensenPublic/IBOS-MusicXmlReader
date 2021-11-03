using System.Xml;
using System.Collections.Generic;
using BrailleMusicDecoder;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Wrapper class for isolating the client code from the Decoder implementation.
    /// Using this class as the interface between the client and the Decoder the client does not need to reference BRailleMusicDecoder.dll.
    /// DecoderItem.cs may be "using" and referencing BrailleMusicDecoder in private members, but not in the interface upwards.
    /// </summary>
    public class DecoderItem
    {
        private string stringRepresentation;

        /// <summary>
        /// Returns a string to be shown to the user, for instance through a ListBox.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return stringRepresentation;
        }
        private List<XmlNode> xmlRepresentation;

        private string tokenString = null;
        public string TokenString { get { return tokenString; } }
        private int startIndex = 0;
        public int StartIndex { get { return startIndex; } }

        private DecoderStateMachine.StateEnum initialDecoderState;
        public DecoderStateMachine.StateEnum InitialDecoderState { get {  return initialDecoderState; } }
        public int InitialDecoderStateNumber { get { return (int)initialDecoderState; } }
        public string InitialDecoderStateName { get { return initialDecoderState.ToString(); } }

        public string XmlToString()
        {
            if (null == XmlRepresentation) return "";
            if (1 != XmlRepresentation.Count) return "";
            if (null == XmlRepresentation[0]) return "";          
            return ToString(XmlRepresentation[0]); 
        }

        private string ToString(XmlNode xmlNode)
        {
#warning: Consider using a ToString defined on MusicXmlNoteElement
            XmlNode pitch = xmlNode.SelectSingleNode("pitch");
            if (null == pitch) return "";
            XmlNode octave = pitch.SelectSingleNode("octave");
            string octaveString = octave.InnerText;
            XmlNode step = pitch.SelectSingleNode("step");
            string stepString = step.InnerText;
            XmlNode alter = pitch.SelectSingleNode("alter");
            string alterString = "";
            if (null != alter)
            {
                switch (alter.InnerText)
                {
                    case "-1": alterString = "b"; break;
                    case "1": alterString = "kryds"; break;
                }
            }
            XmlNode noteType = xmlNode.SelectSingleNode("type");
            string noteTypeString = "";
            switch (noteType.InnerText)
            {
                case "whole": noteTypeString = "hel"; break;
                case "half": noteTypeString = "halv"; break;
                case "quarter": noteTypeString = "kvart"; break;
                case "eighth": noteTypeString = "ottendedel"; break;
                case "16th": noteTypeString = "sekstendelel"; break;
                case "32nd": noteTypeString = "toogtredivtedel"; break;
                case "64th": noteTypeString = "fireogtredsindstyvendedel"; break;
                case "128th": noteTypeString = "hundredeogotteogtyvendedel"; break;
#warning todo Localize
                default: noteTypeString = noteType.InnerText; break;
            }

#warning TODO Consider the changing the implementation as described below::
            // In the current implementation the dot is added to the NoteElement during the interpretation of the next MusicBraille character and is thus not available now,
            // Consider adding a reference to the Interpreatation and expanding it by calling ToString() on it at a later time when the dot element is available.
            // In the current implementation the 5 lines below  have no function !
            string dotString = "";
            XmlNode dotNode = xmlNode.SelectSingleNode("dot");
            if (null != dotNode)
            {
                dotString = "punkteret";
            }

            string voiceString = "";
            XmlNode voiceNode = xmlNode.SelectSingleNode("voice");
            if (null != voiceNode)
            {
                switch (voiceNode.InnerText)
                {
                    case "1": break;
                    default: voiceString = "stemme " + voiceNode.InnerText; break;
                }
            }

            string result = string.Format("XML {0} {1} {2} {3} {4} {5}", stepString, alterString, octaveString, dotString, noteTypeString, voiceString);
            return result;
        }





        /// <summary>
        /// Returns the XML representation, typically as a MusicXml NoteElement, geneted by BrailleMusicDecoder.MusicXmlBuilder class
        /// </summary>
        public List<XmlNode> XmlRepresentation { get { return xmlRepresentation; } }

  
        private DecoderItem(string stringRepresentation, List<XmlNode> xmlRepresentation,string tokenString, int startIndex, DecoderStateMachine.StateEnum initialDecoderState)
        {
            this.stringRepresentation = stringRepresentation;
            this.xmlRepresentation = xmlRepresentation;
            this.tokenString = tokenString;
            this.startIndex = startIndex;
            this.initialDecoderState = initialDecoderState;

        }

        public static DecoderItem Create(string stringRepredsentation)
        {
            return new DecoderItem(stringRepredsentation, null,null, 0 , DecoderStateMachine.StateEnum.Unknown);
        }

        public static DecoderItem Create(string stringRepredsentation, List<XmlNode> xmlRepresentation, string tokenString, int startIndex, DecoderStateMachine.StateEnum initialDecoderState)
        {
            return new DecoderItem(stringRepredsentation, xmlRepresentation,tokenString, startIndex,initialDecoderState);
        }

    }
}
