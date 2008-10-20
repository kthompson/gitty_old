using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Gitty.Util;
using System.IO;
using Gitty.Lib;

namespace Gitty
{
    public partial class Git
    {
        public class CLICore : Core
        {

            #region Core Method implementations
            public override void Init()
            {
                Command("init");
            }

            public override void Clone(System.IO.DirectoryInfo repository, string name, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void LogCommits(params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void FullLogCommits(params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void RevParse(string shaOrName)
            {
                throw new NotImplementedException();
            }

            public override string NameRev(string commitish)
            {
                throw new NotImplementedException();
            }

            public override Gitty.Lib.ObjectType ObjectType(string sha)
            {
                throw new NotImplementedException();
            }

            public override long ObjectSize(string sha)
            {
                throw new NotImplementedException();
            }

            public override Gitty.Lib.Repository GetRepo()
            {
                throw new NotImplementedException();
            }

            public override object CommitData(string sha)
            {
                throw new NotImplementedException();
            }

            public override object ProcessCommitData(string data, string sha)
            {
                throw new NotImplementedException();
            }

            public override string ObjectContents(string sha)
            {
                throw new NotImplementedException();
            }

            public override object LsTree(string sha)
            {
                throw new NotImplementedException();
            }

            public override string[] BranchesAll()
            {
                throw new NotImplementedException();
            }

            public override string[] ListFiles(string refDir)
            {
                throw new NotImplementedException();
            }

            public override string BranchCurrent()
            {
                throw new NotImplementedException();
            }

            public override void Grep(string search, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void DiffFull(string obj1, string obj2, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void DiffStats(string obj1, string obj2, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override object[] DiffFiles()
            {
                throw new NotImplementedException();
            }

            public override object[] DiffIndex(string treeish)
            {
                throw new NotImplementedException();
            }

            public override object[] LsFiles()
            {
                throw new NotImplementedException();
            }

            public override object ConfigRemote(string name)
            {
                throw new NotImplementedException();
            }

            public override object ConfigGet(string name)
            {
                throw new NotImplementedException();
            }

            public override object ConfigList()
            {
                throw new NotImplementedException();
            }

            public override void ConfigSet(string name, string value)
            {
                throw new NotImplementedException();
            }

            public override void Add(string path)
            {
                throw new NotImplementedException();
            }

            public override void Remove(string path, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void Commit(string message, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void Reset(Gitty.Lib.Commit commit, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void BranchNew(string branchName)
            {
                throw new NotImplementedException();
            }

            public override void BranchDelete(string branchName)
            {
                throw new NotImplementedException();
            }

            public override void Checkout(string branchName, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void Merge(string branch, string message)
            {
                throw new NotImplementedException();
            }

            public override string[] Unmerged()
            {
                throw new NotImplementedException();
            }

            public override string[] Conflicts()
            {
                throw new NotImplementedException();
            }

            public override void RemoteAdd(string name, string url, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void RemoteRemove(string name)
            {
                throw new NotImplementedException();
            }

            public override string[] Remotes()
            {
                throw new NotImplementedException();
            }

            public override string[] Tags()
            {
                throw new NotImplementedException();
            }

            public override string Tag(string tag)
            {
                throw new NotImplementedException();
            }

            public override void Fetch(string remote)
            {
                throw new NotImplementedException();
            }

            public override void Push(string remote, string branch)
            {
                throw new NotImplementedException();
            }

            public override void TagSha(string tagName)
            {
                throw new NotImplementedException();
            }

            public override void Repack()
            {
                throw new NotImplementedException();
            }

            public override void ReadTree(string treeish, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override string WriteTree()
            {
                throw new NotImplementedException();
            }

            public override string CommitTree(string tree, params string[] options)
            {
                throw new NotImplementedException();
            }

            public override void UpdateRef(string branch, string commit)
            {
                throw new NotImplementedException();
            }

            public override void CheckoutIndex(params string[] options)
            {
                throw new NotImplementedException();
            }

            public override System.IO.FileInfo Archive(string sha, System.IO.FileInfo file, params string[] options)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region private methods
            private string[] CommandLines(string command, bool changeDirectory, params KeyValuePair<string, string>[] options)
            {
                return Command(command, changeDirectory, options).Split('\n');
            }

            private string Command(string command, params KeyValuePair<string, string>[] options)
            {
                return Command(command, true, options);
            }
            private string Command(string command, bool changeDirectory, params KeyValuePair<string,string>[] options)
            {

#error this line needs to be corrected to use the correct path
                DirectoryInfo path = new DirectoryInfo(".");

                string opts = string.Join(" ", options.Select(o => o.Value).ToArray());

                Process proc = new Process();
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.FileName = "git";
                proc.StartInfo.Arguments = command + " " + options;

                string wd = Directory.GetCurrentDirectory();

                if (changeDirectory && (wd != path.FullName))
                {
                    Directory.SetCurrentDirectory(path.FullName);
                    proc.Start();
                    proc.WaitForExit();
                    Directory.SetCurrentDirectory(wd);
                }
                else
                {
                    proc.Start();
                    proc.WaitForExit();
                }

                string output = proc.StandardOutput.ReadToEnd();
                string err = proc.StandardError.ReadToEnd();

                if(proc.ExitCode > 0)
                {
                    if(proc.ExitCode ==1 && output == "")
                        return "";
                    throw new GitException(string.Format("git {0}: {1}", proc.StartInfo.Arguments, output + err));
                }

                return output;
            }
            #endregion
        }
    }
}
