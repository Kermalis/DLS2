using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	public sealed class DataChunk : RawDataChunk
	{
		internal const string EXPECTED_NAME = "data";

		public DataChunk(byte[] data) : base(EXPECTED_NAME, data)
		{
			//
		}
		internal DataChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			//
		}
	}
}
