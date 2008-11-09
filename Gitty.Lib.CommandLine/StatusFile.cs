﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CommandLine
{
    public class StatusFile : IStatusFile
    {
        public StatusFile()
        {
        }

        public StatusFile(ILsFilesFile file)
        {
            ConstructIndex(file);
            ConstructFile(file);
            Stage = file.Stage;
        }

        public StatusFile(IDiffFilesFile file)
        {
            ConstructIndex(file);
            ConstructFile(file);
        }

        public StatusFile(IDiffIndexFile file)
        {
            ConstructRepository(file);
            ConstructIndex(file);
            ConstructFile(file);
        }

        private void ConstructRepository(IRepositoryFile file)
        {
            RepositoryMode = file.RepositoryMode;
            RepositorySha = file.RepositorySha;
        }

        private void ConstructIndex(IIndexFile file)
        {
            IndexMode = file.IndexMode;
            IndexSha = file.IndexSha;
        }

        private void ConstructFile(IFile file)
        {
            Path = file.Path;
        }

        public string Path { get; set; }
        public string IndexMode { get; set; }
        public string IndexSha { get; set; }
        public bool Stage { get; set; }
        public string RepositoryMode { get; set; }
        public string RepositorySha { get; set; }
        public StatusType Type { get; set; }
        public bool Untracked { get; set; }

    }
}
