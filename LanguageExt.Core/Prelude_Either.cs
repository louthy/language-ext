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
        public static Either<L, R> Right<L, R>(R value) =>
            Either<L, R>.Right(value);

        public static Either<L, R> Left<L, R>(L value) =>
            Either<L, R>.Left(value);

        public static Either<L, R> Right<L, R>(Nullable<R> value) where R : struct =>
            value == null
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Right(value.Value);

        public static Either<L, R> Left<L, R>(Nullable<L> value) where L : struct =>
            value == null
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Left(value.Value);

        public static R failure<L, R>(Either<L, R> either, Func<R> None) =>
            either.Failure(None);

        public static R failure<L, R>(Either<L, R> either, R noneValue) =>
            either.Failure(noneValue);

        public static Ret match<L, R, Ret>(Either<L, R> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.Match(Right, Left);

        public static Unit match<L, R>(Either<L, R> either, Action<R> Right, Action<L> Left) =>
            either.Match(Right, Left);

        public static S fold<S, L, R>(Either<L, R> either, S state, Func<S, R, S> folder) =>
            either.Fold(state, folder);

        public static bool forall<L, R>(Either<L, R> either, Func<R, bool> pred) =>
            either.ForAll(pred);

        public static int count<L, R>(Either<L, R> either) =>
            either.Count;

        public static bool exists<L, R>(Either<L, R> either, Func<R, bool> pred) =>
            either.Exists(pred);

        public static Either<L, Ret> map<L, R, Ret>(Either<L, R> either, Func<R, Ret> mapper) =>
            either.Map(mapper);

        public static bool filter<L, R>(Either<L, R> either, Func<R, bool> pred) =>
            either.Filter(pred);

        public static Either<L, Ret> bind<L, R, Ret>(Either<L, R> either, Func<R, Either<L, Ret>> binder) =>
            either.Bind(binder);

        public static IEnumerable<Ret> match<L, R, Ret>(IEnumerable<Either<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            list.Match(
                () => new Ret[0],
                val => val.Right(v => Right(v)).Left(v => Left(v)),
                (x, xs) => x.Right(v => Right(v)).Left(v => Left(v)).Concat(match(xs, Right, Left)) // TODO: Flatten recursion
            );

        public static IEnumerable<Ret> match<L, R, Ret>(IEnumerable<Either<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list, Right, _ => Left);

        public static IEnumerable<R> failure<L, R>(IEnumerable<Either<L, R>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            match(list, r => new R[1] { r }, Left);

        public static IEnumerable<R> failure<L, R>(IEnumerable<Either<L, R>> list,
            IEnumerable<R> Left
            ) =>
            match(list, r => new R[1] { r }, _ => Left);

        public static IEnumerable<R> failWithEmpty<L, R>(IEnumerable<Either<L, R>> list) =>
            match(list, r => new R[1] { r }, _ => new R[0]);

        public static IEnumerable<Ret> Match<L, R, Ret>(this IEnumerable<Either<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            Func<L, IEnumerable<Ret>> Left
            ) =>
            match(list, Right, Left);

        public static IEnumerable<Ret> Match<L, R, Ret>(this IEnumerable<Either<L, R>> list,
            Func<R, IEnumerable<Ret>> Right,
            IEnumerable<Ret> Left
            ) =>
            match(list, Right, Left);

        public static IEnumerable<R> Failure<L, R>(this IEnumerable<Either<L, R>> list,
            Func<L, IEnumerable<R>> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> Failure<L, R>(this IEnumerable<Either<L, R>> list,
            IEnumerable<R> Left
            ) =>
            failure(list, Left);

        public static IEnumerable<R> FailWithEmpty<L, R>(this IEnumerable<Either<L, R>> list) =>
            failWithEmpty(list);

        public static IImmutableList<R> toList<L, R>(Either<L, R> either) =>
            either.ToList();

        public static ImmutableArray<R> toArray<L, R>(Either<L, R> either) =>
            either.ToArray();

        public static IQueryable<R> toQuery<L, R>(Either<L, R> either) =>
            either.AsQueryable();
    }
}
