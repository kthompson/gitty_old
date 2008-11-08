using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CommandLine
{
    public class StatusResult : LsFilesFile, IStatusResult
    {
        #region IStatusResult Members

        public StatusType Type { get; set; }

        public string RepositoryMode { get; set; }

        public string RepositorySha { get; set; }

        public bool Untracked { get; set; }

        #endregion
    }
}
