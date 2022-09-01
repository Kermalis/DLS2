using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	// Region Header Chunk - Page 45 of spec
	public sealed class RegionHeaderChunk : DLSChunk
	{
		internal const string EXPECTED_NAME = "rgnh";

		public Range KeyRange { get; set; }
		public Range VelocityRange { get; set; }
		public ushort Options { get; set; }
		public ushort KeyGroup { get; set; }
		public ushort Layer { get; set; }

		public RegionHeaderChunk() : base(EXPECTED_NAME)
		{
			KeyRange = new Range(0, 127);
			VelocityRange = new Range(0, 127);

			UpdateSize();
		}
		internal RegionHeaderChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			KeyRange = new Range(reader);
			VelocityRange = new Range(reader);
			Options = reader.ReadUInt16();
			KeyGroup = reader.ReadUInt16();
			if (Size >= 14) // Size of 12 is also valid
			{
				Layer = reader.ReadUInt16();
			}
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = Range.SIZE // KeyRange
				+ Range.SIZE // VelocityRange
				+ 2 // Options
				+ 2 // KeyGroup
				+ 2; // Layer
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			KeyRange.Write(writer);
			VelocityRange.Write(writer);
			writer.WriteUInt16(Options);
			writer.WriteUInt16(KeyGroup);
			writer.WriteUInt16(Layer);
		}
	}
}
