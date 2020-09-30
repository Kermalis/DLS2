using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
    public sealed class UnsupportedChunk : DLSChunk
    {
        public byte[] Data { get; set; } // TODO: Verify setter, update size

        internal UnsupportedChunk(string name, EndianBinaryReader reader) : base(name, reader)
        {
            Data = reader.ReadBytes((int)Size);
        }

        internal override void UpdateSize()
        {
            Size = (uint)Data.Length;
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Data);
        }
    }
}
