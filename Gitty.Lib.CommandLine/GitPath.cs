using System;
using System.IO;

namespace Gitty.Lib
{
    public class GitPath : IPath
    {
        public DirectoryInfo Directory { get; private set; }
        public FileInfo File { get; private set; }

        public GitPath(FileInfo file)
            : this(file.Directory)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            File = file;
        }

        public GitPath(DirectoryInfo directory)
        {
            if(directory == null)
                throw new ArgumentNullException("directory");

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

        public static implicit operator FileInfo(GitPath value)
        {
            return value.File;
        }

        public override string ToString()
        {
            return File != null ? File.ToString() : Directory.FullName;
        }

    }
}
