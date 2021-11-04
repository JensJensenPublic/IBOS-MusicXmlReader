using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MusicXmlReaderModel;
using System.Text;

namespace BrailleMusicDecoder.MusicXmlHandlers
{


    /// <summary>
    /// Handles the Graphic repeat symbols. (Not the Music Braille specific PartMeasureRepeat and FullMeasure repeat !)
    /// </summary>
    class GraphicRepeatHandler
    {
        private MusicXmlElementFactory musicXmlElementFactory;


        public void OnStartRepeat(XmlNode currentMeasureNode)
        {
            XmlNode startRepeat = musicXmlElementFactory.RepeatElement("forward");
            Logger.LogCF(ToString(startRepeat));
            OnRepeat(currentMeasureNode, startRepeat, RepeatDirection.forward);
        }

        public void OnEndRepeat(XmlNode currentMeasureNode)
        {            
            XmlNode endRepeat = musicXmlElementFactory.RepeatElement("backward");
            Logger.LogCF(ToString(endRepeat));
            OnRepeat(currentMeasureNode,endRepeat,RepeatDirection.backward);


        }

        private XmlNode pendingFirstEndingStop;

        public void OnFirstEnding(XmlNode currentMeasureNode,string text)
        {
            Logger.LogCF(string.Format("({0})", text));
            XmlNode firstEnding = musicXmlElementFactory.EndingElement(text, "start");
            pendingFirstEndingStop = musicXmlElementFactory.EndingElement(text, "stop");
            Logger.LogCF(ToString(firstEnding));
            OnEnding(currentMeasureNode,firstEnding);
        }

        private XmlNode pendingSecondEndingDiscontinue;


        private int GetNumberOfRepetitions(XmlNode firstEnding, string secondEnding)
        {
            // Find the number of repetitions in the first ending
            XmlNode numberAttribute = firstEnding.Attributes.GetNamedItem("number");
            string numberValue = numberAttribute.Value;
            string[] numbers1 = numberValue.Split(',');
            int repeatitionsInFirstEnding = numbers1.Length;
            string[] numbers2 = secondEnding.Split(',');
            int repeatitionsInSecondEnding = numbers2.Length;
            return repeatitionsInFirstEnding + repeatitionsInSecondEnding;
        }

        public void OnSecondEnding(XmlNode currentMeasureNode, string text)
        {
            Logger.LogCF(string.Format("({0})", text));
            //// Terminate first ending using the pending EndingElement created when the first ending was added.
            XmlNode previousMeasureNode = currentMeasureNode.PreviousSibling;
            if ((null != previousMeasureNode) && (null != pendingFirstEndingStop))
            {
                // Insert a right barline with this ending into the previous measure:
                // (This is the first time we can compute the total number of repetitions)
                int nRepetitions = GetNumberOfRepetitions(pendingFirstEndingStop, text);
       
            
                XmlNode barlineElement1 = musicXmlElementFactory.BarLineElement(MusicXmlElementFactory.BarStyleEnum.lightHeavy, MusicXmlElementFactory.BarLocationEnum.right);
                barlineElement1.AppendChild(pendingFirstEndingStop);
              
                if (nRepetitions > 2)
                {
                    // Create and add a repeatelement describing the total number of repetitions. Not needed for the simple cases
                    XmlNode repeatElement = musicXmlElementFactory.RepeatElement("backward");
                    repeatElement.Attributes.Append(musicXmlElementFactory.NameValueAttribute("times", nRepetitions.ToString()));
                    barlineElement1.AppendChild(repeatElement);
                }

                previousMeasureNode.AppendChild(barlineElement1);
            }
            else
            {
                Logger.LogCF(string.Format(": Failed create FirstEndingStop. {0} {1}",
                    (null != previousMeasureNode) ? "" : "previousMeasureNode is null", // {0}
                    (null != pendingFirstEndingStop) ? "" : "pendingFirstEndingStop is null")); // {1}
            }
            pendingFirstEndingStop = null; // Not pending anymore

           
            // Insert a first ending start at the (existing) left barline at the beginning of this measure
            XmlNode secondEnding = musicXmlElementFactory.EndingElement(text, "start");
            pendingSecondEndingDiscontinue = musicXmlElementFactory.EndingElement(text, "discontinue");
            Logger.LogCF(ToString(secondEnding));
            OnEnding(currentMeasureNode,secondEnding);

            // Insert  the pending second ending discontinue as a right bar
            XmlNode barlineElement2 = musicXmlElementFactory.BarLineElement(MusicXmlElementFactory.BarStyleEnum.normal, MusicXmlElementFactory.BarLocationEnum.right);
            barlineElement2.AppendChild(pendingSecondEndingDiscontinue);
            currentMeasureNode.AppendChild(barlineElement2);
        }

        private bool IsFirstEnding(List<string> values)
        {
            bool result = (values[0] == "1");
            return result;
        }

