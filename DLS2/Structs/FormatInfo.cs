using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
	public abstract class FormatInfo
	{
		public ushort BitsPerSample { get; set; }

		internal abstract void Write(EndianBinaryWriter writer);
	}
	public sealed class PCMInfo : FormatInfo
	{
		internal PCMInfo()
		{
			//
		}
		internal PCMInfo(EndianBinaryReader reader)
		{
			BitsPerSample = reader.ReadUInt16();
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			writer.WriteUInt16(BitsPerSample);
		}
	}
	// Untested!
	public sealed class ExtensibleInfo : FormatInfo
	{
		public ushort ExtraInfo { get; set; }
		public uint ChannelMask { get; set; }
		public DLSID SubFormat { get; set; }

		internal ExtensibleInfo()
		{
			SubFormat = new DLSID();
		}
		internal ExtensibleInfo(EndianBinaryReader reader)
		{
			BitsPerSample = reader.ReadUInt16();
			ushort byteSize = reader.ReadUInt16();
			if (byteSize != 22)
			{
				throw new InvalidDataException($"Level1ArticulatorChunk byteSize was not 22 bytes ({byteSize})");
			}
			ExtraInfo = reader.ReadUInt16();
			ChannelMask = reader.ReadUInt32();
			SubFormat = new DLSID(reader);
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			writer.WriteUInt16(BitsPerSample);
			writer.WriteUInt32(22);
			writer.WriteUInt16(ExtraInfo);
			writer.WriteUInt32(ChannelMask);
			SubFormat.Write(writer);
		}
	}
}
