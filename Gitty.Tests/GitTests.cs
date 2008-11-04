using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;
using System.IO;

namespace Gitty.Tests
{
    [TestFixture]
    public class GitTests
    {
        [Test]
        public void InitTest()
        {
            string repoPath = Path.Combine(Path.GetTempPath(), "git" + new Random().Next(int.MaxValue).ToString());
            Git git = Git.Init(new DirectoryInfo(repoPath));
            Assert.That(repoPath, Is.EqualTo(git.Repository.Directory.FullName));
        }
    }
}
