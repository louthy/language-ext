using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append the Right(x) of one option to the Right(y) of another.  If either of the
        /// options are Left then the result is Left
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static EitherUnsafe<L, R> append<SEMI, L, R>(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) where SEMI : struct, Semigroup<R> =>
            from x in lhs
            from y in rhs
            select TypeClass.append<SEMI, R>(x, y);

        /// <summary>
        /// Add the bound values of x and y, uses an Add type-class to provide the add
        /// operation for type A.  For example x.Add<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="ADD">Add of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option with y added to x</returns>
        [Pure]
        public static EitherUnsafe<L, R> add<ADD, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where ADD : struct, Addition<R> =>
            from a in x
            from b in y
            select TypeClass.add<ADD, R>(a, b);

        /// <summary>
        /// Find the difference between the two bound values of x and y, uses a Difference type-class 
        /// to provide the difference operation for type A.  For example x.Difference<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="DIFF">Difference of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option with the difference between x and y</returns>
        [Pure]
        public static EitherUnsafe<L, R> difference<DIFF, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where DIFF : struct, Difference<R> =>
            from a in x
            from b in y
            select TypeClass.difference<DIFF, R>(a, b);

        /// <summary>
        /// Find the product between the two bound values of x and y, uses a Product type-class 
        /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="PROD">Product of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option with the product of x and y</returns>
        [Pure]
        public static EitherUnsafe<L, R> product<PROD, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where PROD : struct, Product<R> =>
            from a in x
            from b in y
            select TypeClass.product<PROD, R>(a, b);

        /// <summary>
        /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
        /// operation for type A.  For example x.Divide<TDouble,double>(y)
        /// </summary>
        /// <typeparam name="DIV">Divide of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option x / y</returns>
        [Pure]
        public static EitherUnsafe<L, R> divide<DIV, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where DIV : struct, Divisible<R> =>
            from a in x
            from b in y
            select TypeClass.divide<DIV, R>(a, b);

        /// Apply y to x
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, B> apply<L, A, B>(EitherUnsafe<L, Func<A, B>> x, EitherUnsafe<L, A> y) =>
            x.Apply<EitherUnsafe<L, B>, A, B>(y);

        /// <summary>
        /// Apply y and z to x
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, C> apply<L, A, B, C>(EitherUnsafe<L, Func<A, B, C>> x, EitherUnsafe<L, A> y, EitherUnsafe<L, B> z) =>
            x.Apply<EitherUnsafe<L, C>, A, B, C>(y, z);

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, Func<B, C>> apply<L, A, B, C>(EitherUnsafe<L, Func<A, Func<B, C>>> x, EitherUnsafe<L, A> y) =>
            x.Apply<EitherUnsafe<L, Func<B, C>>, A, B, C>(y);

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, Func<B, C>> apply<L, A, B, C>(EitherUnsafe<L, Func<A, B, C>> x, EitherUnsafe<L, A> y) =>
            x.Apply<EitherUnsafe<L, Func<B, C>>, A, B, C>(y);

        /// <summary>
        /// Apply x, then y, ignoring the result of x
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, B> action<L, A, B>(EitherUnsafe<L, A> x, EitherUnsafe<L, B> y) =>
            x.Action<EitherUnsafe<L, B>, A, B>(y);


        /// <summary>
        /// Returns the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Either is in a Right state</returns>
        [Pure]
        public static bool isRight<L, R>(EitherUnsafe<L, R> value) =>
            value.IsRight;

        /// <summary>
        /// Returns the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Either is in a Left state</returns>
        [Pure]
        public static bool isLeft<L, R>(EitherUnsafe<L, R> value) =>
            value.IsLeft;

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Right value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static EitherUnsafe<L, R> RightUnsafe<L, R>(R value) =>
            EitherUnsafe<L, R>.Right(value);

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Left value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static EitherUnsafe<L, R> LeftUnsafe<L, R>(L value) =>
            EitherUnsafe<L, R>.Left(value);

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static R ifLeftUnsafe<L, R>(EitherUnsafe<L, R> either, Func<R> Left) =>
           either.IfLeftUnsafe(Left);

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static R ifLeftUnsafe<L, R>(EitherUnsafe<L, R> either, Func<L, R> leftMap) =>
           either.IfLeftUnsafe(leftMap);

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="rightValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static R ifLeftUnsafe<L, R>(EitherUnsafe<L, R> either, R rightValue) =>
           either.IfLeftUnsafe(rightValue);

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Unit ifLeftUnsafe<L, R>(EitherUnsafe<L, R> either, Action<L> Left) =>
           either.IfLeftUnsafe(Left);

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        [Pure]
        public static Unit ifRightUnsafe<L, R>(EitherUnsafe<L, R> either, Action<R> Right) =>
           either.IfRightUnsafe(Right);

        /// <summary>
        /// Returns the leftValue if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="leftValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static L ifRightUnsafe<L, R>(EitherUnsafe<L, R> either, L leftValue) =>
           either.IfRightUnsafe(leftValue);

        /// <summary>
        /// Returns the result of Left() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static L ifRightUnsafe<L, R>(EitherUnsafe<L, R> either, Func<L> Left) =>
           either.IfRightUnsafe(Left);

        /// <summary>
        /// Returns the result of leftMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static L ifRightUnsafe<L, R>(EitherUnsafe<L, R> either, Func<R, L> leftMap) =>
           either.IfRightUnsafe(leftMap);

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="either">Either to match</param>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public static Ret matchUnsafe<L, R, Ret>(EitherUnsafe<L, R> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.MatchUnsafe(Right, Left);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to match</param>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public static Unit matchUnsafe<L, R>(EitherUnsafe<L, R> either, Action<R> Right, Action<L> Left) =>
            either.MatchUnsafe(Right, Left);

        /// <summary>
        /// Folds the either into an S
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <typeparam name="S">State</typeparam>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, L, R>(EitherUnsafe<L, R> either, S state, Func<S, R, S> folder) =>
            either.Fold(state, folder);

        /// <summary>
        /// Folds the either into an S
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <typeparam name="S">State</typeparam>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Right">Right fold function</param>
        /// <param name="Left">Left fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S bifold<L, R, S>(EitherUnsafe<L, R> either, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            either.BiFold(state, Right, Left);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to forall</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Either is in a Left state.  
        /// True if the Either is in a Right state and the predicate returns True.  
        /// False otherwise.</returns>
        [Pure]
        public static bool forall<L, R>(EitherUnsafe<L, R> either, Func<R, bool> pred) =>
            either.ForAll(pred);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="Right">Right predicate</param>
        /// <param name="Left">Left predicate</param>
        /// <returns>True if the predicate returns True.  True if the Either is in a bottom state.</returns>
        [Pure]
        public static bool biforall<L, R>(EitherUnsafe<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.BiForAll(Right, Left);

        /// <summary>
        /// Counts the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to count</param>
        /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
        [Pure]
        public static int count<L, R>(EitherUnsafe<L, R> either) =>
            either.Count();

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to check existence of</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
        [Pure]
        public static bool exists<L, R>(EitherUnsafe<L, R> either, Func<R, bool> pred) =>
            either.Exists(pred);

        /// <summary>
        /// Invokes a predicate on the value of the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to check existence of</param>
        /// <param name="Right">Right predicate</param>
        /// <param name="Left">Left predicate</param>
        /// <returns>True if the predicate returns True.  False otherwise or if the Either is in a bottom state.</returns>
        [Pure]
        public static bool biexists<L, R>(EitherUnsafe<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.BiExists(Right,Left);

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="either">Either to map</param>
        /// <param name="mapper">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public static EitherUnsafe<L, Ret> map<L, R, Ret>(EitherUnsafe<L, R> either, Func<R, Ret> mapper) =>
            either.Map(mapper);

        /// <summary>
        /// Bi-maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="Right">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public static EitherUnsafe<LRet, RRet> bimap<L, R, LRet, RRet>(EitherUnsafe<L, R> either, Func<R, RRet> Right, Func<L, LRet> Left) =>
            either.BiMap(Right, Left);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static EitherUnsafe<L, Func<T2, R>> parmap<L, T1, T2, R>(EitherUnsafe<L, T1> either, Func<T1, T2, R> func) =>
            either.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static EitherUnsafe<L, Func<T2, Func<T3, R>>> parmap<L, T1, T2, T3, R>(EitherUnsafe<L, T1> either, Func<T1, T2, T3, R> func) =>
            either.ParMap(func);

        /// <summary>
        /// Filter the Either
        /// </summary>
        /// <remarks>
        /// This may give unpredictable results for a filtered value.  The Either won't
        /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
        /// should be checked for.
        /// </remarks>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to filter</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>If the Either is in the Left state it is returned as-is.  
        /// If in the Right state the predicate is applied to the Right value.
        /// If the predicate returns True the Either is returned as-is.
        /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
        [Pure]
        public static EitherUnsafe<L, R> filter<L, R>(EitherUnsafe<L, R> either, Func<R, bool> pred) =>
            either.Filter(pred);

        /// <summary>
        /// Bi-filter the Either
        /// </summary>
        /// <remarks>
        /// This may give unpredictable results for a filtered value.  The Either won't
        /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
        /// should be checked for.
        /// </remarks>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to filter</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>
        /// If the Either is in the Left state then the Left predicate is run against it.
        /// If the Either is in the Right state then the Right predicate is run against it.
        /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
        [Pure]
        public static EitherUnsafe<L, R> bifilter<L, R>(EitherUnsafe<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.BiFilter(Right, Left);

        /// <summary>
        /// Monadic bind function
        /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret"></typeparam>
        /// <param name="either"></param>
        /// <param name="binder"></param>
        /// <returns>Bound Either</returns>
        [Pure]
        public static EitherUnsafe<L, Ret> bind<L, R, Ret>(EitherUnsafe<L, R> either, Func<R, EitherUnsafe<L, Ret>> binder) =>
            either.Bind(binder);

        /// <summary>
        /// Monadic bind function
        /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret"></typeparam>
        /// <param name="self">this</param>
        /// <param name="Right">Right bind function</param>
        /// <param name="Left">Left bind function</param>
        /// <returns>Bound Either</returns>
        [Pure]
        public static EitherUnsafe<LRet, RRet> bibind<L, R, LRet, RRet>(EitherUnsafe<L, R> either, Func<R, EitherUnsafe<LRet, RRet>> Right, Func<L, EitherUnsafe<LRet, RRet>> Left) =>
            either.BiBind(Right, Left);

        /// <summary>
        /// Match over a sequence of Eithers
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped type</typeparam>
        /// <param name="list">Sequence to match over</param>
        /// <param name="Right">Right match function</param>
        /// <param name="Left">Left match function</param>
        /// <returns>Sequence of mapped values</returns>
        [Pure]
        public static IEnumerable<Ret> Match<L, R, Ret>(this IEnumerable<EitherUnsafe<L, R>> list,
            Func<R, Ret> Right,
            Func<L, Ret> Left
            ) =>
            matchUnsafe(list, Right, Left);

        /// <summary>
        /// Match over a sequence of Eithers
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped type</typeparam>
        /// <param name="list">Sequence to match over</param>
        /// <param name="Right">Right match function</param>
        /// <param name="Left">Left match function</param>
        /// <returns>Sequence of mapped values</returns>
        [Pure]
        public static IEnumerable<Ret> matchUnsafe<L, R, Ret>(IEnumerable<EitherUnsafe<L, R>> list,
            Func<R, Ret> Right,
            Func<L, Ret> Left
            )
        {
            foreach (var item in list)
            {
                if (item.IsBottom) continue;
                if (item.IsLeft) yield return Left(item.LeftValue);
                if (item.IsRight) yield return Right(item.RightValue);
            }
        }

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Lst<R> rightToList<L, R>(EitherUnsafe<L, R> either) =>
            either.RightToList();

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static R[] rightToArray<L, R>(EitherUnsafe<L, R> either) =>
            either.RightToArray();

        /// <summary>
        /// Project the Either into a Lst L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Left state, a Lst of L with one item.  A zero length Lst L otherwise</returns>
        [Pure]
        public static Lst<L> leftToList<L, R>(EitherUnsafe<L, R> either) =>
            either.LeftToList();

        /// <summary>
        /// Project the Either into an array of L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an array of L with one item.  A zero length array of L otherwise</returns>
        [Pure]
        public static L[] leftToArray<L, R>(EitherUnsafe<L, R> either) =>
            either.LeftToArray();

        /// <summary>
        /// Project the Either into an IQueryable of R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an IQueryable of R with one item.  A zero length IQueryable R otherwise</returns>
        [Pure]
        public static IQueryable<R> rightToQuery<L, R>(EitherUnsafe<L, R> either) =>
            either.RightAsEnumerable().AsQueryable();

        /// <summary>
        /// Project the Either into an IQueryable of L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Left state, an IQueryable of L with one item.  A zero length IQueryable L otherwise</returns>
        [Pure]
        public static IQueryable<L> leftToQuery<L, R>(EitherUnsafe<L, R> either) =>
            either.LeftAsEnumerable().AsQueryable();

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Left' elements.
        /// All the 'Left' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static IEnumerable<L> lefts<L, R>(IEnumerable<EitherUnsafe<L, R>> self) =>
            self.Lefts();

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Right' elements.
        /// All the 'Right' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static IEnumerable<R> rights<L, R>(IEnumerable<EitherUnsafe<L, R>> self) =>
            self.Rights();

        /// <summary>
        /// Partitions a list of 'Either' into two lists.
        /// All the 'Left' elements are extracted, in order, to the first
        /// component of the output.  Similarly the 'Right' elements are extracted
        /// to the second component of the output.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
        [Pure]
        public static Tuple<IEnumerable<L>, IEnumerable<R>> partition<L, R>(IEnumerable<EitherUnsafe<L, R>> self) =>
            Tuple(lefts(self), rights(self));

        public static Task<EitherUnsafe<L, R2>> mapAsync<L, R, R2>(EitherUnsafe<L, R> self, Func<R, Task<R2>> map) =>
            self.MapAsync(map);

        public static Task<EitherUnsafe<L, R2>> mapAsync<L, R, R2>(Task<EitherUnsafe<L, R>> self, Func<R, Task<R2>> map) =>
            self.MapAsync(map);

        public static Task<EitherUnsafe<L, R2>> mapAsync<L, R, R2>(Task<EitherUnsafe<L, R>> self, Func<R, R2> map) =>
            self.MapAsync(map);

        public static Task<EitherUnsafe<L, R2>> mapAsync<L, R, R2>(EitherUnsafe<L, Task<R>> self, Func<R, R2> map) =>
            self.MapAsync(map);

        public static Task<EitherUnsafe<L, R2>> mapAsync<L, R, R2>(EitherUnsafe<L, Task<R>> self, Func<R, Task<R2>> map) =>
            self.MapAsync(map);

        public static Task<EitherUnsafe<L, R2>> bindAsync<L, R, R2>(EitherUnsafe<L, R> self, Func<R, Task<EitherUnsafe<L, R2>>> bind) =>
            self.BindAsync(bind);

        public static Task<EitherUnsafe<L, R2>> bindAsync<L, R, R2>(Task<EitherUnsafe<L, R>> self, Func<R, Task<EitherUnsafe<L, R2>>> bind) =>
            self.BindAsync(bind);

        public static Task<EitherUnsafe<L, R2>> bindAsync<L, R, R2>(Task<EitherUnsafe<L, R>> self, Func<R, EitherUnsafe<L, R2>> bind) =>
            self.BindAsync(bind);

        public static Task<EitherUnsafe<L, R2>> bindAsync<L, R, R2>(EitherUnsafe<L, Task<R>> self, Func<R, EitherUnsafe<L, R2>> bind) =>
            self.BindAsync(bind);

        public static Task<EitherUnsafe<L, R2>> bindAsync<L, R, R2>(EitherUnsafe<L, Task<R>> self, Func<R, Task<EitherUnsafe<L, R2>>> bind) =>
            self.BindAsync(bind);

        public static Task<Unit> iterAsync<L, R>(Task<EitherUnsafe<L, R>> self, Action<R> action) =>
            self.IterAsync(action);

        public static Task<Unit> iterAsync<L, R>(this EitherUnsafe<L, Task<R>> self, Action<R> action) =>
            self.IterAsync(action);

        public static Task<int> countAsync<L, R>(Task<EitherUnsafe<L, R>> self) =>
            self.CountAsync();

        public static Task<int> sumAsync<L>(Task<EitherUnsafe<L, int>> self) =>
            self.SumAsync();

        public static Task<int> sumAsync<L>(EitherUnsafe<L, Task<int>> self) =>
            self.SumAsync();

        public static Task<S> foldAsync<L, R, S>(Task<EitherUnsafe<L, R>> self, S state, Func<S, R, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<S> foldAsync<L, R, S>(EitherUnsafe<L, Task<R>> self, S state, Func<S, R, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<bool> forallAsync<L, R>(Task<EitherUnsafe<L, R>> self, Func<R, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> forallAsync<L, R>(EitherUnsafe<L, Task<R>> self, Func<R, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> existsAsync<L, R>(Task<EitherUnsafe<L, R>> self, Func<R, bool> pred) =>
            self.ExistsAsync(pred);

        public static Task<bool> existsAsync<L, R>(EitherUnsafe<L, Task<R>> self, Func<R, bool> pred) =>
            self.ExistsAsync(pred);
    }
}
