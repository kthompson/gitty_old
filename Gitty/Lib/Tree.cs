using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class Tree : TreeEntry, Treeish
    {
        public Tree(Repository repo, ObjectId myId, byte[] raw)
            : base(null,null,null)
        {
            throw new NotImplementedException();
        }

        #region Treeish Members

        public ObjectId GetTreeId()
        {
            throw new NotImplementedException();
        }

        public Tree GetTree()
        {
            throw new NotImplementedException();
        }

        #endregion

        internal bool IsRoot()
        {
            throw new NotImplementedException();
        }

        internal void RemoveEntry(TreeEntry treeEntry)
        {
            throw new NotImplementedException();
        }

        internal void AddEntry(TreeEntry treeEntry)
        {
            throw new NotImplementedException();
        }

        internal static int CompareNames(byte[] NameUTF8, byte[] p, int p_3, int p_4)
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
