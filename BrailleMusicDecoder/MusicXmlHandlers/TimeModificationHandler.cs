using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder.MusicXmlHandlers
{
    class TimeModificationHandler
    {
        MusicXmlElementFactory musicXmlElementFactory;

        private XmlNode currentTimemodification = null; // For adding timemodification element
        int currentTupletBracketNumber; // For adding tuplet brackets
        int lastTupletBracketNumber; // For adding tuplet brackets
        int actualNotes = 3; //
        int normalNotes = 2; //


        public void OnNewMeasure()
        {
            currentTimemodification = null; // A time modification never survives to the next measure.
        }

        public int GetNextTupleInfo(out XmlNode timeModification, out XmlNode bracketInformation, ref int divisions)
        {
            timeModification = null;
            bracketInformation = null;
            if (null == this.currentTimemodification)
            {
                // No timemodification is active. No logging in this case.
                this.currentTupletBracketNumber = -1;
                return -1; // Divisions is unchanged
            }
            
            currentTupletBracketNumber++;
            timeModification = this.currentTimemodification.Clone(); // Need to clone othrerwise overwrite will occur!
            string bracketString = BracketNumberToString(currentTupletBracketNumber);
            bracketInformation = (null == bracketString) ? null : musicXmlElementFactory.TupletBracket(currentTupletBracketNumber,bracketString);
            int temp = divisions * normalNotes;
            divisions = temp / actualNotes;
            Logger.LogCF(string.Format(": CurrentTupletBracketNumber= {0} of {1} TimeModification={2} BracketInformation={3}", currentTupletBracketNumber, lastTupletBracketNumber, timeModification.InnerText, (null == bracketInformation) ? "" :  bracketInformation.InnerText));
            if (currentTupletBracketNumber == lastTupletBracketNumber)
            {
                this.currentTimemodification = null;
                this.currentTupletBracketNumber = -1;
            }
            return currentTupletBracketNumber;

        }


        /// <summary>
        /// In Music Braille a triplet is described by dot23 in front of the first note in the triplet
        /// In MusicXml a tuplet (and thus also a triplet) is described by adding 2 XmlNodes to each of the notes:
        /// 1) An XmlNode immediately after the type describing the timemodification as a fraction, for instance 3/2 for a tuplet - and used for music synthesis
        /// 2) An XmlNode in the notations node describing the graphial looks of the timemodification.
        /// </summary>
        public void OnTripletStart()
        {
            Logger.LogCF(string.Format(": Is not completely implemented yet."));
            this.actualNotes = 3;
            this.normalNotes = 2;
            this.currentTimemodification  = musicXmlElementFactory.TimeModificationElement(actualNotes, normalNotes);
            this.currentTupletBracketNumber = 0;
            this.lastTupletBracketNumber = 3;
           
        }

        private string BracketNumberToString(int bracketNumber)
        {
            switch (bracketNumber)
            {
                case 1: return "start";
 //               case 2: return "continue";
                case 2: return null; // No BracketElement generated
                case 3: return "stop";
                default:    Logger.LogCF(string.Format(": Unexpected bracketNumbet={0}", bracketNumber));
                    return "";
            }
        }

        private TimeModificationHandler()
        {}

        private TimeModificationHandler(MusicXmlElementFactory musicXmlElementFactory)
        {
            this.musicXmlElementFactory = musicXmlElementFactory;
        }

        static public  TimeModificationHandler Create(MusicXmlElementFactory musicXmlElementFactory)
        {
            return new TimeModificationHandler(musicXmlElementFactory);
        }
    }
}
