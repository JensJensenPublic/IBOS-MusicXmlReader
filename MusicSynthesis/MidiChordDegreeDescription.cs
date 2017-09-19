using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicSynthesis; // Needed because The Localisation files are defined there !

namespace JSJ.MusicSynthesis
{
    public enum DegreeTypeEnum { unknown, add, alter, subtract, none };

    public class MidiChordDegreeDescription
    {

        string className = "MidiChordDegreeDescription";
        int degree = 0;
        int alter = 0;
        DegreeTypeEnum degreeType = DegreeTypeEnum.none;
   

        private MidiChordDegreeDescription()
        { }

        private MidiChordDegreeDescription(int degree, DegreeTypeEnum degreeType, int alter)
        {
            this.degree = degree;
            this.degreeType = degreeType;
            this.alter = alter;
        }

        public int Degree
        {
            get
            {
                return degree;
            }
        }

        public int Alter
        {
            get
            {
                return alter;
            }
        }

        public DegreeTypeEnum DegreeType
        {
            get
            {
                return degreeType;
            }
        }

        /// <summary>
        /// For test and logging 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("({0}:{1}({2}))", degree, degreeType, alter); // For instance "5:alter(1)"
        }

        /// <summary>
        /// For real UI 
        /// </summary>
        /// <returns></returns>
        public string ToLocalizedString()
        {
            string functionName = "ToLocalizedString";

            string alterSymbol = "";
            switch (alter)
            {
                case -1: alterSymbol = ResourcesForMusicSynthesis.MidiChordDegreeDescription_flat; break;
                case  0: alterSymbol = ResourcesForMusicSynthesis.MidiChordDegreeDescription_natural ; break;
                case  1: alterSymbol = ResourcesForMusicSynthesis.MidiChordDegreeDescription_sharp; break;
                default:
                    alterSymbol = "";
                    MusicSynthesisLogger.Log(string.Format("{0}.{1}: Unexpected value of alter={2}", className, functionName, alter));
                    break;
            }

            string degreeTypeString = "";
            switch (degreeType)
            {
                case DegreeTypeEnum.add:        degreeTypeString = ResourcesForMusicSynthesis.DegreeTypeEnum_add; break; 
                case DegreeTypeEnum.alter:      degreeTypeString = ResourcesForMusicSynthesis.DegreeTypeEnum_alter; break;
                case DegreeTypeEnum.none:       degreeTypeString = ""; break;
                case DegreeTypeEnum.subtract:   degreeTypeString = ResourcesForMusicSynthesis.DegreeTypeEnum_subtract; break;
                default:
                    degreeTypeString = "";
                    MusicSynthesisLogger.Log(string.Format("{0}.{1}: Unexpected value of degreeType={2}", className, functionName, degreeType));
                    break;
            }

            return string.Format("{0} {1} {2} ", degreeTypeString, alterSymbol, degree); // For instance "add kryds 11
        }



        public static MidiChordDegreeDescription Create(int degree, DegreeTypeEnum DegreeType, int alter)
        {
            return new MidiChordDegreeDescription(degree, DegreeType,alter);

        }
    }
}
