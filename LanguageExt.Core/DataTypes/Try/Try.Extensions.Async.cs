using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.Common;

/// <summary>
/// Extension methods for the Try monad
/// </summary>
public static class TryExtensionsAsync
{
    /// <summary>
    /// Converts this Try to a TryAsync
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="self">This</param>
    /// <returns>Asynchronous Try</returns>
    [Pure]
    public static TryAsync<A> ToAsync<A>(this Try<A> self) => () =>
        self.Match(
            Succ: x => new Result<A>(x),
            Fail: e => new Result<A>(e))
       .AsTask();

    /// <summary>
    /// Runs the Try asynchronously.  Invoke a delegate if the Try returns a 
    /// value successfully
    /// </summary>
    /// <param name="Succ">Delegate to invoke if successful</param>
    [Pure]
    public static Task<Unit> IfSuccAsync<A>(this Try<A> self, Action<A> Succ) =>
        self.ToAsync().IfSucc(Succ);

    /// <summary>
    /// Runs the Try asynchronously.  Return a default value if the Try fails
    /// </summary>
    /// <param name="failValue">Default value to use on failure</param>
    /// <returns>failValue on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfFailAsync<A>(this Try<A> self, A failValue) =>
        self.ToAsync().IfFail(failValue);

    /// <summary>
    /// Runs the Try asynchronously.  Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="Fail">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfFailAsync<A>(this Try<A> self, Func<Task<A>> Fail) =>
        self.ToAsync().IfFail(Fail);

    /// <summary>
    /// Runs the Try asynchronously.  Returns the Succ(value) of the 
    /// Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static Task<A> IfFailAsync<A>(this Try<A> self, Func<Exception, Task<A>> Fail) =>
        self.ToAsync().IfFail(Fail);

