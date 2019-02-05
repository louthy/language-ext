using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class Query
    {
        public static T head<T>(IQueryable<T> list) => list.First();

        public static Option<T> headOrNone<T>(IQueryable<T> list) =>
            list.Take(1).AsEnumerable().HeadOrNone();

        public static Either<L, R> headOrLeft<L, R>(IQueryable<R> list, L left) =>
            list.Take(1).AsEnumerable().HeadOrLeft(left);

        public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IQueryable<Success> list, Fail fail) =>
            list.Take(1).AsEnumerable().HeadOrInvalid(fail);

        public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IQueryable<Success> list, Seq<Fail> fail) =>
            list.Take(1).AsEnumerable().HeadOrInvalid(fail);

        public static Validation<MonoidFail, Fail, Success> headOrInvalid<MonoidFail, Fail, Success>(IQueryable<Success> list, Fail fail)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
                list.Take(1).AsEnumerable().HeadOrInvalid<MonoidFail, Fail, Success>(fail);

        public static IQueryable<T> tail<T>(IQueryable<T> list) =>
            Queryable.Skip(list, 1);

        public static IQueryable<R> map<T, R>(IQueryable<T> list, Expression<Func<T, R>> map) =>
            Queryable.Select(list, map);

        public static IQueryable<R> map<T, R>(IQueryable<T> list, Expression<Func<int, T, R>> map)
        {
            var paramT = Expression.Parameter(typeof(T), "t");
            var paramI = Expression.Parameter(typeof(int), "i");

            return zip(list, Range(0, Int32.MaxValue),
                Expression.Lambda<Func<T, int, R>>(
                    Expression.Invoke(map, paramI, paramT),
                    paramT,
                    paramI
                    ));
        }

        public static IQueryable<T> filter<T>(IQueryable<T> list, Expression<Func<T, bool>> predicate) =>
            list.Where(predicate);

        public static IQueryable<U> choose<T, U>(IQueryable<T> list, Expression<Func<T, Option<U>>> selector) =>
            map(filter(map(list, selector), t => t.IsSome), t => t.Value);

        public static IQueryable<U> choose<T, U>(IQueryable<T> list, Expression<Func<int, T, Option<U>>> selector) =>
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
            Queryable.Reverse(list);

        public static IQueryable<T> append<T>(IQueryable<T> lhs, IQueryable<T> rhs) =>
            Queryable.Concat(lhs, rhs);

        public static S fold<S, T>(IQueryable<T> list, S state, Expression<Func<S, T, S>> folder)
        {
            return Queryable.Aggregate(list, state, folder);
        }

        public static S foldBack<S, T>(IQueryable<T> list, S state, Expression<Func<S, T, S>> folder) =>
            fold(rev(list), state, folder);

        public static T reduce<T>(IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
            match(headOrNone(list),
                Some: x => fold(tail(list), x, reducer),
                None: () => failwith<T>("Input list was empty")
            );

        public static T reduceBack<T>(IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
            reduce(rev(list), reducer);

        public static Option<T> find<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
            headOrNone(filter(list, pred));

        public static Lst<T> freeze<T>(IQueryable<T> list) =>
            toList(list);

        public static IQueryable<V> zip<T, U, V>(IQueryable<T> list, IEnumerable<U> other, Expression<Func<T, U, V>> zipper) =>
            Queryable.Zip(list, other, zipper);

        public static int length<T>(IQueryable<T> list) =>
            Queryable.Count(list);

        public static bool forall<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
            Queryable.All(list, pred);

        public static IQueryable<T> distinct<T>(IQueryable<T> list) =>
            Queryable.Distinct(list);

        public static IQueryable<T> take<T>(IQueryable<T> list, int count) =>
            Queryable.Take(list, count);

        public static IQueryable<T> takeWhile<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
            Queryable.TakeWhile(list, pred);

        public static IQueryable<T> takeWhile<T>(IQueryable<T> list, Expression<Func<T, int, bool>> pred) =>
            Queryable.TakeWhile(list, pred);

        public static bool exists<T>(IQueryable<T> list, Expression<Func<T, bool>> pred) =>
            Queryable.Any(list, pred);
    }
}

