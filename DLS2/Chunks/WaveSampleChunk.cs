﻿using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
	public sealed class WaveSampleChunk : DLSChunk
	{
		internal const string EXPECTED_NAME = "wsmp";

		public ushort UnityNote { get; set; }
		public short FineTune { get; set; }
		public int Gain { get; set; }
		public WaveSampleOptions Options { get; set; }

		public WaveSampleLoop? Loop { get; set; } // Combining "SampleLoops" and the loop list

		public WaveSampleChunk() : base(EXPECTED_NAME)
		{
			UnityNote = 60;
			Loop = null;

			UpdateSize();
		}
		internal WaveSampleChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			uint byteSize = reader.ReadUInt32();
			if (byteSize != 20)
			{
				throw new InvalidDataException($"WaveSampleChunk byteSize was not 20 bytes ({byteSize})");
			}

			UnityNote = reader.ReadUInt16();
			FineTune = reader.ReadInt16();
			Gain = reader.ReadInt32();
			Options = reader.ReadEnum<WaveSampleOptions>();
			if (reader.ReadUInt32() == 1)
			{
				Loop = new WaveSampleLoop(reader);
			}
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 4 // byteSize
				+ 2 // UnityNote
				+ 2 // FineTune
				+ 4 // Gain
				+ 4 // Options
				+ 4 // DoesLoop
				+ (Loop is null ? 0u : WaveSampleLoop.SIZE); // Loop
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteUInt32(20);
			writer.WriteUInt16(UnityNote);
			writer.WriteInt16(FineTune);
			writer.WriteInt32(Gain);
			writer.WriteEnum(Options);
			if (Loop is null)
			{
				writer.WriteUInt32(0);
			}
			else
			{
				writer.WriteUInt32(1);
				Loop.Write(writer);
			}
		}
	}
}
