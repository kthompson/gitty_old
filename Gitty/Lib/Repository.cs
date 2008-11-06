using System.IO;

namespace Gitty.Lib
{
    public class Repository: GitPath
    {

        public Repository(string directory, string subdirectory)
            : this(Path.Combine(directory, subdirectory))
        {
        }

        public Repository(string directory)
            : this(new DirectoryInfo(directory))
        {
        }

        public Repository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public Repository(WorkingDirectory directory)
            : this(Path.Combine(directory, ".git"))
        {
        }
    }
}
