using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Builder;

namespace Gitty
{
    public abstract class GitModule : Module
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual bool CanUse()
        {
            return true;
        }
    }
}
