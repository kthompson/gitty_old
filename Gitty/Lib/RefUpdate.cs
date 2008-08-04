using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty.Lib
{
    public class RefUpdate
    {
        public enum Result
        {
            NotAttempted,
            LockFailure,
            NoChange,
            New,
            Forced, 
            FastForward,
            Rejected,
            IOFailure,
        }



        internal void SetNewObjectId(ObjectId id)
        {
            throw new NotImplementedException();
        }

        internal void SetRefLogMessage(string p, bool p_2)
        {
            throw new NotImplementedException();
        }

        internal Result ForceUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
