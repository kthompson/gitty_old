using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IStatus : IDictionary<string,IStatusFile>
    {
        IEnumerable<IStatusFile> Changed { get; }
        IEnumerable<IStatusFile> Added { get; }
        IEnumerable<IStatusFile> Deleted { get; }
        IEnumerable<IStatusFile> Untracked { get; }
    }
}
