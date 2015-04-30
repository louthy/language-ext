using System;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Query
    {
        public static T head<T>(IQueryable<T> list) => list.First();

        [Obsolete("headSafe has been deprecated, please use headOrNone")]
        public static Option<T> headSafe<T>(IQueryable<T> list) =>
            (from x in list
             select Some(x))
            .DefaultIfEmpty(None)
            .FirstOrDefault();

        public static Option<T> headOrNone<T>(IQueryable<T> list) =>
            (from x in list
             select Some(x))
            .DefaultIfEmpty(None)
            .FirstOrDefault();

        public static IQueryable<T> tail<T>(IQueryable<T> list) =>
            list.Skip(1);

        public static IQueryable<R> map<T, R>(IQueryable<T> list, Expression<Func<T, R>> map) =>
            list.Select(map);

        public static IQueryable<R> map<T, R>(IQueryable<T> list, Expression<Func<int, T, R>> map)
        {
            var paramT = Expression.Parameter(typeof(T), "t");
            var paramI = Expression.Parameter(typeof(int), "i");

            return zip(list, range(0, Int32.MaxValue).AsQueryable(),
                Expression.Lambda<Func<T, int, R>>(
                    Expression.Invoke(map, paramI, paramT),
                    paramT,
                    paramI
                    ));
        }

        public static IQueryable<T> filter<T>(IQueryable<T> list, Expression<Func<T, bool>> predicate) =>
            list.Where(predicate);

        public static IQueryable<T> choose<T>(IQueryable<T> list, Expression<Func<T, Option<T>>> selector) =>
            map(filter(map(list, selector), t => t.IsSome), t => t.Value);

        public static IQueryable<T> choose<T>(IQueryable<T> list, Expression<Func<int, T, Option<T>>> selector) =>
            map(filter(map(list, selector), t => t.IsSome), t => t.Value);

        public static IQueryable<R> collect<T, R>(IQueryable<T> list, Expression<Func<T, IEnumerable<R>>> map)
        {
            var paramT = Expression.Parameter(typeof(T), "t");

            return list.SelectMany(
                Expression.Lambda<Func<T, IEnumerable<R>>>(
                    Expression.Invoke(map, paramT),
                    paramT
                    ));
        }

        public static int sum(IQueryable<int> list) => 
            fold(list, 0, (x, s) => s + x);

        public static float sum(IQueryable<float> list) => 
            fold(list, 0.0f, (x, s) => s + x);

        public static double sum(IQueryable<double> list) => 
            fold(list, 0.0, (x, s) => s + x);

        public static decimal sum(IQueryable<decimal> list) => 
            fold(list, (decimal)0, (x, s) => s + x);

        public static IQueryable<T> rev<T>(IQueryable<T> list) =>
            list.Reverse();

        public static IQueryable<T> append<T>(IQueryable<T> lhs, IQueryable<T> rhs) =>
            lhs.Concat(rhs);

        public static S fold<S, T>(IQueryable<T> list, S state, Expression<Func<T, S, S>> folder)
        {
            var paramS = Expression.Parameter(typeof(S), "s");
            var paramT = Expression.Parameter(typeof(T), "t");

            return list.Aggregate(state,
                Expression.Lambda<Func<S, T, S>>(
                    Expression.Invoke(folder, paramS, paramT),
                    paramS,
                    paramT));
        }

        public static S foldBack<S, T>(IQueryable<T> list, S state, Expression<Func<T, S, S>> folder) =>
            fold(rev(list), state, folder);

        public static T reduce<T>(IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
            match(headOrNone(list),
                Some: x  => fold(tail(list), x, reducer),
                None: () => failwith<T>("Input list was empty")
            );

        public static T reduceBack<T>(IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
            reduce(rev(list), reducer);

        public static Option<T> find<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
            headOrNone(filter(list, pred));

        public static IImmutableList<T> freeze<T>(IQueryable<T> list) =>
            toList(list);

        public static IQueryable<V> zip<T, U, V>(IQueryable<T> list, IQueryable<U> other, Expression<Func<T, U, V>> zipper) =>
            list.Zip(other, zipper);

        public static int length<T>(IQueryable<T> list) =>
            list.Count();

        public static Unit iter<T>(IQueryable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return unit;
        }

        public static Unit iter<T>(IQueryable<T> list, Action<int, T> action)
        {
            int i = 0;
            foreach (var item in list)
            {
                action(i++, item);
            }
            return unit;
        }

        public static bool forall<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
            list.All(pred);

        public static IQueryable<T> distinct<T>(IQueryable<T> list) =>
            list.Distinct();

        public static IQueryable<T> take<T>(IQueryable<T> list, int count) =>
            list.Take(count);

        public static IQueryable<T> takeWhile<T>(IQueryable<T> list, Expression<Func<T,bool>> pred) =>
            list.TakeWhile(pred);

        public static IQueryable<T> takeWhile<T>(IQueryable<T> list, Expression<Func<T, int, bool>> pred) =>
            list.TakeWhile(pred);

        public static bool exists<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
            list.Any(pred);
    }
}

public static class __QueryExt
{
    public static T Head<T>(this IQueryable<T> list) =>
        Query.head(list);

    [Obsolete("HeadSafe has been deprecated, please use HeadOrNone")]
    public static Option<T> HeadSafe<T>(this IQueryable<T> list) =>
        Query.headSafe(list);

    public static Option<T> HeadOrNone<T>(this IQueryable<T> list) =>
        Query.headOrNone(list);

    public static IQueryable<T> Tail<T>(this IQueryable<T> list) =>
        Query.tail(list);

    public static IQueryable<R> Map<T, R>(this IQueryable<T> list, Expression<Func<T, R>> map) =>
        Query.map(list, map);

    public static IQueryable<R> Map<T, R>(this IQueryable<T> list, Expression<Func<int, T, R>> map) =>
        Query.map(list, map);

    public static IQueryable<T> Filter<T>(this IQueryable<T> list, Expression<Func<T, bool>> predicate) =>
        Query.filter(list, predicate);

    public static IQueryable<T> Choose<T>(this IQueryable<T> list, Expression<Func<T, Option<T>>> selector) =>
        Query.choose(list, selector);

    public static IQueryable<T> Choose<T>(this IQueryable<T> list, Expression<Func<int, T, Option<T>>> selector) =>
        Query.choose(list, selector);

    public static IQueryable<R> Collect<T, R>(this IQueryable<T> list, Expression<Func<T, IEnumerable<R>>> map) =>
        Query.collect(list, map);

    public static IQueryable<T> Rev<T>(this IQueryable<T> list) =>
        Query.rev(list);

    public static IQueryable<T> Append<T>(this IQueryable<T> lhs, IQueryable<T> rhs) =>
        Query.append(lhs, rhs);

    public static S Fold<S, T>(this IQueryable<T> list, S state, Expression<Func<T, S, S>> folder) =>
        Query.fold(list, state, folder);

    public static S FoldBack<S, T>(this IQueryable<T> list, S state, Expression<Func<T, S, S>> folder) =>
        Query.foldBack(list, state, folder);

    public static T Reduce<T>(this IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
        Query.reduce(list, reducer);

    public static T ReduceBack<T>(this IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
        Query.reduceBack(list, reducer);

    public static IImmutableList<T> Freeze<T>(this IQueryable<T> list) =>
        Query.freeze(list);

    public static IQueryable<V> Zip<T, U, V>(this IQueryable<T> list, IQueryable<U> other, Expression<Func<T, U, V>> zipper) =>
        Query.zip(list, other, zipper);

    public static int Length<T>(this IQueryable<T> list) =>
        Query.length(list);

    public static Unit Iter<T>(this IQueryable<T> list, Action<T> action) =>
        Query.iter(list, action);

    public static Unit Iter<T>(this IQueryable<T> list, Action<int, T> action) =>
        Query.iter(list, action);

    public static bool ForAll<T>(this IQueryable<T> list, Expression<Func<T, bool>> pred) =>
        Query.forall(list, pred);

    public static IQueryable<T> Distinct<T>(IQueryable<T> list) =>
        Query.distinct(list);

    public static bool Exists<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
        Query.exists(list, pred);
}
