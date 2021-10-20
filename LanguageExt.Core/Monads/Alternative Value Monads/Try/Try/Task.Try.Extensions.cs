using System;
using LanguageExt;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.Common;

public static class TaskTryExtensions
{
    /// <summary>
    /// Convert a Task<Try<A>> to a TryAsync<A>
    /// </summary>
    [Pure]
    public static TryAsync<A> ToAsync<A>(this Task<Try<A>> self) =>
        async () =>
        {
            try
            {
                var resT = await self.ConfigureAwait(false);
                return resT.Try();
            }
            catch (Exception e)
            {
                return new Result<A>(e);
            }
        };

    /// <summary>
    /// Convert a Task to a TryAsync<Unit>
    /// </summary>
    [Pure]
    public static TryAsync<Unit> ToAsync(this Try<Task> self) =>
        async () =>
        {
            try
            {
                var task = self.Try();
                await task.Value.ConfigureAwait(false);
                return new Result<Unit>(Unit.Default);
            }
            catch (Exception e)
            {
                return new Result<Unit>(e);
            }
        };

    /// <summary>
    /// Convert a Task<Try<A>> to a TryAsync<A>
    /// </summary>
    [Pure]
    public static TryAsync<A> ToAsync<A>(this Try<Task<A>> self) =>
        async () =>
        {
            try
            {
                var task = self.Try();
                return await task.Value.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return new Result<A>(e);
            }
        };

    /// <summary>
    /// Convert the structure to an Eff
    /// </summary>
    /// <returns>An Eff representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Eff<A> ToEff<A>(this Try<A> ma) =>
        Prelude.EffMaybe(() =>
            ma.Match(Succ: Fin<A>.Succ, 
                     Fail: e => Fin<A>.Fail(e)));

    /// <summary>
    /// Convert the structure to an Aff
    /// </summary>
    /// <returns>An Aff representation of the structure</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Aff<A> ToAff<A>(this Try<A> ma) =>
        ToEff(ma);

    /// <summary>
    /// Invoke a delegate if the Try returns a value successfully
    /// </summary>
    /// <param name="Succ">Delegate to invoke if successful</param>
    public static Task<Unit> IfSucc<A>(this Task<Try<A>> self, Action<A> Succ) =>
        self.ToAsync().IfSucc(Succ);

    /// <summary>
    /// Return a default value if the Try fails
    /// </summary>
    /// <param name="failValue">Default value to use on failure</param>
    /// <returns>failValue on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfFail<A>(this Task<Try<A>> self, A failValue) =>
        self.ToAsync().IfFail(failValue);

    /// <summary>
    /// Invoke a delegate if the Try fails
    /// </summary>
    /// <param name="Fail">Delegate to invoke on failure</param>
    /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
    [Pure]
    public static Task<A> IfFail<A>(this Task<Try<A>> self, Func<A> Fail) =>
        self.ToAsync().IfFail(Fail);

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static Task<A> IfFail<A>(this Task<Try<A>> self, Func<Exception, A> Fail) =>
        self.ToAsync().IfFail(Fail);

    /// <summary>
    /// Provides a fluent exception matching interface which is invoked
    /// when the Try fails.
    /// </summary>
    /// <returns>Fluent exception matcher</returns>
    [Pure]
    public static ExceptionMatchAsync<A> IfFail<A>(this Task<Try<A>> self) =>
        self.ToAsync().IfFail();

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<Try<A>> self, Func<A, R> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Default value to use if the Try computation fails</param>
    /// <returns>The result of either the Succ delegate or the Fail value</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<Try<A>> self, Func<A, R> Succ, R Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    public static Task<Unit> Match<A>(this Task<Try<A>> self, Action<A> Succ, Action<Exception> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<Try<A>> self, Func<A, Task<R>> Succ, Func<Exception, R> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<Try<A>> self, Func<A, Task<R>> Succ, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Pattern matches the two possible states of the Try computation
    /// </summary>
    /// <typeparam name="R">Type of the resulting bound value</typeparam>
    /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
    /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
    /// <returns>The result of either the Succ or Fail delegates</returns>
    [Pure]
    public static Task<R> Match<A, R>(this Task<Try<A>> self, Func<A, R> Succ, Func<Exception, Task<R>> Fail) =>
        self.ToAsync().Match(Succ, Fail);

    /// <summary>
    /// Memoise the try
    /// </summary>
    [Pure]
    public static TryAsync<A> Memo<A>(this Task<Try<A>> self) =>
        self.ToAsync().Memo();

    [Pure]
    public static Task<Option<A>> ToOption<A>(this Task<Try<A>> self) =>
        self.ToAsync().ToOption();

