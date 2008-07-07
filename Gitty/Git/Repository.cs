using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public class Repository : Path
        {
            public Repository(string path, bool checkPath)
                : base(path, checkPath)
            {
            }

            public Repository(string path)
                : base(path)
            {
            }
        }
    }
}
