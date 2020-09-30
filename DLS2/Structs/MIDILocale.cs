using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
    // MIDILOCALE - Page 45 of spec
    public sealed class MIDILocale
    {
        public uint Bank { get; set; } // TODO: bits
        public uint Instrument { get; set; }

        internal MIDILocale(EndianBinaryReader reader)
        {
            Bank = reader.ReadUInt32();
            Instrument = reader.ReadUInt32();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(Bank);
            writer.Write(Instrument);
        }
    }
}
