using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
    public sealed class WaveSampleLoop
    {
        private readonly uint _byteSize;
        public uint LoopType { get; set; }
        public uint LoopStart { get; set; }
        public uint LoopLength { get; set; }

        internal WaveSampleLoop(EndianBinaryReader reader)
        {
            _byteSize = reader.ReadUInt32();
            if (_byteSize != 16)
            {
                throw new InvalidDataException();
            }
            LoopType = reader.ReadUInt32();
            LoopStart = reader.ReadUInt32();
            LoopLength = reader.ReadUInt32();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(_byteSize);
            writer.Write(LoopType);
            writer.Write(LoopStart);
            writer.Write(LoopLength);
        }
    }
}
