using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Check if Option is in a Some state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Option</param>
        /// <returns>True if value is in a Some state</returns>
        public static bool isSome<T>(Option<T> value) =>
            value.IsSome;

        /// <summary>
        /// Check if Option is in a None state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Option</param>
        /// <returns>True if value is in a None state</returns>
        public static bool isNone<T>(Option<T> value) =>
            value.IsNone;

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

        public static Unit ifSome<T>(Option<T> option, Action<T> Some) => 
            option.IfSome(Some);

        public static T ifNone<T>(Option<T> option, Func<T> None) =>
            option.IfNone(None);

        public static T ifNone<T>(Option<T> option, T noneValue) =>
            option.IfNone(noneValue);

        public static T ifNoneUnsafe<T>(Option<T> option, Func<T> None) =>
            option.IfNoneUnsafe(None);

        public static T ifNoneUnsafe<T>(Option<T> option, T noneValue) =>
            option.IfNoneUnsafe(noneValue);

        public static R match<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.Match(Some, None);

        public static R matchUnsafe<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        public static Unit match<T>(Option<T> option, Action<T> Some, Action None) =>
            option.Match(Some, None);

        /// <summary>
        /// Apply an Optional value to an Optional function
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        public static Option<R> apply<T, R>(Option<Func<T, R>> option, Option<T> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply an Optional value to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg">Optional argument</param>
        /// <returns>Returns the result of applying the optional argument to the optional function:
        /// an optonal function of arity 1</returns>
        public static Option<Func<T2, R>> apply<T1, T2, R>(Option<Func<T1, T2, R>> option, Option<T1> arg) =>
            option.Apply(arg);

        /// <summary>
        /// Apply Optional values to an Optional function of arity 2
        /// </summary>
        /// <param name="option">Optional function</param>
        /// <param name="arg1">Optional argument</param>
        /// <param name="arg2">Optional argument</param>
        /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
        public static Option<R> apply<T1, T2, R>(Option<Func<T1, T2, R>> option, Option<T1> arg1, Option<T2> arg2) =>
            option.Apply(arg1, arg2);

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

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        public static Option<Func<T2, R>> map<T1, T2, R>(Option<T1> option, Func<T1, T2, R> mapper) =>
            option.Map(mapper);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        public static Option<Func<T2, Func<T3, R>>> map<T1, T2, T3, R>(Option<T1> option, Func<T1, T2, T3, R> mapper) =>
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

        public static T[] toArray<T>(Option<T> option) =>
            option.ToArray();

        public static IQueryable<T> toQuery<T>(Option<T> option) =>
            option.AsEnumerable().AsQueryable();
    }
}
