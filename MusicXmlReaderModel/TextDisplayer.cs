using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{


    public class TextDisplayer
    {
        private IDebugDisplayerClient iDebugDisplayerClient;

        private TextDisplayer()
        { }

        private TextDisplayer(IDebugDisplayerClient iDebugDisplayerClient)
        {
            this.iDebugDisplayerClient = iDebugDisplayerClient;
        }

        public static TextDisplayer Create(IDebugDisplayerClient iDebugDisplayerClient)
        {
            return new TextDisplayer(iDebugDisplayerClient);
        }

        /// <summary>
        /// Used when the playing manually.
        /// The User selects a note at a time. 
        /// or
        /// The user selects an EventDescription at a time. This may contain several notes to be played simultaneously
        /// </summary>
        /// <param name="eventDescription"></param>
        public void SelectedIndexChanged(EventDescription eventDescription)
        {
            //if (playing) return;
            if (null == eventDescription) return;
            iDebugDisplayerClient.WriteNormalTextString(eventDescription.TextRepresentation);
        }
    }
}
