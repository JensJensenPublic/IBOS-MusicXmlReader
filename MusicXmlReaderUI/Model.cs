using System.Collections.Generic;
using NAudio.Midi;
using System.Windows.Forms;
using System.Xml;

namespace MusicXmlReaderUI
{
    class Model
    {
        List<MusicXmlObject> allMusicXmlObjecsts; // Holds all information from the .xml file
        MidiOut midiOut;
        ListBox listBoxParts; // Lists elements grouped per part
        ListBox listBoxPoly; // Lists elements grouped per time
        public MusicPlayer musicPlayer;
        public PartlistElement partList;
        int divisions; // Current number of divisions of a quarternode
        int tempo;     // Current tempo in beats pr minute
        int currentMeasureNumber = 0 ; // Current measure number
        int latestMeasureNumber = 0;
        int numberOfParts; // Number of parts
        //int currentPartitionNumber = -1;
        string currentPartId = "";
        int currentPartNumber = 0;
        ScorePartElement scorePartElement = null;
        UserSettings userSettings;
        string fullXmlFileName;


        public void LoadMusicXmlFile()
        {
            {
                string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);
                string fileName = "Node.xml";
                fullXmlFileName = System.IO.Path.Combine(executingDirectory, fileName);
                XmlDocument doc = new XmlDocument();
                XmlTextReader reader = new XmlTextReader(fullXmlFileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                doc.Load(reader);
                Recurse(doc.ChildNodes);
                Init(); // Experimental code !! 
                LoadListBox();
            }
        }

        
        /// <summary>
        /// Constructor
        /// </summary>
        public Model(ListBox listBox, ListBox listBoxPoly)
        {
            allMusicXmlObjecsts = new List<MusicXmlObject>();
            midiOut = new MidiOut(0);
            musicPlayer = new MusicPlayer(listBox, listBoxPoly,midiOut);
            this.listBoxParts = listBox;
            this.listBoxPoly = listBoxPoly;
        }
 
