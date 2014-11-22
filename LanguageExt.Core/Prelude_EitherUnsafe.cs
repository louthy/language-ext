using System;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using TvdP.Collections;

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

    }
}
