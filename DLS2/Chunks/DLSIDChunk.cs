using Kermalis.EndianBinaryIO;
using System;
using System.IO;

namespace Kermalis.DLS2
{
    // DLSID Chunk - Page 40 of spec
    public sealed class DLSIDChunk : DLSChunk
    {
        private DLSID _dlsid;
        public DLSID DLSID
        {
            get => _dlsid;
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _dlsid = value;
            }
        }

        public DLSIDChunk(DLSID id) : base("dlid")
        {
            DLSID = id;
        }
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
