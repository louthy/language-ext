using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// Construct a list from head and tail
        /// head becomes the first item in the list
        /// Is lazy
        /// </summary>
        public static IEnumerable<T> cons<T>(this T head, IEnumerable<T> tail)
        {
            yield return head;
            foreach (var item in tail)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Construct a list from head and tail
        /// </summary>
        public static IImmutableList<T> cons<T>(this T head, IImmutableList<T> tail) =>
            tail.Insert(0, head);

        /// <summary>
        /// Create an empty IEnumerable<T>
        /// </summary>
        public static IEnumerable<T> empty<T>() =>
            new T[0];

        /// <summary>
        /// Lazily generate a range of integers.  
        /// from and to are inclusive.
        /// </summary>
        public static IEnumerable<int> range(int from, int to) =>
            Enumerable.Range(from, to);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static IImmutableDictionary<K, V> map<K, V>() =>
            ImmutableDictionary.Create<K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static IImmutableDictionary<K, V> map<K, V>(params Tuple<K, V>[] items) =>
            map(items.Select(i => new KeyValuePair<K, V>(i.Item1, i.Item2)).ToArray());

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static IImmutableDictionary<K, V> map<K, V>(params KeyValuePair<K, V>[] items)
        {
            var builder = ImmutableDictionary.CreateBuilder<K, V>();
            builder.AddRange(items);
            return builder.ToImmutableDictionary();
        }

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static IImmutableList<T> list<T>() =>
            ImmutableList.Create<T>();

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static IImmutableList<T> list<T>(params T[] items) =>
            ImmutableList.Create<T>(items);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static IImmutableList<T> list<T>(Option<T> option) =>
            List.freeze(option.ToList());

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static IImmutableList<T> list<T>(OptionUnsafe<T> option) =>
            List.freeze(option.ToList());

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static ImmutableArray<T> array<T>() =>
            ImmutableArray.Create<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static ImmutableArray<T> array<T>(T item) =>
            ImmutableArray.Create<T>(item);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static ImmutableArray<T> array<T>(params T[] items) =>
            ImmutableArray.Create<T>(items);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static IImmutableQueue<T> queue<T>() =>
            ImmutableQueue.Create<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static IImmutableQueue<T> queue<T>(T item) =>
            ImmutableQueue.Create<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static IImmutableQueue<T> queue<T>(params T[] items) =>
            ImmutableQueue.Create<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static IImmutableStack<T> stack<T>() =>
            ImmutableStack.Create<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static IImmutableStack<T> stack<T>(T item) =>
            ImmutableStack.Create<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static IImmutableStack<T> stack<T>(params T[] items) =>
            ImmutableStack.Create<T>(items);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static IImmutableSet<T> set<T>() =>
            ImmutableHashSet.Create<T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static IImmutableSet<T> set<T>(T item) =>
            ImmutableHashSet.Create<T>(item);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static IImmutableSet<T> set<T>(params T[] items) =>
            ImmutableHashSet.Create<T>(items);

    }
}
