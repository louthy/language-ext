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
using LanguageExt.Instances;

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
        select add<NUM, A>(x, y);

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