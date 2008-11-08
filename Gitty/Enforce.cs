using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gitty
{
    public class Enforce
    {
        private Enforce()
        {
            
        }

        public static void ArgumentNotNull<T>(T arg, string name) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(name);
        }

        public static void ArgumentNotNullOrEmpty(string arg, string name)
        {
            if (string.IsNullOrEmpty(arg))
                throw new ArgumentNullException(name);
        }

        public static void ArgumentGreaterThan(long arg, long expected, string name)
        {
            if (arg < expected)
                throw new ArgumentException(string.Format("Argument must be at least {0}", expected), name);
        }

        public static void ArgumentGreaterThanOrEqual(long arg, long expected, string name)
        {
            if (arg <= expected)
                throw new ArgumentException(string.Format("Argument must be at least {0}", expected), name);
        }
    }
}