public static class QueryExtensions
{
    public static T Head<T>(this IQueryable<T> list) =>
        LanguageExt.Query.head(list);

    public static Option<T> HeadOrNone<T>(this IQueryable<T> list) =>
        LanguageExt.Query.headOrNone(list);

    public static Validation<S, T> HeadOrInvalid<S, T>(this IQueryable<T> list, S fail) =>
        LanguageExt.Query.headOrInvalid(list, fail);

    public static Either<S, T> HeadOrLeft<S, T>(this IQueryable<T> list, S left) =>
        LanguageExt.Query.headOrLeft(list, left);

    public static IQueryable<T> Tail<T>(this IQueryable<T> list) =>
        LanguageExt.Query.tail(list);

    public static IQueryable<R> Map<T, R>(this IQueryable<T> list, Expression<Func<T, R>> map) =>
        LanguageExt.Query.map(list, map);

    public static IQueryable<R> Map<T, R>(this IQueryable<T> list, Expression<Func<int, T, R>> map) =>
        LanguageExt.Query.map(list, map);

    public static IQueryable<T> Filter<T>(this IQueryable<T> list, Expression<Func<T, bool>> predicate) =>
        LanguageExt.Query.filter(list, predicate);

    public static IQueryable<U> Choose<T, U>(this IQueryable<T> list, Expression<Func<T, Option<U>>> selector) =>
        LanguageExt.Query.choose(list, selector);

    public static IQueryable<U> Choose<T, U>(this IQueryable<T> list, Expression<Func<int, T, Option<U>>> selector) =>
        LanguageExt.Query.choose(list, selector);

    public static IQueryable<R> Collect<T, R>(this IQueryable<T> list, Expression<Func<T, IEnumerable<R>>> map) =>
        LanguageExt.Query.collect(list, map);

    public static IQueryable<T> Rev<T>(this IQueryable<T> list) =>
        LanguageExt.Query.rev(list);

    public static IQueryable<T> Append<T>(this IQueryable<T> lhs, IQueryable<T> rhs) =>
        LanguageExt.Query.append(lhs, rhs);

    public static S Fold<S, T>(this IQueryable<T> list, S state, Expression<Func<S, T, S>> folder) =>
        LanguageExt.Query.fold(list, state, folder);

    public static S FoldBack<S, T>(this IQueryable<T> list, S state, Expression<Func<S, T, S>> folder) =>
        LanguageExt.Query.foldBack(list, state, folder);

    public static T Reduce<T>(this IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
        LanguageExt.Query.reduce(list, reducer);

    public static T ReduceBack<T>(this IQueryable<T> list, Expression<Func<T, T, T>> reducer) =>
        LanguageExt.Query.reduceBack(list, reducer);

    public static Lst<T> Freeze<T>(this IQueryable<T> list) =>
        LanguageExt.Query.freeze(list);

    public static IQueryable<V> Zip<T, U, V>(this IQueryable<T> list, IEnumerable<U> other, Expression<Func<T, U, V>> zipper) =>
        LanguageExt.Query.zip(list, other, zipper);

    public static int Length<T>(this IQueryable<T> list) =>
        LanguageExt.Query.length(list);

    public static bool ForAll<T>(this IQueryable<T> list, Expression<Func<T, bool>> pred) =>
        LanguageExt.Query.forall(list, pred);

    public static IQueryable<T> Distinct<T>(this IQueryable<T> list) =>
        LanguageExt.Query.distinct(list);

    public static bool Exists<T>(this IQueryable<T> list, Expression<Func<T, bool>> pred) =>
        LanguageExt.Query.exists(list, pred);
}
