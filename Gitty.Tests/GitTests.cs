using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Gitty.Tests
{
    [TestFixture]
    public class GitTests
    {

        [Test]
        public void InitTest()
        {
            //setup 
            string Root = Path.Combine(Path.GetTempPath(), "git" + new Random().Next(int.MaxValue));
            string RepoPath = Path.Combine(Root, ".git");

            Directory.CreateDirectory(Root);

            Git git = Git.Init(new DirectoryInfo(Root));
            Assert.That(RepoPath, Is.EqualTo(git.Repository.Directory.FullName));


            //teardown 
            Directory.Delete(Root, true);
        }

        [Test]
        public void CloneTest()
        {
            //setup 
            string Root = Path.Combine(Path.GetTempPath(), "git" + new Random().Next(int.MaxValue));
            string RepoPath = Path.Combine(Root, ".git");
            Directory.CreateDirectory(Root);

            Git git = Git.Clone(new DirectoryInfo(Root), "git://localhost/kthompson/gitty.git");
            Assert.That(RepoPath, Is.EqualTo(git.Repository.Directory.FullName));

            //teardown 
            Directory.Delete(Root, true);
        }

     
    }
}