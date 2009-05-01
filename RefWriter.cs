/*
 * Copyright (C) 2008, Shawn O. Pearce <spearce@spearce.org>
 * Copyright (C) 2008, Charles O'Farrell <charleso@charleso.org>
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
using System.IO;

namespace Gitty.Core
{
    /// <summary>
    /// Writes out refs to the <see ref="Constants.InfoRefs"/> 
    /// and <see ref="Constants.PackedRefs"/> files.
    /// This class is abstract as the writing of the files must be handled by the
    /// caller. This is because it is used by transport classes as well.
    /// </summary>
    public abstract class RefWriter
    {
        private readonly List<Ref> _refs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefWriter"/> class.
        /// </summary>
        /// <param name="refs">the complete set of references. This should have been computed 
        /// by applying updates to the advertised refs already discovered.</param>
        public RefWriter(ICollection<Ref> refs)
        {
            this._refs = RefComparer.Instance.Sort(refs);
        }

        /// <summary>
        /// Rebuild the <see ref="Constants.InfoRefs"/>.
        /// <p>
        /// This method rebuilds the contents of the <see ref="Constants.InfoRefs"/> file
        /// to match the passed list of references.
        /// </summary>
        public void WriteInfoRefs()
        {
            StringWriter w = new StringWriter();
            char[] tmp = new char[Constants.ObjectId.Length * 2];
            foreach (Ref r in _refs)
            {
                if (Constants.Head.Equals(r.Name))
                {
                    // Historically HEAD has never been published through
                    // the INFO_REFS file. This is a mistake, but its the
                    // way things are.
                    //
                    continue;
                }

                r.ObjectId.CopyTo(tmp, w);
                w.Write('\t');
                w.Write(r.Name);
                w.Write('\n');

                if (r.PeeledObjectId != null)
                {
                    r.PeeledObjectId.CopyTo(tmp, w);
                    w.Write('\t');
                    w.Write(r.Name);
                    w.Write("^{}\n");
                }
            }
            WriteFile(Constants.Refs.InfoRefs, Encoding.ASCII.GetBytes(w.ToString()));
        }

        /// <summary>
        /// Rebuild the <see ref="Constants.PackedRefs"/> file.
        /// <p>
        /// This method rebuilds the contents of the <see ref="Constants.PackedRefs"/>
        /// file to match the passed list of references, including only those refs
        /// that have a storage type of <see ref="Ref.Storage.Packed"/>.
        /// </summary>
        public void WritePackedRefs()
        {
            bool peeled = false;

            foreach (Ref r in _refs)
            {
                if (r.StorageFormat != Ref.Storage.Packed)
                    continue;
                if (r.PeeledObjectId != null)
                    peeled = true;
            }

            StringWriter w = new StringWriter();
            if (peeled)
            {
                w.Write("# pack-refs with:");
                if (peeled)
                    w.Write(" peeled");
                w.Write('\n');
            }

            char[] tmp = new char[Constants.ObjectId.StringLength];
            foreach (Ref r in _refs)
            {
                if (r.StorageFormat != Ref.Storage.Packed)
                    continue;

                r.ObjectId.CopyTo(tmp, w);
                w.Write(' ');
                w.Write(r.Name);
                w.Write('\n');

                if (r.PeeledObjectId != null)
                {
                    w.Write('^');
                    r.PeeledObjectId.CopyTo(tmp, w);
                    w.Write('\n');
                }
            }
            WriteFile(Constants.Refs.PackedRefs, Encoding.ASCII.GetBytes(w.ToString()));
        }

        /// <summary>
        /// Handles actual writing of ref files to the git repository, which may
        /// differ slightly depending on the destination and transport.
        /// </summary>
        /// <param name="file">path to ref file.</param>
        /// <param name="content">byte content of file to be written.</param>
        protected abstract void WriteFile(string file, byte[] content);
    }
}
