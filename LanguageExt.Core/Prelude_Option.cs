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

        public static T failure<T>(Option<T> option, Func<T> None) =>
            option.Failure(None);

        public static T failure<T>(Option<T> option, T noneValue) =>
            option.Failure(noneValue);

        public static R match<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.Match(Some, None);

        public static Unit match<T>(Option<T> option, Action<T> Some, Action None) =>
            option.Match(Some, None);

        public static S fold<S, T>(Option<T> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(Option<T> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(Option<T> option) =>
            option.Count;

        public static bool exists<T>(Option<T> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static Option<R> map<T, R>(Option<T> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static bool filter<T>(Option<T> option, Func<T, bool> pred) =>
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

        public static IImmutableList<T> toList<T>(Option<T> option) =>
            option.ToList();

        public static ImmutableArray<T> toArray<T>(Option<T> option) =>
            option.ToArray();

        public static IQueryable<T> toQuery<T>(Option<T> option) =>
            option.AsQueryable();
    }
}
