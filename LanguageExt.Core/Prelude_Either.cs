using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append the Right of one either to the Right of another
        /// For numeric values the behaviour is to sum the Rights (lhs + rhs)
        /// For string values the behaviour is to concatenate the strings
        /// For Lst/Stck/Que values the behaviour is to concatenate the lists
        /// For Map or Set values the behaviour is to merge the sets
        /// Otherwise if the R type derives from IAppendable then the behaviour
        /// is to call lhs.Append(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static Either<L, R> append<L, R>(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Subtract the Right of one either from the Right of another
        /// For numeric values the behaviour is to find the difference between the Rights (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static Either<L, R> subtract<L, R>(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Find the product of the Rights 
        /// For numeric values the behaviour is to multiply the Rights (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public static Either<L, R> multiply<L, R>(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Multiply(rhs);

        /// <summary>
        /// Divide the Rights 
        /// For numeric values the behaviour is to divide the Rights (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static Either<L, R> divide<L, R>(Either<L, R> lhs, Either<L, R> rhs) =>
            lhs.Divide(rhs);

        /// <summary>
        /// Returns the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Either is in a Right state</returns>
        [Pure]
        public static bool isRight<L, R>(Either<L, R> value) =>
            value.IsRight;

        /// <summary>
        /// Returns the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Either to check</param>
        /// <returns>True if the Either is in a Left state</returns>
        [Pure]
        public static bool isLeft<L, R>(Either<L, R> value) =>
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
        public static Either<L, R> Right<L, R>(R value) =>
            Either<L, R>.Right(value);

        /// <summary>
        /// Either constructor
        /// Constructs an Either in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Left value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static Either<L, R> Left<L, R>(L value) =>
            Either<L, R>.Left(value);

        /// <summary>
        /// Either constructor from Nullable
        /// Constructs an Either in a Right state or throws a ValueIsNullException if the value is null
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Right value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static Either<L, R> Right<L, R>(R? value) where R : struct =>
            isnull(value)
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Right(value.Value);

        /// <summary>
        /// Either constructor from Nullable
        /// Constructs an Either in a Left state or throws a ValueIsNullException if the value is null
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="value">Left value</param>
        /// <returns>A new Either instance</returns>
        [Pure]
        public static Either<L, R> Left<L, R>(L? value) where L : struct =>
            isnull(value)
                ? raise<Either<L, R>>(new ValueIsNullException())
                : Either<L, R>.Left(value.Value);

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static R ifLeft<L, R>(Either<L, R> either, Func<R> Left) =>
           either.IfLeft(Left);

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static R ifLeft<L, R>(Either<L, R> either, Func<L, R> leftMap) =>
           either.IfLeft(leftMap);

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="rightValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static R ifLeft<L, R>(Either<L, R> either, R rightValue) =>
           either.IfLeft(rightValue);

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public static Unit ifLeft<L, R>(Either<L, R> either, Action<L> Left) =>
           either.IfLeft(Left);

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        [Pure]
        public static Unit ifRight<L, R>(Either<L, R> either, Action<R> Right) =>
           either.IfRight(Right);

        /// <summary>
        /// Returns the leftValue if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="leftValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static L ifRight<L, R>(Either<L, R> either, L leftValue) =>
           either.IfRight(leftValue);

        /// <summary>
        /// Returns the result of Left() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static L ifRight<L, R>(Either<L, R> either, Func<L> Left) =>
           either.IfRight(Left);

        /// <summary>
        /// Returns the result of leftMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="leftMap">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static L ifRight<L, R>(Either<L, R> either, Func<R, L> leftMap) =>
           either.IfRight(leftMap);

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
        public static Ret match<L, R, Ret>(Either<L, R> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.Match(Right, Left);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either provided
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to match</param>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public static Unit match<L, R>(Either<L, R> either, Action<R> Right, Action<L> Left) =>
            either.Match(Right, Left);

        /// <summary>
        /// Apply an Either value to an Either function
        /// </summary>
        /// <param name="either">Either function</param>
        /// <param name="arg">Either argument</param>
        /// <returns>Returns the result of applying the Either argument to the Either function</returns>
        [Pure]
        public static Either<L, Res> apply<L, R, Res>(Either<L, Func<R, Res>> either, Either<L, R> arg) =>
            either.Apply(arg);

        /// <summary>
        /// Apply an Either value to an Either function of arity 2
        /// </summary>
        /// <param name="self">Either function</param>
        /// <param name="arg">Either argument</param>
        /// <returns>Returns the result of applying the Either argument to the Either function:
        /// an Either function of arity 1</returns>
        [Pure]
        public static Either<L, Func<T2, R>> apply<L, T1, T2, R>(Either<L, Func<T1, T2, R>> either, Either<L, T1> arg) =>
            either.Apply(arg);

        /// <summary>
        /// Apply Either values to an Either function of arity 2
        /// </summary>
        /// <param name="self">Either function</param>
        /// <param name="arg1">Either argument</param>
        /// <param name="arg2">Either argument</param>
        /// <returns>Returns the result of applying the optional arguments to the optional function</returns>
        [Pure]
        public static Either<L, R> apply<L, T1, T2, R>(Either<L, Func<T1, T2, R>> either, Either<L, T1> arg1, Either<L, T2> arg2) =>
            either.Apply(arg1, arg2);

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
        public static S fold<S, L, R>(Either<L, R> either, S state, Func<S, R, S> folder) =>
            either.Fold(state, folder);

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
        public static S fold<S, L, R>(Either<L, R> either, S state, Func<S, L, S> folder) =>
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
        public static S fold<L, R, S>(Either<L, R> either, S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            either.Fold(state, Right, Left);

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
        public static bool forall<L, R>(Either<L, R> either, Func<R, bool> pred) =>
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
        public static bool forall<L, R>(Either<L, R> either, Func<L, bool> pred) =>
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
        public static bool forall<L, R>(Either<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.ForAll(Right, Left);

        /// <summary>
        /// Counts the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to count</param>
        /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
        [Pure]
        public static int count<L, R>(Either<L, R> either) =>
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
        public static bool exists<L, R>(Either<L, R> either, Func<R, bool> pred) =>
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
        public static bool exists<L, R>(Either<L, R> either, Func<L, bool> pred) =>
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
        public static bool exists<L, R>(Either<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.Exists(Right,Left);

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
        public static Either<L, Ret> map<L, R, Ret>(Either<L, R> either, Func<R, Ret> mapper) =>
            either.Map(mapper);

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
        public static Either<Ret, R> mapLeft<L, R, Ret>(Either<L, R> either, Func<L, Ret> mapper) =>
            either.MapLeft(mapper);

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
        public static Either<LRet, RRet> bimap<L, R, LRet, RRet>(Either<L, R> either, Func<R, RRet> Right, Func<L, LRet> Left) =>
            either.BiMap(Right, Left);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Either<L, Func<T2, R>> parmap<L, T1, T2, R>(Either<L, T1> either, Func<T1, T2, R> func) =>
            either.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Either<L, Func<T2, Func<T3, R>>> parmap<L, T1, T2, T3, R>(Either<L, T1> either, Func<T1, T2, T3, R> func) =>
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
        public static Either<L, R> filter<L, R>(Either<L, R> either, Func<R, bool> pred) =>
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
        /// <returns>If the Either is in the Right state it is returned as-is.  
        /// If in the Left state the predicate is applied to the Left value.
        /// If the predicate returns True the Either is returned as-is.
        /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
        [Pure]
        public static Either<L, R> filter<L, R>(Either<L, R> either, Func<L, bool> pred) =>
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
        public static Either<L, R> filter<L, R>(Either<L, R> either, Func<R, bool> Right, Func<L, bool> Left) =>
            either.Filter(Right, Left);

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
        public static Either<L, Ret> bind<L, R, Ret>(Either<L, R> either, Func<R, Either<L, Ret>> binder) =>
            either.Bind(binder);

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
        public static Either<Ret, R> bind<L, R, Ret>(Either<L, R> either, Func<L, Either<Ret, R>> binder) =>
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
        public static Either<LRet, RRet> bind<L, R, LRet, RRet>(Either<L, R> either, Func<R, Either<LRet, RRet>> Right, Func<L, Either<LRet, RRet>> Left) =>
            either.Bind(Right, Left);

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
        public static IEnumerable<Ret> Match<L, R, Ret>(this IEnumerable<Either<L, R>> list,
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
        public static IEnumerable<Ret> match<L, R, Ret>(IEnumerable<Either<L, R>> list,
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
        public static Lst<R> rightToList<L, R>(Either<L, R> either) =>
            either.RightToList();

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static R[] rightToArray<L, R>(Either<L, R> either) =>
            either.RightToArray();

        /// <summary>
        /// Project the Either into a Lst L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Left state, a Lst of L with one item.  A zero length Lst L otherwise</returns>
        [Pure]
        public static Lst<L> leftToList<L, R>(Either<L, R> either) =>
            either.LeftToList();

        /// <summary>
        /// Project the Either into an array of L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an array of L with one item.  A zero length array of L otherwise</returns>
        [Pure]
        public static L[] leftToArray<L, R>(Either<L, R> either) =>
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
        public static IQueryable<R> toQuery<L, R>(Either<L, R> either) =>
            either.AsEnumerable().AsQueryable();

        /// <summary>
        /// Project the Either into an IQueryable of R
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Right state, an IQueryable of R with one item.  A zero length IQueryable R otherwise</returns>
        [Pure]
        public static IQueryable<R> rightToQuery<L, R>(Either<L, R> either) =>
            either.RightAsEnumerable().AsQueryable();

        /// <summary>
        /// Project the Either into an IQueryable of L
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="either">Either to project</param>
        /// <returns>If the Either is in a Left state, an IQueryable of L with one item.  A zero length IQueryable L otherwise</returns>
        [Pure]
        public static IQueryable<L> leftToQuery<L, R>(Either<L, R> either) =>
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
        public static IEnumerable<L> lefts<L, R>(IEnumerable<Either<L, R>> self) =>
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
        public static IEnumerable<R> rights<L, R>(IEnumerable<Either<L, R>> self) =>
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
        public static Tuple<IEnumerable<L>, IEnumerable<R>> partition<L, R>(IEnumerable<Either<L, R>> self) =>
            Tuple(lefts(self), rights(self));

        public static Task<Either<L, R2>> mapAsync<L, R, R2>(Either<L, R> self, Func<R, Task<R2>> map) =>
            self.MapAsync(map);

        public static Task<Either<L, R2>> mapAsync<L, R, R2>(Task<Either<L, R>> self, Func<R, Task<R2>> map) =>
            self.MapAsync(map);

        public static Task<Either<L, R2>> mapAsync<L, R, R2>(Task<Either<L, R>> self, Func<R, R2> map) =>
            self.MapAsync(map);

        public static Task<Either<L, R2>> mapAsync<L, R, R2>(Either<L, Task<R>> self, Func<R, R2> map) =>
            self.MapAsync(map);

        public static Task<Either<L, R2>> mapAsync<L, R, R2>(Either<L, Task<R>> self, Func<R, Task<R2>> map) =>
            self.MapAsync(map);

        public static Task<Either<L, R2>> bindAsync<L, R, R2>(Either<L, R> self, Func<R, Task<Either<L, R2>>> bind) =>
            self.BindAsync(bind);

        public static Task<Either<L, R2>> bindAsync<L, R, R2>(Task<Either<L, R>> self, Func<R, Task<Either<L, R2>>> bind) =>
            self.BindAsync(bind);

        public static Task<Either<L, R2>> bindAsync<L, R, R2>(Task<Either<L, R>> self, Func<R, Either<L, R2>> bind) =>
            self.BindAsync(bind);

        public static Task<Either<L, R2>> bindAsync<L, R, R2>(Either<L, Task<R>> self, Func<R, Either<L, R2>> bind) =>
            self.BindAsync(bind);

        public static Task<Either<L, R2>> bindAsync<L, R, R2>(Either<L, Task<R>> self, Func<R, Task<Either<L, R2>>> bind) =>
            self.BindAsync(bind);

        public static Task<Unit> iterAsync<L, R>(Task<Either<L, R>> self, Action<R> action) =>
            self.IterAsync(action);

        public static Task<Unit> iterAsync<L, R>(this Either<L, Task<R>> self, Action<R> action) =>
            self.IterAsync(action);

        public static Task<int> countAsync<L, R>(Task<Either<L, R>> self) =>
            self.CountAsync();

        public static Task<int> sumAsync<L>(Task<Either<L, int>> self) =>
            self.SumAsync();

        public static Task<int> sumAsync<L>(Either<L, Task<int>> self) =>
            self.SumAsync();

        public static Task<S> foldAsync<L, R, S>(Task<Either<L, R>> self, S state, Func<S, R, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<S> foldAsync<L, R, S>(Either<L, Task<R>> self, S state, Func<S, R, S> folder) =>
            self.FoldAsync(state, folder);

        public static Task<bool> forallAsync<L, R>(Task<Either<L, R>> self, Func<R, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> forallAsync<L, R>(Either<L, Task<R>> self, Func<R, bool> pred) =>
            self.ForAllAsync(pred);

        public static Task<bool> existsAsync<L, R>(Task<Either<L, R>> self, Func<R, bool> pred) =>
            self.ExistsAsync(pred);

        public static Task<bool> existsAsync<L, R>(Either<L, Task<R>> self, Func<R, bool> pred) =>
            self.ExistsAsync(pred);
    }
}
