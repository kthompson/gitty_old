using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class Index : GitPath
    {
        public Index(string directory, string subdirectory)
            : base(directory, subdirectory)
        {
        }

        public Index(string directory)
            : base(directory)
        {
        }

        public Index(DirectoryInfo directory)
            : base(directory)
        {
        }
    }
}
