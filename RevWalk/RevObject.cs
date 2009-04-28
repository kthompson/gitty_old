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

namespace Gitty.Core.RevWalk
{
    public abstract class RevObject : ObjectId
    {
        protected static readonly int PARSED = 1;

        protected int flags;

        protected RevObject(AnyObjectId name)
            : base(name)
        {

        }

        protected abstract void Parse(RevWalk walk);

        public abstract ObjectType ObjectType { get; }

        public ObjectId Id
        {
            get
            {
                return this;
            }
        }

        public override bool Equals(AnyObjectId obj)
        {
            return this == obj;
        }

        public override bool Equals(object obj)
        {
            return this == obj;
        }

        /// <summary>
        /// Test to see if the flag has been set on this object.
        /// </summary>
        /// <param name="flag">the flag to test</param>
        /// <returns>true if the flag has been added to this object; false if not</returns>
        public bool Has(RevFlag flag)
        {
            return (flags & flag.Mask) != 0;
        }

        /// <summary>
        /// Test to see if any flag in the set has been set on this object.
        /// </summary>
        /// <param name="flag">the flags to test.</param>
        /// <returns>true if any flag in the set has been added to this object; false if not.</returns>
        public bool HasAny(RevFlagSet set)
        {
            return (flags & set.Mask) != 0;
        }

        /// <summary>
        /// Test to see if all flags in the set have been set on this object.
        /// </summary>
        /// <param name="set">the flags to test</param>
        /// <returns>true if all flags of the set have been added to this object; false if some or none have been added.</returns>
        public bool HasAll(RevFlagSet set)
        {
            return (flags & set.Mask) == set.Mask;
        }

        /**
         * Add a flag to this object.
         * <p>
         * If the flag is already set on this object then the method has no effect.
         * 
         * @param flag
         *            the flag to mark on this object, for later testing.
         */
        public void Add(RevFlag flag)
        {
            flags |= flag.Mask;
        }

        /**
         * Add a set of flags to this object.
         * 
         * @param set
         *            the set of flags to mark on this object, for later testing.
         */
        public void Add(RevFlagSet set)
        {
            flags |= set.Mask;
        }

        /**
         * Remove a flag from this object.
         * <p>
         * If the flag is not set on this object then the method has no effect.
         * 
         * @param flag
         *            the flag to remove from this object.
         */
        public void Remove(RevFlag flag)
        {
            flags &= ~flag.Mask;
        }

        /**
         * Remove a set of flags from this object.
         * 
         * @param set
         *            the flag to remove from this object.
         */
        public void Remove(RevFlagSet set)
        {
            flags &= ~set.Mask;
        }

        /** Release as much memory as possible from this object. */
        public virtual void Dispose()
        {
            // Nothing needs to be done for most objects.
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(Codec.TypeString(this.ObjectType));
            s.Append(' ');
            s.Append(Name());
            s.Append(' ');
            AppendCoreFlags(s);
            return s.ToString();
        }

        /**
	     * @param s
	     *            buffer to append a debug description of core RevFlags onto.
	     */
        protected void AppendCoreFlags(StringBuilder s)
        {
            s.Append((flags & RevWalk.TOPO_DELAY) != 0 ? 'o' : '-');
            s.Append((flags & RevWalk.TEMP_MARK) != 0 ? 't' : '-');
            s.Append((flags & RevWalk.REWRITE) != 0 ? 'r' : '-');
            s.Append((flags & RevWalk.UNINTERESTING) != 0 ? 'u' : '-');
            s.Append((flags & RevWalk.SEEN) != 0 ? 's' : '-');
            s.Append((flags & RevWalk.PARSED) != 0 ? 'p' : '-');
        }

    }
}
