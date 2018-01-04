using System.Collections.Generic;
using System.Text;
using JSJ.ScreenReaderAPI;

//Unicode for Braille
//https://en.wikipedia.org/wiki/Braille_Patterns
//http://www.unicode.org/charts/PDF/U2800.pdf

namespace MusicXmlReaderModel
{

    /// <summary>
    /// This interface is only used to attempt to avoid polluting the Model with references to Windows.Forms
    /// </summary>
    public interface IDebugDisplayerClient
    {
        void WriteBrailleString(string s); // Write a string to be interpreted as Braille Unicode characters
        void WriteTextString(string s);    // Write a string to be interpreted as normal  Unicode text
        void WriteNormalTextString(string s); //   // Write a string to be interpreted as normal  Unicode text
        void WriteStatusInformation(string s); // Write a string to be interpreted as normal Unicode Text
    }

    /// <summary>
    /// Similar function to MusicPlyuer
    /// </summary>
    public class BrailleDisplayer
    {
        private string className = "BrailleDisplayer";
        public static readonly char UnicodeBrailleBase = (char)0x2800;

        // private PlatformDependencies.FSBrlDspAPIWrapper fSBrlDspAPIWrapper; // Used by experimental code for accessing a Freedom Scientific Braille display directly.
        private IDebugDisplayerClient brailleDisplayerClient; // The client receiving MusicBraille bytes, represented as UniCode
        private string emptyBrailleString;
        //private NvdaControllerClientWrapper nvda;
        private int displaySize;
        private string latestMusicBrailleString = string.Empty;
        private ScreenReaderAPI screenReaderAPI;



         private BrailleDisplayer(IDebugDisplayerClient brailleDisplayerClient, int displaySize,ScreenReaderAPI screenReaderAPI)
        {
            string functionName = "BrailleDisplayer";
            this.brailleDisplayerClient = brailleDisplayerClient;
            this.displaySize = displaySize;
            this.screenReaderAPI = screenReaderAPI;

            Logger.Log(string.Format("{0}.{1} Skipping PlatformDependencies.FSBrlDspAPIWrapper.Create() and fSBrlDspAPIWrapper.Open()", className, functionName));
            // NOTE: The following 2 lines seem to make Perkins input from the FOCUS14 fail !!!
            //fSBrlDspAPIWrapper = PlatformDependencies.FSBrlDspAPIWrapper.Create(); // For direct access to physical Braille Display
            //fSBrlDspAPIWrapper.Open(); // TODO Insert this line again after placing FSBrlDspApi.dll in the 3.Party directory.

            // nvda = NvdaControllerClientWrapper.Create(); // For access to physical Braille Display through NVDA 
            emptyBrailleString = new StringBuilder().Append(UnicodeBrailleBase, displaySize).ToString();

            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; (i < this.displaySize); i++) { sb.Append(UnicodeBrailleBase);};
            //emptyBrailleString = sb.ToString();
        }

  
        public static BrailleDisplayer Create(IDebugDisplayerClient ws,int displaySize,ScreenReaderAPI screenReaderAPI)
        {
             return new BrailleDisplayer(ws,displaySize, screenReaderAPI);
        }



        private string BytesToString(List<byte> bytes, int displaySize)
        {
            StringBuilder musicBrailleStringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                musicBrailleStringBuilder.Append((char)(UnicodeBrailleBase + (char)b));
            }
            int size = musicBrailleStringBuilder.Length;
            if ( size < displaySize)
            {
                // First the typial case
                return musicBrailleStringBuilder.Append(UnicodeBrailleBase, (displaySize - size)).ToString();
            }
            else
            {
                return musicBrailleStringBuilder.ToString(0, displaySize);
            }
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
            string methodName = "SelectedIndexChanged";
            //StopRefreshing(); // Stop refreshing the Braille Display; Also happens when controllooses focus      

            //if (playing) return;
            if (null == eventDescription) return;

            //BrailleBuilder bb = eventDescription.ToBraille();
            //StringBuilder text = new StringBuilder();
            //brailleDisplayerClient.WriteBrailleString(bb.ToBrailleString());
            //brailleDisplayerClient.WriteTextString(bb.Text.ToString()); 
            brailleDisplayerClient.WriteBrailleString(eventDescription.MusicBrailleRepresentation);
            brailleDisplayerClient.WriteTextString(eventDescription.MusicBrailleAsTextRepresentation);
            if (null != eventDescription.StatusInformation)
            {
                brailleDisplayerClient.WriteStatusInformation(eventDescription.StatusInformation.ToString());
            }
            else
            {
                Logger.LogOnce(string.Format("{0}.{1} : eventDescription.StatusInformation is null", className, methodName));
            }

            screenReaderAPI.Silence(); // Prevent overloading the internal queue in NVDA when rapidly changing between different events                  
            if (ScreenReaderAPI.ScreenReaderType.NVDA == screenReaderAPI.GetScreenReaderType())
            {
                // NVDA will read the MusicBraille characters as "Braille 1,2,3,4,5,6,7,8"
                // So in the NVDA case the MusicBraille characters must NOT shown in the listbox.
                // The MusicBraille characteres are thus not automatically shown on the Braille display.
                // Instead we must explicitly write them to the Braille display:
                screenReaderAPI.Braille(eventDescription.ToMusicBrailleAndTextBrailleString(), true);
            }
            return;
        }

        public void StartRefreshing()
        {

        }


        public void StopRefreshing()
        {
            // Clear the BrailleDisplay first.     
            screenReaderAPI.Braille(emptyBrailleString,false);
            //screenReaderAPI.StopRefreshing();
            brailleDisplayerClient.WriteBrailleString(emptyBrailleString);
        }

    }


}
