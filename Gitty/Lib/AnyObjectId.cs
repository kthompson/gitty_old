using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public abstract class AnyObjectId
    {
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
    }
}
