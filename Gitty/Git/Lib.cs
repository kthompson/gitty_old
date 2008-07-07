using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public class Lib
        {

            public Lib()
                : this(null, null)
            {
            }

            public Lib(Base @base)
                : this(@base, null)
            {
            }

            public Lib(Base @base, object logger)
            {
                throw new NotImplementedException();
            }

            internal Options Clone(string repository, string name, Options options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
