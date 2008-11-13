using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    /// <summary>
    /// This class will be used to implement a wrapper when certain information will be ran multiple times.
    /// 
    /// The intent is to cache these methods to improve speed
    /// </summary>
    public class GitContext : IGit
    {

        private IGit git;

        public GitContext(IGit instance)
        {
            git = instance;
        }

        #region IGit Members

        public IRepository Repository
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IWorkingDirectory WorkingDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CurrentBranch
        {
            get { throw new NotImplementedException(); }
        }

        public string[] Branches
        {
            get { throw new NotImplementedException(); }
        }

        public void AddInteractive(params string[] options)
        {
            throw new NotImplementedException();
        }

        public bool Add(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Am(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Annotate(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Apply(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Archive(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Bisect(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Blame(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Branch(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Checkout(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void CherryPick(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Citool(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Clean(params string[] options)
        {
            throw new NotImplementedException();
        }

        public IGit Clone(string repospec, string name, params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Commit(string message, params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Diff(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Fetch(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void FormatPatch(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Gc(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Grep(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Gui(params string[] options)
        {
            throw new NotImplementedException();
        }

        public IGit Init(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Log(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Merge(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Mv(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Pull(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Push(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Rebase(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void RebaseInteractive(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Reset(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Revert(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Rm(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Shortlog(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Show(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Stash(params string[] options)
        {
            throw new NotImplementedException();
        }

        public IStatus Status(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Submodule(params string[] options)
        {
            throw new NotImplementedException();
        }

        public void Tag(params string[] options)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, ILsFilesFile> LsFiles(params string[] options)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IDiffFilesFile> DiffFiles(params string[] options)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IDiffIndexFile> DiffIndex(string treeish, params string[] options)
        {
            throw new NotImplementedException();
        }

        public GitContext GetContext()
        {
            return new GitContext(git);
        }

        #endregion
    }
}