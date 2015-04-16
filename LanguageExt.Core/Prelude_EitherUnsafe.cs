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
        public static EitherUnsafe<L, R> RightUnsafe<L, R>(R value) =>
            EitherUnsafe<L, R>.Right(value);

        public static EitherUnsafe<L, R> LeftUnsafe<L, R>(L value) =>
            EitherUnsafe<L, R>.Left(value);

        public static R failureUnsafe<L, R>(EitherUnsafe<L, R> either, Func<R> None) =>
            either.FailureUnsafe(None);

        public static R failureUnsafe<L, R>(EitherUnsafe<L, R> either, R noneValue) =>
            either.FailureUnsafe(noneValue);

        public static Ret matchUnsafe<L, R, Ret>(EitherUnsafe<L, R> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.MatchUnsafe(Right, Left);

        public static Unit matchUnsafe<L, R>(EitherUnsafe<L, R> either, Action<R> Right, Action<L> Left) =>
            either.MatchUnsafe(Right, Left);

        public static S foldUnsafe<S, L, R>(EitherUnsafe<L, R> either, S state, Func<S, R, S> folder) =>
            either.FoldUnsafe(state, folder);

        public static bool forallUnsafe<L, R>(EitherUnsafe<L, R> either, Func<R, bool> pred) =>
            either.ForAllUnsafe(pred);

        public static int count<L, R>(EitherUnsafe<L, R> either) =>
            either.Count;

        public static bool existsUnsafe<L, R>(EitherUnsafe<L, R> either, Func<R, bool> pred) =>
            either.ExistsUnsafe(pred);

        public static EitherUnsafe<L, Ret> mapUnsafe<L, R, Ret>(EitherUnsafe<L, R> either, Func<R, Ret> mapper) =>
            either.MapUnsafe(mapper);

        public static bool filterUnsafe<L, R>(EitherUnsafe<L, R> either, Func<R, bool> pred) =>
            either.FilterUnsafe(pred);

        public static EitherUnsafe<L, Ret> bindUnsafe<L, R, Ret>(EitherUnsafe<L, R> either, Func<R, EitherUnsafe<L, Ret>> binder) =>
            either.BindUnsafe(binder);

        public static IEnumerable<Ret> match<L, R, Ret>(IEnumerable<EitherUnsafe<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            list.Match(
                () => new Ret[0],
                val => val.Right(v => Right(v)).Left(v => Left(v)),
                (x, xs) => x.Right(v => Right(v)).Left(v => Left(v)).Concat(match(xs, Right, Left)) // TODO: Flatten recursion
            );

        public static IEnumerable<Ret> match<R, L, Ret>(IEnumerable<EitherUnsafe<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list, Right, _ => Left);

        public static IEnumerable<R> failure<L, R>(IEnumerable<EitherUnsafe<L, R>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            match(list, r => new R[1] { r }, Left);

        public static IEnumerable<R> failure<L, R>(IEnumerable<EitherUnsafe<L, R>> list,
            IEnumerable<R> Left
            ) =>
            match(list, r => new R[1] { r }, _ => Left);

        public static IEnumerable<R> failWithEmpty<L, R>(IEnumerable<EitherUnsafe<L, R>> list) =>
            match(list, r => new R[1] { r }, _ => new R[0]);

        public static IEnumerable<Ret> Match<R, L, Ret>(this IEnumerable<EitherUnsafe<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            match(list, Right, Left);

        public static IEnumerable<Ret> Match<R, L, Ret>(this IEnumerable<EitherUnsafe<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list, Right, Left);

        public static IEnumerable<R> Failure<L, R>(this IEnumerable<EitherUnsafe<L, R>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> Failure<L, R>(this IEnumerable<EitherUnsafe<L, R>> list,
            IEnumerable<R> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> FailWithEmpty<L, R>(this IEnumerable<EitherUnsafe<L, R>> list) =>
            failWithEmpty(list);

        public static IImmutableList<R> toList<L, R>(EitherUnsafe<L, R> either) =>
            either.ToList();

        public static ImmutableArray<R> toArray<L, R>(EitherUnsafe<L, R> either) =>
            either.ToArray();

        public static IQueryable<R> toQuery<L, R>(EitherUnsafe<L, R> either) =>
            either.AsQueryable();
    }
}
