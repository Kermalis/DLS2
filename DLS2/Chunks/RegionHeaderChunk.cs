using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
    // Region Header Chunk - Page 45 of spec
    public sealed class RegionHeaderChunk : DLSChunk
    {
        public Range KeyRange { get; }
        public Range VelocityRange { get; }
        public ushort Options { get; set; }
        public ushort KeyGroup { get; set; }
        public ushort Layer { get; set; }

        internal RegionHeaderChunk(EndianBinaryReader reader) : base("rgnh", reader)
        {
            if (Size != 12 && Size != 14)
            {
                throw new InvalidDataException();
            }
            KeyRange = new Range(reader);
            VelocityRange = new Range(reader);
            Options = reader.ReadUInt16();
            KeyGroup = reader.ReadUInt16();
            if (Size == 14)
            {
                Layer = reader.ReadUInt16();
            }
            else
            {
                Size = 14; // We are now adding Layer
            }
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            KeyRange.Write(writer);
            VelocityRange.Write(writer);
            writer.Write(Options);
            writer.Write(KeyGroup);
            writer.Write(Layer);
        }
    }
}
