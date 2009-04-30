/*
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
using Gitty.Core.Util;
using Gitty.Core.Exceptions;

namespace Gitty.Core
{
    [Complete]
    public class RefDatabase
    {
        public Repository Repository { get; private set; }

        private DirectoryInfo _gitDir;
        private DirectoryInfo _refsDir;
        private FileInfo _packedRefsFile;

        private Dictionary<String, Ref> looseRefs;
		private Dictionary<string, DateTime> looseRefsMTime;
		private Dictionary<string, string> looseSymRefs;
        private Dictionary<String, Ref> packedRefs;

        private DateTime packedRefsLastModified;
        private long packedRefsLength;

		private int lastRefModification;

		private int lastNotifiedRefModification;

		private int refModificationCounter;

		
        public RefDatabase(Repository repo)
        {
            this.Repository = repo;
            _gitDir = repo.Directory;
            _refsDir = PathUtil.CombineDirectoryPath(_gitDir, "refs");
            _packedRefsFile = PathUtil.CombineFilePath(_gitDir, "packed-refs");
            ClearCache();
        }

        public void ClearCache()
        {
            looseRefs = new Dictionary<String, Ref>();
			looseRefsMTime = new Dictionary<string, DateTime>();
            packedRefs = new Dictionary<String, Ref>();
			looseSymRefs = new Dictionary<string, string>();
            packedRefsLastModified = DateTime.MinValue;
            packedRefsLength = 0;
        }

        public void Create()
        {
            _refsDir.Create();
            PathUtil.CombineDirectoryPath(_refsDir, "heads").Create();
            PathUtil.CombineDirectoryPath(_refsDir, "tags").Create();
        }

        public ObjectId IdOf(string name)
        {
            Ref r = ReadRefBasic(name, 0);
            return (r != null) ? r.ObjectId : null;
        }

        public RefUpdate NewUpdate(string name)
        {
            Ref r = ReadRefBasic(name, 0);
            if (r == null)
                r = new Ref(Ref.Storage.New, name, null);
            return new RefUpdate(this, r, FileForRef(r.Name));
        }

        public void Stored(string origName, string name, ObjectId id, DateTime time)
        {
			lock(this)
			{
            	looseRefs.Add(name, new Ref(Ref.Storage.Loose, origName, name, id));
				looseRefsMTime.Add(name, time);
				SetModified();
			}
			this.OnChanged();
        }

        public void Link(string name, string target)
        {
			throw new NotImplementedException();
            byte[] content = Constants.Encoding.GetBytes("ref: " + target + "\n");
            LockFile lck = new LockFile(FileForRef(name));
            if (!lck.Lock())
                throw new ObjectWritingException("Unable to lock " + name);
            try
            {
                lck.Write(content);
            }
            catch (IOException ioe)
            {
                throw new ObjectWritingException("Unable to write " + name, ioe);
            }
            if (!lck.Commit())
                throw new ObjectWritingException("Unable to write " + name);
        }

		private void SetModified() {
			lastRefModification = refModificationCounter++;
		}

        public Ref ReadRef(String partialName)
        {
            RefreshPackedRefs();
            foreach (var searchPath in Constants.RefSearchPaths)
            {
                Ref r = ReadRefBasic(searchPath + partialName, 0);
                if (r != null && r.ObjectId != null)
                    return r;
            }
            return null;
        }

        public Dictionary<string, Ref> GetAllRefs()
        {
            return ReadRefs();
        }

        public Dictionary<String, Ref> GetTags()
        {
            Dictionary<String, Ref> tags = new Dictionary<String, Ref>();
            foreach (Ref r in ReadRefs().Values)
            {
                if (r.Name.StartsWith(Constants.RefsTags))
                    tags.Add(r.Name.Substring(Constants.RefsTags.Length), r);
            }
            return tags;
        }

        public Dictionary<String, Ref> GetBranches()
        {
            var branches = new Dictionary<String, Ref>();
            foreach (Ref r in ReadRefs().Values)
            {
                if (r.Name.StartsWith(Constants.RefsHeads))
                    branches.Add(r.Name.Substring(Constants.RefsTags.Length), r);
            }
            return branches;
        }

        public Dictionary<String, Ref> GetRemotes()
        {
            var remotes = new Dictionary<String, Ref>();
            foreach (Ref r in ReadRefs().Values)
            {
                if (r.Name.StartsWith(Constants.RefsRemotes))
                    remotes.Add(r.Name.Substring(Constants.RefsRemotes.Length), r);
            }
            return remotes;
        }

        private Dictionary<string, Ref> ReadRefs()
        {
            var avail = new Dictionary<String, Ref>();
            ReadPackedRefs(avail);
            ReadLooseRefs(avail, Constants.Refs, _refsDir);
       		try 
			{
				Ref r = ReadRefBasic(Constants.Head, 0);
				if (r != null && r.ObjectId != null)
					avail.AddOrReplace(Constants.Head, r);
			} catch (IOException e) {
				// ignore here
			}
			this.OnChanged();
			return avail;
            
		}

        private void ReadPackedRefs(Dictionary<string, Ref> avail)
        {
            RefreshPackedRefs();
            foreach (KeyValuePair<string, Ref> kv in packedRefs)
            {
                avail.Add(kv.Key, kv.Value);
            }
        }

        private void ReadLooseRefs(Dictionary<string, Ref> avail, string prefix, DirectoryInfo dir)
        {
            var entries = dir.GetFileSystemInfos();
            if (entries.Length == 0)
                return;

            foreach (FileSystemInfo ent in entries)
            {
                if(ent is DirectoryInfo)
                    ReadLooseRefs(avail, prefix + ent.Name, (DirectoryInfo)ent);
                else
                    ReadOneLooseRef(avail, prefix + "/" + ent.Name, prefix + "/" + ent.Name, ent);
            }

        }

        private void ReadOneLooseRef(Dictionary<string, Ref> avail, string origName, string refName, FileSystemInfo ent)
        {
            Ref reff = looseRefs.GetValue(refName);

            if (reff != null)
            {
				var cachedLastModified = looseRefsMTime[refName];
                if (cachedLastModified == ent.LastWriteTime)
                {
                    avail.Add(reff.Name, reff);
                    return;
                }
                looseRefs.Remove(refName);
				looseRefsMTime.Remove(refName);
            }

            // Assume its a valid loose reference we need to cache.
            //
            try
            {

                using (var reader = new StreamReader(ent.FullName))
                {
                    var str = reader.ReadToEnd().Trim();                    
                    var id = ObjectId.FromString(str);

                    if (id == null)
                        return;

                    reff = new Ref(Ref.Storage.Loose, origName, refName, id, null, false);

                    looseRefs.AddOrReplace(reff.Name, reff);
					looseRefsMTime.AddOrReplace(reff.Name, ent.LastWriteTime);
                    avail.AddOrReplace(reff.Name, reff);
                }
            }
            catch (FileNotFoundException)
            {
                // Deleted while we were reading? Its gone now!
                //
            }
            catch (IOException err)
            {
                // Whoops.
                //
                throw new GitException("Cannot read ref " + ent, err);
            }
        }

        private FileInfo FileForRef(string name)
        {
            if (name.StartsWith(Constants.Refs))
                return PathUtil.CombineFilePath(_refsDir, name.Substring(Constants.Refs.Length));
            return PathUtil.CombineFilePath(_gitDir, name);
        }

		private Ref ReadRefBasic(string name, int depth)
		{
			return ReadRefBasic(name, name,depth);
		}
		
        private Ref ReadRefBasic(string origName, string name, int depth)
        {
            // Prefer loose ref to packed ref as the loose
            // file can be more up-to-date than a packed one.
            //
			Ref r = looseRefs.GetValue(origName);
            FileInfo loose = FileForRef(name);
            DateTime mtime = loose.LastWriteTime;

            if (r != null)
            {
				var cachedLastModified = looseRefsMTime.GetValue(name);
                if (cachedLastModified != null && cachedLastModified == mtime)
                    return r;
                looseRefs.Remove(name);
				looseRefsMTime.Remove(name);
            }

            if (!loose.Exists)
            {
                // If last modified is 0 the file does not exist.
                // Try packed cache.
                //
				r = packedRefs.GetValue(name);
				if(r != null && !r.OriginalName.Equals(origName))
					r = new Ref(Ref.Storage.LoosePacked, origName, name, r.ObjectId);
                return r;
            }
			

            string line = null;
			try 
			{
				var cachedLastModified = looseRefsMTime.GetValue(name);
				if(cachedLastModified != null && cachedLastModified == mtime)
				{
					line = looseSymRefs.GetValue(name);
				}
				if(line == null)
				{
					line = ReadLine(loose);
					looseRefsMTime.AddOrReplace(name, mtime);
					looseSymRefs.AddOrReplace(name, line);
				}
			}
			catch (FileNotFoundException) 
			{
				return packedRefs.GetValue(name);	
			}

            if (string.IsNullOrEmpty(line))
			{
				looseRefs.Remove(origName);
				looseRefsMTime.Remove(origName);
                return new Ref(Ref.Storage.Loose,origName, name, null);
			}
			
            if (line.StartsWith("ref: "))
            {
                if (depth >= 5)
                    throw new IOException("Exceeded maximum ref depth of " + depth + " at " + name + ".  Circular reference?");

                String target = line.Substring("ref: ".Length);
                Ref rr = ReadRefBasic(target, depth + 1);
				var cachedMtime = looseRefsMTime.GetValue(name);
				if (cachedMtime != null && cachedMtime != mtime)
					SetModified();
				looseRefsMTime.AddOrReplace(name, mtime);
				if (rr == null)
					return new Ref(Ref.Storage.Loose, origName, target, null);
				if (!origName.Equals(rr.Name))
					rr = new Ref(Ref.Storage.LoosePacked, origName, rr.Name, rr.ObjectId, rr.PeeledObjectId, true);
				return rr; 
            }

			SetModified();
			
            ObjectId id;
            try
            {
                id = ObjectId.FromString(line);
            }
            catch (ArgumentException)
            {
                throw new IOException("Not a ref: " + name + ": " + line);
            }

            r = new Ref(Ref.Storage.Loose, origName, name, id);
			looseRefs.AddOrReplace(origName, r);
            r = new Ref(Ref.Storage.Loose, origName, id);
			looseRefs.AddOrReplace(name, r);
			looseRefsMTime.AddOrReplace(name, mtime);
            return r;
        }

        private void RefreshPackedRefs()
        {
            if (!_packedRefsFile.Exists)
                return;

            DateTime currTime = _packedRefsFile.LastWriteTime;
            long currLen = currTime == DateTime.MinValue ? 0 : _packedRefsFile.Length;
            if (currTime == packedRefsLastModified && currLen == packedRefsLength)
                return;
            if (currTime == DateTime.MinValue)
            {
                packedRefsLastModified = DateTime.MinValue;
                packedRefsLength = 0;
                packedRefs = new Dictionary<String, Ref>();
                return;
            }

            Dictionary<String, Ref> newPackedRefs = new Dictionary<String, Ref>();
            try
            {
                using(var b = OpenReader(_packedRefsFile))
                {
                    String p;
                    Ref last = null;
                    while ((p = b.ReadLine()) != null)
                    {
                        if (p[0] == '#')
                            continue;

                        if (p[0] == '^')
                        {
                            if (last == null)
                                throw new IOException("Peeled line before ref.");

                            ObjectId id = ObjectId.FromString(p.Substring(1));
                            last = new Ref(Ref.Storage.Packed, last.Name, last.Name, last.ObjectId, id, true);
                            newPackedRefs.Add(last.Name, last);
                            continue;
                        }

                        int sp = p.IndexOf(' ');
                        ObjectId id2 = ObjectId.FromString(p.Substring(0, sp));
                        String name = p.Substring(sp + 1);
                        last = new Ref(Ref.Storage.Packed, name, id2);
                        newPackedRefs.Add(last.Name, last);
                    }
                }
                packedRefsLastModified = currTime;
                packedRefsLength = currLen;
                packedRefs = newPackedRefs;
            }
            catch (FileNotFoundException)
            {
                // Ignore it and leave the new map empty.
                //
                packedRefsLastModified = DateTime.MinValue;
                packedRefsLength = 0;
                packedRefs = newPackedRefs;
            }
            catch (IOException e)
            {
                throw new GitException("Cannot read packed refs", e);
            }
        }

        /// <summary>
        /// Returns the object that this object points to if this is a commit.
        /// </summary>
        /// <param name="dref">The ref.</param>
        /// <returns></returns>
        internal Ref Peel(Ref dref)
        {
            if (dref.Peeled)
                return dref;

            ObjectId peeled = null;
            try
            {
                object target = Repository.MapObject(dref.ObjectId, dref.Name);

                while (target is Tag)
                {
                    Tag tag = (Tag)target;
                    peeled = tag.Id;

                    if (tag.TagType == Constants.ObjectTypes.Tag)
                        target = Repository.MapObject(tag.Id, dref.Name);
                    else
                        break;
                }
            }
            catch (IOException)
            {
                // Ignore a read error.  Callers will also get the same error
                // if they try to use the result of getPeeledObjectId.
            }
            return new Ref(dref.StorageFormat, dref.Name, dref.ObjectId, peeled, true);
        }

        private string ReadLine(FileInfo file)
        {
            using (BufferedReader br = OpenReader(file))
            {
                return br.ReadLine();
            }
        }

        private BufferedReader OpenReader(FileInfo file)
        {
            return new BufferedReader(file.FullName);
        }
		
		public event EventHandler<EventArgs> Changed;
		protected void OnChanged()
		{
			var handler = this.Changed;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
    }
}
