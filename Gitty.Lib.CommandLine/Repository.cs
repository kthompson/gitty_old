using System.IO;
using Gitty.Lib.CLI;
using Gitty.Lib.CommandLine;

namespace Gitty.Lib.CLI
{
    public class Repository: GitPath, IRepository
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
            Index = new Index((IRepository)this);
        }

        public Repository(IWorkingDirectory directory)
            : this(Path.Combine(directory.ToString(), ".git"))
        {
        }


        public IIndex Index { get; private set; }
    }
}