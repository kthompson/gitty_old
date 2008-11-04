using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gitty.Lib
{
    public class Repository
    {
        #region properties
        public DirectoryInfo Directory { get; private set; }
        #endregion


        #region constructors
        private Repository(DirectoryInfo directory)
        {
            Directory = directory;
        }
        #endregion

        #region public static methods
        public static Repository Open(DirectoryInfo directory)
        {
            DirectoryInfo dir = FindGitDirectory(directory);
            return dir == null ? null : new Repository(dir);
        }

        public static Repository Init(DirectoryInfo directory)
        {
            if(directory == null || !directory.Exists)
                return null;

            return GitLib.For(directory).Init();
        }

        public static Repository Clone(DirectoryInfo directory, string repouri)
        {
            if (directory == null || string.IsNullOrEmpty(repouri))
                return null;

            return GitLib.For(directory).Clone(repouri);
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

    }
}
