﻿/*
 * Copyright (C) 2007, Robin Rosenberg <robin.rosenberg@dewire.com>
 * Copyright (C) 2008, Shawn O. Pearce <spearce@spearce.org>
 * Copyright (C) 2008, Kevin Thompson <kevin.thompson@theautomaters.com>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * - Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above
 *   copyright notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 * - Neither the name of the Git Development Community nor the
 *   names of its contributors may be used to endorse or promote
 *   products derived from this software without specific prior
 *   written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gitty.Core.Exceptions;
using Gitty.Core.Util;
using System.IO.Compression;

namespace Gitty.Core
{
    [Complete]
	public class PackFile : IEnumerable<PackIndex.MutableEntry>
	{
		private FileStream _stream;
        private long _packLastModified;

		private FileInfo _indexFile;
        private PackIndex _index;
        public PackIndex Index
        {
            get
            {
                lock (this)
                {
                    if (_index == null)
                        _index = PackIndex.Open(_indexFile);

                    return _index;
                }
            }
        }

		/**
		 * Construct a reader for an existing, pre-indexed packfile.
		 * 
		 * @param idxFile
		 *            path of the <code>.idx</code> file listing the contents.
		 * @param packFile
		 *            path of the <code>.pack</code> file holding the data.
		 */
		public PackFile(FileInfo idxFile, FileInfo packFile)
        {
            this._indexFile = idxFile;
            this._packLastModified = packFile.LastWriteTime.Ticks;
            this.File = packFile;
            _stream = this.File.Open(System.IO.FileMode.Open);
            ReadPackHeader();
		}

        internal PackedObjectLoader ResolveBase(long offset)
        {
            return Reader(offset);
        }

		/**
		 * Determine if an object is contained within the pack file.
		 * <p>
		 * For performance reasons only the index file is searched; the main pack
		 * content is ignored entirely.
		 * </p>
		 * 
		 * @param id
		 *            the object to look for. Must not be null.
		 * @return true if the object is in this pack; false otherwise.
		 */
		public bool HasObject(AnyObjectId id)
		{
            return Index.HasObject(id);
		}

        public FileInfo File { get; private set; }

        /**
         * Get an object from this pack.
         * 
         * @param curs
         *            temporary working space associated with the calling thread.
         * @param id
         *            the object to obtain from the pack. Must not be null.
         * @return the object loader for the requested object if it is contained in
         *         this pack; null if the object was not found.
         * @throws IOException
         *             the pack file or the index could not be read.
         */
        public PackedObjectLoader Get(AnyObjectId id)
        {
            long offset = Index.FindOffset(id);
            return (offset == -1) ? null : Reader(offset);
        }

		/**
		 * Close the resources utilized by this repository
		 */
		public void Close()
		{
            #warning TODO: UnpackedObjectCache.Purge(pack);
			_stream.Close();
		}



		/**
		 * Obtain the total number of objects available in this pack. This method
		 * relies on pack index, giving number of effectively available objects.
		 * 
		 * @return number of objects in index of this pack, likewise in this pack
		 */
		public long ObjectCount
		{
            get { return Index.ObjectCount; }
		}

		/**
		 * Search for object id with the specified start offset in associated pack
		 * (reverse) index.
		 *
		 * @param offset
		 *            start offset of object to find
		 * @return object id for this offset, or null if no object was found
		 */
		public ObjectId FindObjectForOffset(long offset)
		{
			return ReverseIndex.FindObject(offset);
		}

        public byte[] Decompress(long position, long totalSize)
        {
            byte[] dstbuf = new byte[totalSize];
            _stream.Seek(position, SeekOrigin.Begin);
            var deflate = new DeflateStream(_stream, CompressionMode.Decompress);
            deflate.Read(dstbuf, 0, dstbuf.Length);
            return dstbuf;
        }

        internal void CopyRawData(PackedObjectLoader loader, Stream stream)
        {
            long objectOffset = loader.objectOffset;
            long dataOffset = loader.DataOffset;
            int cnt = (int)(FindEndOffset(objectOffset) - dataOffset);

            if (this.Index.HasCRC32Support)
            {
                Crc32 crc = new Crc32();
                int headerCnt = (int)(dataOffset - objectOffset);
                _stream.Seek(objectOffset, SeekOrigin.Begin);
                var reader = new BinaryReader(_stream);
                var header = reader.ReadBytes(headerCnt);
                crc.Update(header);

                var output = new CheckedOutputStream(stream, crc);
                
                CopyToStream(dataOffset, cnt, output);
                
                long computed = crc.Value;

                ObjectId id = FindObjectForOffset(objectOffset);
                long expected = Index.FindCRC32(id);
                if (computed != expected)
                    throw new CorruptObjectException("Object at " + dataOffset + " in " + File.FullName + " has bad zlib stream");
            }
            else
            {
                try
                {
                    VerifyCompressed(dataOffset);
                }
                catch (FormatException fe)
                {
                    throw new CorruptObjectException("Object at " + dataOffset + " in " + File.FullName + " has bad zlib stream", fe);
                }
                
                CopyToStream(dataOffset, cnt, stream);
            }
        }

        private void CopyToStream(long dataOffset, int count, Stream stream)
        {
            _stream.Seek(dataOffset, SeekOrigin.Begin);
            var data = new byte[count];
            var bytesRead = _stream.Read(data, 0, data.Length);
            stream.Write(data, 0, bytesRead);
        }

        public void VerifyCompressed(long dataOffset)
        {
#warning TODO: Implement this
            //_stream.Seek(dataOffset, SeekOrigin.Begin);
            //var deflate = new DeflateStream(_stream, CompressionMode.Decompress);
            
        }

		public bool SupportsFastCopyRawData
		{
            get { return Index.HasCRC32Support; }
        }

        private void ReadPackHeader()
        {
            var reader = new BinaryReader(_stream);

            var sig = reader.ReadBytes(Constants.PackFile.Signature.Length);

            for (int k = 0; k < Constants.PackFile.Signature.Length; k++)
                if (sig[k] != Constants.PackFile.Signature[k])
                    throw new IOException("Not a PACK file.");
            
            var vers = reader.ReadUInt32();
            if (vers != 2 && vers != 3)
                throw new IOException("Unsupported pack version " + vers + ".");
            
            long objectCnt = reader.ReadUInt32();
            if (Index.ObjectCount != objectCnt)
                throw new IOException("Pack index object count mismatch; expected " + objectCnt + " found " + Index.ObjectCount + ": " + _stream.Name);
        }

        private PackedObjectLoader Reader(long objOffset)
        {
            var reader = new BinaryReader(_stream);
            long pos = objOffset;
            int p = 0;
            byte[] ib = reader.ReadBytes(Constants.ObjectId.Length);
            int c = ib[p++] & 0xff;
            int typeCode = (c >> 4) & 7;
            long dataSize = c & 15;
            int shift = 4;
            while ((c & 0x80) != 0)
            {
                c = ib[p++] & 0xff;
                dataSize += (c & 0x7f) << shift;
                shift += 7;
            }
            pos += p;

            switch ((ObjectType)typeCode)
            {
                case ObjectType.Commit:
                case ObjectType.Tree:
                case ObjectType.Blob:
                case ObjectType.Tag:
                    return new WholePackedObjectLoader(this, pos, objOffset, (ObjectType)typeCode, (int)dataSize);
                case ObjectType.OffsetDelta:
                    ib = reader.ReadBytes(Constants.ObjectId.Length);
                    p = 0;
                    c = ib[p++] & 0xff;
                    long ofs = c & 127;
                    while ((c & 0x80) != 0)
                    {
                        ofs += 1;
                        c = ib[p++] & 0xff;
                        ofs <<= 7;
                        ofs += (c & 127);
                    }
                    return new DeltaOfsPackedObjectLoader(this, pos + p, objOffset, (int)dataSize, objOffset - ofs);
                case ObjectType.ReferenceDelta:
                    ib = reader.ReadBytes(Constants.ObjectId.Length);
                    return new DeltaRefPackedObjectLoader(this, pos + ib.Length, objOffset, (int)dataSize, ObjectId.FromRaw(ib));

                default:
                    throw new IOException("Unknown object type " + typeCode + ".");
            }
        }

		private long FindEndOffset(long startOffset)
		{
            long maxOffset = _stream.Length - Constants.ObjectId.Length;
			return ReverseIndex.FindNextOffset(startOffset, maxOffset);
		}

        private PackReverseIndex _reverseIndex;
		private PackReverseIndex ReverseIndex
		{
            get
            {
                if (_reverseIndex == null)
                    _reverseIndex = new PackReverseIndex(this.Index);
                return _reverseIndex;
            }
		}

		#region IEnumerable<MutableEntry> Members

        /**
	     * Provide iterator over entries in associated pack index, that should also
	     * exist in this pack file. Objects returned by such iterator are mutable
	     * during iteration.
	     * <p>
	     * Iterator returns objects in SHA-1 lexicographical order.
	     * </p>
	     * 
	     * @return iterator over entries of associated pack index
	     * 
	     * @see PackIndex#iterator()
	     */

        public IEnumerator<PackIndex.MutableEntry> GetEnumerator()
		{
			return Index.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Index.GetEnumerator();
		}

		#endregion

	}
}
