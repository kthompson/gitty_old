using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public abstract class TreeEntry : IComparable
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void SetModified()
        {
            throw new NotImplementedException();
        }

        public string Name { get; protected set; }
        public Tree Parent { get; private set; }
        private ObjectId _id;
        public ObjectId Id { get 
        {
            return _id;
        }
            set
            {
                //
                Tree p = Parent;
                if (p != null && _id != value)
                {
                    if ((_id == null && value != null) || (_id != null && value == null)
                            || !_id.Equals(value))
                    {
                        p.Id = null;
                    }
                }

                _id = value;
            }
        }

        public bool IsModified
        {
            get
            {
                return _id == null;
            }
        }

        public string GetFullName()
        {
            throw new NotImplementedException();
        }


    }
}
