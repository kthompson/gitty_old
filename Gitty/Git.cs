using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using Autofac;
using Autofac.Builder;
using Gitty.Lib;

namespace Gitty
{
    public class Git 
    {
        #region properties
        private static readonly IContainer container;
        #endregion

        #region constructors

        static Git()
        {
            var libraryTypes = from file in new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("Git.Core*.dll") 
                               from type in Assembly.Load(file.FullName).GetTypes()
                               where typeof (IGit).IsAssignableFrom(type)
                               select type;
            var builder = new ContainerBuilder();
            foreach (var type in libraryTypes)
            {
                builder.Register(type).As<IGit>().FactoryScoped();
            }

            container = builder.Build();
        }
        #endregion

        #region private static methods
        private static DirectoryInfo FindGitDirectory(DirectoryInfo path)
        {
            if (path == null || !path.Exists)
                return null;

            if (path.Name == ".git")
                return path;

            DirectoryInfo[] dirs = path.GetDirectories(".git");
            if (dirs.Length > 0)
                return dirs[0];

            return path.Parent == null ? null : FindGitDirectory(path.Parent);
        }
       

        #endregion

        #region public member methods

        public static IGit Open(DirectoryInfo directory)
        {
            DirectoryInfo dir = FindGitDirectory(directory);
            if(dir == null)
                return null;

            var git = container.Resolve<IGit>();
            git.Repository = new Repository(dir);

            return git;
        }

        public static IGit Bare(DirectoryInfo directory)
        {
            throw new NotImplementedException();
        }

        public static IGit Init(DirectoryInfo directory)
        {
            if (directory == null || !directory.Exists)
                return null;

            var git = container.Resolve<IGit>();
            git.WorkingDirectory = new WorkingDirectory(directory);
            git.Init();
            return git;
        }

        public static IGit Clone(DirectoryInfo directory, string repouri)
        {
            if (directory == null || string.IsNullOrEmpty(repouri))
                return null;

            var git = container.Resolve<IGit>();
            git.WorkingDirectory = new WorkingDirectory(directory);
            git.Clone(repouri);
            return git;
        }

        #endregion
    }
}
