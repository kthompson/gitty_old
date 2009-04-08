using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.CommandLine
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

        public StatusFile(IIndexFile file)
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

        public bool Stage { get; set; }

        public string IndexMode { get; set; }
        public string IndexSha { get; set; }
        
        public string RepositoryMode { get; set; }
        public string RepositorySha { get; set; }

        public string WorkingDirectoryMode { get; set; }
        public string WorkingDirectorySha { get; set; }

        public DiffMode Type { get; set; }
        public bool Untracked { get; set; }

        
    }
}
