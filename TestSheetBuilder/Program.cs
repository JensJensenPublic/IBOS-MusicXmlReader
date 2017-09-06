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

            sb.Append("<part-list>\n");
            for (int part = 1; (part <= numberOfParts); part++)
            {
                sb.Append(string.Format("<score-part id=\"P{0}\">\n", part));
                sb.Append(string.Format("<part-name>Part{0}</part-name>\n", part));
                sb.Append(string.Format("<part-abbreviation>P{0}</part-abbreviation>\n", part));

                for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
                {
                    sb.Append(string.Format("<score-instrument id=\"P{0}-I{1}\">\n", part, instrument));
                    sb.Append(string.Format("<instrument-name>Instrument{0}</instrument-name>\n", instrument));
                    sb.Append(string.Format("</score-instrument>\n"));
                }

                sb.Append(string.Format("<midi-device port=\"{0}\"/>\n", 1)); // Use instrument of constant "1" ??

                for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
                {
                    sb.Append(string.Format("<midi-instrument id=\"P{0}-I{1}\">\n", part, instrument));
                    sb.Append(string.Format("<midi-channel>{0}</midi-channel>\n", 10));
                    sb.Append(string.Format("<midi-program>{0}</midi-program>\n", 1));
                    sb.Append(string.Format("<midi-unpitched>{0}</midi-unpitched>\n", instrument));
                    sb.Append(string.Format("<volume>{0}</volume>\n", 78));
                    sb.Append(string.Format("<pan>{0}</pan>\n",0));
                    sb.Append(string.Format("</midi-instrument>\n"));
                }

                sb.Append(string.Format("</score-part>\n"));
            }
            sb.Append("</part-list>\n");
            return sb.ToString();
        }


        static string GetPart(int part, int firstInstrument, int lastInstrument)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<part id=\"P{0}\">\n",part));

            // Generate one note for each instrument
            int measure = 1;
            bool addAttributes = true;
            for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
            {
                sb.Append(string.Format("<measure number=\"{0}\">\n", measure++)); // Keep each note in one measure to keep things simple !
                if (addAttributes)
                {
                    sb.Append(string.Format("<attributes>\n"));
                    sb.Append(string.Format("<divisions>{0}</divisions>\n", 32)); // A quarter note is devided into 32 divisions, each an 128th

                    sb.Append(string.Format("<time>\n"));
                    sb.Append(string.Format("<beats>{0}</beats>\n", 4));
                    sb.Append(string.Format("<beat-type>{0}</beat-type>\n", 4));
                    sb.Append(string.Format("</time>\n"));

                    sb.Append(string.Format("<clef>\n"));
                    sb.Append(string.Format("<sign>{0}</sign>\n", "percussion"));
                    sb.Append(string.Format("<line>{0}</line>\n", 2));
                    sb.Append(string.Format("</clef>\n"));

                    sb.Append(string.Format("</attributes>\n"));
                    addAttributes = false;
                }

                sb.Append(string.Format("<note>\n"));

                sb.Append(string.Format("<unpitched>\n"));
                sb.Append(string.Format("<display-step>F</display-step>\n"));
                sb.Append(string.Format("<display-octave>4</display-octave>\n"));
                sb.Append(string.Format("</unpitched>\n"));

                sb.Append(string.Format("<duration>{0}</duration>\n", 128)); 
                sb.Append(string.Format("<instrument id=\"P{0}-I{1}\"/>\n", part,instrument));
                sb.Append(string.Format("<voice>{0}</voice>\n", 1));
                sb.Append(string.Format("<type>{0}</type>\n","whole"));

                sb.Append(string.Format("<stem>{0}</stem>\n", "up"));

                sb.Append(string.Format("<lyric>\n"));
                sb.Append(string.Format("<text>{0}</text>\n",instrument)); // Show the instrument number as text
                sb.Append(string.Format("</lyric>\n"));

                sb.Append(string.Format("</note>\n"));

                sb.Append(string.Format("</measure>\n"));
            }

            sb.Append(string.Format("</part>\n"));
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
