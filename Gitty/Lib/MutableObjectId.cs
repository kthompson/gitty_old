using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Util;

namespace Gitty.Lib
{
    public class MutableObjectId : AnyObjectId
    {

        public MutableObjectId()
            : base()
        {

        }

        public MutableObjectId(MutableObjectId src)
        {
            this.W1 = src.W1;
            this.W2 = src.W2;
            this.W3 = src.W3;
            this.W4 = src.W4;
            this.W5 = src.W5;
        }

        public void fromRaw(byte[] bs)
        {
            fromRaw(bs, 0);
        }

        public void fromRaw(byte[] bs, int p)
        {
            W1 = NB.DecodeInt32(bs, p);
            W2 = NB.DecodeInt32(bs, p + 4);
            W3 = NB.DecodeInt32(bs, p + 8);
            W4 = NB.DecodeInt32(bs, p + 12);
            W5 = NB.DecodeInt32(bs, p + 16);
        }

        public void fromRaw(int[] ints)
        {
            fromRaw(ints, 0);
        }

        public void fromRaw(int[] ints, int p)
        {
            W1 = ints[p];
            W2 = ints[p + 1];
            W3 = ints[p + 2];
            W4 = ints[p + 3];
            W5 = ints[p + 4];
        }

        public void fromString(byte[] buf, int offset)
        {
            fromHexString(buf, offset);
        }

        public void fromString(String str)
        {
            if (str.Length != Constants.StringLength)
                throw new ArgumentException("Invalid id: " + str);
            fromHexString(Encoding.ASCII.GetBytes(str), 0);
        }

        private void fromHexString(byte[] bs, int p)
        {
            try
            {
                W1 = Hex.HexStringToUInt32(bs, p);
                W2 = Hex.HexStringToUInt32(bs, p + 8);
                W3 = Hex.HexStringToUInt32(bs, p + 16);
                W4 = Hex.HexStringToUInt32(bs, p + 24);
                W5 = Hex.HexStringToUInt32(bs, p + 32);
            }
            catch (IndexOutOfRangeException)
            {
                try
                {
                    String str = new string(Encoding.ASCII.GetChars(bs, p, Constants.StringLength));
                    throw new ArgumentException("Invalid id: " + str);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Invalid id");
                }
            }
        }

        public override ObjectId ToObjectId()
        {
            throw new NotImplementedException();
        }
    }
}
