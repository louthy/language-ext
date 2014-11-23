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
        public static Either<R, L> Right<R, L>(R value) =>
            Either<R, L>.Right(value);

        public static Either<R, L> Left<R, L>(L value) =>
            Either<R, L>.Left(value);

        public static Either<R, L> Right<R, L>(Nullable<R> value) where R : struct =>
            value == null
                ? raise<Either<R, L>>(new ValueIsNullException())
                : Either<R, L>.Right(value.Value);

        public static Either<R, L> Left<R, L>(Nullable<L> value) where L : struct =>
            value == null
                ? raise<Either<R, L>>(new ValueIsNullException())
                : Either<R, L>.Left(value.Value);

        public static R failure<R, L>(Either<R, L> either, Func<R> None) =>
            either.Failure(None);

        public static R failure<R, L>(Either<R, L> either, R noneValue) =>
            either.Failure(noneValue);

        public static Ret match<R, L, Ret>(Either<R, L> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.Match(Right, Left);

        public static Unit match<R, L>(Either<R, L> either, Action<R> Right, Action<L> Left) =>
            either.Match(Right, Left);

        public static S fold<S, R, L>(Either<R,L> either, S state, Func<S, R, S> folder) =>
            either.Fold(state, folder);

        public static bool forall<R, L>(Either<R, L> either, Func<R, bool> pred) =>
            either.ForAll(pred);

        public static int count<R, L>(Either<R, L> either) =>
            either.Count;

        public static bool exists<R, L>(Either<R, L> either, Func<R, bool> pred) =>
            either.Exists(pred);

        public static Either<Ret, L> map<R, L, Ret>(Either<R, L> either, Func<R, Ret> mapper) =>
            either.Map(mapper);

        public static Either<Ret, L> bind<R, L, Ret>(Either<R, L> either, Func<R, Either<Ret, L>> binder) =>
            either.Bind(binder);

        public static IEnumerable<Ret> match<R, L, Ret>(IEnumerable<Either<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            list.Match(
                () => new Ret[0],
                val => val.Right(v => Right(v)).Left(v => Left(v)),
                (x, xs) => x.Right(v => Right(v)).Left(v => Left(v)).Concat(match(xs, Right, Left)) // TODO: Flatten recursion
            );

        public static IEnumerable<Ret> Match<R, L, Ret>(this IEnumerable<Either<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            match(list, Right, Left);

        public static IEnumerable<Ret> match<R, L, Ret>(IEnumerable<Either<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list, Right, _ => Left);

        public static IEnumerable<Ret> Match<R, L, Ret>(this IEnumerable<Either<R, L>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list,Right,Left);

        public static IEnumerable<R> failure<R, L>(IEnumerable<Either<R, L>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            match(list, r => new R[1] { r }, Left);

        public static IEnumerable<R> Failure<R, L>(this IEnumerable<Either<R, L>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> failure<R, L>(IEnumerable<Either<R, L>> list,
            IEnumerable<R> Left
            ) =>
            match(list, r => new R[1] { r }, _ => Left);

        public static IEnumerable<R> Failure<R, L>(this IEnumerable<Either<R, L>> list,
            IEnumerable<R> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> failWithEmpty<R, L>(IEnumerable<Either<R, L>> list) =>
            match(list, r => new R[1] { r }, _ => new R[0]);

        public static IEnumerable<R> FailWithEmpty<R, L>(this IEnumerable<Either<R, L>> list) =>
            failWithEmpty(list);

        public static IImmutableList<R> toList<R, L>(Either<R, L> either) =>
            either.ToList();

        public static ImmutableArray<R> toArray<R, L>(Either<R, L> either) =>
            either.ToArray();
    }
}
