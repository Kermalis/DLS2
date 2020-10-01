using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
    public sealed class InfoSubChunk : DLSChunk
    {
        public string Text { get; set; } // TODO: Verify setter, update size

        public InfoSubChunk(string name, string text) : base(name)
        {
            Text = text;
        }
        internal InfoSubChunk(string name, EndianBinaryReader reader) : base(name, reader)
        {
            Text = reader.ReadString((int)Size, true);
        }

        internal override void UpdateSize()
        {
            Size = (uint)Text.Length + 1; // +1 for \0
            if (Size % 2 == 1) // Align by 2 bytes
            {
                Size++;
            }
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Text, (int)Size);
        }
    }
}
