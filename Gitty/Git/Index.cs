using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public class Index : Path
        {
            public Index(string path, bool checkPath)
                : base(path, checkPath)
            {
            }

            public Index(string path)
                : base(path)
            {
            }
        }
    }
}
