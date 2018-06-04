using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Holds a number of textual representations for an eventdescription.
    /// In this way the UI can hold its own private copy of all events as a pure text representation 
    /// while the PlayerThread holds the original list of eventdescriptions
    /// </summary>
    public class EventDescriptionTextRepresentation
    {
        private string textRepresentation = "";
        private string statusInformation = "";
        private string normalTextString = "";
        private string musicBrailleString = "";

        public override string ToString()
        {
            return textRepresentation; // Shows Braille and status live
            //return normalTextString; // REspects UserSettings
        }

        public string TextRepresenttion { get { return textRepresentation; } }
        public string StatusInfrormation { get { return statusInformation; } }

        private string safeCopy(string s)
        {
            return (null == s) ? "" : s;
        }



        private EventDescriptionTextRepresentation(EventDescription eventDescription)
        {
            // IMPORTANT! Get copies, not references. We need to decouple the UI thread from the MusicPlayer Thread !         
            textRepresentation = safeCopy(eventDescription.TextRepresentation); 
            normalTextString   = safeCopy(eventDescription.ToNormalTextString());
            musicBrailleString = safeCopy(eventDescription.ToMusicBrailleString().ToString());
            statusInformation  = (null == eventDescription.StatusInformation) ? "" : safeCopy( eventDescription.StatusInformation.ToString());

        }


        static public EventDescriptionTextRepresentation Create(EventDescription eventDescription)
        {
            return new EventDescriptionTextRepresentation(eventDescription);
        }
    }
}
