using Kermalis.EndianBinaryIO;
using System.IO;

namespace Kermalis.DLS2
{
    // DLSID Chunk - Page 40 of spec
    internal sealed class DLSIDChunk : DLSChunk
    {
        public DLSID DLSID { get; }

        public DLSIDChunk(EndianBinaryReader reader) : base("dlid", reader)
        {
            if (Size != 16)
            {
                throw new InvalidDataException();
            }
            DLSID = new DLSID(reader);
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            DLSID.Write(writer);
        }
    }
}
