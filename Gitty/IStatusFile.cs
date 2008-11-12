using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public enum StatusType
    {
        None,
        Repository,
        Index
    }

    public interface IStatusFile : ILsFilesFile, IRepositoryFile, IWorkingDirectoryFile
    {
        DiffMode Type { get; set; }
        bool Untracked { get; set; }
    }
}
