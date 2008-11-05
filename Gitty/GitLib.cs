﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Lib;
using System.IO;
using System.Diagnostics;

namespace Gitty
{
    public class GitLib
    {

        public class Constants
        {
            public static readonly string[] GitSearchPaths = { 
                                                                Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Git\bin"), 
                                                                "/bin", 
                                                                Environment.SystemDirectory,
                                                                Environment.CurrentDirectory                                                                
                                                                };
        }

        #region properties
        public DirectoryInfo WorkingDirectory { get; private set; }
        public string GitExecutable { get; private set; }
        #endregion

        #region constructors
        private GitLib(DirectoryInfo working)
        {
            WorkingDirectory = working;
            GitExecutable = GetGitPath();
            
        }
        #endregion

        #region syntactic sugar
        public static GitLib For(DirectoryInfo dir)
        {
            return new GitLib(dir);
        }
        #endregion

        #region the porcelain
        public void AddInteractive(params string[] options)
        {
        }
        public void Add(params string[] options) 
        {
        }
        public void Am(params string[] options) 
        {
        }
        public void Annotate(params string[] options) { }
        public void Apply(params string[] options) { }
        public void Archive(params string[] options) { }
        public void Bisect(params string[] options) { }
        public void Blame(params string[] options) { }
        public void Branch(params string[] options) { }
        public void Checkout(params string[] options) { }
        public void CherryPick(params string[] options){}
        public void Citool(params string[] options){}
        public void Clean(params string[] options){}
        public Repository Clone(string repospec, params string[] options)
        {
            Command("clone", repospec);
            return Repository.Open(WorkingDirectory);
        }
        public void Commit(params string[] options){}
        public void Diff(params string[] options){}
        public void Fetch(params string[] options){}
        public void FormatPatch(params string[] options){}
        public void Gc(params string[] options){}
        public void Grep(params string[] options){}
        public void Gui(params string[] options){}
        public Repository Init(params string[] options)
        {
            // run git init and return the resulting repo
            Command("init");
            return Repository.Open(WorkingDirectory);
        }
        public void Log(params string[] options){}
        public void Merge(params string[] options){}
        public void Mv(params string[] options){}
        public void Pull(params string[] options){}
        public void Push(params string[] options){}
        public void Rebase(params string[] options){}
        public void RebaseInteractive(params string[] options){}
        public void Reset(params string[] options){}
        public void Revert(params string[] options){}
        public void Rm(params string[] options){}
        public void Shortlog(params string[] options){}
        public void Show(params string[] options){}
        public void Stash(params string[] options){}
        public void Status(params string[] options){}
        public void Submodule(params string[] options){}
        public void Tag(params string[] options){ }
        #endregion

        #region private methods

        private static string GetGitPath()
        {
            foreach (string path in Constants.GitSearchPaths)
            {
                if (File.Exists(Path.Combine(path, "git")))
                    return Path.Combine(path, "git");
                if(File.Exists(Path.Combine(path, "git.exe")))
                    return Path.Combine(path, "git.exe");
            }

            throw new FileNotFoundException("Could not find git executable");
        }

        private string[] CommandLines(string command, params string[] options)
        {
            return Command(command, options).Split('\n');
        }

        private string Command(string command, params string[] options)
        {
            string opts = string.Join(" ", options.Select(s => s.Contains(" ") ? "\"" + s + "\"" : s).ToArray());
            
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = GitExecutable;
            proc.StartInfo.Arguments = command + " " + opts; //redirect error to output
            proc.StartInfo.WorkingDirectory = WorkingDirectory.FullName;
            
            //Directory.SetCurrentDirectory(WorkingDirectory.FullName);
            proc.Start();

            string output = proc.StandardOutput.ReadToEnd();
            output += proc.StandardError.ReadToEnd();

            proc.WaitForExit();
            
            if (proc.ExitCode != 0)
            {
                if (proc.ExitCode == 1 && string.IsNullOrEmpty(output))
                    return "";
                
                throw new Exception(string.Format("git {0}: {1}", proc.StartInfo.Arguments, output));
            }

            return output;
        }
        #endregion
    }
}
