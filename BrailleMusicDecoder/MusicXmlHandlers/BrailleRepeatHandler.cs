using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;
using System.Xml;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// For handling repatition only found in the Music Braille representation, not in the graphics.
    /// </summary>
    class BrailleRepeatHandler
    {
        private MusicXmlElementFactory musicXmlElementFactory;
        private XmlNode repetitionStart = null;

        /// <summary>
        /// To be called on creation of a new measure
        /// </summary>
        public void OnNewMeasure()
        {
            repetitionStart = null;
        }

        /// <summary>
        /// To be called on  start of an InAccordFullMeasure
        /// </summary>
        public void OnInAccordFullMeasure()
        {
            repetitionStart = null;
        }

        /// <summary>
        /// To be called whenever a new node is added to a measure
        /// </summary>
        /// <param name="xmlNode"></param>
        public void OnNote(XmlNode xmlNode)
        {
            Set(xmlNode);
        }

        public void OnRest(XmlNode xmlNode)
        {
            Set(xmlNode);
        }

        private void Set(XmlNode xmlNode)
        {
            if (null != repetitionStart) return;
            repetitionStart = xmlNode;
        }

        private bool SomethingToRepeat()
        {
            if (null !=  repetitionStart) return true;
            Logger.LogCF1(": Nothing to repeat.");
            return false;
        }

        private XmlNode GetNumberNode(XmlNode node)
        {
            return node.Attributes.GetNamedItem("number");
        }


        /// <summary>
        /// Special case where another part interrupted immediately after the iitial noDots
        /// </summary>
        public void FullMeasureRepeatNoInitialBlank()
        {
            Logger.LogCF(": *** Not implemented yet! ***");
        }

        /// <summary>
        /// For debugging.
        /// </summary>
        /// <returns></returns>
        private string GetPartInfo()
        {
            MusicXmlBuilderStatePart part = this.musicXmlBuilderStatePart; // Just a shorthand in the following:
            string message = string.Format(": Part.CurrentMeasureNumber={0} Part.CurrentMeasure.MeasureNumber={1}",part.CurrentMeasureNumber,part.CurrentMeasure.Value);
            return message;
        }

        public void RepeatFromRepetitionStart()
        {
            RepeatFromRepetitionStart(1);
        }


        /// <summary>
        /// To be called whenever a repetition within a measure must occur 
        /// </summary>
        public void RepeatFromRepetitionStart(int numberOfRepetitions)
        {
            MusicXmlBuilderStatePart part = musicXmlBuilderStatePart; // Just a shorthand
            Logger.LogCF(string.Format(": Entry {0}",part.GetDebugInfo()));
            if (!SomethingToRepeat())
            {
                part.ClonePreviousMeasure();
                return;
            }

            XmlNode measureNode = repetitionStart.ParentNode; 
            string measureNumber = GetNumberNode(measureNode).Value; // Probably only needed for debugging
            List<XmlNode> nodesToRepeat = new List<XmlNode>();
            List<int> nodeDurations = new List<int>(); // Parallel to nodesToREpeat, contains the durations
            int totalRepeatDuration = 0;
            XmlNode node = repetitionStart;

            while (null != node)
            {
                nodesToRepeat.Add(node);
                // Extract duration
                XmlNode durationElement = node.SelectSingleNode("duration");
                string durationString = durationElement.InnerText;
                int durationInt = int.Parse(durationString);                
                totalRepeatDuration += durationInt;
                nodeDurations.Add(durationInt); // Pick up the durations on the way
                node = node.NextSibling;
            }

#warning: ToDo Only repeat the latests notes until the measure has been filled up !

            int fullMeasureDuration = musicXmlBuilderStatePart.CurrentFullMeasureDivisions;
            int currentPosition = musicXmlBuilderStatePart.CurrentPositionWithinMeasure;
            // int currentPosition = musicXmlBuilderStatePart.
            Logger.LogCF(string.Format(": FullMeasureDuration = {0} TotalDuration = {1} CurrentPositionWithinMeasure = {2}", fullMeasureDuration, totalRepeatDuration, currentPosition));
            int maxRepeatLength = fullMeasureDuration - currentPosition; // Max length of the repeated sequence
            int repeatStart = totalRepeatDuration - maxRepeatLength; 

            int initialCount = measureNode.ChildNodes.Count;
            int position = 0;
            for (int repeats = 0; (repeats < numberOfRepetitions); repeats++)
            {
                for (int i = 0; (i < nodesToRepeat.Count); i++)
                {
                    if (position >= repeatStart) // Drop all notes until there is room for the rest within the measure !
                    {
                        XmlNode nodeToRepeat = nodesToRepeat[i];
                        // We can't append a node once more, so we need to clone it first and append the clone !
                        XmlNode clone = nodeToRepeat.Clone();
                        measureNode.AppendChild(clone);
                    }
                    position += nodeDurations[i];
                }
            }
            int finalCount = measureNode.ChildNodes.Count;
            Logger.LogCF(string.Format(": MeasureNumber={0} NodesToRepeat={1} changing number of nodes from {2} to {3} ", measureNumber, nodesToRepeat.Count,initialCount,finalCount));
        }

        MusicXmlBuilderStatePart musicXmlBuilderStatePart;
        private BrailleRepeatHandler(MusicXmlElementFactory musicXmlElementFactory, MusicXmlBuilderStatePart musicXmlBuilderStatePart)
        {
            this.musicXmlElementFactory = musicXmlElementFactory;
            this.musicXmlBuilderStatePart = musicXmlBuilderStatePart;
        }

        public static BrailleRepeatHandler Create(MusicXmlElementFactory musicXmlElementFactory, MusicXmlBuilderStatePart musicXmlBuilderStatePart)
        {
            return new BrailleRepeatHandler(musicXmlElementFactory, musicXmlBuilderStatePart);
        }
    }
}
