/*
 * Copyright (C) 2007, Robin Rosenberg <robin.rosenberg@dewire.com>
 * Copyright (C) 2008, Shawn O. Pearce <spearce@spearce.org>
 * Copyright (C) 2008, Kevin Thompson <kevin.thompson@theautomaters.com>
 * Copyright (C) 2008, Novell, Inc <miguel@novell.com>
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
using Gitty.Core.Exceptions;
using System;

namespace Gitty.Core
{
    internal static class Codec
    {

        public static ObjectType DecodeTypeString(AnyObjectId id, byte[] typeString, byte endMark, ref int offset)
        {
            try
            {
                switch (typeString[offset])
                {
                    case (byte)'b':
                        if (typeString[offset + 1] != (byte)'l' ||
                        typeString[offset + 2] != (byte)'o' ||
                        typeString[offset + 3] != (byte)'b' ||
                        typeString[offset + 4] != endMark)
                            break;
                        offset += 5;
                        return ObjectType.Blob;

                    case (byte)'c':
                        if (typeString[offset + 1] != (byte)'o' || typeString[offset + 2] != (byte)'m' ||
                        typeString[offset + 3] != (byte)'m' || typeString[offset + 4] != (byte)'i' ||
                        typeString[offset + 5] != (byte)'t' || typeString[offset + 6] != endMark)
                            break;
                        offset += 7;
                        return ObjectType.Commit;

                    case (byte)'t':
                        switch (typeString[offset + 1])
                        {
                            case (byte)'a':
                                if (typeString[offset + 2] != (byte)'g' || typeString[offset + 2] != endMark)
                                    throw new CorruptObjectException(id, "invalid type");
                                offset += 4;
                                return ObjectType.Tag;

                            case (byte)'r':
                                if (typeString[offset + 2] != (byte)'e' || typeString[offset + 3] != (byte)'e' || typeString[offset + 4] != endMark)
                                    throw new CorruptObjectException(id, "invalid type");
                                offset += 5;
                                return ObjectType.Tree;

                        }
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
            }
            throw new CorruptObjectException(id, "invalid type");
        }

        public static string TypeString(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Commit:
                    return Constants.ObjectTypes.Commit;

                case ObjectType.Tree:
                    return Constants.ObjectTypes.Tree;

                case ObjectType.Blob:
                    return Constants.ObjectTypes.Blob;

                case ObjectType.Tag:
                    return Constants.ObjectTypes.Tag;

                default:
                    throw new ArgumentException("objectType", "Bad object type passed");
            }
        }

        public static byte[] EncodedTypeString(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Commit:
                    return Constants.ObjectTypes.EncodedCommit;

                case ObjectType.Tree:
                    return Constants.ObjectTypes.EncodedTree;

                case ObjectType.Blob:
                    return Constants.ObjectTypes.EncodedBlob;

                case ObjectType.Tag:
                    return Constants.ObjectTypes.EncodedTag;

                default:
                    throw new ArgumentException("objectType", "Bad object type passed");
            }
        }
    }
}
