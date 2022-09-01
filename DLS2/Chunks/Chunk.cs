using Kermalis.EndianBinaryIO;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.DLS2
{
	public abstract class DLSChunk
	{
		/// <summary>Length 4</summary>
		public string ChunkName { get; }
		/// <summary>Size in bytes. Only accurate after <see cref="UpdateSize"/>.</summary>
		public uint Size { get; protected set; }

		protected DLSChunk(string chunkName)
		{
			ChunkName = chunkName;
		}
		protected DLSChunk(string chunkName, EndianBinaryReader reader)
		{
			ChunkName = chunkName;
			Size = reader.ReadUInt32();
		}

		protected long GetEndOffset(EndianBinaryReader reader)
		{
			return reader.Stream.Position + Size;
		}
		protected void EatRemainingBytes(EndianBinaryReader reader, long endOffset)
		{
			if (reader.Stream.Position > endOffset)
			{
				throw new InvalidDataException($"Chunk was too short ({ChunkName} = {Size})");
			}
			reader.Stream.Position = endOffset;
		}

		internal abstract void UpdateSize();

		internal virtual void Write(EndianBinaryWriter writer)
		{
			UpdateSize();
			writer.WriteChars_Count(ChunkName, 4);
			writer.WriteUInt32(Size);
		}

		internal static List<DLSChunk> GetAllChunks(EndianBinaryReader reader, long endOffset)
		{
			var chunks = new List<DLSChunk>();
			while (reader.Stream.Position < endOffset)
			{
				chunks.Add(SwitchNextChunk(reader));
			}
			if (reader.Stream.Position > endOffset)
			{
				throw new InvalidDataException("Expected to read a certain amount of chunks, but the data was read incorrectly...");
			}
			return chunks;
		}
		private static DLSChunk SwitchNextChunk(EndianBinaryReader reader)
		{
			string chunkName = reader.ReadString_Count(4);
			switch (chunkName)
			{
				case Level1ArticulatorChunk.EXPECTED_NAME: return new Level1ArticulatorChunk(reader);
				case Level2ArticulatorChunk.EXPECTED_NAME: return new Level2ArticulatorChunk(reader);
				case CollectionHeaderChunk.EXPECTED_NAME: return new CollectionHeaderChunk(reader);
				case DataChunk.EXPECTED_NAME: return new DataChunk(reader);
				case DLSIDChunk.EXPECTED_NAME: return new DLSIDChunk(reader);
				case FormatChunk.EXPECTED_NAME: return new FormatChunk(reader);
				case InstrumentHeaderChunk.EXPECTED_NAME: return new InstrumentHeaderChunk(reader);
				case ListChunk.EXPECTED_NAME: return new ListChunk(reader);
				case PoolTableChunk.EXPECTED_NAME: return new PoolTableChunk(reader);
				case RegionHeaderChunk.EXPECTED_NAME: return new RegionHeaderChunk(reader);
				case WaveLinkChunk.EXPECTED_NAME: return new WaveLinkChunk(reader);
				case WaveSampleChunk.EXPECTED_NAME: return new WaveSampleChunk(reader);
				// InfoSubChunks
				case "IARL":
				case "IART":
				case "ICMS":
				case "ICMD":
				case "ICOP":
				case "ICRD":
				case "IENG":
				case "IGNR":
				case "IKEY":
				case "IMED":
				case "INAM":
				case "IPRD":
				case "ISBJ":
				case "ISFT":
				case "ISRC":
				case "ISRF":
				case "ITCH": return new InfoSubChunk(chunkName, reader);
				default: return new UnsupportedChunk(chunkName, reader);
			}
		}
	}
}
