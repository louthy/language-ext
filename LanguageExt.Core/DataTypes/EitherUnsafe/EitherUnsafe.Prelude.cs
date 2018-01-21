﻿using System;
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
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Add the bound values of x and y, uses an Add type-class to provide the add
        /// operation for type A.  For example x.Add<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option with y added to x</returns>
        [Pure]
        public static EitherUnsafe<L, R> plus<NUM, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Plus(a, b);

        /// <summary>
        /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
        /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="DIFF">Subtract of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option with the subtract between x and y</returns>
        [Pure]
        public static EitherUnsafe<L, R> subtract<NUM, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Subtract(a, b);

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
        public static EitherUnsafe<L, R> product<NUM, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Product(a, b);

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
        public static EitherUnsafe<L, R> divide<NUM, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Divide(a, b);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static EitherUnsafe<L, B> apply<L, A, B>(EitherUnsafe<L, Func<A, B>> fab, EitherUnsafe<L, A> fa) =>
            ApplEitherUnsafe<L, A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static EitherUnsafe<L, B> apply<L, A, B>(Func<A, B> fab, EitherUnsafe<L, A> fa) =>
            ApplEitherUnsafe<L, A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static EitherUnsafe<L, C> apply<L, A, B, C>(EitherUnsafe<L, Func<A, B, C>> fabc, EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
            from x in fabc
            from y in ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(x), fa, fb)
            select y;

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static EitherUnsafe<L, C> apply<L, A, B, C>(Func<A, B, C> fabc, EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
            ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(fabc), fa, fb);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherUnsafe<L, Func<B, C>> apply<L, A, B, C>(EitherUnsafe<L, Func<A, B, C>> fabc, EitherUnsafe<L, A> fa) =>
            from x in fabc
            from y in ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(x), fa)
            select y;

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherUnsafe<L, Func<B, C>> apply<L, A, B, C>(Func<A, B, C> fabc, EitherUnsafe<L, A> fa) =>
            ApplEitherUnsafe<L, A, B, C>.Inst.Apply(curry(fabc), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherUnsafe<L, Func<B, C>> apply<L, A, B, C>(EitherUnsafe<L, Func<A, Func<B, C>>> fabc, EitherUnsafe<L, A> fa) =>
            ApplEitherUnsafe<L, A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherUnsafe<L, Func<B, C>> apply<L, A, B, C>(Func<A, Func<B, C>> fabc, EitherUnsafe<L, A> fa) =>
            ApplEitherUnsafe<L, A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type Option<B></returns>
        [Pure]
        public static EitherUnsafe<L, B> action<L, A, B>(EitherUnsafe<L, A> fa, EitherUnsafe<L, B> fb) =>
            ApplEitherUnsafe<L, A, B>.Inst.Action(fa, fb);

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
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if structure is in a Right state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static S fold<S, L, R>(EitherUnsafe<L, R> either, S state, Func<S, R, S> folder) =>
            either.Fold(state, folder);

        /// <summary>
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Right">Folder function, applied if Either is in a Right state</param>
        /// <param name="Left">Folder function, applied if Either is in a Left state</param>
        /// <returns>The aggregate state</returns>
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
            either.BiExists(Right, Left);

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="either">Either to map</param>
        /// <param name="mapper">Map function</param>
        /// <returns>Mapped EitherUnsafe</returns>
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
        /// <returns>Mapped EitherUnsafe</returns>
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
        /// Monadic bind function
        /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret"></typeparam>
        /// <param name="either"></param>
        /// <param name="binder"></param>
        /// <returns>Bound EitherUnsafe</returns>
        [Pure]
        public static EitherUnsafe<L, Ret> bind<L, R, Ret>(EitherUnsafe<L, R> either, Func<R, EitherUnsafe<L, Ret>> binder) =>
            either.Bind(binder);

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
            match(list, Right, Left);

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
        public static IEnumerable<Ret> match<L, R, Ret>(IEnumerable<EitherUnsafe<L, R>> list,
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
        public static Arr<R> rightToArray<L, R>(EitherUnsafe<L, R> either) =>
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
        public static Arr<L> leftToArray<L, R>(EitherUnsafe<L, R> either) =>
            either.LeftToArray();

        /// <summary>
        /// Project the Either into an IQueryable of R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an IQueryable of R with one item.  A zero length IQueryable R otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("ToQuery has been deprecated.  Please use RightToQuery.")]
        public static IQueryable<R> toQuery<L, R>(EitherUnsafe<L, R> either) =>
            either.RightAsEnumerable().AsQueryable();

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
            lefts<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Left' elements.
        /// All the 'Left' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static Seq<L> lefts<L, R>(Seq<EitherUnsafe<L, R>> self) =>
            lefts<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

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
            rights<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Right' elements.
        /// All the 'Right' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static Seq<R> rights<L, R>(Seq<EitherUnsafe<L, R>> self) =>
            rights<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

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
        public static (IEnumerable<L> Lefts, IEnumerable<R> Rights) partition<L, R>(IEnumerable<EitherUnsafe<L, R>> self) =>
            partition<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);

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
        public static (Seq<L> Lefts, Seq<R> Rights) partition<L, R>(Seq<EitherUnsafe<L, R>> self) =>
            partition<MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>(self);
    }
}
