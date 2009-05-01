/*
 * Copyright (C) 2008, Shawn O. Pearce <spearce@spearce.org>
 * Copyright (C) 2009, Kevin Thompson <kevin.thompson@theautomaters.com>
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
namespace Gitty.Core
{

    public static class RawParseUtils
    {

        public static int ParseBase10(byte[] b, ref int ptr)
        {
            int r = 0;
            int sign = 1;

            try
            {
                int sz = b.Length;
                while (ptr < sz && b[ptr] == ' ')
                    ptr++;

                if (ptr >= sz)
                    return 0;

                if (b[ptr] == '-')
                {
                    sign = -1;
                    ptr++;
                }
                else if (b[ptr] == '+')
                    ptr++;

                while (ptr < sz)
                {
                    byte d = b[ptr];
                    if ((d < (byte)'0') | (d > (byte)'9'))
                        break;
                    r = r * 10 + (d - (byte)'0');
                    ptr++;
                }
            }
            catch (IndexOutOfRangeException)
            {
                // Not a valid digit
            }

            return sign * r;
        }

        internal static int Author(byte[] raw, int p)
        {
            throw new NotImplementedException();
        }

        internal static PersonIdent ParsePersonIdent(byte[] raw, int nameB)
        {
            throw new NotImplementedException();
        }

        internal static int Committer(byte[] raw, int p)
        {
            throw new NotImplementedException();
        }

        internal static int CommitMessage(byte[] raw, int p)
        {
            throw new NotImplementedException();
        }

        internal static System.Text.Encoding ParseEncoding(byte[] raw)
        {
            throw new NotImplementedException();
        }

        internal static string Decode(System.Text.Encoding enc, byte[] raw, int msgB, int p)
        {
            throw new NotImplementedException();
        }

        internal static int EndOfParagraph(byte[] raw, int msgB)
        {
            throw new NotImplementedException();
        }
    }
}