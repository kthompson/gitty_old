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
        private static readonly ContainerBuilder builder = new ContainerBuilder();
        #endregion

        #region constructors

        static Git()
        {
            var modules =
                from module in new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("Gitty.Lib.*.dll")
                from type in Assembly.LoadFrom(module.FullName).GetTypes()
                where typeof (IModule).IsAssignableFrom(type)
                select type;

            //add first module
            builder.RegisterModule((IModule)Activator.CreateInstance(modules.First()));

        }

        private Git()
        {

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

            builder.Register(dir);
            using(var container = builder.Build())
            {
                var git = container.Resolve<IGit>();
                git.Repository = container.Resolve<IRepository>();
                return git;    
            }
            
        }

        public static IGit Bare(DirectoryInfo directory)
        {
            throw new NotImplementedException();
        }

        public static IGit Init(DirectoryInfo directory)
        {
            if (directory == null || !directory.Exists)
                return null;

            builder.Register(directory);
            using (var container = builder.Build())
            {
                var git = container.Resolve<IGit>();
                git.WorkingDirectory = container.Resolve<IWorkingDirectory>(new TypedParameter(typeof(DirectoryInfo), directory));
                git.Init();
                return git;
            }
        }

        public static IGit Clone(DirectoryInfo directory, string repouri)
        {
            if (directory == null || string.IsNullOrEmpty(repouri))
                return null;

            builder.Register(directory);
            using (var container = builder.Build())
            {
                var git = container.Resolve<IGit>();
                git.WorkingDirectory = container.Resolve<IWorkingDirectory>(new TypedParameter(typeof(DirectoryInfo), directory));
                git.Clone(repouri);
                return git;
            }
        }

        #endregion
    }
}
