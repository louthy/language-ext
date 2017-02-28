using System;
using System.Linq;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Reactive.Linq;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for the Try monad
/// </summary>
public static class TryExtensions
{
    /// <summary>
    /// Converts this Try to a TryAsync
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="self">This</param>
    /// <returns>Asynchronous Try</returns>
    [Pure]
    public static TryAsync<A> ToAsync<A>(this Try<A> self) => () =>
        Task.Run(() => self.Try());

    /// <summary>
    /// Invoke a delegate if the Try returns a value successfully
    /// </summary>
    /// <param name="Succ">Delegate to invoke if successful</param>
    [Pure]
    public static Unit IfSucc<A>(this Try<A> self, Action<A> Succ)
    {
        var res = self.Try();
        if (!res.IsFaulted)
        {
            Succ(res.Value);
        }
        return unit;
    }

    /// <summary>
    /// Runs the Try asynchronously.  Invoke a delegate if the Try returns a 
    /// value successfully
    /// </summary>
    /// <param name="Succ">Delegate to invoke if successful</param>
    [Pure]
    public static Task<Unit> IfSuccAsync<A>(this Try<A> self, Action<A> Succ) =>
        self.ToAsync().IfSucc(Succ);

    /// <summary>
    /// Return a default value if the Try fails
    /// </summary>
    /// <param name="failValue">Default value to use on failure</param>
    /// <returns>failValue on failure, the result of the Try otherwise</returns>
    [Pure]
    public static A IfFail<A>(this Try<A> self, A failValue)
    {
        if (isnull(failValue)) throw new ArgumentNullException(nameof(failValue));

        var res = self.Try();
        if (res.IsFaulted)
            return failValue;
        else
            return res.Value;
    }

    /// <summary>
    /// Runs the Try asynchronously.  Return a default value if the Try fails
    /// </summary>
    /// <param name="failValue">Default value to use on failure</param>
    /// <returns>failValue on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfFailAsync<A>(this Try<A> self, A failValue) =>
        self.ToAsync().IfFail(failValue);

