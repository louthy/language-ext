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
    /// Invoke a delegate if the Try returns a value successfully
    /// </summary>
    /// <param name="Succ">Delegate to invoke if successful</param>
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
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    [Pure]
    public static A IfFail<A>(this Try<A> self, Func<Exception, A> defaultAction)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return defaultAction(res.Exception);
        else
            return res.Value;
    }

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

    public static async Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, Task<R>> Succ, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Task.FromResult(Fail(res.Exception))
            : Succ(res.Value));
    }

    public static async Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, Task<R>> Succ, Func<Exception, Task<R>> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value));
    }

    public static async Task<R> MatchAsync<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, Task<R>> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Fail(res.Exception)
            : Task.FromResult(Succ(res.Value)));
    }

    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Observable.Return(Fail(res.Exception))
            : Succ(res.Value);
    }

    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, IObservable<R>> Succ, Func<Exception, IObservable<R>> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    }

    public static IObservable<R> MatchObservable<A, R>(this Try<A> self, Func<A, R> Succ, Func<Exception, IObservable<R>> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Observable.Return(Succ(res.Value));
    }

    /// <summary>
    /// Memoise the try
    /// </summary>
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
    public static TryOption<A> ToTryOption<A>(this Try<A> self) =>
        TryOption(() =>
        {
            var res = self.Try();
            return res.IsFaulted
                ? None
                : Optional(res.Value);
        });

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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Try<U> Select<A, U>(this Try<A> self, Func<A, U> select) => () =>
        select(self().Value);

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
    /// Tests that a predicate holds for any value of the bound value T
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
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
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
    /// <typeparam name="R">Resulting bound value type</typeparam>
    /// <param name="self">Try computation</param>
    /// <param name="mapper">Delegate to map the bound value</param>
    /// <returns>Mapped Try computation</returns>
    [Pure]
    public static Try<R> Map<A, R>(this Try<A> self, Func<A, R> mapper) => () =>
        new TryResult<R>(mapper(self().Value));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <typeparam name="T">Type of the bound value</typeparam>
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Try<A> Where<A>(this Try<A> self, Func<A, bool> pred) =>
        self.Filter(pred);

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
    public static Lst<Either<Exception, A>> ToList<A>(this Try<A> self) =>
        toList(self.AsEnumerable());

    [Pure]
    public static Either<Exception, A>[] ToArray<A>(this Try<A> self) =>
        toArray(self.AsEnumerable());

    [Pure]
    public static TrySuccContext<A, R> Succ<A,R>(this Try<A> self, Func<A, R> succHandler) =>
        new TrySuccContext<A, R>(self, succHandler);

    [Pure]
    public static TrySuccUnitContext<A> Succ<A>(this Try<A> self, Action<A> succHandler) =>
        new TrySuccUnitContext<A>(self, succHandler);

    [Pure]
    public static string AsString<A>(this Try<A> self) =>
        match(self,
            Succ: v => isnull(v)
                      ? "Succ(null)"
                      : $"Succ({v})",
            Fail: ex => $"Fail({ex.Message})");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Try<V> SelectMany<A, U, V>(
        this Try<A> self,
        Func<A, Try<U>> bind,
        Func<A, U, V> project) => () =>
        {
            var resT = self();
            return project(resT.Value, bind(resT.Value)().Value);
        };

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    public static TryResult<T> Try<T>(this Try<T> self)
    {
        if(self == null)
        {
            return new TryResult<T>(new ArgumentNullException("this is null in Try()"));
        }
        try
        {
            return self();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new TryResult<T>(e);
        }
    }

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, U> select)
        where T : IDisposable => () =>
            {
                TryResult<T> t = default(TryResult<T>);
                try
                {
                    t = self();
                    return t.IsFaulted
                        ? throw t.Exception
                        : select(t.Value);
                }
                finally
                {
                    t.Value?.Dispose();
                }
            };

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, Try<U>> select)
        where T : IDisposable => () =>
        {
            TryResult<T> t = default(T);
            try
            {
                t = self();
                var u = t.IsFaulted
                    ? throw t.Exception
                    : select(t.Value)();

                return u.IsFaulted
                    ? throw u.Exception
                    : u.Value;
            }
            finally
            {
                t.Value?.Dispose();
            }
        };

    [Pure]
    public static int Sum(this Try<int> self) =>
        self.Try().Value;

    public static async Task<R> MatchAsync<T, R>(this Task<Try<T>> self, Func<T, R> Succ, Func<Exception, R> Fail) =>
        await self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
        });

    public static async Task<R> MatchAsync<T, R>(this Task<Try<T>> self, Func<T, Task<R>> Succ, Func<Exception, R> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Task.FromResult(Fail(res.Exception))
                : Succ(res.Value);
        })
               from t in tt
               select t);

    public static async Task<R> MatchAsync<T, R>(this Task<Try<T>> self, Func<T, Task<R>> Succ, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Succ(res.Value);
        })
        from t in tt
        select t);

    public static async Task<R> MatchAsync<T, R>(this Task<Try<T>> self, Func<T, R> Succ, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : Task.FromResult(Succ(res.Value));
        })
        from t in tt
        select t);


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
    /// <param name="option">Optional function</param>
    /// <param name="x">Argument to apply</param>
    /// <returns>Returns the result of applying the optional argument to the optional function</returns>
    [Pure]
    public static Try<B> Apply<A, B>(this Try<Func<A, B>> x, Try<A> y) =>
        apply<MTry<Func<A, B>>, MTry<A>, MTry<B>, Try<Func<A, B>>, Try<A>, Try<B>, A, B>(x, y);

    /// <summary>
    /// Apply
    /// </summary>
    /// <returns>Returns the result of applying the Try argument to the Try function</returns>
    [Pure]
    public static Try<C> Apply<A, B, C>(this Try<Func<A, B, C>> x, Try<A> y, Try<B> z) =>
        apply<MTry<Func<A, B, C>>, MTry<A>, MTry<B>, MTry<C>, Try<Func<A, B, C>>, Try<A>, Try<B>, Try<C>, A, B, C>(x, y, z);

    /// <summary>
    /// Apply
    /// </summary>
    /// <returns>Returns the result of applying the Try argument to the Try function</returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Try<Func<A, B, C>> x, Try<A> y) =>
        apply<MTry<Func<A, B, C>>, MTry<A>, MTry<Func<B, C>>, Try<Func<A, B, C>>, Try<A>, Try<Func<B, C>>, A, B, C>(x, y);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="x">Argument to apply</param>
    /// <returns>Returns the result of applying the Try argument to the Try function</returns>
    [Pure]
    public static Try<Func<B, C>> Apply<A, B, C>(this Try<Func<A, Func<B, C>>> x, Try<A> y) =>
        apply2<MTry<Func<A, Func<B, C>>>, MTry<A>, MTry<Func<B, C>>, Try<Func<A, Func<B, C>>>, Try<A>, Try<Func<B, C>>, A, B, C>(x, y);

    /// <summary>
    /// Apply
    /// </summary>
    [Pure]
    public static Try<B> Action<A, B>(this Try<A> x, Try<B> y) =>
        action<MTry<A>, MTry<B>, Try<A>, Try<B>, A, B>(x, y);

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
    public static Try<A> Divide<NUM, A>(this Try<A> lhs, Try<A> rhs) where NUM : struct, Num<A> =>
        from x in lhs
        from y in rhs
        select divide<NUM, A>(x, y);

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
}