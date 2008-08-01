using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gitty.Lib;

namespace Gitty
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository repo = new Repository(new DirectoryInfo(@"..\..\..\.git"));
            Console.WriteLine(repo.Directory.FullName);
            Console.Read();
        }
    }
}
