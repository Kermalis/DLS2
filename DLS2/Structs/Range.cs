using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	public sealed class Range
	{
		internal const int SIZE = 4;

		public ushort Low { get; set; }
		public ushort High { get; set; }

		public Range()
		{
			//
		}
		public Range(ushort low, ushort high)
		{
			Low = low;
			High = high;
		}
		internal Range(EndianBinaryReader reader)
		{
			Low = reader.ReadUInt16();
			High = reader.ReadUInt16();
		}

		internal void Write(EndianBinaryWriter writer)
		{
			writer.WriteUInt16(Low);
			writer.WriteUInt16(High);
		}
	}
}
