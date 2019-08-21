using System;
using LanguageExt;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.Common;

public static class TaskTryOptionExtensions
{
    /// <summary>
    /// Convert a Task<TryOption<A>> to a TryOptionAsync<A>
    /// </summary>
    [Pure]
    public static TryOptionAsync<A> ToAsync<A>(this Task<TryOption<A>> self) => 
        async () =>
        {
            try
            {
                var resT = await self;
                return resT.Try();
            }
            catch (Exception e)
            {
                return new OptionalResult<A>(e);
            }
        };

    /// <summary>
    /// Convert a Task<TryOption<A>> to a TryOptionAsync<A>
    /// </summary>
    [Pure]
    public static TryOptionAsync<A> ToAsync<A>(this TryOption<Task<A>> self) =>
        async () =>
        {
            try
            {
                var task = self.Try();
                if (task.IsFaultedOrNone) return new OptionalResult<A>(task.Exception);
                return await task.Value.Value;
            }
            catch (Exception e)
            {
                return new OptionalResult<A>(e);
            }
        };


    /// <summary>
    /// Invoke a delegate if the Try returns a value successfully
    /// </summary>
    /// <param name="Some">Delegate to invoke if successful</param>
    public static Task<Unit> IfSome<A>(this Task<TryOption<A>> self, Action<A> Some) =>
        self.ToAsync().IfSome(Some);

    /// <summary>
    /// Invoke a delegate if the Try is in a Fail or None state
    /// </summary>
    /// <param name="None">Delegate to invoke if successful</param>
    public static Task<Unit> IfNoneOrFail<A>(this Task<TryOption<A>> self, Action None) =>
        self.ToAsync().IfNoneOrFail(None);

    /// <summary>
    /// Return a default value if the Try fails
    /// </summary>
    /// <param name="defaultValue">Default value to use on failure</param>
    /// <returns>failValue on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfNoneOrFail<A>(this Task<TryOption<A>> self, A defaultValue) =>
        self.ToAsync().IfNoneOrFail(defaultValue);

    /// <summary>
    /// Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="None">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfNoneOrFail<A>(this Task<TryOption<A>> self, Func<A> None) =>
        self.ToAsync().IfNoneOrFail(None);

    /// <summary>
    /// Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="None">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfNoneOrFail<A>(this Task<TryOption<A>> self, Func<Task<A>> None) =>
        self.ToAsync().IfNoneOrFail(None);

    /// <summary>
    /// Invoke delegates based on None or Failed stateds
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="None">Delegate to invoke if the result is None</param>
    /// <param name="Fail">Delegate to invoke if the result is Fail</param>
    /// <returns>Success value, or the result of the None or Failed delegate</returns>
    [Pure]
    public static Task<A> IfNoneOrFail<A>(
        this Task<TryOption<A>> self,
        Func<A> None,
        Func<Exception, A> Fail) =>
            self.ToAsync().IfNoneOrFail(None, Fail);

    /// <summary>
    /// Invoke delegates based on None or Failed stateds
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="None">Delegate to invoke if the result is None</param>
    /// <param name="Fail">Delegate to invoke if the result is Fail</param>
    /// <returns>Success value, or the result of the None or Failed delegate</returns>
    [Pure]
    public static Task<A> IfNoneOrFail<A>(
        this Task<TryOption<A>> self,
        Func<Task<A>> None,
        Func<Exception, A> Fail) =>
            self.ToAsync().IfNoneOrFail(None, Fail);

    /// <summary>
    /// Invoke delegates based on None or Failed stateds
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="None">Delegate to invoke if the result is None</param>
    /// <param name="Fail">Delegate to invoke if the result is Fail</param>
    /// <returns>Success value, or the result of the None or Failed delegate</returns>
    [Pure]
    public static Task<A> IfNoneOrFail<A>(
        this Task<TryOption<A>> self,
        Func<A> None,
        Func<Exception, Task<A>> Fail) =>
            self.ToAsync().IfNoneOrFail(None, Fail);

