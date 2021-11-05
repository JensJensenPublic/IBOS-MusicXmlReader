using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MusicXmlReaderModel;


namespace MusicBrailleDecoderTest
{
    public class MusicBrailleTestFiles
    {
        public enum FileSourceEnum
        {
            NOTA,
            BrailleOrch, // More than 300 files are available from http://www.brailleorch.org/en/
            LarsPetersen,
            ManipulatedByJSJ,
            NoteFromSN20210110,
            NotesFromSN20210323,
        };


        public enum TestFileEnum
        {
            Unknown,
            DetErForår,
            PåskeblomstHvadVilDuHer,
            HilDigFrelserOgForsoner,
            DenKedsomVinterGikSinGang,
            DetErIdagEtvejr,
            UlandsVise,
            DenneMorgensMulighed,
            FourPianoBluesLeoSmith,
            FourPianoBluesAndorFoldes,
            NuErJordOgHimmelStille,
            FürElize,
            DuSomGirOsLivOgGørOsGlade,
            WhatAWonderfulWorld, // Problem at start of section TODO: Solve
            MorningHasBroken,  // Problem at start of section TODO Solve
            TearsInHeaven,
            GodMorgenLilleLand,
            LysetSpringerPludsligUd,
            OpAlDenTing,
            AtSigeVerdenRetFarvel,
            LetItBe,
            OmLidtBlirHerStille,
            DenBlaaAnemone,
            SangenOmLarsen
     
        }

        //********************************************************************************
        // Select file source and file name here
        //********************************************************************************
        public void SelectTestFile(TestFileEnum testFile,out string shortFileName, out FileSourceEnum fileSourceEnum)
        {
            switch (testFile)
            {
                case TestFileEnum.UlandsVise: fileSourceEnum = FileSourceEnum.NOTA; shortFileName = "nr 189 Ulandsvise.txt"; return;
                case TestFileEnum.DenneMorgensMulighed: fileSourceEnum = FileSourceEnum.NOTA; shortFileName = "nr 26 Denne morgens mulighed.txt"; return;
                case TestFileEnum.NuErJordOgHimmelStille: fileSourceEnum = FileSourceEnum.NOTA; shortFileName = "nr 539 Nu er jord og himmel stille.txt"; return;
                case TestFileEnum.FourPianoBluesLeoSmith: fileSourceEnum = FileSourceEnum.BrailleOrch; shortFileName = "BrailleOrch Copland - Four Piano Blues.1.For Leo Smith.brf"; return;// Only the first of the 4 blues
                case TestFileEnum.FürElize: fileSourceEnum = FileSourceEnum.BrailleOrch; shortFileName = "BrailleOrch 001.Beethoven - Für Elise.brf"; return;
                case TestFileEnum.FourPianoBluesAndorFoldes: fileSourceEnum = FileSourceEnum.ManipulatedByJSJ; shortFileName = "BrailleOrch Copland - Four Piano Blues.2.For Andor Foldes.JSJ.brf"; return; // The second of the 4 bluses saved in Git and manipulated!
                                                                                                                                                                                                //                case TestFileEnum.MariaGennemTorneGår: fileSourceEnum = FileSourceEnum.LarsPetersen; shortFileName = "Maria gennem torne ga.P1.S.brl"; break;
                case TestFileEnum.DuSomGirOsLivOgGørOsGlade: fileSourceEnum = FileSourceEnum.NoteFromSN20210110;  shortFileName = "Nr. 76A Du som gir os liv og gør os glade.txt"; return;
                case TestFileEnum.MorningHasBroken: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "nr. 23 Morning has broken.txt"; return;
                case TestFileEnum.WhatAWonderfulWorld: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 127 What a wonderful world.txt"; return;
                case TestFileEnum.TearsInHeaven: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 144 Tears in heaven.txt"; return;

                case TestFileEnum.GodMorgenLilleLand: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "nr. 30 Godmorgen lille land.txt"; return;
                case TestFileEnum.LysetSpringerPludsligUd: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 33 Lyset springer pludslig ud.txt"; return;
                case TestFileEnum.OpAlDenTing: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 47 op al den ting som gud har gjort.txt"; return;
                case TestFileEnum.AtSigeVerdenRetFarvel: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 58A At sige verden ret farvel.txt" ; return;
                case TestFileEnum.LetItBe: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 121 Let it be when i find myself in times of trouble.txt"; return;
                case TestFileEnum.OmLidtBlirHerStille: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 138 Om lidt blir her stille.txt";  return;
                case TestFileEnum.DenBlaaAnemone: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 262 Den blå anemone.txt"; return;
                case TestFileEnum.SangenOmLarsen: fileSourceEnum = FileSourceEnum.NoteFromSN20210110; shortFileName = "Nr. 107 Sangen om Larsen.txt"; return;
                case TestFileEnum.DetErIdagEtvejr: fileSourceEnum = FileSourceEnum.NotesFromSN20210323; shortFileName = "nr 266A 267 Det er i dag et vejr.txt"; return;
                case TestFileEnum.DenKedsomVinterGikSinGang: fileSourceEnum = FileSourceEnum.NotesFromSN20210323; shortFileName = "nr 269 Aria den kedsom vinter gik sin gang.txt"; return;
                case TestFileEnum.PåskeblomstHvadVilDuHer: fileSourceEnum = FileSourceEnum.NotesFromSN20210323; shortFileName = "nr 274 Påskeblomst hvad vil du her.txt"; return;
                case TestFileEnum.HilDigFrelserOgForsoner: fileSourceEnum = FileSourceEnum.NotesFromSN20210323; shortFileName = "Nr 279A Hil dig frelser og forsoner.txt"; return;
                 case TestFileEnum.DetErForår : fileSourceEnum = FileSourceEnum.NotesFromSN20210323; shortFileName = "nr 285 Det er forår.txt";return;
                default: throw new Exception(string.Format("Unsupported testfile={0}", testFile));
            }
        }


