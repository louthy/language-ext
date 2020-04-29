using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.DataTypes.Serialisation;

/// <summary>
/// Extension methods for the TryAsync monad
/// </summary>
public static class TryAsyncExtensions
{
    /// <summary>
    /// Use for pattern-matching the case of the target
    /// </summary>
    [Pure]
    public static async Task<TryCase<A>> Case<A>(this TryAsync<A> ma)
    {
        if (ma == null) return FailCase<A>.New(Error.Bottom);
        var res = await ma.Try();
        return res.IsSuccess
            ? SuccCase<A>.New(res.Value)
            : FailCase<A>.New(res.Exception);
    }

    /// <summary>
    /// Memoize the computation so that it's only run once
    /// </summary>
    public static TryAsync<A> Memo<A>(this TryAsync<A> ma)
    {
        bool run = false;
        var result = Result<A>.Bottom.AsTask();
        return new TryAsync<A>(() =>
        {
            if (run) return result;
            result = ma.Try();
            run = true;
            return result;
        });
    }

    /// <summary>
    /// Forces evaluation of the lazy TryAsync
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Computation to evaluate</param>
    /// <returns>The Try with the computation executed</returns>
    public static TryAsync<A> Strict<A>(this TryAsync<A> ma)
    {
        var res = ma.Try();
        return () => res;
    }

    /// <summary>
    /// Test if the TryAsync is in a success state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Computation to evaluate</param>
    /// <returns>True if computation has succeeded</returns>
    public static async Task<bool> IsSucc<A>(this TryAsync<A> ma) =>
        (await ma.Try()).IsSuccess;

    /// <summary>
    /// Test if the TryAsync is in a faulted state
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Computation to evaluate</param>
    /// <returns>True if computation is faulted</returns>
    public static async Task<bool> IsFail<A>(this TryAsync<A> ma) =>
        (await ma.Try()).IsFaulted;

