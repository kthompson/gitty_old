using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gitty.Lib.CommandLine
{
    public class Status : Dictionary<string, IStatusFile>,  IStatus
    {
        public IGit Git { get; private set; }

        public Status(IGit git)
        {
            Git = git;

            foreach (var file in Git.LsFiles())
            {
                Add(file.Key , new StatusFile(file.Value));
            }

            foreach (var file in git.WorkingDirectory.GetFiles())
            {
                var relPath = GitPath.GetRelativePath(git.WorkingDirectory, file.FullName);
                if (!ContainsKey(relPath) && ((file.Attributes & FileAttributes.Directory) != FileAttributes.Directory))
                    Add(relPath, new StatusFile { Untracked = true, Path = relPath });
            }

            foreach (var pair in git.DiffFiles())
            {
                if (ContainsKey(pair.Key))
                {
                    var diffFile = pair.Value;
                    var currentFile = this[pair.Key];

                    currentFile.Path = diffFile.Path;
                    currentFile.IndexMode = diffFile.IndexMode;
                    currentFile.IndexSha = diffFile.IndexSha;
                    currentFile.Type = diffFile.Type;
                    currentFile.WorkingDirectoryMode = diffFile.WorkingDirectoryMode;
                    currentFile.WorkingDirectorySha = diffFile.WorkingDirectorySha;
                }
                else
                    Add(pair.Key, new StatusFile(pair.Value));
            }


            foreach (var pair in git.DiffIndex("HEAD"))
            {
                if (ContainsKey(pair.Key))
                {
                    var diffFile = pair.Value;
                    var currentFile = this[pair.Key];

                    currentFile.Path = diffFile.Path;
                    currentFile.IndexMode = diffFile.IndexMode;
                    currentFile.IndexSha = diffFile.IndexSha;
                    currentFile.Type = diffFile.Type;
                    currentFile.RepositoryMode = diffFile.RepositoryMode;
                    currentFile.RepositorySha = diffFile.RepositorySha;
                }
                else
                    Add(pair.Key, new StatusFile(pair.Value));
            }
        }




        public IEnumerable<IStatusFile> Changed
        {
            get { return this.Where(file => file.Value.Type == DiffMode.Modification).Select(file => file.Value); }
        }

        public IEnumerable<IStatusFile> Added
        {
            get { return this.Where(file => file.Value.Type == DiffMode.Add).Select(file => file.Value); }
        }

        public IEnumerable<IStatusFile> Deleted
        {
            get { return this.Where(file => file.Value.Type == DiffMode.Delete).Select(file => file.Value); }
        }

        public IEnumerable<IStatusFile> Untracked
        {
            get { return this.Where(file => file.Value.Untracked).Select(file => file.Value); }
        }
    }
}
