using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;

namespace TestSheetBuilder
{
    class Program
    {
        const string className = "Program";
        const string prologFileName = "prolog.xml";
        const string epilogFileName = "epilog.xml";
        const string testSheetFileName = "TestSheet.xml";
        static List<string> staticChords;
        static List<string> staticBasses;


        static List<string> InitChords()
        {
            List<string> chords = new List<string>() { };
            chords.Add("major");
            chords.Add("minor");
            chords.Add("augmented");
            chords.Add("diminished");
            chords.Add("dominant");
            chords.Add("major-seventh");
            chords.Add("minor-seventh");
            chords.Add("diminished-seventh");
            chords.Add("augmented-seventh");
            chords.Add("half-diminished");
            chords.Add("major-minor");
            chords.Add("major-sixth");
            chords.Add("minor-sixth");
            chords.Add("dominant-ninth");
            chords.Add("major-ninth");
            chords.Add("dominant-11th");
            chords.Add("major-11th");
            chords.Add("minor-11th");
            chords.Add("dominant-13th");
            chords.Add("major-13th");
            chords.Add("minor-13th");
            chords.Add("suspended-second");
            chords.Add("suspended-fourth");
            return chords;
        }


        static List<string> InitBasses()
        {
            List<string> basses = new List<string>() { };
            basses.Add(""); // No Bass Note
            basses.Add("A");
            basses.Add("B");
            basses.Add("C");
            basses.Add("D");
            basses.Add("E");
            basses.Add("F");
            basses.Add("G");
            return basses;
        }


            static string SimpleElement(string name, string value)
        {
            return string.Format("<{0}>{1}</{0}>\n", name, value);
        }

        static string SimpleElement(string name, int value)
        {
            return SimpleElement(name, value.ToString());
        }

        static string GetPartList(int numberOfParts, int firstInstrument, int lastInstrument)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<part-list>\n");
            for (int part = 1; (part <= numberOfParts); part++)
            {
                sb.Append(string.Format("<score-part id=\"P{0}\">\n", part));
                sb.Append(SimpleElement("part-name", string.Format("Part{0}", part)));
                sb.Append(SimpleElement("part-abbreviation", string.Format("P{0}", part)));

                for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
                {
                    sb.Append(string.Format("<score-instrument id=\"P{0}-I{1}\">\n", part, instrument));
                    // sb.Append(string.Format("<instrument-name>Instrument{0}</instrument-name>\n", instrument));
                    sb.Append(SimpleElement("instrument-name", string.Format("Instrument{0}", instrument)));
                    sb.Append(string.Format("</score-instrument>\n"));
                }

                sb.Append(string.Format("<midi-device port=\"{0}\"/>\n", 1)); // Use instrument of constant "1" ??

                for (int instrument = firstInstrument; (instrument <= lastInstrument); instrument++)
                {
                    sb.Append(string.Format("<midi-instrument id=\"P{0}-I{1}\">\n", part, instrument));
                    sb.Append(SimpleElement("midi-channel", 10));
                    sb.Append(SimpleElement("midi-program", 1));
                    sb.Append(SimpleElement("midi-unpitched", instrument));
                    sb.Append(SimpleElement("volume", 78));
                    sb.Append(SimpleElement("pan", 0));
                    sb.Append(string.Format("</midi-instrument>\n"));
                }

                sb.Append(string.Format("</score-part>\n"));
            }
            sb.Append("</part-list>\n");
            return sb.ToString();
        }


        /// <summary>
        /// Generate all 23*8 combinations of kind and bass for root=C
        /// 23 values of kind
        /// 8  values of root (Including no root)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        static string Harmony(int index)
        {
            int iKind = index % staticChords.Count; // Vary kind first
            int iBass = (index / staticChords.Count) % staticBasses.Count ; // Vary bass next
            string kind = staticChords[iKind];
            string bass = staticBasses[iBass];
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<harmony>\n"));
            sb.Append(string.Format("<root>\n"));
            sb.Append(SimpleElement("root-step", "C"));
            sb.Append(string.Format("</root>\n"));

            sb.Append(SimpleElement("kind", kind));

            if (!string.IsNullOrEmpty(bass))
            {
                sb.Append(string.Format("<bass>\n"));
                sb.Append(SimpleElement("bass-step", bass));
                sb.Append(string.Format("</bass>\n"));
            }

            sb.Append(SimpleElement("staff", 1));
            sb.Append(string.Format("</harmony>\n"));
            return sb.ToString();
        }


        static string GetPart(int part, int firstIndex, int lastIndex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<part id=\"P{0}\">\n",part));

            // Generate one note for each instrument
            int measure = 1;
            bool addAttributes = true;
            for (int index = firstIndex; (index <= lastIndex); index++)
            {
                int instrument = 1 + (index % 128);
                sb.Append(string.Format("<measure number=\"{0}\">\n", measure++)); // Keep each note in one measure to keep things simple !
                if (addAttributes)
                {
                    sb.Append(string.Format("<attributes>\n"));
        
                    sb.Append(SimpleElement("divisions",32));

                    sb.Append(string.Format("<time>\n"));             
                    sb.Append(SimpleElement("beats", 4));           
                    sb.Append(SimpleElement("beat-type", 4));
                    sb.Append(string.Format("</time>\n"));

                    sb.Append(string.Format("<clef>\n"));    
                    sb.Append(SimpleElement("sign", "percussion"));   
                    sb.Append(SimpleElement("line", 2));
                    sb.Append(string.Format("</clef>\n"));

                    sb.Append(string.Format("</attributes>\n"));
                    addAttributes = false;
                }

                sb.Append(string.Format("<note>\n"));

                sb.Append(string.Format("<unpitched>\n"));
                sb.Append(SimpleElement("display-step", "F"));
                sb.Append(SimpleElement("display-octave", "4"));
                sb.Append(string.Format("</unpitched>\n"));

                sb.Append(SimpleElement("duration", 128));
                sb.Append(string.Format("<instrument id=\"P{0}-I{1}\"/>\n", part,instrument));
                sb.Append(SimpleElement("voice", 1));
                sb.Append(SimpleElement("type", "whole"));

                sb.Append(SimpleElement("stem", "up"));

                sb.Append(string.Format("<lyric>\n"));
                string s = ((UnpitchedMidiInstrumentEnum)(instrument - 1)).ToString();
                sb.Append(string.Format("<text>MusicXml={0} Midi={1}({2})</text>\n",instrument,instrument-1, s)); // Show the instrument number as text
                sb.Append(string.Format("</lyric>\n"));

                sb.Append(string.Format("</note>\n"));

                sb.Append(Harmony(index)); // Hack: Use instrument to vary Harmony kind !



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
            staticChords = InitChords();
            staticBasses = InitBasses();
            if (!File.Exists(prologFullFilename))
            {
                Console.WriteLine(string.Format("{0}.{1}: {2} not found. Exiting", className, functionName, prologFullFilename));
            }
            else
            {

                string prolog = System.IO.File.ReadAllText(prologFullFilename);
                Console.WriteLine(string.Format("{0}.{1}: Read {2} bytes from {3}", className, functionName, prolog.Length, prologFileName));

                int firstInstrument = 1;
                int lastInstrument = 128;
                //                int firstInstrument = 35;
                //                int lastInstrument = 81;

                string partList = GetPartList(1, firstInstrument, lastInstrument);
                string part1 = GetPart(1, 0, 23*8 -1);

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
