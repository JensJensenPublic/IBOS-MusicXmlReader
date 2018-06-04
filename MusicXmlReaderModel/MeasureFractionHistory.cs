using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{


//    /// <summary>
//    /// For generating information about the position within a measure. Examples:
//    /// "1/4 + 1/6" represents the position of a quarternote plus a triolized halfnote relative to the start of the measure.
//    /// </summary>
//    public class MeasureFractionHistory
//    {
//        private List<MeasureFraction> list;
//        private int length;

//        //public override string ToString()
//        //{
//        //    // This implementation builds ub from a liat of fractions  
//        //    StringBuilder sb = new StringBuilder();
//        //    string conc = "";
//        //    foreach (MeasureFraction measureFraction in this.list.Take(this.length))
//        //    {
//        //        sb.Append(conc + measureFraction.ToString());
//        //        conc = " + ";
//        //    }
//        //    return sb.ToString();            
//        //}


//        public override string ToString()
//        {
//           // This implementation decomposes a single fraction into its values. Should be simplified !! 
//           return  this.list[this.length - 1].ToString();
//        }


//        private List<MeasureFraction> Normalize()
//        {
//#warning ToDo Implement Normalize !!
//            return null;
//        }

//        private MeasureFractionHistory(MeasureFractionHistory existingList, MeasureFraction measureFraction)
//        {
//            if (null == existingList)
//            {
//                this.list = new List<MeasureFraction>();
//            }
//            else
//            {
//                this.list = existingList.list;
//            }

//            if (null != measureFraction)
//            {
//                this.list.Add(measureFraction);
//            }
//            this.length = this.list.Count; // Reuse the existing list, but remember the current number of list elements!
//        }

//        public static MeasureFractionHistory Create(MeasureFractionHistory existingList, MeasureFraction measureFraction)
//        {
//            return new MeasureFractionHistory(existingList, measureFraction);
//        }
//    }
}
