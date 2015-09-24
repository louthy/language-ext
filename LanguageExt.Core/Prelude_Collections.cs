using System;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Construct a list from head and tail
        /// head becomes the first item in the list
        /// Is lazy
        /// </summary>
        public static IEnumerable<T> Cons<T>(this T head, IEnumerable<T> tail)
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
        public static Lst<T> Cons<T>(this T head, Lst<T> tail) =>
            tail.Insert(0, head);

        /// <summary>
        /// Lazily generate a range of integers.  
        /// </summary>
        public static IntegerRange Range(int from, int count, int step = 1) =>
            new IntegerRange(from, count, step);

        /// <summary>
        /// Lazily generate a range of chars.  
        /// 
        ///   Remarks:
        ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
        /// </summary>
        public static CharRange Range(char from, char to) =>
            new CharRange(from, to);

        /// <summary>
        /// Lazily generate integers from any number of provided ranges
        /// </summary>
        public static IEnumerable<int> Range(params IntegerRange[] ranges) =>
            from range in ranges
            from i in range
            select i;

        /// <summary>
        /// Lazily generate chars from any number of provided ranges
        /// </summary>
        public static IEnumerable<char> Range(params CharRange[] ranges) =>
            from range in ranges
            from c in range
            select c;

        /// <summary>
        /// Generates a sequence of T using the provided delegate to initialise
        /// each item.
        /// </summary>
        public static IEnumerable<T> init<T>(int count,Func<int,T> generator) =>
            from i in Range(0, count)
            select generator(i);

        /// <summary>
        /// Generates an infinite sequence of T using the provided delegate to initialise
        /// each item.
        /// 
        ///   Remarks: Not truly infinite, will end at Int32.MaxValue
        /// 
        /// </summary>
        public static IEnumerable<T> initInfinite<T>(Func<int, T> generator) =>
            from i in Range(0, Int32.MaxValue)
            select generator(i);

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>
        public static IEnumerable<T> repeat<T>(T item, int count) =>
            from _ in Range(0, count)
            select item;

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static Map<K, V> Map<K, V>() =>
            LanguageExt.Map.empty<K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static Map<K, V> Map<K, V>(params Tuple<K, V>[] items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static Map<K, V> Map<K, V>(params KeyValuePair<K, V>[] items) =>
            LanguageExt.Map.createRange(from x in items
                            select Tuple(x.Key,x.Value));

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static Lst<T> List<T>() =>
            new Lst<T>();

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static Lst<T> List<T>(params T[] items) =>
            new Lst<T>(items);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static Lst<T> toList<T>(IEnumerable<T> items) =>
            new Lst<T>(items);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static T[] Array<T>() =>
            new T[0];

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static T[] Array<T>(T item) =>
            new T[1] { item };

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static T[] Array<T>(params T[] items)
        {
            var a = new T[items.Length];
            int i = 0;
            foreach (var item in items)
            {
                a[i] = item;
                i++;
            }
            return a;
        }

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static T[] toArray<T>(IEnumerable<T> items)
        {
            var a = new T[items.Count()];
            int i = 0;
            foreach (var item in items)
            {
                a[i] = item;
                i++;
            }
            return a;
        }

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static Que<T> Queue<T>() =>
            new Que<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static Que<T> Queue<T>(params T[] items) =>
            new Que<T>(items);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static Que<T> toQueue<T>(IEnumerable<T> items) =>
            new Que<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static Stck<T> Stack<T>() =>
            new Stck<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static Stck<T> Stack<T>(params T[] items) =>
            new Stck<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static Stck<T> toStack<T>(IEnumerable<T> items) =>
            new Stck<T>(items);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static Set<T> Set<T>() =>
            LanguageExt.Set.create<T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static Set<T> Set<T>(T item) =>
            LanguageExt.Set.create<T>().Add(item);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static Set<T> Set<T>(params T[] items) =>
            LanguageExt.Set.createRange<T>(items);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static Set<T> toSet<T>(IEnumerable<T> items) =>
            LanguageExt.Set.createRange<T>(items);

        /// <summary>
        /// Create a queryable
        /// </summary>
        public static IQueryable<T> Query<T>(params T[] items) =>
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

        public static R match<K, V, R>(Map<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            match(LanguageExt.Map.find(map, key),
                   Some,
                   None );

        public static Unit match<K, V>(Map<K, V> map, K key, Action<V> Some, Action None) =>
            match(LanguageExt.Map.find(map, key),
                   Some,
                   None);
    }
}
