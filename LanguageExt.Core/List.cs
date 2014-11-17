using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class List
    {
        public static T head<T>(this IEnumerable<T> list) => list.First();

        public static Option<T> headSafe<T>(this IEnumerable<T> list) =>
            list.Take(1).Count() == 1
                ? Some(list.First())
                : None;

        public static IEnumerable<T> tail<T>(this IEnumerable<T> list) => list.Skip(1);

        public static IEnumerable<R> map<T, R>(this IEnumerable<T> list, Func<T, R> map) =>
            list.Select(map);

        public static IEnumerable<T> filter<T>(this IEnumerable<T> list, Func<T, bool> predicate) =>
            list.Where(predicate);

        public static int sum(this IEnumerable<int> list) => fold(list, 0, (x, s) => s + x);

        public static float sum(this IEnumerable<float> list) => fold(list, 0.0f, (x, s) => s + x);

        public static double sum(this IEnumerable<double> list) => fold(list, 0.0, (x, s) => s + x);

        public static decimal sum(this IEnumerable<decimal> list) => fold(list, (decimal)0, (x, s) => s + x);

        public static S fold<S, T>(this IEnumerable<T> list, S state, Func<T, S, S> folder)
        {
            foreach (var item in list)
            {
                state = folder(item, state);
            }
            return state;
        }

        public static S foldr<S, T>(this IEnumerable<T> list, S state, Func<T, S, S> folder)
        {
            foreach (var item in list.Reverse())
            {
                state = folder(item, state);
            }
            return state;
        }

        public static IEnumerable<int> range(int from, int to)
        {
            for (var i = from; i <= to; i++)
            {
                yield return i;
            }
        }
    }
}