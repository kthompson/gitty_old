using System;
using System.IO;
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
            string Root = Path.Combine(Path.GetTempPath(), "git" + new Random().Next(int.MaxValue));
            string RepoPath = Path.Combine(Root, ".git");

            Directory.CreateDirectory(Root);

            var git = Git.Init(new DirectoryInfo(Root));
            Assert.AreEqual(RepoPath, git.Repository.ToString(), "Test#010");

            Assert.IsTrue(git.Repository.Directory.Exists, "Test#020");
            Assert.IsFalse(git.Index.File.Exists, "Test#030");
            Assert.IsTrue(git.WorkingDirectory.Directory.Exists, "Test#040");

            using(StreamWriter writer = File.CreateText(Path.Combine(git.WorkingDirectory, "hello")))
            {
                writer.WriteLine("Simple file to be added to repo");
            }

            git.Add();

            git.Index.File.Refresh();

            Assert.IsTrue(git.Index.File.Exists, "Test#050");

            git.Commit("-m", "sample commit");

            //teardown 
            Directory.Delete(Root, true);
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

     
    }
}