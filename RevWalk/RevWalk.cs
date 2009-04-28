/*
 * Copyright (C) 2007, Robin Rosenberg <robin.rosenberg@dewire.com>
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
    public class RevWalk 
    {

        /**
	 * Set on objects whose important header data has been loaded.
	 * <p>
	 * For a RevCommit this indicates we have pulled apart the tree and parent
	 * references from the raw bytes available in the repository and translated
	 * those to our own local RevTree and RevCommit instances. The raw buffer is
	 * also available for message and other header filtering.
	 * <p>
	 * For a RevTag this indicates we have pulled part the tag references to
	 * find out who the tag refers to, and what that object's type is.
	 */
        public static int PARSED = 1 << 0;

        /**
         * Set on RevCommit instances added to our {@link #pending} queue.
         * <p>
         * We use this flag to avoid adding the same commit instance twice to our
         * queue, especially if we reached it by more than one path.
         */
        public static int SEEN = 1 << 1;

        /**
         * Set on RevCommit instances the caller does not want output.
         * <p>
         * We flag commits as uninteresting if the caller does not want commits
         * reachable from a commit given to {@link #markUninteresting(RevCommit)}.
         * This flag is always carried into the commit's parents and is a key part
         * of the "rev-list B --not A" feature; A is marked UNINTERESTING.
         */
        public static int UNINTERESTING = 1 << 2;

        /**
         * Set on a RevCommit that can collapse out of the history.
         * <p>
         * If the {@link #treeFilter} concluded that this commit matches his
         * parents' for all of the paths that the filter is interested in then we
         * mark the commit REWRITE. Later we can rewrite the parents of a REWRITE
         * child to remove chains of REWRITE commits before we produce the child to
         * the application.
         * 
         * @see RewriteGenerator
         */
        public static int REWRITE = 1 << 3;

        /**
         * Temporary mark for use within generators or filters.
         * <p>
         * This mark is only for local use within a single scope. If someone sets
         * the mark they must unset it before any other code can see the mark.
         */
        public static int TEMP_MARK = 1 << 4;

        /**
         * Temporary mark for use within {@link TopoSortGenerator}.
         * <p>
         * This mark indicates the commit could not produce when it wanted to, as at
         * least one child was behind it. Commits with this flag are delayed until
         * all children have been output first.
         */
        public static int TOPO_DELAY = 1 << 5;

        /** Number of flag bits we keep internal for our own use. See above flags. */
        public static int RESERVED_FLAGS = 6;

        private static int APP_FLAGS = -1 & ~((1 << RESERVED_FLAGS) - 1);


        public Repository Repository { get; private set; }

    }
}
