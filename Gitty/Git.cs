using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gitty.Lib;

namespace Gitty
{
    public class Git 
    {
        #region properties
        public Repository Repository { get; private set; }
        public WorkingDirectory WorkingDirectory { get; private set; }
        public Index Index { get; private set; }
        #endregion

        #region constructors

        private Git(Repository repo)
            : this(null, repo, null)
        {
            if(repo != null && repo.Directory.Parent!= null)
            {
                Index = new Index(repo.Directory.Parent.FullName, "index");
            }
        }

        private Git(Repository repo, Index index)
            : this(null, repo, index)
        {
        }

        private Git(WorkingDirectory working, Repository repo, Index index)
        {
            Repository = repo;
            WorkingDirectory = working;
            Index = index;
        }

        private Git(WorkingDirectory working, Repository repo)
            : this(working, repo, new Index(working, "index"))
        {
        }

        private Git(WorkingDirectory working)
            : this(working, new Repository(working, ".git"))
        {
        }
        #endregion

        #region private static methods
        private static DirectoryInfo FindGitDirectory(DirectoryInfo path)
        {
            if (path == null || !path.Exists)
                return null;

            if (path.Name == ".git")
                return path;

            DirectoryInfo[] dirs = path.GetDirectories(".git");
            if (dirs.Length > 0)
                return dirs[0];

            return path.Parent == null ? null : FindGitDirectory(path.Parent);
        }
        private static DirectoryInfo FindGitDirectory(FileInfo path)
        {
            if (!path.Exists)
                return null;

            return FindGitDirectory(path.Directory);
        }

        private static IGit NewGit(Repository repo)
        {
            throw new NotImplementedException();
        }

        private static IGit NewGit(WorkingDirectory wd)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region public member methods

        public static IGit Open(DirectoryInfo directory)
        {
            DirectoryInfo dir = FindGitDirectory(directory);
            if(dir == null)
                return null;

            return NewGit(new Repository(dir));
        }

        public static IGit Bare(DirectoryInfo directory)
        {
            throw new NotImplementedException();
        }

        public static IGit Init(DirectoryInfo directory)
        {
            if (directory == null || !directory.Exists)
                return null;

            return NewGit(new WorkingDirectory(directory)).Init();
        }

        public static IGit Clone(DirectoryInfo directory, string repouri)
        {
            if (directory == null || string.IsNullOrEmpty(repouri))
                return null;

            return NewGit(new WorkingDirectory(directory)).Clone(repouri);
        }

        #endregion
    }
}
