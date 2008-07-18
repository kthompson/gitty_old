using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public interface Treeish
    {
        ObjectId GetTreeId();
        Tree GetTree();
    }
}
