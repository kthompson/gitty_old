using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gitty
{
    public partial class Git
    {

        public ICoreLib Core { get; private set; }

        #region constructors
        public Git(ICoreLib corelib)
        {
            if (corelib == null)
                throw new ArgumentNullException("corelib cannot be null.");

            this.Core = corelib;
        }

        public Git(Type corelib)
        {
            if(corelib == null)
                throw new ArgumentNullException("corelib cannot be null.");

            if (!typeof(ICoreLib).IsAssignableFrom(corelib))
                throw new ArgumentException(string.Format("{0} is not assignable as {1}.", corelib.ToString(), typeof(ICoreLib).ToString()));

            if (corelib.IsAbstract)
                throw new ArgumentException("CoreLib cannot be an abstract class.");

            if(!corelib.IsClass)
                throw new ArgumentException("CoreLib must be a class.");

            try
            {
                this.Core = (ICoreLib)Activator.CreateInstance(corelib);
            }
            catch(Exception e)
            {
                throw new ArgumentException("An error occurred while creating the specified ICoreLib class.", e);
            }
        }
        #endregion

        #region public static methods
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
        #endregion

        #region public member methods

        #endregion



    }
}
