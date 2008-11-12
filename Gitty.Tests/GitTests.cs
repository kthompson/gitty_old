using System;
using System.IO;
using Gitty.Lib.CommandLine;
using NUnit.Framework;

namespace Gitty.Tests
{
    [TestFixture]
    public class GitTests
    {

        [Test]
        public void InitAndCloneTest()
        {
            //setup 
            string Root = GetTempFolder("git" + new Random().Next(int.MaxValue));
            string RepoPath = Path.Combine(Root, ".git");

            Directory.CreateDirectory(Root);

            var git = Git.Init(new DirectoryInfo(Root));
            Assert.AreEqual(RepoPath, git.Repository.ToString(), "Test#010");

            Assert.IsTrue(git.Repository.Directory.Exists, "Test#020");
            Assert.IsFalse(git.Repository.Index.File.Exists, "Test#030");
            Assert.IsTrue(git.WorkingDirectory.Directory.Exists, "Test#040");

            using(StreamWriter writer = File.CreateText(Path.Combine(git.WorkingDirectory.ToString(), "hello")))
            {
                writer.WriteLine("Simple file to be added to repo");
            }

            git.Add();

            git.Repository.Index.File.Refresh();

            Assert.IsTrue(git.Repository.Index.File.Exists, "Test#050");

            //git.Status();
            

            git.Commit("sample commit");

            //teardown 
            Directory.Delete(Root, true);
        }

        private static string GetTempFolder(string name)
        {
            return Path.Combine(Path.GetTempPath(), name);
        }

        [Test]
        public void CloneTest()
        {
            //setup 
            string name = "cloneTest" + new Random().Next(int.MaxValue);
            string Root2 = Path.Combine(Path.GetTempPath(), name);

            var git1 = GetTempGit();

            Touch(Path.Combine(git1.WorkingDirectory.ToString(), "file1"));
            Touch(Path.Combine(git1.WorkingDirectory.ToString(), "file2"));
            Touch(Path.Combine(git1.WorkingDirectory.ToString(), "file3"));

            git1.Add();

            git1.Commit("committed");

            var git = Git.Clone(new DirectoryInfo(Path.GetTempPath()), git1.WorkingDirectory.ToString(), name);
            //Assert.AreEqual(RepoPath, git.Repository);
            var files = git.LsFiles();
            Assert.IsTrue(files.ContainsKey("file1"), "0100");
            Assert.IsTrue(files.ContainsKey("file2"), "0200");
            Assert.IsTrue(files.ContainsKey("file3"), "0300");
            //teardown 
            Directory.Delete(git1.WorkingDirectory.ToString(), true);
            Directory.Delete(Root2, true);
        }

        [Test]
        public void CloneTest2()
        {
            //setup 
            
            string name = "cloneTest" + new Random().Next(int.MaxValue);
            string root1 = GetTempFolder(name);
            
            Directory.CreateDirectory(root1);

            IGit git1 = GetPopulatedGitRepo();

            string root2 = Path.Combine(root1, git1.WorkingDirectory.Directory.Name);

            var git = Git.Clone(new DirectoryInfo(root1), git1.WorkingDirectory.ToString());

            Assert.AreEqual(git.WorkingDirectory.ToString(), root2, "0050");

            var files = git.LsFiles();
            Assert.IsTrue(files.ContainsKey("file1"), "0100");
            Assert.IsTrue(files.ContainsKey("file2"), "0200");
            Assert.IsTrue(files.ContainsKey("file3"), "0300");
            //teardown 
            Directory.Delete(git1.WorkingDirectory.ToString(), true);
            Directory.Delete(root1, true);
        }

        private static IGit GetPopulatedGitRepo()
        {
            var git1 = GetTempGit();

            

            Touch(Path.Combine(git1.WorkingDirectory.ToString(), "file1"));
            Touch(Path.Combine(git1.WorkingDirectory.ToString(), "file2"));
            Touch(Path.Combine(git1.WorkingDirectory.ToString(), "file3"));

            git1.Add();

            git1.Commit("committed");
            return git1;
        }

