using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
    public sealed class ConnectionBlock
    {
        public ushort Source { get; set; }
        public ushort Control { get; set; }
        public ushort Destination { get; set; }
        public ushort Transform { get; set; }
        public int Scale { get; set; }

        public ConnectionBlock() { }
        internal ConnectionBlock(EndianBinaryReader reader)
        {
            Source = reader.ReadUInt16();
            Control = reader.ReadUInt16();
            Destination = reader.ReadUInt16();
            Transform = reader.ReadUInt16();
            Scale = reader.ReadInt32();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(Source);
            writer.Write(Control);
            writer.Write(Destination);
            writer.Write(Transform);
            writer.Write(Scale);
        }
    }
}
