using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class FileTreeEntry : TreeEntry 
    {
        public FileTreeEntry(Tree parent, ObjectId id, byte[] nameUTF8, bool execute)
            : base(parent,id, nameUTF8)
        {
            throw new NotImplementedException();
        }

        public override FileMode Mode
        {
            get { throw new NotImplementedException(); }
        }

        public override void Accept(TreeVisitor tv, int flags)
        {
            throw new NotImplementedException();
        }
    }
}
