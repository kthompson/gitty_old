using System.IO;

namespace Gitty.Lib
{
    public class WorkingDirectory : GitPath
    {
        public WorkingDirectory(string directory, string subdirectory)
            : base(directory, subdirectory)
        {
        }

        public WorkingDirectory(string directory)
            : base(directory)
        {
        }

        public WorkingDirectory(DirectoryInfo directory)
            : base(directory)
        {
        }
    }
}
