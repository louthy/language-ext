using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;

namespace LanguageExt
{
    public static partial class Set
    {
        public static Set<T> create<T>() =>
            Set<T>.Empty;

        public static Set<T> createRange<T>(IEnumerable<T> range) =>
            new Set<T>(range);

        public static Set<T> empty<T>() =>
            Set<T>.Empty;

        public static Set<T> add<T>(Set<T> set, T value) =>
            set.Add(value);

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        public static bool compare<T>(Set<T> setA, Set<T> setB) =>
            setA.SetEquals(setB);

        public static int length<T>(Set<T> set) =>
            set.Count();

        /// <summary>
        /// Returns setA - setB.  Only the items in setA that are not in setB will be left.
        /// </summary>
        public static Set<T> difference<T>(Set<T> setA, Set<T> setB) =>
            setA.Except(setB);

        public static bool exists<T>(Set<T> set, Func<T, bool> pred)
        {
            foreach (var item in set)
            {
                if (pred(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Create a new set that contains all elements of both sets
        /// </summary>
        public static Set<T> union<T>(Set<T> setA, Set<T> setB) =>
            setA.Union(setB);

        public static IEnumerable<T> filter<T>(Set<T> set, Func<T, bool> pred) =>
            set.Where(pred);

        /// <summary>
        /// Returns the elements that are in both setA and setB
        /// </summary>
        public static Set<T> intersect<T>(Set<T> setA, Set<T> setB) =>
            setA.SymmetricExcept(setB);

        public static IEnumerable<R> map<T, R>(Set<T> set, Func<T, R> mapper) =>
            set.Select(mapper);

        public static bool contains<T>(Set<T> set, T value) =>
            set.Contains(value);

        public static Set<T> remove<T>(Set<T> set, T value) =>
            set.Remove(value);

        public static bool isSubset<T>(Set<T> setA, Set<T> setB) =>
            setA.IsSubsetOf(setB);

        public static bool isProperSubset<T>(Set<T> setA, Set<T> setB) =>
            setA.IsProperSubsetOf(setB);
    }
}


public static class __SetExt
{
    public static bool Compare<T>(this Set<T> setA, Set<T> setB) =>
        LanguageExt.Set.compare(setA, setB);

    public static int Length<T>(this Set<T> set) =>
        LanguageExt.Set.length(set);

    public static Set<T> Difference<T>(this Set<T> setA, Set<T> setB) =>
        LanguageExt.Set.difference(setA, setB);

    public static IEnumerable<T> Filter<T>(this Set<T> set, Func<T, bool> pred) =>
        LanguageExt.Set.filter(set, pred);

    public static Set<T> Intersect<T>(this Set<T> setA, Set<T> setB) =>
        LanguageExt.Set.intersect(setA, setB);

    public static IEnumerable<R> Map<T, R>(this Set<T> set, Func<T, R> mapper) =>
        LanguageExt.Set.map(set, mapper);

    public static Option<T> find<T>(Set<T> set, T value) =>
        set.Find(value);

    public static bool exists<T>(Set<T> set, Func<T, bool> pred) =>
        LanguageExt.Set.exists(set, pred);
}