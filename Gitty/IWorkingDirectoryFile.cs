using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IWorkingDirectoryFile : IFile
    {
        string WorkingDirectoryMode { get; }
        string WorkingDirectorySha { get; }
    }
}
