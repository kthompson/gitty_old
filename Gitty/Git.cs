using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public readonly string Version = "0.0.1";
        //public static 
        public static Base Bare(string gitDirectory)
        {
            return Git.Bare(gitDirectory, null);
        }

        public static Base Bare(string gitDirectory, Options options)
        {
            return Base.Bare(gitDirectory, options);
        }

        public static Base Open(string workingDirectory)
        {
            return Base.Open(workingDirectory, null);
        }

        public static Base Open(string workingDirectory, Options options)
        {
            return Base.Open(workingDirectory, options);
        }
    }
}
