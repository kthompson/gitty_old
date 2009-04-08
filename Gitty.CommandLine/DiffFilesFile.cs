using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CommandLine
{
    public class DiffFilesFile : IDiffFilesFile
    {
        public DiffFilesFile(string line)
        {
            var parts = line.Split('\t');
            var options = parts[0].Split(' ');

            Path = parts[1];
            WorkingDirectoryMode = options[0].Substring(1, 7);
            IndexMode = options[1];
            WorkingDirectorySha = options[2];
            IndexSha = options[3];
            Type = (DiffMode)options[4][0];
        }

        public DiffFilesFile()
        {
        }

        public string WorkingDirectoryMode{ get; set; }
        public string WorkingDirectorySha{ get; set; }
        public string Path { get; set; }
        public string IndexMode { get; set; }
        public string IndexSha { get; set; }
        public DiffMode Type { get; set; }
    }
}
