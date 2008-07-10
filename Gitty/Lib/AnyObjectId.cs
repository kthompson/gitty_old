using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Util;

namespace Gitty.Lib
{
    public abstract class AnyObjectId 
        : IComparable<ObjectId>, IComparable
    {
        static readonly int StringLength = Constants.ObjectIdLength * 2;
        static byte[] fromhex;

        static AnyObjectId()
        {
            fromhex = new byte['f' + 1];
            for (int i = 0; i < fromhex.Length; i++)
                fromhex[i] = byte.MaxValue;
            for (char i = '0'; i <= '9'; i++)
                fromhex[i] = (byte)(i - '0');
            for (char i = 'a'; i <= 'f'; i++)
                fromhex[i] = (byte)((i - 'a') + 10);
        }

        public static int HexUInt32(byte[] bs, int p)
        {
            int r = fromhex[bs[p]] << 4;

            r |= fromhex[bs[p + 1]];
            r <<= 4;

            r |= fromhex[bs[p + 2]];
            r <<= 4;

            r |= fromhex[bs[p + 3]];
            r <<= 4;

            r |= fromhex[bs[p + 4]];
            r <<= 4;

            r |= fromhex[bs[p + 5]];
            r <<= 4;

            r |= fromhex[bs[p + 6]];
            r <<= 4;

            int last = fromhex[bs[p + 7]];
            if (r < 0 || last < 0)
                throw new IndexOutOfRangeException();

            return (r << 4) | last;

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

        public override bool Equals(object obj)
        {
            AnyObjectId id = obj as AnyObjectId;
            if (obj == null)
                return false;

            return (this == id);
        }

        public override int GetHashCode()
        {
            return this.W2;
        }


        public int W1 { get; set; }
        public int W2 { get; set; }
        public int W3 { get; set; }
        public int W4 { get; set; }
        public int W5 { get; set; }

        public int GetFirstByte()
        {
            //same as W1 >>> 24 in java
            return Numbers.UnsignedRightShift(W1, 24);
        }

        #region IComparable<ObjectId> Members

        public int CompareTo(ObjectId other)
        {
            if (this == other)
                return 0;
            
            int cmp;

            cmp = Numbers.Compare(W1, other.W1);
            if (cmp != 0)
                return cmp;

            cmp = Numbers.Compare(W2, other.W2);
            if (cmp != 0)
                return cmp;

            cmp = Numbers.Compare(W3, other.W3);
            if (cmp != 0)
                return cmp;

            cmp = Numbers.Compare(W4, other.W4);
            if (cmp != 0)
                return cmp;

            return Numbers.Compare(W5, other.W5);

        }



        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this.CompareTo((ObjectId)obj);
        }

        #endregion
    }
}
