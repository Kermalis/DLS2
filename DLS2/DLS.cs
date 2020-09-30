using Kermalis.EndianBinaryIO;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.DLS2
{
    public class DLS
    {
        private uint _size;
        private readonly List<DLSChunk> _chunks;

#if DEBUG
        public static void Main()
        {
            new DLS(@"C:\Users\Kermalis\Documents\Emulation\GBA\Games\M\test.dls");
            //new DLS(@"C:\Users\Kermalis\Music\Samples, Presets, Soundfonts, VSTs, etc\Soundfonts\Arachno SoundFont - Version 1.0.dls");
            //new DLS(@"C:\Users\Kermalis\Music\Samples, Presets, Soundfonts, VSTs, etc\Soundfonts\Musyng Kite.dls");
        }
#endif

        public DLS(string path)
        {
            using (var reader = new EndianBinaryReader(File.Open(path, FileMode.Open)))
            {
                string str = reader.ReadString(4, false);
                if (str != "RIFF")
                {
                    throw new InvalidDataException("RIFF header was not found at the start of the file.");
                }
                _size = reader.ReadUInt32();
                str = reader.ReadString(4, false);
                if (str != "DLS ")
                {
                    throw new InvalidDataException("DLS header was not found at the expected offset.");
                }
                _chunks = GetAllChunks(reader, reader.BaseStream.Position + (_size - 4)); // Subtract 4 for the "DLS "
            }
#if DEBUG
            Save(@"C:\Users\Kermalis\Documents\Emulation\GBA\Games\M\test2.dls");
#endif
        }

        public void Save(string path)
        {
            using (var writer = new EndianBinaryWriter(File.Open(path, FileMode.Create)))
            {
                UpdateSize();

                writer.Write("RIFF", 4);
                writer.Write(_size);
                writer.Write("DLS ", 4);
                foreach (DLSChunk c in _chunks)
                {
                    c.Write(writer);
                }
            }
        }

        internal void UpdateSize()
        {
            _size = 4;
            foreach (DLSChunk c in _chunks)
            {
                c.UpdateSize();
                _size += c.Size + 8;
            }
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
                default: return new UnsupportedChunk(str, reader);
            }
        }
    }
}
