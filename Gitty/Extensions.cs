using System;
using System.Collections.Generic;
using System.Text;

namespace Gitty
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this object s)
        {
            if (s is string)
                return string.IsNullOrEmpty(s as string);
            else
                return (s == null);
        }

        public static bool IsNotNullOrEmpty(this object s)
        {
            return !(s.IsNullOrEmpty());
        }
    }
}
