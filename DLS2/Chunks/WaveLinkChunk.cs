using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
    public sealed class WaveLinkChunk : DLSChunk
    {
        public ushort Options { get; set; }
        public ushort PhaseGroup { get; set; }
        public uint Channel { get; set; }
        public uint TableIndex { get; set; }

        public WaveLinkChunk() : base("wlnk") { }
        internal WaveLinkChunk(EndianBinaryReader reader) : base("wlnk", reader)
        {
            if (Size != 12)
            {
                throw new InvalidDataException();
            }
            Options = reader.ReadUInt16();
            PhaseGroup = reader.ReadUInt16();
            Channel = reader.ReadUInt32();
            TableIndex = reader.ReadUInt32();
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Options);
            writer.Write(PhaseGroup);
            writer.Write(Channel);
            writer.Write(TableIndex);
        }
    }
}
