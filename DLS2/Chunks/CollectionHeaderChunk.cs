using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
    // Collection Header Chunk - Page 40 of spec
    internal sealed class CollectionHeaderChunk : DLSChunk
    {
        public uint NumInstruments { get; }

        public CollectionHeaderChunk(EndianBinaryReader reader) : base("colh", reader)
        {
            if (Size != 4)
            {
                throw new InvalidDataException();
            }
            NumInstruments = reader.ReadUInt32();
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(NumInstruments);
        }
    }
}
