using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gitty.Util;

namespace Gitty.Lib
{
    public class Repository
    {

        public sealed class Constants
        {
            public static readonly string HeadsPrefix = "refs/heads";
            public static readonly string Head = "HEAD";
            public static readonly string Master = "master";
        }

        private RefDatabase _refs;
        private List<PackFile> _packs ;
        private Index _index;

        [Complete]
        public Repository(DirectoryInfo gitDirectory)
        {
            this.Directory = gitDirectory;
            _objectsDirs = new List<DirectoryInfo>();
            _objectsDirs = ReadObjectsDirs(Path.Combine(gitDirectory.FullName, "objects"), ref _objectsDirs);

            this.Config = new RepositoryConfig(this);
            _refs = new RefDatabase(this);
            _packs = new List<PackFile>();
            
            bool isExisting = _objectsDirs[0].Exists;
		    if (isExisting) {
			    this.Config.Load();
			    string repositoryFormatVersion = this.Config.GetString("core", null, "repositoryFormatVersion");

			    if (!"0".Equals(repositoryFormatVersion)) {
				    throw new IOException("Unknown repository format \""
						    + repositoryFormatVersion + "\"; expected \"0\".");
			    }
		    } else {
			    this.Config.Create();
		    }
		    if (isExisting)
			    ScanForPacks();
        }


        [Complete]
        public void Create()
        {
            if (this.Directory.Exists)
                throw new GitException("Unable to create repository. Directory already exists.");

            this.Directory.Create();
            this._refs.Create();

            this._objectsDirs[0].Create();
            new DirectoryInfo(Path.Combine(this._objectsDirs[0].FullName, "pack")).Create();
            new DirectoryInfo(Path.Combine(this._objectsDirs[0].FullName, "info")).Create();

            new DirectoryInfo(Path.Combine(this.Directory.FullName, "branches")).Create();
            new DirectoryInfo(Path.Combine(this.Directory.FullName, "remote")).Create();

            string master = Constants.HeadsPrefix + "/" + Constants.Master;

            this._refs.Link(Constants.Head, master);

            this.Config.Create();
            this.Config.Save();

        }

        public bool HasObject(AnyObjectId objectId)
        {
            int k = this._packs.Count;
            if (k > 0)
            {
                do
                {
                    if(this._packs[--k].HasObject(objectId))
                        return true;
                } while (k > 0);
            }
            return ToFile(objectId).Exists;
        }


        #region private methods

        private void ScanForPacks()
        {
            List<PackFile> p = new List<PackFile>();
            for (int i = 0; i < _objectsDirs.Count; ++i)
                ScanForPacks(new DirectoryInfo(Path.Combine(_objectsDirs[i].FullName, "pack")), p);

            _packs = p;

        }

        private void ScanForPacks(DirectoryInfo packDir, List<PackFile> packList) {
           // Must match "pack-[0-9a-f]{40}.idx" to be an index.
           IEnumerable<FileInfo> idxList = packDir.GetFiles().Where(file => file.Name.Length == 49 && file.Name.EndsWith(".idx") && file.Name.StartsWith("pack-"));
    
            if (idxList != null) {
            foreach (FileInfo indexName in idxList) {
                String n = indexName.FullName.Substring(0, indexName.FullName.Length - 4);
                FileInfo idxFile = new FileInfo(n + ".idx");
                FileInfo packFile = new FileInfo(n + ".pack");

                if (!packFile.Exists) {
                    // Sometimes C Git's http fetch transport leaves a
                    // .idx file behind and does not download the .pack.
                    // We have to skip over such useless indexes.
                    //
                    continue;
                }

                try {
                    packList.Add(new PackFile(this, idxFile, packFile));
                } catch (IOException) {
                    // Whoops. That's not a pack!
                    //
                }
            }
        }
    }

