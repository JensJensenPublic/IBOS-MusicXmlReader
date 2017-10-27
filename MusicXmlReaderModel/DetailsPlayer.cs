using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;
using NAudio.Midi;

namespace MusicXmlReaderModel
{


    /// <summary>
    /// Class for showing (and playing) items in the details window.
    /// The first 2 kinds of details are parts and harmonies, but it is easy to implement more, for instance
    /// pitched and unpitched instruments used in the score, by just adding the appropriate .Create() methods
    /// </summary>
    class DetailsPlayer
    {
        private const int bassOctave = 3; // Play the Bass note (if any) in 3. octave
        private const int chordOctave = 4; // Build the chord starting in 4. octave, possibly spreading into 5. octave
        private const int velocity = 90;
        private MusicPlayer musicPlayer;
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
        private DetailsPlayer(HarmonyElement harmonyElement, MusicPlayer musicPlayer)
        {
            this.musicPlayer = musicPlayer;
            // Interval[] intervals = MidiChord.GetChordIntervals(harmonyElement.ChordType); // In this way we will use the same definitions for the sound and the text  
            Interval[] intervals = MidiChord.GetModifiedChordIntervals(harmonyElement.ChordType, harmonyElement.Degrees);        
            ChromaticStep root = harmonyElement.ChromaticRootStep;
            detailsDescriptions = new List<DetailsDescription>();
            string chordName = harmonyElement.ToLocalizedString();
            detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", ResourcesForModel.HarmonyElement_Chord,chordName ), harmonyElement)); // The full representation of the chord 
            if ((null != harmonyElement.BassElement) && (harmonyElement.ChromaticRootStep != harmonyElement.ChromaticBassStep))
            {
                string s = string.Format("{0} {1}", ResourcesForModel.HarmonyElement_BassTone, harmonyElement.BassElement.ToString());
                detailsDescriptions.Add(DetailsDescription.Create(s, harmonyElement.ChromaticBassStep,bassOctave)); // The bass tone if different from the root. For instance "Bass E" 
            }
            for (int i = 0; (i < intervals.Length); i++) // Each note in the Harmony, represented by function and by name. 
            {
                int iSum = ((int)root) + ((int)intervals[i]); // Do simple arithmetics !
                int iStep   = iSum % (int)ChromaticStep.NumberOfSteps; // 12
                int iOctave = iSum / (int)ChromaticStep.NumberOfSteps; // 12
                ChromaticStep step = (ChromaticStep)(iStep);                
                string function = MidiChord.ToLocalizedChordFunction(intervals[i]);
                detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", function, step.ToString()), step, iOctave + chordOctave)); // For instance : "Third E"
            };
            detailsDescriptions.Reverse();
        }


        /// <summary>
        /// Build a list of all parts
        /// NOTE: The contents of this list does NOT depend of the position of the cursor on the List-Window, only of the parts defined in the MusicXml file.
        /// </summary>
        /// <param name="partList"></param>
        private DetailsPlayer(PartlistElement partList)
        {
            int numberOfParts = partList.NumberOfParts();
            detailsDescriptions = new List<DetailsDescription>();
        //    string text = (1 == numberOfParts) ? ResourcesForModel.DetailsPlayer_Part : ResourcesForModel.DetailsPlayer_Parts; // Singularis / Pluralis
        //    detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", numberOfParts, text)));
        }


        private DetailsPlayer(EventDescription eventDescription, PartlistElement partList, UserSettings userSettings, MusicPlayer musicPlayer, bool noteLevel)
        {
            if (noteLevel)
            {
                NoteDetailsPlayer(eventDescription, partList, userSettings, musicPlayer);
            }
            else
            {
                PartDetailsPlayer(eventDescription, partList, userSettings, musicPlayer);
            }
        }


