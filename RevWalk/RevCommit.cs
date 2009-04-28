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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core.Exceptions;

namespace Gitty.Core.RevWalk
{
    /** A commit reference to a commit in the DAG. */
    public class RevCommit : RevObject
    {
        static readonly RevCommit[] NO_PARENTS = { };

        private static readonly string TYPE_COMMIT = Constants.ObjectTypes.Commit;

        public RevTree Tree { get; protected set; }

        /// <summary>
        /// Obtain an array of all parents (<b>NOTE - THIS IS NOT A COPY</b>).
        /// <p>
        /// This method is exposed only to provide very fast, efficient access to
        /// this commit's parent list. Applications relying on this list should be
        /// very careful to ensure they do not modify its contents during their use
        /// of it.
        /// </summary>
        public RevCommit[] Parents { get; protected set; }

        public int CommitTime { get; protected set; } // An int here for performance, overflows in 2038

        int inDegree;

        /// <summary>
        /// Obtain the raw unparsed commit body (<b>NOTE - THIS IS NOT A COPY</b>).
        /// <p>
        /// This method is exposed only to provide very fast, efficient access to
        /// this commit's message buffer within a RevFilter. Applications relying on
        /// this buffer should be very careful to ensure they do not modify its
        /// contents during their use of it.
        /// </summary>
        public byte[] RawBuffer { get; protected set; }

        /**
	     * Create a new commit reference.
	     * 
	     * @param id
	     *            object name for the commit.
	     */
        protected RevCommit(AnyObjectId id)
            : base(id)
        {

        }


        protected override void Parse(RevWalk walk)
        {
            throw new NotImplementedException();
            //ObjectLoader ldr = walk.db.openObject(walk.curs, this);
            //if (ldr == null)
            //    throw new MissingObjectException(this, ObjectType.Commit);
            //byte[] data = ldr.CachedBytes;
            //if (Constants.ObjectTypes.Commit != ldr.ObjectType)
            //    throw new IncorrectObjectTypeException(this, ObjectType.Commit);
            //ParseCanonical(walk, data);
        }

        void ParseCanonical(RevWalk walk, byte[] raw)
        {
            throw new NotImplementedException();
            //MutableObjectId idBuffer = walk.idBuffer;
            //idBuffer.fromString(raw, 5);
            //this.Tree = walk.lookupTree(idBuffer);

            //int ptr = 46;
            //if (this.Parents == null)
            //{
            //    RevCommit[] pList = new RevCommit[1];
            //    int nParents = 0;
            //    for (; ; )
            //    {
            //        if (raw[ptr] != 'p')
            //            break;
            //        idBuffer.fromString(raw, ptr + 7);
            //        RevCommit p = walk.lookupCommit(idBuffer);
            //        if (nParents == 0)
            //            pList[nParents++] = p;
            //        else if (nParents == 1)
            //        {
            //            pList = new RevCommit[] { pList[0], p };
            //            nParents = 2;
            //        }
            //        else
            //        {
            //            if (pList.length <= nParents)
            //            {
            //                RevCommit[] old = pList;
            //                pList = new RevCommit[pList.length + 32];
            //                System.arraycopy(old, 0, pList, 0, nParents);
            //            }
            //            pList[nParents++] = p;
            //        }
            //        ptr += 48;
            //    }
            //    if (nParents != pList.length)
            //    {
            //        RevCommit[] old = pList;
            //        pList = new RevCommit[nParents];
            //        System.arraycopy(old, 0, pList, 0, nParents);
            //    }
            //    this.Parents = pList;
            //}

            //// extract time from "committer "
            //ptr = RawParseUtils.committer(raw, ptr);
            //if (ptr > 0)
            //{
            //    ptr = RawParseUtils.nextLF(raw, ptr, '>');

            //    // In 2038 commitTime will overflow unless it is changed to long.
            //    this.CommitTime = RawParseUtils.parseBase10(raw, ptr, null);
            //}

            //buffer = raw;
            //flags |= PARSED;
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.Commit; }
        }

        static void CarryFlags(RevCommit c, int carry)
        {
            for (; ; )
            {
                RevCommit[] pList = c.Parents;
                if (pList == null)
                    return;
                int n = pList.Length;
                if (n == 0)
                    return;

                for (int i = 1; i < n; i++)
                {
                    RevCommit p = pList[i];
                    if ((p.flags & carry) == carry)
                        continue;
                    p.flags |= carry;
                    CarryFlags(p, carry);
                }

                c = pList[0];
                if ((c.flags & carry) == carry)
                    return;
                c.flags |= carry;
            }
        }



        /**
         * Carry a RevFlag set on this commit to its parents.
         * <p>
         * If this commit is parsed, has parents, and has the supplied flag set on
         * it we automatically add it to the parents, grand-parents, and so on until
         * an unparsed commit or a commit with no parents is discovered. This
         * permits applications to force a flag through the history chain when
         * necessary.
         * 
         * @param flag
         *            the single flag value to carry back onto parents.
         */
        public void Carry(RevFlag flag)
        {
            int carry = flags & flag.Mask;
            if (carry != 0)
                CarryFlags(this, carry);
        }


        /**
         * Parse this commit buffer for display.
         * 
         * @param walk
         *            revision walker owning this reference.
         * @return parsed commit.
         */
        public Commit AsCommit(RevWalk walk)
        {
            return new Commit(walk.Repository, this, RawBuffer);
        }

