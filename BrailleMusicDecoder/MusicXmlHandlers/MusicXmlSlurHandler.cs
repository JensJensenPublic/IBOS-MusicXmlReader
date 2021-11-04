using System;
using System.Xml;
using MusicXmlReaderModel;
using BrailleMusicDecoder.MusicXmlElements;

namespace BrailleMusicDecoder.MusicXmlHandlers
{
    /// <summary>
    /// For isolating all handling of slurs in one class
    /// </summary>
    public class MusicXmlSlurHandler
    {
        private MusicXmlElementFactory musicXmlElementFacfory;

        bool longSlurStarted = false;

        public void OnSlur(MusicXmlNoteElement currentNote, InputSubCategoryEnum subCategory, InputSubSubCategoryEnum subSubCategory)
        {
            string value = "";
            switch (subCategory)
            {
                case InputSubCategoryEnum.SlurNormal:
                    switch (subSubCategory)
                    {
                        case InputSubSubCategoryEnum.OccursOnce:  // Starts a short slur or terminates a long slur
                            value = longSlurStarted ? "stop" : "start";
                            longSlurStarted = false;
                            break;
                        case InputSubSubCategoryEnum.OccursTwice: value = "start"; longSlurStarted = true; break; // Starts a long slur
                        default:
                            Logger.LogCF(string.Format(""));
                            break;

                    }
                    break;
                case InputSubCategoryEnum.SlurStartBracketSlur: break;
                case InputSubCategoryEnum.SlurEndBracketSlur: break;
                case InputSubCategoryEnum.SlurDouble: value = "start"; break;
                case InputSubCategoryEnum.SlursThatDoNotLeadtoNotes: break;
                default:
                    Logger.LogCF(string.Format(": Unexpected Subcategory={0}", subCategory));
                    break;
            }
            if (!String.IsNullOrEmpty(value))
            {
                if (null != currentNote)
                {
                    currentNote.AddSlur(musicXmlElementFacfory, value);
                }
                else
                {
                    Logger.LogCF(string.Format("CurrentNote is null while adding slur with value={0}",value));
                }
            }
        }


        private MusicXmlSlurHandler(MusicXmlElementFactory musicXmlElementFactory)
        {
            this.musicXmlElementFacfory = musicXmlElementFactory;
        }

        public static MusicXmlSlurHandler Create(MusicXmlElementFactory musicXmlElementFactory)
        {
            return new MusicXmlSlurHandler(musicXmlElementFactory);
        }
    }
}
