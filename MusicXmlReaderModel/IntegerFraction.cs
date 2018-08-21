using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{


    /// <summary>
    /// Describes a simple interger fraction
    /// </summary>
    public class IntegerFraction
    {
        protected Int64 nominator;
        public Int64 Nominator { get { return nominator; } }
        protected Int64 denominator;
        public Int64 Denominator { get { return denominator; } }

        public override string ToString()
        {
            if (0 == nominator) return "0";
            //return this.Nominator + "/" + this.Denominator;
            return string.Format("{0} {1}", this.Nominator, DenominatorString); 
        }

#warning ToDo: Localization !!!


        /// <summary>
        /// Unfortunately JAWS does not know how to pronounce fractions in the form "N/M" so it must be implemented here !
        /// </summary>
        private string DenominatorString
        {
            get
            {
                if (nominator != 1)
                {
                    // Use pluralis
                    switch (denominator)
                    {
                        //case  1: return "hele";
                        //case  2: return "halve";
                        //case  3: return "tredjedele";
                        //case  4: return "fjerdedele";
                        //case  5: return "femtedele";
                        //case  6: return "sjettedele"; 
                        //case  8: return "ottendedele";
                        //case 10: return "tiendedele";
                        //case 12: return "tolvtedele";
                        //case 16: return "sekstendedele";
                        //case 18: return "attendelele";
                        //case 24: return "fireogtyvendedele";
                        //case 32: return "toogtredivtedele";
                        //case 64: return "fireogtredsindstyvendedele";
                        //case 128: return "hundredeogotteogtyvendedele";
                        //case 256: return "tohundredeogseksogtredsindstyvendedele";

                        case 1: return ResourcesForModel.IntegerFraction_Denominator_001_P;
                        case 2: return ResourcesForModel.IntegerFraction_Denominator_002_P;
                        case 3: return ResourcesForModel.IntegerFraction_Denominator_003_P;
                        case 4: return ResourcesForModel.IntegerFraction_Denominator_004_P;
                        case 5: return ResourcesForModel.IntegerFraction_Denominator_005_P;
                        case 6: return ResourcesForModel.IntegerFraction_Denominator_006_P;
                        case 8: return ResourcesForModel.IntegerFraction_Denominator_008_P;
                        case 10: return ResourcesForModel.IntegerFraction_Denominator_010_P;
                        case 12: return ResourcesForModel.IntegerFraction_Denominator_012_P;
                        case 16: return ResourcesForModel.IntegerFraction_Denominator_016_P;
                        case 18: return ResourcesForModel.IntegerFraction_Denominator_018_P;
                        case 24: return ResourcesForModel.IntegerFraction_Denominator_024_P;
                        case 32: return ResourcesForModel.IntegerFraction_Denominator_032_P;
                        case 64: return ResourcesForModel.IntegerFraction_Denominator_064_P;
                        case 128: return ResourcesForModel.IntegerFraction_Denominator_128_P;
                        case 256: return ResourcesForModel.IntegerFraction_Denominator_256_P;
                        case 512: return ResourcesForModel.IntegerFraction_Denominator_512_P;
                        default: break;
                    }
                }
                else
                {
                    // Use singularis
                    switch (denominator)
                    {
                        //case 1: return "hel";
                        //case 2: return "halv";
                        //case 3: return "tredjedel";
                        //case 4: return "fjerdedel";
                        //case 5: return "femtedel";
                        //case 6: return "sjettedele";
                        //case 8: return "ottendedel";
                        //case 10: return "tiendedel";
                        //case 12: return "tolvtedel";
                        //case 16: return "sekstendedel";
                        //case 18: return "attendelel";
                        //case 24: return "fireogtyvendedel";
                        //case 32: return "toogtredivtedel";
                        //case 64: return "fireogtredsindstyvendedel";
                        //case 128: return "hundredeogotteogtyvendedel";
                        //case 256: return "tohundredeogseksogtredsindstyvendedel";
                        case 1: return ResourcesForModel.IntegerFraction_Denominator_001_S;
                        case 2: return ResourcesForModel.IntegerFraction_Denominator_002_S;
                        case 3: return ResourcesForModel.IntegerFraction_Denominator_003_S;
                        case 4: return ResourcesForModel.IntegerFraction_Denominator_004_S;
                        case 5: return ResourcesForModel.IntegerFraction_Denominator_005_S;
                        case 6: return ResourcesForModel.IntegerFraction_Denominator_006_S;
                        case 8: return ResourcesForModel.IntegerFraction_Denominator_008_S;
                        case 10: return ResourcesForModel.IntegerFraction_Denominator_010_S;
                        case 12: return ResourcesForModel.IntegerFraction_Denominator_012_S;
                        case 16: return ResourcesForModel.IntegerFraction_Denominator_016_S;
                        case 18: return ResourcesForModel.IntegerFraction_Denominator_018_S;
                        case 24: return ResourcesForModel.IntegerFraction_Denominator_024_S;
                        case 32: return ResourcesForModel.IntegerFraction_Denominator_032_S;
                        case 64: return ResourcesForModel.IntegerFraction_Denominator_064_S;
                        case 128: return ResourcesForModel.IntegerFraction_Denominator_128_S;
                        case 256: return ResourcesForModel.IntegerFraction_Denominator_256_S;
                        case 512: return ResourcesForModel.IntegerFraction_Denominator_512_S;


                        default: break;
                    }
                }

                // Common fallback for singularis and pluralis:
                Logger.LogCF(string.Format(": Using default for {0}/{1}", nominator, denominator));
                return string.Format("{0} {1}", "af", denominator);
            }


        }

        public void Add(IntegerFraction that)
        {
            this.nominator = this.nominator * that.denominator + that.nominator * this.denominator;
            this.denominator = this.denominator * that.denominator;
        }

        public void Normalize()
        {
            List<int> primes = new List<int> { 2, 3, 5, 7, 9, 11, 13, 17 }; // Rhapsody in Blue uses 17 ! 
            Int64 oldNominator = nominator;
            Int64 oldDenominator = denominator;
            foreach (int prime in primes)
            {
                bool continueLoop = true; 
                while (continueLoop)
                {
                    Int64 nominatorDivPrime = nominator / prime;
                    Int64 nominatorRemPrime = nominator % prime;
                    Int64 denominatorDivPrime = denominator / prime;
                    Int64 denominatorRemPrime = denominator % prime;
                    if ((0 == nominatorRemPrime) & (0 == denominatorRemPrime))
                    {
                        nominator = nominatorDivPrime;
                        denominator = denominatorDivPrime;                   
                    }
                    else
                    {
                        continueLoop = false;
                    }
                }
            }
            // Logger.LogCF(string.Format(": {0}/{1}->{2}/{3}", oldNominator, oldDenominator, nominator, denominator));
        }

        public bool Equals(IntegerFraction that)
        {
            return this.nominator * that.denominator == this.denominator * that.nominator;
        }

        public IntegerFraction()
        {
            this.nominator = 0;
            this.denominator = 1;
        }

        public IntegerFraction(Int64 nominator, Int64 denominator)
        {
            this.nominator = nominator;
            this.denominator = denominator;
        }
    }

}
