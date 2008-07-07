using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public class WorkingDirectory : Path
        {
            public WorkingDirectory(string path, bool checkPath)
                : base(path, checkPath)
            {
            }

            public WorkingDirectory(string path)
                : base(path)
            {
            }
        }
    }
}