        public bool WriteElement(XmlNode node)
        {
            bool continueRecursion = true;
            switch (node.Name)
            {
                case "note":
                    // If the measure number has changed add the new measure number to the node
                    int tempMeasureNumber = 0;
                    if (this.currentMeasureNumber != this.latestMeasureNumber)
                    {
                        this.latestMeasureNumber = this.currentMeasureNumber;
                        tempMeasureNumber = this.currentMeasureNumber;
                    }
                    allMusicXmlObjecsts.Add(NoteElement.Create(node, this.divisions, tempMeasureNumber, this.currentPartId, this.currentPartNumber));
                    continueRecursion = false;
                    break;
                case "part-list":
                    // We also save the part-list in the model fpr later reference.
                    partList = PartlistElement.Create(node);
                    allMusicXmlObjecsts.Add(partList);
                    this.numberOfParts = partList.NumberOfParts();
                    // Now we know the number of parts.
                    userSettings = UserSettings.Create(this.numberOfParts);
                    // allMusicXmlObjecsts.AddRange(   (partitionList.ToStrings());
                    continueRecursion = false;
                    break;
                case "measure":
                    MeasureElement measureElement = MeasureElement.Create(node);
                    // allMusicXmlObjecsts.Add(measureElement); // Avoid the "Ikke VAlgt" error message from screenreader
                    this.currentMeasureNumber = int.Parse(measureElement.Number);
                    break;
                case "score-part":
                    // Describes the meta-data related to a part.
                    // This includes "part-name", "score-instrument" and "midi-instrument".
                    // (The notes and pauses are described in "part")
                    scorePartElement = ScorePartElement.Create(node);
                    allMusicXmlObjecsts.Add(scorePartElement);
                    continueRecursion = false;
                    break;
                case "part": 
                    // Describes the notes (and pauses) of a part.
                    // (The mata-data is described in "score-part")               
                    PartElement partElement = PartElement.Create(node);
                    allMusicXmlObjecsts.Add(partElement);
                    // Save the current part Id
                    this.currentPartId = partElement.PartId;
                    // Look up the partition in the partList
                    ScorePartElement currentScorePartElement = partList.GetPartFromId(partElement.PartId);
                    // Save the part number as the current part number. This will be saved with each note!  
                    this.currentPartNumber = currentScorePartElement.partNumber;        
                    break;
                case "software":
                case "encoding-date":
                case "rights":
                case "encoding":
                case "identification":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node));
                    break;
                case "divisions":
                    DivisionsElement divisionsElement = DivisionsElement.Create(node);
                    allMusicXmlObjecsts.Add(divisionsElement);
                    this.divisions = divisionsElement.GetDivisions();
                    break;
                case "key":
                    allMusicXmlObjecsts.Add(KeyElement.Create(node));
                    continueRecursion = false;
                    break;
                case "harmony":
                    allMusicXmlObjecsts.Add(HarmonyElement.Create(node));
                    continueRecursion = false;
                    break;
                case "sound":
                    SoundElement soundElement = SoundElement.Create(node);
                    allMusicXmlObjecsts.Add(soundElement);
                    this.tempo = soundElement.GetTempo();
                    break;
                case "time":
                    allMusicXmlObjecsts.Add(TimeElement.Create(node));
                    continueRecursion = false;
                    break;
                case "clef":
                    allMusicXmlObjecsts.Add(ClefElement.Create(node));
                    continueRecursion = false;
                    break;
                case "print":
                    continueRecursion = false; // This is all graphics stuff!
                    break;
                case "defaults":
                    continueRecursion = false; // This is all graphics stuff!
                    break;
                case "score-partwise":
                    allMusicXmlObjecsts.Add(ScorePartwiseElement.Create(node));                 
                    break;
                case "attributes":
                     // Ignore until we need them
                     break;
                case "backup":
                    // Needed soon!
                    allMusicXmlObjecsts.Add(BackupElement.Create(node, divisions));
                    continueRecursion = false;
                    break;
                case "forward":
                    // Needed soon!
                    allMusicXmlObjecsts.Add(ForwardElement.Create(node, divisions));
                    continueRecursion = false;
                    break;
                case "scaling":
                case "millimeters":
                case "tenths":
                case "page-layout":
                case "page-height":
                case "page-width":
                case "page-margins":
                case "left-margin":
                case "top-margin":
                case "bottom-margin":
                case "system-layout":
                case "system-margins":
                case "top-system-distance":
                case "staff-layout":
                case "staff-distance":
                case "appearance":
                case "line-width":
                case "right-margin":
                case "system-distance":
                case "note-size":
                case "distance":
                case "music-font":
                case "word-font":
                case "credit":
                case "credit-type":
                case "credit-words":
                case "barline":
                case "bar-style":
                    break; // Explicitly ignoring graphic information!
                default:
                    allMusicXmlObjecsts.Add(UnimplementedElement.Create(node));
                    break;
            }
            return continueRecursion;
        }


        public void Recurse(XmlNodeList childrenNodes)
        {
            foreach (XmlNode childNode in childrenNodes)
            {

                //if (showRaw)
                //{
                //    // The Unfiltered data
                //    if (listBoxRaw.Items.Count >= 1000) return;
                //    switch (childNode.NodeType)
                //    {
                //        case XmlNodeType.Element:
                //            listBoxRaw.Items.Add(childNode.NodeType + " " + childNode.Name);
                //            break;
                //    }
                //}

                // The filtered data

                bool doRecursion = true;
                switch (childNode.NodeType)
                {
                    case XmlNodeType.Element:
                        doRecursion = WriteElement(childNode);
                        break;
                    case XmlNodeType.Comment:
                        // allMusicXmlObjecsts.Add(childNode.InnerText);
                        break;
                }
                if (doRecursion)
                {
                    Recurse(childNode.ChildNodes);
                }                          
                                    
            }
        }

        // The following 2 lists contain references into allMusicXmlObjecsts where the decoded information is kept! 
        private PartDescriptionList  partDescriptionList;
        private TimeDescriptionList  timeDescriptionList;
        private EventDescriptionList eventDescriptionList;

        public string FullXmlFileName
        {
            get
            {
                return fullXmlFileName;
            }
        }

        //private MeasureDescriptionList measureDescriptionList;

        public void Init()
        {
            partDescriptionList = PartDescriptionList.Create(allMusicXmlObjecsts,numberOfParts);
            divisions = 24; // TODO compute!
            timeDescriptionList = TimeDescriptionList.Create(partDescriptionList, divisions);
            eventDescriptionList = EventDescriptionList.Create(timeDescriptionList, numberOfParts,userSettings);
            //measureDescriptionList = MeasureDescriptionList.Create(allMusicXmlObjecsts);
        }

        //*****************************************************************************************
        // Event handlers called directly from the GUI and distributing control to other objects.
        //*****************************************************************************************

        public void LoadListBox()
        {
            foreach (MusicXmlObject musicXmlObject in allMusicXmlObjecsts)
            {
                {
                    // Add ALL objects to make it possible to browse manually through them
                    // During auto-play only node items (including pauses) will be selected to allow for correct timing!
                    listBoxParts.Items.Add(musicXmlObject);
                }
            }
        }


        public void StartPlayingMono()
        {
            musicPlayer.Reset(numberOfParts);
            musicPlayer.StartPlayingMono();
        }

        public void StartPlayingPoly()
        {
            musicPlayer.Reset(numberOfParts);
            musicPlayer.StartPlayingPoly();
        }

        public void LoadListBoxTimes()
        {
            eventDescriptionList.LoadListBox(listBoxPoly);
            //timeDescriptionList.LoadListBox(listBoxTimes); 
        }

        public void LoadListBoxOfParts(CheckedListBox checkedListBox)
        {
            for (int i = 0; (i < numberOfParts); i++)
            {
                ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
                checkedListBox.Items.Add(string.Format("{0} {1}", scorePartElement.partId, scorePartElement.partName));
                checkedListBox.SetItemChecked(i, true);
            }
            checkedListBox.CheckOnClick = true;
        }

        //public void LoadListBoxPartsToPlay(CheckedListBox checkedListBoxPartsToPlay)
        //{
        //    for (int i = 0; (i < numberOfParts); i++)
        //    {
        //        ScorePartElement scorePartElement = partList.GetPartFromNumber(i);
        //        checkedListBoxPartsToPlay.Items.Add(string.Format("{0} {1}", scorePartElement.partId, scorePartElement.partName));
        //        checkedListBoxPartsToPlay.SetItemChecked(i, true);
        //    }
        //    checkedListBoxPartsToPlay.CheckOnClick = true;
        //}

        public void LoadListBoxPartsToRead(CheckedListBox checkedListBoxPartsToPlay)
        {
        }


        public void SetPartsToPlay(int partNumber, bool value)
        {
            this.userSettings.partsToPlay[partNumber] = value;
            //musicPlayer.PartsToPlay = this.userSettings.partsToPlay;
            musicPlayer.UserSettings = this.userSettings;
        }

        public void SetPartsToRead(int partNumber, bool value)
        {
            this.userSettings.partsToRead[partNumber] = value;
        }

        public void PlaySpeedChanged(float newValue)
        {
            userSettings.userSlowDown = newValue;
        }


        public void SetShowStartTime(bool value)
        {
            userSettings.readDivisions = value;
        }

        public void SetShowHarmonies(bool value)
        {
            userSettings.readHarmonies = value;
        }


        public void SetShowHarmonyCodes(bool value)
        {
            userSettings.readHarmonyCodes = value;
        }

        public void SetPlayHarmonies(bool value)
        {
            userSettings.playHarmonies = value;
        }
    }
}
