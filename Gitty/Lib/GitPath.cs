using System.IO;

namespace Gitty.Lib
{
    public class GitPath
    {
        public DirectoryInfo Directory { get; private set; }

        public GitPath(string directory)
            : this(new DirectoryInfo(directory))
        {
        }

        public GitPath(DirectoryInfo directory)
        {
            Directory = directory;
        }

        public static implicit operator string(GitPath value)
        {
            return value.Directory.FullName;
        }

        public static implicit operator DirectoryInfo(GitPath value)
        {
            return value.Directory;
        }

        public static explicit operator GitPath(DirectoryInfo value)
        {
            return new GitPath(value);
        }

        public static explicit operator GitPath(string value)
        {
            return new GitPath(value);
        }

        public override string ToString()
        {
            return Directory.FullName;
        }

    }
}
