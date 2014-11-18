using System;
using System.Collections.Immutable;
using System.Linq;
using LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class List
    {
        public static IImmutableList<T> add<T>(this IImmutableList<T> self, T value) =>
            self.Add(value);

        public static IImmutableList<T> remove<T>(this IImmutableList<T> self, T value) =>
            self.Remove(value);

        public static IImmutableList<T> removeAt<T>(this IImmutableList<T> self, int index) =>
            self.RemoveAt(index);

        public static T head<T>(this IImmutableList<T> list) => list.First();

        public static Option<T> headSafe<T>(this IImmutableList<T> list) =>
            list.Take(1).Count() == 1
                ? Some(list.First())
                : None;

        public static IImmutableList<T> tail<T>(this IImmutableList<T> list) => 
            list.Skip(1).ToImmutableList();

        public static IImmutableList<R> map<T, R>(this IImmutableList<T> list, Func<T, R> map) =>
            list.Select(map).ToImmutableList();

        public static IImmutableList<T> filter<T>(this IImmutableList<T> list, Func<T, bool> predicate) =>
            list.Where(predicate).ToImmutableList();

        public static int sum(this IImmutableList<int> list) => fold(list, 0, (x, s) => s + x);

        public static float sum(this IImmutableList<float> list) => fold(list, 0.0f, (x, s) => s + x);

        public static double sum(this IImmutableList<double> list) => fold(list, 0.0, (x, s) => s + x);

        public static decimal sum(this IImmutableList<decimal> list) => fold(list, (decimal)0, (x, s) => s + x);

        public static IImmutableList<T> rev<T>(this IImmutableList<T> list) =>
            list.Reverse().ToImmutableList();

        public static IImmutableList<T> append<T>(this IImmutableList<T> lhs, IImmutableList<T> rhs) =>
            lhs.Concat(rhs).ToImmutableList();

        public static S fold<S, T>(this IImmutableList<T> list, S state, Func<T, S, S> folder)
        {
            each(list, item => { state = folder(item, state); } );
            return state;
        }

        public static S foldr<S, T>(this IImmutableList<T> list, S state, Func<T, S, S> folder) =>
            fold(rev(list), state, folder);

        public static T reduce<T>(this IImmutableList<T> list, Func<T, T, T> reducer) =>
            match(headSafe(list),
                Some: x => fold(tail(list), x, reducer),
                None: () => failwith<T>("Input list was empty")
            );

        public static Unit each<T>(this IImmutableList<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return unit;
        }

        public static Unit each<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return unit;
        }

    }
}