﻿using Kermalis.EndianBinaryIO;
using System.Collections;
using System.Collections.Generic;

namespace Kermalis.DLS2
{
	// LIST Chunk - Page 40 of spec
	public sealed class ListChunk : DLSChunk, IList<DLSChunk>, IReadOnlyList<DLSChunk>
	{
		internal const string EXPECTED_NAME = "LIST";

		/// <summary>Length 4</summary>
		public string Identifier { get; set; }
		private readonly List<DLSChunk> _children;

		public int Count => _children.Count;
		public bool IsReadOnly => false;
		public DLSChunk this[int index]
		{
			get => _children[index];
			set => _children[index] = value;
		}

		public ListChunk(string identifier) : base(EXPECTED_NAME)
		{
			Identifier = identifier;
			_children = new List<DLSChunk>();

			UpdateSize();
		}
		internal ListChunk(EndianBinaryReader reader) : base(EXPECTED_NAME, reader)
		{
			long endOffset = GetEndOffset(reader);
			Identifier = reader.ReadString_Count(4);
			_children = GetAllChunks(reader, endOffset);
		}

		internal override void UpdateSize()
		{
			Size = 4; // Identifier
			foreach (DLSChunk c in _children)
			{
				c.UpdateSize();
				Size += c.Size + 8;
			}
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteChars_Count(Identifier, 4);
			foreach (DLSChunk c in _children)
			{
				c.Write(writer);
			}
		}

		public void Add(DLSChunk chunk)
		{
			_children.Add(chunk);
		}
		public void Clear()
		{
			_children.Clear();
		}
		public bool Contains(DLSChunk chunk)
		{
			return _children.Contains(chunk);
		}
		public void CopyTo(DLSChunk[] array, int arrayIndex)
		{
			_children.CopyTo(array, arrayIndex);
		}
		public int IndexOf(DLSChunk chunk)
		{
			return _children.IndexOf(chunk);
		}
		public void Insert(int index, DLSChunk chunk)
		{
			_children.Insert(index, chunk);
		}
		public bool Remove(DLSChunk chunk)
		{
			return _children.Remove(chunk);
		}
		public void RemoveAt(int index)
		{
			_children.RemoveAt(index);
		}

		public IEnumerator<DLSChunk> GetEnumerator()
		{
			return _children.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _children.GetEnumerator();
		}
	}
}
