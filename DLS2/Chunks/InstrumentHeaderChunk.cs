using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	// Instrument Header Chunk - Page 45 of spec
	public sealed class InstrumentHeaderChunk : DLSChunk
	{
		internal const string EXPECTED_NAME = "insh";

		public uint NumRegions { get; set; }
		public MIDILocale Locale { get; set; }

		public InstrumentHeaderChunk(MIDILocale locale) : base(EXPECTED_NAME)
		{
			Locale = locale;

			UpdateSize();
		}
		internal InstrumentHeaderChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			NumRegions = reader.ReadUInt32();
			Locale = new MIDILocale(reader);
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 4 // NumRegions
				+ MIDILocale.SIZE; // Locale
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteUInt32(NumRegions);
			Locale.Write(writer);
		}
	}
}
