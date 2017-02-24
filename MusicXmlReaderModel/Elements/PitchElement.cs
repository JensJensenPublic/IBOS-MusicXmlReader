using System;
using System.Xml;

namespace MusicXmlReaderModel
{
    public class PitchElement
    {
        // The basic enumeration for the 8 note values representing the white keys on the piano and used by MusicXml
        // Also includes a special value for Rest. This seems to fit the physical world: Rest.Frequency=0 !
        public enum FullStepEnum { Unknown = 0, C, D, E, F, G, A, B, Rest }; 
        public enum HSE {Unknown = 0,C,Cis,Des,D,Dis,Es,E,Eis,F,Fis,Ges,G,Gis,As,A,Ais,Bb,B }; // HSE is short for HalfStepEnum

        // These tables handle altered notes within the range of -2 to +2
        // Note that the octave may be changed in some rare cases!

        public static HSE[,] enums =       
        { { HSE.Bb ,HSE.C  ,HSE.D  ,HSE.Es ,HSE.F  ,HSE.G  ,HSE.A  }, // Alter = -2
          { HSE.B  ,HSE.Des,HSE.Es ,HSE.E  ,HSE.Ges,HSE.As ,HSE.Bb }, // Alter = -1
          { HSE.C  ,HSE.D  ,HSE.E  ,HSE.F  ,HSE.G  ,HSE.A  ,HSE.B  }, // Alter =  0
          { HSE.Cis,HSE.Dis,HSE.Eis,HSE.Fis,HSE.Gis,HSE.Ais,HSE.C  }, // Alter = +1   
          { HSE.D  ,HSE.E  ,HSE.Fis,HSE.G  ,HSE.A  ,HSE.B  ,HSE.Cis}};// Alter= +2

        public static int[,] carries =
        { { -1,+0,+0,+0,+0,+0,+0 }, // Alter = -2
          { -1,+0,+0,+0,+0,+0,+0 }, // Alter = -1
          { +0,+0,+0,+0,+0,+0,+0 }, // Alter =  0
          { +0,+0,+0,+0,+0,+0,+1 }, // Alter = +1   
          { +0,+0,+0,+0,+0,+0,+1 }};// Alter = +2


        private string GetLocalizedPitch(HSE pitch)
        {
            switch (pitch)
            {
                case HSE.C: return ResourcesForModel.PitchElement_c;
                case HSE.Cis: return ResourcesForModel.PitchElement_cSharp;
                case HSE.Des: return ResourcesForModel.PitchElement_dFlat;
                case HSE.D: return ResourcesForModel.PitchElement_d;
                case HSE.Dis: return ResourcesForModel.PitchElement_dSharp;
                case HSE.Es:return ResourcesForModel.PitchElement_eFlat;
                case HSE.E:return ResourcesForModel.PitchElement_e;
                case HSE.Eis:return ResourcesForModel.PitchElement_eSharp;
                case HSE.F:return ResourcesForModel.PitchElement_f;
                case HSE.Fis: return ResourcesForModel.PitchElement_fSharp;
                case HSE.Ges:return ResourcesForModel.PitchElement_gFlat;
                case HSE.G:return ResourcesForModel.PitchElement_g;
                case HSE.Gis: return ResourcesForModel.PitchElement_gSharp;
                case HSE.As:return ResourcesForModel.PitchElement_aFlat;
                case HSE.A:return ResourcesForModel.PitchElement_a;
                case HSE.Ais:return ResourcesForModel.PitchElement_aSharp;
                case HSE.Bb: return ResourcesForModel.PitchElement_b;
                case HSE.B: return ResourcesForModel.PitchElement_b;
                case HSE.Unknown:
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unknown Pitch value={2}", className, "GetLocalizedPitch", pitch));
                    return "";
            }
        }


        private const string className = "PitchElement";
        protected int alter = 0;
        int octave = 0;
        protected FullStepEnum step;

        private string name;
        private int semiTonesAboveC0;


        public string Name
        {
            get
            {
                return name;
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

        public int Alter
        {
            get
            {
                return alter;
            }
        }

        public int Octave
        {
            get
            {
                return octave;
            }
        }

        public FullStepEnum Step
        {
            get
            {
                return step;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        protected PitchElement()
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



        protected FullStepEnum GetFullStep(string s)
        {
            switch (s)
            {
                case "C": return FullStepEnum.C;
                case "D": return FullStepEnum.D;
                case "E": return FullStepEnum.E;
                case "F": return FullStepEnum.F;
                case "G": return FullStepEnum.G;
                case "A": return FullStepEnum.A;
                case "B": return FullStepEnum.B;
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unknown Fullstep value={2}", className, "GetFullStep", s));
                    return FullStepEnum.Unknown;
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
                int carry = carries[iAlter + 2, iStep]; // Convert iAlter to an index in the table!
                if (0 != carry)
                {
                    this.octave = octave + carry;
                }
            }

        

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
