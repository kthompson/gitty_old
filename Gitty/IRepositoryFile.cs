using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IRepositoryFile : IFile
    {
        string RepositoryMode { get; }
        string RepositorySha { get; }
    }
}
