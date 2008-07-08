using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class RefDatabase
    {
        public RefDatabase(Repository repo)
        {
            this.Repository = repo;
        }

        public Repository Repository { get; private set; }

        internal void Create()
        {
            throw new NotImplementedException();
        }

        internal void Link(string p, string master)
        {
            throw new NotImplementedException();
        }
    }
}
