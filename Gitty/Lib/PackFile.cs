using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gitty.Lib
{
    public class PackFile
    {
        public PackFile(Repository repo, FileInfo indexFile, FileInfo packFile)
        {

        }

        internal bool HasObject(AnyObjectId objectId)
        {
            throw new NotImplementedException();
        }


        public PackedObjectLoader Get(WindowCursor windowCursor, AnyObjectId id)
        {
            throw new NotImplementedException();
        }
    }
}
