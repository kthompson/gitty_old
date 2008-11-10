using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IRepositoryFile : IFile
    {
        string RepositoryMode { get; set; }
        string RepositorySha { get; set; }
    }
}
