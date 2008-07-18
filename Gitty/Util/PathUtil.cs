using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gitty.Util
{
    public sealed class PathUtil
    {
        public static string Combine(params string[] paths)
        {
            if (paths.Length < 2)
                throw new ArgumentException("Must have at least two paths", "paths");

            string path = paths[0];
            for (int i = 0; i < paths.Length; ++i )
            {
                path = Path.Combine(path, paths[i]);
            }
            return path;
        }
    }
}
