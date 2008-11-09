using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gitty.Lib.CommandLine
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

        public string[] GetFiles()
        {
            return GetFiles(Directory, "");
        }

        private static string[] GetFiles(DirectoryInfo directory, string parentPath)
        {

            if (directory == null) throw new ArgumentNullException("directory");
            if (parentPath == null) throw new ArgumentNullException("parentPath");

            if (parentPath.Length != 0 && !parentPath.EndsWith("/"))
                parentPath += "/";


            var results = new List<string>();
            

            var files = from file in directory.GetFiles()
                        select parentPath + file.Name;

            results.AddRange(files);

            foreach (var folder in directory.GetDirectories())
            {
                var name = parentPath + folder.Name;
                results.Add(name);
                results.AddRange(GetFiles(folder, name));
            }

            return results.ToArray();
        }
    }
}