        private List<DirectoryInfo> ReadObjectsDirs(string objectsDir, ref List<DirectoryInfo> ret)
        {
            ret.Add(new DirectoryInfo(objectsDir));
            FileInfo altFile = new FileInfo(Path.Combine(Path.Combine(objectsDir, "info"), "alternates"));
            if (altFile.Exists)
            {
                using (StreamReader reader = altFile.OpenText())
                {
                    for (String alt = reader.ReadLine(); alt != null; alt = reader.ReadLine())
                    {
                        ReadObjectsDirs(Path.Combine(objectsDir, alt), ref ret);
                    }
                }
            }
            return ret;
        }

        #endregion

        #region properties
        private List<DirectoryInfo> _objectsDirs = new List<DirectoryInfo>();
        public DirectoryInfo ObjectsDirectory
        {
            get { return this._objectsDirs[0]; }
        }

        public DirectoryInfo Directory { get; private set; }
        public RepositoryConfig Config { get; private set; }
        #endregion

        public FileInfo ToFile(AnyObjectId objectId)
        {
            string n = objectId.ToString();
            string d = n.Substring(0, 2);
            string f = n.Substring(2);
            for (int i = 0; i < _objectsDirs.Count; ++i)
            {
                FileInfo ret = new FileInfo(PathUtil.Combine(_objectsDirs[i].FullName, d,f));
                if (ret.Exists)
                    return ret;
            }
            return new FileInfo(PathUtil.Combine(_objectsDirs[0].FullName, d, f));
        }

        public ObjectLoader OpenObject(AnyObjectId id)
        {
            return OpenObject(new WindowCursor(), id);
        }

        public ObjectLoader OpenObject(WindowCursor windowCursor, AnyObjectId id)
        {
            int k = _packs.Count;
            if(k > 0)
            {
                do
                {
                    ObjectLoader ol = _packs[--k].Get(windowCursor, id);
                    if (ol != null)
                        return ol;

                } while (k > 0);
            }
            try
            {
                return new UnpackedObjectLoader(this, id.ToObjectId());
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public ICollection<PackedObjectLoader> OpenObjectInAllPacks(AnyObjectId objectId, WindowCursor cursor)
        {
            ICollection<PackedObjectLoader> result = new LinkedList<PackedObjectLoader>();
            OpenObjectInAllPacks(objectId, result, cursor);
            return result;
        }

        private void OpenObjectInAllPacks(AnyObjectId objectId, ICollection<PackedObjectLoader> resultLoaders, WindowCursor cursor)
        {
            foreach (PackFile pack in _packs)
            {
                PackedObjectLoader loader = pack.Get(cursor, objectId);
                if (loader != null)
                    resultLoaders.Add(loader);
            }
        }

        public ObjectLoader OpenBlob(ObjectId id)
        {
            return OpenObject(id);
        }

        public ObjectLoader OpenTree(ObjectId id)
        {
            return OpenObject(id);
        }

        public Commit MapCommit(string resolveString)
        {
            ObjectId id = Resolve(resolveString);
            return id != null ? MapCommit(id) : null;
        }

        private Commit MapCommit(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public object MapCommit(ObjectId id, string refName)
        {
            ObjectLoader or = OpenObject(id);
            byte[] raw = or.Bytes;
            if (or.ObjectType == ObjectType.Tree)
                return MakeTree(id, raw);
            if (or.ObjectType == ObjectType.Commit)
                return MakeCommit(id, raw);
            if (or.ObjectType == ObjectType.Tag)
                return MakeTag(id, refName, raw);
            if (or.ObjectType == ObjectType.Blob)
                return raw;
            return null;

        }

        private object MakeTag(ObjectId id, string refName, byte[] raw)
        {
            throw new NotImplementedException();
        }

        private object MakeCommit(ObjectId id, byte[] raw)
        {
            throw new NotImplementedException();
        }

        private object MakeTree(ObjectId id, byte[] raw)
        {
            throw new NotImplementedException();
        }

        private ObjectId Resolve(string resolveString)
        {
            throw new NotImplementedException();
        }

    }
}