        /// <summary>
        /// Get the number of parent commits listed in this commit.
        /// </summary>
        public int ParentCount
        {
            get
            {
                if (this.Parents == null)
                    return 0;
                return this.Parents.Length;
            }
        }


        /**
         * Get the nth parent from this commit's parent list.
         * 
         * @param nth
         *            parent index to obtain. Must be in the range 0 through
         *            {@link #getParentCount()}-1.
         * @return the specified parent.
         * @throws ArrayIndexOutOfBoundsException
         *             an invalid parent index was specified.
         */
        public RevCommit GetParent(int nth)
        {
            return this.Parents[nth];
        }

        /**
         * Parse the author identity from the raw buffer.
         * <p>
         * This method parses and returns the content of the author line, after
         * taking the commit's character set into account and decoding the author
         * name and email address. This method is fairly expensive and produces a
         * new PersonIdent instance on each invocation. Callers should invoke this
         * method only if they are certain they will be outputting the result, and
         * should cache the return value for as long as necessary to use all
         * information from it.
         * <p>
         * RevFilter implementations should try to use {@link RawParseUtils} to scan
         * the {@link #getRawBuffer()} instead, as this will allow faster evaluation
         * of commits.
         * 
         * @return identity of the author (name, email) and the time the commit was
         *         made by the author; null if no author line was found.
         */
        public PersonIdent GetAuthorIdent()
        {
            byte[] raw = RawBuffer;
            int nameB = RawParseUtils.Author(raw, 0);
            if (nameB < 0)
                return null;
            return RawParseUtils.ParsePersonIdent(raw, nameB);
        }



        /**
         * Parse the committer identity from the raw buffer.
         * <p>
         * This method parses and returns the content of the committer line, after
         * taking the commit's character set into account and decoding the committer
         * name and email address. This method is fairly expensive and produces a
         * new PersonIdent instance on each invocation. Callers should invoke this
         * method only if they are certain they will be outputting the result, and
         * should cache the return value for as long as necessary to use all
         * information from it.
         * <p>
         * RevFilter implementations should try to use {@link RawParseUtils} to scan
         * the {@link #getRawBuffer()} instead, as this will allow faster evaluation
         * of commits.
         * 
         * @return identity of the committer (name, email) and the time the commit
         *         was made by the committer; null if no committer line was found.
         */
        public PersonIdent GetCommitterIdent()
        {
            byte[] raw = RawBuffer;
            int nameB = RawParseUtils.Committer(raw, 0);
            if (nameB < 0)
                return null;
            return RawParseUtils.ParsePersonIdent(raw, nameB);
        }

        /**
         * Parse the complete commit message and decode it to a string.
         * <p>
         * This method parses and returns the message portion of the commit buffer,
         * after taking the commit's character set into account and decoding the
         * buffer using that character set. This method is a fairly expensive
         * operation and produces a new string on each invocation.
         * 
         * @return decoded commit message as a string. Never null.
         */
        public string GetFullMessage()
        {
            byte[] raw = RawBuffer;
            int msgB = RawParseUtils.CommitMessage(raw, 0);
            if (msgB < 0)
                return "";
            Encoding enc = RawParseUtils.ParseEncoding(raw);
            return RawParseUtils.Decode(enc, raw, msgB, raw.Length);
        }

        /**
         * Parse the commit message and return the first "line" of it.
         * <p>
         * The first line is everything up to the first pair of LFs. This is the
         * "oneline" format, suitable for output in a single line display.
         * <p>
         * This method parses and returns the message portion of the commit buffer,
         * after taking the commit's character set into account and decoding the
         * buffer using that character set. This method is a fairly expensive
         * operation and produces a new string on each invocation.
         * 
         * @return decoded commit message as a string. Never null. The returned
         *         string does not contain any LFs, even if the first paragraph
         *         spanned multiple lines. Embedded LFs are converted to spaces.
         */
        public string GetShortMessage()
        {
            byte[] raw = RawBuffer;
            int msgB = RawParseUtils.CommitMessage(raw, 0);
            if (msgB < 0)
                return "";

            Encoding enc = RawParseUtils.ParseEncoding(raw);
            int msgE = RawParseUtils.EndOfParagraph(raw, msgB);
            String str = RawParseUtils.Decode(enc, raw, msgB, msgE);
            if (HasLF(raw, msgB, msgE))
                str = str.Replace('\n', ' ');
            return str;
        }

        static bool HasLF(byte[] r, int b, int e)
        {
            while (b < e)
                if (r[b++] == '\n')
                    return true;
            return false;
        }

        /**
         * Reset this commit to allow another RevWalk with the same instances.
         * <p>
         * Subclasses <b>must</b> call <code>super.reset()</code> to ensure the
         * basic information can be correctly cleared out.
         */
        public void Reset()
        {
            inDegree = 0;
        }

        public void Dispose()
        {
            flags &= ~PARSED;
            RawBuffer = null;
        }


        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(Codec.TypeString(this.ObjectType));
            s.Append(' ');
            s.Append(Name());
            s.Append(' ');
            s.Append(CommitTime);
            s.Append(' ');
            AppendCoreFlags(s);
            return s.ToString();
        }
    }
}
