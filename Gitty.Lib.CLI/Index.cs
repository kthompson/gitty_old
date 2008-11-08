using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CLI
{
    public class Index : GitPath, IIndex
    {
        public Index(string directory, string file)
            : this(Path.Combine(directory, file))
        {
        }

        public Index(string file)
            : this(new FileInfo(file))
        {
        }

        public Index(FileInfo file)
            : base(file)
        {
        }

        public Index(IRepository repo)
            : this(repo.ToString(), "index")
        {
        }

        #region IIndex Members

        public void Checkout(string branch)
        {
            throw new NotImplementedException();
        }

        public bool Add(FileInfo file, string sha)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}