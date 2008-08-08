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

        internal byte[] Decompress(long p, long p_2, WindowCursor curs)
        {
            throw new NotImplementedException();
        }

        internal UnpackedObjectCache.Entry ReadCache(long p)
        {
            throw new NotImplementedException();
        }

        internal void SaveCache(long p, byte[] data, ObjectType objectType)
        {
            throw new NotImplementedException();
        }
    }
}
