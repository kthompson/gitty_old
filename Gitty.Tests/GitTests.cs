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
            

            git.Commit("-m", "sample commit");

            //teardown 
            Directory.Delete(Root, true);
        }

        private string GetTempFolder(string name)
        {
            return Path.Combine(Path.GetTempPath(), name);
        }

        [Test]
        public void CloneTest()
        {
            //setup 
            string Root2 = Path.GetTempPath();

            var git = Git.Clone(new DirectoryInfo(Root2), "git://localhost/kthompson/gitty.git");
            //Assert.AreEqual(RepoPath, git.Repository);

            //teardown 
            Directory.Delete(Root2, true);
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

        private void Touch(string combine)
        {
            File.Create(combine).Close();
        }
    }
}