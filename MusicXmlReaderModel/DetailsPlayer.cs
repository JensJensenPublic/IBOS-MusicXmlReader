using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;
using NAudio.Midi;

namespace MusicXmlReaderModel
{
    class DetailsPlayer
    {
        private MidiOut midiOut;
        private List<DetailsDescription> detailsDescriptions = new List<DetailsDescription>();
        public List<DetailsDescription> DetailsDescriptionList
        {
            get
            {
                return detailsDescriptions;
            }
        }

        public DetailsDescription[] DetailsDescriptionArray
        {
            get
            {
                // Convert from list to array here !!
                DetailsDescription[] result = new DetailsDescription[detailsDescriptions.Count];
                for (int i = 0; (i < detailsDescriptions.Count); i++)
                {
                    result[i] = detailsDescriptions[i];
                }
                return result;
            }
        }

        /// <summary>
        /// Builds a localized, detailed description of the harmony. For instance in danish, "C/E" is represented as
        /// Kvint G
        /// Stor terts E
        /// Grundtone C
        /// Bastone C
        /// Becifring C/E
        /// </summary>
        /// <returns></returns>
        private DetailsPlayer(HarmonyElement harmonyElement, MidiOut midiOut)
        {
            this.midiOut = midiOut;
            Interval[] intervals = MidiChord.GetChordIntervals(harmonyElement.ChordType); // In this way we will use the same definitions for the sound and the text
            ChromaticStep root = harmonyElement.ChromaticRootStep;
            detailsDescriptions = new List<DetailsDescription>();
            string chordName = harmonyElement.ToLocalizedString();
            detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", ResourcesForModel.HarmonyElement_Chord,chordName ), harmonyElement)); // The full representation of the chord 
            if ((null != harmonyElement.BassElement) && (harmonyElement.ChromaticRootStep != harmonyElement.ChromaticBassStep))
            {
                detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", ResourcesForModel.HarmonyElement_BassTone, harmonyElement.BassElement.ToString()), harmonyElement.ChromaticBassStep)); // The bass tone if different from the root. For instance "Bass E" 
            }
            for (int i = 0; (i < intervals.Length); i++) // Each note in the Harmony, represented by function and by name. 
            {
                int iStep = (((int)root) + ((int)intervals[i])) % 12; // Do simple arithmetics !
                ChromaticStep step = (ChromaticStep)(iStep);
                string function = MidiChord.ToLocalizedChordFunction(intervals[i]);
                detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", function, step.ToString()), step)); // For instance : "Third E"
            };
        }



        private DetailsPlayer(EventDescription eventDescription, PartlistElement partList, UserSettings userSettings, MidiOut midiOut)
        {
            this.midiOut = midiOut;
            int numberOfParts = partList.NumberOfParts();
            detailsDescriptions = new List<DetailsDescription>();
            for (int i = 0; (i < numberOfParts); i++)
            {
                List<NoteElement> notesForPart = eventDescription.NoteLists[i];
                if ((null != notesForPart) && (0 != notesForPart.Count))
                {
                    // String variables for desribing the detail as text
                    string partId = "";
                    string partName = "";
                    string notes = "";
                    string lyrics = "";

                    if ((userSettings.MusicAsSpeech) && (userSettings.partsToRead[i]))
                    {
                        // The eventdescription contains notes for this part so we dig out the part parameters:
                        ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                        partId = scorePartElement.partId;
                        partName = scorePartElement.partName;
                        // By using  eventDescription.NotesForOnePart for formatting the notes we assure the usage of identical formatting.
                        notes = eventDescription.NotesForOnePart(notesForPart);
                        // By using  eventDescription.LyricsForOnePart for formatting the lyrics we assure the usage of identical formatting.
                        lyrics = eventDescription.LyricsForOnePart(notesForPart);
                    }

                    // String variables for desribing the detail as MusicBraille
                    string musicBraille = "";
                    if ((userSettings.MusicAsMusicBraille) && (userSettings.partsToBraille[i]))
                    {
                        BrailleBuilder musicBrailleDetails = eventDescription.NotesForOnePartAsBraille(notesForPart);
                        musicBraille = musicBrailleDetails.ToBrailleString();
                    }

                    // Compose all details, always showing MusicBraille first
                    string detailString = string.Format("{0} {1} {2} {3} {4}", musicBraille, partId, partName, notes, lyrics);
                    detailsDescriptions.Add(DetailsDescription.Create(detailString));
                }

            }

            // Add any harmonies after the last part
            HarmonyElement harmonyElement = eventDescription.HarmonyElement;
            if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.Harmonies)) && (null != harmonyElement))
            {
                string harmony = string.Format("{0}{1}  ", harmonyElement.ChromaticRootStep, harmonyElement.LocalizedChordType); // Use same formatting as used in the status line !!
                detailsDescriptions.Add(DetailsDescription.Create(harmony));
            }
        }



        private MidiNote currentDetailsMidiNote = null;
        private MidiChord currentDetailsMidiChord = null;

        public void SelectedDetailsIndexChanged(DetailsDescription detailsDescription)
        {
            if (null != currentDetailsMidiNote)
            {
                currentDetailsMidiNote.StopPlaying(midiOut);
            }
            if (null != currentDetailsMidiChord)
            {
                currentDetailsMidiChord.StopPlaying(midiOut);
            }
            MidiNote midiNote = new MidiNote(detailsDescription.PitchRepresentation, 4, 70, Interval.Unison);
            midiNote.StartPlaying(midiOut);

            HarmonyElement hE = detailsDescription.HarmonyElement;
            if (null != hE)
            {
                int octave = 4;
                int velocity = 90;            
                currentDetailsMidiChord = new MidiChord(hE.ChromaticRootStep,octave,velocity,hE.ChordType,hE.ChromaticBassStep,null,null);
                currentDetailsMidiChord.StartPlaying(midiOut);
            }
            

        }





        public static DetailsPlayer Create(HarmonyElement harmonyElement,MidiOut midiOut)
        {
            return new DetailsPlayer(harmonyElement,midiOut);
        }


        public static DetailsPlayer Create(EventDescription eventDescription, PartlistElement partList, UserSettings userSettings, MidiOut midiOut)
        {
            return new DetailsPlayer(eventDescription, partList, userSettings,midiOut);
        }
    }
}