    /// <summary>
    /// Runs the Try asynchronously.  Provides a fluent exception matching interface 
    /// which is invoked when the Try fails.
    /// </summary>
    /// <returns>Fluent exception matcher</returns>
    [Pure]
    public static ExceptionMatchAsync<A> IfFailAsync<A>(this Try<A> self) =>
        self.ToAsync().IfFail();

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Default value to use if the Try computation fails</param>
    /// <returns>The result of either the Succ delegate or the Fail value</returns>
    [Pure]
    public static Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, R> Succ, R Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    public static Task<Unit> MatchAsync<A>(this Try<A> self, Action<A> Succ, Action<Exception> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, Task<R>> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, Task<R>> Succ, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    [Pure]
    public static Task<Option<A>> ToOptionAsync<A>(this Try<A> self) =>
        self.ToAsync().ToOption();

    [Pure]
    public static Task<OptionUnsafe<A>> ToOptionUnsafeAsync<A>(this Try<A> self) =>
        self.ToAsync().ToOptionUnsafe();

    [Pure]
    public static EitherAsync<Error, A> ToEitherAsync<A>(this Try<A> self) =>
        self.ToAsync().ToEither();

    [Pure]
    public static Task<EitherUnsafe<Error, A>> ToEitherUnsafeAsync<A>(this Try<A> self) =>
        self.ToAsync().ToEitherUnsafe();

    [Pure]
    public static Task<A> IfFailThrowAsync<A>(this Try<A> self) =>
        self.ToAsync().IfFailThrow();

    /// <summary>
    /// Map the bound value from A to Task of B
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Returned bound value type</typeparam>
    /// <param name="self">This</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Returns an asynchronous Try</returns>
    [Pure]
    public static TryAsync<B> Select<A, B>(this Try<A> self, Func<A, Task<B>> f) =>
        self.ToAsync().Select(f);

    /// <summary>
    /// Apply Try values to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg1">Try argument</param>
    /// <param name="arg2">Try argument</param>
    /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
    public static Task<Unit> IterAsync<A>(this Try<A> self, Action<A> action) =>
        self.ToAsync().IfSucc(action);

    /// <summary>
    /// Counts the number of bound values.  
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">TrTry computation</param>
    /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
    [Pure]
    public static Task<int> CountAsync<A>(this Try<A> self) =>
        self.ToAsync().Count();

    /// <summary>
    /// Tests that a predicate holds for all values of the bound value T
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value, or if the Try computation
    /// fails.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ForAllAsync<A>(this Try<A> self, Func<A, bool> pred) =>
        self.ToAsync().ForAll(pred);

    /// <summary>
    /// Tests that a predicate holds for all values of the bound value T
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value, or if the Try computation
    /// fails.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ForAllAsync<A>(this Try<A> self, Func<A, Task<bool>> pred) =>
        self.ToAsync().ForAllAsync(pred);

    /// <summary>
    /// Folds Try value into an S.
    /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> FoldAsync<A, S>(this Try<A> self, S state, Func<S, A, S> folder) =>
        self.ToAsync().Fold(state, folder);

    /// <summary>
    /// Folds Try value into an S.
    /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> FoldAsync<A, S>(this Try<A> self, S state, Func<S, A, Task<S>> folder) =>
        self.ToAsync().FoldAsync(state, folder);

    /// <summary>
    /// Folds Try value into an S.
    /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFoldAsync<A, S>(this Try<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, S> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFoldAsync<A, S>(this Try<A> self, S state, Func<S, A, Task<S>> Succ, Func<S, Exception, S> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFoldAsync<A, S>(this Try<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, Task<S>> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// [wikipedia.org/wiki/Fold_(higher-order_function)](https://en.wikipedia.org/wiki/Fold_(higher-order_function))
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFoldAsync<A, S>(this Try<A> self, S state, Func<S, A, Task<S>> Succ, Func<S, Exception, Task<S>> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ExistsAsync<A>(this Try<A> self, Func<A, bool> pred) =>
        self.ToAsync().Exists(pred);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ExistsAsync<A>(this Try<A> self, Func<A, Task<bool>> pred) =>
        self.ToAsync().ExistsAsync(pred);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<R> MapAsync<A, R>(this Try<A> self, Func<A, R> mapper) =>
        self.ToAsync().Map(mapper);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<R> MapAsync<A, R>(this Try<A> self, Func<A, Task<R>> mapper) =>
        self.ToAsync().MapAsync(mapper);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<R> BiMapAsync<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<R> BiMapAsync<A, R>(this Try<A> self, Func<A, Task<R>> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<R> BiMapAsync<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<R> BiMapAsync<A, R>(this Try<A> self, Func<A, Task<R>> Succ, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    [Pure]
    public static TryAsync<A> FilterAsync<A>(this Try<A> self, Func<A, bool> pred) =>
        self.ToAsync().Filter(pred);

    [Pure]
    public static TryAsync<A> FilterAsync<A>(this Try<A> self, Func<A, Task<bool>> pred) =>
        self.ToAsync().Filter(pred);

    [Pure]
    public static TryAsync<A> BiFilterAsync<A>(this Try<A> self, Func<A, bool> Succ, Func<Exception, bool> Fail) =>
        self.ToAsync().BiFilter(Succ, Fail);

    [Pure]
    public static TryAsync<A> BiFilterAsync<A>(this Try<A> self, Func<A, Task<bool>> Succ, Func<Exception, bool> Fail) =>
        self.ToAsync().BiFilter(Succ, Fail);

    [Pure]
    public static TryAsync<A> BiFilterAsync<A>(this Try<A> self, Func<A, bool> Succ, Func<Exception, Task<bool>> Fail) =>
        self.ToAsync().BiFilter(Succ, Fail);

    [Pure]
    public static TryAsync<A> BiFilterAsync<A>(this Try<A> self, Func<A, Task<bool>> Succ, Func<Exception, Task<bool>> Fail) =>
        self.ToAsync().BiFilter(Succ, Fail);

    [Pure]
    public static TryAsync<A> Where<A>(this Try<A> self, Func<A, Task<bool>> pred) =>
        self.FilterAsync(pred);

    [Pure]
    public static Task<Seq<A>> ToSeqAsync<A>(this Try<A> self) =>
        self.ToAsync().ToSeq();

    [Pure]
    public static Task<Seq<A>> AsEnumerableAsync<A>(this Try<A> self) =>
        self.ToAsync().AsEnumerable();

    [Pure]
    public static Task<Lst<A>> ToListAsync<A>(this Try<A> self) =>
        self.ToAsync().ToList();

    [Pure]
    public static Task<Arr<A>> ToArrayAsync<A>(this Try<A> self) =>
        self.ToAsync().ToArray();

    [Pure]
    public static Task<string> AsStringAsync<A>(this Try<A> self) =>
        self.ToAsync().AsString();

    [Pure]
    public static Task<int> SumAsync(this Try<int> self) =>
        self.ToAsync().Sum();

    /// <summary>
    /// Compare the bound value of Try(x) to Try(y).  If either of the
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>1 if lhs > rhs, 0 if lhs == rhs, -1 if lhs < rhs</returns>
    [Pure]
    public static Task<int> CompareAsync<ORD, A>(this Try<A> lhs, Try<A> rhs) where ORD : struct, Ord<A> =>
        lhs.ToAsync().Compare<ORD, A>(rhs.ToAsync());

    /// <summary>
    /// Append the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs ++ rhs</returns>
    [Pure]
    public static TryAsync<A> AppendAsync<SEMI, A>(this Try<A> lhs, Try<A> rhs) where SEMI : struct, Semigroup<A> =>
        lhs.ToAsync().Append<SEMI, A>(rhs.ToAsync());

    /// <summary>
    /// Add the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> AddAsync<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Add<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Find the subtract of the bound value of Try(x) and Try(y).  If either of 
    /// the Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> SubtractAsync<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Subtract<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> ProductAsync<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Product<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> DivideAsync<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Divide<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Convert the Try type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Try to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static Task<A?> ToNullableAsync<A>(this Try<A> ma) where A : struct =>
        ma.ToAsync().ToNullable();
}
