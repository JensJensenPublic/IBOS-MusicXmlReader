using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSynthesis
{
    public enum DegreeTypeEnum { unknown, add, alter, subtract, none };

    public class MidiChordDegreeDescription
    {
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

        public override string ToString()
        {
            return string.Format("({0}:{1}({2}))", degree, degreeType, alter); // For instance "5:alter(1)"
        }


        public static MidiChordDegreeDescription Create(int degree, DegreeTypeEnum DegreeType, int alter)
        {
            return new MidiChordDegreeDescription(degree, DegreeType,alter);

        }
    }
}