    /// <summary>
    /// Invoke delegates based on None or Failed stateds
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="None">Delegate to invoke if the result is None</param>
    /// <param name="Fail">Delegate to invoke if the result is Fail</param>
    /// <returns>Success value, or the result of the None or Failed delegate</returns>
    [Pure]
    public static Task<A> IfNoneOrFail<A>(
        this Task<TryOption<A>> self,
        Func<Task<A>> None,
        Func<Exception, Task<A>> Fail) =>
            self.ToAsync().IfNoneOrFail(None, Fail);

    /// <summary>
    /// Provides a fluent exception matching interface which is invoked
    /// when the Try fails.
    /// </summary>
    /// <returns>Fluent exception matcher</returns>
    [Pure]
    public static ExceptionMatchOptionalAsync<A> IfFail<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().IfFail();

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<TryOption<A>> self, Func<A, R> Succ, Func<R> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<TryOption<A>> self, Func<A, R> Some, Func<R> None, Func<Exception, R> Fail) =>
        self.ToAsync().Match(Some, None, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Default value to use if the Try computation fails</param>
    /// <returns>The result of either the Succ delegate or the Fail value</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<TryOption<A>> self, Func<A, R> Succ, R Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    public static Task<Unit> Match<A>(this Task<TryOption<A>> self, Action<A> Succ, Action Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <param name="Some">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    public static Task<Unit> Match<A>(this Task<TryOption<A>> self, Action<A> Some, Action None, Action<Exception> Fail) =>
        self.ToAsync().Match(Some, None, Fail);

    /// <summary>
    /// Pattern matches the three possible states of the computation computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Some">Delegate to invoke if the computation completes successfully</param>
    /// <param name="None">Delegate to invoke if the computation completes successfully but returns no value</param>
    /// <param name="Fail">Delegate to invoke if the computation fails</param>
    /// <returns>The result of either the Succ, None, or Fail delegate</returns>
    public static Task<R> MatchAsync<A, R>(this Task<TryOption<A>> self, Func<A, Task<R>> Some, Func<R> None, Func<Exception, R> Fail) =>
        self.ToAsync().MatchAsync(Some, None, Fail);

    /// <summary>
    /// Pattern matches the three possible states of the computation computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the computation fails</param>
    /// <returns>The result of either the Succ, None, or Fail delegate</returns>
    public static Task<R> MatchAsync<A, R>(this Task<TryOption<A>> self, Func<A, Task<R>> Succ, Func<Task<R>> Fail) =>
        self.ToAsync().MatchAsync(Succ, Fail);

    /// <summary>
    /// Pattern matches the three possible states of the computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Some">Delegate to invoke if the computation completes successfully</param>
    /// <param name="None">Delegate to invoke if the computation completes successfully but returns no value</param>
    /// <param name="Fail">Delegate to invoke if the computation fails</param>
    /// <returns>The result of either the Some, None, or Fail delegate</returns>
    public static Task<R> MatchAsync<A, R>(this Task<TryOption<A>> self, Func<A, Task<R>> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().MatchAsync(Some, None, Fail);

    /// <summary>
    /// Pattern matches the three possible states of the computation computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the computation fails</param>
    /// <returns>The result of either the Succ, Fail delegate</returns>
    public static Task<R> MatchAsync<A, R>(this Task<TryOption<A>> self, Func<A, R> Succ, Func<Task<R>> Fail) =>
        self.ToAsync().MatchAsync(Succ, Fail);

    /// <summary>
    /// Pattern matches the three possible states of the computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Some">Delegate to invoke if the computation completes successfully</param>
    /// <param name="None">Delegate to invoke if the computation completes successfully but returns no value</param>
    /// <param name="Fail">Delegate to invoke if the computation fails</param>
    /// <returns>The result of either the Some, None, or Fail delegate</returns>
    public static Task<R> MatchAsync<A, R>(this Task<TryOption<A>> self, Func<A, R> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().MatchAsync(Some, None, Fail);

    [Pure]
    public static Task<Option<A>> ToOption<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().ToOption();

    [Pure]
    public static Task<A> IfFailThrow<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().IfFailThrow();

    /// <summary>
    /// Apply Try values to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg1">Try argument</param>
    /// <param name="arg2">Try argument</param>
    /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
    public static Task<Unit> Iter<A>(this Task<TryOption<A>> self, Action<A> action) =>
        self.ToAsync().Iter(action);

    /// <summary>
    /// Counts the number of bound values.  
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">TrTry computation</param>
    /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
    [Pure]
    public static Task<int> Count<A>(this Task<TryOption<A>> self) =>
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
    public static Task<bool> ForAll<A>(this Task<TryOption<A>> self, Func<A, bool> pred) =>
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
    public static Task<bool> ForAllAsync<A>(this Task<TryOption<A>> self, Func<A, Task<bool>> pred) =>
        self.ToAsync().ForAllAsync(pred);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> Fold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, S> folder) =>
        self.ToAsync().Fold(state, folder);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> FoldAsync<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, Task<S>> folder) =>
        self.ToAsync().FoldAsync(state, folder);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, S> Succ, Func<S, S> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, Task<S>> Succ, Func<S, S> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, S> Succ, Func<S, Task<S>> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> BiFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, Task<S>> Succ, Func<S, Task<S>> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Success</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> TriFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, S> Some, Func<S, S> None, Func<S, Exception, S> Fail) =>
        self.ToAsync().TriFold(state, Some, None, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Success</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> TriFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, Task<S>> Some, Func<S, S> None, Func<S, Exception, S> Fail) =>
        self.ToAsync().TriFold(state, Some, None, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Success</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> TriFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, S> Some, Func<S, Task<S>> None, Func<S, Exception, S> Fail) =>
        self.ToAsync().TriFold(state, Some, None, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Success</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> TriFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, S> Some, Func<S, S> None, Func<S, Exception, Task<S>> Fail) =>
        self.ToAsync().TriFold(state, Some, None, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Success</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> TriFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, Task<S>> Some, Func<S, Task<S>> None, Func<S, Exception, S> Fail) =>
        self.ToAsync().TriFold(state, Some, None, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Success</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> TriFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, S> Some, Func<S, Task<S>> None, Func<S, Exception, Task<S>> Fail) =>
        self.ToAsync().TriFold(state, Some, None, Fail);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Success</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> TriFold<A, S>(this Task<TryOption<A>> self, S state, Func<S, A, Task<S>> Some, Func<S, Task<S>> None, Func<S, Exception, Task<S>> Fail) =>
        self.ToAsync().TriFold(state, Some, None, Fail);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> Exists<A>(this Task<TryOption<A>> self, Func<A, bool> pred) =>
        self.ToAsync().Exists(pred);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ExistsAsync<A>(this Task<TryOption<A>> self, Func<A, Task<bool>> pred) =>
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
    public static TryOptionAsync<R> Map<A, R>(this Task<TryOption<A>> self, Func<A, R> mapper) =>
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
    public static TryOptionAsync<R> MapAsync<A, R>(this Task<TryOption<A>> self, Func<A, Task<R>> mapper) =>
        self.ToAsync().MapAsync(mapper);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> BiMap<A, B>(this Task<TryOption<A>> self, Func<A, B> Succ, Func<B> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> BiMap<A, B>(this Task<TryOption<A>> self, Func<A, Task<B>> Succ, Func<B> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> BiMap<A, B>(this Task<TryOption<A>> self, Func<A, B> Succ, Func<Task<B>> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> BiMap<A, B>(this Task<TryOption<A>> self, Func<A, Task<B>> Succ, Func<Task<B>> Fail) =>
        self.ToAsync().BiMap(Succ, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Some">Delegate to map the bound value</param>
    /// <param name="None">Delegate to map the None to the desired bound result type</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> TriMap<A, B>(this Task<TryOption<A>> self, Func<A, B> Some, Func<B> None, Func<Exception, B> Fail) =>
        self.ToAsync().TriMap(Some, None, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Some">Delegate to map the bound value</param>
    /// <param name="None">Delegate to map the None to the desired bound result type</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> TriMap<A, B>(this Task<TryOption<A>> self, Func<A, Task<B>> Some, Func<B> None, Func<Exception, B> Fail) =>
        self.ToAsync().TriMap(Some, None, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Some">Delegate to map the bound value</param>
    /// <param name="None">Delegate to map the None to the desired bound result type</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> TriMap<A, B>(this Task<TryOption<A>> self, Func<A, B> Some, Func<Task<B>> None, Func<Exception, B> Fail) =>
        self.ToAsync().TriMap(Some, None, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Some">Delegate to map the bound value</param>
    /// <param name="None">Delegate to map the None to the desired bound result type</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> TriMap<A, B>(this Task<TryOption<A>> self, Func<A, B> Some, Func<B> None, Func<Exception, Task<B>> Fail) =>
        self.ToAsync().TriMap(Some, None, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Some">Delegate to map the bound value</param>
    /// <param name="None">Delegate to map the None to the desired bound result type</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> TriMap<A, B>(this Task<TryOption<A>> self, Func<A, Task<B>> Some, Func<Task<B>> None, Func<Exception, B> Fail) =>
        self.ToAsync().TriMap(Some, None, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Some">Delegate to map the bound value</param>
    /// <param name="None">Delegate to map the None to the desired bound result type</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> TriMap<A, B>(this Task<TryOption<A>> self, Func<A, B> Some, Func<Task<B>> None, Func<Exception, Task<B>> Fail) =>
        self.ToAsync().TriMap(Some, None, Fail);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">computation</param>
    /// <param name="Some">Delegate to map the bound value</param>
    /// <param name="None">Delegate to map the None to the desired bound result type</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped computation</returns>
    [Pure]
    public static TryOptionAsync<B> TriMap<A, B>(this Task<TryOption<A>> self, Func<A, Task<B>> Some, Func<Task<B>> None, Func<Exception, Task<B>> Fail) =>
        self.ToAsync().TriMap(Some, None, Fail);

    [Pure]
    public static TryOptionAsync<A> Filter<A>(this Task<TryOption<A>> self, Func<A, bool> pred) =>
        self.ToAsync().Filter(pred);

    [Pure]
    public static TryOptionAsync<A> Filter<A>(this Task<TryOption<A>> self, Func<A, Task<bool>> pred) =>
        self.ToAsync().Filter(pred);

    [Pure]
    public static TryOptionAsync<A> Where<A>(this Task<TryOption<A>> self, Func<A, bool> pred) =>
        self.ToAsync().Where(pred);

    [Pure]
    public static TryOptionAsync<A> Where<A>(this Task<TryOption<A>> self, Func<A, Task<bool>> pred) =>
        self.ToAsync().Where(pred);

    [Pure]
    public static Task<Seq<A>> AsEnumerable<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().AsEnumerable();

    [Pure]
    public static Task<Seq<A>> ToSeq<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().ToSeq();

    [Pure]
    public static Task<Lst<A>> ToList<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().ToList();

    [Pure]
    public static Task<Arr<A>> ToArray<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().ToArray();

    [Pure]
    public static Task<string> AsString<A>(this Task<TryOption<A>> self) =>
        self.ToAsync().AsString();

    [Pure]
    public static Task<int> Sum(this Task<TryOption<int>> self) =>
        self.ToAsync().Sum();

    /// <summary>
    /// Add the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryOptionAsync<A> Add<NUM, A>(this Task<TryOption<A>> lhs, Task<TryOption<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Add<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Find the subtract of the bound value of Try(x) and Try(y).  If either of 
    /// the Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryOptionAsync<A> Subtract<NUM, A>(this Task<TryOption<A>> lhs, Task<TryOption<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Subtract<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryOptionAsync<A> Product<NUM, A>(this Task<TryOption<A>> lhs, Task<TryOption<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Product<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryOptionAsync<A> Divide<NUM, A>(this Task<TryOption<A>> lhs, Task<TryOption<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Divide<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Convert the Try type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Try to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static Task<A?> ToNullable<A>(this Task<TryOption<A>> ma) where A : struct =>
        ma.ToAsync().ToNullable();
}