        public void OnIntervalEnding(XmlNode currentMeasure, List<string> values)
        {
            string parameters = string.Format("({0}-{1})", values[0], values[1]);
            Logger.LogCF(string.Format("{0}",parameters));
            int firstValue = int.Parse(values[0]);
            int lastValue = int.Parse(values[1]);
            StringBuilder s = new StringBuilder();
            string glue = "";
            for (int i = firstValue; (i<=lastValue); i++)
            {
                s.Append(glue + i.ToString());
                glue = ", "; 
            }
            string text = s.ToString();
            if (IsFirstEnding(values))
            {
                OnFirstEnding(currentMeasure, text); 
            }
            else
            {
                OnSecondEnding(currentMeasure, text); 
            }
        }

        public void OnNumericEnding(XmlNode currentMeasure, List<string> values)
        {
            string parameters = string.Format("({0})", values[0]);
            Logger.LogCF(string.Format("{0}", parameters));
            if (IsFirstEnding(values))
            {
                OnFirstEnding(currentMeasure, values[0]);
            }
            else
            {
                OnSecondEnding(currentMeasure, values[0]);
            }
        }

        private string ToString(XmlNode xmlNode)
        {
            return string.Format(": XML={0}", xmlNode.OuterXml);
        }

        private enum RepeatDirection { forward, backward};
        private enum InsertMode { prepend, append};


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="currentMeasureNode"></param>
        ///// <param name="xmlNode"></param>
        ///// <param name="repeatDirection"></param>
        ///// <param name="insertMode">Insert or prepend: Ending must always be placed before repeat</param>
        //private void OnRepeatOrEnding(XmlNode currentMeasureNode, XmlNode xmlNode, RepeatDirection repeatDirection, InsertMode insertMode)
        //{
        //    switch (repeatDirection)
        //    {
        //        case RepeatDirection.backward:
        //            // The barline already exists. Modify it
        //            XmlNode currentBarline = currentMeasureNode.SelectSingleNode("barline");
        //            if (null != currentBarline)
        //            {
        //                if (InsertMode.prepend == insertMode)
        //                {
        //                    currentBarline.PrependChild(xmlNode);
        //                }
        //                else
        //                {
        //                    currentBarline.AppendChild(xmlNode);
        //                }
        //            }
        //            else
        //            {
        //                Logger.LogCF(string.Format(": Barline is null when attempting to add {0}", xmlNode.OuterXml));
        //            }
        //            break;
        //        case RepeatDirection.forward:
        //            // Create a new barline for the forward element :
        //            XmlNode newBarline = elements.BarLineElement(MusicXmlBuilderElements.BarStyleEnum.heavyLight, MusicXmlBuilderElements.BarLocationEnum.left);
        //            newBarline.AppendChild(xmlNode);
        //            // Find and remove any existing barline element
        //            XmlNode oldBarline = currentMeasureNode.SelectSingleNode("barline");
        //            if (null != oldBarline)
        //            {
        //                currentMeasureNode.RemoveChild(oldBarline);
        //            }
        //            currentMeasureNode.AppendChild(newBarline);
        //            break;
        //    }

        //}

        private void OnEnding(XmlNode currentMeasureNode, XmlNode xmlNode)
        {
            // The barline already exists. Modify it
            XmlNode currentBarline = currentMeasureNode.SelectSingleNode("barline");
            if (null != currentBarline)
            {
#warning todo: Find a general way to handle sorting of Chile elements to fir specification!!
                // Respect sequence : bar-style, ending, repeat
                XmlNode barStyleElenent = currentBarline.SelectSingleNode("bar-style");
                if (null != barStyleElenent)
                {
                    currentBarline.InsertAfter(xmlNode, barStyleElenent);
                }
                else
                {
                    currentBarline.PrependChild(xmlNode);
                }
            }
            else
            {
                Logger.LogCF(string.Format(": Barline is null when attempting to add Ending {0}", xmlNode.OuterXml));
            }
        }



        private void OnRepeat(XmlNode currentMeasureNode, XmlNode xmlNode, RepeatDirection repeatDirection)
        {
            switch (repeatDirection)
            {

                case RepeatDirection.backward:
                    // The barline already exists. Modify it
                    XmlNode currentBarline = currentMeasureNode.SelectSingleNode("barline");
                    if (null != currentBarline)
                    {   
                            currentBarline.AppendChild(xmlNode);                     
                    }
                    else
                    {
                        Logger.LogCF(string.Format(": Barline is null when attempting to add Repeat {0}", xmlNode.OuterXml));
                    }
                    break;
                case RepeatDirection.forward:
                    // Create a new barline for the forward element :
                    XmlNode newBarline = musicXmlElementFactory.BarLineElement(MusicXmlElementFactory.BarStyleEnum.heavyLight, MusicXmlElementFactory.BarLocationEnum.left);
                    newBarline.AppendChild(xmlNode);
                    // Find and remove any existing barline element
                    XmlNode oldBarline = currentMeasureNode.SelectSingleNode("barline");
                    if (null != oldBarline)
                    {
                        currentMeasureNode.RemoveChild(oldBarline);
                    }
                    currentMeasureNode.AppendChild(newBarline);
                    break;
            }

        }







        private GraphicRepeatHandler(MusicXmlElementFactory musicXmlElementFactory)
        {
            this.musicXmlElementFactory = musicXmlElementFactory;
        }

        static public GraphicRepeatHandler Create(MusicXmlElementFactory musicXmlElementFactory)
        {
            return new GraphicRepeatHandler(musicXmlElementFactory);
        }
    }
}
