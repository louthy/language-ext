using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LSeq = LanguageExt.Seq;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, R>(Tuple<T1, T2> self, Func<T1, T2, R> func) =>
            func(self.Item1, self.Item2);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, R>(Tuple<T1, T2, T3> self, Func<T1, T2, T3, R> func) =>
            func(self.Item1, self.Item2, self.Item3);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, R>(Tuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, T5, R>(Tuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, T5, T6, R>(Tuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, T5, T6, T7, R>(Tuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2>(Tuple<T1, T2> self, Action<T1, T2> func)
        {
            func(self.Item1, self.Item2);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3>(Tuple<T1, T2, T3> self, Action<T1, T2, T3> func)
        {
            func(self.Item1, self.Item2, self.Item3);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4>(Tuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4, T5>(Tuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4, T5, T6>(Tuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4, T5, T6, T7>(Tuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
            return Unit.Default;
        }

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2> tuple<T1, T2>(T1 item1, T2 item2) =>
            System.Tuple.Create(item1, item2);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3> tuple<T1, T2, T3>(T1 item1, T2 item2, T3 item3) =>
            System.Tuple.Create(item1, item2, item3);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4> tuple<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) =>
            System.Tuple.Create(item1, item2, item3, item4);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4, T5> tuple<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) =>
            System.Tuple.Create(item1, item2, item3, item4, item5);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4, T5, T6> tuple<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) =>
            System.Tuple.Create(item1, item2, item3, item4, item5, item6);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7> tuple<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) =>
            System.Tuple.Create(item1, item2, item3, item4, item5, item6, item7);

        /// <summary>
        /// Create a queryable
        /// </summary>
        [Obsolete("Use 'Query'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IQueryable<T> query<T>(params T[] items) =>
            toQuery(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Obsolete("Use 'Map'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> map<K, V>() =>
            LanguageExt.Map.empty<K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Obsolete("Use 'Map'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> map<K, V>(params Tuple<K, V>[] items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Obsolete("Use 'Map'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> map<K, V>(params KeyValuePair<K, V>[] items) =>
            LanguageExt.Map.createRange(from x in items
                                        select Tuple(x.Key, x.Value));

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Obsolete("Use 'List'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> list<T>() =>
            Lst<T>.Empty;

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Obsolete("Use 'List'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> list<T>(params T[] items) =>
            new Lst<T>(items);

        /// <summary>
        /// Create an immutable array
        /// </summary>
        [Obsolete("Use 'Array'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T[] array<T>() =>
            new T[0];

        /// <summary>
        /// Create an immutable array
        /// </summary>
        [Obsolete("Use 'Array'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T[] array<T>(T item) =>
            new T[1] {item};

        /// <summary>
        /// Create an immutable array
        /// </summary>
        [Obsolete("Use 'Array'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T[] array<T>(params T[] items)
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
        /// Create an immutable set
        /// </summary>
        [Obsolete("Use 'Set'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Set<T> set<T>() =>
            new Set<T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Obsolete("Use 'Set'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Set<T> set<T>(T item) =>
            new Set<T>().Add(item);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Obsolete("Use 'Set'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Set<T> set<T>(params T[] items) =>
            new Set<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Obsolete("Use 'Stack'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Stck<T> stack<T>() =>
            new Stck<T>();

        /// <summary>
        /// Create an empty IEnumerable T
        /// </summary>
        [Obsolete("Use List.empty")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> empty<T>() =>
            Lst<T>.Empty;

        /// <summary>
        /// Construct a list from head and tail
        /// head becomes the first item in the list
        /// Is lazy
        /// </summary>
        [Obsolete("Use 'Cons'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> cons<T>(T head, IEnumerable<T> tail)
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
        [Obsolete("Use 'Cons'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> cons<T>(T head, Lst<T> tail) =>
            tail.Insert(0, head);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, R>(T1 value1, T2 value2, Func<T1, T2, R> project) =>
            project(value1, value2);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, R>(T1 value1, T2 value2, T3 value3, Func<T1, T2, T3, R> project) =>
            project(value1, value2, value3);

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Obsolete("toList has been deprecated.  Please use rightToList.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<R> toList<L, R>(Either<L, R> either) =>
            either.ToList();

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Obsolete("ToArray has been deprecated.  Please use RightToArray.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Arr<R> toArray<L, R>(Either<L, R> either) =>
            either.ToArray();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static R With<T1, T2, R>(this Tuple<T1, T2> self, Func<T1, T2, R> func)
        {
            return func(self.Item1, self.Item2);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static R With<T1, T2, T3, R>(this Tuple<T1, T2, T3> self, Func<T1, T2, T3, R> func)
        {
            return func(self.Item1, self.Item2, self.Item3);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static R With<T1, T2, T3, T4, R>(this Tuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, R> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static R With<T1, T2, T3, T4, T5, R>(this Tuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, R> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static R With<T1, T2, T3, T4, T5, T6, R>(this Tuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, R> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static R With<T1, T2, T3, T4, T5, T6, T7, R>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, R> func)
        {
            return func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static Unit With<T1, T2>(this Tuple<T1, T2> self, Action<T1, T2> func)
        {
            func(self.Item1, self.Item2);
            return Unit.Default;
        }

        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit With<T1, T2, T3>(this Tuple<T1, T2, T3> self, Action<T1, T2, T3> func)
        {
            func(self.Item1, self.Item2, self.Item3);
            return Unit.Default;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static Unit With<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4);
            return Unit.Default;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static Unit With<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
            return Unit.Default;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static Unit With<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
            return Unit.Default;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("'With' has been renamed to 'Map', please use that instead")]
        public static Unit With<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
            return Unit.Default;
        }
        
        [Pure]
        [Obsolete("Use TaskFail")]
        public static Task<A> asFailedTask<A>(Exception ex)
        {
            var tcs = new TaskCompletionSource<A>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        /// <summary>
        /// Convert a value to a Task that completes immediately
        /// </summary>
        [Pure]
        [Obsolete("Use TaskSucc")]
        public static Task<A> asTask<A>(A self) =>
            Task.FromResult(self);

        /// <summary>
        /// Project the Either into an IQueryable of R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an IQueryable of R with one item.  A zero length IQueryable R otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("ToQuery has been deprecated.  Please use RightToQuery.")]
        public static IQueryable<R> toQuery<L, R>(Either<L, R> either) =>
            either.RightAsEnumerable().AsQueryable();    

        /// <summary>
        /// Construct a sequence from any value
        ///     T     : [x]
        ///     null  : []
        /// </summary>
        [Pure]
        [Obsolete("SeqOne has been deprecated for the more concise Seq1")]
        public static Seq<A> SeqOne<A>(A value) =>
            value.IsNull()
                ? Empty
                : LSeq.FromSingleValue(value);
        
    }
}
