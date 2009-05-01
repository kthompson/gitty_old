﻿/*
 * Copyright (C) 2008, Google Inc.
 * Copyright (C) 2009, Stanley Lara <stanley.lara.iii@gmail.com>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * - Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above
 *   copyright notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 * - Neither the name of the Git Development Community nor the
 *   names of its contributors may be used to endorse or promote
 *   products derived from this software without specific prior
 *   written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core.Util;

namespace Gitty.Core
{
    // A prefix abbreviation of an {@link ObjectId}.
    //
    // Sometimes Git produces abbreviated SHA-1 strings, using sufficient leading
    // digits from the ObjectId name to still be unique within the repository the
    // string was generated from. These ids are likely to be unique for a useful
    // period of time, especially if they contain at least 6-10 hex digits.
    //
    // This class converts the hex string into a binary form, to make it more
    // efficient for matching against an object.

    public class AbbreviatedObjectId
    {
        // Convert an AbbreviatedObjectId from hex characters (US-ASCII).
        //
        // @param buf
        //              the US-ASCII buffer to read from.
        // @param end
        //              position to read the first character from.
        // @param end
        //              one past the lastposition to read (<code>end-offset</code> is
        //              the length of the string).
        // @return the converted object id.

        public static AbbreviatedObjectId FromString(byte[] buf, int offset, int end)
        {
            if (end - offset < AnyObjectId.Constants.StringLength)
            {
                throw new ArgumentException("Invalid id");
            }
            return FromHexString(buf, offset, end);
        }

        // Convert an AbbreviatedObjectId from hex characters.
        //
        // @param str
        //              the string to read from. Must be &lt;= 40 characters.
        // @return the converted object id.

        public static AbbreviatedObjectId FromString(String str)
        {
            if (str.Length > AnyObjectId.Constants.StringLength)
            {
                throw new ArgumentException("Invalid id: " + str);
            }
            byte[] b = Encoding.ASCII.GetBytes(str);
            return FromHexString(b, 0, b.Length);
        }

        private static AbbreviatedObjectId FromHexString(byte[] bs, int ptr, int end)
        {
            try
            {
                int a = HexUInt32(bs, ptr, end);
                int b = HexUInt32(bs, ptr + 8, end);
                int c = HexUInt32(bs, ptr + 16, end);
                int d = HexUInt32(bs, ptr + 24, end);
                int e = HexUInt32(bs, ptr + 32, end);
                return new AbbreviatedObjectId(end - ptr, a, b, c, d, e);
            }
            catch (IndexOutOfRangeException e1)
            {
                try
                {
                    String str = Encoding.ASCII.GetString(bs, ptr, end - ptr);
                    throw new ArgumentException("Invalid id: " + str);
                }
                catch (System.Threading.ThreadInterruptedException)
                {
                    throw;
                }
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw new ArgumentException("Invalid id");
                }
            }
        }

        private static int HexUInt32(byte[] bs, int p, int end)
        {
            if (8 <= end - p)
            {
                return Hex.HexStringToUInt32(bs, p);
            }

            int r = 0, n = 0;
            while (n < 8 && p < end)
            {
                int v = Hex.HexCharToValue(bs[p++]);
                if (v < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                r <<= 4;
                r |= v;
                n++;
            }
            return r << (8 - n) * 4;
        }

        internal static int Mask(int nibbles, int word, int v)
        {
            int b = (word - 1) * 8;
            if (b + 8 <= nibbles)
            {
                // We have all of the bits required for this word.
                return v;
            }

            if (nibbles < b)
            {
                // We have none of the bits required for this word.
                return 0;
            }

            int s = 32 - (nibbles - b) * 4;
            return (v.UnsignedRightShift(s)) << s;
        }

        // Number of half-bytes used by this id.
        int nibbles;
        int w1;
        int w2;
        int w3;
        int w4;
        int w5;

        internal AbbreviatedObjectId(int n, int new_1, int new_2, int new_3, int new_4, int new_5)
        {
            nibbles = n;
            w1 = new_1;
            w2 = new_2;
            w3 = new_3;
            w4 = new_4;
            w5 = new_5;
        }

        // @return number of hex digits appearing in this id
        public int Length
        {
            get
            {
                return nibbles;
            }
        }

        // @return true if this ObjectId is actually a complete id.
        public bool IsComplete
        {
            get
            {
                return Length == AnyObjectId.Constants.ObjectIdLength * 2;
            }
        }

        // @return a complete ObjectId; null if {@link #isComplete()} is false
        public ObjectId ToObjectId()
        {
            return IsComplete ? new ObjectId(w1, w2, w3, w4, w5) : null;
        }

        // Compares this abbreviation to a full object id.
        //
        // @param other
        //            the other object id.
        // @return &lt;0 if this abbreviation names an object that is less than
        //         <code>other</code>; 0 if this abbreviation exactly matches the
        //         first {@link #length()} digits of <code>other.name()</code>;
        //         &gt;0 if this abbreviation names an object that is after
        //         <code>other</code>.

        public int PrefixCompare(AnyObjectId other)
        {
            int cmp;

            cmp = NB.CompareUInt32(w1, Mask(1, other.W1));
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = NB.CompareUInt32(w2, Mask(2, other.W2));
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = NB.CompareUInt32(w3, Mask(3, other.W3));
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = NB.CompareUInt32(w4, Mask(4, other.W4));
            if (cmp != 0)
            {
                return cmp;
            }

            return NB.CompareUInt32(w5, Mask(5, other.W5));
        }

        private int Mask(int word, int v)
        {
            return Mask(nibbles, word, v);
        }

        public override int GetHashCode()
        {
            return w2;
        }

        public override bool Equals(Object o)
        {
            if (o is AbbreviatedObjectId)
            {
                AbbreviatedObjectId b = (AbbreviatedObjectId)o;
                return nibbles == b.nibbles && w1 == b.w1 && w2 == b.w2
                    && w3 == b.w3 && w4 == b.w4 && w5 == b.w5;
            }
            return false;
        }

        // @return string form of the abbreviation, in lower case hexadecimal.

        public String Name()
        {
            char[] b = new char[AnyObjectId.Constants.StringLength];

            Hex.FillHexCharArray(b, 0, w1);

            if (nibbles <= 8)
            {
                return new String(b, 0, nibbles);
            }

            Hex.FillHexCharArray(b, 8, w2);
            if (nibbles <= 16)
                return new String(b, 0, nibbles);

            Hex.FillHexCharArray(b, 16, w3);
            if (nibbles <= 24)
                return new String(b, 0, nibbles);

            Hex.FillHexCharArray(b, 24, w4);
            if (nibbles <= 32)
                return new String(b, 0, nibbles);

            Hex.FillHexCharArray(b, 32, w5);
            return new String(b, 0, nibbles);
        }

        public override String ToString()
        {
            return "AbbreviatedObjectId[" + Name() + "]";
        }
    }
}
