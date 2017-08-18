using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSheetBuilder
{
    class Program
    {
        const string className = "Program";
        const string prologFileName = "prolog.xml";
        const string epilogFileName = "epilog.xml";
        const string testSheetFileName = "TestSheet.xml";

        static string GetPartList(int numberOfParts, int firstInstrument, int lastInstrument)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<part-list>");
            for (int part = 1; (part <= numberOfParts); part++)
            {
                sb.Append(string.Format("<score-part id=\"P{0}\">", part));
                sb.Append(string.Format("<part-name>Part{0}</part-name>", part));
                sb.Append(string.Format("<part-abbreviation>P{0}</part-abbreviation>", part));
                for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
                {
                    sb.Append(string.Format("<score-instrument id=\"P{0}-I{1}\"", part, instrument));
                    sb.Append(string.Format("<instrument-name>Instrument{0}/<instrument-name>", instrument));
                    sb.Append(string.Format("</score-instrument"));
                }

                sb.Append(string.Format("<midi-device port=\"{0}\"/>", 1)); // Use instrument of constant "1" ??

                for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
                {
                    sb.Append(string.Format("<midi-instrument id=\"P{0}-I{1}\"", part, instrument));
                    sb.Append(string.Format("<midi-channel>{0}</midi-channel>", 10));
                    sb.Append(string.Format("<midi-program>{0}</midi-program>", 1));
                    sb.Append(string.Format("<midi-unpitched>{0}</midi-unpitched>", instrument));
                    sb.Append(string.Format("<volume>{0}</volume>", 78.7402));
                    sb.Append(string.Format("<pan>{0}</pan>",0));
                    sb.Append(string.Format("</midi-instrument"));
                }


            }
            return sb.ToString();
        }


        static string GetPart(int part, int firstInstrument, int lastInstrument)
        {
            return ""; // ToDo Implement.
        }



        static void Main(string[] args)
        {
            const string functionName = "Main";
            Console.WriteLine(string.Format("{0}.{1}:Starting",className,functionName));    
            string prolog = System.IO.File.ReadAllText(prologFileName);
            Console.WriteLine(string.Format("{0}.{1}: Read {2} bytes from {3}", className, functionName, prolog.Length, prologFileName));
            string epilog = System.IO.File.ReadAllText(epilogFileName);
            Console.WriteLine(string.Format("{0}.{1}: Read {2} bytes from {3}", className, functionName, epilog.Length, epilogFileName));
  

            string partList = GetPartList(1, 0, 127);
            string part1 = GetPart(1, 0, 127);

            string testSheet = prolog + partList + part1 + epilog;

            System.IO.File.WriteAllText("TestSheet.xml", testSheet);
            Console.WriteLine(string.Format("{0}.{1}: wrote {2} bytes to {3}", className, functionName, testSheet.Length, testSheetFileName));
            Console.WriteLine("Press any key to exit");
            Console.Read();
            Console.WriteLine(string.Format("{0}.{1}:Exiting", className, functionName));
        }
    }
}
