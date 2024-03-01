using System.Collections.Generic;
using System.Xml;
using MusicXmlReaderModel;
using BrailleMusicDecoder.MusicXmlElements;

namespace BrailleMusicDecoder
{
    class MusicXmlTieHandler
    {
        List<MusicXmlNoteElement> allNotesWithStartedTies;
        MusicXmlElementFactory musicXmlElementFactory;

        public void Add(MusicXmlNoteElement newNote)
        {
            allNotesWithStartedTies.Add(newNote);
        }

//        public void AddTieStop(XmlNode newNote)
        public void AddTieStop(MusicXmlNoteElement newNote)
        {
            // Check if allNotesWithStartedTies contains a Note, which matches this one.
            // If so add a TieStop to the new note and remove the matching note from the list
            MusicXmlNoteElement matchingStartNote = null;
            foreach (MusicXmlNoteElement startNode in allNotesWithStartedTies)
            {
                if (Match(startNode, newNote))
                {
                    matchingStartNote = startNode;
                    break;
                }
            }

            if (null != matchingStartNote)
            {
                allNotesWithStartedTies.Remove(matchingStartNote);
                //MusicXmlNoteElement note = newNote as MusicXmlNoteElement;
                newNote.AddTieAndTied(musicXmlElementFactory, "stop");
                // Insert "stop" in newNote
            }
        }

        private bool Match(XmlNode n1, XmlNode n2)
        {
#warning ToDo implement   correctly taking account alter etc         
            XmlNode p1 = n1.SelectSingleNode("pitch");
            XmlNode p2 = n2.SelectSingleNode("pitch");
            if ((null == p1) || (null == p2)) return false; // Added 2024.03.01. Prevent crashes
            XmlNode s1 = p1.SelectSingleNode("step");
            XmlNode s2 = p2.SelectSingleNode("step");
            XmlNode o1 = p1.SelectSingleNode("octave");
            XmlNode o2 = p2.SelectSingleNode("octave");
            if (0 != string.Compare(s1.InnerText, s2.InnerText)) return false;
            if (0 != string.Compare(o1.InnerText, o2.InnerText)) return false;
            // Check alter etc

            string  s =string.Format(": Match found: Step={0} Octave={1}", s1.InnerText, o1.InnerText); //
            // Logger.LogCF(s);
            return true; 
        }


        private MusicXmlTieHandler(MusicXmlElementFactory musicXmlElementFactory)
        {
            this.musicXmlElementFactory = musicXmlElementFactory;
            allNotesWithStartedTies = new List<MusicXmlNoteElement>();
        }

        public static MusicXmlTieHandler Create(MusicXmlElementFactory musicXmlElementFactory)
        {
            return new MusicXmlTieHandler(musicXmlElementFactory);
        }
    }
}
