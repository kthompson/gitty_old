using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac.Builder;
using Module=Autofac.Builder.Module;

namespace Gitty.Lib.CommandLine
{
    public class GitCommandLine : GitModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new WorkingDirectory(c.Resolve<DirectoryInfo>())).As<IWorkingDirectory>().ContainerScoped();
            builder.Register(c => new Repository(c.Resolve<IWorkingDirectory>())).As<IRepository>().ContainerScoped();
            builder.Register(c => new Index(c.Resolve<IRepository>())).As<IIndex>();
            builder.Register(c => new GitLib()).As<IGit>();
        }

        public override string Name
        {
            get { return "Command Line Library"; }
        }

        public override string Description
        {
            get { return "The Gitty Command Line libary is dependant on msysgit for windows."; }
        }
    }
}