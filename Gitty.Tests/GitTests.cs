using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Gitty.Tests
{
    [TestFixture]
    public class GitTests
    {
        public string Root { get; private set; }
        public string RepoPath { get; private set; }

        [SetUp]
        public void Setup()
        {
            Root = Path.Combine(Path.GetTempPath(), "git" + new Random().Next(int.MaxValue));
            RepoPath = Path.Combine(Root, ".git");

            Directory.CreateDirectory(Root);
        }

        [Test]
        public void InitTest()
        {
            Git git = Git.Init(new DirectoryInfo(Root));
            Assert.That(RepoPath, Is.EqualTo(git.Repository.Directory.FullName));
        }

        [TearDown]
        public void Teardown()
        {
            Directory.Delete(Root, true);
        }
    }
}