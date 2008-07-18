using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Util
{
    public sealed class Hex
    {
        static byte[] __hexCharToValue;
        static char[] __valueToHexChar;
        public static int Nibble = 4;
        static Hex()
        {
            __valueToHexChar = new char[] {'0', '1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'};

            __hexCharToValue = new byte['f' + 1];
            for (int i = 0; i < __hexCharToValue.Length; i++)
                __hexCharToValue[i] = byte.MaxValue;
            for (char i = '0'; i <= '9'; i++)
                __hexCharToValue[i] = (byte)(i - '0');
            for (char i = 'a'; i <= 'f'; i++)
                __hexCharToValue[i] = (byte)((i - 'a') + 10);
        }

        public static byte HexCharToValue(Char c)
        {
            return __hexCharToValue[c];
        }

        public static byte HexCharToValue(byte c)
        {
            return __hexCharToValue[c];
        }

        public static uint HexStringToUInt32(byte[] bs, int offset)
        {
            uint r = __hexCharToValue[bs[offset]];
            r <<= Nibble; // push one nibble

            r |= __hexCharToValue[bs[offset + 1]];
            r <<= Nibble;

            r |= __hexCharToValue[bs[offset + 2]];
            r <<= Nibble;

            r |= __hexCharToValue[bs[offset + 3]];
            r <<= Nibble;

            r |= __hexCharToValue[bs[offset + 4]];
            r <<= Nibble;

            r |= __hexCharToValue[bs[offset + 5]];
            r <<= Nibble;

            r |= __hexCharToValue[bs[offset + 6]];
            r <<= Nibble;

            uint last = __hexCharToValue[bs[offset + 7]];
            if (r < 0 || last < 0)
                throw new IndexOutOfRangeException();

            return (r << Nibble) | last;
        }

        public static void FillHexCharArray(char[] dest, int offset, uint value){
            int curOffset = offset + 7;
            while (curOffset >= offset && value != 0)
            {
                dest[curOffset--] = __valueToHexChar[value & 0xf];
                value >>= 4;
            }
            while (curOffset >= offset)
                dest[curOffset--] = '0';
        }

    }
}
