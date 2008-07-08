using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public FileInfo ToFile(AnyObjectId objectId)
        {
            string n = objectId.ToString();
            string d = n.Substring(0, 2);
            string f = n.Substring(2);
            for (int i = 0; i < this._objectsDirs.Count; ++i )
            {
                FileInfo ret = new FileInfo(Path.Combine(Path.Combine(this._objectsDirs[i].FullName, d), f));
                if (ret.Exists)
                    return ret;
            }

            return new FileInfo(Path.Combine(Path.Combine(this._objectsDirs[0].FullName, d), f));
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
            throw new NotImplementedException();
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

    }
}
