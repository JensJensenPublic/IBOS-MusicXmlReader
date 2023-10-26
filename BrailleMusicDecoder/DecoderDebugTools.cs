using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel; // Namespace, not reference !

namespace BrailleMusicDecoder
{

    public enum DevelopmentOptionEnum {
        None,
        MariaGennemTorneGårFromNOTA,
        NuErJordOgHimmelStille,
        Ulandsvise,
        DenneMorgensMulighed,
        FurElize,
        PianoBluesAndorFoldesJSJ,
        MorningHasBroken,
        GodmorgenLilleLand,
        Nr76ADuSomGirOsLivOgGørOsGlade,
        SangenOmLarsen,
        DetErIdagEtVejr,
        AchtKleinePraeludienUndFugen
    };

    public class DecoderDebugTools
    {

        private DevelopmentOptionEnum developmentOptions = DevelopmentOptionEnum.None;
        public DevelopmentOptionEnum DevelopmentOptions { get { return developmentOptions; } }

        /// <summary>
        /// During development HACKS may be introduced in order to temporarily fix specific problemc:
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DevelopmentOptionEnum GetDevelopmentOptions(string fileName)
        {
            if (fileName.EndsWith(@"Maria_gennem_torne_går.NOTA.txt")) return DevelopmentOptionEnum.MariaGennemTorneGårFromNOTA;  // Missing BrailleMusicStart
            if (fileName.EndsWith(@"nr 539 Nu er jord og himmel stille.txt")) return DevelopmentOptionEnum.NuErJordOgHimmelStille;
            if (fileName.EndsWith(@"nr 189 Ulandsvise.txt")) return DevelopmentOptionEnum.Ulandsvise;
            if (fileName.EndsWith(@"nr 26 Denne morgens mulighed.txt")) return DevelopmentOptionEnum.DenneMorgensMulighed;
            if (fileName.EndsWith(@"001.Beethoven - Für Elise.brf")) return DevelopmentOptionEnum.FurElize;
            if (fileName.EndsWith("Piano Blues.2.For Andor Foldes.JSJ.brf")) return DevelopmentOptionEnum.PianoBluesAndorFoldesJSJ;
            if (fileName.EndsWith("nr. 23 Morning has broken.txt")) return DevelopmentOptionEnum.MorningHasBroken;
            if (fileName.EndsWith("nr. 30 Godmorgen lille land.txt")) return DevelopmentOptionEnum.GodmorgenLilleLand;
            if (fileName.EndsWith("Nr. 76A Du som gir os liv og gør os glade.txt")) return DevelopmentOptionEnum.Nr76ADuSomGirOsLivOgGørOsGlade;
            if (fileName.EndsWith("Nr. 107 Sangen om Larsen.txt")) return DevelopmentOptionEnum.SangenOmLarsen;
            if (fileName.EndsWith("nr 266A 267 Det er i dag et vejr.txt")) return DevelopmentOptionEnum.DetErIdagEtVejr;
            if (fileName.EndsWith("802631 - Acht kleine Praeludien und Fugen.txt")) return DevelopmentOptionEnum.AchtKleinePraeludienUndFugen;
            return DevelopmentOptionEnum.None;
        }


        /// <summary>
        /// Allows easy modification of the Unicode representation of MusicBraille files before the Unicode is applied to the decoder.
        /// This is much easier to control than modifications to the real Music BRaille files !!!
        /// </summary>
        /// <param name="developmentOptons">A coded identification of the type of modification to apply</param>
        /// <param name="brailleFileAsUnicode">The original Unicode contents of the input file</param>
        /// <returns>The (possibly) modified contents of the input file</returns>
        public string ModifyDuringDebug(string brailleFileAsUnicode)
        {
            // NOTE: Modify at highest index first in order not to change the indexes!
            switch (developmentOptions)
            {
                case DevelopmentOptionEnum.PianoBluesAndorFoldesJSJ:
                    // In "Copland - Four Piano Blues.2.For Andor Foldes.JSJ.brf" it is assumed that a double measurebar causes a statechange to state text.
                    // This is not assumed in "Morning has broken" and is neither documented anywhere.
                    // So for now we assume that double messurebar causes no statechange and we add "Toword" to Andor Foldes after measure 28 to make things work there.
                    // DOT56 DOT23 is the ToText symbol, which triggers a transition to the Text state
                    char dot56 = (char)(0x2800 + 48); // DOT56;
                    char dot23 = (char)(0x2800 + 6); // DOT23;
                    string modif = dot56.ToString() + dot23.ToString(); // This is the ToText symbol to insert at the following 2 locations:
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 1905, modif); // Start at highest address !!
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 1872, modif);
                    break;

