using Kermalis.EndianBinaryIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _chunks[index] = value;
            }
        }

        public CollectionHeaderChunk CollectionHeader => GetChunk<CollectionHeaderChunk>();
        public ListChunk InstrumentList => GetListChunk("lins");
        public PoolTableChunk PoolTable => GetChunk<PoolTableChunk>();
        public ListChunk WaveInfoList => GetListChunk("wvpl");

        private T GetChunk<T>() where T : DLSChunk
        {
            return (T)_chunks.Find(c => c is T);
        }
        private ListChunk GetListChunk(string str)
        {
            return (ListChunk)_chunks.Find(c => c is ListChunk lc && lc.Identifier == str);
        }

#if DEBUG
        public static void Main()
        {
            new DLS(@"C:\Users\Kermalis\Documents\Emulation\GBA\Games\M\test.dls");
            //new DLS(@"C:\Users\Kermalis\Music\Samples, Presets, Soundfonts, VSTs, etc\Soundfonts\Arachno SoundFont - Version 1.0.dls");
            //new DLS(@"C:\Users\Kermalis\Music\Samples, Presets, Soundfonts, VSTs, etc\Soundfonts\Musyng Kite.dls");
        }
#endif

        /// <summary>For creating.</summary>
        public DLS()
        {
            _chunks = new List<DLSChunk>()
            {
                new CollectionHeaderChunk(),
                new ListChunk("lins"),
                new PoolTableChunk(),
                new ListChunk("wvpl"),
            };
        }
        public DLS(string path)
        {
            using (var reader = new EndianBinaryReader(File.Open(path, FileMode.Open)))
            {
                string str = reader.ReadString(4, false);
                if (str != "RIFF")
                {
                    throw new InvalidDataException("RIFF header was not found at the start of the file.");
                }
                uint size = reader.ReadUInt32();
                str = reader.ReadString(4, false);
                if (str != "DLS ")
                {
                    throw new InvalidDataException("DLS header was not found at the expected offset.");
                }
                _chunks = GetAllChunks(reader, reader.BaseStream.Position + (size - 4)); // Subtract 4 for the "DLS "
            }
#if DEBUG
            Save(@"C:\Users\Kermalis\Documents\Emulation\GBA\Games\M\test2.dls");
#endif
        }

        public void Save(string path)
        {
            using (var writer = new EndianBinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write("RIFF", 4);
                writer.Write(UpdateSize());
                writer.Write("DLS ", 4);
                foreach (DLSChunk c in _chunks)
                {
                    c.Write(writer);
                }
            }
        }

        public string GetHierarchy()
        {
            string str = string.Empty;
            int tabLevel = 0;
            void ApplyTabLevel()
            {
                for (int t = 0; t < tabLevel; t++)
                {
                    str += '\t';
                }
            }
            void Recursion(IReadOnlyList<DLSChunk> parent, string listName)
            {
                ApplyTabLevel();
                str += $"{listName} ({parent.Count})";
                tabLevel++;
                foreach (DLSChunk c in parent)
                {
                    str += Environment.NewLine;
                    if (c is ListChunk lc)
                    {
                        Recursion(lc, $"{lc.ChunkName} '{lc.Identifier}'");
                    }
                    else
                    {
                        ApplyTabLevel();
                        str += $"<{c.ChunkName}>";
                        if (c is InfoSubChunk ic)
                        {
                            str += $" [\"{ic.Text}\"]";
                        }
                    }
                }
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                tabLevel--;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            }
            Recursion(this, "RIFF 'DLS '");
            return str;
        }

        private uint UpdateSize()
        {
            uint size = 4;
            foreach (DLSChunk c in _chunks)
            {
                c.UpdateSize();
                size += c.Size + 8;
            }
            return size;
        }

        internal static List<DLSChunk> GetAllChunks(EndianBinaryReader reader, long endOffset)
        {
            var chunks = new List<DLSChunk>();
            while (reader.BaseStream.Position < endOffset)
            {
                chunks.Add(SwitchNextChunk(reader));
            }
            if (reader.BaseStream.Position > endOffset)
            {
                throw new InvalidDataException();
            }
            return chunks;
        }
        private static DLSChunk SwitchNextChunk(EndianBinaryReader reader)
        {
            string str = reader.ReadString(4, false);
            switch (str)
            {
                case "art1": return new Level1ArticulatorChunk(reader);
                case "art2": return new Level2ArticulatorChunk(reader);
                case "colh": return new CollectionHeaderChunk(reader);
                case "dlid": return new DLSIDChunk(reader);
                case "insh": return new InstrumentHeaderChunk(reader);
                case "LIST": return new ListChunk(reader);
                case "ptbl": return new PoolTableChunk(reader);
                case "rgnh": return new RegionHeaderChunk(reader);
                case "wlnk": return new WaveLinkChunk(reader);
                case "wsmp": return new WaveSampleChunk(reader);
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
                case "ITCH": return new InfoSubChunk(str, reader);
                default: return new UnsupportedChunk(str, reader);
            }
        }

        public void Add(DLSChunk chunk)
        {
            if (chunk is null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }
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
            if (chunk is null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }
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
