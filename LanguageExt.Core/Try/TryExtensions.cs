using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Reactive.Linq;
using LanguageExt.TypeClasses;
using System.Collections.Generic;

/// <summary>
/// Extension methods for the Try monad
/// </summary>
public static class TryExtensions
{
    [Pure]
    public static TryResult<T> Try<T>(this Try<T> self)
    {
        try
        {
            return self.Run();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new TryResult<T>(e);
        }
    }

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, U> select)
        where T : IDisposable =>
            Prelude.Try(() =>
            {
                T t = default(T);
                try
                {
                    t = self.Run();
                    return select(t);
                }
                finally
                {
                    t?.Dispose();
                }
            });

    [Pure]
    public static Try<U> Use<T, U>(this Try<T> self, Func<T, Try<U>> select)
        where T : IDisposable =>
        Prelude.Try(() =>
        {
            var t = default(T);
            try
            {
                t = self.Run();
                return select(t).Run();
            }
            finally
            {
                t?.Dispose();
            }
        });

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
        Prelude.Try(() => self.Run().Run());

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<Try<T>>> self) =>
        Prelude.Try(() => self.Run().Run().Run());

    [Pure]
    public static Try<T> Flatten<T>(this Try<Try<Try<Try<T>>>> self) =>
        Prelude.Try(() => self.Run().Run().Run().Run());

    /// <summary>
    /// Apply a Try value to a Try function
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg">Try argument</param>
    /// <returns>Returns the result of applying the Try argument to the Try function</returns>
    [Pure]
    public static Try<R> Apply<T, R>(this Try<Func<T, R>> self, Try<T> arg) =>
        Prelude.Try(() => self.Run()(arg.Run()));

    /// <summary>
    /// Apply a Try value to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg">Try argument</param>
    /// <returns>Returns the result of applying the Try argument to the Try function:
    /// a Try function of arity 1</returns>
    [Pure]
    public static Try<Func<T2, R>> Apply<T1, T2, R>(this Try<Func<T1, T2, R>> self, Try<T1> arg) =>
        Prelude.Try(() => par(self.Run(), arg.Run()));

    /// <summary>
    /// Apply Try values to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg1">Try argument</param>
    /// <param name="arg2">Try argument</param>
    /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
    [Pure]
    public static Try<R> Apply<T1, T2, R>(this Try<Func<T1, T2, R>> self, Try<T1> arg1, Try<T2> arg2) =>
        Prelude.Try(() => self.Run()(arg1.Run(), arg2.Run()));

    /// <summary>
    /// Append the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Append<SEMI, A>(this Try<A> lhs, Try<A> rhs) where SEMI : struct, Semigroup<A> =>
        from x in lhs
        from y in rhs
        select append<SEMI, A>(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <typeparam name="EQ">Type-class of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>True if the bound values are equal</returns>
    [Pure]
    public static bool Equals<EQ, A>(this Try<A> lhs, Try<A> rhs) where EQ : struct, Eq<A>
    {
        var x = lhs.Try();
        var y = lhs.Try();
        if (x.IsFaulted && y.IsFaulted) return true;
        if (x.IsFaulted || y.IsFaulted) return false;
        return default(EQ).Equals(x.Value, y.Value);
    }

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
    public static Try<A> Add<ADD, A>(this Try<A> lhs, Try<A> rhs) where ADD : struct, Addition<A> =>
        from x in lhs
        from y in rhs
        select add<ADD, A>(x, y);

    /// <summary>
    /// Find the difference of the bound value of Try(x) and Try(y).  If either of 
    /// the Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Difference<DIFF, A>(this Try<A> lhs, Try<A> rhs) where DIFF : struct, Difference<A> =>
        from x in lhs
        from y in rhs
        select difference<DIFF, A>(x, y);

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Product<PROD, A>(this Try<A> lhs, Try<A> rhs) where PROD : struct, Product<A> =>
        from x in lhs
        from y in rhs
        select product<PROD, A>(x, y);

    /// <summary>
    /// Multiply the bound value of Try(x) and Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Try<A> Divide<DIV, A>(this Try<A> lhs, Try<A> rhs) where DIV : struct, Divisible<A> =>
        from x in lhs
        from y in rhs
        select divide<DIV, A>(x, y);

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