using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IDiffIndexFile : IIndexFile, IRepositoryFile
    {
        DiffMode Type { get; }
    }
}
