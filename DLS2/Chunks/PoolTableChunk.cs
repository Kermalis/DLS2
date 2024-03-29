﻿using Kermalis.EndianBinaryIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.DLS2
{
	// Pool Table Chunk - Page 54 of spec
	public sealed class PoolTableChunk : DLSChunk, IReadOnlyList<uint>
	{
		internal const string EXPECTED_NAME = "ptbl";

		internal uint[] PoolCues;

		public uint this[int index] => PoolCues[index];
		public int Count => PoolCues.Length;

		internal PoolTableChunk() : base(EXPECTED_NAME)
		{
			PoolCues = Array.Empty<uint>();

			UpdateSize();
		}
		internal PoolTableChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			uint byteSize = reader.ReadUInt32();
			if (byteSize != 8)
			{
				throw new InvalidDataException($"PoolTableChunk byteSize was not 8 bytes ({byteSize})");
			}

			PoolCues = new uint[reader.ReadUInt32()];
			reader.ReadUInt32s(PoolCues);
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 4 // byteSize
				+ 4 // numCues
				+ (uint)(4u * PoolCues.Length); // _poolCues
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteUInt32(8);
			writer.WriteUInt32((uint)PoolCues.Length);
			writer.WriteUInt32s(PoolCues);
		}

		public IEnumerator<uint> GetEnumerator()
		{
			return ((IEnumerable<uint>)PoolCues).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return PoolCues.GetEnumerator();
		}
	}
}
