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
            return this.Nominator + "/" + this.Denominator;
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
