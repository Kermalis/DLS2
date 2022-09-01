using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	// DLSID Chunk - Page 40 of spec
	public sealed class DLSIDChunk : DLSChunk
	{
		internal const string EXPECTED_NAME = "dlid";

		public DLSID DLSID { get; set; }

		public DLSIDChunk(DLSID id) : base(EXPECTED_NAME)
		{
			DLSID = id;

			UpdateSize();
		}
		public DLSIDChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			DLSID = new DLSID(reader);
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = DLSID.SIZE; // DLSID
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			DLSID.Write(writer);
		}
	}
}
