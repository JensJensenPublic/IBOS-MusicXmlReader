using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using JSJ.MusicSynthesis;
using NAudio.Midi;

namespace NAudioDemo
{
    class Program
    {

        //private static WaveOut waveOut;


        //public abstract class WaveProvider32 : IWaveProvider
        //{
        //    private WaveFormat waveFormat;

        //    public WaveProvider32()
        //        : this(44100, 1)
        //    {
        //    }

        //    public WaveProvider32(int sampleRate, int channels)
        //    {
        //        SetWaveFormat(sampleRate, channels);
        //    }

        //    public void SetWaveFormat(int sampleRate, int channels)
        //    {
        //        this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        //    }

        //    public int Read(byte[] buffer, int offset, int count)
        //    {
        //        WaveBuffer waveBuffer = new WaveBuffer(buffer);
        //        int samplesRequired = count / 4;
        //        int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
        //        return samplesRead * 4;
        //    }

        //    public abstract int Read(float[] buffer, int offset, int sampleCount);

        //    public WaveFormat WaveFormat
        //    {
        //        get { return waveFormat; }
        //    }



        //    public class SineWaveProvider32 : WaveProvider32
        //    {
        //        int sample;

        //        public SineWaveProvider32()
        //        {
        //            Frequency = 1000;
        //            Amplitude = 0.25f; // let's not hurt our ears            
        //        }

        //        public float Frequency { get; set; }
        //        public float Amplitude { get; set; }

        //        public override int Read(float[] buffer, int offset, int sampleCount)
        //        {
        //            int sampleRate = WaveFormat.SampleRate;
        //            for (int n = 0; n < sampleCount; n++)
        //            {
        //                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
        //                sample++;
        //                if (sample >= sampleRate) sample = 0;
        //            }
        //            return sampleCount;
        //        }
        //    }


        //    private void button1_Click(object sender, EventArgs e)
        //    {
        //        StartStopSineWave();
        //    }

        //    private static void StartStopSineWave()
        //    {
        //        if (waveOut == null)
        //        {
        //            var sineWaveProvider = new SineWaveProvider32();
        //            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
        //            sineWaveProvider.Frequency = 1000;
        //            sineWaveProvider.Amplitude = 0.25f;
        //            waveOut = new WaveOut();
        //            waveOut.Init(sineWaveProvider);
        //            waveOut.Play();                   
        //        }
        //        else
        //        {
        //            waveOut.Stop();
        //            waveOut.Dispose();
        //            waveOut = null;
        //        }
        //    }


        static private void GetMidiInfo()
        {
            int n = NAudio.Midi.MidiOut.NumberOfDevices;
            for (int i = 0; (i < n); i++)
            {
                NAudio.Midi.MidiOutCapabilities oc = NAudio.Midi.MidiOut.DeviceInfo(i);
                string f = " {0} {1}";
                System.Console.WriteLine(string.Format("Midi device number {0}", i));
                System.Console.WriteLine(string.Format(f, "Manufacturer", oc.Manufacturer));
                System.Console.WriteLine(string.Format(f, "Notes", oc.Notes));
                System.Console.WriteLine(string.Format(f, "ProductId", oc.ProductId));
                System.Console.WriteLine(string.Format(f, "ProductName", oc.ProductName));
                System.Console.WriteLine(string.Format(f, "SupportsAllChannels", oc.SupportsAllChannels));
                System.Console.WriteLine(string.Format(f, "SupportsMidiStreamOut", oc.SupportsMidiStreamOut));
                System.Console.WriteLine(string.Format(f, "SupportsPatchCaching", oc.SupportsPatchCaching));
                System.Console.WriteLine(string.Format(f, "SupportsSeparateLeftAndRightVolume", oc.SupportsSeparateLeftAndRightVolume));
                System.Console.WriteLine(string.Format(f, "SupportsVolumeControl", oc.SupportsVolumeControl));
                System.Console.WriteLine(string.Format(f, "Technology", oc.Technology));
                System.Console.WriteLine(string.Format(f, "Voices", oc.Voices));
            }
        }

