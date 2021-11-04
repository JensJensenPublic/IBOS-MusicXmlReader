using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// For handling accidentals within a single measure:
    /// Each time an accidental is found in the Music Braille input, it is added to the related fullstep,
    /// thus influincing the rest of the measure for that fullstep. 
    /// </summary>
    public class MusicXmlBuilderAccidentalHandler
    {
        public enum FullStepEnum { C=0, D=1, E=2, F=3, G=4, A=5, B=6, NumberOfFullSteps=7 } // All internal logic is based on this enum


        const int numberOfFullSteps = (int) FullStepEnum.NumberOfFullSteps; 
        InputSubCategoryEnum[] accidentals;
        InputSubCategoryEnum[] initialAccidentals;

        private int currentKey; // The number of sharps (positive) or flats (negative) in the kurrent key signature 

        private InputSubCategoryEnum currentAccidental = InputSubCategoryEnum.None;  // Note: Accidentals in Music Braille are placed BEFORE the note they relate to !
        public  InputSubCategoryEnum CurrentAccidental
        {
            get { return currentAccidental; }
            set { currentAccidental = value;}
        }

        private void LogCF(string s)
        {
            Logger.LogCF1(s);
        }


        // Internal conversion 
        private int GetIndex(FullStepEnum fullStep)
        {
            switch (fullStep)
            {
                case FullStepEnum.C: return 0;
                case FullStepEnum.D: return 1;
                case FullStepEnum.E: return 2;
                case FullStepEnum.F: return 3;
                case FullStepEnum.G: return 4;
                case FullStepEnum.A: return 5;
                case FullStepEnum.B: return 6;
            }
            Logger.LogCF(string.Format(": Fullstep out of range: {0}", fullStep.ToString()));
            return 0;
        }

        // Internal conversion
        private FullStepEnum GetFullStepEnum(InputSubCategoryEnum fullStep)
        {
            switch (fullStep)
            {
                case InputSubCategoryEnum.FullStepC: return FullStepEnum.C;
                case InputSubCategoryEnum.FullStepD: return FullStepEnum.D;
                case InputSubCategoryEnum.FullStepE: return FullStepEnum.E;
                case InputSubCategoryEnum.FullStepF: return FullStepEnum.F;
                case InputSubCategoryEnum.FullStepG: return FullStepEnum.G;
                case InputSubCategoryEnum.FullStepA: return FullStepEnum.A;
                case InputSubCategoryEnum.FullStepB: return FullStepEnum.B;
            }
            Logger.LogCF(string.Format(": Fullstep out of range: {0}", fullStep.ToString()));
            return 0;
        }

        private FullStepEnum GetFullStepEnum(int fullStep)
        {
            switch (fullStep)
            {
                case 0: return FullStepEnum.C;
                case 1: return FullStepEnum.D;
                case 2: return FullStepEnum.E;
                case 3: return FullStepEnum.F;
                case 4: return FullStepEnum.G;
                case 5: return FullStepEnum.A;
                case 6: return FullStepEnum.B;
            }
            Logger.LogCF(string.Format(": Fullstep out of range: {0}", fullStep.ToString()));
            return FullStepEnum.NumberOfFullSteps;
        }


        private FullStepEnum GetFullStepEnum(string fullStep)
        {
            switch (fullStep)
            {
                case "C": return FullStepEnum.C;
                case "D": return FullStepEnum.D;
                case "E": return FullStepEnum.E;
                case "F": return FullStepEnum.F;
                case "G": return FullStepEnum.G;
                case "A": return FullStepEnum.A;
                case "B": return FullStepEnum.B;
            }
            Logger.LogCF(string.Format(": Fullstep out of range: {0}", fullStep.ToString()));
            return 0;
        }

        private void Set(FullStepEnum fullStep)
        {
            if (currentAccidental == InputSubCategoryEnum.None) return; // Keep existing accidental for this fullstep
            int index = GetIndex(fullStep);
            accidentals[index] = currentAccidental; // Use the accidental for this fullStep for the rest of the measure
            currentAccidental = InputSubCategoryEnum.None; // Do not use this accidental for other fullSteps
        }

        private InputSubCategoryEnum Get(FullStepEnum fullStep)
        {
            int index = GetIndex(fullStep);
            return accidentals[index];
        }

        private int GetAlter(FullStepEnum fullStep)
        { 
            int index = GetIndex(fullStep);
            InputSubCategoryEnum accidental = this.Get(fullStep);
            switch (accidental)
            {
                case InputSubCategoryEnum.None: return 0;
                case InputSubCategoryEnum.AccidentalCourtesyNatural: return 0;
                case InputSubCategoryEnum.AccidentalNatural: return 0;
                case InputSubCategoryEnum.AccidentalCourtesyFlat: return -1;
               case InputSubCategoryEnum.AccidentalFlat: return -1;
               case InputSubCategoryEnum.AccidentalCourtesySharp: return 1;
               case InputSubCategoryEnum.AccidentalSharp: return 1;
            }
            Logger.LogCF(string.Format(": Unexpected Accidental={0}", accidental));
            return 0;
        }

        //
        // Public methods. GetAlter() and Set() both accept either a  InputSubCategoryEnum or a string as input for describing the fullstep.
        //

        public int GetAlter(InputSubCategoryEnum fullStep)
        {
            return GetAlter(GetFullStepEnum(fullStep)); // Call private implementation
        }

        public int GetAlter(string fullStep) // Call private implementation
        {
            return GetAlter(GetFullStepEnum(fullStep));
        }

        public int GetAlter(int fullStep) 
        {
            return GetAlter(GetFullStepEnum(fullStep)); // Call private implementation
        }


        public void Set(InputSubCategoryEnum fullStep)
        {
            Set(GetFullStepEnum(fullStep)); // Call private implementation
        }

        public void Set(int fullStep)
        {
            Set(GetFullStepEnum(fullStep)); // Call private implementation
        }

        public void OnNewMeasure()
        {
            currentAccidental = InputSubCategoryEnum.None;
            ResetAccidentals();
        }

        /// <summary>
        /// Copy all sharps and flats related to the key signature to emulate that they were accidentals
        /// </summary>
        private void ResetAccidentals()
        {
            for (int i = 0; i < numberOfFullSteps; i++)
            {
                accidentals[i] = initialAccidentals[i];
            }
        }


        /// <summary>
        /// Please rfresh your knowledge about "The circle of fifths"
        /// </summary>
        /// <param name="fullStepNumber">Representing one of the fullsteps: 0=C 1=D 2=E 3=F 4=G 5=A 6=B</param>
        /// <param name="currentKey">Represents the Key Signature by the number of shaps (if positive) of flate (if negative)</param>
        /// <returns>The value Sharp, Flat or None representing the initial accidental (Danish: "Fast fortegn"</returns>
        private InputSubCategoryEnum GetInitialAccidental(int fullStepNumber, int currentKey)
        {
            InputSubCategoryEnum result = InputSubCategoryEnum.None;

            // return result;

            switch (fullStepNumber)
            {               
                case 0:  // FullStep C:
                    if (currentKey >= +2) return InputSubCategoryEnum.AccidentalSharp;
                    if (currentKey <= -6) return InputSubCategoryEnum.AccidentalFlat;
                    return InputSubCategoryEnum.None;
                case 1: // FullStep D
                    if (currentKey >= +4) return InputSubCategoryEnum.AccidentalSharp;
                    if (currentKey <= -4 )return InputSubCategoryEnum.AccidentalFlat;
                    return InputSubCategoryEnum.None;
                case 2: // FullStep E
                    if (currentKey >= +6) return InputSubCategoryEnum.AccidentalSharp;
                    if (currentKey <= -2) return InputSubCategoryEnum.AccidentalFlat;
                    return InputSubCategoryEnum.None;
                case 3:
                    // FullStep F
                    if (currentKey >= +1) return InputSubCategoryEnum.AccidentalSharp;
                    if (currentKey <= -7) return InputSubCategoryEnum.AccidentalFlat;
                    return InputSubCategoryEnum.None;
                case 4:
                    // FullStep G
                    if (currentKey >= +3) return InputSubCategoryEnum.AccidentalSharp;
                    if (currentKey <= -5) return InputSubCategoryEnum.AccidentalFlat;
                    return InputSubCategoryEnum.None;
                case 5:
                    // FullStep A
                    if (currentKey >= +5) return InputSubCategoryEnum.AccidentalSharp;
                    if (currentKey <= -3) return InputSubCategoryEnum.AccidentalFlat;
                    return InputSubCategoryEnum.None;
                case 6:
                    // FullStep B
                    if (currentKey >= +7) return InputSubCategoryEnum.AccidentalSharp;
                    if (currentKey <= -1) return InputSubCategoryEnum.AccidentalFlat;
                    return InputSubCategoryEnum.None;
            }
            LogCF(string.Format(": Invalid value of 'FullStepNumber'={0} Must be [0..6]", fullStepNumber));
            return result;
        }

        private MusicXmlBuilderAccidentalHandler(int currentKey)
        {
            if (( currentKey) < -7  || ( +7 < currentKey))
            {
                LogCF(string.Format(": Invalid parameter 'currentKey'={0} Must be in interval [-7 .. 7]", currentKey));
            }
            this.currentKey = currentKey;
            currentAccidental = InputSubCategoryEnum.None;

            // Build an array with each index representing the "fixed accidental" for the current key signature.
            // The array will survive until the key signature is changed.
            // The array is used to initialize the "currentAccidentals" array on the start of a new measure.
            initialAccidentals = new InputSubCategoryEnum[numberOfFullSteps];
            for (int i = 0; (i < numberOfFullSteps); i++)
            {
                initialAccidentals[i] = GetInitialAccidental(i, currentKey);
            }
            accidentals = new InputSubCategoryEnum[numberOfFullSteps];
            ResetAccidentals(); 
        }

        public static MusicXmlBuilderAccidentalHandler Create(int currentKey)
        {
            return new MusicXmlBuilderAccidentalHandler(currentKey);
        }
    }
}
