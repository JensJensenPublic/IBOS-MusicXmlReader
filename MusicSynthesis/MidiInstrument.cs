using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSJ.MusicSynthesis
{
        /// <summary>
        /// Note: These definitions are 1-based. In the Line Protocol they are mapped to 0-based representation!
        /// </summary>
        public enum PitchedMidiInstrumentEnum
        {
            //Piano:
            AcousticGrandPiano = 1,
            BrightAcousticPiano = 2,
            ElectricGrandPiano = 3,
            HonkyTonkPiano = 4,
            ElectricPiano = 5,
            ElectricPiano2 = 6,
            Harpsichord = 7,
            Clavinet = 8,
            // Chromatic Percussion:
            Celesta = 9,
            Glockenspiel = 10,
            MusicBox = 11,
            Vibraphone = 12,
            Marimba = 13,
            Xylophone = 14,
            TubularBells = 15,
            Dulcimer = 16,
            // Organ:
            DrawbarOrgan = 17,
            PercussiveOrgan = 18,
            RockOrgan = 19,
            ChurchOrgan = 20,
            ReedOrgan = 21,
            Accordion = 22,
            Harmonica = 23,
            TangoAccordion = 24,
            // Guitar:
            AcousticGuitarNylon = 25,
            AcousticGuitarSteel = 26,
            ElectricGuitarJazz = 27,
            ElectricGuitarClean = 28,
            ElectricGuitarMuted = 29,
            OverdrivenGuitar = 20,
            DistortionGuitar = 31,
            Guitarharmonics = 32,
            // Bass:
            AcousticBass = 33,
            ElectricBassFinger = 34,
            ElectricBassPick = 35,
            FretlessBass = 36,
            SlapBass1 = 37,
            SlapBass2 = 38,
            SynthBass1 = 39,
            SynthBass2 = 40,
            //Strings:
            Violin = 41,
            Viola = 42,
            Cello = 43,
            Contrabass = 44,
            TremoloStrings = 45,
            PizzicatoStrings = 46,
            OrchestralHarp = 47,
            Timpani = 48,
            //Strings(continued):
            StringEnsemble1 = 49,
            StringEnsemble2 = 50,
            SynthStrings1 = 51,
            SynthStrings2 = 52,
            ChoirAahs = 53,
            VoiceOohs = 54,
            SynthVoice = 55,
            OrchestraHit = 56,
            //Brass:
            Trumpet = 57,
            Trombone = 58,
            Tuba = 59,
            MutedTrumpet = 60,
            FrenchHorn = 61,
            BrassSection = 62,
            SynthBrass1 = 63,
            SynthBrass2 = 64,
            //Reed:
            SopranoSax = 65,
            AltoSax = 66,
            TenorSax = 67,
            BaritoneSax = 68,
            Oboe = 69,
            EnglishHorn = 70,
            Bassoon = 71,
            Clarinet = 72,
            //Pipe:
            Piccolo = 73,
            Flute = 74,
            Recorder = 75,
            PanFlute = 76,
            BlownBottle = 77,
            Shakuhachi = 78,
            Whistle = 79,
            Ocarina = 80,
            //Synth Lead:
            Lead1Square = 81,
            Lead2Sawtooth = 82,
            Lead3Calliope = 83,
            Lead4Chiff = 84,
            Lead5Charang = 85,
            Lead6Voice = 86,
            Lead7Fifths = 87,
            Lead8BassPlusLead = 88,
            //Synth Pad:
            Pad1NewAge = 89,
            Pad2Warm = 90,
            Pad3Polysynth = 91,
            Pad4Choir = 92,
            Pad5Bowed = 93,
            Pad6Metallic = 94,
            Pad7Halo = 95,
            Pad8Sweep = 96,
            //Synth Effects:
            FX1Rain = 97,
            FX2Soundtrack = 98,
            FX3Crystal = 99,
            FX4Atmosphere = 100,
            FX5Brightness = 101,
            FX6Goblins = 102,
            FX7Echoes = 103,
            FX8SciFi = 104,
            //Ethnic:
            Sitar = 105,
            Banjo = 106,
            Shamisen = 107,
            Koto = 108,
            Kalimba = 109,
            BagPipe = 110,
            Fiddle = 111,
            Shanai = 112,
            //Percussive:
            TinkleBell = 113,
            Agogo = 114,
            SteelDrums = 115,
            Woodblock = 116,
            TaikoDrum = 117,
            MelodicTom = 118,
            SynthDrum = 119,
            //Sound effects:
            ReverseCymbal = 120,
            GuitarFretNoise = 121,
            BreathNoise = 122,
            Seashore = 123,
            BirdTweet = 124,
            TelephoneRing = 125,
            Helicopter = 126,
            Applause = 127,
            Gunshot = 128
        }



        // For General MIDI Level 2 see: 
        // https://en.wikipedia.org/wiki/General_MIDI_Level_2#Percussive

        // https://www.midi.org/specifications/item/gm-level-1-sound-set
        // http://www.music.mcgill.ca/~ich/classes/mumt306/StandardMIDIfileformat.html
        // On MIDI Channel 10, each MIDI Note number("Key#") corresponds to a different drum sound, as shown below.
        // GM-compatible instruments must have the sounds on the keys shown here.
        // While many current instruments also have additional sounds above or below the range show here,
        // and may even have additional "kits" with variations of these sounds, only these sounds are supported by General MIDI Level 1 devices.
        // NOTE:
        // According to the example given in http://www.musicxml.com/tutorial/percussion/multiple-instruments/
        // the Instrument number in the MusicXml file is equal to the (Midi Instrument number below) + 1 
        // For instance A Crash Cymbal in is represented by 50 in the MusicXml file and by 49 in the table below.
        public enum UnpitchedMidiInstrumentEnum
        {
            Pitched = 0,
            HighQ = 27,         // GM Level 2
            Slap = 28,          // GM Level 2
            SchratchPush = 29,  // GM Level 2
            ScratchPull = 30,   // GM Level 2
            Sticks = 31,        // GM Level 2
            SquareClick = 32,   // GM Level 2
            MetronomeClick = 33,// GM Level 2
            MetronomeBell = 34, // GM Level 2
            AcousticBassDrum = 35,
            BassDrum1 = 36,
            SideStick = 37,
            AcousticSnare = 38,
            HandClap = 39,
            ElectricSnare = 40,
            LowFloorTom = 41,
            ClosedHiHat = 42,
            HighFloorTom = 43,
            PedalHiHat = 44,
            LowTom = 45,
            OpenHiHat = 46,
            LowMidTom = 47,
            HiMidTom = 48,
            CrashCymbal1 = 49,
            HighTom = 50,
            RideCymbal1 = 51,
            ChineseCymbal = 52,
            RideBell = 53,
            Tambourine = 54,
            SplashCymbal = 55,
            Cowbell = 56,
            CrashCymbal2 = 57,
            Vibraslap = 58,
            RideCymbal2 = 59,
            HiBongo = 60,
            LowBongo = 61,
            MuteHiConga = 62,
            OpenHiConga = 63,
            LowConga = 64,
            HighTimbale = 65,
            LowTimbale = 66,
            HighAgogo = 67,
            LowAgogo = 68,
            Cabasa = 69,
            Maracas = 70,
            ShortWhistle = 71,
            LongWhistle = 72,
            ShortGuiro = 73,
            LongGuiro = 74,
            Claves = 75,
            HiWoodBlock = 76,
            LowWoodBlock = 77,
            MuteCuica = 78,
            OpenCuica = 79,
            MuteTriangle = 80,
            OpenTriangle = 81,
            Shaker = 82,        // GM Level 2
            JingleBell = 83,    // GM Level 2
            BellTree = 84,      // GM Level 2
            Castanets = 85,     // GM Level 2
            MuteSurdo = 86,     // GM Level 2
            OpenSurdo = 87      // GM Level 2
        };

}
