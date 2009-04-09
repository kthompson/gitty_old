using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core;

namespace Gitty
{
    public class Branch : Command
    {
        public override bool RequiresRepository
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresArguments
        {
            get { throw new NotImplementedException(); }
        }

        public Branch(Repository repo)
            : base(repo)
        {

        }
    }
}
