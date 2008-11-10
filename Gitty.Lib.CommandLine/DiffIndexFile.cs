using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CommandLine
{
    public class DiffIndexFile : IDiffIndexFile
    {
        public DiffIndexFile()
        {
            
        }

        public DiffIndexFile(string line)
        {
       
            var parts = line.Split('\t');
            var options = parts[0].Split(' ');

            Path = parts[1];
            RepositoryMode = options[0].Substring(1, 6);
            IndexMode = options[1];
            RepositorySha = options[2];
            IndexSha = options[3];
            Type = (DiffMode)options[4][0];
        }

        public string Path { get; set; }
        public string IndexMode { get; set; }
        public string IndexSha { get; set; }
        public string RepositoryMode { get; set; }
        public string RepositorySha { get; set; }
        public DiffMode Type { get; set; }
    }
}
