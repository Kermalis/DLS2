using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
	public sealed class WaveSampleLoop
	{
		internal const int SIZE = 16;

		public LoopType LoopType { get; set; }
		public uint LoopStart { get; set; }
		public uint LoopLength { get; set; }

		public WaveSampleLoop()
		{
			//
		}
		internal WaveSampleLoop(EndianBinaryReader reader)
		{
			uint byteSize = reader.ReadUInt32();
			if (byteSize != SIZE)
			{
				throw new InvalidDataException($"Wave sample loop was not {SIZE} bytes! ({byteSize} bytes)");
			}

			LoopType = reader.ReadEnum<LoopType>();
			LoopStart = reader.ReadUInt32();
			LoopLength = reader.ReadUInt32();
		}

		internal void Write(EndianBinaryWriter writer)
		{
			writer.WriteUInt32(SIZE);
			writer.WriteEnum(LoopType);
			writer.WriteUInt32(LoopStart);
			writer.WriteUInt32(LoopLength);
		}
	}
}
