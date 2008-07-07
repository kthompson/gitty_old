using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gitty
{
    public class File
    {
        public static string ExpandPath(string path)
        {
            return new FileInfo(path).FullName;
        }

        public static bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public static string Join(params string[] paths)
        {
            string path = paths[0];
            for(int i = 1; i < paths.Length; i++)
                path = System.IO.Path.Combine(path, paths[i]);

            return path;
        }

        
    }
}
