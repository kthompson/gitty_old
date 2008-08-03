using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gitty.Lib
{
    [Complete]
    public class AbstractIndexTreeVisitor : IndexTreeVisitor
    {
        public virtual void FinishVisitTree(Tree tree, Tree auxTree, String curDir)
        {
            // Empty
        }

        public virtual void FinishVisitTree(Tree tree, int i, String curDir)
        {
            // Empty
        }

        public virtual void VisitEntry(TreeEntry treeEntry, GitIndex.Entry indexEntry, FileInfo file)
        {
            // Empty
        }

        public virtual void VisitEntry(TreeEntry treeEntry, TreeEntry auxEntry,
                GitIndex.Entry indexEntry, FileInfo file)
        {
            // Empty
        }
    }
}
