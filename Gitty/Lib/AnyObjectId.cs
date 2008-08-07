﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Util;
using System.IO;
using Gitty.Extensions;

namespace Gitty.Lib
{
    [Complete]
    public abstract class AnyObjectId 
        : IComparable<ObjectId>, IComparable
    {
        internal class Constants
        {
            public static readonly int ObjectIdLength = 20;
            public static readonly int StringLength = ObjectIdLength * 2;
        }
        
        
        public static bool operator ==(AnyObjectId a, AnyObjectId b)
        {
            return (a.W2 == b.W2) &&
                   (a.W3 == b.W3) &&
                   (a.W4 == b.W4) &&
                   (a.W5 == b.W5) &&
                   (a.W1 == b.W1);
        }

        public static bool operator !=(AnyObjectId a, AnyObjectId b)
        {
            return !(a == b);
        }

        public bool Equals(ObjectId obj)
        {
            return (obj != null) ? Equals((object)obj) : false;
        }

        public override bool Equals(object obj)
        {
            AnyObjectId id = obj as AnyObjectId;
            if (obj == null)
                return false;

            return (this == id);
        }

        public void CopyTo(StreamWriter w)
        {
            w.Write(ToHexByteArray());
        }

        private byte[] ToHexByteArray()
        {
            byte[] dst = new byte[Constants.StringLength];

            Hex.FillHexByteArray(dst, 0, W1);
            Hex.FillHexByteArray(dst, 8, W2);
            Hex.FillHexByteArray(dst, 16, W3);
            Hex.FillHexByteArray(dst, 24, W4);
            Hex.FillHexByteArray(dst, 32, W5);
            return dst;
        }

        public override int GetHashCode()
        {
            return (int)this.W2;
        }


        public int W1 { get; set; }
        public int W2 { get; set; }
        public int W3 { get; set; }
        public int W4 { get; set; }
        public int W5 { get; set; }

        public int GetFirstByte()
        {
            //same as W1 >>> 24 in java
            return  (byte)((W1 & 0xf000) >> 24);
        }

        #region IComparable<ObjectId> Members

        public int CompareTo(ObjectId other)
        {
            if (this == other)
                return 0;
            
            int cmp;

            cmp = NB.CompareUInt32(W1, other.W1);
            if (cmp != 0)
                return cmp;

            cmp = NB.CompareUInt32(W2, other.W2);
            if (cmp != 0)
                return cmp;

            cmp = NB.CompareUInt32(W3, other.W3);
            if (cmp != 0)
                return cmp;

            cmp = NB.CompareUInt32(W4, other.W4);
            if (cmp != 0)
                return cmp;

            return NB.CompareUInt32(W5, other.W5);

        }



        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this.CompareTo((ObjectId)obj);
        }

        #endregion


        private char[] ToHexCharArray()
        {
            char[] dest = new char[Constants.StringLength];
            ToHexCharArray(dest);
            return dest;
        }

        private void ToHexCharArray(char[] dest)
        {
            Hex.FillHexCharArray(dest, 0, W1);
            Hex.FillHexCharArray(dest, 8, W2);
            Hex.FillHexCharArray(dest, 16, W3);
            Hex.FillHexCharArray(dest, 24, W4);
            Hex.FillHexCharArray(dest, 32, W5);
        }

        public override string ToString()
        {
            return new string(ToHexCharArray());
        }

        public ObjectId Copy() {
            if (this.GetType() == typeof(ObjectId))
                return (ObjectId)this;
            return new ObjectId(this);            
        }

        public abstract ObjectId ToObjectId();


    }
}
