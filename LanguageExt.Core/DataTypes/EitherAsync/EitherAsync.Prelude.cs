using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
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
        /// Monadic join
        /// </summary>
        [Pure]
        public static EitherAsync<L, R> flatten<L, R>(EitherAsync<L, EitherAsync<L, R>> ma) =>
            ma.Bind(identity);

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
        public static EitherAsync<L, R> plus<NUM, L, R>(EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Plus(a, b);

        /// <summary>
        /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
        /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option with the subtract between x and y</returns>
        [Pure]
        public static EitherAsync<L, R> subtract<NUM, L, R>(EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Subtract(a, b);

        /// <summary>
        /// Find the product between the two bound values of x and y, uses a Product type-class 
        /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option with the product of x and y</returns>
        [Pure]
        public static EitherAsync<L, R> product<NUM, L, R>(EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
            from a in x
            from b in y
            select default(NUM).Product(a, b);

        /// <summary>
        /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
        /// operation for type A.  For example x.Divide<TDouble,double>(y)
        /// </summary>
        /// <typeparam name="NUM">Num of A</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An option x / y</returns>
        [Pure]
        public static EitherAsync<L, R> divide<NUM, L, R>(EitherAsync<L, R> x, EitherAsync<L, R> y) where NUM : struct, Num<R> =>
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
        public static EitherAsync<L, B> apply<L, A, B>(EitherAsync<L, Func<A, B>> fab, EitherAsync<L, A> fa) =>
            ApplEitherAsync<L, A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        public static EitherAsync<L, B> apply<L, A, B>(Func<A, B> fab, EitherAsync<L, A> fa) =>
            ApplEitherAsync<L, A, B>.Inst.Apply(fab, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static EitherAsync<L, C> apply<L, A, B, C>(EitherAsync<L, Func<A, B, C>> fabc, EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
            from x in fabc
            from y in ApplEitherAsync<L, A, B, C>.Inst.Apply(curry(x), fa, fb)
            select y;

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        public static EitherAsync<L, C> apply<L, A, B, C>(Func<A, B, C> fabc, EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
            ApplEitherAsync<L, A, B, C>.Inst.Apply(curry(fabc), fa, fb);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherAsync<L, Func<B, C>> apply<L, A, B, C>(EitherAsync<L, Func<A, B, C>> fabc, EitherAsync<L, A> fa) =>
            from x in fabc
            from y in ApplEitherAsync<L, A, B, C>.Inst.Apply(curry(x), fa)
            select y;

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherAsync<L, Func<B, C>> apply<L, A, B, C>(Func<A, B, C> fabc, EitherAsync<L, A> fa) =>
            ApplEitherAsync<L, A, B, C>.Inst.Apply(curry(fabc), fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherAsync<L, Func<B, C>> apply<L, A, B, C>(EitherAsync<L, Func<A, Func<B, C>>> fabc, EitherAsync<L, A> fa) =>
            ApplEitherAsync<L, A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        public static EitherAsync<L, Func<B, C>> apply<L, A, B, C>(Func<A, Func<B, C>> fabc, EitherAsync<L, A> fa) =>
            ApplEitherAsync<L, A, B, C>.Inst.Apply(fabc, fa);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type Option<B></returns>
        [Pure]
        public static EitherAsync<L, B> action<L, A, B>(EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
            ApplEitherAsync<L, A, B>.Inst.Action(fa, fb);

        /// <summary>
        /// Returns the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Either is in a Right state</returns>
        [Pure]
        public static Task<bool> isRight<L, R>(EitherAsync<L, R> value) =>
            value.IsRight;

        /// <summary>
        /// Returns the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Either is in a Left state</returns>
        [Pure]
        public static Task<bool> isLeft<L, R>(EitherAsync<L, R> value) =>
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
        public static EitherAsync<L, R> RightAsync<L, R>(Task<R> value) =>
            EitherAsync<L, R>.RightAsync(value);

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Right value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static EitherAsync<L, R> RightAsync<L, R>(Func<Unit, Task<R>> value) =>
            EitherAsync<L, R>.RightAsync(value(unit));

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Right value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static EitherAsync<L, R> RightAsync<L, R>(R value) =>
            EitherAsync<L, R>.Right(value);

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Left value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static EitherAsync<L, R> LeftAsync<L, R>(Task<L> value) =>
            EitherAsync<L, R>.LeftAsync(value);

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Left value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static EitherAsync<L, R> LeftAsync<L, R>(Func<Unit, Task<L>> value) =>
            EitherAsync<L, R>.LeftAsync(value(unit));

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Left value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static EitherAsync<L, R> LeftAsync<L, R>(L value) =>
            EitherAsync<L, R>.Left(value);

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Task<R> ifLeft<L, R>(EitherAsync<L, R> either, Func<R> LeftSync) =>
           either.IfLeft(LeftSync);

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Task<R> ifLeft<L, R>(EitherAsync<L, R> either, Func<L, R> LeftSync) =>
           either.IfLeft(LeftSync);

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Task<R> ifLeftAsync<L, R>(EitherAsync<L, R> either, Func<L, Task<R>> LeftAsync) =>
           either.IfLeftAsync(LeftAsync);

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="rightValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Task<R> ifLeft<L, R>(EitherAsync<L, R> either, R Right) =>
           either.IfLeft(Right);

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Task<Unit> ifLeft<L, R>(EitherAsync<L, R> either, Action<L> Left) =>
           either.IfLeft(Left);

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Task<Unit> ifLeftAsync<L, R>(EitherAsync<L, R> either, Func<L, Task> Left) =>
           either.IfLeftAsync(Left);

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        [Pure]
        public static Task<Unit> ifRight<L, R>(EitherAsync<L, R> either, Action<R> Right) =>
           either.IfRight(Right);

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        [Pure]
        public static Task<Unit> ifRightAsync<L, R>(EitherAsync<L, R> either, Func<R, Task> RightAsync) =>
           either.IfRightAsync(RightAsync);

        /// <summary>
        /// Returns the `Left` argument if the `Either` is in a `Right` state.
        /// Returns the `Left` value from the Either if it's in a `Left` state.
        /// </summary>
        /// <param name="Left">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<L> ifRight<L, R>(EitherAsync<L, R> either, L Left) =>
           either.IfRight(Left);

        /// <summary>
        /// Returns the `Left` argument if the `Either` is in a `Right` state.
        /// Returns the `Left` value from the Either if it's in a `Left` state.
        /// </summary>
        /// <param name="Left">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<L> ifRightAsync<L, R>(EitherAsync<L, R> either, Func<Task<L>> RightAsync) =>
           either.IfRightAsync(RightAsync: RightAsync);

        /// <summary>
        /// Returns the result of Left() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<L> ifRight<L, R>(EitherAsync<L, R> either, Func<L> Left) =>
           either.IfRight(Left);

        /// <summary>
        /// Returns the result of Left if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<L> ifRight<L, R>(EitherAsync<L, R> either, Func<R, L> Left) =>
           either.IfRight(Left);

        /// <summary>
        /// Returns the result of Left if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<L> ifRightAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<L>> LeftAsync) =>
           either.IfRightAsync(LeftAsync);

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
        public static Task<Ret> match<L, R, Ret>(EitherAsync<L, R> either, Func<R, Ret> Right, Func<L, Ret> Left, Func<Ret> Bottom = null) =>
            either.Match(Right, Left, Bottom);

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
        public static Task<Ret> matchAsync<L, R, Ret>(EitherAsync<L, R> either, Func<R, Task<Ret>> RightAsync, Func<L, Ret> Left, Func<Ret> Bottom = null) =>
            either.MatchAsync(RightAsync, Left, Bottom);

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
        public static Task<Ret> matchAsync<L, R, Ret>(EitherAsync<L, R> either, Func<R, Ret> Right, Func<L, Task<Ret>> LeftAsync, Func<Ret> Bottom = null) =>
            either.MatchAsync(Right, LeftAsync, Bottom);

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
        public static Task<Ret> matchAsync<L, R, Ret>(EitherAsync<L, R> either, Func<R, Task<Ret>> RightAsync, Func<L, Task<Ret>> LeftAsync, Func<Ret> Bottom = null) =>
            either.MatchAsync(RightAsync, LeftAsync, Bottom);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to match</param>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public static Task<Unit> match<L, R>(EitherAsync<L, R> either, Action<R> Right, Action<L> Left, Action Bottom = null) =>
            either.Match(Right, Left, Bottom);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to match</param>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public static Task<Unit> matchAsync<L, R>(EitherAsync<L, R> either, Func<R, Task> RightAsync, Action<L> Left, Action Bottom = null) =>
            either.MatchAsync(RightAsync, Left, Bottom);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to match</param>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public static Task<Unit> matchAsync<L, R>(EitherAsync<L, R> either, Action<R> Right, Func<L, Task> LeftAsync, Action Bottom = null) =>
            either.MatchAsync(Right, LeftAsync, Bottom);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to match</param>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public static Task<Unit> matchAsync<L, R>(EitherAsync<L, R> either, Func<R, Task> RightAsync, Func<L, Task> LeftAsync, Action Bottom = null) =>
            either.MatchAsync(RightAsync, LeftAsync, Bottom);

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
        /// <param name="Right">Folder function, applied if structure is in a Right state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> fold<S, L, R>(EitherAsync<L, R> either, S state, Func<S, R, S> Right) =>
            either.Fold(state, Right);

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
        /// <param name="RightAsync">Folder function, applied if structure is in a Right state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static Task<S> foldAsync<S, L, R>(EitherAsync<L, R> either, S state, Func<S, R, Task<S>> RightAsync) =>
            either.FoldAsync(state, RightAsync);

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
        public static Task<S> bifold<L, R, S>(EitherAsync<L, R> either, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            either.BiFold(state, Right, Left);

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
        public static Task<S> bifoldAsync<L, R, S>(EitherAsync<L, R> either, S state, Func<S, R, Task<S>> RightAsync, Func<S, L, S> Left) =>
            either.BiFoldAsync(state, RightAsync, Left);

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
        public static Task<S> bifoldAsync<L, R, S>(EitherAsync<L, R> either, S state, Func<S, R, S> Right, Func<S, L, Task<S>> LeftAsync) =>
            either.BiFoldAsync(state, Right, LeftAsync);

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
        public static Task<S> bifoldAsync<L, R, S>(EitherAsync<L, R> either, S state, Func<S, R, Task<S>> RightAsync, Func<S, L, Task<S>> LeftAsync) =>
            either.BiFoldAsync(state, RightAsync, LeftAsync);

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
        public static Task<bool> forall<L, R>(EitherAsync<L, R> either, Func<R, bool> pred) =>
            either.ForAll(pred);

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
        public static Task<bool> forallAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<bool>> pred) =>
            either.ForAllAsync(pred);

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
        public static Task<bool> biforall<L, R>(EitherAsync<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.BiForAll(Right, Left);

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
        public static Task<bool> biforallAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<bool>> RightAsync, Func<L, bool> Left) =>
            either.BiForAllAsync(RightAsync, Left);

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
        public static Task<bool> biforallAsync<L, R>(EitherAsync<L, R> either, Func<R, bool> Right, Func<L, Task<bool>> LeftAsync) =>
            either.BiForAllAsync(Right, LeftAsync);

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
        public static Task<bool> biforallAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<bool>> RightAsync, Func<L, Task<bool>> LeftAsync) =>
            either.BiForAllAsync(RightAsync, LeftAsync);

        /// <summary>
        /// Counts the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to count</param>
        /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
        [Pure]
        public static Task<int> count<L, R>(EitherAsync<L, R> either) =>
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
        public static Task<bool> exists<L, R>(EitherAsync<L, R> either, Func<R, bool> pred) =>
            either.Exists(pred);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to check existence of</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
        [Pure]
        public static Task<bool> existsAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<bool>> pred) =>
            either.ExistsAsync(pred);

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
        public static Task<bool> biexists<L, R>(EitherAsync<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.BiExists(Right,Left);

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
        public static Task<bool> biexistsAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<bool>> RightAsync, Func<L, bool> Left) =>
            either.BiExistsAsync(RightAsync, Left);

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
        public static Task<bool> biexistsAsync<L, R>(EitherAsync<L, R> either, Func<R, bool> Right, Func<L, Task<bool>> LeftAsync) =>
            either.BiExistsAsync(Right, LeftAsync);

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
        public static Task<bool> biexistsAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<bool>> RightAsync, Func<L, Task<bool>> LeftAsync) =>
            either.BiExistsAsync(RightAsync, LeftAsync);

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="either">Either to map</param>
        /// <param name="f">Map function</param>
        /// <returns>Mapped EitherAsync</returns>
        [Pure]
        public static EitherAsync<L, Ret> map<L, R, Ret>(EitherAsync<L, R> either, Func<R, Ret> f) =>
            either.Map(f);

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="either">Either to map</param>
        /// <param name="f">Map function</param>
        /// <returns>Mapped EitherAsync</returns>
        [Pure]
        public static EitherAsync<L, Ret> mapAsync<L, R, Ret>(EitherAsync<L, R> either, Func<R, Task<Ret>> f) =>
            either.MapAsync(f);

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
        /// <returns>Mapped EitherAsync</returns>
        [Pure]
        public static EitherAsync<LRet, RRet> bimap<L, R, LRet, RRet>(EitherAsync<L, R> either, Func<R, RRet> Right, Func<L, LRet> Left) =>
            either.BiMap(Right, Left);

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
        /// <returns>Mapped EitherAsync</returns>
        [Pure]
        public static EitherAsync<LRet, RRet> bimapAsync<L, R, LRet, RRet>(EitherAsync<L, R> either, Func<R, Task<RRet>> RightAsync, Func<L, LRet> Left) =>
            either.BiMapAsync(RightAsync, Left);

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
        /// <returns>Mapped EitherAsync</returns>
        [Pure]
        public static EitherAsync<LRet, RRet> bimapAsync<L, R, LRet, RRet>(EitherAsync<L, R> either, Func<R, RRet> Right, Func<L, Task<LRet>> LeftAsync) =>
            either.BiMapAsync(Right, LeftAsync);

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
        /// <returns>Mapped EitherAsync</returns>
        [Pure]
        public static EitherAsync<LRet, RRet> bimapAsync<L, R, LRet, RRet>(EitherAsync<L, R> either, Func<R, Task<RRet>> RightAsync, Func<L, Task<LRet>> LeftAsync) =>
            either.BiMapAsync(RightAsync, LeftAsync);

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public static EitherAsync<L, Func<T2, R>> parmap<L, T1, T2, R>(EitherAsync<L, T1> either, Func<T1, T2, R> func) =>
            either.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public static EitherAsync<L, Func<T2, Func<T3, R>>> parmap<L, T1, T2, T3, R>(EitherAsync<L, T1> either, Func<T1, T2, T3, R> func) =>
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
        public static EitherAsync<L, R> filter<L, R>(EitherAsync<L, R> either, Func<R, bool> pred) =>
            either.Filter(pred);

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
        public static EitherAsync<L, R> filterAsync<L, R>(EitherAsync<L, R> either, Func<R, Task<bool>> pred) =>
            either.FilterAsync(pred);

        /// <summary>
        /// Monadic bind function
        /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret"></typeparam>
        /// <param name="either"></param>
        /// <param name="binder"></param>
        /// <returns>Bound EitherAsync</returns>
        [Pure]
        public static EitherAsync<L, Ret> bind<L, R, Ret>(EitherAsync<L, R> either, Func<R, EitherAsync<L, Ret>> binder) =>
            either.Bind(binder);

        /// <summary>
        /// Match over a sequence of EitherAsyncs
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped type</typeparam>
        /// <param name="list">Sequence to match over</param>
        /// <param name="Right">Right match function</param>
        /// <param name="Left">Left match function</param>
        /// <returns>Sequence of mapped values</returns>
        [Pure]
        public async static Task<IEnumerable<Ret>> match<L, R, Ret>(IEnumerable<EitherAsync<L, R>> list,
            Func<R, Ret> Right,
            Func<L, Ret> Left) =>
            (await Task.WhenAll(list.Map(item => item.Match(Right, Left))).ConfigureAwait(false)).AsEnumerable();

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Task<Lst<R>> rightToList<L, R>(EitherAsync<L, R> either) =>
            either.RightToList();

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static Task<Arr<R>> rightToArray<L, R>(EitherAsync<L, R> either) =>
            either.RightToArray();

        /// <summary>
        /// Project the Either into a Lst L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Left state, a Lst of L with one item.  A zero length Lst L otherwise</returns>
        [Pure]
        public static Task<Lst<L>> leftToList<L, R>(EitherAsync<L, R> either) =>
            either.LeftToList();

        /// <summary>
        /// Project the Either into an array of L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an array of L with one item.  A zero length array of L otherwise</returns>
        [Pure]
        public static Task<Arr<L>> leftToArray<L, R>(EitherAsync<L, R> either) =>
            either.LeftToArray();

        /// <summary>
        /// Project the Either into an IQueryable of R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an IQueryable of R with one item.  A zero length IQueryable R otherwise</returns>
        [Pure]
        public static Task<IQueryable<R>> rightToQuery<L, R>(EitherAsync<L, R> either) =>
            either.RightAsEnumerable()
                  .Map(x => x.AsEnumerable())
                  .Map(Queryable.AsQueryable);

        /// <summary>
        /// Project the Either into an IQueryable of L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Left state, an IQueryable of L with one item.  A zero length IQueryable L otherwise</returns>
        [Pure]
        public static Task<IQueryable<L>> leftToQuery<L, R>(EitherAsync<L, R> either) =>
            either.LeftAsEnumerable()
                  .Map(x => x.AsEnumerable())
                  .Map(Queryable.AsQueryable);

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Left' elements.
        /// All the 'Left' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static Task<IEnumerable<L>> lefts<L, R>(IEnumerable<EitherAsync<L, R>> self) =>
            leftsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Left' elements.
        /// All the 'Left' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static Task<Seq<L>> lefts<L, R>(Seq<EitherAsync<L, R>> self) =>
            leftsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Right' elements.
        /// All the 'Right' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static Task<IEnumerable<R>> rights<L, R>(IEnumerable<EitherAsync<L, R>> self) =>
            rightsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Right' elements.
        /// All the 'Right' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static Task<Seq<R>> rights<L, R>(Seq<EitherAsync<L, R>> self) =>
            rightsAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

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
        public static Task<(IEnumerable<L> Lefts, IEnumerable<R> Rights)> partition<L, R>(IEnumerable<EitherAsync<L, R>> self) =>
            partitionAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);

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
        public static Task<(Seq<L> Lefts, Seq<R> Rights)> partition<L, R>(Seq<EitherAsync<L, R>> self) =>
            partitionAsync<MEitherAsync<L, R>, EitherAsync<L, R>, L, R>(self);
    }
}
