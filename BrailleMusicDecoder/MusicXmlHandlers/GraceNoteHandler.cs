using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// Collects gracenote information for a note.
    /// This is needed because grace note informatio in MusicBraille is placed before the note.
    /// This class is modelled over the ArticulationsHAndler
    /// </summary>
    class GraceNoteHandler
    {
        private MusicXmlElementFactory musicXmlElementFactory;
        private bool isGrace;
        private bool slash;

        internal void OnGrace(bool slash)
        {
            isGrace = true;
            this.slash = slash;
        }

        internal bool OnNote(XmlNode currentNote)
        {
            if (!isGrace) return false;
            // "Thus grace notes do not have a duration element. Cue notes have a duration element, as do forward elements, but no tie elements."
            XmlNode durationElement = currentNote.SelectSingleNode("duration");
            currentNote.RemoveChild(durationElement);
            //// It looks like we also have to remove a tieElement if we remove a durationElement
            //XmlNode tieElement = currentNote.SelectSingleNode("tie");
            //if (null != tieElement)
            //{
            //    currentNote.RemoveChild(tieElement);
            //}
            // If a GreceElement exists it must be the first element in the note: 
            XmlNode graceElement = musicXmlElementFactory.GraceElement(slash);
            XmlNode firstElement = currentNote.FirstChild;
            currentNote.InsertBefore(graceElement, firstElement);
            isGrace = false;
            return true;
        }


        private GraceNoteHandler(MusicXmlElementFactory musicXmlElementFactory)
        {
            this.musicXmlElementFactory = musicXmlElementFactory;
        }
        

        public static GraceNoteHandler Create(MusicXmlElementFactory musicXmlElementFactory)
        {
            return new GraceNoteHandler(musicXmlElementFactory);
        }

    }
}
