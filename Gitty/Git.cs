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

        private Git(WorkingDirectory working, Repository repo, Index index)
        {
            Repository = repo;
            WorkingDirectory = working;
            Index = index;
        }

        
        private Git(WorkingDirectory working, Repository repo)
            : this(working, repo, new Index(Path.Combine(working, "index")))
        {
        }

        private Git(WorkingDirectory working)
            : this(working, new Repository(Path.Combine(working, ".git")))
        {
        }
        #endregion

        #region private static methods
        private static DirectoryInfo FindGitDirectory(DirectoryInfo path)
        {
            if (path == null || !path.Exists)
                return null;

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
        

        #endregion

        #region public member methods

        public static Git Open(DirectoryInfo directory)
        {
            DirectoryInfo dir = FindGitDirectory(directory);
            return dir == null ? null : new Git(new WorkingDirectory(dir));
        }

        public static Git Bare(DirectoryInfo directory)
        {
            throw new NotImplementedException();
        }

        public static Git Init(DirectoryInfo directory)
        {
            if (directory == null || !directory.Exists)
                return null;

            return GitLib.For(directory).Init();
        }

        public static Git Clone(DirectoryInfo directory, string repouri)
        {
            if (directory == null || string.IsNullOrEmpty(repouri))
                return null;

            return GitLib.For(directory).Clone(repouri);
        }

        #endregion



    }
}
