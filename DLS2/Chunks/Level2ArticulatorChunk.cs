﻿using Kermalis.EndianBinaryIO;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.DLS2
{
    // Level 2 Articulator Chunk - Page 49 of spec
    public sealed class Level2ArticulatorChunk : DLSChunk
    {
        private readonly uint _numConnectionBlocks;
        private readonly List<ConnectionBlock> _connectionBlocks;

        internal Level2ArticulatorChunk(EndianBinaryReader reader) : base("art2", reader)
        {
            long endOffset = GetEndOffset(reader);
            uint byteSize = reader.ReadUInt32();
            if (byteSize != 8)
            {
                throw new InvalidDataException();
            }
            _numConnectionBlocks = reader.ReadUInt32();
            _connectionBlocks = new List<ConnectionBlock>((int)_numConnectionBlocks);
            for (uint i = 0; i < _numConnectionBlocks; i++)
            {
                _connectionBlocks.Add(new ConnectionBlock(reader));
            }
            EatRemainingBytes(reader, endOffset);
        }

        internal override void UpdateSize()
        {
            Size = 4 // byteSize
                + 4 // _numConnectionBlocks
                + (12 * _numConnectionBlocks); // _connectionBlocks
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(8u);
            writer.Write(_numConnectionBlocks);
            for (int i = 0; i < _numConnectionBlocks; i++)
            {
                _connectionBlocks[i].Write(writer);
            }
        }
    }
}