    /// <summary>
    /// Invoke a delegate if the Try returns a value successfully
    /// </summary>
    /// <param name="Succ">Delegate to invoke if successful</param>
    public static async Task<Unit> IfSucc<A>(this TryAsync<A> self, Action<A> Succ)
    {
        if (isnull(self)) throw new ArgumentNullException(nameof(self));
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));

        try
        {
            var res = await self.Try();
            if (!res.IsFaulted)
            {
                Succ(res.Value);
            }
            return unit;
        }
        catch(Exception e)
        {
            TryConfig.ErrorLogger(e);
            return unit;
        }
    }

    /// <summary>
    /// Return a default value if the Try fails
    /// </summary>
    /// <param name="failValue">Default value to use on failure</param>
    /// <returns>failValue on failure, the result of the Try otherwise</returns>
    [Pure]
    public static async Task<A> IfFail<A>(this TryAsync<A> self, A failValue)
    {
        if (isnull(self)) throw new ArgumentNullException(nameof(self));
        if (isnull(failValue)) throw new ArgumentNullException(nameof(failValue));

        try
        {
            var res = await self.Try();
            if (res.IsFaulted)
                return failValue;
            else
                return res.Value;
        }
        catch(Exception e)
        {
            TryConfig.ErrorLogger(e);
            return failValue;
        }
    }

    /// <summary>
    /// Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="Fail">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static async Task<A> IfFail<A>(this TryAsync<A> self, Func<Task<A>> Fail)
    {
        if (isnull(self)) throw new ArgumentNullException(nameof(self));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        try
        {
            var res = await self.Try();
            if (res.IsFaulted)
                return await Fail();
            else
                return res.Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return await Fail();
        }
    }

    /// <summary>
    /// Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="Fail">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static async Task<A> IfFail<A>(this TryAsync<A> self, Func<A> Fail)
    {
        if (isnull(self)) throw new ArgumentNullException(nameof(self));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        try
        {
            var res = await self.Try();
            if (res.IsFaulted)
                return Fail();
            else
                return res.Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return Fail();
        }
    }

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static async Task<A> IfFail<A>(this TryAsync<A> self, Func<Exception, Task<A>> Fail)
    {
        if (isnull(self)) throw new ArgumentNullException(nameof(self));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        try
        {
            var res = await self.Try();
            if (res.IsFaulted)
                return await Fail(res.Exception);
            else
                return res.Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return await Fail(e);
        }
    }

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static async Task<A> IfFail<A>(this TryAsync<A> self, Func<Exception, A> Fail)
    {
        if (isnull(self)) throw new ArgumentNullException(nameof(self));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        try
        {
            var res = await self.Try();
            if (res.IsFaulted)
                return Fail(res.Exception);
            else
                return res.Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return Fail(e);
        }
    }

    /// <summary>
    /// Provides a fluent exception matching interface which is invoked
    /// when the Try fails.
    /// </summary>
    /// <returns>Fluent exception matcher</returns>
    [Pure]
    public static ExceptionMatchAsync<A> IfFail<A>(this TryAsync<A> self) =>
        new ExceptionMatchAsync<A>(self.Try());

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static async Task<R> Match<A, R>(this TryAsync<A> self, Func<A, R> Succ, Func<Exception, R> Fail)
    {
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = await self.Try();
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
    public static async Task<R> Match<A, R>(this TryAsync<A> self, Func<A, Task<R>> Succ, Func<Exception, R> Fail)
    {
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = await self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : await Succ(res.Value);
    }

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static async Task<R> Match<A, R>(this TryAsync<A> self, Func<A, R> Succ, Func<Exception, Task<R>> Fail)
    {
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = await self.Try();
        return res.IsFaulted
            ? await Fail(res.Exception)
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
    public static async Task<R> Match<A, R>(this TryAsync<A> self, Func<A, Task<R>> Succ, Func<Exception, Task<R>> Fail)
    {
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = await self.Try();
        return res.IsFaulted
            ? await Fail(res.Exception)
            : await Succ(res.Value);
    }

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Default value to use if the Try computation fails</param>
    /// <returns>The result of either the Succ delegate or the Fail value</returns>
    [Pure]
    public static async Task<R> Match<A, R>(this TryAsync<A> self, Func<A, R> Succ, R Fail)
    {
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = await self.Try();
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
    public static async Task<R> Match<A, R>(this TryAsync<A> self, Func<A, Task<R>> Succ, R Fail)
    {
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = await self.Try();
        return res.IsFaulted
            ? Fail
            : await Succ(res.Value);
    }

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    public static async Task<Unit> Match<A>(this TryAsync<A> self, Action<A> Succ, Action<Exception> Fail)
    {
        if (isnull(Succ)) throw new ArgumentNullException(nameof(Succ));
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = await self.Try();

        if (res.IsFaulted)
            Fail(res.Exception);
        else
            Succ(res.Value);

        return unit;
    }


    [Pure]
    public static Task<Validation<FAIL, A>> ToValidation<A, FAIL>(this TryAsync<A> self, Func<Exception, FAIL> Fail) =>
        self.Match(
            Succ: v => Success<FAIL, A>(v),
            Fail: e => Fail<FAIL, A>(Fail(e)));

    [Pure]
    public static Task<Option<A>> ToOption<A>(this TryAsync<A> self) =>
        self.Match(
              Succ: v => Some(v),
              Fail: _ => Option<A>.None);

    [Pure]
    public static Task<OptionUnsafe<A>> ToOptionUnsafe<A>(this TryAsync<A> self) =>
        self.Match(
              Succ: v => OptionUnsafe<A>.Some(v),
              Fail: _ => OptionUnsafe<A>.None);

    [Pure]
    public static TryOptionAsync<A> ToTryOption<A>(this TryAsync<A> self) =>
        async () => (await self.Try()).ToOptional();

    [Pure]
    public static Task<EitherUnsafe<Error, A>> ToEitherUnsafe<A>(this TryAsync<A> self) =>
        self.Match(
              Succ: v => EitherUnsafe<Error, A>.Right(v),
              Fail: x => EitherUnsafe<Error, A>.Left(Error.New(x)));

    [Pure]
    public static Task<EitherUnsafe<L, A>> ToEitherUnsafe<A, L>(this TryAsync<A> self, Func<Error, L> Fail) =>
        self.Match(
              Succ: v => EitherUnsafe<L, A>.Right(v),
              Fail: x => EitherUnsafe<L, A>.Left(Fail(Error.New(x))));

    [Pure]
    public static EitherAsync<Error, A> ToEither<A>(this TryAsync<A> self) => new EitherAsync<Error, A>(
        self.Match(
              Succ: v => EitherData.Right<Error, A>(v),
              Fail: x => EitherData.Left<Error, A>(Error.New(x))));

    [Pure]
    public static EitherAsync<L, A> ToEither<A, L>(this TryAsync<A> self, Func<Error, L> Fail) => new EitherAsync<L, A>(
        self.Match(
              Succ: v => EitherData.Right<L, A>(v),
              Fail: x => EitherData.Left<L, A>(Fail(Error.New(x)))));

    [Pure]
    public static async Task<A> IfFailThrow<A>(this TryAsync<A> self)
    {
        try
        {
            var res = await self.Try();
            if (res.IsBottom) throw new BottomException();
            if (res.IsFaulted) throw new InnerException(res.Exception);
            return res.Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            throw;
        }
    }

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="select">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> Select<A, B>(this TryAsync<A> self, Func<A, B> select) =>
        Map(self, select);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="select">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> Select<A, B>(this TryAsync<A> self, Func<A, Task<B>> select) =>
        MapAsync(self, select);


    /// <summary>
    /// Apply Try values to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg1">Try argument</param>
    /// <param name="arg2">Try argument</param>
    /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
    public static Task<Unit> Iter<A>(this TryAsync<A> self, Action<A> action) =>
        IfSucc(self, action);

    /// <summary>
    /// Counts the number of bound values.  
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">TrTry computation</param>
    /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
    [Pure]
    public static Task<int> Count<A>(this TryAsync<A> self) =>
        Map(self, _ => 1).IfFail(0);

    /// <summary>
    /// Tests that a predicate holds for all values of the bound value T
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value, or if the Try computation
    /// fails.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ForAll<A>(this TryAsync<A> self, Func<A, bool> pred) =>
        Map(self, pred).IfFail(true);

    /// <summary>
    /// Tests that a predicate holds for all values of the bound value T
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value, or if the Try computation
    /// fails.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ForAllAsync<A>(this TryAsync<A> self, Func<A, Task<bool>> pred) =>
        MapAsync(self, pred).IfFail(true);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> Fold<A, S>(this TryAsync<A> self, S state, Func<S, A, S> folder) =>
        Map(self, v => folder(state, v)).IfFail(state);

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static Task<S> FoldAsync<A, S>(this TryAsync<A> self, S state, Func<S, A, Task<S>> folder) =>
        MapAsync(self, v => folder(state, v)).IfFail(state);

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
    public static Task<S> BiFold<A, S>(this TryAsync<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, S> Fail) =>
        BiMap(self,
            Succ: v => Succ(state, v),
            Fail: x => Fail(state, x)).IfFail(state);

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
    public static Task<S> BiFold<A, S>(this TryAsync<A> self, S state, Func<S, A, S> Succ, Func<S, Exception, Task<S>> Fail) =>
        BiMap(self,
            Succ: v => Succ(state, v),
            Fail: x => Fail(state, x)).IfFail(state);


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
    public static Task<S> BiFold<A, S>(this TryAsync<A> self, S state, Func<S, A, Task<S>> Succ, Func<S, Exception, S> Fail) =>
        BiMap(self,
            Succ: v => Succ(state, v),
            Fail: x => Fail(state, x)).IfFail(state);

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
    public static Task<S> BiFold<A, S>(this TryAsync<A> self, S state, Func<S, A, Task<S>> Succ, Func<S, Exception, Task<S>> Fail) =>
        BiMap(self,
            Succ: v => Succ(state, v),
            Fail: x => Fail(state, x)).IfFail(state);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> Exists<A>(this TryAsync<A> self, Func<A, bool> pred) =>
        self.Map(pred).IfFail(false);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ExistsAsync<A>(this TryAsync<A> self, Func<A, Task<bool>> pred) =>
        self.MapAsync(pred).IfFail(false);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static TryAsync<A> Do<A>(this TryAsync<A> ma, Action<A> f) => new TryAsync<A>(async () =>
    {
        var r = await ma.Try();
        if (!r.IsFaulted)
        {
            f(r.Value);
        }
        return r;
    });

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> Map<A, B>(this TryAsync<A> self, Func<A, B> f) =>
        Memo(async () => (await self.Try()).Map(f));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> MapAsync<A, B>(this TryAsync<A> self, Func<A, Task<B>> f)  =>
        Memo(async () => await (await self.Try()).MapAsync(f));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> BiMap<A, B>(this TryAsync<A> self, Func<A, B> Succ, Func<Exception, B> Fail) => Memo<B>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    });

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> BiMap<A, B>(this TryAsync<A> self, Func<A, Task<B>> Succ, Func<Exception, B> Fail) => Memo<B>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : await Succ(res.Value);
    });

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> BiMap<A, B>(this TryAsync<A> self, Func<A, B> Succ, Func<Exception, Task<B>> Fail) => Memo<B>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? await Fail(res.Exception)
            : Succ(res.Value);
    });

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="Succ">Delegate to map the bound value</param>
    /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static TryAsync<B> BiMap<A, B>(this TryAsync<A> self, Func<A, Task<B>> Succ, Func<Exception, Task<B>> Fail) => Memo<B>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? await Fail(res.Exception)
            : await Succ(res.Value);
    });

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static TryAsync<Func<B, R>> ParMap<A, B, R>(this TryAsync<A> self, Func<A, B, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static TryAsync<Func<B, Func<C, R>>> ParMap<A, B, C, R>(this TryAsync<A> self, Func<A, B, C, R> func) =>
        self.Map(curry(func));

    [Pure]
    public static TryAsync<A> Filter<A>(this TryAsync<A> self, Func<A, bool> pred) => Memo<A>(async () =>
    {
        var res = await self.Try();
        if (res.IsFaulted) return res;
        return pred(res.Value)
            ? res
            : raise<A>(new BottomException());
    });

    [Pure]
    public static TryAsync<A> Filter<A>(this TryAsync<A> self, Func<A, Task<bool>> pred) => Memo(async () =>
    {
        var res = await self.Try();
        if (res.IsFaulted) return res;
        return await pred(res.Value)
            ? res
            : raise<A>(new BottomException());
    });

    [Pure]
    public static TryAsync<A> BiFilter<A>(this TryAsync<A> self, Func<A, bool> Succ, Func<Exception, bool> Fail) => Memo<A>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
                ? res.Value
                : raise<A>(new BottomException())
            : Succ(res.Value)
                ? res.Value
                : raise<A>(new BottomException());
    });

    [Pure]
    public static TryAsync<A> BiFilter<A>(this TryAsync<A> self, Func<A, Task<bool>> Succ, Func<Exception, bool> Fail) => Memo<A>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
                ? res.Value
                : raise<A>(new BottomException())
            : await Succ(res.Value)
                ? res.Value
                : raise<A>(new BottomException());
    });


    [Pure]
    public static TryAsync<A> BiFilter<A>(this TryAsync<A> self, Func<A, bool> Succ, Func<Exception, Task<bool>> Fail) => Memo<A>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? await Fail(res.Exception)
                ? res.Value
                : raise<A>(new BottomException())
            : Succ(res.Value)
                ? res.Value
                : raise<A>(new BottomException());
    });

    [Pure]
    public static TryAsync<A> BiFilter<A>(this TryAsync<A> self, Func<A, Task<bool>> Succ, Func<Exception, Task<bool>> Fail) => Memo<A>(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? await Fail(res.Exception)
                ? res.Value
                : raise<A>(new BottomException())
            : await Succ(res.Value)
                ? res.Value
                : raise<A>(new BottomException());
    });

    [Pure]
    public static TryAsync<A> Where<A>(this TryAsync<A> self, Func<A, bool> pred) =>
        self.Filter(pred);

    [Pure]
    public static TryAsync<A> Where<A>(this TryAsync<A> self, Func<A, Task<bool>> pred) =>
        self.Filter(pred);

    [Pure]
    public static TryAsync<B> Bind<A, B>(this TryAsync<A> ma, Func<A, TryAsync<B>> f) => Memo(async () =>
    {
        try
        {
            var ra = await ma();
            if(ra.IsSuccess)
            {
                return await f(ra.Value)();
            }
            else
            {
                return new Result<B>(ra.Exception);
            }
        }
        catch(Exception e)
        {
            return new Result<B>(e);
        }
    });

    [Pure]
    public static TryAsync<B> BindAsync<A, B>(this TryAsync<A> ma, Func<A, Task<TryAsync<B>>> f) => Memo(async () =>
    {
        try
        {
            var ra = await ma();
            if (ra.IsSuccess)
            {
                return await (await f(ra.Value))();
            }
            else
            {
                return new Result<B>(ra.Exception);
            }
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new Result<B>(e);
        }
    });

    [Pure]
    public static TryAsync<R> BiBind<A, R>(this TryAsync<A> self, Func<A, TryAsync<R>> Succ, Func<Exception, TryAsync<R>> Fail) => Memo(async () =>
    {
        var res = await self.Try();
        return res.IsFaulted
            ? await Fail(res.Exception).Try()
            : await Succ(res.Value).Try();
    });

    [Pure]
    public static TryAsync<A> Plus<A>(this TryAsync<A> ma, TryAsync<A> mb) =>
        default(MTryAsync<A>).Plus(ma, mb);

    [Pure]
    public static TryAsync<A> PlusFirst<A>(this TryAsync<A> ma, TryAsync<A> mb) =>
        default(MTryFirstAsync<A>).Plus(ma, mb);

    [Pure]
    public static Task<Seq<A>> ToSeq<A>(this TryAsync<A> self) =>
        self.Match(
            Succ: v => v.Cons(Empty),
            Fail: x => Empty);

    [Pure]
    public static Task<Seq<A>> AsEnumerable<A>(this TryAsync<A> self) =>
        self.Match(
            Succ: v => v.Cons(Empty),
            Fail: x => Empty);

    [Pure]
    public static async Task<Lst<A>> ToList<A>(this TryAsync<A> self) =>
        toList(await self.AsEnumerable());

    [Pure]
    public static async Task<Arr<A>> ToArray<A>(this TryAsync<A> self) =>
        toArray(await self.AsEnumerable());

    [Pure]
    public static TryAsyncSuccContext<A, R> Succ<A,R>(this TryAsync<A> self, Func<A, R> succHandler) =>
        new TryAsyncSuccContext<A, R>(self, succHandler);

    [Pure]
    public static TryAsyncSuccUnitContext<A> Succ<A>(this TryAsync<A> self, Action<A> succHandler) =>
        new TryAsyncSuccUnitContext<A>(self, succHandler);

    [Pure]
    public static Task<string> AsString<A>(this TryAsync<A> self) =>
        self.Match(
            Succ: v => isnull(v)
                      ? "Succ(null)"
                      : $"Succ({v})",
            Fail: ex => $"Fail({ex.Message})");

    [Pure]
    public static TryAsync<C> SelectMany<A, B, C>(
        this TryAsync<A> ma,
        Func<A, TryAsync<B>> bind,
        Func<A, B, C> project) => Memo(async () =>
        {
            try
            {
                var ra = await ma();
                if (ra.IsSuccess)
                {
                    var mb = bind(ra.Value);
                    var rb = await mb();

                    if (rb.IsSuccess)
                    {
                        return new Result<C>(project(ra.Value, rb.Value));
                    }
                    else
                    {
                        return new Result<C>(rb.Exception);
                    }
                }
                else
                {
                    return new Result<C>(ra.Exception);
                }
            }
            catch (Exception e)
            {
                return new Result<C>(e);
            }
        });

    [Pure]
    public static TryAsync<C> SelectMany<A, B, C>(
        this TryAsync<A> ma,
        Func<A, Task<TryAsync<B>>> bind,
        Func<A, B, C> project) => Memo(async () =>
        {
            try
            {
                var ra = await ma();
                if (ra.IsSuccess)
                {
                    var mb = await bind(ra.Value);
                    var rb = await mb();

                    if (rb.IsSuccess)
                    {
                        return new Result<C>(project(ra.Value, rb.Value));
                    }
                    else
                    {
                        return new Result<C>(rb.Exception);
                    }
                }
                else
                {
                    return new Result<C>(ra.Exception);
                }
            }
            catch (Exception e)
            {
                return new Result<C>(e);
            }
        });

    [Pure]
    public static TryAsync<C> SelectMany<A, B, C>(
        this TryAsync<A> ma,
        Func<A, Task<TryAsync<B>>> bind,
        Func<A, B, Task<C>> project) => Memo(async () =>
        {
            try
            {
                var ra = await ma();
                if (ra.IsSuccess)
                {
                    var mb = await bind(ra.Value);
                    var rb = await mb();

                    if (rb.IsSuccess)
                    {
                        return new Result<C>(await project(ra.Value, rb.Value));
                    }
                    else
                    {
                        return new Result<C>(rb.Exception);
                    }
                }
                else
                {
                    return new Result<C>(ra.Exception);
                }
            }
            catch (Exception e)
            {
                return new Result<C>(e);
            }
        });

    [Pure]
    public static TryAsync<C> SelectMany<A, B, C>(
        this TryAsync<A> ma,
        Func<A, TryAsync<B>> bind,
        Func<A, B, Task<C>> project) => Memo(async () =>
        {
            try
            {
                var ra = await ma();
                if (ra.IsSuccess)
                {
                    var mb = bind(ra.Value);
                    var rb = await mb();

                    if (rb.IsSuccess)
                    {
                        return new Result<C>(await project(ra.Value, rb.Value));
                    }
                    else
                    {
                        return new Result<C>(rb.Exception);
                    }
                }
                else
                {
                    return new Result<C>(ra.Exception);
                }
            }
            catch (Exception e)
            {
                return new Result<C>(e);
            }
        });

    [Pure]
    public static TryAsync<V> Join<A, U, K, V>(
        this TryAsync<A> self,
        TryAsync<U> inner,
        Func<A, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<A, U, V> project) =>
            Memo<V>(async () =>
            {
                var selfTask = self.Try();
                var innerTask = inner.Try();
                await Task.WhenAll(selfTask, innerTask);

                if (selfTask.IsFaulted) return new Result<V>(selfTask.Exception);
                if (selfTask.Result.IsFaulted) return new Result<V>(selfTask.Result.Exception);
                if (innerTask.IsFaulted) return new Result<V>(innerTask.Exception);
                if (innerTask.Result.IsFaulted) return new Result<V>(innerTask.Result.Exception);
                return EqualityComparer<K>.Default.Equals(outerKeyMap(selfTask.Result.Value), innerKeyMap(innerTask.Result.Value))
                    ? project(selfTask.Result.Value, innerTask.Result.Value)
                    : throw new BottomException();
            });

    [Pure]
    public static async Task<Result<T>> Try<T>(this TryAsync<T> self)
    {
        try
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            try
            {
                return await self();
            }
            catch(Exception e)
            {
                TryConfig.ErrorLogger(e);
                return new Result<T>(e);
            }
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new Result<T>(e);
        }
    }

    [Pure]
    public static TryAsync<U> Use<T, U>(this TryAsync<T> self, Func<T, U> select)
        where T : IDisposable => async () =>
            {
                var t = default(T);
                try
                {
                    var res = await self.Try();
                    if (res.IsFaulted) return new Result<U>(res.Exception);
                    t = res.Value;
                    return select(t);
                }
                finally
                {
                    t?.Dispose();
                }
            };

    [Pure]
    public static TryAsync<U> Use<T, U>(this TryAsync<T> self, Func<T, TryAsync<U>> select)
        where T : IDisposable => async () =>
        {
            var t = default(T);
            try
            {
                var res = await self.Try();
                if (res.IsFaulted) return default(U);
                t = res.Value;
                return await select(t).Try();
            }
            finally
            {
                t?.Dispose();
            }
        };

    [Pure]
    public static Task<int> Sum(this TryAsync<int> self) =>
        from x in self.Try()
        select x.IfFail(0);

    [Pure]
    public static TryAsync<T> Flatten<T>(this TryAsync<TryAsync<T>> self) =>
        from x in self
        from y in x
        select y;

    [Pure]
    public static TryAsync<T> Flatten<T>(this TryAsync<TryAsync<TryAsync<T>>> self) =>
        from x in self
        from y in x
        from z in y
        select z;

    [Pure]
    public static TryAsync<T> Flatten<T>(this TryAsync<TryAsync<TryAsync<TryAsync<T>>>> self) =>
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
    public static TryAsync<B> Apply<A, B>(this TryAsync<Func<A, B>> fab, TryAsync<A> fa) =>
        ApplTryAsync<A, B>.Inst.Apply(fab, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static TryAsync<B> Apply<A, B>(this Func<A, B> fab, TryAsync<A> fa) =>
        ApplTryAsync<A, B>.Inst.Apply(TryAsync(fab), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static TryAsync<C> Apply<A, B, C>(this TryAsync<Func<A, B, C>> fabc, TryAsync<A> fa, TryAsync<B> fb) =>
        fabc.Bind(f => ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(f).AsTask()), fa, fb));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static TryAsync<C> Apply<A, B, C>(this Func<A, B, C> fabc, TryAsync<A> fa, TryAsync<B> fb) =>
        ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(fabc).AsTask()), fa, fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static TryAsync<Func<B, C>> Apply<A, B, C>(this TryAsync<Func<A, B, C>> fabc, TryAsync<A> fa) =>
        fabc.Bind(f => ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(f).AsTask()), fa));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static TryAsync<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, TryAsync<A> fa) =>
        ApplTryAsync<A, B, C>.Inst.Apply(MTryAsync<Func<A, Func<B, C>>>.Inst.ReturnAsync(curry(fabc).AsTask()), fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static TryAsync<Func<B, C>> Apply<A, B, C>(this TryAsync<Func<A, Func<B, C>>> fabc, TryAsync<A> fa) =>
        ApplTryAsync<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static TryAsync<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, TryAsync<A> fa) =>
        ApplTryAsync<A, B, C>.Inst.Apply(TryAsync(fabc), fa);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static TryAsync<B> Action<A, B>(this TryAsync<A> fa, TryAsync<B> fb) =>
        ApplTryAsync<A, B>.Inst.Action(fa, fb);

    /// <summary>
    /// Compare the bound value of Try(x) to Try(y).  If either of the
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>1 if lhs > rhs, 0 if lhs == rhs, -1 if lhs < rhs</returns>
    [Pure]
    public static async Task<int> Compare<ORD, A>(this TryAsync<A> lhs, TryAsync<A> rhs) where ORD : struct, Ord<A> 
    {
        var x = lhs.Try();
        var y = rhs.Try();
        await Task.WhenAll(x, y);

        if (x.IsFaulted && y.IsFaulted) return 0;
        if (x.IsFaulted && !y.IsFaulted) return -1;
        if (!x.IsFaulted && y.IsFaulted) return 1;
        if (x.Result.IsFaulted && y.Result.IsFaulted) return 0;
        if (x.Result.IsFaulted && !y.Result.IsFaulted) return -1;
        if (!x.Result.IsFaulted && y.Result.IsFaulted) return 1;
        return default(ORD).Compare(x.Result.Value, y.Result.Value);
    }

    /// <summary>
    /// Append the bound value of TryAsync(x) to TryAsync(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs ++ rhs</returns>
    [Pure]
    public static TryAsync<A> Append<SEMI, A>(this TryAsync<A> lhs, TryAsync<A> rhs) where SEMI : struct, Semigroup<A> => Memo(async () =>
    {
        var x = lhs.Try();
        var y = rhs.Try();
        await Task.WhenAll(x, y);
        if (x.IsFaulted || x.Result.IsFaulted) return x.Result;
        if (y.IsFaulted || y.Result.IsFaulted) return y.Result;
        return append<SEMI, A>(x.Result.Value, y.Result.Value);
    });

    /// <summary>
    /// Add the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Add<NUM, A>(this TryAsync<A> lhs, TryAsync<A> rhs) where NUM : struct, Num<A> => Memo(async () =>
    {
        var x = lhs.Try();
        var y = rhs.Try();
        await Task.WhenAll(x, y);
        if (x.IsFaulted || x.Result.IsFaulted) return x.Result;
        if (y.IsFaulted || y.Result.IsFaulted) return y.Result;
        return plus<NUM, A>(x.Result.Value, y.Result.Value);
    });

    /// <summary>
    /// Find the subtract of the bound value of Try(x) and Try(y).  If either of 
    /// the Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Subtract<NUM, A>(this TryAsync<A> lhs, TryAsync<A> rhs) where NUM : struct, Num<A> => Memo(async () =>
    {
        var x = lhs.Try();
        var y = rhs.Try();
        await Task.WhenAll(x, y);
        if (x.IsFaulted || x.Result.IsFaulted) return x.Result;
        if (y.IsFaulted || y.Result.IsFaulted) return y.Result;
        return subtract<NUM, A>(x.Result.Value, y.Result.Value);
    });

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Product<NUM, A>(this TryAsync<A> lhs, TryAsync<A> rhs) where NUM : struct, Num<A> => Memo(async () =>
    {
        var x = lhs.Try();
        var y = rhs.Try();
        await Task.WhenAll(x, y);
        if (x.IsFaulted || x.Result.IsFaulted) return x.Result;
        if (y.IsFaulted || y.Result.IsFaulted) return y.Result;
        return product<NUM, A>(x.Result.Value, y.Result.Value);
    });

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Divide<NUM, A>(this TryAsync<A> lhs, TryAsync<A> rhs) where NUM : struct, Num<A> => Memo(async () =>
    {
        var x = lhs.Try();
        var y = rhs.Try();
        await Task.WhenAll(x, y);
        if (x.IsFaulted || x.Result.IsFaulted) return x.Result;
        if (y.IsFaulted || y.Result.IsFaulted) return y.Result;
        return divide<NUM, A>(x.Result.Value, y.Result.Value);
    });

    /// <summary>
    /// Convert the Try type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Try to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static async Task<A?> ToNullable<A>(this TryAsync<A> ma) where A : struct
    {
        var x = await ma.Try();
        return x.IsFaulted
            ? (A?)null
            : x.Value;
    }
}
