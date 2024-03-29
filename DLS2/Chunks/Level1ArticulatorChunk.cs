﻿using Kermalis.EndianBinaryIO;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.DLS2
{
	// Level 1 Articulator Chunk - Page 46 of spec
	public sealed class Level1ArticulatorChunk : DLSChunk, IList<Level1ArticulatorConnectionBlock>, IReadOnlyList<Level1ArticulatorConnectionBlock>
	{
		internal const string EXPECTED_NAME = "art1";

		private readonly List<Level1ArticulatorConnectionBlock> _connectionBlocks;

		public Level1ArticulatorConnectionBlock this[int index]
		{
			get => _connectionBlocks[index];
			set => _connectionBlocks[index] = value;
		}
		public int Count => _connectionBlocks.Count;
		public bool IsReadOnly => false;

		public Level1ArticulatorChunk() : base(EXPECTED_NAME)
		{
			_connectionBlocks = new List<Level1ArticulatorConnectionBlock>();

			UpdateSize();
		}
		internal Level1ArticulatorChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			uint byteSize = reader.ReadUInt32();
			if (byteSize != 8)
			{
				throw new InvalidDataException($"Level1ArticulatorChunk byteSize was not 8 bytes ({byteSize})");
			}

			uint numConnectionBlocks = reader.ReadUInt32();
			_connectionBlocks = new List<Level1ArticulatorConnectionBlock>((int)numConnectionBlocks);
			for (uint i = 0; i < numConnectionBlocks; i++)
			{
				_connectionBlocks.Add(new Level1ArticulatorConnectionBlock(reader));
			}
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 4 // byteSize
				+ 4 // _numConnectionBlocks
				+ (uint)(12u * _connectionBlocks.Count); // _connectionBlocks
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteUInt32(8);
			writer.WriteUInt32((uint)_connectionBlocks.Count);
			for (int i = 0; i < _connectionBlocks.Count; i++)
			{
				_connectionBlocks[i].Write(writer);
			}
		}

		public IEnumerator<Level1ArticulatorConnectionBlock> GetEnumerator()
		{
			return _connectionBlocks.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _connectionBlocks.GetEnumerator();
		}

		public void Add(Level1ArticulatorConnectionBlock item)
		{
			_connectionBlocks.Add(item);
		}
		public void Clear()
		{
			_connectionBlocks.Clear();
		}
		public void CopyTo(Level1ArticulatorConnectionBlock[] array, int arrayIndex)
		{
			_connectionBlocks.CopyTo(array, arrayIndex);
		}
		public bool Contains(Level1ArticulatorConnectionBlock item)
		{
			return _connectionBlocks.Contains(item);
		}
		public int IndexOf(Level1ArticulatorConnectionBlock item)
		{
			return _connectionBlocks.IndexOf(item);
		}
		public void Insert(int index, Level1ArticulatorConnectionBlock item)
		{
			_connectionBlocks.Insert(index, item);
		}
		public bool Remove(Level1ArticulatorConnectionBlock item)
		{
			return _connectionBlocks.Remove(item);
		}
		public void RemoveAt(int index)
		{
			_connectionBlocks.RemoveAt(index);
		}
	}
}
