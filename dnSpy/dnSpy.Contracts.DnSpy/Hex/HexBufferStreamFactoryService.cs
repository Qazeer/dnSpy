﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;

namespace dnSpy.Contracts.Hex {
	/// <summary>
	/// Creates <see cref="HexBufferStream"/>s
	/// </summary>
	public abstract class HexBufferStreamFactoryService {
		/// <summary>
		/// Constructor
		/// </summary>
		protected HexBufferStreamFactoryService() { }

		/// <summary>
		/// Creates a <see cref="HexBufferStream"/>
		/// </summary>
		/// <param name="filename">Filename</param>
		/// <returns></returns>
		public HexBufferStream Create(string filename) {
			if (filename == null)
				throw new ArgumentNullException(nameof(filename));
			return Create(File.ReadAllBytes(filename), filename);
		}

		/// <summary>
		/// Creates a <see cref="HexBufferStream"/>
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="name">Name, can be anything and is usually the filename</param>
		/// <returns></returns>
		public abstract HexBufferStream Create(byte[] data, string name);

		/// <summary>
		/// Creates a <see cref="HexBufferStream"/>
		/// </summary>
		/// <param name="simpleStream">Underlying stream</param>
		/// <returns></returns>
		public abstract HexCachedBufferStream CreateCached(HexSimpleBufferStream simpleStream);

		/// <summary>
		/// Creates a process stream
		/// </summary>
		/// <param name="hProcess">Process handle</param>
		/// <param name="name">Name or null to use the default name</param>
		/// <param name="isReadOnly">true if it's read only</param>
		/// <param name="isVolatile">true if the memory can be changed by other code</param>
		/// <returns></returns>
		public HexCachedBufferStream CreateCached(IntPtr hProcess, string name = null, bool isReadOnly = false, bool isVolatile = true) {
			var simpleStream = CreateSimpleStream(hProcess, name, isReadOnly, isVolatile);
			return CreateCached(simpleStream);
		}

		/// <summary>
		/// Creates a process stream
		/// </summary>
		/// <param name="hProcess">Process handle</param>
		/// <param name="name">Name or null to use the default name</param>
		/// <param name="isReadOnly">true if it's read only</param>
		/// <param name="isVolatile">true if the memory can be changed by other code</param>
		/// <returns></returns>
		public abstract HexSimpleBufferStream CreateSimpleStream(IntPtr hProcess, string name = null, bool isReadOnly = false, bool isVolatile = true);
	}

	/// <summary>
	/// A cached buffer stream
	/// </summary>
	public abstract class HexCachedBufferStream : HexBufferStream {
		/// <summary>
		/// Constructor
		/// </summary>
		protected HexCachedBufferStream() { }

		/// <summary>
		/// Invalidates all memory
		/// </summary>
		public void InvalidateAll() => Invalidate(Span);

		/// <summary>
		/// Invalidates a region of memory
		/// </summary>
		/// <param name="span">Span</param>
		public abstract void Invalidate(HexSpan span);
	}

	/// <summary>
	/// A buffer stream with less methods
	/// </summary>
	public abstract class HexSimpleBufferStream {
		/// <summary>
		/// Constructor
		/// </summary>
		protected HexSimpleBufferStream() { }

		/// <summary>
		/// true if the content can change at any time
		/// </summary>
		public abstract bool IsVolatile { get; }

		/// <summary>
		/// true if it's a read-only stream
		/// </summary>
		public abstract bool IsReadOnly { get; }

		/// <summary>
		/// Gets the span
		/// </summary>
		public abstract HexSpan Span { get; }

		/// <summary>
		/// Gets the name. This could be the filename if the data was read from a file
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the page size of the underlying data store or 0 if it's unknown. Eg. if it's
		/// memory in some process, <see cref="Environment.SystemPageSize"/> can be returned
		/// here. The returned value must be 0 or a power of 2.
		/// </summary>
		public virtual ulong PageSize => 0;

		/// <summary>
		/// Reads bytes. Returns number of bytes read.
		/// </summary>
		/// <param name="position">Position</param>
		/// <param name="destination">Destination array</param>
		/// <param name="destinationIndex">Index</param>
		/// <param name="length">Length</param>
		/// <returns></returns>
		public abstract HexPosition Read(HexPosition position, byte[] destination, long destinationIndex, long length);

		/// <summary>
		/// Writes bytes. Returns number of bytes written.
		/// </summary>
		/// <param name="position">Position</param>
		/// <param name="source">Data</param>
		/// <param name="sourceIndex">Index</param>
		/// <param name="length">Length</param>
		/// <returns></returns>
		public abstract HexPosition Write(HexPosition position, byte[] source, long sourceIndex, long length);
	}
}