using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gitty
{
    public partial class Git
    {
        public static DirectoryInfo FindGitDirectory()
        {
            return FindGitDirectory(new DirectoryInfo("."));
        }

        public static DirectoryInfo FindGitDirectory(DirectoryInfo path)
        {
            if (!path.Exists)
                throw new FileNotFoundException();

            DirectoryInfo[] dirs = path.GetDirectories(".git");
            if (dirs.Length > 0)
                return dirs[0];
            else if(path.Parent == null)
                throw new FileNotFoundException();
            else
                return FindGitDirectory(path.Parent);
        }

        public static DirectoryInfo FindGitDirectory(FileInfo path)
        {
            if (!path.Exists)
                throw new FileNotFoundException();

            return FindGitDirectory(path.Directory);
        }
    }
}