    [Pure]
    public static TryOptionAsync<A> ToTryOption<A>(this Task<Try<A>> self) =>
      self.ToAsync().ToTryOption();

    [Pure]
    public static Task<A> IfFailThrow<A>(this Task<Try<A>> self) =>
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
    public static Task<Try<B>> Select<A, B>(this Task<Try<A>> self, Func<A, B> f) =>
        from x in self.ToAsync().Select(f).Try()
        select new Try<B>(() => x);

    /// <summary>
    /// Map the bound value from A to Task of B
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">Returned bound value type</typeparam>
    /// <param name="self">This</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Returns an asynchronous Try</returns>
    [Pure]
    public static Task<Try<B>> Select<A, B>(this Task<Try<A>> self, Func<A, Task<B>> f) =>
        from x in self.ToAsync().Select(f).Try()
        select new Try<B>(() => x);

    /// <summary>
    /// Apply Try values to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg1">Try argument</param>
    /// <param name="arg2">Try argument</param>
    /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
    public static Task<Unit> Iter<A>(this Task<Try<A>> self, Action<A> action) =>
        self.IfSucc(action);

    /// <summary>
    /// Counts the number of bound values.  
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <param name="self">TrTry computation</param>
    /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
    [Pure]
    public static Task<int> Count<A>(this Task<Try<A>> self) =>
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
    public static Task<bool> ForAll<A>(this Task<Try<A>> self, Func<A, bool> pred) =>
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
    public static Task<bool> ForAllAsync<A>(this Task<Try<A>> self, Func<A, Task<bool>> pred) =>
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
    public static Task<S> Fold<A, S>(this Task<Try<A>> self, S state, Func<S, A, S> folder) =>
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
    public static Task<S> FoldAsync<A, S>(this Task<Try<A>> self, S state, Func<S, A, Task<S>> folder) =>
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
    public static Task<S> BiFold<A, S>(this Task<Try<A>> self, S state, Func<S, A, S> Succ, Func<S, Exception, S> Fail) =>
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
    public static Task<S> BiFold<A, S>(this Task<Try<A>> self, S state, Func<S, A, Task<S>> Succ, Func<S, Exception, S> Fail) =>
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
    public static Task<S> BiFold<A, S>(this Task<Try<A>> self, S state, Func<S, A, S> Succ, Func<S, Exception, Task<S>> Fail) =>
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
    public static Task<S> BiFold<A, S>(this Task<Try<A>> self, S state, Func<S, A, Task<S>> Succ, Func<S, Exception, Task<S>> Fail) =>
        self.ToAsync().BiFold(state, Succ, Fail);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> Exists<A>(this Task<Try<A>> self, Func<A, bool> pred) =>
        self.ToAsync().Exists(pred);

    /// <summary>
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="pred">Predicate to test the bound value against</param>
    /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
    [Pure]
    public static Task<bool> ExistsAsync<A>(this Task<Try<A>> self, Func<A, Task<bool>> pred) =>
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
    public static Task<Try<R>> Map<A, R>(this Task<Try<A>> self, Func<A, R> mapper) =>
        self.Select(mapper);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static Task<Try<R>> Map<A, R>(this Task<Try<A>> self, Func<A, Task<R>> mapper) =>
        self.Select(mapper);

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
    public static Task<Try<R>> BiMap<A, R>(this Task<Try<A>> self, Func<A, R> Succ, Func<Exception, R> Fail) =>
        from x in self.ToAsync().BiMap(Succ, Fail).Try()
        select new Try<R>(() => x);

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
    public static Task<Try<R>> BiMap<A, R>(this Task<Try<A>> self, Func<A, Task<R>> Succ, Func<Exception, R> Fail) =>
        from x in self.ToAsync().BiMap(Succ, Fail).Try()
        select new Try<R>(() => x);

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
    public static Task<Try<R>> BiMap<A, R>(this Task<Try<A>> self, Func<A, R> Succ, Func<Exception, Task<R>> Fail) =>
        from x in self.ToAsync().BiMap(Succ, Fail).Try()
        select new Try<R>(() => x);

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
    public static Task<Try<R>> BiMap<A, R>(this Task<Try<A>> self, Func<A, Task<R>> Succ, Func<Exception, Task<R>> Fail) =>
        from x in self.ToAsync().BiMap(Succ, Fail).Try()
        select new Try<R>(() => x);

