using Kermalis.EndianBinaryIO;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Kermalis.DLS2
{
	public sealed class InfoSubChunk : DLSChunk
	{
		private string _text;
		public string Text
		{
			get => _text;
			[MemberNotNull(nameof(_text))]
			set
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] > sbyte.MaxValue)
					{
						throw new ArgumentException("Text must be ASCII");
					}
				}
				_text = value;
			}
		}

		public InfoSubChunk(string name, string text) : base(name)
		{
			Text = text;
		}
		internal InfoSubChunk(string name, EndianBinaryReader reader) : base(name, reader)
		{
			long endOffset = GetEndOffset(reader);
			_text = reader.ReadString_NullTerminated();
			EatRemainingBytes(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = (uint)_text.Length + 1; // +1 for \0
			if (Size % 2 != 0) // Align by 2 bytes
			{
				Size++;
			}
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteChars_Count(_text, (int)Size);
		}
	}
}
