using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class Tree : TreeEntry, Treeish
    {
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
    }
}
