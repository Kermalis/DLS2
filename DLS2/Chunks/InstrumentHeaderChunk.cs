using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
    // Instrument Header Chunk - Page 45 of spec
    public sealed class InstrumentHeaderChunk : DLSChunk
    {
        public uint NumRegions { get; }
        public MIDILocale Locale { get; }

        internal InstrumentHeaderChunk(EndianBinaryReader reader) : base("insh", reader)
        {
            long endOffset = GetEndOffset(reader);
            NumRegions = reader.ReadUInt32();
            Locale = new MIDILocale(reader);
            EatRemainingBytes(reader, endOffset);
        }

        internal override void UpdateSize()
        {
            Size = 4 // NumRegions
                + 8; // Locale
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(NumRegions);
            Locale.Write(writer);
        }
    }
}
