using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IStatus : IDictionary<string,IStatusFile>
    {
        List<IStatusFile> Changed { get; }
        List<IStatusFile> Added { get; }
        List<IStatusFile> Deleted { get; }
        List<IStatusFile> Untracked { get; }
    }
}
