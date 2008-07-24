using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gitty.Util
{
    internal class Numbers
    {

        public static int Compare(uint a, uint b)
        {
            if (a > b)
                return -1;
            else if (a < b)
                return 1;
            else 
                return 0;
        }

        public static int UnsignedRightShift(int n, int s) //n is an integer
        {
            if (n > 0)
            {
                return n >> s;
            }
            else
            {
                return (n >> s) + (2 << ~s);
            }
        }

        public static long UnsignedRightShift(long n, int s)  //Overloaded function where n is a long
        {
            if (n > 0)
            {
                return n >> s;
            }
            else
            {
                return (n >> s) + (((long)2) << ~s);
            }
        }

        public static uint DecodeUInt(byte[] buffer, int offset)
        {
            uint r = buffer[offset];
            r <<= 8;

            r |= (byte)(buffer[offset + 1]);
            r <<= 8;

            r |= (byte)(buffer[offset + 2]);
            r <<= 8;

            r |= (byte)(buffer[offset + 3]);
            return r;
        }
    }
}
