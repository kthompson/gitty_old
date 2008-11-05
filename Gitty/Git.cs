using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gitty
{
    public partial class Git
    {


        #region constructors
        private Git()
        {
            
        }
        #endregion

        #region public static methods
        private static DirectoryInfo FindGitDirectory()
        {
            return FindGitDirectory(new DirectoryInfo("."));
        }

        private static DirectoryInfo FindGitDirectory(DirectoryInfo path)
        {
            if (!path.Exists)
                throw new FileNotFoundException();

            DirectoryInfo[] dirs = path.GetDirectories(".git");
            if (dirs.Length > 0)
                return dirs[0];
            else if(path.Parent == null)
                return null;
            else
                return FindGitDirectory(path.Parent);
        }

        private static DirectoryInfo FindGitDirectory(FileInfo path)
        {
            if (!path.Exists)
                return null;

            return FindGitDirectory(path.Directory);
        }
        #endregion

        #region public member methods

        #endregion



    }
}
