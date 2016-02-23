using System;

namespace MusicXmlReaderUI
{
    class Pitch
    {
        // These tables handle altered notes within the range of -2 to +2
        // Note that the octave may be changed in some rare cases!

        public static string[,] names =
        { { "Bb ","C  ","D  ","Es ","F  ","G  ","A  " }, // Alter = -2
          { "B  ","Des","Es ","E  ","Ges","As ","Bb " }, // Alter = -1
          { "C  ","D",  "E  ","F  ","G  ","A  ","B  " }, // Alter =  0
          { "Cis","Dis","Eis","Fis","Gis","Ais","C  " }, // Alter = +1   
          { "D",  "E",  "Fis","G  ","A  ","B  ","Cis" }};// Alter= +2

        public static int[,] carries =
        { { -1,+0,+0,+0,+0,+0,+0 }, // Alter = -2
          { -1,+0,+0,+0,+0,+0,+0 }, // Alter = -1
          { +0,+0,+0,+0,+0,+0,+0 }, // Alter =  0
          { +0,+0,+0,+0,+0,+0,+1 }, // Alter = +1   
          { +0,+0,+0,+0,+0,+0,+1 }};// Alter = +2



        private string name;
        private string octave;

        public string Name
        {
            get
            {
                return name;
            }

        }

        public string Octave
        {
            get
            {
                return octave;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private Pitch()
        {
        }

        int AlterToInt(string alter)
        {
            switch (alter)
            {
                case "-2": return -2;
                case "-1": return -1;
                case "0": return  0;
                case "1": return  1;
                case "2": return 2;
                // No attempt to handle all alter values!
                default: throw new ArgumentException();
            }
        }

        int StepToInt(string step)
        {
            switch (step)
            {
                case "C": return 0;
                case "D": return 1;
                case "E": return 2;
                case "F": return 3;
                case "G": return 4;
                case "A": return 5;
                case "B": return 6;
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
        private Pitch(string step,string alter,string octave)
        {
            if (string.IsNullOrEmpty(alter))
            {
                // Optimize for the simple and frequent case !
                this.name = step;
                this.octave = octave;
                return;
            }
            // The note has been altered
            int iAlter = AlterToInt(alter);
            int iStep = StepToInt(step);
            this.name = names[  iAlter + 2, iStep]; // Convert iAlter to an index in the table!
            int carry = carries[iAlter + 2, iStep]; // Convert iAlter to an index in the table!
            if (0 == carry)
            {
                this.octave = octave;
            }
            else
            {
                this.octave = (int.Parse(octave) + carry).ToString();
            }
        }

        public static Pitch Create(string step, string alter,string octave)
        {
            return new Pitch(step, alter, octave);
        }
    }
}
