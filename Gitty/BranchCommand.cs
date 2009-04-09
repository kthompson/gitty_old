using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core;

namespace Gitty
{
    public class BranchCommand : Command
    {
        public override bool RequiresRepository
        {
            get { return true; }
        }

        public override bool RequiresArguments
        {
            get { return false; }
        }

        public BranchCommand(Repository repo)
            : base(repo)
        {
        }
    }
}
