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
        /// <summary>
        /// Create a Some of T (OptionUnsafe<T>).  Use the to wrap any-type without coercian.
        /// That means you can wrap null, Nullable<T>, or Option<T> to get Option<Option<T>>
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to make optional</param>
        /// <returns>Option<T> in a Some state</returns>
        public static OptionUnsafe<T> SomeUnsafe<T>(T value) =>
            OptionUnsafe<T>.Some(value);

        public static T failureUnsafe<T>(OptionUnsafe<T> option, Func<T> None) =>
            option.FailureUnsafe(None);

        public static T failure<T>(OptionUnsafe<T> option, T noneValue) =>
            option.FailureUnsafe(noneValue);

        public static R matchUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        public static Unit matchUnsafe<T>(OptionUnsafe<T> option, Action<T> Some, Action None) =>
            option.MatchUnsafe(Some, None);

        public static S foldUnsafe<S, T>(OptionUnsafe<T> option, S state, Func<S, T, S> folder) =>
            option.FoldUnsafe(state, folder);

        public static bool forallUnsafe<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.ForAllUnsafe(pred);

        public static int count<T>(OptionUnsafe<T> option) =>
            option.Count;

        public static bool existsUnsafe<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.ExistsUnsafe(pred);

        public static OptionUnsafe<R> mapUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> mapper) =>
            option.MapUnsafe(mapper);

        public static bool filterUnsafe<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.FilterUnsafe(pred);

        public static OptionUnsafe<R> bindUnsafe<T, R>(OptionUnsafe<T> option, Func<T, OptionUnsafe<R>> binder) =>
            option.BindUnsafe(binder);

        public static IEnumerable<R> matchUnsafe<T, R>(IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            list.Match(
                None,
                opt => opt.SomeUnsafe(v => Some(v)).None(None),
                (x, xs) => x.SomeUnsafe(v => Some(v)).None(None).Concat(matchUnsafe(xs, Some, None)) // TODO: Flatten recursion
            );

        public static IEnumerable<R> matchUnsafe<T, R>(IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            matchUnsafe(list, Some, () => None);

        public static IEnumerable<T> failureUnsafe<T>(IEnumerable<OptionUnsafe<T>> list,
            Func<IEnumerable<T>> None
            ) =>
            matchUnsafe(list, v => new T[1] { v }, None);

        public static IEnumerable<T> failureUnsafe<T>(IEnumerable<OptionUnsafe<T>> list,
            IEnumerable<T> None
            ) =>
            matchUnsafe(list, v => new T[1] { v }, () => None);

        public static IEnumerable<R> MatchUnsafe<T, R>(this IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            matchUnsafe(list, Some, None);

        public static IEnumerable<R> MatchUnsafe<T, R>(this IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            matchUnsafe(list, Some, () => None);

        public static IEnumerable<T> FailureUnsafe<T>(this IEnumerable<OptionUnsafe<T>> list,
            Func<IEnumerable<T>> None
            ) =>
            matchUnsafe(list, v => new T[1] { v }, None);

        public static IEnumerable<T> FailureUnsafe<T>(this IEnumerable<OptionUnsafe<T>> list,
            IEnumerable<T> None
            ) =>
            matchUnsafe(list, v => new T[1] { v }, () => None);


        public static IImmutableList<T> toList<T>(OptionUnsafe<T> option) =>
            option.ToList();

        public static ImmutableArray<T> toArray<T>(OptionUnsafe<T> option) =>
            option.ToArray();

        public static IQueryable<T> toQuery<T>(OptionUnsafe<T> option) =>
            option.AsQueryable();
    }
}
