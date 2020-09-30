using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
    public class DLSChunk
    {
        /// <summary>Length 4</summary>
        public string ChunkName { get; }
        /// <summary>Size in bytes</summary>
        protected internal uint Size { get; protected set; }

        protected DLSChunk(string name)
        {
            ChunkName = name;
        }
        protected DLSChunk(string name, EndianBinaryReader reader)
        {
            ChunkName = name;
            Size = reader.ReadUInt32();
        }

        internal virtual void UpdateSize() { }

        internal virtual void Write(EndianBinaryWriter writer)
        {
            UpdateSize();
            writer.Write(ChunkName, 4);
            writer.Write(Size);
        }
    }
}