        // Simple convenience metod
        public string GetFullFileName(FileSourceEnum fileSourceEnum, string shortFileName, ref BrailleFileHandler.FileEncoding fileEncoding, ref DecoderOptions.RegionalOptionsEnum decoderRegionalOptionsEnum)
        {
            string baseDirectory = null;
            string userName = System.Environment.UserName;
            string dropboxBase = Path.Combine(@"C:\Users", userName);
            string dropboxDir = Path.Combine(dropboxBase, "Dropbox"); // The Dropbox directory for the current user on the current PC
            string dropBoxRoot = Path.Combine(dropboxDir, "Root"); // Owned by G75Z, shared by IMB and JJP
            decoderRegionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.AutoSelect;
            switch (fileSourceEnum)
            {
                case FileSourceEnum.NOTA:
                    fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252;
                    baseDirectory = Path.Combine(dropBoxRoot, @"MusicXml sample file archive\Susanne Nolsø\Modtaget 2020.05.17");
                    decoderRegionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.Danish;
                    break;                    
                case FileSourceEnum.NoteFromSN20210110:
                    fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252; 
                     baseDirectory = Path.Combine(dropboxDir, @"Visual Studio 2015\Solutions\Tactile MusicXmlReader\BrailleMusicDecoder\Testfiles for Decoder\NOTA fra SN 2021.01.10");
                    decoderRegionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.Danish;
                    break;
                case FileSourceEnum.NotesFromSN20210323:
                    fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252;
                    baseDirectory = Path.Combine(dropboxDir, @"Visual Studio 2015\Solutions\Tactile MusicXmlReader\BrailleMusicDecoder\Testfiles for Decoder\NOTA fra SN 2021.03.23");
                    decoderRegionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.Danish;
                    break;
                case FileSourceEnum.BrailleOrch:
                    fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII_Ex;
                    baseDirectory = Path.Combine(dropboxDir, @"Visual Studio 2015\Solutions\Tactile MusicXmlReader\BrailleMusicDecoder\Testfiles for Decoder\BrailleOrchOrg");
                    decoderRegionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.English;
                    break;
                case FileSourceEnum.LarsPetersen:
                    fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252;
                    baseDirectory = Path.Combine(dropBoxRoot, @"Music Braille sample file archive\LarsPetersen");
                    decoderRegionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.Danish;
                    break;
                case FileSourceEnum.ManipulatedByJSJ:
                    fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII_Ex;
                    baseDirectory = Path.Combine(dropboxDir, @"Visual Studio 2015\Solutions\Tactile MusicXmlReader\BrailleMusicDecoder\Testfiles for Decoder\BrailleOrchOrg");
                    decoderRegionalOptionsEnum = DecoderOptions.RegionalOptionsEnum.English;
                    break;
                default:
                    return null;
            }
            string result = Path.Combine(baseDirectory, shortFileName);
            return result;
        }

        private MusicBrailleTestFiles()
        { }

        public static MusicBrailleTestFiles Create()
        {
            return new MusicBrailleTestFiles();
        }
    }
}
