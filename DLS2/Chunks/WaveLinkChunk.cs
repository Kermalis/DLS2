﻿using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	public sealed class WaveLinkChunk : DLSChunk
	{
		internal const string EXPECTED_NAME = "wlnk";

		public WaveLinkOptions Options { get; set; }
		public ushort PhaseGroup { get; set; }
		public WaveLinkChannels Channels { get; set; }
		public uint TableIndex { get; set; }

		public WaveLinkChunk() : base(EXPECTED_NAME)
		{
			Channels = WaveLinkChannels.Left;

			UpdateSize();
		}
		internal WaveLinkChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			Options = reader.ReadEnum<WaveLinkOptions>();
			PhaseGroup = reader.ReadUInt16();
			Channels = reader.ReadEnum<WaveLinkChannels>();
			TableIndex = reader.ReadUInt32();
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 2 // Options
				+ 2 // PhaseGroup
				+ 4 // Channel
				+ 4; // TableIndex
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteEnum(Options);
			writer.WriteUInt16(PhaseGroup);
			writer.WriteEnum(Channels);
			writer.WriteUInt32(TableIndex);
		}
	}
}
