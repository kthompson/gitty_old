using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public enum DiffMode
    {
        Add = 'A',
        Copy = 'C',
        Delete = 'D',
        Modification = 'M',
        Rename = 'R',
        TypeChange = 'T',
        Unmerged = 'U',
        Unknown = 'X',
    }

    public interface IDiffFilesFile: IWorkingDirectoryFile, IIndexFile
    {
        DiffMode Type{ get; }
    }
}
