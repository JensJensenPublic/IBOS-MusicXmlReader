using System;
using System.Collections.Generic;

namespace MusicXmlReaderModel
{

    /// <summary>
    ///  Generally usable class for comparing NoteElements, usable by the Sort() function
    ///  By declaring a class imolementing the IComparer interface instead of just a static Compare() method we allow easy modification through member variables.
    /// </summary>
    public class NoteElementComperator : IComparer<NoteElement>
    {
        /// <summary>
        /// Flags for modifying the default behaviour of the Compare function
        /// </summary>
        [Flags]
        public enum SpecialOptionsEnum
        {
            NoSort = 1,     // Default is: Sort
            HighestPitchLast = 8, // Default is: Highest pitch first (Rest last)
            UseStaffNumbers = 0x10, // Default is: Ignore staff numbers
            IgnorePartNumbers =  0x20, // Default is Use part numbers 
            LogDifferences = 0x40, // Default is Do not log
        }

        private UserSettings userSettings;
        public UserSettings  UserSettings{ get{return userSettings;}}
        private SpecialOptionsEnum specialOptions;
        public  SpecialOptionsEnum SpecialOptions { get { return specialOptions; } }

        private bool noSort = false; // Default is Sort
        private int highPitchValue = -1; // The Default: NoteElements with high pitch are shown first
        private int lowStaffNumberValue = -1; // The Default: Staff 1 is shown first
        private bool usePartNumbers = true;
        private bool useStaffNumbers = false;
        private bool logDifferences = false;


        public int Compare(NoteElement noteX, NoteElement noteY)
        {
            if (noSort) return 0;
            try
            {
                // throw new Exception("test");
                // Put notes with low part numbers before notes with high part numbers
                if (usePartNumbers)
                {
#warning TODO check this !!!   
                    if (noteX.PartNumber > noteY.PartNumber) return +1;
                    if (noteX.PartNumber < noteY.PartNumber) return -1;
                }
                // The notes represent the same part.
                // Put notes with low staff numbers before notes with high staff numbers
                if (useStaffNumbers)
                {
#warning TODO check this !!!   
                    if (noteX.Staff > noteY.Staff) return -lowStaffNumberValue;
                    if (noteX.Staff < noteY.Staff) return lowStaffNumberValue;
                }

                // The notes represent the same part and the same staff
                // Put pauses and rests at the bottom of the list (last in list)
                if ((noteX.UnPitched) || (null == noteX.PitchValue) || (noteX.IsPause)) return -highPitchValue;
                if ((noteY.UnPitched) || (null == noteY.PitchValue) || (noteY.IsPause)) return highPitchValue;
                // Both NoteElements describe real, pitched notes!
                // Put high pitch at the top of the list (first in list)
                if (noteX.Octave > noteY.Octave) return highPitchValue;
                if (noteX.Octave < noteY.Octave) return -highPitchValue;
                // Same octeve
                if (noteX.Step > noteY.Step) return highPitchValue;
                if (noteX.Step < noteY.Step) return -highPitchValue;

            }
            catch (Exception e)
            {
                Logger.LogCFE(e);

            }
            return 0;
        }

        string ValueToString(int value)
        {
            if (value < 0) return "ShowFirst";
            if (value > 0) return "ShowLast";
            return "NoSorting";
        }

        private NoteElementComperator(UserSettings userSettings, SpecialOptionsEnum specialOptionsEnum)
        {
            this.userSettings = userSettings;
            this.specialOptions = specialOptionsEnum;

            if (0 != (this.specialOptions & SpecialOptionsEnum.NoSort))
            {
                noSort = true;
            }

            if (0 != (specialOptions & SpecialOptionsEnum.HighestPitchLast))
            {
                highPitchValue = -highPitchValue; // NoteElements with high pitch is now shown last
            }

            if (0 != (specialOptions & SpecialOptionsEnum.IgnorePartNumbers))
            {
                usePartNumbers = false;
            }

            if (0 != (specialOptions & SpecialOptionsEnum.UseStaffNumbers))
            {
                useStaffNumbers = true;
            }
            Logger.LogCF(string.Format(":  noSort={0} usePartNumbers={1}  useStaffNumbers={2} HighPitchValue={4}", noSort,  usePartNumbers, useStaffNumbers, ValueToString(highPitchValue), logDifferences));
        }
        
        public static NoteElementComperator Create(UserSettings userSettings)
        {
            return new NoteElementComperator(userSettings,0);
        }


        public static NoteElementComperator Create(UserSettings userSettings, SpecialOptionsEnum specialOptionsEnum)
        {
            return new NoteElementComperator(userSettings, specialOptionsEnum);
        }

    }
}
