using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core;

namespace Gitty
{
    public class TagCommand : Command
    {
        public override bool RequiresRepository
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresArguments
        {
            get { throw new NotImplementedException(); }
        }

        public TagCommand(Repository repo)
            : base(repo)
        {

        }
    }
}
