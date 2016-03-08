using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace MusicXmlReaderUI
{
    
    public class EventDescription
    {
        int startTime;
        int numberOfParts;
        UserSettings userSettings;

        /// <summary>
        /// Notes to be played at this time
        /// </summary>
        private NoteElement[] notes; // TO DO: Remove notes. Use noteLists instead!!

        private List<NoteElement>[] noteLists; // An array of lists of notes

        public NoteElement[] Notes // TO DO: Remove Notes. Use NoteLists instead!!
        {
            get
            {
                return notes;
            }
        }

        public int StartTime
        {
            get
            {
                return startTime;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private EventDescription()
        {
        }

        private EventDescription(int time, int numberOfParts, UserSettings userSettings)
        {
            this.startTime = time;
            this.numberOfParts = numberOfParts;
            this.notes = new NoteElement[numberOfParts];
            // For each part a list is needed to handle to handle multiple notes within the same part!
            this.noteLists = new List<NoteElement>[numberOfParts];
            for (int i = 0; (i < numberOfParts); i++)
            {
                noteLists[i] = new List<NoteElement>();
            } 
            this.userSettings = userSettings;
        }

        public static EventDescription Create(int time, int numberOfParts,  UserSettings userSettings)
        {
            return new EventDescription(time, numberOfParts, userSettings);
        }

        public void AddNote(NoteElement noteElement)
        {
            notes[noteElement.PartNumber] = noteElement;
            noteLists[noteElement.PartNumber].Add(noteElement);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("{0,6}: ", startTime));
            StringBuilder sbText = new StringBuilder();
            // foreach (NoteElement noteElement in notes)
            foreach (List<NoteElement> noteElementList in noteLists) // Iterate over the fixed number of parts.
            {
                //if (2 == noteElementList.Count)
                //{
                //    System.Threading.Thread.Sleep(0); // Only for settitn a bp
                //}
                string s = "-";
                if (0 == noteElementList.Count)
                {
                    // Nothing happens in this part
                    sb.Append("   - ");
                }
                else
                {
                    foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
                    {
                        //              sb.Append(string.Format("{0} ", noteElement.PartId));


                        // Add pitch information

                        if (userSettings.partsToRead[noteElement.PartNumber])
                        {
                            s = string.IsNullOrEmpty(noteElement.Step) ? "P" : noteElement.PitchValue.Name + noteElement.PitchValue.Octave;
                        }


                        // Add any lyrics
                        if (userSettings.partsToRead[noteElement.PartNumber]) // TODO use userSettings.textPartsToRead instead!
                        {
                            if (!string.IsNullOrEmpty(noteElement.Text))
                            {
                                sbText.Append(noteElement.Text);
                            }
                        }

                        sb.Append(string.Format("{0,4} ", s.Replace(" ", "")));  // Remove any blanks   
                    }
                }
            }
            return sb.ToString() + " " + sbText.ToString();
        }
    }
}
