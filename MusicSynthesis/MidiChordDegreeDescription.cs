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
        DegreeTypeEnum degreeType = DegreeTypeEnum.none;

        private MidiChordDegreeDescription()
        { }

        private MidiChordDegreeDescription(int degree, DegreeTypeEnum degreeType)
        {
            this.degree = degree;
            this.degreeType = degreeType;
        }

        public int Degree
        {
            get
            {
                return degree;
            }
        }

        public DegreeTypeEnum DegreeType
        {
            get
            {
                return degreeType;
            }
        }

        public static MidiChordDegreeDescription Create(int degree, DegreeTypeEnum DegreeType)
        {
            return new MidiChordDegreeDescription(degree, DegreeType);

        }
    }
}
