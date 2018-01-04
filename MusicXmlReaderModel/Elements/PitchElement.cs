using System;
using System.Xml;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Class for handling NoteElement.pitch
    /// </summary>
    public class PitchElement : PitchElementBase
    {
        private int octave = 0;

        /// <summary>
        /// NOTE: "Octave" is inplemented by PitchElement, but neither by BassElement nor RootElement 
        /// </summary>
        public int Octave
        {
            get
            {
                return octave;
            }
        }

        public int SemiToneWithinOctave(FullStepEnum step)
        {
            switch (step)
            {
                case FullStepEnum.C: return 0;
                case FullStepEnum.D: return 2;
                case FullStepEnum.E: return 4;
                case FullStepEnum.F: return 5;
                case FullStepEnum.G: return 7;
                case FullStepEnum.A: return 9;
                case FullStepEnum.B: return 11;
                // No attempt to handle all alter values!
                default: throw new ArgumentException();
            }
        }


        int StepToInt(FullStepEnum step)
        {
            switch (step)
            {
                case FullStepEnum.C: return 0;
                case FullStepEnum.D: return 1;
                case FullStepEnum.E: return 2;
                case FullStepEnum.F: return 3;
                case FullStepEnum.G: return 4;
                case FullStepEnum.A: return 5;
                case FullStepEnum.B: return 6;
                // No attempt to handle all alter values!
                default: throw new ArgumentException();
            }
        }
        

        /// <summary>
        /// Constructor, taking care of altered notes
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <param name="octave"></param>
        private PitchElement(XmlNode node)
        {
            step = GetFullStep(Utilities.GetChildValue(node, "step")); // Special parsing of step
            Utilities.Parse(Utilities.GetChildValue(node, "alter"), ref alter, -2, +2, "PitchElement: alter", true);
            Utilities.Parse(Utilities.GetChildValue(node, "octave"), ref octave, 0, 9, "PitchElement: octave", false);

            // First compute the value of semiTonesAboveC0 from the ORIGINAL parameters
            semiTonesAboveC0 = (12 * octave) + SemiToneWithinOctave(step) + alter;

            if (0 == alter)
            {
                // Optimize for the simple and frequent case !
                this.name = StepToString(step);
            }
            else
            {
                // The note has been altered
                int iAlter = alter;
                int iStep = StepToInt(step);
                HSE pitch = enums[iAlter + 2, iStep];
                this.name = GetLocalizedPitch(pitch);
                //this.name = names[iAlter + 2, iStep]; // Convert iAlter to an index in the table!
            //    int carry = carries[iAlter + 2, iStep]; // Convert iAlter to an index in the table!
            //    if (0 != carry)
            //    {
            //        this.octave = octave + carry;
            //    }
            }

        

        }


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        protected PitchElement()
        {
        }


        /// <summary>
        /// An non-typical implementation: Returns null if no "step" is specified
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static PitchElement Create(XmlNode node)
        {            
            string stepString = Utilities.GetChildValue(node, "step");
            if (!string.IsNullOrEmpty(stepString))
            {
                return new PitchElement(node);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Used for handling Unpitched notes as pitched notes, not as rests !
        /// </summary>
        /// <param name="step"></param>
        /// <param name="octave"></param>
        private PitchElement(FullStepEnum step, int octave)
        {
            this.step = step;
            this.octave = octave; 
        }

        /// <summary>
        ///  Used for handling Unpitched notes as pitched notes, not as rests !
        /// </summary>
        /// <param name="step"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        public static PitchElement Create(FullStepEnum step, int octave)
        {
            return new PitchElement(step, octave);
        }


        //public static PitchElement Create(FullStepEnum step, int alter, int octave)
        //{
        //    return new PitchElement(step, alter, octave);
        //}

        public override string ToString()
        {
            return name + octave;
        }
    }
}
