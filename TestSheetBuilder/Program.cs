using System;
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
                    sb.Append(string.Format("<score-instrument id=\"P{0}-I{1}\">", part, instrument));
                    sb.Append(string.Format("<instrument-name>Instrument{0}</instrument-name>", instrument));
                    sb.Append(string.Format("</score-instrument>"));
                }

                sb.Append(string.Format("<midi-device port=\"{0}\"/>", 1)); // Use instrument of constant "1" ??

                for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
                {
                    sb.Append(string.Format("<midi-instrument id=\"P{0}-I{1}\">", part, instrument));
                    sb.Append(string.Format("<midi-channel>{0}</midi-channel>", 10));
                    sb.Append(string.Format("<midi-program>{0}</midi-program>", 1));
                    sb.Append(string.Format("<midi-unpitched>{0}</midi-unpitched>", instrument));
                    sb.Append(string.Format("<volume>{0}</volume>", 78));
                    sb.Append(string.Format("<pan>{0}</pan>",0));
                    sb.Append(string.Format("</midi-instrument>"));
                }

                sb.Append(string.Format("</score-part>"));
            }
            sb.Append("</part-list>");
            return sb.ToString();
        }


        static string GetPart(int part, int firstInstrument, int lastInstrument)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<part id=\"P{0}\">",part));
            sb.Append(string.Format("<measure number=\"{0}\">", 1)); // Keep everything in one measure to keep things simple !

            sb.Append(string.Format("<attributes>"));
            sb.Append(string.Format("<divisions>{0}</divisions>",32)); // A quarter note is devided into 32 divisions, each an 128th

            sb.Append(string.Format("<time>"));
            sb.Append(string.Format("<beats>{0}</beats>", 4));
            sb.Append(string.Format("<beat-type>{0}</beat-type>", 4));
            sb.Append(string.Format("</time>"));

            sb.Append(string.Format("</attributes>"));

            // Generate one note for each instrument
            for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
            {
                sb.Append(string.Format("<note>"));
                sb.Append(string.Format("<unpitched>"));
                sb.Append(string.Format("<display-step>F</display-step>"));
                sb.Append(string.Format("<display-octave>4</display-octave>"));
                sb.Append(string.Format("</unpitched>"));
                sb.Append(string.Format("<duration>{0}</duration>", 1)); // In this way we can keep everything inside one measure!
                sb.Append(string.Format("<instrument id=\"P{0}-I{1}\"/>", part,instrument));
                sb.Append(string.Format("<voice>{0}</voice>", 1));
                sb.Append(string.Format("<stem>{0}</stem>", "up"));
                sb.Append(string.Format("</note>"));
            }

            sb.Append(string.Format("</measure>"));

            sb.Append(string.Format("</part>"));
            return sb.ToString();
        }



        static void Main(string[] args)
        {
            const string functionName = "Main";
            Console.WriteLine(string.Format("{0}.{1}:Starting", className, functionName));
            // The MusicXml file must be found in the execution directory.
            string prologFullFilename = Path.Combine(System.Environment.CurrentDirectory, prologFileName);
            if (!File.Exists(prologFullFilename))
            {
                Console.WriteLine(string.Format("{0}.{1}: {2} not found. Exiting", className, functionName, prologFullFilename));
            }
            else
            {

                string prolog = System.IO.File.ReadAllText(prologFullFilename);
                Console.WriteLine(string.Format("{0}.{1}: Read {2} bytes from {3}", className, functionName, prolog.Length, prologFileName));

                int firstInstrument = 1;
                int lastInstrument = 127;
//                int firstInstrument = 35;
//                int lastInstrument = 81;

                string partList = GetPartList(1, firstInstrument, lastInstrument);
                string part1 = GetPart(1, firstInstrument, lastInstrument);

                // Insert the autogenerated partlist and parts just before the closing text  "</score-partwise>
                string closingText = "</score-partwise>";
                int position = prolog.IndexOf(closingText);
                prolog = prolog.Substring(0, position);

                //string testSheet = prolog + partList + part1 + closingText;
                string testSheet = prolog + partList + part1 + closingText;

                System.IO.File.WriteAllText("TestSheet.xml", testSheet);
                Console.WriteLine(string.Format("{0}.{1}: wrote {2} bytes to {3}", className, functionName, testSheet.Length, testSheetFileName));
            }
            Console.WriteLine("Press any key to exit");
            Console.Read(); 
            Console.WriteLine(string.Format("{0}.{1}:Exiting", className, functionName));
        }
    }
}
