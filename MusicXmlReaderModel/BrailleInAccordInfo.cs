using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Needed only for generating Braille Music Measure-division information:
    /// While parsing each part of the MusicXml file  a double-linked chain is established between NoteElements which are connected in time and belong to the same part and staff
    /// This chain can later be used to determine exactly which notes should be shown in the same line in Braille Measure Division representation.
    /// </summary>
    public class BrailleInAccordInfo
    {
        private bool log = false;

        private NoteElement owningNoteElement;
        public NoteElement OwningNoteElement { get { return owningNoteElement; } }
        private BrailleInAccordInfo prev;
        public BrailleInAccordInfo Prev { get { return prev; } }
        private BrailleInAccordInfo next;
        public BrailleInAccordInfo Next { set { next = value; } get { return next; } }
        private int chordIndex = 0;
        public int ChordIndex { get { return chordIndex; } }
        

        private BrailleInAccordInfo() { } // Prevent new()

        private bool BuildLink(NoteElement ownerNote)
        {
            if (null == previousInfo) return false; // Nothing to link to
            NoteElement previousNote = previousInfo.owningNoteElement;
            // Note: We can not compare (previousNote.StartTime + previousNote.DurationInCommonDivisions != ownerNote.StartTime) because they are not yet defined !
            if (previousNote.PartNumber != ownerNote.PartNumber) return false;
            if (previousNote.Staff != ownerNote.Staff) return false;
            // We establish the link even if (previousNote.Voice != ownerNote.Voice) !!
            return true;
        }

        private BrailleInAccordInfo(NoteElement ownerNote, int measureNumber)
        {
            this.owningNoteElement = ownerNote;
            currentChordIndex = ownerNote.Chord ? currentChordIndex + 1 : 0;
            this.chordIndex = currentChordIndex;
            if (BuildLink(ownerNote))
            {
                previousInfo.Next = this;
                this.prev = previousInfo;              
            }
            else
            {
                if (log) Logger.LogCF("--");
                currentSequenceIndex = 0;
                currentChordIndex = 0;
            }
            // Note at this point NoteElement.OwningEventDescription is still null, so we need an explicit measurenumber!
            string chordString = ownerNote.Chord ? string.Format(" CHORD[{0}] ",currentChordIndex) : "";
            if (log) Logger.LogCF(string.Format(": Measure={0,-3} I={1,-2} {2}{3}", measureNumber, currentSequenceIndex++,  ownerNote.ToShortDebugString(false, false),chordString)); // (false,false) <=> Do not log times

        }

        public static BrailleInAccordInfo Create(NoteElement ownerNote,int measureNumber)
        {
            BrailleInAccordInfo result = new BrailleInAccordInfo(ownerNote, measureNumber);
            previousInfo = result;
            return result;
        }

        // Internal state variables:
        private static BrailleInAccordInfo previousInfo = null;
        private static int currentSequenceIndex = 0;
        private static int currentChordIndex = 0;

    }
}
