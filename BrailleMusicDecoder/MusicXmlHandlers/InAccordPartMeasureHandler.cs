using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;
using System.Xml;

namespace BrailleMusicDecoder
{
    class InAccordPartMeasureHandler
    {
        private const int unknownPosition = 0;
        private int positionOfLatestMeasureDivisionWithinMeasure = unknownPosition;
        private MusicXmlElementFactory musicXmlElementFactory;

        public void OnNewMeasure()
        {
            positionOfLatestMeasureDivisionWithinMeasure = 0; // As default a partmeasure measuredivision stats at start of measure un til the first measuredivision is found
        }

        public void OnMeasureDivision(int positionWithinMeasure)
        {
            positionOfLatestMeasureDivisionWithinMeasure = positionWithinMeasure;
        }

        public int OnInAccordPartMeasure(int positionWithinMeasure)
        {
            int backup = positionWithinMeasure - positionOfLatestMeasureDivisionWithinMeasure;
            Logger.LogCF(string.Format(": Position={0} DivisionPosition={1} Backup={2} ", positionWithinMeasure, positionOfLatestMeasureDivisionWithinMeasure, backup));
            return backup;
 
        }

        private InAccordPartMeasureHandler(MusicXmlElementFactory musicXmlElementFactory)
        {
            this.musicXmlElementFactory = musicXmlElementFactory;
        }

        public static InAccordPartMeasureHandler Create(MusicXmlElementFactory musicXmlElementFactory)
        {
            return new InAccordPartMeasureHandler(musicXmlElementFactory);
        }
    }
}
