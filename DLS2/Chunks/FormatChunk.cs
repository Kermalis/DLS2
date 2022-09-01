using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	// Format Chunk - Page 57 of spec
	public sealed class FormatChunk : DLSChunk
	{
		internal const string EXPECTED_NAME = "fmt ";

		public WaveInfo WaveInfo { get; }
		public FormatInfo FormatInfo { get; }

		public FormatChunk(WaveFormat format) : base(EXPECTED_NAME)
		{
			WaveInfo = new WaveInfo { FormatTag = format };
			if (format == WaveFormat.Extensible)
			{
				FormatInfo = new ExtensibleInfo();
			}
			else
			{
				FormatInfo = new PCMInfo();
			}

			UpdateSize();
		}
		internal FormatChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			WaveInfo = new WaveInfo(reader);
			if (WaveInfo.FormatTag == WaveFormat.Extensible)
			{
				FormatInfo = new ExtensibleInfo(reader);
			}
			else
			{
				FormatInfo = new PCMInfo(reader);
			}
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 14 // WaveFormat
				+ (WaveInfo.FormatTag == WaveFormat.Extensible ? 26u : 2u); // FormatInfo
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			WaveInfo.Write(writer);
			FormatInfo.Write(writer);
		}
	}
}
