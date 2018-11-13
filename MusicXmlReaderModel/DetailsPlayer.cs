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
        private const string className = "DetailsPlayer";
        private const int bassOctave = 3; // Play the Bass note (if any) in 3. octave
        private DetailsDescription detailsDescriptionCurrentlyPlaying;

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

        private DetailsPlayer(MusicPlayer musicPlayer)
        {
            this.musicPlayer = musicPlayer;
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
            const int chordOctave   = 4; // Build the chord starting in 4. octave, possibly spreading into 5. octave
            const int dynamics      = (int)Dynamics.mf; // Dynamics "velocity" of each note. Percentage of max velocity.
            this.musicPlayer = musicPlayer;
            Interval[] intervals = MidiChord.GetModifiedChordIntervals(harmonyElement.ChordType, harmonyElement.Degrees);        
            ChromaticStep root = harmonyElement.ChromaticRootStep;
            detailsDescriptions = new List<DetailsDescription>();
            string chordName = harmonyElement.ToString();
            //detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", ResourcesForModel.HarmonyElement_Chord, chordName), harmonyElement, chordOctave)); // The full representation of the chord 
            detailsDescriptions.Add(DetailsDescription.Create(chordName, harmonyElement, chordOctave)); // The full representation of the chord 
            if ((null != harmonyElement.BassElement) && (harmonyElement.ChromaticRootStep != harmonyElement.ChromaticBassStep))
            {
                string s = string.Format("{0} {1}", ResourcesForModel.HarmonyElement_BassTone, harmonyElement.BassElement.ToString());
                detailsDescriptions.Add(DetailsDescription.Create(s, harmonyElement.ChromaticBassStep,bassOctave,dynamics)); // The bass tone if different from the root. For instance "Bass E" 
            }
            for (int i = 0; (i < intervals.Length); i++) // Each note in the Harmony, represented by function and by name. 
            {
                int iSum = ((int)root) + ((int)intervals[i]); // Do simple arithmetics !
                int iStep   = iSum % (int)ChromaticStep.NumberOfSteps; // 12
                int iOctave = iSum / (int)ChromaticStep.NumberOfSteps; // 12
                ChromaticStep step = (ChromaticStep)(iStep);                
                string function = MidiChord.ToLocalizedChordFunction(intervals[i]);
                string s = string.Format("{0} {1}", function, step.ToString());
                detailsDescriptions.Add(DetailsDescription.Create(s, step, iOctave + chordOctave,dynamics)); // For instance : "Third E"
            };
            detailsDescriptions.Reverse();
        }


        /// <summary>
        /// Build a list of all parts
        /// NOTE: The contents of this list does NOT depend of the position of the cursor on the List-Window, only of the parts defined in the MusicXml file.
        /// </summary>
        /// <param name="partList"></param>
        private DetailsPlayer(PartlistElement partList,MusicPlayer musicPlayer)
        {
            int numberOfParts = partList.NumberOfParts();
            this.musicPlayer = musicPlayer;
            detailsDescriptions = new List<DetailsDescription>();
        //    string text = (1 == numberOfParts) ? ResourcesForModel.DetailsPlayer_Part : ResourcesForModel.DetailsPlayer_Parts; // Singularis / Pluralis
        //    detailsDescriptions.Add(DetailsDescription.Create(string.Format("{0} {1}", numberOfParts, text)));
        }

        private DetailsPlayer()
        {
            detailsDescriptions = new List<DetailsDescription>();
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
        /// Add the textual description of an Element
        /// </summary>
        /// <param name="element"></param>
        private void AddStatusText(Element element)
        {
            if (null == element) return;
            detailsDescriptions.Add(DetailsDescription.Create(element.ToString()));
        }

        /// <summary>
        /// Add a simple line of text
        /// </summary>
        /// <param name="text"></param>
        private void AddStatusText(string text)
        { 
            detailsDescriptions.Add(DetailsDescription.Create((null == text) ? "" : text));
        }

        private void AddStatusText(string pre, Element element, string post)
        {
            string s1 = (null == pre) ? "" : pre;
            string s2 = (null == element) ? "" : element.ToString();
            string s3 = (null == post) ? "" : post;
            AddStatusText(s1 + s2 + s3);
        }

        private string Format(EventElement eventElement)
        {
            return (null == eventElement) ? "null" : eventElement.Caption + " " + eventElement.ToString();
        }

        private string Format(HarmonyElement harmonyElement)
        {
            // In this implementation we explicitly return "No Chord" if no harmony is found.
            // We might also choose to skip the full line (This must be implemented in the function calling Format: (DetailsPlayer(StatusInformation statusInformation))
            return (null == harmonyElement) ? MidiChord.LocalizedChordKindNone : harmonyElement.Caption + " " + harmonyElement.ToString();
        }

        private string Format(Element element, string s)
        {
            return element.Caption + " " + s;
        }

        private string Format(MeasureFraction fraction)
        {
            return  " " + ((null == fraction) ? "?" : fraction.ToString());
        }

        private string Format(MetronomeElement metronomeElement, string s)
        {
            string caption = (null == metronomeElement) ? ResourcesForModel.MetronomeElement_Tempo : metronomeElement.Caption;
            return caption + " " + s;
        }

        /// <summary>
        /// Describes the format of the status information when shown in the Details window
        /// Please compare to StatusInformation.Format()
        /// </summary>
        /// <param name="statusInformation"></param>
        private DetailsPlayer(StatusInformation statusInformation)
        {
            detailsDescriptions = new List<DetailsDescription>();
            //AddStatusText(Format(statusInformation.CurrentMeasureElement) + " + " + statusInformation.CurrentMeasureFractions.ToString());
            AddStatusText(Format(statusInformation.CurrentMeasureElement) + Format(statusInformation.CurrentMeasureFraction)); // + " + statusInformation.CurrentMeasureFractions.ToString());
            AddStatusText(Format(statusInformation.CurrentKeyElement));
            AddStatusText(Format(statusInformation.CurrentMetronomeElement,statusInformation.GetTempoString())); 
            AddStatusText(Format(statusInformation.CurrentHarmonyElement));
            AddStatusText(Format(statusInformation.CurrentTimeElement));
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
            this.musicPlayer = musicPlayer;
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

                    if ((userSettings.MusicAsSpeech) && (userSettings.GetParts(UserSettings.Category.Speech,i)))
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
                    if ((userSettings.MusicAsMusicBraille) && (userSettings.GetParts(UserSettings.Category.MusicBraille,i)))
                    {
                        BrailleBuilder musicBrailleDetails = eventDescription.NotesForOnePartAsBraille(notesForPart);
                        musicBraille = musicBrailleDetails.ToBrailleString();
                    }

                    // Compose all details, always showing MusicBraille first
                    string detailString = string.Format("{0} {1} {2} {3} {4}", musicBraille, partId, partName, notes, lyrics);
                    if (!string.IsNullOrWhiteSpace(detailString))
                    {
                        detailsDescriptions.Add(DetailsDescription.Create(detailString, eventDescription.NoteLists[i]));
                    }
                }

            }
        }
        

        /// <summary>
        /// Reports details at the Note level
        /// Is activated directly from the NoteList, not from a specific part, so we need to specify information about the part too.
        /// </summary>
        /// <param name="eventDescription"></param>
        /// <param name="partList"></param>
        /// <param name="userSettings"></param>
        /// <param name="musicPlayer"></param>
        private void NoteDetailsPlayer(EventDescription eventDescription, PartlistElement partList, UserSettings userSettings, MusicPlayer musicPlayer)
        {
            int numberOfParts = partList.NumberOfParts();
            this.musicPlayer = musicPlayer;

            detailsDescriptions = new List<DetailsDescription>();
            for (int i = 0; (i < numberOfParts); i++)
            {
                List<NoteElement> notesForPart = eventDescription.NoteLists[i];
                if ((null != notesForPart) && (0 != notesForPart.Count))
                {
                    // String variables for desribing the detail as text
                    string partId = "";
                    string partName = "";

                    if ((userSettings.MusicAsSpeech) && (userSettings.GetParts(UserSettings.Category.Speech, i)))
                    {
                        // The eventdescription contains notes for this part so we dig out the part parameters:
                        ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                        partId = scorePartElement.partId;
                        partName = scorePartElement.partName;
                        string partString = string.IsNullOrEmpty(partName) ? partId : partName; // Prefere PartName for PartId, i.i "Violin" for "P1"
                        bool showLeftRightHand = (partName == "Piano");
#warning ToDO Add Organ  etc !!
                        // By using  eventDescription.NotesForOnePart for formatting the notes we assure the usage of identical formatting.
                        foreach (NoteElement noteElement in eventDescription.NoteLists[i])
                        {
                            //notes = string.Format("",noteElement.)
                            string leftRightHand = showLeftRightHand ? noteElement.LocalizedHand() : "";
                            string noteString = noteElement.ToDetailsString(); // Only information from the NoteElement and elements contained within it
                            string musicBraille = "";
                            string lyrics = noteElement.Text;
                            string detailString = string.Format("{0} {1} {2} {3} {4}", musicBraille, partString, leftRightHand, noteString, lyrics); // All information, including information from the NoteElement 
                            detailsDescriptions.Add(NoteListDetailsDescription.Create(detailString, noteElement));
                            // partId   = ""; // Only list first time
                            partName = ""; // Only list first time
                        }
                    }
                }

                detailsDescriptions.Sort(Compare);

            }
        }


        /// <summary>
        /// If x and y are both of type SingleNoteDetailsDescription and are both pitched
        /// the function returns the difference (in semitones) between the frequencies of the 2 notes.
        /// Otherwise it returns 0
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(DetailsDescription x, DetailsDescription y)
        {
            try
            {
                if (!((x is SingleNoteDetailsDescription) && (y is SingleNoteDetailsDescription)))
                {
                    //Logger.LogCF(string.Format("x.Type={0} y.Type={1}", x.GetType(), y.GetType()));
                    return 0;
                }

                SingleNoteDetailsDescription nx = x as SingleNoteDetailsDescription;
                SingleNoteDetailsDescription ny = y as SingleNoteDetailsDescription;

                NoteElement nex = nx.NoteElement;
                NoteElement ney = ny.NoteElement;

                if (!(nex.Pitched && (ney.Pitched)))
                {
                    //Logger.LogCF(string.Format("x.Pitched={0} y.Pitched={1}", nex.Pitched, ney.Pitched));
                    return 0;
                }
                
                int result = ney.PitchValue.SemiTonesAboveC0 - nex.PitchValue.SemiTonesAboveC0;
                // Logger.LogCF(string.Format("Result = {0}", result));
                return result;
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            return 0;

        }


        public void SelectedDetailsIndexChanged(DetailsDescription detailsDescription)
        {
            string functionName = "SelectedDetailsIndexChanged";
            try
            {
                if (null != detailsDescriptionCurrentlyPlaying)
                {
                    detailsDescriptionCurrentlyPlaying.Stop(musicPlayer);
                }

                if (null != detailsDescription)
                {
                    detailsDescription.Play(musicPlayer);
                    detailsDescriptionCurrentlyPlaying = detailsDescription;
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} failed. Message= {2}", className, functionName, e.Message));
                System.Media.SystemSounds.Hand.Play();
            }
        }

        public void ListBoxDetailsLeave()
        {
            // Stop any note

            if (null != detailsDescriptionCurrentlyPlaying)
            {
                detailsDescriptionCurrentlyPlaying.Stop(musicPlayer);
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

        public static DetailsPlayer Create(PartlistElement partList, MusicPlayer musicPlayer)
        {
            return new DetailsPlayer(partList,musicPlayer);
        }


        public static DetailsPlayer Create(MusicPlayer musicPlayer)
        {
            return new DetailsPlayer(musicPlayer);
        }

        public static DetailsPlayer Create(StatusInformation statusInformation)
        {
            return new DetailsPlayer(statusInformation);
        }

        public static  DetailsPlayer Create()
        {
            return new DetailsPlayer();
        }
        
    }
}
