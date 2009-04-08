using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gitty.CommandLine
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

        public FileSystemInfo[] GetFiles()
        {
            return GetFiles(Directory);
        }

        private static FileSystemInfo[] GetFiles(DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException("directory");

            var results = new List<FileSystemInfo>();
            
            results.AddRange(directory.GetFileSystemInfos());

            foreach (var folder in directory.GetDirectories())
            {
                results.AddRange(GetFiles(folder));
            }
            
            return results.ToArray();
        }
        public string GetRelativePath(string path)
        {
            return GetRelativePath(this, path);
        }

        public static string GetRelativePath(IPath directory, string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            var relativeToParts = new List<string>(directory.Directory.FullName.Split(Path.DirectorySeparatorChar));
            var pathParts = new List<string>(path.Split(Path.DirectorySeparatorChar));

            while (relativeToParts.Count > 0 && pathParts.Count > 0 && relativeToParts[0] == pathParts[0])
            {
                relativeToParts.RemoveAt(0);
                pathParts.RemoveAt(0);
            }

            while (relativeToParts.Count > 0)
            {
                relativeToParts.RemoveAt(0);
                pathParts.Insert(0, "..");
            }

            return string.Join("/", pathParts.ToArray());

        }
    }
}