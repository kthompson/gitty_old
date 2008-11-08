using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac.Builder;
using Gitty.Lib.CommandLine;
using Module=Autofac.Builder.Module;

namespace Gitty.Lib.CLI
{
    public class GitCommandLine : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new WorkingDirectory(c.Resolve<DirectoryInfo>())).As<IWorkingDirectory>().ContainerScoped();
            builder.Register(c => new Repository(c.Resolve<IWorkingDirectory>())).As<IRepository>().ContainerScoped();
            builder.Register(c => new Index(c.Resolve<IRepository>())).As<IIndex>();
            builder.Register(c => new GitLib()).As<IGit>();
        }
    }
}
