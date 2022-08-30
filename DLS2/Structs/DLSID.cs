using Kermalis.EndianBinaryIO;
using System;
using System.Linq;

namespace Kermalis.DLS2
{
	public sealed class DLSID
	{
		public uint Data1 { get; set; }
		public ushort Data2 { get; set; }
		public ushort Data3 { get; set; }
		public byte[] Data4 { get; }

		public static DLSID Query_GMInHardware { get; } = new DLSID(0x178F2F24, 0xC364, 0x11D1, new byte[] { 0xA7, 0x60, 0x00, 0x00, 0xF8, 0x75, 0xAC, 0x12 });
		public static DLSID Query_GSInHardware { get; } = new DLSID(0x178F2F25, 0xC364, 0x11D1, new byte[] { 0xA7, 0x60, 0x00, 0x00, 0xF8, 0x75, 0xAC, 0x12 });
		public static DLSID Query_XGInHardware { get; } = new DLSID(0x178F2F26, 0xC364, 0x11D1, new byte[] { 0xA7, 0x60, 0x00, 0x00, 0xF8, 0x75, 0xAC, 0x12 });
		public static DLSID Query_SupportsDLS1 { get; } = new DLSID(0x178F2F27, 0xC364, 0x11D1, new byte[] { 0xA7, 0x60, 0x00, 0x00, 0xF8, 0x75, 0xAC, 0x12 });
		public static DLSID Query_SampleMemorySize { get; } = new DLSID(0x178F2F28, 0xC364, 0x11D1, new byte[] { 0xA7, 0x60, 0x00, 0x00, 0xF8, 0x75, 0xAC, 0x12 });
		public static DLSID Query_SamplePlaybackRate { get; } = new DLSID(0x2A91F713, 0xA4BF, 0x11D2, new byte[] { 0xBB, 0xDF, 0x00, 0x60, 0x08, 0x33, 0xDB, 0xD8 });
		public static DLSID Query_ManufacturersID { get; } = new DLSID(0xB03E1181, 0x8095, 0x11D2, new byte[] { 0xA1, 0xEF, 0x00, 0x60, 0x08, 0x33, 0xDB, 0xD8 });
		public static DLSID Query_ProductID { get; } = new DLSID(0xB03E1182, 0x8095, 0x11D2, new byte[] { 0xA1, 0xEF, 0x00, 0x60, 0x08, 0x33, 0xDB, 0xD8 });
		public static DLSID Query_SupportsDLS2 { get; } = new DLSID(0xF14599E5, 0x4689, 0x11D2, new byte[] { 0xAF, 0xA6, 0x00, 0xAA, 0x00, 0x24, 0xD8, 0xB6 });

		public DLSID()
		{
			Data4 = new byte[8];
		}
		internal DLSID(EndianBinaryReader reader)
		{
			Data1 = reader.ReadUInt32();
			Data2 = reader.ReadUInt16();
			Data3 = reader.ReadUInt16();
			Data4 = new byte[8];
			reader.ReadBytes(Data4);
		}
		public DLSID(uint data1, ushort data2, ushort data3, byte[] data4)
		{
			if (data4.Length != 8)
			{
				throw new ArgumentOutOfRangeException(nameof(data4), data4, "data4 length must be 8.");
			}
			Data1 = data1;
			Data2 = data2;
			Data3 = data3;
			Data4 = data4;
		}
		public DLSID(byte[] data)
		{
			if (data.Length != 16)
			{
				throw new ArgumentOutOfRangeException(nameof(data), data, "data length must be 16.");
			}
			Data1 = EndianBinaryPrimitives.ReadUInt32(data.AsSpan(0, 4), Endianness.LittleEndian);
			Data2 = EndianBinaryPrimitives.ReadUInt16(data.AsSpan(4, 2), Endianness.LittleEndian);
			Data3 = EndianBinaryPrimitives.ReadUInt16(data.AsSpan(6, 2), Endianness.LittleEndian);
			Data4 = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				Data4[i] = data[8 + i];
			}
		}

		public void Write(EndianBinaryWriter writer)
		{
			writer.WriteUInt32(Data1);
			writer.WriteUInt16(Data2);
			writer.WriteUInt16(Data3);
			writer.WriteBytes(Data4);
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(obj, this))
			{
				return true;
			}
			if (obj is DLSID id)
			{
				return id.Data1 == Data1 && id.Data2 == Data2 && id.Data3 == Data3 && id.Data4.SequenceEqual(Data4);
			}
			return false;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(Data1, Data2, Data3, Data4);
		}
		public override string ToString()
		{
			string str = Data1.ToString("X8") + '-' + Data2.ToString("X4") + '-' + Data3.ToString("X4") + '-';
			for (int i = 0; i < 2; i++)
			{
				str += Data4[i].ToString("X2");
			}
			str += '-';
			for (int i = 2; i < 8; i++)
			{
				str += Data4[i].ToString("X2");
			}
			return str;
		}
	}
}
