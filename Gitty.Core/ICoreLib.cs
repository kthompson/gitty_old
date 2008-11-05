using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gitty
{
    public interface ICoreLib
    {
        public abstract void Init();
        public abstract void Clone(DirectoryInfo repository, string name, params string[] options);
        public abstract void LogCommits(params string[] options);
        public abstract void FullLogCommits(params string[] options);
        public abstract void RevParse(string shaOrName);
        public abstract string NameRev(string commitish);
        public abstract ObjectType ObjectType(string sha);
        public abstract long ObjectSize(string sha);
        public abstract Repository GetRepo();

        // proper return type needs to be defined
        public abstract object CommitData(string sha);
        public object ProcessCommitData(string data);
        public abstract object ProcessCommitData(string data, string sha);

        public abstract string ObjectContents(string sha);
        
        // proper return type needs to be defined
        public abstract object LsTree(string sha);
        public abstract string[] BranchesAll();
        public abstract string[] ListFiles(string refDir);
        public abstract string BranchCurrent();
        public abstract void Grep(string search, params string[] options);

        public void DiffFull();
        public void DiffFull(string obj1);
        public abstract void DiffFull(string obj1, string obj2, params string[] options);

        public void DiffStats();
        public void DiffStats(string obj1);
        public abstract void DiffStats(string obj1, string obj2, params string[] options);

        // proper return types need to be defined
        public abstract object[] DiffFiles();
        public abstract object[] DiffIndex(string treeish);
        public abstract object[] LsFiles();

        public abstract object ConfigRemote(string name);
        public abstract object ConfigGet(string name);
        public abstract object ConfigList();

        public abstract void ConfigSet(string name, string value);
        public void Add();
        public abstract void Add(string path);
        public void Remove();
        public abstract void Remove(string path, params string[] options);
        public abstract void Commit(string message, params string[] options);
        public abstract void Reset(Commit commit, params string[] options);
        public abstract void BranchNew(string branchName);
        public abstract void BranchDelete(string branchName);
        public abstract void Checkout(string branchName, params string[] options);
        public void Merge(string branch);
        public abstract void Merge(string branch, string message);
        public abstract string[] Unmerged();
        public abstract string[] Conflicts();
        public abstract void RemoteAdd(string name, string url, params string[] options);
        public abstract void RemoteRemove(string name);
        public abstract string[] Remotes();
        public abstract string[] Tags();
        public abstract string Tag(string tag);
        public abstract void Fetch(string remote);
        public void Push(string remote);
        
        public abstract void Push(string remote, string branch);
        public abstract void TagSha(string tagName);
        public abstract void Repack();
        public abstract void ReadTree(string treeish, params string[] options);
        public abstract string WriteTree();
        public abstract string CommitTree(string tree, params string[] options);
        public abstract void UpdateRef(string branch, string commit);
        public abstract void CheckoutIndex(params string[] options);
        public FileInfo Archive(string sha);
        public abstract FileInfo Archive(string sha, FileInfo file, params string[] options);

    }
}
