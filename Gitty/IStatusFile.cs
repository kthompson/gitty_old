using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public enum StatusType
    {
        Repository,
        Index
    }

    public interface IStatusFile : ILsFilesFile, IRepositoryFile
    {
        StatusType Type { get; }
        bool Untracked { get; }
    }
}
