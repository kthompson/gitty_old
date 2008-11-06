using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class Index : GitPath
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

        public Index(Repository repo)
            : this(repo, "index")
        {
        }
    }
}
