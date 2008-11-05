﻿using System.IO;

namespace Gitty.Lib
{
    public class Repository: GitPath
    {
        public Repository(string directory)
            : base(directory)
        {
        }

        public Repository(DirectoryInfo directory)
            : base(directory)
        {
        }
    }
}
