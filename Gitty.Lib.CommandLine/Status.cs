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

                }
                else
                {
                    Add(pair.Key, new StatusFile(pair.Value));
                }
            }


            foreach (var pair in git.DiffIndex("HEAD"))
            {
                if (ContainsKey(pair.Key))
                {

                }
                else
                {
                    Add(pair.Key, new StatusFile(pair.Value));
                }
            }

            throw new NotImplementedException();
        }

     
       

        public List<IStatusFile> Changed
        {
            get { throw new NotImplementedException(); }
        }

        public List<IStatusFile> Added
        {
            get { throw new NotImplementedException(); }
        }

        public List<IStatusFile> Deleted
        {
            get { throw new NotImplementedException(); }
        }

        public List<IStatusFile> Untracked
        {
            get { throw new NotImplementedException(); }
        }

       
    }
}
