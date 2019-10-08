using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
{


    /// <summary>
    /// Contains score-wide meta-information for each part of the score, sutch as PartName and Instrument.
    /// Does NOT contain musical information such as NoteElements
    /// </summary>
    public class PartlistElement : Element
    {

        /// <summary>
        ///  Allows for identifying each part through a unique integer index.
        /// </summary>
        private ScorePartElement[] partArray;

        List<ScorePartElement> scorePartElements = new List<ScorePartElement>();   

        public int NumberOfParts()
        {
            return scorePartElements.Count;
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private PartlistElement()
        { }

      
        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private PartlistElement(XmlNode node)
        {


            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                // None expected!
                throw new System.ArgumentException();
            }

            // Dig out elements      
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "score-part":
                        this.scorePartElements.Add(ScorePartElement.Create(n));
                        break;
                    //case "part-name": partName = n.InnerText; break;
                    //case "score-instrument":
                    //    // TO DO: Fill in
                    //    break;
                    //case "midi-instrument":
                    //    // TO DO: Fill in
                    //    break;
                    case "part-group":
                        // This is pure graphic information!
                        break;

                    default:
                        throw new System.ArgumentException();
                }
            }

            // Now we know the number of parts. Create an array of them.
            partArray = new ScorePartElement[scorePartElements.Count];
            // Insert a reference to each part at the relevant index.
            int partIndex = 0;      
            foreach (ScorePartElement scorePartElement in scorePartElements)
            {             
                partArray[partIndex] = scorePartElement;
                scorePartElement.partNumber = partIndex++; // Each part knows its own index! 
            }
        }

        public ScorePartElement GetPartFromNumber(int number)
        {
            if (number < partArray.Length) return partArray[number];
            return null;
        }

        public ScorePartElement GetPartFromId(string id)
        {
            foreach (ScorePartElement scorePartElement in partArray)
            {
                if (id == scorePartElement.partId) return scorePartElement;
            }
            return null;
        }

        public ScorePartElement GetPartFromName(string name)
        {
            foreach (ScorePartElement scorePartElement in partArray)
            {
                if (name == scorePartElement.partName) return scorePartElement;
            }
            return null;
        }

        public static PartlistElement Create(XmlNode node)
        {
            return new PartlistElement(node);
        }

        public override string ToString()
        {
            return string.Format( "{0} {1} {2}:" , ResourcesForModel.PartListElement_Message,scorePartElements.Count, ResourcesForModel.PartListElement_Parts);
        }


        /// <summary>
        ///  SAme as ToStrings, but using different formatting
        /// </summary>
        /// <returns></returns>
        public string[] ToUserFriendlyStrings()
        {
            List<string> list = new List<string>();
            foreach (ScorePartElement scorePartElement in partArray)
            {
                // Build up all substrings first:
                string partId = scorePartElement.partId;
                string partName = scorePartElement.partName;  
                string scoreInstrumentString = (null == scorePartElement.ScoreInstrumentElement) ? "" : scorePartElement.ScoreInstrumentElement.ToUserFriendlyString();
                string midiInstrumentString = (null == scorePartElement.MidiInstrumentElement) ? "" : scorePartElement.MidiInstrumentElement.ToUserFriendlyString();
                // Concatenate
                string total = string.Format("{0} {1} {2} {3}", partId, partName, scoreInstrumentString, midiInstrumentString);
                list.Add(total);
                //scorePartElement.ScoreInstrumentString: id, instrumentSound, instrumentName, instrumentAbbreviation, solo, virtualInstrumentElement;
                // MidiInstrumentString: id, midiProgram, midiChannel, midiVolume, pan, midiUnpitchedInstrumentNumber));
            }
            // Convert from List to Array:
            string[] strings = new string[list.Count];
            for (int i = 0; (i < list.Count); i++)
            {
                strings[i] = list[i];
            }
            return strings;
        }


        public string[] ToStrings() 
        {
            List<string> list = new List<string>();
            //foreach (ScorePartElement spe in scorePartElements)
            //{
            //    list.Add(string.Format("Stemme: {0} ({1})",spe.partName,spe.partId));
            //    list.Add(string.Format("   {0}",spe.scoreInstrumentElement.ToString()));
            //    list.Add(string.Format("   {0}",spe.midiInstrumentElement.ToString()));
            //}
            foreach (ScorePartElement scorePartElement in partArray)
            {
                string part = ResourcesForModel.PartListElement_Part;
                list.Add(string.Format("{0}[{1}]: {2} ({3})", part,scorePartElement.partNumber, scorePartElement.partName, scorePartElement.partId));
                list.Add(string.Format("   {0}", scorePartElement.ScoreInstrumentString));
                list.Add(string.Format("   {0}", scorePartElement.MidiInstrumentString));

            }
            
            // Convert from List to Array:
            string[] strings = new string[list.Count];
            for (int i = 0; (i < list.Count); i++)
            {
                strings[i] = list[i];
            }
            return strings;            
        }
        
    }
}
