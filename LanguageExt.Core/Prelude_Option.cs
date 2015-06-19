using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// Create a Some of T (Option<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>Option<T> in a Some state or throws ValueIsNullException
        /// if value == null.</returns>
        public static Option<T> Some<T>(T value) =>
            value == null
                ? raise<Option<T>>(new ValueIsNullException())
                : Option<T>.Some(value);

        /// <summary>
        /// Create a Some of T from a Nullable<T> (Option<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>Option<T> in a Some state or throws ValueIsNullException
        /// if value == null</returns>
        public static Option<T> Some<T>(Nullable<T> value) where T : struct =>
            value.HasValue
                ? Option<T>.Some(value.Value)
                : raise<Option<T>>(new ValueIsNullException());

        /// <summary>
        /// Create an Option
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>If the value is null it will be None else Some(value)</returns>
        public static Option<T> Optional<T>(T value) =>
            value == null
                ? Option<T>.None
                : Option<T>.Some(value);

        /// <summary>
        /// Create an Option
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>If the value is null it will be None else Some(value)</returns>
        public static Option<T> Optional<T>(Nullable<T> value) where T : struct =>
            value.HasValue
                ? Option<T>.Some(value.Value)
                : Option<T>.None;

        [Obsolete("'failure' has been deprecated.  Please use 'ifNone' instead")]
        public static T failure<T>(Option<T> option, Func<T> None) =>
            option.Failure(None);

        [Obsolete("'failure' has been deprecated.  Please use 'ifNone' instead")]
        public static T failure<T>(Option<T> option, T noneValue) =>
            option.Failure(noneValue);

        public static Unit ifSome<T>(Option<T> option, Action<T> Some) => 
            option.IfSome(Some);

        public static T ifNone<T>(Option<T> option, Func<T> None) =>
            option.IfNone(None);

        public static T ifNone<T>(Option<T> option, T noneValue) =>
            option.IfNone(noneValue);

        public static R match<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.Match(Some, None);

        public static Unit match<T>(Option<T> option, Action<T> Some, Action None) =>
            option.Match(Some, None);

        public static S fold<S, T>(Option<T> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(Option<T> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(Option<T> option) =>
            option.Count();

        public static bool exists<T>(Option<T> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static Option<R> map<T, R>(Option<T> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static Option<T> filter<T>(Option<T> option, Func<T, bool> pred) =>
            option.Filter(pred);

        public static Option<R> bind<T, R>(Option<T> option, Func<T, Option<R>> binder) =>
            option.Bind(binder);

        public static IEnumerable<R> match<T, R>(IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            list.Match(
                None,
                opt => opt.Some(v => Some(v)).None(None),
                (x, xs) => x.Some(v => Some(v)).None(None).Concat(match(xs, Some, None)) // TODO: Flatten recursion
            );

        public static IEnumerable<R> match<T, R>(IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            match(list, Some, () => None);

        public static IEnumerable<T> failure<T>(IEnumerable<Option<T>> list,
            Func<IEnumerable<T>> None
            ) =>
            match(list, v => new T[1] { v }, None);

        public static IEnumerable<T> failure<T>(IEnumerable<Option<T>> list,
            IEnumerable<T> None
            ) =>
            match(list, v => new T[1] { v }, () => None);

        public static IEnumerable<R> Match<T, R>(this IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            match(list, Some, None);

        public static IEnumerable<R> Match<T, R>(this IEnumerable<Option<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            match(list, Some, () => None);

        public static IEnumerable<T> Failure<T>(this IEnumerable<Option<T>> list,
            Func<IEnumerable<T>> None
            ) =>
            match(list, v => new T[1] { v }, None);

        public static IEnumerable<T> Failure<T>(this IEnumerable<Option<T>> list,
            IEnumerable<T> None
            ) =>
            match(list, v => new T[1] { v }, () => None);

        public static Lst<T> toList<T>(Option<T> option) =>
            option.ToList();

        public static ImmutableArray<T> toArray<T>(Option<T> option) =>
            option.ToArray();

        public static IQueryable<T> toQuery<T>(Option<T> option) =>
            option.AsQueryable();

        // 
        // Option<IEnumerable<T>>
        // 

        public static S fold<S, T>(Option<IEnumerable<T>> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(Option<IEnumerable<T>> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(Option<IEnumerable<T>> option) =>
            option.Count();

        public static bool exists<T>(Option<IEnumerable<T>> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static Option<IEnumerable<R>> map<T, R>(Option<IEnumerable<T>> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static Option<IEnumerable<T>> filter<T>(Option<IEnumerable<T>> option, Func<T, bool> pred) =>
            option.Filter(pred);

        public static Option<IEnumerable<R>> bind<T, R>(Option<IEnumerable<T>> option, Func<T, Option<R>> binder) =>
            option.Bind(binder);

        // 
        // Option<Lst<T>>
        // 

        public static S fold<S, T>(Option<Lst<T>> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(Option<Lst<T>> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(Option<Lst<T>> option) =>
            option.Count();

        public static bool exists<T>(Option<Lst<T>> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static Option<Lst<R>> map<T, R>(Option<Lst<T>> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static Option<Lst<T>> filter<T>(Option<Lst<T>> option, Func<T, bool> pred) =>
            option.Filter(pred);

        public static Option<Lst<R>> bind<T, R>(Option<Lst<T>> option, Func<T, Option<R>> binder) =>
            option.Bind(binder);

        // 
        // Option<Option<T>>
        // 

        public static S fold<S, T>(Option<Option<T>> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(Option<Option<T>> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(Option<Option<T>> option) =>
            option.Count();

        public static bool exists<T>(Option<Option<T>> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static Option<Option<R>> map<T, R>(Option<Option<T>> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static Option<Option<T>> filter<T>(Option<Option<T>> option, Func<T, bool> pred) =>
            option.Filter(pred);

        public static Option<Option<R>> bind<T, R>(Option<Option<T>> option, Func<T, Option<R>> binder) =>
            option.Bind(binder);

        // 
        // Option<TryOption<T>>
        // 

        public static S fold<S, T>(Option<TryOption<T>> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(Option<TryOption<T>> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(Option<TryOption<T>> option) =>
            option.Count();

        public static bool exists<T>(Option<TryOption<T>> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static Option<TryOption<R>> map<T, R>(Option<TryOption<T>> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static Option<TryOption<T>> filter<T>(Option<TryOption<T>> option, Func<T, bool> pred) =>
            option.Filter(pred);

        public static Option<TryOption<R>> bind<T, R>(Option<TryOption<T>> option, Func<T, TryOption<R>> binder) =>
            option.Bind(binder);

        // 
        // Option<Try<T>>
        // 

        public static S fold<S, T>(Option<Try<T>> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(Option<Try<T>> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(Option<Try<T>> option) =>
            option.Count();

        public static bool exists<T>(Option<Try<T>> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static Option<Try<R>> map<T, R>(Option<Try<T>> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static Option<Try<T>> filter<T>(Option<Try<T>> option, Func<T, bool> pred) =>
            option.Filter(pred);

        public static Option<Try<R>> bind<T, R>(Option<Try<T>> option, Func<T, Try<R>> binder) =>
            option.Bind(binder);

        // 
        // Option<Either<L,R>>
        // 

        public static S fold<L, R, S>(Option<Either<L, R>> option, S state, Func<S, R, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<L,R>(Option<Either<L, R>> option, Func<R, bool> pred) =>
            option.ForAll(pred);

        public static int count<L, R>(Option<Either<L, R>> option) =>
            option.Count();

        public static bool exists<L, R>(Option<Either<L, R>> option, Func<R, bool> pred) =>
            option.Exists(pred);

        public static Option<Either<L, R2>> map<L, R, R2>(Option<Either<L, R>> option, Func<R, R2> mapper) =>
            option.Map(mapper);

        public static Option<Either<Unit, R>> filter<L, R>(Option<Either<L, R>> option, Func<R, bool> pred) =>
            option.Filter(pred);

        public static Option<Either<L, R2>> bind<L, R, R2>(Option<Either<L, R>> option, Func<R, Either<L, R2>> binder) =>
            option.Bind(binder);
    }
}
