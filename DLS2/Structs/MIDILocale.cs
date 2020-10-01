using Kermalis.EndianBinaryIO;
using System;

namespace Kermalis.DLS2
{
    // MIDILOCALE - Page 45 of spec
    public sealed class MIDILocale
    {
        public uint Bank_Raw { get; set; }
        public uint Instrument_Raw { get; set; }

        public byte CC32
        {
            get => (byte)(Bank_Raw & 127);
            set
            {
                if (value > 127)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                Instrument_Raw |= value;
            }
        }
        public byte CC0
        {
            get => (byte)((Bank_Raw >> 7) & 127);
            set
            {
                if (value > 127)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                Instrument_Raw |= (uint)(value << 7);
            }
        }
        public bool IsDrum
        {
            get => (Bank_Raw >> 31) != 0;
            set
            {
                const uint val = 1u << 31;
                if (value)
                {
                    Instrument_Raw |= val;
                }
                else
                {
                    Instrument_Raw &= ~val;
                }
            }
        }
        public byte Instrument
        {
            get => (byte)(Instrument_Raw & 127);
            set
            {
                if (value > 127)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                Instrument_Raw |= value;
            }
        }

        public MIDILocale() { }
        public MIDILocale(byte cc32, byte cc0, bool isDrum, byte instrument)
        {
            CC32 = cc32;
            CC0 = cc0;
            IsDrum = isDrum;
            Instrument = instrument;
        }
        internal MIDILocale(EndianBinaryReader reader)
        {
            Bank_Raw = reader.ReadUInt32();
            Instrument_Raw = reader.ReadUInt32();
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write(Bank_Raw);
            writer.Write(Instrument_Raw);
        }
    }
}
