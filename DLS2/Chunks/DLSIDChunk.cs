using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	// DLSID Chunk - Page 40 of spec
	public sealed class DLSIDChunk : DLSChunk
	{
		public DLSID DLSID { get; set; }

		public DLSIDChunk(DLSID id) : base("dlid")
		{
			DLSID = id;
		}
		public DLSIDChunk(EndianBinaryReader reader) : base("dlid", reader)
		{
			long endOffset = GetEndOffset(reader);
			DLSID = new DLSID(reader);
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 16; // DLSID
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			DLSID.Write(writer);
		}
	}
}
