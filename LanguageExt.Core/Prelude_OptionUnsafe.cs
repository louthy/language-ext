using System;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class Prelude
    {
        public static bool isSome<T>(OptionUnsafe<T> value) =>
            value.IsSome;

        public static bool isNone<T>(OptionUnsafe<T> value) =>
            value.IsNone;

        /// <summary>
        /// Create a Some of T (OptionUnsafe<T>).  Use the to wrap any-type without coercian.
        /// That means you can wrap null, Nullable<T>, or Option<T> to get Option<Option<T>>
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to make optional</param>
        /// <returns>Option<T> in a Some state</returns>
        public static OptionUnsafe<T> SomeUnsafe<T>(T value) =>
            OptionUnsafe<T>.Some(value);

        public static Unit ifSomeUnsafe<T>(OptionUnsafe<T> option, Action<T> Some) =>
            option.IfSomeUnsafe(Some);

        public static T ifNoneUnsafe<T>(OptionUnsafe<T> option, Func<T> None) =>
            option.IfNoneUnsafe(None);

        public static T ifNoneUnsafe<T>(OptionUnsafe<T> option, T noneValue) =>
            option.IfNoneUnsafe(noneValue);

        public static R matchUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        public static Unit matchUnsafe<T>(OptionUnsafe<T> option, Action<T> Some, Action None) =>
            option.MatchUnsafe(Some, None);

        public static S fold<S, T>(OptionUnsafe<T> option, S state, Func<S, T, S> folder) =>
            option.Fold(state, folder);

        public static bool forall<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.ForAll(pred);

        public static int count<T>(OptionUnsafe<T> option) =>
            option.Count();

        public static bool exists<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.Exists(pred);

        public static OptionUnsafe<R> map<T, R>(OptionUnsafe<T> option, Func<T, R> mapper) =>
            option.Map(mapper);

        public static OptionUnsafe<Func<T2, R>> map<T1, T2, R>(OptionUnsafe<T1> option, Func<T1, T2, R> mapper) =>
            option.Map(mapper);

        public static OptionUnsafe<Func<T2, Func<T3, R>>> map<T1, T2, T3, R>(OptionUnsafe<T1> option, Func<T1, T2, T3, R> mapper) =>
            option.Map(mapper);

        public static OptionUnsafe<T> filter<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.Filter(pred);

        public static OptionUnsafe<R> bind<T, R>(OptionUnsafe<T> option, Func<T, OptionUnsafe<R>> binder) =>
            option.Bind(binder);

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

        /// <summary>
        /// Apply an Optional value to an Optional function
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        public static OptionUnsafe<R> apply<T, R>(OptionUnsafe<Func<T, R>> option, OptionUnsafe<T> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply an Optional value to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function:
        /// an optonal function of arity 1</returns>
        public static OptionUnsafe<Func<T2, R>> apply<T1, T2, R>(OptionUnsafe<Func<T1, T2, R>> option, OptionUnsafe<T1> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply Optional values to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg1">Optional argument</param>
        /// <param name="arg2">Optional argument</param>
        /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
        public static OptionUnsafe<R> apply<T1, T2, R>(OptionUnsafe<Func<T1, T2, R>> option, OptionUnsafe<T1> arg1, OptionUnsafe<T2> arg2) =>
            option.Apply(arg1, arg2);

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


        public static Lst<T> toList<T>(OptionUnsafe<T> option) =>
            option.ToList();

        public static T[] toArray<T>(OptionUnsafe<T> option) =>
            option.ToArray();

        public static IQueryable<T> toQuery<T>(OptionUnsafe<T> option) =>
            option.AsEnumerable().AsQueryable();
    }
}
