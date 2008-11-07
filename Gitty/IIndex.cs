using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IIndex : IPath
    {
        void Checkout(string branch);
        bool Add(FileInfo file, string sha);
    }
}
