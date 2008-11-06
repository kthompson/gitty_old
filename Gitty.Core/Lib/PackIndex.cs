﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gitty.Extensions;
using Gitty.Util;

namespace Gitty.Lib
{
    [Complete]
    public abstract class PackIndex : IEnumerable<PackIndex.MutableEntry>
    {


        /**
 * Open an existing pack <code>.idx</code> file for reading.
 * <p>
 * The format of the file will be automatically detected and a proper access
 * implementation for that format will be constructed and returned to the
 * caller. The file may or may not be held open by the returned instance.
 * </p>
 * 
 * @param idxFile
 *            existing pack .idx to read.
 * @return access implementation for the requested file.
 * @throws FileNotFoundException
 *             the file does not exist.
 * @throws IOException
 *             the file exists but could not be read due to security errors,
 *             unrecognized data version, or unexpected data corruption.
 */
        public static PackIndex open(FileInfo idxFile)
        {
            
            FileStream fd = idxFile.OpenRead();
            try
            {
                byte[] hdr = new byte[8];
                NB.ReadFully(fd, hdr, 0, hdr.Length);
                if (IsTOC(hdr))
                {
                    int v = NB.DecodeInt32(hdr, 4);
                    switch (v)
                    {
                        case 2:
                            return new PackIndexV2(fd);
                        default:
                            throw new IOException("Unsupported pack index version " + v);
                    }
                }
                return new PackIndexV1(fd, hdr);
            }
            catch (IOException ioe)
            {
                throw new IOException("Unreadable pack index: " + idxFile.FullName, ioe);;
            }
            finally
            {
                try
                {
                    fd.Close();
                }
                catch (IOException)
                {
                    // ignore
                }
            }
        }

        private static bool IsTOC(byte[] h)
        {
            byte[] toc = PackIndexWriter.TOC;
            for (int i = 0; i < toc.Length; i++)
                if (h[i] != toc[i])
                    return false;
            return true;
        }

        /**
         * Determine if an object is contained within the pack file.
         * 
         * @param id
         *            the object to look for. Must not be null.
         * @return true if the object is listed in this index; false otherwise.
         */
        public bool HasObject(AnyObjectId id)
        {
            return FindOffset(id) != -1;
        }

        /**
         * Provide iterator that gives access to index entries. Note, that iterator
         * returns reference to mutable object, the same reference in each call -
         * for performance reason. If client needs immutable objects, it must copy
         * returned object on its own.
         * <p>
         * Iterator returns objects in SHA-1 lexicographical order.
         * </p>
         * 
         * @return iterator over pack index entries
         */

        #region IEnumerable<MutableEntry> Members

        public abstract IEnumerator<PackIndex.MutableEntry> GetEnumerator();

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion


        /**
	 * Obtain the total number of objects described by this index.
	 * 
	 * @return number of objects in this index, and likewise in the associated
	 *         pack that this index was generated from.
	 */
        public abstract long ObjectCount { get; protected set; }

        /**
         * Obtain the total number of objects needing 64 bit offsets.
         *
         * @return number of objects in this index using a 64 bit offset; that is an
         *         object positioned after the 2 GB position within the file.
         */
        public abstract long Offset64Count { get; protected set; }

        /**
         * Get ObjectId for the n-th object entry returned by {@link #iterator()}.
         * <p>
         * This method is a constant-time replacement for the following loop:
         *
         * <pre>
         * Iterator&lt;MutableEntry&gt; eItr = index.iterator();
         * int curPosition = 0;
         * while (eItr.hasNext() &amp;&amp; curPosition++ &lt; nthPosition)
         * 	eItr.next();
         * ObjectId result = eItr.next().toObjectId();
         * </pre>
         *
         * @param nthPosition
         *            position within the traversal of {@link #iterator()} that the
         *            caller needs the object for. The first returned
         *            {@link MutableEntry} is 0, the second is 1, etc.
         * @return the ObjectId for the corresponding entry.
         */
        public abstract ObjectId GetObjectId(long nthPosition);

        /**
         * Get ObjectId for the n-th object entry returned by {@link #iterator()}.
         * <p>
         * This method is a constant-time replacement for the following loop:
         *
         * <pre>
         * Iterator&lt;MutableEntry&gt; eItr = index.iterator();
         * int curPosition = 0;
         * while (eItr.hasNext() &amp;&amp; curPosition++ &lt; nthPosition)
         * 	eItr.next();
         * ObjectId result = eItr.next().toObjectId();
         * </pre>
         *
         * @param nthPosition
         *            unsigned 32 bit position within the traversal of
         *            {@link #iterator()} that the caller needs the object for. The
         *            first returned {@link MutableEntry} is 0, the second is 1,
         *            etc. Positions past 2**31-1 are negative, but still valid.
         * @return the ObjectId for the corresponding entry.
         */
        public ObjectId GetObjectId(int nthPosition)
        {
            if (nthPosition >= 0)
                return GetObjectId((long)nthPosition);
            int u31 = nthPosition.UnsignedRightShift(1);
            int one = nthPosition & 1;
            return GetObjectId((((long)u31) << 1) | (uint)one);
        }

        /**
         * Locate the file offset position for the requested object.
         * 
         * @param objId
         *            name of the object to locate within the pack.
         * @return offset of the object's header and compressed content; -1 if the
         *         object does not exist in this index and is thus not stored in the
         *         associated pack.
         */
        public abstract long FindOffset(AnyObjectId objId);

        /**
         * Retrieve stored CRC32 checksum of the requested object raw-data
         * (including header).
         *
         * @param objId
         *            id of object to look for
         * @return CRC32 checksum of specified object (at 32 less significant bits)
         * @throws MissingObjectException
         *             when requested ObjectId was not found in this index
         * @throws UnsupportedOperationException
         *             when this index doesn't support CRC32 checksum
         */
        public abstract long FindCRC32(AnyObjectId objId);


        /**
         * Check whether this index supports (has) CRC32 checksums for objects.
         *
         * @return true if CRC32 is stored, false otherwise
         */
        public abstract bool HasCRC32Support();

        public class MutableEntry : MutableObjectId
        {
            /**
             * Empty constructor. Object fields should be filled in later.
             */
            public MutableEntry()
                : base()
            {
            }

            /**
             * Returns offset for this index object entry
             * 
             * @return offset of this object in a pack file
             */
            public long Offset { get; set; }

            private MutableEntry(MutableEntry src)
                : base(src)
            {
                this.Offset = src.Offset;
            }

            /**
             * Returns mutable copy of this mutable entry.
             * 
             * @return copy of this mutable entry
             */
            public MutableEntry CloneEntry()
            {
                return new MutableEntry(this);
            }
        }

        protected abstract class EntriesIterator : IEnumerator<MutableEntry>
        {
            protected MutableEntry _objectId = new MutableEntry();

            protected long returnedNumber = 0;

            #region IEnumerator<MutableEntry> Members

            public MutableEntry Current
            {
                get { return _objectId; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return _objectId; }
            }

            public abstract bool MoveNext();
            public abstract void Reset();

            #endregion
        }



        internal static PackIndex Open(FileInfo idxFile)
        {
            throw new NotImplementedException();
        }
    }
}
