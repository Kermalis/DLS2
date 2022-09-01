using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	// Collection Header Chunk - Page 40 of spec
	public sealed class CollectionHeaderChunk : DLSChunk
	{
		internal const string EXPECTED_NAME = "colh";

		public uint NumInstruments { get; internal set; }

		internal CollectionHeaderChunk() : base(EXPECTED_NAME)
		{
			UpdateSize();
		}
		public CollectionHeaderChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			NumInstruments = reader.ReadUInt32();
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 4; // NumInstruments
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteUInt32(NumInstruments);
		}
	}
}
