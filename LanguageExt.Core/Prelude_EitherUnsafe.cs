using System;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
    public static partial class Prelude
    {
        public static EitherUnsafe<R, L> RightUnsafe<R, L>(R value) =>
            EitherUnsafe<R, L>.Right(value);

        public static EitherUnsafe<R, L> LeftUnsafe<R, L>(L value) =>
            EitherUnsafe<R, L>.Left(value);

        public static R failureUnsafe<R, L>(EitherUnsafe<R, L> either, Func<R> None) =>
            either.FailureUnsafe(None);

        public static R failureUnsafe<R, L>(EitherUnsafe<R, L> either, R noneValue) =>
            either.FailureUnsafe(noneValue);

        public static Ret matchUnsafe<R, L, Ret>(EitherUnsafe<R, L> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.MatchUnsafe(Right, Left);

        public static Unit matchUnsafe<R, L>(EitherUnsafe<R, L> either, Action<R> Right, Action<L> Left) =>
            either.MatchUnsafe(Right, Left);

        public static S foldUnsafe<S, R, L>(EitherUnsafe<R, L> either, S state, Func<S, R, S> folder) =>
            either.FoldUnsafe(state, folder);

        public static bool forallUnsafe<R, L>(EitherUnsafe<R, L> either, Func<R, bool> pred) =>
            either.ForAllUnsafe(pred);

        public static int count<R, L>(EitherUnsafe<R, L> either) =>
            either.Count;

        public static bool existsUnsafe<R, L>(EitherUnsafe<R, L> either, Func<R, bool> pred) =>
            either.ExistsUnsafe(pred);

        public static EitherUnsafe<Ret, L> mapUnsafe<R, L, Ret>(EitherUnsafe<R, L> either, Func<R, Ret> mapper) =>
            either.MapUnsafe(mapper);

        public static EitherUnsafe<Ret, L> bindUnsafe<R, L, Ret>(EitherUnsafe<R, L> either, Func<R, EitherUnsafe<Ret, L>> binder) =>
            either.BindUnsafe(binder);

        public static IEnumerable<Ret> match<R, L, Ret>(IEnumerable<EitherUnsafe<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            list.Match(
                () => new Ret[0],
                val => val.Right(v => Right(v)).Left(v => Left(v)),
                (x, xs) => x.Right(v => Right(v)).Left(v => Left(v)).Concat(match(xs, Right, Left)) // TODO: Flatten recursion
            );

        public static IEnumerable<Ret> Match<R, L, Ret>(this IEnumerable<EitherUnsafe<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            match(list, Right, Left);

        public static IEnumerable<Ret> match<R, L, Ret>(IEnumerable<EitherUnsafe<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list, Right, _ => Left);

        public static IEnumerable<Ret> Match<R, L, Ret>(this IEnumerable<EitherUnsafe<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list, Right, Left);

        public static IEnumerable<R> failure<R, L>(IEnumerable<EitherUnsafe<R, L>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            match(list, r => new R[1] { r }, Left);

        public static IEnumerable<R> Failure<R, L>(this IEnumerable<EitherUnsafe<R, L>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> failure<R, L>(IEnumerable<EitherUnsafe<R, L>> list,
            IEnumerable<R> Left
            ) =>
            match(list, r => new R[1] { r }, _ => Left);

        public static IEnumerable<R> Failure<R, L>(this IEnumerable<EitherUnsafe<R, L>> list,
            IEnumerable<R> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> failWithEmpty<R, L>(IEnumerable<EitherUnsafe<R, L>> list) =>
            match(list, r => new R[1] { r }, _ => new R[0]);

        public static IEnumerable<R> FailWithEmpty<R, L>(this IEnumerable<EitherUnsafe<R, L>> list) =>
            failWithEmpty(list);
    }
}
