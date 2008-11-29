using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Threading;
using Gitty.Lib;
using Autofac;
using Autofac.Builder;

namespace Gitty
{
    public class Git 
    {

        #region properties
        #endregion

        #region constructors

        static Git()
        {
            
        }

        private static ContainerBuilder GetBuilder()
        {
            var modules =
                from module in new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("Gitty.Lib.*.dll")
                from type in Assembly.LoadFrom(module.FullName).GetTypes()
                where typeof (GitModule).IsAssignableFrom(type)
                select type;

            //add first module
            var builder = new ContainerBuilder();
            builder.RegisterModule((IModule)Activator.CreateInstance(modules.First()));

            return builder;
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

            var builder = GetBuilder();

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

            var builder = GetBuilder();

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
            var lastPart = repouri.Split(new[]{'/', '\\'}).Last();

            var name = lastPart.EndsWith(".git") ? lastPart.Remove(lastPart.Length - 4) : lastPart;

            return Clone(directory, repouri, name);
        }
        public static IGit Clone(DirectoryInfo directory, string repouri, string name)
        {
            if (directory == null || string.IsNullOrEmpty(repouri))
                return null;

            var builder = GetBuilder();

            builder.Register(directory);
            try
            {
                using (var container = builder.Build())
                {
                    var git = container.Resolve<IGit>();
                    git.WorkingDirectory = container.Resolve<IWorkingDirectory>(new TypedParameter(typeof (DirectoryInfo), directory));
                    git.Clone(repouri, name);
                    return git;
                }
            }
            catch(InvalidOperationException ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
