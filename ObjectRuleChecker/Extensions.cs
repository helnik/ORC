﻿using System;

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

        //internal static HashSet<ValidationRule<T>> ToHashSet<T>(this IEnumerable<ValidationRule<T>> enumerable) where T : class
        //{
        //    //excluded from project code exists in directory
        //    return new HashSet<ValidationRule<T>>(enumerable, ValidationRuleExpressionEqualityComparer<T>.Instance);
        //}
    }
}
