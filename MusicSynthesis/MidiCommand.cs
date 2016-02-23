using NAudio.Midi;

namespace JSJ.MusicSynthesis
{
    public class MidiCommand
    {
        public void ChangeInstrument(int instrument,MidiOut midiOut)
        {
            byte[] command = new byte[2];
            command[0] = 0xC0; // Change
            command[1] = (byte)instrument;
            // command[1] = 0x19; // Guitar
            midiOut.SendBuffer(command);
        }
    }
}
