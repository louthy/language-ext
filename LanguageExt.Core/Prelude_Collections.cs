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
        /// </summary>
        public static IntegerRange range(int from, int count, int step = 1) =>
            new IntegerRange(from, count, step);

        /// <summary>
        /// Lazily generate a range of chars.  
        /// 
        ///   Remarks:
        ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
        /// </summary>
        public static CharRange range(char from, char to) =>
            new CharRange(from, to);

        /// <summary>
        /// Lazily generate integers from any number of provided ranges
        /// </summary>
        public static IEnumerable<int> range(params IntegerRange[] ranges) =>
            from range in ranges
            from i in range
            select i;

        /// <summary>
        /// Lazily generate chars from any number of provided ranges
        /// </summary>
        public static IEnumerable<char> range(params CharRange[] ranges) =>
            from range in ranges
            from c in range
            select c;

        /// <summary>
        /// Generates a sequence of T using the provided delegate to initialise
        /// each item.
        /// </summary>
        public static IEnumerable<T> init<T>(int count,Func<int,T> generator) =>
            from i in range(0, count)
            select generator(i);

        /// <summary>
        /// Generates an infinite sequence of T using the provided delegate to initialise
        /// each item.
        /// 
        ///   Remarks: Not truly infinite, will end at Int32.MaxValue
        /// 
        /// </summary>
        public static IEnumerable<T> initInfinite<T>(Func<int, T> generator) =>
            from i in range(0, Int32.MaxValue)
            select generator(i);

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        public static IEnumerable<T> repeat<T>(T item, int count) =>
            from _ in range(0, count)
            select item;

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
        public static IImmutableList<T> toList<T>(IEnumerable<T> items) =>
            ImmutableList.CreateRange(items);
        
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
        /// Create an immutable array
        /// </summary>
        public static ImmutableArray<T> toArray<T>(IEnumerable<T> items) =>
            ImmutableArray.CreateRange(items);

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
        /// Create an immutable queue
        /// </summary>
        public static IImmutableQueue<T> toQueue<T>(IEnumerable<T> items) =>
            ImmutableQueue.CreateRange<T>(items);

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
        /// Create an immutable stack
        /// </summary>
        public static IImmutableStack<T> toStack<T>(IEnumerable<T> items) =>
            ImmutableStack.CreateRange<T>(items);

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

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static IImmutableSet<T> toSet<T>(IEnumerable<T> items) =>
            ImmutableHashSet.CreateRange<T>(items);

        /// <summary>
        /// Create a queryable
        /// </summary>
        public static IQueryable<T> query<T>(params T[] items) =>
            toQuery(items);

        /// <summary>
        /// Convert to queryable
        /// </summary>
        public static IQueryable<T> toQuery<T>(IEnumerable<T> items) =>
            items.AsQueryable();

        /// <summary>
        /// List matching
        /// </summary>
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, IEnumerable<T>, R> More) =>
            list.Match(Empty, More);

        /// <summary>
        /// List matching
        /// </summary>
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, IEnumerable<T>, R> More ) =>
            list.Match(Empty, One, More);

        /// <summary>
        /// List matching
        /// </summary>
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, More);

        /// <summary>
        /// List matching
        /// </summary>
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, More);

        /// <summary>
        /// List matching
        /// </summary>
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, Four, More);

        /// <summary>
        /// List matching
        /// </summary>
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, T, R> Five,
            Func<T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, Four, Five, More);

        /// <summary>
        /// List matching
        /// </summary>
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, T, R> Five,
            Func<T, T, T, T, T, T, R> Six,
            Func<T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, Four, Five, Six, More);

        public static R match<K, V, R>(IImmutableDictionary<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            match( Map.find(map, key),
                   Some,
                   None );

        public static Unit match<K, V>(IImmutableDictionary<K, V> map, K key, Action<V> Some, Action None) =>
            match(Map.find(map, key),
                   Some,
                   None);

    }
}