        //static private MidiNote StartNote(Step step, byte octave, byte velocity)
        //{
        //    return new MidiNote(step, octave, velocity, midiOut);
        //    //MidiNote midiNote = new MidiNote(step, octave, velocity, midiOut);
        //    //midiNote.StartPlaying(midiOut);
        //    //return midiNote;
        //}

        //static private void StopNote(MidiNote midiNote)
        //{
        //    midiNote.StopPlaying(midiOut);
        //}

        static MidiOut midiOut;

        static void GetInstrumentNames()
        {
            Console.WriteLine("Instruments");
            for (int i = 0; (i < 128); i++)
            {
                string instrumentName = PatchChangeEvent.GetPatchName(i);
                Console.WriteLine(string.Format(" {0} : {1}", i, instrumentName));
            }
        }



        static void Main(string[] args)
        {
            GetMidiInfo();
            GetInstrumentNames();

            midiOut = new NAudio.Midi.MidiOut(0);
            //NotePlayer notePlayer = new NotePlayer(midiOut);

            MidiCommand midiCommand = new MidiCommand();
            midiCommand.ChangeInstrument(1,1,midiOut);

            // Start by playing an unaltered C in octave 4
            //MidiNote midiNote = StartNote(ChromaticStep.C,0,4, 0x7f);
            //MidiNote midiNote = new MidiNote("C","", "4", 0x7f,midiOut);
            MidiNote midiNote = new MidiNote("C",0,4, 0x7f,midiOut);
            System.Threading.Thread.Sleep(1000);
            midiNote.StopPlaying(midiOut);
            //StopNote(midiNote);
            System.Threading.Thread.Sleep(1000);

            //// Iterate over all Instruments
            //for (int i = 0; (i < 128); i++)
            //{
            //    midiCommand.ChangeInstrument(i, midiOut);
            //    Console.WriteLine("Instrument {0} = {1}",i, PatchChangeEvent.GetPatchName(i));
            //    MidiNote midiNote1 = new MidiNote(Step.C, 4, 0x7f, midiOut);
            //    System.Threading.Thread.Sleep(1000);
            //    Console.WriteLine("Pause");
            //    midiNote1.StopPlaying(midiOut); 
            //    System.Threading.Thread.Sleep(1000);
            //}

            // Back to Instrument Acoustic piano
            midiCommand.ChangeInstrument(1,1,midiOut);

            // Iterate over all chord types
            foreach (ChordType chordType in Enum.GetValues(typeof(ChordType)))
            {
                Console.WriteLine(chordType.ToString());
                MidiChord midiChord = new MidiChord(ChromaticStep.C, 4, 0x7F, chordType);
                midiChord.StartPlaying(midiOut);
                //notePlayer.StartChord(Step.C, 4, 0x7F, chordType);
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Pause");
                midiChord.StopPlaying(midiOut);
                System.Threading.Thread.Sleep(1000);
            }
            System.Array noteValues = Enum.GetValues(typeof(ChromaticStep));
            //Iterate over all base notes
            foreach (ChromaticStep step in Enum.GetValues(typeof(ChromaticStep)))
            {
                Console.WriteLine(step.ToString());
                MidiChord midiChord = new MidiChord(step, 4, 0x7F, ChordType.Major);
                midiChord.StartPlaying(midiOut);
                //notePlayer.StartChord(step, 4, 0x7F,ChordType.Major);
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Pause");
                midiChord.StopPlaying(midiOut);
                System.Threading.Thread.Sleep(1000);
            }

            // Iterate over all octaves
            for (int octave = -1; (octave < 10); octave++)
            {
                Console.WriteLine(octave.ToString());
                MidiChord midiChord = new MidiChord(ChromaticStep.C, (byte)octave, 0x7F,ChordType.Major);
                midiChord.StartPlaying(midiOut);
                //notePlayer.StartChord(Step.C, (byte) octave, 0x7F, ChordType.Major);
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Pause");
                midiChord.StopPlaying(midiOut);
                System.Threading.Thread.Sleep(1000);
            }



            //StartStopSineWave();
            //StartStopSineWave();
        }
    }
}
