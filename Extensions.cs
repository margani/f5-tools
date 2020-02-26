using System;
using System.Collections.Generic;
using System.Text;

namespace F5Tools
{
    public static class Extensions
    {
        public static string PadBoth(this string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = (spaces / 2) + source.Length;
            return source.PadLeft(padLeft).PadRight(length);
        }
    }
}