        [Test]
        public void CloneTest3()
        {
            //setup 

            string name = "cloneTest" + new Random().Next(int.MaxValue);
            string root1 = GetTempFolder(name);

            Directory.CreateDirectory(root1);

            IGit git1 = GetPopulatedGitRepo();

            string root2 = Path.Combine(root1, git1.WorkingDirectory.Directory.Name);

            var git = Git.Clone(new DirectoryInfo(root1), git1.WorkingDirectory+".git");

            Assert.AreEqual(git.WorkingDirectory.ToString(), root2, "0050");

            var files = git.LsFiles();
            Assert.IsTrue(files.ContainsKey("file1"), "0100");
            Assert.IsTrue(files.ContainsKey("file2"), "0200");
            Assert.IsTrue(files.ContainsKey("file3"), "0300");
            //teardown 
            Directory.Delete(git1.WorkingDirectory.ToString(), true);
            Directory.Delete(root1, true);
        }

        [Test]
        public void TestGitPath()
        {

            var testdir = GetTempFolder("testdir");
            Directory.CreateDirectory(testdir);

            Touch(Path.Combine(testdir, "file1"));
            Touch(Path.Combine(testdir, "file2"));
            Touch(Path.Combine(testdir, "file3"));

            var subdir = Path.Combine(testdir, "dir");
            Directory.CreateDirectory(subdir);

            Touch(Path.Combine(subdir, "file4"));

            var subdir2 = Path.Combine(subdir, "dir2");
            Directory.CreateDirectory(subdir2);

            Touch(Path.Combine(subdir2, "file5"));
            Touch(Path.Combine(subdir2, "file6"));

            var simpledir = new DirectoryInfo(testdir);
            var path = new GitPath(simpledir);

            Assert.AreEqual(simpledir, path.Directory,"0100");

            var files = path.GetFiles();
            Assert.AreEqual("dir", path.GetRelativePath(files[0].FullName), "0200");
            Assert.AreEqual("file1", path.GetRelativePath(files[1].FullName), "0300");
            Assert.AreEqual("file2", path.GetRelativePath(files[2].FullName), "0400");
            Assert.AreEqual("file3", path.GetRelativePath(files[3].FullName), "0450");
            Assert.AreEqual("dir/dir2", path.GetRelativePath(files[4].FullName), "0500");
            Assert.AreEqual("dir/file4", path.GetRelativePath(files[5].FullName), "0600");
            Assert.AreEqual("dir/dir2/file5", path.GetRelativePath(files[6].FullName), "0700");
            Assert.AreEqual("dir/dir2/file6", path.GetRelativePath(files[7].FullName), "0800");

            Directory.Delete(testdir, true);
        }


        [Test]
        public void GitStatusTest()
        {
            //setup 
            IGit git = GetTempGit();
            string wd = git.WorkingDirectory.ToString();

            using (StreamWriter writer = File.CreateText(Path.Combine(wd, "hello")))
            {
                writer.WriteLine("Simple file to be added to repo");
            }

            git.Add("hello");

            git.Commit("first commit");

            using (StreamWriter writer = File.CreateText(Path.Combine(wd, "uncommitedfile")))
            {
                writer.WriteLine("<empty file>");
            }

            git.Add("uncommitedfile");


            using (StreamWriter writer = File.CreateText(Path.Combine(wd, "untrackedfile")))
            {
                writer.WriteLine("<empty file>");
            }

            var status = git.Status();

            Assert.IsTrue(status.ContainsKey("hello"), "0100");
            Assert.AreEqual("hello",status["hello"].Path, "0200");

            Assert.IsTrue(status.ContainsKey("uncommitedfile"), "0300");
            Assert.AreEqual(DiffMode.Add, status["uncommitedfile"].Type, "0400");

            Assert.IsTrue(status.ContainsKey("untrackedfile"), "0500");
            Assert.IsTrue(status["untrackedfile"].Untracked, "0600");
            
            
            //teardown 
            Directory.Delete(wd, true);
        }

        private static IGit GetTempGit()
        {
            string Root = GetTempFolder("git" + new Random().Next(int.MaxValue));

            Directory.CreateDirectory(Root);

            return Git.Init(new DirectoryInfo(Root));
        }

        private static void Touch(string combine)
        {
            File.Create(combine).Close();
        }
    }
}