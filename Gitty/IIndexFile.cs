using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IIndexFile : IFile
    {
        string IndexMode { get; set; }
        string IndexSha { get; set; }
    }
}
