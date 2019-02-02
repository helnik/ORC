using System;

namespace ObjectRuleChecker
{
    internal static class Extensions
    {
        internal static bool HasContext(this string s) => !string.IsNullOrWhiteSpace(s);

        internal static bool HasNoContext(this string s) => !s.HasContext();

        internal static void EnsureContext(this string s, string errorMessage)
        {
            if (s.HasNoContext()) throw  new ArgumentException("errorMessage");
        }
    }
}
