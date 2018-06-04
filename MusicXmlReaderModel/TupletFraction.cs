using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{



    /// <summary>
    /// An Integer fraction describing properties of a tuplet, which includes the MusicXml properties Normla and Actual
    /// </summary>
    public class TupletFraction : IntegerFraction
    {

        private Int64 normal;
        public  Int64 Normal { get { return normal; } }
        private Int64 actual;
        public  Int64 Actual { get { return actual; } }
        private Int64 duration;
        public  Int64 Duration { get { return duration; } }
        private string localizedDuration;
        public  string LocalizedDuration{get{ return localizedDuration; } }




        public override string ToString()
        {
            // ToDo add code for sugaring triplets
            if ((this.normal == 2) && (this.actual == 3))
            {
                // This is a simple triplet
                string triplet = ResourcesForModel.TupletElement_Triplet;
                return string.Format(" {0} {1} ",triplet, localizedDuration); // For instance "triplet eight" of "triol ottendedel"
            }

#warning ToDo Sugar other tuplets

            // If we can not sugar this just handle at as a simple Integer Fraction.
            return base.ToString();
        }




        private TupletFraction(Int64 normal, Int64 actual ,Int64 duration,string localizedDuration) : base(normal,actual * duration)
        {
            this.normal = normal;
            this.actual = actual;
            this.duration = duration;
            this.localizedDuration = localizedDuration;
        }

        /// <summary>
        ///  Tupletfraction works as constructor for IntegerFraction, but also saves the values of
        /// actual and normal from the MusicXml TimeModificationElement which may be needed for later functional extensions.
        /// </summary>
        /// <param name="normal">The contents of TimeModificationElement.normal for the related NoteElement</param>
        /// <param name="actual">The contents of TimeModificationElement.normal for the related NoteElement</param>
        /// <param name="duration">The contents of the related NoteElement.Duration converted to an integer: 1 means FullNote, 4 means QuarterNote etc.</param>
        /// <param name="localizedduration">The localized value of the related NoteElement.Duration
        /// <returns></returns>
        public static TupletFraction Create(Int64 normal, Int64 actual, Int64 duration, string localizedDuration)
        {
            TupletFraction result = new TupletFraction(normal, actual, duration, localizedDuration);
            return result;
        }

    }



}
