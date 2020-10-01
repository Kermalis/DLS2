﻿using Kermalis.EndianBinaryIO;
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
        internal PCMInfo(EndianBinaryReader reader)
        {
            BitsPerSample = reader.ReadUInt16();
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            writer.Write(BitsPerSample);
        }
    }
    // Untested!
    public sealed class ExtensibleInfo : FormatInfo
    {
        public ushort ExtraInfo { get; set; }
        public uint ChannelMask { get; set; }
        public DLSID SubFormat { get; set; }

        internal ExtensibleInfo(EndianBinaryReader reader)
        {
            BitsPerSample = reader.ReadUInt16();
            ushort byteSize = reader.ReadUInt16();
            if (byteSize != 22)
            {
                throw new InvalidDataException();
            }
            ExtraInfo = reader.ReadUInt16();
            ChannelMask = reader.ReadUInt32();
            SubFormat = new DLSID(reader);
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            writer.Write(BitsPerSample);
            writer.Write(22u);
            writer.Write(ExtraInfo);
            writer.Write(ChannelMask);
            SubFormat.Write(writer);
        }
    }

    // Format Chunk - Page 57 of spec
    public sealed class FormatChunk : DLSChunk
    {
        public WaveInfo WaveFormat { get; }
        public FormatInfo FormatInfo { get; }

        internal FormatChunk(EndianBinaryReader reader) : base("fmt ", reader)
        {
            long endOffset = GetEndOffset(reader);
            WaveFormat = new WaveInfo(reader);
            if (WaveFormat.FormatTag == DLS2.WaveFormat.Extensible)
            {
                FormatInfo = new ExtensibleInfo(reader);
            }
            else
            {
                FormatInfo = new PCMInfo(reader);
            }
            EatRemainingBytes(reader, endOffset);
        }

        internal override void UpdateSize()
        {
            Size = 14 // WaveFormat
                + (WaveFormat.FormatTag == DLS2.WaveFormat.Extensible ? 26u : 2u); // FormatInfo
        }

        internal override void Write(EndianBinaryWriter writer)
        {
            base.Write(writer);
            WaveFormat.Write(writer);
            FormatInfo.Write(writer);
        }
    }
}
