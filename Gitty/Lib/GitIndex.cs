using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class GitIndex
    {

        public Repository Repository { get; private set; }

        public class Entry
        {
            public string Name { get; private set; }
            public ObjectId ObjectId { get; private set; }
            internal bool IsModified(System.IO.DirectoryInfo root, bool p)
            {
                throw new NotImplementedException();
            }
        }
    }
}
