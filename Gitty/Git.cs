using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gitty.Lib;

namespace Gitty
{
    public partial class Git
    {
        #region properties
        public Repository Repository { get; private set; }
        public DirectoryInfo WorkingDirectory { get; private set; }

        #endregion

        #region constructors
        private Git(Repository repo)
        {
            this.Repository = repo;
        }
        #endregion

        
        #region public member methods
        public static Git Open(DirectoryInfo directory)
        {
            Repository repo = Repository.Open(directory);
            if (repo == null)
                return null;
            return new Git(repo);
        }


        public static Git Init(DirectoryInfo directory)
        {
            Repository repo = Repository.Init(directory);
            if (repo == null)
                return null;
            return new Git(repo);
        }
        #endregion



    }
}
