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

        internal void CopyRawData(PackedObjectLoader packedObjectLoader, Stream o, byte[] buf)
        {
            throw new NotImplementedException();
        }

        internal bool SupportsFastCopyRawData()
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}
