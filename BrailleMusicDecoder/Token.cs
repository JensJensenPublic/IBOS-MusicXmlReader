using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// Represent all details about the interpretation of a single input token
    /// </summary>
    public class Token
    {
        private string stringRepresentation;
        /// <summary>
        /// The textual representation of this token, directly usable in the UI
        /// </summary>
        public string StringRepresentation{ get { return stringRepresentation; } }
        private string accumulatedText;
        /// <summary>
        /// The textual representation of extra information related to this Token, directly usable in the UI
        /// </summary>
        public  string AccumulatedText { get { return accumulatedText; } }
        /// <summary>
        /// The Unicode string representing this token in the Unicode representaion of the input stream.
        /// Each Unicode char is in the interval [0x2800 .. 0x283F]
        /// This string can be used for instance for a Music Braille editor, allowing the expeienced user to change the input stream at the raw Music Braille level.
        /// </summary>
        public string TokenString { get { return (null != inputInterpretation) ? inputInterpretation.TokenString : "" ; } }
        private int startIndex = 0;
        /// <summary>
        /// The start position within the Unicode representation of the input stream of this Token.
        /// </summary>
        public int StartIndex { get { return startIndex; } }

        private DecoderStateMachine.StateEnum initialDecoderState;
        public DecoderStateMachine.StateEnum InitialDecoderState { get { return initialDecoderState; } }

        /// <summary>
        /// The raw, detailed input interpretation of this Token.
        /// This member is intensionally made private! 
        /// All references to the InputInterpretation object must go through accessors.
        /// </summary>
        private InputInterpretation inputInterpretation;
        private readonly List<XmlNode> emptyXmlNodeContainer = new List<XmlNode>();

        /// <summary>
        /// Contains a single XmlNode containing the MuSicXml representation of this node
        /// By using a List<> as a container we allow for adding the Xml contents at any time, thus allowing for resolving Note Grouping!
        /// </summary>
        public List<XmlNode> XmlRepresentation  { get {return (null != inputInterpretation) ?  inputInterpretation.XmlNodeContainer : emptyXmlNodeContainer;}}

        Token(string stringRepresentation, string accumulatedText, InputInterpretation inputInterpretation, int startIndex, DecoderStateMachine.StateEnum initialDecoderState)
        {
            this.stringRepresentation = stringRepresentation;
            this.accumulatedText = accumulatedText;
            this.inputInterpretation = inputInterpretation;
            this.startIndex = startIndex;
            this.initialDecoderState = initialDecoderState;
        }

        /// <summary>
        /// Creates a Token object  not related to any specific InputInterpretation object, and thus only containing supplemantary information.
        /// </summary>
        /// <param name="stringRepresentation"></param>
        /// <param name="accumulatedText"></param>
        /// <returns></returns>
        public static  Token Create(string stringRepresentation, string accumulatedText, DecoderStateMachine.StateEnum initialDecoderState)
        {
            return new Token(stringRepresentation, accumulatedText,null,0,initialDecoderState);
        }


        /// <summary>
        /// Creates an object containing all information about a single Token, dericed from the MusicBraille input file.
        /// This token may represent any other musical symbol, as defined in the Music Braille definition, for instance
        /// - A musical note,
        /// - A measure bar
        /// - A repetition mark
        /// - An accidental
        /// - A punctuation
        /// This Token object can be used by
        /// - The UI in a GUI application such as a MusicBraille reader or a Music Braille editor
        /// - A other application, such as for instance commmandline based regression test.
        /// Note that the inputInterpretation value is saved in a private variable and is only accessible through accessors or functions.
        /// This means that the UI (or any other application, using this class) does not need to referense the BrailleMusicDecoder dll where the InputInterpretation is defined.
        /// In this way we accomplish a separation bwtween the UI and the lowlevel BrailleMusicDecoder implementation.
        /// </summary>
        /// <param name="stringRepresentation">The userfriendly string representation of the token, controlled by the varions sonfiguration settings</param>
        /// <param name="accumulatedText">Supplementary textual information related to this token</param>
        /// <param name="inputInterpretation">The full lowlevel interpretation of the token</param>
        /// <param name="startIndex">The position within the token within the input stream</param>
        /// <returns></returns>
        public static Token Create(string stringRepresentation, string accumulatedText, InputInterpretation inputInterpretation, int startIndex, DecoderStateMachine.StateEnum initialDecoderState)
        {
            return new Token(stringRepresentation, accumulatedText, inputInterpretation, startIndex,initialDecoderState);
        }

    }
}