        /// <summary>
        /// Reports dettails at the Part level
        /// </summary>
        /// <param name="eventDescription"></param>
        /// <param name="partList"></param>
        /// <param name="userSettings"></param>
        /// <param name="musicPlayer"></param>
        private void PartDetailsPlayer(EventDescription eventDescription, PartlistElement partList, UserSettings userSettings, MusicPlayer musicPlayer)
        {
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

            // Do NOT add any harmonies after the last part! HArmonies have their own meshanisms !
            //HarmonyElement harmonyElement = eventDescription.HarmonyElement;
            //if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.Harmonies)) && (null != harmonyElement))
            //{
            //    string harmony = string.Format("{0}{1}  ", harmonyElement.ChromaticRootStep, harmonyElement.LocalizedChordType); // Use same formatting as used in the status line !!
            //    detailsDescriptions.Add(DetailsDescription.Create(harmony));
            //}
        }


        /// <summary>
        /// Reports details at the Note level
        /// </summary>
        /// <param name="eventDescription"></param>
        /// <param name="partList"></param>
        /// <param name="userSettings"></param>
        /// <param name="musicPlayer"></param>
        private void NoteDetailsPlayer(EventDescription eventDescription, PartlistElement partList, UserSettings userSettings, MusicPlayer musicPlayer)
        {
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
                        foreach (NoteElement noteElement in eventDescription.NoteLists[i])
                        {
                            //notes = string.Format("",noteElement.)
                            notes = noteElement.ToDetailsString();
                            string musicBraille = "";
                            string detailString = string.Format("{0} {1} {2} {3} {4}", musicBraille, partId, partName, notes, lyrics);
                            detailsDescriptions.Add(DetailsDescription.Create(detailString));
                            // partId   = ""; // Only list first time
                            partName = ""; // Only list first time
                        }
                    }
                }
            }

            // Do NOT add any harmonies after the last part! HArmonies have their own meshanisms !
            //HarmonyElement harmonyElement = eventDescription.HarmonyElement;
            //if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.Harmonies)) && (null != harmonyElement))
            //{
            //    string harmony = string.Format("{0}{1}  ", harmonyElement.ChromaticRootStep, harmonyElement.LocalizedChordType); // Use same formatting as used in the status line !!
            //    detailsDescriptions.Add(DetailsDescription.Create(harmony));
            //}
        }







        private MidiNote currentDetailsMidiNote = null;
        private MidiChord currentDetailsMidiChord = null;

        public void SelectedDetailsIndexChanged(DetailsDescription detailsDescription)
        {
            if (null != currentDetailsMidiNote)
            {
                musicPlayer.StopMidiNote(currentDetailsMidiNote);
            }
            if (null != currentDetailsMidiChord)
            {
                musicPlayer.StopMidiChord(currentDetailsMidiChord);
            }

            if (null != detailsDescription)
            {
                if (detailsDescription.ContainsStep)
                {
                    // This DetailDescription describes a single note
                    currentDetailsMidiNote = new MidiNote(detailsDescription.Step, detailsDescription.Octave, velocity, Interval.Unison);
                    musicPlayer.StartMidiNote(currentDetailsMidiNote);
                }

                HarmonyElement hE = detailsDescription.HarmonyElement;
                if (null != hE)
                {
                    // This DetailDescription describes a harmony       
                    currentDetailsMidiChord = new MidiChord(hE.ChromaticRootStep, chordOctave, velocity, hE.ChordType, hE.ChromaticBassStep, null);
                    musicPlayer.StartMidiChord(currentDetailsMidiChord);
                }
            }
            

        }

        public void ListBoxDetailsLeave()
        {
            // Stop any note
            if (null != currentDetailsMidiNote)
            {
                musicPlayer.StopMidiNote(currentDetailsMidiNote);
            }

            // stop any chord
            if (null != currentDetailsMidiChord)
            {
                musicPlayer.StopMidiChord(currentDetailsMidiChord);
            }
        }





        public static DetailsPlayer Create(HarmonyElement harmonyElement,MusicPlayer musicPlayer)
        {
            return new DetailsPlayer(harmonyElement,musicPlayer);
        }


        public static DetailsPlayer Create(EventDescription eventDescription, PartlistElement partList, UserSettings userSettings, MusicPlayer musicPlayer, bool noteLevel)
        {
            return new DetailsPlayer(eventDescription, partList, userSettings, musicPlayer, noteLevel);
        }

        public static DetailsPlayer Create(PartlistElement partList)
        {
            return new DetailsPlayer(partList);
        }

    }
}
