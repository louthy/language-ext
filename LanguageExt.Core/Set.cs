using System;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;

namespace LanguageExt
{
    public static partial class Set
    {
        public static IImmutableSet<T> add<T>(IImmutableSet<T> set, T value) =>
            set.Add(value);

        public static bool compare<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.SetEquals(setB);

        public static int length<T>(IImmutableSet<T> set) =>
           set.Count();

        public static IImmutableSet<T> difference<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.Except(setB);

        public static bool exists<T>(IImmutableSet<T> set, Func<T, bool> pred)
        {
            foreach (var item in set)
            {
                if (pred(item))
                    return true;
            }
            return false;
        }

        public static IEnumerable<T> filter<T>(IImmutableSet<T> set, Func<T, bool> pred) =>
            set.Where(pred);

        public static IImmutableSet<T> intersect<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.SymmetricExcept(setB);

        public static IEnumerable<R> map<T, R>(IImmutableSet<T> set, Func<T, R> mapper) =>
            set.Select(mapper);

        public static bool contains<T>(IImmutableSet<T> set, T value) =>
            set.Contains(value);

        public static IImmutableSet<T> remove<T>(IImmutableSet<T> set, T value) =>
            set.Remove(value);

        public static bool isSubset<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.IsSubsetOf(setB);

        public static bool isProperSubset<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.IsProperSubsetOf(setB);
    }
}

public static class __SetExt
{
    public static bool Compare<T>(this IImmutableSet<T> setA, IImmutableSet<T> setB) =>
        Set.compare(setA, setB);

    public static int Length<T>(this IImmutableSet<T> set) =>
        Set.length(set);

    public static IImmutableSet<T> Difference<T>(this IImmutableSet<T> setA, IImmutableSet<T> setB) =>
        Set.difference(setA, setB);

    public static bool exists<T>(this IImmutableSet<T> set, Func<T, bool> pred) =>
        Set.exists(set, pred);

    public static IEnumerable<T> Filter<T>(this IImmutableSet<T> set, Func<T, bool> pred) =>
        Set.filter(set, pred);

    public static IImmutableSet<T> Intersect<T>(this IImmutableSet<T> setA, IImmutableSet<T> setB) =>
        Set.intersect(setA, setB);

    public static IEnumerable<R> Map<T, R>(this IImmutableSet<T> set, Func<T, R> mapper) =>
        Set.map(set, mapper);
}