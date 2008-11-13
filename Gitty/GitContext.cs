using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    /// <summary>
    /// This class will be used to implement a wrapper when certain information will be ran multiple times.
    /// 
    /// The intent is to cache these methods to improve speed
    /// </summary>
    public class GitContext
    {

        public IGit GitInstance { get; private set; }

        public GitContext(IGit instance)
        {
            GitInstance = instance;
        }

        private IStatus status;
        public IStatus Status
        {
            get
            {
                if (status == null)
                    status = GitInstance.Status();

                return status;
            }
        }
    }
}