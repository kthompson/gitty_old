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
        public DirectoryInfo WorkingDirectory { get; private set; }

        #endregion

        #region constructors
        private Git(Repository repo)
        {
            Repository = repo;
        }

        private Git(DirectoryInfo working)
        {
            WorkingDirectory = working;
        }

        #endregion

        
        #region public member methods

        public static Git Open(DirectoryInfo directory)
        {
            Repository repo = Repository.Open(directory);
            return repo == null ? null : new Git(repo);
        }


        public static Git Init(DirectoryInfo directory)
        {
            Repository repo = Repository.Init(directory);
            return repo == null ? null : new Git(repo);
        }

        public static Git Clone(DirectoryInfo directory, string repouri)
        {
            Repository repo = Repository.Clone(directory, repouri);
            return repo == null ? null : new Git(repo);
        }

        #endregion



    }
}
