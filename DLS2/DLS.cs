using Kermalis.EndianBinaryIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kermalis.DLS2
{
	public sealed class DLS : IList<DLSChunk>, IReadOnlyList<DLSChunk>
	{
		private readonly List<DLSChunk> _chunks;

		public int Count => _chunks.Count;
		public bool IsReadOnly => false;
		public DLSChunk this[int index]
		{
			get => _chunks[index];
			set => _chunks[index] = value;
		}

		public CollectionHeaderChunk CollectionHeader { get; }
		public ListChunk InstrumentList { get; }
		public PoolTableChunk PoolTable { get; }
		public ListChunk WavePool { get; }

#if DEBUG
		public static void Main()
		{
			var d =
			//new DLS(@"C:\Users\Kermalis\Documents\Emulation\GBA\Games\Custom\test.dls");
			//new DLS(@"C:\Users\Kermalis\Documents\Emulation\GBA\Games\Custom\test2.dls");
			//new DLS(@"D:\Music\Samples, Presets, Soundfonts, VSTs, etc\Soundfonts\Arachno SoundFont - Version 1.0.dls");
			//new DLS(@"D:\Music\Samples, Presets, Soundfonts, VSTs, etc\Soundfonts\Musyng Kite.dls");
			new DLS(@"D:\Music\Samples, Presets, Soundfonts, VSTs, etc\Soundfonts\RSE Corrected Soundfont Revision 17.dls");
			string s = d.GetHierarchy();
			;
		}
#endif

		/// <summary>For creating.</summary>
		public DLS()
		{
			CollectionHeader = new CollectionHeaderChunk();
			InstrumentList = new ListChunk("lins");
			PoolTable = new PoolTableChunk();
			WavePool = new ListChunk("wvpl");
			_chunks = new List<DLSChunk>(4)
			{
				CollectionHeader,
				InstrumentList,
				PoolTable,
				WavePool,
			};
		}
		public DLS(string path)
		{
			using (FileStream stream = File.Open(path, FileMode.Open))
			{
				_chunks = Init(new EndianBinaryReader(stream, ascii: true),
					out CollectionHeaderChunk colh, out ListChunk lins, out PoolTableChunk ptbl, out ListChunk wvpl);
				CollectionHeader = colh;
				InstrumentList = lins;
				PoolTable = ptbl;
				WavePool = wvpl;
			}
		}
		public DLS(Stream stream)
		{
			_chunks = Init(new EndianBinaryReader(stream, ascii: true),
				out CollectionHeaderChunk colh, out ListChunk lins, out PoolTableChunk ptbl, out ListChunk wvpl);
			CollectionHeader = colh;
			InstrumentList = lins;
			PoolTable = ptbl;
			WavePool = wvpl;
		}
		private static List<DLSChunk> Init(EndianBinaryReader reader, out CollectionHeaderChunk colh, out ListChunk lins, out PoolTableChunk ptbl, out ListChunk wvpl)
		{
			string chunkName = reader.ReadString_Count(4);
			if (chunkName != "RIFF")
			{
				throw new InvalidDataException("RIFF header was not found at the start of the file.");
			}

			uint size = reader.ReadUInt32();
			long endOffset = reader.Stream.Position + size;
			chunkName = reader.ReadString_Count(4);
			if (chunkName != "DLS ")
			{
				throw new InvalidDataException("DLS header was not found at the expected offset.");
			}

			List<DLSChunk> chunks = DLSChunk.GetAllChunks(reader, endOffset);
			if (chunks.Count >= 4)
			{
				colh = null!;
				lins = null!;
				ptbl = null!;
				wvpl = null!;

				foreach (DLSChunk ch in chunks)
				{
					switch (ch)
					{
						case CollectionHeaderChunk c: colh = c; break;
						case ListChunk c when c.Identifier == "lins": lins = c; break;
						case PoolTableChunk c: ptbl = c; break;
						case ListChunk c when c.Identifier == "wvpl": wvpl = c; break;
					}
				}

				if (colh is not null && lins is not null && ptbl is not null && wvpl is not null)
				{
					return chunks;
				}
			}

			throw new InvalidDataException("Could not find the 4 required chunks: colh, lins, ptbl, wvpl");
		}

		public void UpdateCollectionHeader()
		{
			CollectionHeader.NumInstruments = (uint)InstrumentList.Count;
		}
		/// <summary>Updates the pointers in the <see cref="PoolTable"/>. Should be called after modifying <see cref="WavePool"/>.</summary>
		public void UpdatePoolTable()
		{
			ListChunk wvpl = WavePool;
			ref uint[] poolCues = ref PoolTable.PoolCues;
			Array.Resize(ref poolCues, wvpl.Count);

			uint cur = 0;
			for (int i = 0; i < wvpl.Count; i++)
			{
				poolCues[i] = cur;
				DLSChunk c = wvpl[i];
				c.UpdateSize();
				cur += c.Size + 8;
			}
		}
		public void Save(string path)
		{
			UpdateCollectionHeader();
			UpdatePoolTable();

			using (FileStream stream = File.Open(path, FileMode.Create))
			{
				var writer = new EndianBinaryWriter(stream, ascii: true);
				writer.WriteChars_Count("RIFF", 4);
				writer.WriteUInt32(UpdateSize());
				writer.WriteChars_Count("DLS ", 4);
				foreach (DLSChunk c in _chunks)
				{
					c.Write(writer);
				}
			}
		}

		public string GetHierarchy()
		{
			static void ApplyTabLevel(StringBuilder str, int tabLevel)
			{
				for (int t = 0; t < tabLevel; t++)
				{
					str.Append('\t');
				}
			}
			static void Recursion(StringBuilder str, ref int tabLevel, IReadOnlyList<DLSChunk> parent, string listName)
			{
				ApplyTabLevel(str, tabLevel);
				str.Append($"{listName} ({parent.Count})");
				tabLevel++;
				foreach (DLSChunk c in parent)
				{
					str.AppendLine();
					if (c is ListChunk lc)
					{
						Recursion(str, ref tabLevel, lc, $"{lc.ChunkName} '{lc.Identifier}'");
					}
					else
					{
						ApplyTabLevel(str, tabLevel);
						str.Append($"<{c.ChunkName}>");
						if (c is InfoSubChunk ic)
						{
							str.Append($" [\"{ic.Text}\"]");
						}
						else if (c is RawDataChunk dc)
						{
							str.Append($" [{dc.Data.Length} bytes]");
						}
					}
				}
				tabLevel--;
			}

			var str = new StringBuilder();
			int tabLevel = 0;
			Recursion(str, ref tabLevel, this, "RIFF 'DLS '");
			return str.ToString();
		}

		public uint UpdateSize()
		{
			uint size = 4;
			foreach (DLSChunk c in _chunks)
			{
				c.UpdateSize();
				size += c.Size + 8;
			}
			return size;
		}

		public void Add(DLSChunk chunk)
		{
			_chunks.Add(chunk);
		}
		public void Clear()
		{
			_chunks.Clear();
		}
		public bool Contains(DLSChunk chunk)
		{
			return _chunks.Contains(chunk);
		}
		public void CopyTo(DLSChunk[] array, int arrayIndex)
		{
			_chunks.CopyTo(array, arrayIndex);
		}
		public int IndexOf(DLSChunk chunk)
		{
			return _chunks.IndexOf(chunk);
		}
		public void Insert(int index, DLSChunk chunk)
		{
			_chunks.Insert(index, chunk);
		}
		public bool Remove(DLSChunk chunk)
		{
			return _chunks.Remove(chunk);
		}
		public void RemoveAt(int index)
		{
			_chunks.RemoveAt(index);
		}

		public IEnumerator<DLSChunk> GetEnumerator()
		{
			return _chunks.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _chunks.GetEnumerator();
		}
	}
}
