using System.IO;

namespace Gitty.Lib
{
    public class WorkingDirectory : GitPath
    {
        public WorkingDirectory(string directory, string subdirectory)
            : this(Path.Combine(directory, subdirectory))
        {
        }

        public WorkingDirectory(string directory)
            : this(new DirectoryInfo(directory))
        {
        }

        public WorkingDirectory(DirectoryInfo directory)
            : base(directory)
        {
        }
    }
}