                case DevelopmentOptionEnum.MorningHasBroken:
                    char barlineChar = (char)0x2800;
                    string barLine = barlineChar.ToString();
                    // Following 3 lines were replaced by the mechansm "OnSectionHeader()" i  MusicXmlBuilder.cs
                    // But this mechanism was later removed again and the lines inserted.
                    // The root problem solved here is that a barline is missing at the end of each section. Might be solved without manipulating Music Braille, later...
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 402, barLine); // Alter Becifring timing 1 in measure 12, Harmony
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 332, barLine); // Alter the 1/4 rest in measure 12, Left Hand
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 250, barLine); // Alter the 1/4 rest in measure 12, Right Hand
                    break;


                case DevelopmentOptionEnum.GodmorgenLilleLand:
                    // Replace the 1/2 stem of Am/c in measure 11 with a 1/4 stem. Note: This is an error in the file from NOTA specifying 1/4 + 1/4 + 1/2 + 1/4
                    int index = 575;
                    // Both stems start with dot456, the difference is coded in the following character:
                    string halfMeasureStem = ToString(new List<int>() { TokenReader.dot13 });
                    string quarterMeasureStem = ToString(new List<int>() { TokenReader.dot1 });
                    Logger.LogCF(string.Format(": Godmorgen Lille land: Replacing at Index={0} (Measure 11, Harmonies) : HalfMeasureStem->QuarterMeasureStem", index));
                    brailleFileAsUnicode = ReplaceContents(index, brailleFileAsUnicode, halfMeasureStem.ToString(), quarterMeasureStem.ToString());

                    // Each part is terminated with a "Repetition End". which is also found int the graphics vesion, but which is not tecnically correct
                    // because the score actually ends after the 4. ending.
                    // A Simple attempt to remove the  "Repetition End" fails because it makes the terminatig G harmony be interpreted as a Ges.
                    // So for the time being we choose to keep the  "Repetition End" which also reduces the amount of fixes at this point !
                    break;

                case DevelopmentOptionEnum.Nr76ADuSomGirOsLivOgGørOsGlade:
                    string existingHarmonyB = ToString(new List<int>() { TokenReader.dot6, TokenReader.dot12 });
                    string replacingHarmonyBb = ToString(new List<int>() { TokenReader.dot6, TokenReader.dot12, TokenReader.dot126 });
                    int index604 = 604;
                    Logger.LogCF(string.Format(": Nr 76A Du som gir os liv og gør os glade: Replacing at Index={0} (Measure 5, Harmonies) : Bmaj->Bbmaj", index604));
                    brailleFileAsUnicode = ReplaceContents(index604, brailleFileAsUnicode, existingHarmonyB, replacingHarmonyBb);
                    break;

                case DevelopmentOptionEnum.SangenOmLarsen:
                    // Add a missing barline after the end of the 1/4 long measure which terminates the first ending.
                    // This causes the addition of an extra measure consisting of the remaining 3¤ of the new measure  + the first 1/4 of measure 1, so it is not perfect!
                    string endrepeat = ToString(new List<int>() { TokenReader.dot126, TokenReader.dot23 });
                    string endRepeatNewMeasure = ToString(new List<int>() { TokenReader.dot126, TokenReader.dot23, TokenReader.noDots });
                    brailleFileAsUnicode = ReplaceContents(951, brailleFileAsUnicode, endrepeat, endRepeatNewMeasure);
                    brailleFileAsUnicode = ReplaceContents(853, brailleFileAsUnicode, endrepeat, endRepeatNewMeasure);
                    brailleFileAsUnicode = ReplaceContents(790, brailleFileAsUnicode, endrepeat, endRepeatNewMeasure);
                    break;

                case DevelopmentOptionEnum.DetErIdagEtVejr:
                    //// Replace an illegal Octobraille code at index 546 with a DOT6 symbol For "Harmony base=C"
                    //// NOTE: This fix was once needed because the Music Braille file contained an error. When using the correct Music Braille file the fix is NOT NEEDED!!!
                    //string illegalAmphersand = "&";
                    //string harmonyBaseC = ToString(new List<int>() { TokenReader.dot6 });
                    //brailleFileAsUnicode = ReplaceContents(546, brailleFileAsUnicode, illegalAmphersand, harmonyBaseC);
                    break;
                case DevelopmentOptionEnum.AchtKleinePraeludienUndFugen:
                    char measureBar = (char)(0x2800 + 00);
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 3080, measureBar.ToString()); // Pedal: measurebar between measure 17 and 18
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2989, measureBar.ToString()); // LeftHand: measurebar between measure 26 and 27
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2918, measureBar.ToString()); // LeftHand: measurebar between measure 21 and 22
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2849, measureBar.ToString()); // LeftHand: measurebar between measure 17 and 18
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2811, measureBar.ToString()); // LeftHand: measurebar between measure 16 and 17 
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2781, measureBar.ToString()); // LeftHand: measurebar between measure 14 and 15
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2610, measureBar.ToString()); // Right: measurebar between measure 24 and 25  
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2574, measureBar.ToString()); // Right: measurebar between measure 22 and 23 Strange grouping!
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2539, measureBar.ToString()); // Right: measurebar between measure 20 and 21 
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2472, measureBar.ToString()); // Right: measurebar between measure 16 and 17 
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2377, measureBar.ToString()); // Pedal: measurebar between measure 10 and 11 
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2343, measureBar.ToString()); // Pedal: measurebar between measure 5 and 6 
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2247, measureBar.ToString()); // LeftHand: measurebar between measure 9 and 10
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2211, measureBar.ToString()); // LeftHand: measurebar between measure 6 and 7
                    brailleFileAsUnicode = Insert(brailleFileAsUnicode, 2038, measureBar.ToString()); // RightHand: measurebar between measure 8 and 9                                                                                 // 
                    break;
                // case... others to come ..

                default: // No modification is needed !
                    break;
            }
            return brailleFileAsUnicode;
        }

        private string ToString(List<int> symbolList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int symbol in symbolList)
            {
                char c = (char)(0x2800 + symbol);
                sb.Append(c);
            }
            return sb.ToString();
        }

        private bool CheckContents(int index, string target, string expectedContents)
        {
            string actualContents = target.Substring(index, expectedContents.Length);
            if (0 == string.Compare(actualContents, expectedContents)) return true;
            string message = string.Format("Index={0} ActualContents={1} differs from ExpectedContents={2} No change!", index, actualContents, expectedContents);
            Logger.LogCF(":" + message);
            return false;
        }

        private string ReplaceContents(int index, string target, string oldContents, string newContents)
        {
            if (CheckContents(index, target, oldContents))
            {
                string message = "";
                UserWarnings.LogUserReplacementWarning(message, target, index, oldContents, Decoder.ToDotNumbers(oldContents), newContents, Decoder.ToDotNumbers(newContents));
                target = target.Remove(index, oldContents.Length);
                target = target.Insert(index, newContents);

            }
            return target;
        }


        private string Insert(string target, int index, string stringToInsert)
        {
            UserWarnings.LogUserInsertionWarning(string.Format("Inserted {0} ", stringToInsert), target, index, stringToInsert, Decoder.ToDotNumbers(stringToInsert));
            return target.Insert(index, stringToInsert);
        }


        //static bool inUse = false;
        static int[] categoryCounters = new int[InputInterpretation.nCategories]; // Count across the whole program lifetime. 
        static int[] subCategoryCounters = new int[InputInterpretation.nSubCategories]; // Count across the whole program lifetime. 
        static int[] subSubCategoryCounters = new int[InputInterpretation.nSubSubCategories]; // Count across the whole program lifetime. 


        /// <summary>
        /// Simple (timeconsuming) mechanism for counting the use of inputcategories through a whole program activation.
        /// </summary>
        /// <param name="category"></param>
        public void CountCategories(InputInterpretation input)
        {
            if (!Logger.DeveloperMode) return;
            categoryCounters[input.CategoryNumber]++;
            subCategoryCounters[(int)input.SubCategory]++;
            subSubCategoryCounters[(int)input.SubSubCategory]++;
        }

        private List<SortItem> GetItems(int[] counters, bool sort)
        {
            List<SortItem> items = new List<SortItem>();
            for (int i = 0; i < counters.Length; i++)
            {
                items.Add(new SortItem(i, counters[i]));
            }
            if (sort)
            {
                items.Sort(SortItem.Compare);
            }
            return items;
        }

        public void DumpCategories(bool sort)
        {
            if (!Logger.DeveloperMode) return;
            Logger.LogCF("+");
            // Dump with largest count first
            List<SortItem> items = GetItems(categoryCounters, sort);
            for (int i = 0; (i < InputInterpretation.nCategories); i++)
            {
                SortItem item = items[i];
                ulong mask = ((ulong)1 << item.Id);
                InputCategoryEnum category = (InputCategoryEnum)mask;
                string name = category.ToString();
                Logger.Log(string.Format("Count ={0,5} for Category=0x{1:X016} {2}", item.Count, mask, name));
            }
            Logger.LogCF("-");
        }

        public void DumpSubCategories(bool sort)
        {
            if (!Logger.DeveloperMode) return;
            Logger.LogCF("+");
            // Dump with largest count first
            List<SortItem> items = GetItems(subCategoryCounters, sort);          
            for (int i = 0; (i < InputInterpretation.nSubCategories); i++)
            {
                SortItem item = items[i];
                InputSubCategoryEnum subCategory = (InputSubCategoryEnum)item.Id;
                string name = subCategory.ToString();
                Logger.Log(string.Format("Count ={0,5} for SubCategory={1,3} {2}", item.Count, item.Id, name));
            }
            Logger.LogCF("-");
        }


        public void DumpSubSubCategories(bool sort)
        {
            if (!Logger.DeveloperMode) return;
            Logger.LogCF("+");
            // Dump with largest count first
            List<SortItem> items = GetItems(subSubCategoryCounters, sort);
            for (int i = 0; (i < InputInterpretation.nSubSubCategories); i++)
            {
                SortItem item = items[i];
                InputSubSubCategoryEnum subSubCategory = (InputSubSubCategoryEnum)item.Id;
                string name = subSubCategory.ToString();
                Logger.Log(string.Format("Count ={0,5} for SubSubCategory={1,3} {2}", item.Count, item.Id, name));
            }
            Logger.LogCF("-");
        }



        private DecoderDebugTools() { }

        private DecoderDebugTools(string fileName)
        {
            this.developmentOptions = this.GetDevelopmentOptions(fileName);
        }

        public static DecoderDebugTools Create(string fileName)
        {
            return new DecoderDebugTools(fileName);
        }
        

        public static DecoderDebugTools Create()
        {
            return new DecoderDebugTools();
        }
    }

    public class SortItem
    {
        private int count;
        public int Count { get { return count; } }
        private int id;
        public int Id { get { return id; } }
        private SortItem(){}
        public SortItem(int id, int count)
        {
            this.id = id;
            this.count = count;
        }

        public static int Compare(SortItem s1, SortItem s2)
        {
            return (int)s2.count - (int)s1.count; // Largest count will be listed first
        }

    }

}
