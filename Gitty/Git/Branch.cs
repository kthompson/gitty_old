using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public class Branch : Path
        {
            
            public Branch(string path, bool checkPath)
                : base(path, checkPath)
            {
            }

            public Branch(string path)
                : base(path)
            {
            }

            private string _full;
            public string Full
            {
                get { return _full; }
                set { _full = value; }
            }

            private Remote _remote;
            public Remote Remote
            {
                get { return _remote; }
                set { _remote = value; }
            }

            private string _name;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
	

        }
    }
}
