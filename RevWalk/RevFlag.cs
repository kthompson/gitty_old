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
    /**
     * Application level mark bit for {@link RevObject}s.
     * <p>
     * To create a flag use {@link RevWalk#newFlag(String)}.
     */
    public class RevFlag
    {
        /**
         * Uninteresting by {@link RevWalk#markUninteresting(RevCommit)}.
         * <p>
         * We flag commits as uninteresting if the caller does not want commits
         * reachable from a commit to {@link RevWalk#markUninteresting(RevCommit)}.
         * This flag is always carried into the commit's parents and is a key part
         * of the "rev-list B --not A" feature; A is marked UNINTERESTING.
         * <p>
         * This is a static flag. Its RevWalk is not available.
         */
        public static RevFlag UNINTERESTING = new StaticRevFlag("UNINTERESTING", RevWalk.UNINTERESTING);

        public RevWalk Walker { get; protected set; }

        public String Name { get; protected set; }

        public int Mask { get; protected set; }

        protected RevFlag(RevWalk w, string n, int m)
        {
            Walker = w;
            Name = n;
            Mask = m;
        }

        public override string ToString()
        {
            return Name;
        }

        class StaticRevFlag : RevFlag
        {
            public StaticRevFlag(String n, int m)
                : base(null, n, m)
            {
            }


            public RevWalk getRevWalk()
            {
                throw new NotSupportedException(ToString()
                        + " is a static flag and has no RevWalk instance");
            }
        }
    }
}
