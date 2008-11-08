using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CommandLine
{
    public class Status : Dictionary<string, IStatusFile>,  IStatus
    {
        public IGit Git { get; private set; }

        public Status(IGit git)
        {
            Git = git;

            var files = Git.LsFiles();

            var wdfiles = git.WorkingDirectory.Directory.GetFiles("**/*");
        }

     
       

        public List<IStatusFile> Changed
        {
            get { throw new System.NotImplementedException(); }
        }

        public List<IStatusFile> Added
        {
            get { throw new System.NotImplementedException(); }
        }

        public List<IStatusFile> Deleted
        {
            get { throw new System.NotImplementedException(); }
        }

        public List<IStatusFile> Untracked
        {
            get { throw new System.NotImplementedException(); }
        }

        public class StatusFile : IStatusFile
        {
            public StatusFile()
            {
            }

            public string Path { get; set; }
            public string IndexMode { get; set; }
            public string IndexSha { get; set; }
            public bool Stage { get; set; }
            public string RepositoryMode { get; set; }
            public string RepositorySha { get; set; }
            public StatusType Type { get; set; }
            public bool Untracked { get; set; }
        }
    }
}
