using System;
using System.Collections.Immutable;
using System.Linq;
using LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class List
    {
        public static IImmutableList<T> add<T>(this IImmutableList<T> list, T value) =>
            list.Add(value);

        public static IImmutableList<T> addRange<T>(this IImmutableList<T> list, IEnumerable<T> value) =>
            list.AddRange(value);

        public static IImmutableList<T> remove<T>(this IImmutableList<T> list, T value) =>
            list.Remove(value);

        public static IImmutableList<T> removeAt<T>(this IImmutableList<T> list, int index) =>
            list.RemoveAt(index);

        public static T head<T>(this IEnumerable<T> list) => list.First();

        public static Option<T> headSafe<T>(this IEnumerable<T> list) =>
            list.Take(1).Count() == 1
                ? Some(list.First())
                : None;

        public static IEnumerable<T> tail<T>(this IEnumerable<T> list) =>
            list.Skip(1);

        public static IEnumerable<R> map<T, R>(this IEnumerable<T> list, Func<T, R> map) =>
            list.Select(map);

        public static IEnumerable<R> mapi<T, R>(this IEnumerable<T> list, Func<int, T, R> map) =>
            zip(list, range(0, Int32.MaxValue), (t, i) => map(i, t));

        public static IEnumerable<T> filter<T>(this IEnumerable<T> list, Func<T, bool> predicate) =>
            list.Where(predicate);

        public static int sum(this IEnumerable<int> list) => fold(list, 0, (x, s) => s + x);

        public static float sum(this IEnumerable<float> list) => fold(list, 0.0f, (x, s) => s + x);

        public static double sum(this IEnumerable<double> list) => fold(list, 0.0, (x, s) => s + x);

        public static decimal sum(this IEnumerable<decimal> list) => fold(list, (decimal)0, (x, s) => s + x);

        public static IEnumerable<T> rev<T>(this IEnumerable<T> list) =>
            list.Reverse();

        public static IEnumerable<T> append<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            lhs.Concat(rhs);

        public static S fold<S, T>(this IEnumerable<T> list, S state, Func<T, S, S> folder)
        {
            iter(list, item => { state = folder(item, state); } );
            return state;
        }

        public static S foldr<S, T>(this IEnumerable<T> list, S state, Func<T, S, S> folder) =>
            fold(rev(list), state, folder);

        public static T reduce<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
            match(headSafe(list),
                Some: x => fold(tail(list), x, reducer),
                None: () => failwith<T>("Input list was empty")
            );

        public static IImmutableList<T> freeze<T>(this IEnumerable<T> self) =>
            self.ToImmutableList();

        public static IEnumerable<V> zip<T, U, V>(this IEnumerable<T> self, IEnumerable<U> other, Func<T, U, V> zipper) =>
            freeze(self.Zip(other, zipper));

        public static int length<T>(this IEnumerable<T> self) =>
           self.Count();

        public static Unit iter<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return unit;
        }

        public static Unit iteri<T>(this IEnumerable<T> list, Action<int, T> action)
        {
            int i = 0;
            foreach (var item in list)
            {
                action(i++, item);
            }
            return unit;
        }

        public static bool forall<T>(this IEnumerable<T> list, Func<T, bool> pred) =>
            list.All(pred);

        /// <summary>
        /// List matching
        /// </summary>
        public static R Match<T, R>(this IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, IEnumerable<T>, R> More
            )
        {
            if (list == null)
            {
                return Empty();
            }
            else
            {
                var head = list.Take(1).ToList();
                var tail = list.Skip(1);

                return head.Count == 0
                    ? Empty()
                    : tail.Take(1).Count() == 0
                        ? One(head.First())
                        : More(head.First(), tail);
            }
        }
    }
}