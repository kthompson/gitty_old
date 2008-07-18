using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public abstract class ObjectLoader
    {
        private ObjectId _id;
        public ObjectId Id {
            get
            {
                if(_id == null){
#warning TODO: finish implementation
                    throw new NotImplementedException();
                }
                return _id;
            }
            set
            {
                if (_id != null)
                    throw new InvalidOperationException("Id already set.");
                _id = value;
            }
        }

        public abstract ObjectType ObjectType{get;}
        public abstract long Size { get; }
        public abstract byte[] Bytes{get;}
        public abstract byte[] CachedBytes { get; }
        public abstract int RawType { get; }
        public abstract long RawSize { get; }
        
    }
}
