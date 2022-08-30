using Kermalis.EndianBinaryIO;

namespace Kermalis.DLS2
{
	public abstract class RawDataChunk : DLSChunk
	{
		public byte[] Data { get; set; }

		protected RawDataChunk(string name, byte[] data) : base(name)
		{
			Data = data;
		}
		protected RawDataChunk(string name, EndianBinaryReader reader) : base(name, reader)
		{
			Data = new byte[Size];
			reader.ReadBytes(Data);
		}

		internal override void UpdateSize()
		{
			Size = (uint)Data.Length;
			if (Size % 2 != 0) // Align by 2 bytes
			{
				Size++;
			}
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteBytes(Data);
			int dif = (int)(Size - Data.Length);
			if (dif > 0)
			{
				writer.WriteZeroes(dif);
			}
		}
	}
}
