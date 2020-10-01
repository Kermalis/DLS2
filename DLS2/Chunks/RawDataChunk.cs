using Kermalis.EndianBinaryIO;
using System;

namespace Kermalis.DLS2
{
    public abstract class RawDataChunk : DLSChunk
    {
        private byte[] _data;
        public byte[] Data
        {
            get => _data;
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _data = value;
            }
        }

        protected RawDataChunk(string name, byte[] data) : base(name)
        {
            Data = data;
        }
        protected RawDataChunk(string name, EndianBinaryReader reader) : base(name, reader)
        {
            _data = reader.ReadBytes((int)Size);
        }

        internal override void UpdateSize()
        {
            Size = (uint)_data.Length;
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(_data);
        }
    }
}
