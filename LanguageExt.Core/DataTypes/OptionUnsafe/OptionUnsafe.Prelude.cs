using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using LanguageExt.Instances;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Subtract the Ts
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static OptionUnsafe<T> subtract<NUM, T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) where NUM : struct, Num<T> =>
            lhs.Subtract<NUM, T>(rhs);

        /// <summary>
        /// Find the product of the Ts
        [Pure]
        public static OptionUnsafe<T> product<NUM, T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) where NUM : struct, Num<T> =>
            lhs.Product<NUM, T>(rhs);

        /// <summary>
        /// Divide the Ts
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static OptionUnsafe<T> divide<NUM, T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) where NUM : struct, Num<T> =>
            lhs.Divide<NUM, T>(rhs);

        /// <summary>
        /// Add the Ts
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static OptionUnsafe<T> add<NUM, T>(OptionUnsafe<T> lhs, OptionUnsafe<T> rhs) where NUM : struct, Num<T> =>
            lhs.Add<NUM, T>(rhs);

        /// <summary>
        /// Check if OptionUnsafe is in a Some state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">OptionUnsafe</param>
        /// <returns>True if value is in a Some state</returns>
        [Pure]
        public static bool isSome<T>(OptionUnsafe<T> value) =>
            value.IsSome;

        /// <summary>
        /// Check if OptionUnsafe is in a None state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">OptionUnsafe</param>
        /// <returns>True if value is in a None state</returns>
        [Pure]
        public static bool isNone<T>(OptionUnsafe<T> value) =>
            value.IsNone;

        /// <summary>
        /// Create a Some of T (OptionUnsafe<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>OptionUnsafe<T> in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        [Pure]
        public static OptionUnsafe<T> SomeUnsafe<T>(T value) =>
            default(MOptionUnsafe<T>).Return(value);

        /// <summary>
        /// Invokes the action if OptionUnsafe is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">Action to invoke if OptionUnsafe is in the Some state</param>
        public static Unit ifSomeUnsafe<T>(OptionUnsafe<T> option, Action<T> Some) =>
            option.IfSomeUnsafe(Some);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public static T ifNoneUnsafe<T>(OptionUnsafe<T> option, Func<T> None) =>
            option.IfNoneUnsafe(None);

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public static T ifNoneUnsafe<T>(OptionUnsafe<T> option, T noneValue) =>
            option.IfNoneUnsafe(noneValue);

        /// <summary>
        /// Match the two states of the OptionUnsafe and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public static R matchUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        /// <summary>
        /// Match the two states of the OptionUnsafe
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        public static Unit matchUnsafe<T>(OptionUnsafe<T> option, Action<T> Some, Action None) =>
            option.MatchUnsafe(Some, None);

        /// <summary>
        /// Apply an Optional argument to an Optional function of arity 1
        /// </summary>
        /// <param name="x">Optional function</param>
        /// <param name="y">Argument to apply</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        [Pure]
        public static OptionUnsafe<B> apply<A, B>(OptionUnsafe<Func<A, B>> x, OptionUnsafe<A> y) =>
            apply<MOptionUnsafe<Func<A, B>>, MOptionUnsafe<A>, MOptionUnsafe<B>, OptionUnsafe<Func<A, B>>, OptionUnsafe<A>, OptionUnsafe<B>, A, B>(x, y);

        /// <summary>
        /// Apply two Optional arguments to an Optional function of arity 2
        /// </summary>
        /// <param name="x">Optional function</param>
        /// <param name="y">Argument to apply</param>
        /// <param name="z">Argument to apply</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        [Pure]
        public static OptionUnsafe<C> apply<A, B, C>(OptionUnsafe<Func<A, B, C>> x, OptionUnsafe<A> y, OptionUnsafe<B> z) =>
            apply<MOptionUnsafe<Func<A, B, C>>, MOptionUnsafe<A>, MOptionUnsafe<B>, MOptionUnsafe<C>, OptionUnsafe<Func<A, B, C>>, OptionUnsafe<A>, OptionUnsafe<B>, OptionUnsafe<C>, A, B, C>(x, y, z);

        /// <summary>
        /// Apply one Optional arguments to an Optional function of arity 2
        /// </summary>
        /// <param name="x">Optional function</param>
        /// <param name="y">Argument to apply</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        [Pure]
        public static OptionUnsafe<Func<B, C>> apply<A, B, C>(OptionUnsafe<Func<A, B, C>> x, OptionUnsafe<A> y) =>
            apply<MOptionUnsafe<Func<A, B, C>>, MOptionUnsafe<A>, MOptionUnsafe<Func<B, C>>, OptionUnsafe<Func<A, B, C>>, OptionUnsafe<A>, OptionUnsafe<Func<B, C>>, A, B, C>(x, y);

        /// <summary>
        /// Apply one Optional arguments to an Optional function of arity 2
        /// </summary>
        /// <param name="x">Optional function</param>
        /// <param name="y">Argument to apply</param>
        /// <returns>Returns the result of applying the optional argument to the optional function</returns>
        [Pure]
        public static OptionUnsafe<Func<B, C>> apply<A, B, C>(OptionUnsafe<Func<A, Func<B, C>>> x, OptionUnsafe<A> y) =>
            apply2<MOptionUnsafe<Func<A, Func<B, C>>>, MOptionUnsafe<A>, MOptionUnsafe<Func<B, C>>, OptionUnsafe<Func<A, Func<B, C>>>, OptionUnsafe<A>, OptionUnsafe<Func<B, C>>, A, B, C>(x, y);

        /// <summary>
        /// Partially apply an Optional argument to a curried Optional function
        /// </summary>
        [Pure]
        public static OptionUnsafe<B> action<A, B>(OptionUnsafe<A> x, OptionUnsafe<B> y) =>
            action<MOptionUnsafe<A>, MOptionUnsafe<B>, OptionUnsafe<A>, OptionUnsafe<B>, A, B>(x, y);

        /// <summary>
        /// <para>
        /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Fold([x1, x2, ..., xn] == x1 `f` (x2 `f` ... (xn `f` z)...)
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if OptionUnsafe is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static S fold<S, A>(OptionUnsafe<A> option, S state, Func<S, A, S> folder) =>
            option.Fold(state, folder);

        /// <summary>
        /// <para>
        /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Fold([x1, x2, ..., xn] == x1 `f` (x2 `f` ... (xn `f` z)...)
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionUnsafe is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionUnsafe is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static S bifold<S, A>(OptionUnsafe<A> option, S state, Func<S, A, S> Some, Func<S, S> None) =>
            option.BiFold(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Fold([x1, x2, ..., xn] == x1 `f` (x2 `f` ... (xn `f` z)...)
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionUnsafe is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionUnsafe is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static S bifold<S, A>(OptionUnsafe<A> option, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            option.BiFold(state, Some, None);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned (because the predicate applies for-all values).
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the predicate supplied.        
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned (because 
        /// the predicate applies for-all values).  If the OptionUnsafe is in a Some state
        /// the value is the result of running applying the bound value to the 
        /// predicate supplied.</returns>
        [Pure]
        public static bool forall<A>(OptionUnsafe<A> option, Func<A, bool> pred) =>
            option.ForAll(pred);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public static bool biforall<A>(OptionUnsafe<A> option, Func<A, bool> Some, Func<Unit, bool> None) =>
            option.BiForAll(Some, None);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public static bool biforall<A>(OptionUnsafe<A> option, Func<A, bool> Some, Func<bool> None) =>
            option.BiForAll(Some, None);

        /// <summary>
        /// <para>
        /// Return the number of bound values in this structure:
        /// </para>
        /// <para>
        ///     None = 0
        /// </para>
        /// <para>
        ///     Some = 1
        /// </para>
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static int count<A>(OptionUnsafe<A> option) =>
            option.Count();

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public static bool exists<A>(OptionUnsafe<A> option, Func<A, bool> pred) =>
            option.Exists(pred);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public static bool biexists<A>(OptionUnsafe<A> option, Func<A, bool> Some, Func<Unit, bool> None) =>
            option.BiExists(Some, None);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public static bool biexists<A>(OptionUnsafe<A> option, Func<A, bool> Some, Func<bool> None) =>
            option.BiExists(Some, None);

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static OptionUnsafe<B> map<A, B>(OptionUnsafe<A> option, Func<A, B> f) =>
            option.Map(f);

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static OptionUnsafe<B> bimap<A, B>(OptionUnsafe<A> option, Func<A, B> Some, Func<B> None) =>
            option.BiMap(Some, None);

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static OptionUnsafe<B> bimap<A, B>(OptionUnsafe<A> option, Func<A, B> Some, Func<Unit, B> None) =>
            option.BiMap(Some, None);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static OptionUnsafe<Func<T2, R>> parmap<T1, T2, R>(OptionUnsafe<T1> option, Func<T1, T2, R> mapper) =>
            option.ParMap(mapper);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static OptionUnsafe<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(OptionUnsafe<T1> option, Func<T1, T2, T3, R> mapper) =>
            option.ParMap(mapper);

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the OptionUnsafe is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public static OptionUnsafe<T> filter<T>(OptionUnsafe<T> option, Func<T, bool> pred) =>
            option.Filter(pred);

        /// <summary>
        /// Monadic bind operation
        /// </summary>
        [Pure]
        public static OptionUnsafe<R> bind<T, R>(OptionUnsafe<T> option, Func<T, OptionUnsafe<R>> binder) =>
            option.Bind(binder);

        /// <summary>
        /// Match the two states of the list of OptionUnsafes
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        [Pure]
        public static IEnumerable<R> matchUnsafe<T, R>(IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            ) =>
            list.Match(
                None,
                opt => matchUnsafe(opt, v => Some(v), None),
                (x, xs) => matchUnsafe(x, v => Some(v), None).Concat(matchUnsafe(xs, Some, None)) // TODO: Flatten recursion
            );

        /// <summary>
        /// Match the two states of the list of OptionUnsafes
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        [Pure]
        public static IEnumerable<R> matchUnsafe<T, R>(IEnumerable<OptionUnsafe<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None
            ) =>
            matchUnsafe(list, Some, () => None);

        /// <summary>
        /// Extracts from a list of 'OptionUnsafe' all the 'Some' elements.
        /// All the 'Some' elements are extracted in order.
        /// </summary>
        [Pure]
        public static IEnumerable<T> somes<T>(IEnumerable<OptionUnsafe<T>> list) =>
            list.Somes();

        /// <summary>
        /// Convert the OptionUnsafe to an immutable list of zero or one items
        /// </summary>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public static Lst<T> toList<T>(OptionUnsafe<T> option) =>
            option.ToList();

        /// <summary>
        /// Convert the OptionUnsafe to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static T[] toArray<T>(OptionUnsafe<T> option) =>
            option.ToArray();
    }
}
