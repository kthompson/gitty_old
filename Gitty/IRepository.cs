﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gitty
{
    public interface IRepository : IPath
    {
        IIndex Index { get; }
    }
}
