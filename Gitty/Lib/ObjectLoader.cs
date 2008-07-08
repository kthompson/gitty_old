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

                }
                return _id;
            }            
        }

    }
}
