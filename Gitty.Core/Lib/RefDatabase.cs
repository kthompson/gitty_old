﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gitty.Util;
using Gitty.Exceptions;

namespace Gitty.Lib
{
    [Complete]
    public class RefDatabase
    {

        public class Constants
        {
            public readonly static Encoding Encoding = Encoding.UTF8;
            public readonly static string RefsSlash = "refs/";
            public readonly static string TagsSlash = Tag.Constants.TagsPrefix + "/";
            public readonly static string HeadsSlash = Repository.Constants.HeadsPrefix + "/";
            public readonly static string[] RefSearchPaths = { "", RefsSlash, TagsSlash, HeadsSlash, Repository.Constants.RemotesPrefix + "/" };
        }

        public Repository Repository { get; private set; }

        private DirectoryInfo _gitDir;
        private DirectoryInfo _refsDir;
        private DirectoryInfo _packedRefsDir;

        private FileInfo packedRefsFile;

        private Dictionary<String, CachedRef> looseRefs;
        private Dictionary<String, Ref> packedRefs;

        private DateTime packedRefsLastModified;
        private long packedRefsLength;

        private string[] refSearchPaths = { "" };

        public RefDatabase(Repository repo)
        {
            this.Repository = repo;
            _gitDir = repo.Directory;
            _refsDir = PathUtil.CombineDirectoryPath(_gitDir, "refs");
            _packedRefsDir = PathUtil.CombineDirectoryPath(_gitDir, "packed-refs");
            ClearCache();
        }

        public void ClearCache()
        {
            looseRefs = new Dictionary<String, CachedRef>();
            packedRefs = new Dictionary<String, Ref>();
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

        public void Stored(String name, ObjectId id, DateTime time)
        {
            looseRefs.Add(name, new CachedRef(Ref.Storage.Loose, name, id, time));
        }

        public void Link(string name, string target)
        {
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


        public Ref ReadRef(String partialName)
        {
            RefreshPackedRefs();
            for (int k = 0; k < refSearchPaths.Length; k++)
            {
                Ref r = ReadRefBasic(refSearchPaths[k] + partialName, 0);
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
                if (r.Name.StartsWith(Constants.TagsSlash))
                    tags.Add(r.Name.Substring(Constants.TagsSlash.Length), r);
            }
            return tags;
        }

        private Dictionary<string, Ref> ReadRefs()
        {
            Dictionary<String, Ref> avail = new Dictionary<String, Ref>();
            ReadPackedRefs(avail);
            ReadLooseRefs(avail, Constants.RefsSlash, _refsDir);
            ReadOneLooseRef(avail, Repository.Constants.Head, PathUtil.CombineFilePath(_gitDir, Repository.Constants.Head));
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

            FileSystemInfo[] entries = dir.GetFileSystemInfos();
            if (entries == null)
                return;

            foreach (FileSystemInfo ent in entries)
            {
                String entName = ent.Name;
                if (".".Equals(entName) || "..".Equals(entName))
                    continue;
                ReadOneLooseRef(avail, prefix + entName, ent);
            }


        }

        private void ReadOneLooseRef(Dictionary<string, Ref> avail, string refName, FileSystemInfo ent)
        {
            CachedRef reff = looseRefs[refName];
            if (reff != null)
            {
                if (reff.LastModified == ent.LastWriteTime)
                {
                    avail.Add(reff.Name, reff);
                    return;
                }
                looseRefs.Remove(refName);
            }

            // Recurse into the directory.
            //
            if ((ent.Attributes | FileAttributes.Directory) == FileAttributes.Directory)
            {
                ReadLooseRefs(avail, refName + "/", new DirectoryInfo(ent.FullName));
                return;
            }

            // Assume its a valid loose reference we need to cache.
            //
            try
            {
                FileStream inn = new FileStream(ent.FullName, System.IO.FileMode.Open);
                try
                {
                    ObjectId id;
                    try
                    {
                        byte[] str = new byte[ObjectId.Constants.ObjectIdLength * 2];
                        NB.ReadFully(inn, str, 0, str.Length);
                        id = ObjectId.FromString(str, 0);
                    }
                    catch (EndOfStreamException)
                    {
                        // Its below the minimum length needed. It could
                        // be a symbolic reference.
                        //
                        return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // It is not a well-formed ObjectId. It may be
                        // a symbolic reference ("ref: ").
                        //
                        return;
                    }

                    reff = new CachedRef(Ref.Storage.Loose, refName, id, ent.LastWriteTime);
                    looseRefs.Add(reff.Name, reff);
                    avail.Add(reff.Name, reff);
                }
                finally
                {
                    inn.Close();
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
            if (name.StartsWith(Constants.RefsSlash))
                return PathUtil.CombineFilePath(_refsDir, name.Substring(Constants.RefsSlash.Length));
            return PathUtil.CombineFilePath(_gitDir, name);
        }

        private Ref ReadRefBasic(string name, int depth)
        {
            // Prefer loose ref to packed ref as the loose
            // file can be more up-to-date than a packed one.
            //
            CachedRef reff = looseRefs[name];
            FileInfo loose = FileForRef(name);
            DateTime mtime = loose.LastWriteTime;

            if (reff != null)
            {
                if (reff.LastModified == mtime)
                    return reff;
                looseRefs.Remove(name);
            }

            if (!loose.Exists)
            {
                // If last modified is 0 the file does not exist.
                // Try packed cache.
                //
                return packedRefs[name];
            }

            String line;
            try
            {
                line = ReadLine(loose);
            }
            catch (FileNotFoundException)
            {
                return packedRefs[name];
            }

            if (line == null || line.Length == 0)
                return new Ref(Ref.Storage.Loose, name, null);

            if (line.StartsWith("ref: "))
            {
                if (depth >= 5)
                {
                    throw new IOException("Exceeded maximum ref depth of " + depth
                            + " at " + name + ".  Circular reference?");
                }

                String target = line.Substring("ref: ".Length);
                Ref r = ReadRefBasic(target, depth + 1);
                return r != null ? r : new Ref(Ref.Storage.Loose, target, null);
            }

            ObjectId id;
            try
            {
                id = ObjectId.FromString(line);
            }
            catch (ArgumentException)
            {
                throw new IOException("Not a ref: " + name + ": " + line);
            }

            reff = new CachedRef(Ref.Storage.Loose, name, id, mtime);
            looseRefs.Add(name, reff);
            return reff;
        }

        private void RefreshPackedRefs()
        {
            DateTime currTime = packedRefsFile.LastWriteTime;
            long currLen = currTime == DateTime.MinValue ? 0 : packedRefsFile.Length;
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
                BufferedReader b = OpenReader(packedRefsFile);
                try
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
                            last = new Ref(Ref.Storage.Packed, last.Name, last.ObjectId, id);
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
                finally
                {
                    b.Close();
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

        private class CachedRef : Ref
        {
            public DateTime LastModified { get; private set; }

            public CachedRef(Storage st, String refName, ObjectId id, DateTime mtime)
                : base(st, refName, id)
            {
                this.LastModified = mtime;
            }
        }

    }
}
