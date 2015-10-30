using System;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append the Some(x) of one option to the Some(y) of another.  If either of the
        /// options are None then the result is None
        /// For numeric values the behaviour is to sum the Somes (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the R type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        public static OptionUnsafe<T> append<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Subtract the Some(x) of one option from the Some(y) of another.  If either of the
        /// options are None then the result is None
        /// For numeric values the behaviour is to find the difference between the Somes (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        public static OptionUnsafe<T> subtract<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Find the product of the Somes.  If either of the options are None then the result is None
        /// For numeric values the behaviour is to multiply the Somes (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IProductable then the behaviour
        /// is to call lhs.Product(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        public static OptionUnsafe<T> product<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Multiply(rhs);

        /// <summary>
        /// Divide the Somes.  If either of the options are None then the result is None
        /// For numeric values the behaviour is to divide the Somes (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        public static OptionUnsafe<T> divide<T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) =>
            lhs.Divide(rhs);

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

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        public static OptionUnsafe<Func<T2, R>> map<T1, T2, R>(OptionUnsafe<T1> option, Func<T1, T2, R> mapper) =>
            option.Map(mapper);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
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

        public static Lst<T> toList<T>(OptionUnsafe<T> option) =>
            option.ToList();

        public static T[] toArray<T>(OptionUnsafe<T> option) =>
            option.ToArray();

        public static IQueryable<T> toQuery<T>(OptionUnsafe<T> option) =>
            option.AsEnumerable().AsQueryable();
    }
}
