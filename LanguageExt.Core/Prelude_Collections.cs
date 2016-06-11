using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Construct a list from head and tail
        /// head becomes the first item in the list
        /// Is lazy
        /// </summary>
        [Pure]
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
        [Pure]
        public static Lst<T> Cons<T>(this T head, Lst<T> tail) =>
            tail.Insert(0, head);

        /// <summary>
        /// Lazily generate a range of integers.  
        /// </summary>
        [Pure]
        public static IntegerRange Range(int from, int count, int step = 1) =>
            new IntegerRange(from, count, step);

        /// <summary>
        /// Lazily generate a range of chars.  
        /// 
        ///   Remarks:
        ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
        /// </summary>
        [Pure]
        public static CharRange Range(char from, char to) =>
            new CharRange(from, to);

        /// <summary>
        /// Lazily generate integers from any number of provided ranges
        /// </summary>
        [Pure]
        public static IEnumerable<int> Range(params IntegerRange[] ranges) =>
            from range in ranges
            from i in range
            select i;

        /// <summary>
        /// Lazily generate chars from any number of provided ranges
        /// </summary>
        [Pure]
        public static IEnumerable<char> Range(params CharRange[] ranges) =>
            from range in ranges
            from c in range
            select c;

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>() =>
            LanguageExt.Map.empty<K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>(params Tuple<K, V>[] items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>(IEnumerable<Tuple<K, V>> items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>(params KeyValuePair<K, V>[] items) =>
            LanguageExt.Map.createRange(from x in items
                                        select Tuple(x.Key, x.Value));

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>(IEnumerable<KeyValuePair<K, V>> items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<T> List<T>() =>
            new Lst<T>();

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<T> List<T>(params T[] items) =>
            new Lst<T>(items);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<T> toList<T>(IEnumerable<T> items) =>
            items is Lst<T>
                ? (Lst<T>)items
                : new Lst<T>(items);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
        public static T[] Array<T>() =>
            new T[0];

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
        public static T[] Array<T>(T item) =>
            new T[1] { item };

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
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
        [Pure]
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
        [Pure]
        public static Que<T> Queue<T>() =>
            new Que<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
        public static Que<T> Queue<T>(params T[] items)
        {
            var q = new Que<T>();
            foreach (var item in items)
            {
                q = q.Enqueue(item);
            }
            return q;
        }

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
        public static Que<T> toQueue<T>(IEnumerable<T> items)
        {
            var q = new Que<T>();
            foreach (var item in items)
            {
                q = q.Enqueue(item);
            }
            return q;
        }

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Pure]
        public static Stck<T> Stack<T>() =>
            new Stck<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Pure]
        public static Stck<T> Stack<T>(params T[] items) =>
            new Stck<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Pure]
        public static Stck<T> toStack<T>(IEnumerable<T> items) =>
            new Stck<T>(items);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<T> Set<T>() =>
            LanguageExt.Set.create<T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<T> Set<T>(T item) =>
            LanguageExt.Set.create<T>().Add(item);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<T> Set<T>(params T[] items) =>
            LanguageExt.Set.createRange<T>(items);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<T> toSet<T>(IEnumerable<T> items) =>
            LanguageExt.Set.createRange<T>(items);

        /// <summary>
        /// Create a queryable
        /// </summary>
        [Pure]
        public static IQueryable<T> Query<T>(params T[] items) =>
            toQuery(items);

        /// <summary>
        /// Convert to queryable
        /// </summary>
        [Pure]
        public static IQueryable<T> toQuery<T>(IEnumerable<T> items) =>
            items.AsQueryable();

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, IEnumerable<T>, R> More) =>
            list.Match(Empty, More);

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, IEnumerable<T>, R> More ) =>
            list.Match(Empty, One, More);

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, More);

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, More);

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, Four, More);

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, T, R> Five,
            Func<T, T, T, T, T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, Four, Five, More);

        /// <summary>
        /// List matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, T, R> Two,
            Func<T, T, T, R> Three,
            Func<T, T, T, T, R> Four,
            Func<T, T, T, T, T, R> Five,
            Func<T, T, T, T, T, T, R> Six,
            Func<T, T, T, T, T, T, IEnumerable<T>, R> More) =>
            list.Match(Empty, One, Two, Three, Four, Five, Six, More);

        [Pure]
        public static R match<K, V, R>(Map<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            match(LanguageExt.Map.find(map, key),
                   Some,
                   None );

        public static Unit match<K, V>(Map<K, V> map, K key, Action<V> Some, Action None) =>
            match(LanguageExt.Map.find(map, key),
                   Some,
                   None);

        /// <summary>
        /// Convert value to [value] or [] if value == null
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(T value) =>
            isnull(value)
                ? new T[0]
                : new T[] { value };

        /// <summary>
        /// Convert a nullable to an enumerable
        /// HasValue : true = [x]
        /// HasValue : false = []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(T? value) where T : struct =>
            value.AsEnumerable();

        /// <summary>
        /// Convert an Enumerable to an Enumerable
        /// Deals with value == null by returning []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(IEnumerable<T> value) =>
            value == null
                ? new T[0]
                : value.AsEnumerable();

        /// <summary>
        /// Convert an option to an enumerable
        /// Some(x) = [x]
        /// None = []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Option<T> value) =>
            value.AsEnumerable();

        /// <summary>
        /// Convert an option to an enumerable
        /// Some(x) = [x]
        /// None = []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(OptionUnsafe<T> value) =>
            value.AsEnumerable();

        /// <summary>
        /// Convert an either to an enumerable
        /// Right(x) = [x]
        /// Left(y) = []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<L, T>(Either<L, T> value) =>
            value.RightAsEnumerable();

        /// <summary>
        /// Convert an either to an enumerable
        /// Right(x) = [x]
        /// Left(y) = []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<L, T>(EitherUnsafe<L, T> value) =>
            value.RightAsEnumerable();

        /// <summary>
        /// Convert a Try to an enumerable
        /// Succ(x) = [x]
        /// Fail(e) = []
        /// value is null : []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Try<T> value) =>
            value == null
                ? new T[0]
                : value.AsEnumerable().Where(x => x.IsRight).Map(x => x.RightValue);

        /// <summary>
        /// Convert a TryOption to an enumerable
        /// Succ(x) = [x]
        /// Fail(e) = []
        /// None = []
        /// value is null : []
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq2<T>(TryOption<T> value) =>
            value == null
                ? new T[0]
                : value.AsEnumerable().Where(x => x.IsRight).Map(x => x.RightValue);

        /// <summary>
        /// Convert a TryOption to an enumerable
        /// Succ(x) = [either(_,x)]
        /// Fail(e) = [either(exception,_)]
        /// None = []
        /// value is null : []
        /// </summary>
        [Pure]
        public static IEnumerable<Either<Exception, T>> seq<T>(TryOption<T> value) =>
            value == null
                ? new Either<Exception,T>[0]
                : value.AsEnumerable();

        /// <summary>
        /// Convert a tuple to an enumerable
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Tuple<T> tup) =>
            tup == null
                ? new T[0]
                : new[] { tup.Item1 };

        /// <summary>
        /// Convert a tuple to an enumerable
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Tuple<T, T> tup) =>
            tup == null
                ? new T[0]
                : new[] { tup.Item1, tup.Item2 };

        /// <summary>
        /// Convert a tuple to an enumerable
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Tuple<T, T, T> tup) =>
            tup == null
                ? new T[0]
                : new[] { tup.Item1, tup.Item2, tup.Item3 };

        /// <summary>
        /// Convert a tuple to an enumerable
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Tuple<T, T, T, T> tup) =>
            tup == null
                ? new T[0]
                : new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4 };

        /// <summary>
        /// Convert a tuple to an enumerable
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Tuple<T, T, T, T, T> tup) =>
            tup == null
                ? new T[0]
                : new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5 };

        /// <summary>
        /// Convert a tuple to an enumerable
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Tuple<T, T, T, T, T, T> tup) =>
            tup == null
                ? new T[0]
                : new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6 };

        /// <summary>
        /// Convert a tuple to an enumerable
        /// </summary>
        [Pure]
        public static IEnumerable<T> seq<T>(Tuple<T, T, T, T, T, T, T> tup) =>
            tup == null
                ? new T[0]
                : new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6, tup.Item7 };
    }
}
