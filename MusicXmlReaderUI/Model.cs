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
        ListBox listBoxTimes; // Lists elements grouped per time
        public MusicPlayer musicPlayer;
        public PartlistElement partList;
        int divisions; // Current number of divisions of a quarternode
        int tempo;     // Current tempo in beats pr minute
        int currentMeasureNumber = 0 ; // Current measure number
        int latestMeasureNumber = 0;
        int numberOfParts; // Number of parts
        //int currentPartitionNumber = -1;
        string currentPartitionId = "";


        /// <summary>
        /// Constructor
        /// </summary>
        public Model(ListBox listBox, ListBox listBoxTimes)
        {
            allMusicXmlObjecsts = new List<MusicXmlObject>();
            midiOut = new MidiOut(0);
            musicPlayer = new MusicPlayer(listBox, midiOut);
            this.listBoxParts = listBox;
            this.listBoxTimes = listBoxTimes;
        }
 
        public bool WriteElement(XmlNode node)
        {
            bool continueRecursion = true;
            switch (node.Name)
            {
                case "note":
                    // If the measure number has changed ann the new measure number to the node
                    int tempMeasureNumber = 0;
                    if (this.currentMeasureNumber != this.latestMeasureNumber)
                    {
                        this.latestMeasureNumber = this.currentMeasureNumber;
                        tempMeasureNumber = this.currentMeasureNumber;
                    }
                    allMusicXmlObjecsts.Add(NoteElement.Create(node, this.divisions, tempMeasureNumber, this.currentPartitionId));
                    continueRecursion = false;
                    break;
                case "part-list":
                    // We also save the partitionlist in the model fpr later reference.
                    partList = PartlistElement.Create(node);
                    allMusicXmlObjecsts.Add(partList);
                    this.numberOfParts = partList.NumberOfParts();
                    // allMusicXmlObjecsts.AddRange(   (partitionList.ToStrings());
                    continueRecursion = false;
                    break;
                case "measure":
                    MeasureElement measureElement = MeasureElement.Create(node);
                    // allMusicXmlObjecsts.Add(measureElement); // Avoid the "Ikke VAlgt" error message from screenreader
                    this.currentMeasureNumber = int.Parse(measureElement.Number);
                    break;
                case "score-part":
                    allMusicXmlObjecsts.Add(ScorePartElement.Create(node));
                    continueRecursion = false;
                    break;
                case "part":
                    PartElement partElement = PartElement.Create(node);
                    allMusicXmlObjecsts.Add(partElement);
                    this.currentPartitionId = partElement.PartId; 
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
        private PartDescriptionList partDescriptionList;
        private TimeDescriptionList timeDescriptionList;
        //private MeasureDescriptionList measureDescriptionList;

        public void Init()
        {
            partDescriptionList = PartDescriptionList.Create(allMusicXmlObjecsts,numberOfParts);
            divisions = 24; // TODO compute!
            timeDescriptionList = TimeDescriptionList.Create(partDescriptionList, divisions);
            //measureDescriptionList = MeasureDescriptionList.Create(allMusicXmlObjecsts);
        }

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

        public void LoadListBoxTimes()
        {
            timeDescriptionList.LoadListBox(listBoxTimes);
        }

    }
}
