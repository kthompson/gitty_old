using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public partial class Git
    {
        public class Stash
        {
            private Base _base;

            public Stash(Base @base, string message, bool existing)
            {
                _base = @base;
                _message = message;
                if (!existing)
                    this.Save();
            }

            public Stash(Base @base, string message)
                :this(@base, message, false)
            {

            }

            public void Save()
            {
                //@saved = @base.lib.stash_save(@message)
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            private bool _saved;
            public bool IsSaved
            {
                get { return _saved; }
            }

            private string _message;
            public string Message
            {
                get { return _message; }
            }

            public override string ToString()
            {
                return _message;
            }
        }
    }
}
