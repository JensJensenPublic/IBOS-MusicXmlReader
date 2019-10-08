using System;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Abstract class used by PitchElement, RootElement and  BassElement 
    /// </summary>
    public abstract class PitchElementBase
    {
        // The basic enumeration for the 8 note values representing the white keys on the piano and used by MusicXml
        // Also includes a special value for Rest. This seems to fit the physical world: Rest.Frequency=0 !
        public enum FullStepEnum { Unknown = 0, C, D, E, F, G, A, B, Rest };
        public enum HSE { Unknown = 0, C, Cis, Des, D, Dis, Es, E, Eis, F, Fis, Ges, G, Gis, As, A, Ais, Bb, B }; // HSE is short for HalfStepEnum

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


        public string GetLocalizedPitch(HSE pitch)
        {
            switch (pitch)
            {
                case HSE.C: return ResourcesForModel.PitchElement_c;
                case HSE.Cis: return ResourcesForModel.PitchElement_cSharp;
                case HSE.Des: return ResourcesForModel.PitchElement_dFlat;
                case HSE.D: return ResourcesForModel.PitchElement_d;
                case HSE.Dis: return ResourcesForModel.PitchElement_dSharp;
                case HSE.Es: return ResourcesForModel.PitchElement_eFlat;
                case HSE.E: return ResourcesForModel.PitchElement_e;
                case HSE.Eis: return ResourcesForModel.PitchElement_eSharp;
                case HSE.F: return ResourcesForModel.PitchElement_f;
                case HSE.Fis: return ResourcesForModel.PitchElement_fSharp;
                case HSE.Ges: return ResourcesForModel.PitchElement_gFlat;
                case HSE.G: return ResourcesForModel.PitchElement_g;
                case HSE.Gis: return ResourcesForModel.PitchElement_gSharp;
                case HSE.As: return ResourcesForModel.PitchElement_aFlat;
                case HSE.A: return ResourcesForModel.PitchElement_a;
                case HSE.Ais: return ResourcesForModel.PitchElement_aSharp;
                case HSE.Bb: return ResourcesForModel.PitchElement_b;
                case HSE.B: return ResourcesForModel.PitchElement_b;
                case HSE.Unknown:
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unknown Pitch value={2}", className, "GetLocalizedPitch", pitch));
                    return "";
            }
        }

        private const string className = "PitchElementBase";
        protected int alter = 0; 
        protected FullStepEnum step;
        protected string name;
        protected int semiTonesAboveC0;
        protected int fullStepsAboveC0;

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

        /// <summary>
        /// (Possibly) used for generating Music Braille Intervalrepresentation
        /// </summary>
        public int FullStepsAboveC0
        {
            get
            {
                return fullStepsAboveC0;
            }
        }


        public int Alter
        {
            get
            {
                return alter;
            }
        }
        
        public FullStepEnum Step
        {
            get
            {
                return step;
            }
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

  
        protected string StepToString(FullStepEnum step)
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


    }
}