    [Pure]
    public static Task<Try<A>> Filter<A>(this Task<Try<A>> self, Func<A, bool> pred) =>
        from x in self.ToAsync().Filter(pred).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Try<A>> Filter<A>(this Task<Try<A>> self, Func<A, Task<bool>> pred) =>
        from x in self.ToAsync().Filter(pred).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Try<A>> BiFilter<A>(this Task<Try<A>> self, Func<A, bool> Succ, Func<Exception, bool> Fail) =>
        from x in self.ToAsync().BiFilter(Succ, Fail).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Try<A>> BiFilter<A>(this Task<Try<A>> self, Func<A, Task<bool>> Succ, Func<Exception, bool> Fail) =>
        from x in self.ToAsync().BiFilter(Succ, Fail).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Try<A>> BiFilter<A>(this Task<Try<A>> self, Func<A, bool> Succ, Func<Exception, Task<bool>> Fail) =>
        from x in self.ToAsync().BiFilter(Succ, Fail).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Try<A>> BiFilter<A>(this Task<Try<A>> self, Func<A, Task<bool>> Succ, Func<Exception, Task<bool>> Fail) =>
        from x in self.ToAsync().BiFilter(Succ, Fail).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Try<A>> Where<A>(this Task<Try<A>> self, Func<A, bool> pred) =>
        from x in self.ToAsync().Filter(pred).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Try<A>> Where<A>(this Task<Try<A>> self, Func<A, Task<bool>> pred) =>
        from x in self.ToAsync().Filter(pred).Try()
        select new Try<A>(() => x);

    [Pure]
    public static Task<Seq<A>> ToSeq<A>(this Task<Try<A>> self) =>
        self.ToAsync().ToSeq();

    [Pure]
    public static Task<Seq<A>> AsEnumerable<A>(this Task<Try<A>> self) =>
        self.ToAsync().AsEnumerable();

    [Pure]
    public static Task<Lst<A>> ToList<A>(this Task<Try<A>> self) =>
        self.ToAsync().ToList();

    [Pure]
    public static Task<Arr<A>> ToArray<A>(this Task<Try<A>> self) =>
        self.ToAsync().ToArray();

    [Pure]
    public static Task<string> AsString<A>(this Task<Try<A>> self) =>
        self.ToAsync().AsString();

    [Pure]
    public static async Task<Try<B>> Bind<A, B>(this Task<Try<A>> self, Func<A, Task<Try<B>>> binder)
    {
        try
        {
            var resA = await self.ConfigureAwait(false);
            var a = resA.Try();
            return a.IsFaulted
                ? new Try<B>(() => new Result<B>(a.Exception))
                : await binder(a.Value).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return new Try<B>(() => new Result<B>(e));
        }
    }

    [Pure]
    public async static Task<Try<C>> SelectMany<A, B, C>(
        this Task<Try<A>> self,
        Func<A, Task<Try<B>>> bind,
        Func<A, B, C> project)
        {
            try
            {
                var resA = await self.ConfigureAwait(false);
                var a = resA.Try();
                if (a.IsFaulted) return new Try<C>(() => new Result<C>(a.Exception));
                var resB = await bind(a.Value).ConfigureAwait(false);
                var b = resB.Try();
                if (b.IsFaulted) return new Try<C>(() => new Result<C>(b.Exception));
                return new Try<C>(() => project(a.Value, b.Value));
            }
            catch(Exception e)
            {
                return new Try<C>(() => new Result<C>(e));
            }
        }
    
    [Pure]
    public static Task<int> Sum(this Task<Try<int>> self) =>
        self.ToAsync().Sum();

    /// <summary>
    /// Compare the bound value of Try(x) to Try(y).  If either of the
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>1 if lhs > rhs, 0 if lhs == rhs, -1 if lhs < rhs</returns>
    [Pure]
    public static Task<int> Compare<ORD, A>(this Task<Try<A>> lhs, Task<Try<A>> rhs) where ORD : struct, Ord<A> =>
        lhs.ToAsync().Compare<ORD, A>(rhs.ToAsync());

    /// <summary>
    /// Add the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Add<NUM, A>(this Task<Try<A>> lhs, Task<Try<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Add<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Find the subtract of the bound value of Try(x) and Try(y).  If either of 
    /// the Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Subtract<NUM, A>(this Task<Try<A>> lhs, Task<Try<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Subtract<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Product<NUM, A>(this Task<Try<A>> lhs, Task<Try<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Product<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryAsync<A> Divide<NUM, A>(this Task<Try<A>> lhs, Task<Try<A>> rhs) where NUM : struct, Num<A> =>
        lhs.ToAsync().Divide<NUM, A>(rhs.ToAsync());

    /// <summary>
    /// Convert the Try type to a Nullable of A
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    /// <param name="ma">Try to convert</param>
    /// <returns>Nullable of A</returns>
    [Pure]
    public static Task<A?> ToNullable<A>(this Task<Try<A>> ma) where A : struct =>
        ma.ToAsync().ToNullable();
}
