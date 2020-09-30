using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
    // Instrument Header Chunk - Page 45 of spec
    public sealed class InstrumentHeaderChunk : DLSChunk
    {
        public uint NumRegions { get; }
        public MIDILocale Locale { get; }

        internal InstrumentHeaderChunk(EndianBinaryReader reader) : base("insh", reader)
        {
            if (Size != 12)
            {
                throw new InvalidDataException();
            }
            NumRegions = reader.ReadUInt32();
            Locale = new MIDILocale(reader);
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(NumRegions);
            Locale.Write(writer);
        }
    }
}