    /// <summary>
    /// Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="Fail">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static A IfFail<A>(this Try<A> self, Func<A> Fail)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return Fail();
        else
            return res.Value;
    }

    /// <summary>
    /// Runs the Try asynchronously.  Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="Fail">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfFailAsync<A>(this Try<A> self, Func<Task<A>> Fail) =>
        self.ToAsync().IfFail(Fail);

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static A IfFail<A>(this Try<A> self, Func<Exception, A> Fail)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return Fail(res.Exception);
        else
            return res.Value;
    }

    /// <summary>
    /// Runs the Try asynchronously.  Returns the Succ(value) of the 
    /// Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static Task<A> IfFailAsync<A>(this Try<A> self, Func<Exception, Task<A>> Fail) =>
        self.ToAsync().IfFail(Fail);

    /// <summary>
    /// Provides a fluent exception matching interface which is invoked
    /// when the Try fails.
    /// </summary>
    /// <returns>Fluent exception matcher</returns>
    [Pure]
    public static ExceptionMatch<A> IfFail<A>(this Try<A> self)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return res.Exception.Match<A>();
        else
            return new ExceptionMatch<A>(res.Value);
    }

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
    public static R Match<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    }

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
    public static R Match<A, R>(this Try<A> self, Func<A, R> Succ, R Fail)
    {
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = self.Try();
        return res.IsFaulted
            ? Fail
            : Succ(res.Value);
    }

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
    public static Unit Match<A>(this Try<A> self, Action<A> Succ, Action<Exception> Fail)
    {
        var res = self.Try();

        if (res.IsFaulted)
            Fail(res.Exception);
        else
            Succ(res.Value);

        return Unit.Default;
    }

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

    /// <summary>
    /// Turns the computation into an observable stream
    /// </summary>
    /// <typeparam name="A">Bound type</typeparam>
    /// <typeparam name="R">Returned observable bound type</typeparam>
    /// <param name="self">This</param>
    /// <param name="Succ">Function to call when the operation succeeds</param>
    /// <param name="Fail">Function to call when the operation fails</param>
    /// <returns>An observable that represents the result of Succ or Fail</returns>
    [Pure]
    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().MatchObservable(Succ, Fail);

    /// <summary>
    /// Turns the computation into an observable stream
    /// </summary>
    /// <typeparam name="A">Bound type</typeparam>
    /// <typeparam name="R">Returned observable bound type</typeparam>
    /// <param name="self">This</param>
    /// <param name="Succ">Function to call when the operation succeeds</param>
    /// <param name="Fail">Function to call when the operation fails</param>
    /// <returns>An observable that represents the result of Succ or Fail</returns>
    [Pure]
    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
        self.ToAsync().MatchObservable(Succ, Fail);

    /// <summary>
    /// Turns the computation into an observable stream
    /// </summary>
    /// <typeparam name="A">Bound type</typeparam>
    /// <typeparam name="R">Returned observable bound type</typeparam>
    /// <param name="self">This</param>
    /// <param name="Succ">Function to call when the operation succeeds</param>
    /// <param name="Fail">Function to call when the operation fails</param>
    /// <returns>An observable that represents the result of Succ or Fail</returns>
    [Pure]
    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, IObservable<R>> Fail) =>
        self.ToAsync().MatchObservable(Succ, Fail);

    /// <summary>
    /// Memoise the try
    /// </summary>
    [Pure]
    public static Try<A> Memo<A>(this Try<A> self)
    {
        var res = self.Try();
        return () =>
        {
            if (res.IsFaulted) throw new InnerException(res.Exception);
            return res.Value;
        };
    }

    [Pure]
    public static Option<A> ToOption<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? None
            : Optional(res.Value);
    }

    [Pure]
    public static Task<Option<A>> ToOptionAsync<A>(this Try<A> self) =>
        self.ToAsync().ToOption();

    [Pure]
    public static TryOption<A> ToTryOption<A>(this Try<A> self) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? None
            : Optional(res.Value);
    };

    // TODO: Need TryOptionAsync
    //[Pure]
    //public static TryOption<A> ToTryOption<A>(this Try<A> self) => () =>

    [Pure]
    public static A IfFailThrow<A>(this Try<A> self)
    {
        try
        {
            return self().Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            throw;
        }
    }

    [Pure]
    public static Task<A> IfFailThrowAsync<A>(this Try<A> self) =>
        self.ToAsync().IfFailThrow();

    /// <summary>
    /// Map the bound value from A to B
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Returned bound value type</typeparam>
    /// <param name="self">This</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped Try</returns>
    [Pure]
    public static Try<B> Select<A, B>(this Try<A> self, Func<A, B> f) => () =>
        f(self().Value);

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
    public static Unit Iter<A>(this Try<A> self, Action<A> action) =>
        self.IfSucc(action);

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
    public static int Count<A>(this Try<A> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? 0
            : 1;
    }

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
    public static bool ForAll<A>(this Try<A> self, Func<A, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : pred(res.Value);
    }

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
        self.ToAsync().ForAll(pred);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<A, S>(this Try<A> self, S state, Func<S, A, S> folder)
    {
        var res = self.Try();
        return res.IsFaulted
            ? state
            : folder(state, res.Value);
    }

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
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
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> FoldAsync<A, S>(this Try<A> self, S state, Func<S, A, Task<S>> folder) =>
        self.ToAsync().Fold(state, folder);

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
    public static S BiFold<A, S>(this Try<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, S> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(state, res.Exception)
            : Succ(state, res.Value);
    }

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
    public static Task<S> BiFoldAsync<A, S>(this Try<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, S> Fail) =>
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
    public static Task<S> BiFoldAsync<A, S>(this Try<A> self, S state, Func<S, A, Task<S>> Succ, Func<S, Exception, S> Fail) =>
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
    public static Task<S> BiFoldAsync<A, S>(this Try<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, Task<S>> Fail) =>
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
    public static bool Exists<A>(this Try<A> self, Func<A, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : pred(res.Value);
    }

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
        self.ToAsync().Exists(pred);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static Try<R> Map<A, R>(this Try<A> self, Func<A, R> mapper) => () =>
        new Result<R>(mapper(self().Value));

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
        self.ToAsync().Map(mapper);

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
    public static Try<R> BiMap<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, R> Fail) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    };

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

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Try<Func<B, R>> ParMap<A, B, R>(this Try<A> self, Func<A, B, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static Try<Func<B, Func<C, R>>> ParMap<A, B, C, R>(this Try<A> self, Func<A, B, C, R> func) =>
        self.Map(curry(func));

    [Pure]
    public static Try<A> Filter<A>(this Try<A> self, Func<A, bool> pred) => () =>
    {
        var res = self();
        return pred(res.Value)
            ? res
            : raise<A>(new BottomException());
    };

    [Pure]
    public static TryAsync<A> FilterAsync<A>(this Try<A> self, Func<A, bool> pred) =>
        self.ToAsync().Filter(pred);

    [Pure]
    public static TryAsync<A> FilterAsync<A>(this Try<A> self, Func<A, Task<bool>> pred) =>
        self.ToAsync().Filter(pred);

    [Pure]
    public static Try<A> BiFilter<A>(this Try<A> self, Func<A, bool> Succ, Func<Exception, bool> Fail) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
                ? res.Value
                : raise<A>(new BottomException())
            : Succ(res.Value)
                ? res.Value
                : raise<A>(new BottomException());
    };

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
    public static Try<A> Where<A>(this Try<A> self, Func<A, bool> pred) =>
        self.Filter(pred);

    [Pure]
    public static TryAsync<A> Where<A>(this Try<A> self, Func<A, Task<bool>> pred) =>
        self.FilterAsync(pred);

    [Pure]
    public static Try<R> Bind<A, R>(this Try<A> self, Func<A, Try<R>> binder) => () => 
        binder(self().Value)().Value;

    [Pure]
    public static Try<R> BiBind<A, R>(this Try<A> self, Func<A, Try<R>> Succ, Func<Exception, Try<R>> Fail) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception).Try()
            : Succ(res.Value).Try();
    };

    [Pure]
    public static IEnumerable<Either<Exception, A>> AsEnumerable<A>(this Try<A> self)
    {
        var res = self.Try();
        if (res.IsFaulted)
        {
            yield return res.Exception;
        }
        else
        {
            yield return res.Value;
        }
    }

    [Pure]
    public static Task<IEnumerable<Either<Exception, A>>> AsEnumerableAsync<A>(this Try<A> self) =>
        self.ToAsync().AsEnumerable();

    [Pure]
    public static Lst<Either<Exception, A>> ToList<A>(this Try<A> self) =>
        toList(self.AsEnumerable());

    [Pure]
    public static Task<Lst<Either<Exception, A>>> ToListAsync<A>(this Try<A> self) =>
        self.ToAsync().ToList();

    [Pure]
    public static Arr<Either<Exception, A>> ToArray<A>(this Try<A> self) =>
        toArray(self.AsEnumerable());

    [Pure]
    public static Task<Arr<Either<Exception, A>>> ToArrayAsync<A>(this Try<A> self) =>
        self.ToAsync().ToArray();

    [Pure]
    public static TrySuccContext<A, R> Succ<A,R>(this Try<A> self, Func<A, R> succHandler) =>
        new TrySuccContext<A, R>(self, succHandler);

    [Pure]
    public static TrySuccUnitContext<A> Succ<A>(this Try<A> self, Action<A> succHandler) =>
        new TrySuccUnitContext<A>(self, succHandler);

    [Pure]
    public static string AsString<A>(this Try<A> self) =>
        self.Match(
            Succ: v => isnull(v)
                      ? "Succ(null)"
                      : $"Succ({v})",
            Fail: ex => $"Fail({ex.Message})");

    [Pure]
    public static Task<string> AsStringAsync<A>(this Try<A> self) =>
        self.ToAsync().AsString();

    [Pure]
    public static Try<V> SelectMany<A, U, V>(
        this Try<A> self,
        Func<A, Try<U>> bind,
        Func<A, U, V> project) => () =>
        {
            var resT = self();
            return project(resT.Value, bind(resT.Value)().Value);
        };

    [Pure]
    public static IEnumerable<V> SelectMany<A, U, V>(
        this Try<A> self, 
        Func<A, IEnumerable<U>> bind,
        Func<A, U, V> project
        )
    {
        var resT = self.Try();
        if (resT.IsFaulted) return new V[0];
        return bind(resT.Value).Map(resU => project(resT.Value, resU));
    }

    [Pure]
    public static Try<V> Join<A, U, K, V>(
        this Try<A> self, 
        Try<U> inner,
        Func<A, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<A, U, V> project) => () =>
        {
            var selfRes = self();
            var innerRes = inner();
            return EqualityComparer<K>.Default.Equals(outerKeyMap(selfRes.Value), innerKeyMap(innerRes.Value))
                ? project(selfRes.Value, innerRes.Value)
                : throw new BottomException();
        };

    [Pure]
    public static Result<T> Try<T>(this Try<T> self)
    {
        try
        {
            if (self == null)
            {
                throw new ArgumentNullException("this is null");
            }
            return self();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new Result<T>(e);
        }
    }

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, U> select)
        where T : IDisposable => () =>
            {
                var t = default(T);
                try
                {
                    return select(self().Value);
                }
                finally
                {
                    t?.Dispose();
                }
            };

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, Try<U>> select)
        where T : IDisposable => () =>
        {
            var t = default(T);
            try
            {
                t = self().Value;
                return select(t)().Value;
            }
            finally
            {
                t?.Dispose();
            }
        };

    [Pure]
    public static int Sum(this Try<int> self) =>
        self.Try().Value;

    [Pure]
    public static Task<int> SumAsync(this Try<int> self) =>
        self.ToAsync().Sum();

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, R> Succ, Func<Exception, R> Fail) =>
        self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        });

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, IObservable<R>> Succ, Func<Exception, R> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Observable.Return(Fail(res.Exception))
                : Succ(res.Value);
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<Try<T>> self, Func<T, R> Succ, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Observable.Return(Succ(res.Value));
        })
        from t in tt
        select t;

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<T>> self) =>
        from x in self
        from y in x
        select y;

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<Try<T>>> self) =>
        from x in self
        from y in x
        from z in y
        select z;

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<Try<Try<T>>>> self) =>
        from w in self
        from x in w
        from y in x
        from z in y
        select z;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Try<B> Apply<A, B>(this Try<Func<A, B>> fab, Try<A> fa) =>
        FTry<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static Try<C> Apply<A, B, C>(this Try<Func<A, B, C>> fabc, Try<A> fa, Try<B> fb) =>
        fabc.Bind(f => FTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa, fb));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Try<Func<A, B, C>> fabc, Try<A> fa) =>
        fabc.Bind(f => FTry<A, B, C>.Inst.Apply(MTry<Func<A, Func<B, C>>>.Inst.Return(curry(f)), fa));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Try<Func<A, Func<B, C>>> fabc, Try<A> fa) =>
        FTry<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static Try<B> Action<A, B>(this Try<A> fa, Try<B> fb) =>
        FTry<A, B>.Inst.Action(fa, fb);

    /// <summary>
    /// Compare the bound value of Try(x) to Try(y).  If either of the
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>1 if lhs > rhs, 0 if lhs == rhs, -1 if lhs < rhs</returns>
    [Pure]
    public static int Compare<ORD, A>(this Try<A> lhs, Try<A> rhs) where ORD : struct, Ord<A>
    {
        var x = lhs.Try();
        var y = lhs.Try();
        if (x.IsFaulted && y.IsFaulted) return 0;
        if (x.IsFaulted && !y.IsFaulted) return -1;
        if (!x.IsFaulted && y.IsFaulted) return 1;
        return default(ORD).Compare(x.Value, y.Value);
    }

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
    /// Add the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Add<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select plus<NUM, A>(x, y);

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
    public static Try<A> Subtract<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select subtract<NUM, A>(x, y);

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
    public static Try<A> Product<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select product<NUM, A>(x, y);

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
    public static Try<A> Divide<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select divide<NUM, A>(x, y);

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
    public static A? ToNullable<A>(this Try<A> ma) where A : struct
    {
        var x = ma.Try();
        return x.IsFaulted
            ? (A?)null
            : x.Value;
    }

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