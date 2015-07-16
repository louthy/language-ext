using System;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using System.Collections;

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

        public static Set<T> add<T>(IImmutableSet<T> set, T value) =>
            Wrap(set.Add(value));

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        public static bool compare<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.SetEquals(setB);

        public static int length<T>(IImmutableSet<T> set) =>
            set.Count();

        /// <summary>
        /// Returns setA - setB.  Only the items in setA that are not in setB will be left.
        /// </summary>
        public static Set<T> difference<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            Wrap(setA.Except(setB));

        public static bool exists<T>(IImmutableSet<T> set, Func<T, bool> pred)
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
        public static Set<T> union<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            Wrap(setA.Union(setB));

        public static IEnumerable<T> filter<T>(IImmutableSet<T> set, Func<T, bool> pred) =>
            set.Where(pred);

        /// <summary>
        /// Returns the elements that are in both setA and setB
        /// </summary>
        public static Set<T> intersect<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            Wrap(setA.SymmetricExcept(setB));

        public static IEnumerable<R> map<T, R>(IImmutableSet<T> set, Func<T, R> mapper) =>
            set.Select(mapper);

        public static bool contains<T>(IImmutableSet<T> set, T value) =>
            set.Contains(value);

        public static Set<T> remove<T>(IImmutableSet<T> set, T value) =>
            Wrap(set.Remove(value));

        public static bool isSubset<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.IsSubsetOf(setB);

        public static bool isProperSubset<T>(IImmutableSet<T> setA, IImmutableSet<T> setB) =>
            setA.IsProperSubsetOf(setB);

        internal static Set<T> Wrap<T>(IImmutableSet<T> set) =>
            new Set<T>(set);

    }

    /// <summary>
    /// Immutable set
    /// </summary>
    /// <remarks>Wraps System.Collections.Immutable.ImmutableHashSet</remarks>
    /// <typeparam name="T">List item type</typeparam>
    public class Set<T> : IImmutableSet<T>
    {
        public static readonly Set<T> Empty = new Set<T>();

        IImmutableSet<T> set;

        internal Set()
        {
            set = ImmutableHashSet.Create<T>();
        }

        internal Set(IEnumerable<T> items)
        {
            set = ImmutableHashSet.CreateRange(items);
        }

        internal Set(IImmutableSet<T> wrapped)
        {
            set = wrapped;
        }

        public int Count
        {
            get
            {
                return set.Count;
            }
        }

        int IReadOnlyCollection<T>.Count
        {
            get
            {
                return Count;
            }
        }

        public Set<T> Add(T value)
        {
            if (value == null) throw new ArgumentNullException("'value' cannot be null.");
            return Set.Wrap(set.Add(value));
        }

        public Option<T> Find(T value)
        {
            T ret;
            return TryGetValue(value, out ret)
                ? Prelude.Some(ret)
                : Prelude.None;
        }

        public Set<T> Intersect(IEnumerable<T> other) =>
            Set.Wrap(set.Intersect(other));

        public Set<T> Except(IEnumerable<T> other) =>
            Set.Wrap(set.Except(other));

        public Set<T> SymmetricExcept(IEnumerable<T> other) =>
            Set.Wrap(set.SymmetricExcept(other));

        public Set<T> Union(IEnumerable<T> other) =>
            Set.Wrap(set.Union(other));

        public Set<T> Clear() =>
            Set.Wrap(set.Clear());

        public IEnumerator<T> GetEnumerator() =>
            set.GetEnumerator();

        public Set<T> Remove(T value) =>
            Set.Wrap(set.Remove(value));

        IEnumerator IEnumerable.GetEnumerator() =>
            set.GetEnumerator();

        public S Fold<S>(S state, Func<S, T, S> folder) =>
            this.Aggregate(state, folder);

        public Lst<U> Map<U>(Func<T, U> map) =>
            new Lst<U>(this.Select(map));

        public Lst<T> Filter(Func<T, bool> pred) =>
            new Lst<T>(this.Where(pred));

        IImmutableSet<T> IImmutableSet<T>.Clear() =>
            set.Clear();

        public bool Contains(T value) =>
            set.Contains(value);

        IImmutableSet<T> IImmutableSet<T>.Add(T value) =>
            set.Add(value);

        IImmutableSet<T> IImmutableSet<T>.Remove(T value) =>
            set.Remove(value);

        public bool TryGetValue(T equalValue, out T actualValue) =>
            set.TryGetValue(equalValue, out actualValue);

        IImmutableSet<T> IImmutableSet<T>.Intersect(IEnumerable<T> other) =>
            set.Intersect(other);

        IImmutableSet<T> IImmutableSet<T>.Except(IEnumerable<T> other) =>
            set.Except(other);

        IImmutableSet<T> IImmutableSet<T>.SymmetricExcept(IEnumerable<T> other) =>
            set.SymmetricExcept(other);

        /// <summary>
        /// Create a new set that contains all elements of both sets
        /// </summary>
        IImmutableSet<T> IImmutableSet<T>.Union(IEnumerable<T> other) =>
            set.Union(other);

        public bool SetEquals(IEnumerable<T> other) =>
            set.Equals(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) =>
            set.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) =>
            set.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) =>
            set.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) =>
            set.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) =>
            set.Overlaps(other);
    }
}

public static class __SetExt
{
    public static bool Compare<T>(this IImmutableSet<T> setA, IImmutableSet<T> setB) =>
        Set.compare(setA, setB);

    public static int Length<T>(this IImmutableSet<T> set) =>
        Set.length(set);

    public static Set<T> Difference<T>(this IImmutableSet<T> setA, IImmutableSet<T> setB) =>
        Set.difference(setA, setB);

    public static IEnumerable<T> Filter<T>(this IImmutableSet<T> set, Func<T, bool> pred) =>
        Set.filter(set, pred);

    public static Set<T> Intersect<T>(this IImmutableSet<T> setA, IImmutableSet<T> setB) =>
        Set.intersect(setA, setB);

    public static IEnumerable<R> Map<T, R>(this IImmutableSet<T> set, Func<T, R> mapper) =>
        Set.map(set, mapper);


    public static Option<T> find<T>(Set<T> set, T value) =>
        set.Find(value);

    public static bool exists<T>(IImmutableSet<T> set, Func<T, bool> pred) =>
        Set.exists(set, pred);
}