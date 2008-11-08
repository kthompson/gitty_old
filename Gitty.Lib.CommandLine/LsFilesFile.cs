using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CommandLine
{
    public class LsFilesFile : ILsFilesFile
    {
        public LsFilesFile()
        {
        }
        public LsFilesFile(string lsFilesResultLine)
        {
            string[] lineParts = lsFilesResultLine.Split('\t');

            string[] options = lineParts[0].Split(' ');
            Path = lineParts[1];
            IndexMode = options[0];
            IndexSha = options[1];
            Stage = options[2] == "0" ? false : true;
        }

        #region ILsFilesFile Members

        public string IndexMode{ get; set; }

        public string IndexSha { get; set; }

        public bool Stage { get; set; }

        public string Path { get; set; }

        #endregion
    }
}
