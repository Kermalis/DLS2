using Kermalis.EndianBinaryIO;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.DLS2
{
    public sealed class WaveSampleChunk : DLSChunk
    {
        public ushort UnityNote { get; set; }
        public short FineTune { get; set; }
        public int Gain { get; set; }
        public uint Options { get; set; }
        public uint SampleLoops { get; }
        private readonly List<WaveSampleLoop> _loops;

        public WaveSampleChunk() : base("wsmp")
        {
            _loops = new List<WaveSampleLoop>();
        }
        internal WaveSampleChunk(EndianBinaryReader reader) : base("wsmp", reader)
        {
            uint byteSize = reader.ReadUInt32();
            if (byteSize != 20)
            {
                throw new InvalidDataException();
            }
            UnityNote = reader.ReadUInt16();
            FineTune = reader.ReadInt16();
            Gain = reader.ReadInt32();
            Options = reader.ReadUInt32();
            SampleLoops = reader.ReadUInt32();
            _loops = new List<WaveSampleLoop>((int)SampleLoops);
            for (uint i = 0; i < SampleLoops; i++)
            {
                _loops.Add(new WaveSampleLoop(reader));
            }
        }

        internal override void UpdateSize()
        {
            Size = 4 // _byteSize
                + 2 // UnityNote
                + 2 // FineTune
                + 4 // Gain
                + 4 // Options
                + 4 // SampleLoops
                + (16 * SampleLoops); // _loops
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(20u);
            writer.Write(UnityNote);
            writer.Write(FineTune);
            writer.Write(Gain);
            writer.Write(Options);
            writer.Write(SampleLoops);
            for (int i = 0; i < SampleLoops; i++)
            {
                _loops[i].Write(writer);
            }
        }
    }
}
