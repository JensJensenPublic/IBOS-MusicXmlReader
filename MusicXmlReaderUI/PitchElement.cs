using System;

namespace MusicXmlReaderUI
{
    class PitchElement
    {
        // These tables handle altered notes within the range of -2 to +2
        // Note that the octave may be changed in some rare cases!

        public static string[,] names =
        { { "Bb" ,"C"  ,"D"  ,"Es" ,"F"  ,"G"  ,"A"  }, // Alter = -2
          { "B"  ,"Des","Es" ,"E"  ,"Ges","As" ,"Bb" }, // Alter = -1
          { "C"  ,"D"  ,"E"  ,"F"  ,"G"  ,"A"  ,"B"  }, // Alter =  0
          { "Cis","Dis","Eis","Fis","Gis","Ais","C"  }, // Alter = +1   
          { "D"  ,"E"  ,"Fis","G"  ,"A"  ,"B"  ,"Cis"}};// Alter= +2

        public static int[,] carries =
        { { -1,+0,+0,+0,+0,+0,+0 }, // Alter = -2
          { -1,+0,+0,+0,+0,+0,+0 }, // Alter = -1
          { +0,+0,+0,+0,+0,+0,+0 }, // Alter =  0
          { +0,+0,+0,+0,+0,+0,+1 }, // Alter = +1   
          { +0,+0,+0,+0,+0,+0,+1 }};// Alter = +2



        private string name;
        private int octave;
        private int semiTonesAboveC0;

        public string Name
        {
            get
            {
                return name;
            }

        }

        public int Octave
        {
            get
            {
                return octave;
            }
        }

        /// <summary>
        /// Used when generating Music Braille for comparing neighbour tones when deciding if the octave must be repeated
        /// </summary>
        public int SemiTonesAboveC0
        {
            get
            {
                return semiTonesAboveC0;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private PitchElement()
        {
        }

        //int AlterToInt(string alter)
        //{
        //    switch (alter)
        //    {
        //        case "-2": return -2;
        //        case "-1": return -1;
        //        case "0": return  0;
        //        case "1": return  1;
        //        case "2": return 2;
        //        // No attempt to handle all alter values!
        //        default: throw new ArgumentException();
        //    }
        //}

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

        string StepToString(FullStepEnum step)
        {
            switch (step)
            {
                case FullStepEnum.C: return "C";
                case FullStepEnum.D: return "D";
                case FullStepEnum.E: return "E";
                case FullStepEnum.F: return "F";
                case FullStepEnum.G: return "G";
                case FullStepEnum.A: return "A";
                case FullStepEnum.B: return "B";
                // No attempt to handle all alter values!
                default: throw new ArgumentException();
            }
        }

        int SemiToneWithinOctave(FullStepEnum step)
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


        /// <summary>
        /// Constructor, taking care of altered notes
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <param name="octave"></param>
        private PitchElement(FullStepEnum step,int alter,int octave)
        {
            if (0 == alter)
            {
                // Optimize for the simple and frequent case !
                this.name = StepToString(step);
                this.octave = octave;
                return;
            }
            // The note has been altered
            int iAlter = alter;
            int iStep = StepToInt(step);
            this.name = names[  iAlter + 2, iStep]; // Convert iAlter to an index in the table!
            int carry = carries[iAlter + 2, iStep]; // Convert iAlter to an index in the table!
            if (0 == carry)
            {
                this.octave = octave;
            }
            else
            {
                this.octave = octave + carry;
            }

            semiTonesAboveC0 = (12 * octave) + SemiToneWithinOctave(step) + alter;

        }

        public static PitchElement Create(FullStepEnum step, int alter,int octave)
        {
            return new PitchElement(step, alter, octave);
        }

        public override string ToString()
        {
            return name + octave;
        }
    }
}
