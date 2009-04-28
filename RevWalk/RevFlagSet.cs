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
    public class RevFlagSet : IList<RevFlag>
    {
        public int Mask { get; private set; }

        private List<RevFlag> active;

        /** Create an empty set of flags. */
        public RevFlagSet()
        {
            active = new List<RevFlag>();
        }

        /**
	     * Create a set of flags, copied from an existing set.
	     * 
	     * @param s
	     *            the set to copy flags from.
	     */
        public RevFlagSet(RevFlagSet s)
        {
            Mask = s.Mask;
            active = new List<RevFlag>(s.active);
        }

        /**
         * Create a set of flags, copied from an existing collection.
         * 
         * @param s
         *            the collection to copy flags from.
         */
        public RevFlagSet(IEnumerable<RevFlag> s)
            : this()
        {
            AddRange(s);
        }



        #region IList<RevFlag> Members
        
        public bool Contains(RevFlag item)
        {
            return (Mask & item.Mask) != 0;
        }

        public bool ContainsAll(RevFlagSet item)
        {
            var mask = item.Mask;
            return (Mask & mask) == mask;
        }

        public int IndexOf(RevFlag item)
        {
            return active.IndexOf(item);
        }

        public void Insert(int index, RevFlag item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        public RevFlag this[int index]
        {
            get
            {
                return active[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<RevFlag> Members

        public void Add(RevFlag flag)
        {
            if ((Mask & flag.Mask) != 0)
                return;
            Mask |= flag.Mask;
            int p = 0;
            while (p < active.Count && active[p].Mask < flag.Mask)
                p++;
            active.Insert(p, flag);
        }

        public void AddRange(IEnumerable<RevFlag> flags)
        {
            foreach (var flag in flags)
                this.Add(flag);
        }

        public void Clear()
        {
            while (active.Count > 0)
            {
                var flag = active[0];
                Mask &= ~flag.Mask;
                active.RemoveAt(0);
            }
        }

        public void CopyTo(RevFlag[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return active.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(RevFlag flag)
        {
            if ((Mask & flag.Mask) == 0)
                return false;
            Mask &= ~flag.Mask;
            for (int i = 0; i < active.Count; i++)
                if (active[i].Mask == flag.Mask)
                    active.RemoveAt(i);
            return true;
        }

        #endregion

        #region IEnumerable<RevFlag> Members

        public IEnumerator<RevFlag> GetEnumerator()
        {
            return active.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return active.GetEnumerator();
        }

        #endregion
    }
}
