using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Lib;

namespace Gitty
{
    public interface IGit
    {
        IRepository Repository { get; set; }
        IWorkingDirectory WorkingDirectory { get; set; }
        string CurrentBranch { get; }
        string[] Branches { get; }

        #region porcelain
        void AddInteractive(params string[] options);
        bool Add(params string[] options);
        void Am(params string[] options);
        void Annotate(params string[] options);
        void Apply(params string[] options);
        void Archive(params string[] options);
        void Bisect(params string[] options);
        void Blame(params string[] options);
        void Branch(params string[] options);
        void Checkout(params string[] options);
        void CherryPick(params string[] options);
        void Citool(params string[] options);
        void Clean(params string[] options);
        IGit Clone(string repospec, string name, params string[] options);
        void Commit(string message, params string[] options);
        void Diff(params string[] options);
        void Fetch(params string[] options);
        void FormatPatch(params string[] options);
        void Gc(params string[] options);
        void Grep(params string[] options);
        void Gui(params string[] options);
        IGit Init(params string[] options);
        void Log(params string[] options);
        void Merge(params string[] options);
        void Mv(params string[] options);
        void Pull(params string[] options);
        void Push(params string[] options);
        void Rebase(params string[] options);
        void RebaseInteractive(params string[] options);
        void Reset(params string[] options);
        void Revert(params string[] options);
        void Rm(params string[] options);
        void Shortlog(params string[] options);
        void Show(params string[] options);
        void Stash(params string[] options);
        IStatus Status(params string[] options);
        void Submodule(params string[] options);
        void Tag(params string[] options);
        #endregion

        #region plumbing

        IDictionary<string,ILsFilesFile> LsFiles(params string[] options);
        IDictionary<string, IDiffFilesFile> DiffFiles(params string[] options);
        IDictionary<string, IDiffIndexFile> DiffIndex(string treeish, params string[] options);
        #endregion

        #region other
        GitContext GetContext();
        #endregion
    }
}
