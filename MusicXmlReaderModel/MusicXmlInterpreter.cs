using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// A generally usable class for interpreting MusicXml nodes.
    /// </summary>
    class MusicXmlInterpreter
    {
        private MusicXmlInterpreter(List<MusicXmlObject> allMusicXmlObjecsts,  MetaInformation metaInformation, int divisions)
        {
            this.allMusicXmlObjecsts = allMusicXmlObjecsts;
            this.metaInformation = metaInformation;
            this.divisions = divisions;
        }

        // The following private variables are used for holding the creation parameters
        private int divisions; // Current number of divisions of a quarternode
        private List<MusicXmlObject> allMusicXmlObjecsts; // This is where the result is built up during the call to Recurse()
        private MetaInformation metaInformation;

        // The following members are accesible from the outside through get- or set- accessors
        private bool handleGraphics = true; // Set to false ro speed up execution
        public bool HandleGraphics { set { handleGraphics = value; } }
        DefaultsElement defaults = null; // Score-wide defaults for scaling, layout and appearance. Exactly one DefaultElement is expected per score.
        public DefaultsElement Defaults { get { return defaults; } }

        // The following private members are only used during the interpretation process and never referenced from the outside!
        private int currentMeasureNumber = 0; // Current measure number
        private int latestMeasureNumber = 0;
        private MeasureElement currentMeasureElement;
        private ScorePartElement currentScorePartElement = null;
        private TimeElement currentTimeElement; // Contains the current TimeElement
        private string currentPartId = "";

   
        PartlistElement partList;
        public PartlistElement PartList { get { return partList; } }



        /// <summary>
        /// Handels the syntax analysis of an XML node representing a MusicXML element while reading the MusicXML file.
        /// </summary>
        /// <param name="node">An XML node representing a MusicXML element.</param>
        /// <returns>True <==> Further recursion is required.</returns>
        private bool WriteElement(XmlNode node)
        {
            bool continueRecursion = true;
            switch (node.Name)
            {
                case "note":
                    if (this.currentMeasureNumber != this.latestMeasureNumber)
                    {
                        this.latestMeasureNumber = this.currentMeasureNumber;
                    }
                    NoteElement note = NoteElement.Create(node, this.divisions, this.currentMeasureNumber, this.currentScorePartElement, this.currentTimeElement); // New version
                    note.BrailleMeasureDivisionInfo = BrailleInAccordInfo.Create(note, currentMeasureNumber); // Only needed for creating BrailleMeasureDivision representation
                    allMusicXmlObjecsts.Add(note);
                    continueRecursion = false;
                    break;
                case "part-list":
                    // We also save the part-list in the model for later reference.
                    partList = PartlistElement.Create(node);
                    allMusicXmlObjecsts.Add(partList);
                    //this.numberOfParts = partList.NumberOfParts();
                    // Now we know the number of parts.
                    //userSettings = UserSettings.Create(partList, theUserSettingsFileName);
                    //userSettings.defaultStringFormat = (ScreenReaderAPI.ScreenReaderType.NVDA == screenReaderAPI.GetScreenReaderType()) ? "{1}" : "{0} {1}";
                    continueRecursion = false;
                    break;
                case "measure":
                    MeasureElement measureElement = MeasureElement.Create(node);
                    measureElement.PartId = currentPartId;
                    measureElement.MeasureDuration = (null == currentTimeElement) ? 0 : currentTimeElement.GetMeasureDuration();
                    // Logger.LogCF(string.Format(": Duration={0}", measureElement.MeasureDuration));
                    allMusicXmlObjecsts.Add(measureElement); // Avoid the "Ikke VAlgt" error message from screenreader
                    this.currentMeasureNumber = measureElement.Number;
                    measureElement.PreviousMeasureElement = currentMeasureElement;
                    this.currentMeasureElement = measureElement;
                    break;
                case "score-part":
                    // Describes the meta-data related to a part.
                    // This includes "part-name", "score-instrument" and "midi-instrument".
                    // (The notes and pauses are described in "part")
                    ScorePartElement scorePartElement = ScorePartElement.Create(node);
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
                    this.currentScorePartElement = partList.GetPartFromId(partElement.PartId);
                    // Intialization of instruments has been moved to MusicPlayer (where it belongs)
                    this.currentMeasureElement = null;
                    break;
                case "work":
                    SimpleTextElement workElement = SimpleTextElement.Create(node, "Titel"); // TODO: Localize
                    allMusicXmlObjecsts.Add(workElement);
                    metaInformation.Work = MetaInfoItem.Create(workElement.Name, workElement.Text);
                    continueRecursion = false;
                    break;
                case "movement-title":
                    SimpleTextElement movementTitle = SimpleTextElement.Create(node, "Opus"); // TODO: Localize
                    allMusicXmlObjecsts.Add(movementTitle);
                    metaInformation.MovementTitle = MetaInfoItem.Create(movementTitle.Name, movementTitle.Text);
                    continueRecursion = false;
                    break;
                case "movement-number":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Nummer"));
                    continueRecursion = false;
                    break;
                case "identification":
                    break;
                case "creator":
                    CreatorElement creatorElement = CreatorElement.Create(node);
                    allMusicXmlObjecsts.Add(creatorElement);
                    metaInformation.DublinCore.Creator = MetaInfoItem.Create(creatorElement.Name, creatorElement.Value);
                    continueRecursion = false;
                    break;
                case "rights":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Rettigheder"));
                    continueRecursion = false;
                    break;
                case "encoding":
                    // Is described in "software", "encoding-date", "encoder", "encoding-description"
                    //allMusicXmlObjecsts.Add(SimpleTextElement.Create(node)); 

                    break;
                case "software":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Software"));
                    continueRecursion = false;
                    break;
                case "encoding-date":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Arrangements dato"));
                    continueRecursion = false;
                    break;
                case "encoder":
                    allMusicXmlObjecsts.Add(SimpleTextElement.Create(node, "Arrangement"));
                    continueRecursion = false;
                    break;
                case "encoding-description":
                    SimpleTextElement encodingDescriptionElement = SimpleTextElement.Create(node, "Kodnings-beskrivelse");
                    allMusicXmlObjecsts.Add(encodingDescriptionElement);
                    Logger.LogCFOnce(string.Format(": Encoding='{0}'" ,node.InnerText));
                    metaInformation.Encoding = MetaInfoItem.Create(encodingDescriptionElement.Name, encodingDescriptionElement.Text);
                    continueRecursion = false;
                    break;
                case "direction":
                    DirectionElement directionElement = DirectionElement.Create(node);
                    allMusicXmlObjecsts.Add(directionElement);
                    if (null != directionElement.SoundElement)
                    {
                        // DirectionElement may contain a soundelement
                        allMusicXmlObjecsts.Add(directionElement.SoundElement);
                    }
                    continueRecursion = false;
                    break;
                case "transpose":
                    TransposeElement transposeElement = TransposeElement.Create(node);
                    allMusicXmlObjecsts.Add(transposeElement);
                    this.currentScorePartElement.TransposeElement = transposeElement;
                    continueRecursion = false;
                    break;
                case "divisions":
                    DivisionsElement divisionsElement = DivisionsElement.Create(node);
                    allMusicXmlObjecsts.Add(divisionsElement);
                    this.divisions = divisionsElement.Divisions;
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
                    break;
                case "time":
                    currentTimeElement = TimeElement.Create(node);
                    allMusicXmlObjecsts.Add(currentTimeElement);
                    continueRecursion = false;
                    break;
                case "clef":
                    allMusicXmlObjecsts.Add(ClefElement.Create(node, this.currentScorePartElement));
                    continueRecursion = false;
                    break;
                case "print":
                    if (handleGraphics)
                    {
                        // This is all graphics stuff, but is decoded anyway to make the information about
                        // New System and New PAge available to the user
                        allMusicXmlObjecsts.Add(PrintElement.Create(node, handleGraphics));
                    }
                    continueRecursion = false; // This is all graphics stuff!
                    break;
                case "defaults":
                    if (handleGraphics)
                    {
                        if (null != defaults)
                        {
                            Logger.LogCFOnce("More than one DefaultsElement found for one score");
                        }
                        defaults = DefaultsElement.Create(node, handleGraphics);  // Score-wide graphic information  
                    }      
                    continueRecursion = false; // This is all graphics stuff!
                    break;
                case "score-partwise":
                    allMusicXmlObjecsts.Add(ScorePartwiseElement.Create(node));
                    break;
                case "attributes": // Maybe attributes are always found under MeasureElement ??
                    //Logger.LogOnce(string.Format("{0}.{1} Unimplemented element: Name={2} Parent.Name={3}",
                    //    className, functionName, node.Name,node.ParentNode.Name));
                    allMusicXmlObjecsts.Add(AttributesElement.Create(node));       // Ignore until we need them 
                    // WE CONTINUE RECURSION below the attributes element, which may contain a lot of other relevant elements:
                    // footnote, level, divisions, key, time, staves, part-symbol,instruments, clef, staff-details, transpose, directive,measure-style
                    // For the time being there is no need to structure these elements into the Attribute Element !          
                    break;
                case "measure-repeat":
                    allMusicXmlObjecsts.Add(MeasureRepeatElement.Create(node));
                    continueRecursion = false;
                    break;
                case "measure-style":
                    allMusicXmlObjecsts.Add(MeasureStyleElement.Create(node));
                    continueRecursion = false;
                    break;
                case "backup":
                    allMusicXmlObjecsts.Add(BackupElement.Create(node, divisions));
                    continueRecursion = false;
                    break;
                case "forward":
                    allMusicXmlObjecsts.Add(ForwardElement.Create(node, divisions));
                    continueRecursion = false;
                    break;
                case "repeat":
                    // TODO Implement !!
                    allMusicXmlObjecsts.Add(RepeatElement.Create(node));
                    break;
                case "barline":
                    allMusicXmlObjecsts.Add(BarlineElement.Create(node, currentScorePartElement));
                    continueRecursion = false;
                    break;
                case "instruments":
                    allMusicXmlObjecsts.Add(InstrumentsElement.Create(node));
                    continueRecursion = false;
                    break;
                case "source":
                    SimpleTextElement source = SimpleTextElement.Create(node, "Source");
                    allMusicXmlObjecsts.Add(source);
                    metaInformation.DublinCore.Source = MetaInfoItem.Create(source.Name, source.Text);
                    continueRecursion = false;
                    break;
                case "miscellaneous":
                    SimpleTextElement miscellaneous = SimpleTextElement.Create(node, "Miscellaneous");
                    allMusicXmlObjecsts.Add(miscellaneous);
                    continueRecursion = false;
                    // Logger.LogCF(string.Format(": Created SimpleTextElement(Name='{0}' Text='{1}')",miscellaneous.Name,miscellaneous.Text));
                    Logger.LogCFOnce(": Created SimpleTextElement from MiscellaneousElement");
                    break;
                case "staves":
                    StavesElement staves = StavesElement.Create(node);
                    allMusicXmlObjecsts.Add(staves);
                    break;
                // The following elements are ignored for the time being, as they describe graphical properties only!
                case "offset":
                case "supports":
                case "staff-details":
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
                case "bar-style":
                case "staff-size":
                case "staff-lines":
                case "staff-tuning":
                case "staff-octave":
                case "measure-numbering":
                    break; // Explicitly ignoring graphic information!
                case "tuning-octave":
                case "tuning-step":
                case "capo":
                    break; // Also ignore these until they are needed!
                default:
                    Logger.LogCFOnce(string.Format(": Name={0}", node.Name));
                    allMusicXmlObjecsts.Add(UnimplementedElement.Create(node));
                    break;
            }
            return continueRecursion;
        }


        /// <summary>
        /// Recursively interprets the childrenNodes
        /// </summary>
        /// <param name="childrenNodes"></param>
        public void Recurse(XmlNodeList childrenNodes)
        {
            foreach (XmlNode childNode in childrenNodes)
            {
                bool doRecursion = true;
                switch (childNode.NodeType)
                {
                    case XmlNodeType.Element:
                        doRecursion = WriteElement(childNode);
                        break;
                    case XmlNodeType.Comment:
                        break;
                }
                if (doRecursion)
                {
                    Recurse(childNode.ChildNodes);
                }

            }
        }



        /// <summary>
        /// Establish an environment to use during the call to the primary "Recourse()" method
        /// </summary>
        /// <param name="allMusicXmlObjecsts">The data structure receiving all MusicXml Elements found during the interpretation proces</param>
        /// <param name="metaInformation">The data structure for collecting all metadata information found during the interpretation</param>
        /// <param name="divisions">The number of divisions internally used roe representing used for a quarter node</param>
        /// <returns></returns>
        public static MusicXmlInterpreter Create(List<MusicXmlObject> allMusicXmlObjecsts,  MetaInformation metaInformation, int divisions)
        {
            return new MusicXmlInterpreter(allMusicXmlObjecsts, metaInformation, divisions);
        }
    }
}
