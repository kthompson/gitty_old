﻿using System;
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
    }
}
