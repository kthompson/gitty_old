/*
 * Copyright (C) 2007, Dave Watson <dwatson@mimvista.com>
 * Copyright (C) 2007, Robin Rosenberg <me@lathund.dewire.com>
 * Copyright (C) 2008, Robin Rosenberg <robin.rosenberg@dewire.com>
 * Copyright (C) 2008, Roger C. Soares <rogersoares@intelinet.com.br>
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
using System.IO;
using System.Linq;
using System.Text;
using Gitty.Core.Util;
using Gitty.Core.Exceptions;

namespace Gitty.Core
{
    /// <summary>
    /// A representation of the Git index.
    /// 
    /// The index points to the objects currently checked out or in the process of
    /// being prepared for committing or objects involved in an unfinished merge.
    /// 
    /// The abstract format is:<br/> path stage flags statdata SHA-1
    /// <ul>
    /// <li>Path is the relative path in the workdir</li>
    /// <li>stage is 0 (normally), but when
    /// merging 1 is the common ancestor version, 2 is 'our' version and 3 is 'their'
    /// version. A fully resolved merge only contains stage 0.</li>
    /// <li>flags is the object type and information of validity</li>
    /// <li>statdata is the size of this object and some other file system specifics,
    /// some of it ignored by JGit</li>
    /// <li>SHA-1 represents the content of the references object</li>
    /// </ul>
    /// 
    /// An index can also contain a tree cache which we ignore for now. We drop the
    /// tree cache when writing the index.
    /// </summary>
    public class GitIndex
    {

        /** Stage 0 represents merged entries. */
        public static int STAGE_0 = 0;

        private FileStream cache;

        private FileInfo cacheFile;

        // Index is modified
        private bool changed;

        // Stat information updated
        private bool statDirty;

        private Header header;

        private DateTime lastCacheTime;

        public Repository Repository { get; protected set; }

        private Dictionary<byte[], Entry> entries = new Dictionary<byte[], Entry>();

        /// <summary>
        /// Construct a Git index representation.
        /// </summary>
        /// <param name="db">
        /// A <see cref="Repository"/>
        /// </param>
        public GitIndex(Repository db)
        {
            this.Repository = db;
            this.cacheFile = PathUtil.CombineFilePath(db.Directory, "index");
        }

        //// <value>
        /// return true if we have modified the index in memory since reading it from disk
        /// </value>
        public bool IsChanged
        {
            get
            {
                return changed || statDirty;
            }
        }

        /// <summary>
        /// Reread index data from disk if the index file has been changed
        /// </summary>
        public void RereadIfNecessary()
        {
            if (cacheFile.Exists && cacheFile.LastWriteTime != lastCacheTime)
            {
                Read();
                this.OnChanged();
            }
        }

        /**
	     * Add the content of a file to the index.
	     *
	     * @param wd workdir
	     * @param f the file
	     * @return a new or updated index entry for the path represented by f
	     */
        public Entry Add(FileInfo wd, FileInfo f)
        {
            byte[] key = MakeKey(wd, f);
            Entry e = entries.GetValue(key);
            if (e == null)
            {
                e = new Entry(this, key, f, 0);                
                entries.AddOrReplace(key, e);
            }
            else
            {
                e.Update(f);
            }
            return e;
        }

        /**
	     * Remove a path from the index.
	     *
	     * @param wd
	     *            workdir
	     * @param f
	     *            the file whose path shall be removed.
	     * @return true if such a path was found (and thus removed)
	     * @throws IOException 
	     */
        public bool Remove(FileInfo wd, FileInfo f)
        {
            byte[] key = MakeKey(wd, f);
            return entries.Remove(key) != null;
        }

        /// <summary>
        /// Read the cache file into memory.
        /// </summary>
        public void Read()
        {
            changed = false;
            statDirty = false;
            if (!cacheFile.Exists)
            {
                header = null;
                entries.Clear();
                lastCacheTime = DateTime.MinValue;
                return;
            }
            using(cache = cacheFile.OpenRead())
            {
                var buffer = new MemoryStream();
                long j = buffer.CopyFrom(cache);

                if (j != buffer.Capacity)
                    throw new IOException("Could not read index in one go, only " + j + " out of " + buffer.Capacity + " read");
                buffer.Position = 0;
                header = new Header(buffer);
                entries.Clear();
                for (int i = 0; i < header.Entries; ++i)
                {
                    Entry entry = new Entry(this, buffer);
                    entries.AddOrReplace(entry.NameUTF8, entry);
                }
                lastCacheTime = cacheFile.LastWriteTime;
            }
        }

        /// <summary>
        /// Write content of index to disk.
        /// </summary>
        public void Write()
        {
            throw new NotImplementedException();
        }

        static bool File_canExecute(FileInfo f)
        {
            throw new NotImplementedException();
        }

        static bool File_setExecute(FileInfo f, bool value)
        {
            throw new NotImplementedException();
        }

        static bool File_hasExecute()
        {
            throw new NotImplementedException();
        }

        static byte[] MakeKey(FileInfo wd, FileInfo f)
        {
            throw new NotImplementedException();
        }

        bool? filemode;
        private bool config_filemode()
        {
            // temporary til we can actually set parameters. We need to be able
            // to change this for testing.
            if (filemode != null)
                return filemode.Value;
            RepositoryConfig config = this.Repository.Config;
            return config.GetBoolean("core", null, "filemode", true);
        }

        public class Entry
        {
            private long ctime;

            private long mtime;

            private int dev;

            private int ino;

            private int uid;

            private int gid;

            private short flags;

            private byte[] name;

            private GitIndex index;

            public Entry(GitIndex idx, byte[] key, FileInfo f, int stage)
            {
                index = idx;
                ctime = f.LastWriteTime.Ticks * 1000000L;
                mtime = ctime; // we use same here
                dev = -1;
                ino = -1;
                if (index.config_filemode() && File_canExecute(f))
                    this.ModeBits = FileMode.ExecutableFile.Bits;
                else
                    this.ModeBits = FileMode.RegularFile.Bits;
                uid = -1;
                gid = -1;
                this.Size = (int)f.Length;
                ObjectWriter writer = new ObjectWriter(this.index.Repository);
                this.ObjectId = writer.WriteBlob(f);
                name = key;
                flags = (short)((stage << 12) | name.Length); // TODO: fix flags
            }

            public Entry(GitIndex idx, byte[] key, FileInfo f, int stage, byte[] newContent)
            {
                index = idx;
                ctime = f.LastWriteTime.Ticks * 1000000L;
                mtime = ctime; // we use same here
                dev = -1;
                ino = -1;
                if (index.config_filemode() && File_canExecute(f))
                    this.ModeBits = FileMode.ExecutableFile.Bits;
                else
                    ModeBits = FileMode.RegularFile.Bits;
                uid = -1;
                gid = -1;
                this.Size = newContent.Length;
                ObjectWriter writer = new ObjectWriter(this.index.Repository);
                this.ObjectId = writer.WriteBlob(newContent);
                name = key;
                flags = (short)((stage << 12) | name.Length); // TODO: fix flags
            }

            public Entry(GitIndex idx, TreeEntry f, int stage)
            {
                index = idx;
                ctime = -1; // hmm
                mtime = -1;
                dev = -1;
                ino = -1;
                this.ModeBits = f.Mode.Bits;
                uid = -1;
                gid = -1;
                try
                {
                    this.Size = (int)this.index.Repository.OpenBlob(f.Id).Size;
                }
                catch (IOException e)
                {
                    //e.printStackTrace();
                    this.Size = -1;
                }
                this.ObjectId = f.Id;
                name = f.FullNameUTF8;
                flags = (short)((stage << 12) | name.Length); // TODO: fix flags
            }

            public Entry(GitIndex idx, MemoryStream b)
            {
                index = idx;
                var reader = new BinaryReader(b);
            
                long startposition = b.Position;
                ctime = reader.ReadInt32() * 1000000000L + (reader.ReadInt32() % 1000000000L);
                mtime = reader.ReadInt32() * 1000000000L + (reader.ReadInt32() % 1000000000L);
                dev = reader.ReadInt32();
                ino = reader.ReadInt32();
                this.ModeBits = reader.ReadInt32();
                uid = reader.ReadInt32();
                gid = reader.ReadInt32();
                this.Size = reader.ReadInt32();
                byte[] sha1bytes = reader.ReadBytes(Constants.ObjectId.Length);
                this.ObjectId = ObjectId.FromRaw(sha1bytes);
                flags = reader.ReadInt16();
                name = reader.ReadBytes(flags & 0xFFF);

                
                b.Position = startposition
                                + ((8 + 8 + 4 + 4 + 4 + 4 + 4 + 4 + 20 + 2 + name.Length + 8) & ~7);
                
            }

            /**
             * Update this index entry with stat and SHA-1 information if it looks
             * like the file has been modified in the workdir.
             *
             * @param f
             *            file in work dir
             * @return true if a change occurred
             * @throws IOException
             */
            public bool Update(FileInfo f)
            {
                long lm = f.LastWriteTime.Ticks * 1000000L;
                bool modified = mtime != lm;
                mtime = lm;
                if (this.Size != f.Length)
                    modified = true;
                if (index.config_filemode())
                {
                    if (File_canExecute(f) != FileMode.ExecutableFile.Equals(this.ModeBits))
                    {
                        this.ModeBits = FileMode.ExecutableFile.Bits;
                        modified = true;
                    }
                }
                if (modified)
                {
                    this.Size = (int)f.Length;
                    ObjectWriter writer = new ObjectWriter(this.index.Repository);
                    ObjectId newsha1 = this.ObjectId = writer.WriteBlob(f);
                    if (!newsha1.Equals(this.ObjectId))
                        modified = true;
                    this.ObjectId = newsha1;
                }
                return modified;
            }

            /**
             * Update this index entry with stat and SHA-1 information if it looks
             * like the file has been modified in the workdir.
             *
             * @param f
             *            file in work dir
             * @param newContent
             *            the new content of the file
             * @return true if a change occurred
             * @throws IOException
             */
            public bool Update(FileInfo f, byte[] newContent)
            {
                bool modified = false;
                this.Size = newContent.Length;
                ObjectWriter writer = new ObjectWriter(this.index.Repository);
                ObjectId newsha1 = this.ObjectId = writer.WriteBlob(newContent);
                if (!newsha1.Equals(this.ObjectId))
                    modified = true;
                this.ObjectId = newsha1;
                return modified;
            }

            void Write(MemoryStream buf)
            {
                long startposition = buf.Position;
                var writer = new BinaryWriter(buf);

                writer.Write((int)(ctime / 1000000000L));
                writer.Write((int)(ctime % 1000000000L));
                writer.Write((int)(mtime / 1000000000L));
                writer.Write((int)(mtime % 1000000000L));
                writer.Write(dev);
                writer.Write(ino);
                writer.Write(this.ModeBits);
                writer.Write(uid);
                writer.Write(gid);
                writer.Write(this.Size);
                this.ObjectId.CopyTo(buf);
                writer.Write(flags);
                writer.Write(name);
                long end = startposition
                        + ((8 + 8 + 4 + 4 + 4 + 4 + 4 + 4 + 20 + 2 + name.Length + 8) & ~7);
                long remain = end - buf.Position;
                while (remain-- > 0)
                    writer.Write((byte)0);
            }

            /**
             * Check if an entry's content is different from the cache, 
             * 
             * File status information is used and status is same we
             * consider the file identical to the state in the working
             * directory. Native git uses more stat fields than we
             * have accessible in Java.
             * 
             * @param wd working directory to compare content with
             * @return true if content is most likely different.
             */
            public bool IsModified(DirectoryInfo wd)
            {
                return IsModified(wd, false);
            }

            /**
             * Check if an entry's content is different from the cache, 
             * 
             * File status information is used and status is same we
             * consider the file identical to the state in the working
             * directory. Native git uses more stat fields than we
             * have accessible in Java.
             * 
             * @param wd working directory to compare content with
             * @param forceContentCheck True if the actual file content
             * should be checked if modification time differs.
             * 
             * @return true if content is most likely different.
             */
            public bool IsModified(FileSystemInfo wd, bool forceContentCheck)
            {

                if (IsAssumedValid)
                    return false;

                if (IsUpdateNeeded)
                    return true;

                FileInfo file = GetFile((DirectoryInfo)wd);
                if (!file.Exists)
                    return true;

                // JDK1.6 has file.canExecute
                // if (file.canExecute() != FileMode.EXECUTABLE_FILE.equals(mode))
                // return true;
                int exebits = FileMode.ExecutableFile.Bits ^ FileMode.RegularFile.Bits;

                if (index.config_filemode() && FileMode.ExecutableFile.Equals(this.ModeBits))
                {
                    if (!File_canExecute(file) && File_hasExecute())
                        return true;
                }
                else
                {
                    if (FileMode.RegularFile.Equals(this.ModeBits & ~exebits))
                    {
                        if (!file.IsFile())
                            return true;
                        if (index.config_filemode() && File_canExecute(file) && File_hasExecute())
                            return true;
                    }
                    else
                    {
                        if (FileMode.Symlink.Equals(this.ModeBits))
                        {
                            return true;
                        }
                        else
                        {
                            if (FileMode.Tree.Equals(this.ModeBits))
                            {
                                if (!file.IsDirectory())
                                    return true;
                            }
                            else
                            {
                                Console.WriteLine("Does not handle mode " + this.ModeBits + " (" + file + ")");
                                return true;
                            }
                        }
                    }
                }

                if (file.Length != this.Size)
                    return true;

                // Git under windows only stores seconds so we round the timestamp
                // Java gives us if it looks like the timestamp in index is seconds
                // only. Otherwise we compare the timestamp at millisecond prevision.
                long javamtime = mtime / 1000000L;
                long lastm = file.LastWriteTime.Ticks;
                if (javamtime % 1000 == 0)
                    lastm = lastm - lastm % 1000;
                if (lastm != javamtime)
                {
                    if (!forceContentCheck)
                        return true;

                    using (var input = file.OpenRead())
                    {
                        try
                        {
                            ObjectWriter objectWriter = new ObjectWriter(this.index.Repository);
                            ObjectId newId = objectWriter.ComputeBlobSha1(file.Length, input);
                            bool ret = !newId.Equals(this.ObjectId);
                            return ret;
                        }
                        catch (IOException e)
                        {
                            //e.printStackTrace();
                        }
                    }

                }
                return false;
            }

            // for testing
            void ForceRecheck()
            {
                mtime = -1;
            }

            private FileInfo GetFile(DirectoryInfo wd)
            {
                return PathUtil.CombineFilePath(wd, GetName());
            }

            public override string ToString()
            {
                return GetName() + "/SHA-1(" + this.ObjectId.Name() + ")/M:"
                        + new DateTime(ctime / 1000000L) + "/C:"
                        + new DateTime(mtime / 1000000L) + "/d" + dev + "/i" + ino
                        + "/m" + Convert.ToString(this.ModeBits, 8) + "/u" + uid + "/g"
                        + gid + "/s" + this.Size + "/f" + flags + "/@" + Stage;
            }

            /**
             * @return path name for this entry
             */
            public String GetName()
            {
                return Encoding.UTF8.GetString(NameUTF8);
            }

            /**
             * @return path name for this entry as byte array, hopefully UTF-8 encoded
             */
            public byte[] NameUTF8 { get; protected set; }

            /**
             * @return SHA-1 of the entry managed by this index
             */
            public ObjectId ObjectId { get; protected set; }

            /**
             * @return the stage this entry is in
             */
            public int Stage
            {
                get
                {
                    return (flags & 0x3000) >> 12;
                }
            }

            /**
             * @return size of disk object
             */
            public int Size { get; protected set; }

            /**
             * @return true if this entry shall be assumed valid
             */
            public bool IsAssumedValid
            {
                get
                {
                    return (flags & 0x8000) != 0;
                }
            }

            /**
             * @return true if this entry should be checked for changes
             */
            public bool IsUpdateNeeded
            {
                get
                {
                    return (flags & 0x4000) != 0;
                }
            }

            /**
             * Set whether to always assume this entry valid
             *
             * @param assumeValid true to ignore changes
             */
            public void SetAssumeValid(bool assumeValid)
            {
                throw new NotImplementedException();
                //if (assumeValid)
                //    flags |= 0x8000;
                //else
                //    flags &= ~0x8000;
            }

            /**
             * Set whether this entry must be checked
             *
             * @param updateNeeded
             */
            public void SetUpdateNeeded(bool updateNeeded)
            {
                if (updateNeeded)
                    flags |= 0x4000;
                else
                    flags &= ~0x4000;
            }

            /**
             * Return raw file mode bits. See {@link FileMode}
             * @return file mode bits
             */
            public int ModeBits { get; protected set; }
        }

        sealed class Header
        {
            private int signature;

            private int version;

            public int Entries{ get; private set; }

            public Header(MemoryStream map)
            {
                Read(map);
            }

            private void Read(MemoryStream buf)
            {
                var reader = new BinaryReader(buf);
                signature = reader.ReadInt32();
                version = reader.ReadInt32();
                Entries = reader.ReadInt32();
                if (signature != 0x44495243)
                    throw new CorruptObjectException("Index signature is invalid: " + signature);
                if (version != 2)
                    throw new CorruptObjectException("Unknown index version (or corrupt index):" + version);
            }

            void Write(MemoryStream buf)
            {
                var writer = new BinaryWriter(buf);
                //buf.order(ByteOrder.BIG_ENDIAN);
                writer.Write(signature);
                writer.Write(version);
                writer.Write(Entries);
            }

            Header(Dictionary<byte[], Entry> entryset)
            {
                signature = 0x44495243;
                version = 2;
                Entries = entryset.Count;
            }
        }

        #region events
        public event EventHandler<IndexChangedEventArgs> Changed;
        protected void OnChanged()
        {
            var handler = this.Changed;
            if (handler != null)
                handler(this, new IndexChangedEventArgs(this.Repository));
        }
        #endregion


    }
}
