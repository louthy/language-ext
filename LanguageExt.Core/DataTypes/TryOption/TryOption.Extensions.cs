using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Reactive.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for the Try monad
/// </summary>
public static class TryOptionExtensions
{
    /// <summary>
    /// Savely invokes the TryOption computation
    /// </summary>
    /// <typeparam name="T">Bound value of the computation</typeparam>
    /// <param name="self">TryOption to invoke</param>
    /// <returns>TryOptionResult</returns>
    [Pure]
    public static TryOptionResult<T> Try<T>(this TryOption<T> self)
    {
        try
        {
            return self.Run();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new TryOptionResult<T>(e);
        }
    }

    [Pure]
    public static TryOption<U> Use<T, U>(this TryOption<T> self, Func<T, U> select)
        where T : IDisposable =>
            TryOption(() =>
            {
                T t = default(T);
                try
                {
                    var opt = self.Run();
                    if (opt.IsNone) return opt.Map(select);
                    t = opt.Value;
                    return select(t);
                }
                finally
                {
                    t?.Dispose();
                }
            });

    [Pure]
    public static int Sum(this TryOption<int> self) =>
        self.Try().Value.Sum();

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
        await self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Some(res.Value.Value)
                : None();
        });

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, R> Some, Func<R> Fail) =>
        await self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted || res.Value.IsNone
            ? Fail()
            : Some(res.Value.Value);
        });

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Some, Func<R> None, Func<Exception, R> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Task.FromResult(Fail(res.Exception))
                : res.Value.IsSome
                    ? Some(res.Value.Value)
                    : Task.FromResult(None());
        })
        from t in tt
        select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Some, Func<R> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted || res.Value.IsNone
                ? Task.FromResult(Fail())
                : Some(res.Value.Value);
        })
        from t in tt
        select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Some, Func<Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Some(res.Value.Value);
        })
        from t in tt
        select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome 
                    ? Some(res.Value.Value)
                    : None();
        })
        from t in tt
        select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, R> Some, Func<Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Task.FromResult(Some(res.Value.Value));
        })
        from t in tt
        select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, R> Some, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Task.FromResult(Some(res.Value.Value))
                    : None();
        })
        from t in tt
        select t);

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<R> Fail) =>
        self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Some(res.Value.Value);
        });

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<R> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted || res.Value.IsNone
                ? Observable.Return(Fail())
                : Some(res.Value.Value);
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Some(res.Value.Value);
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted || res.Value.IsNone
                ? Fail()
                : Observable.Return(Some(res.Value.Value));
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
        self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome 
                    ? Some(res.Value.Value)
                    : None();
        });

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<R> None, Func<Exception, R> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Observable.Return(Fail(res.Exception))
                : res.Value.IsSome 
                    ? Some(res.Value.Value)
                    : Observable.Return(None());
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome 
                    ? Some(res.Value.Value)
                    : None();
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome 
                    ? Observable.Return(Some(res.Value.Value))
                    : None();
        })
        from t in tt
        select t;

    [Pure]
    public static TryOption<T> Flatten<T>(this TryOption<TryOption<T>> self) =>
        from x in self
        from y in x
        select y;

    [Pure]
    public static TryOption<T> Flatten<T>(this TryOption<TryOption<TryOption<T>>> self) =>
        from x in self
        from y in x
        from z in y
        select z;

    [Pure]
    public static TryOption<T> Flatten<T>(this TryOption<TryOption<TryOption<TryOption<T>>>> self) =>
        from x in self
        from y in x
        from z in y
        from w in z
        select w;

    /// <summary>
    /// Apply a TryOptional argument to a TryOptional function of arity 1
    /// </summary>
    /// <param name="option">Optional function</param>
    /// <param name="x">Argument to apply</param>
    /// <returns>Returns the result of applying the optional argument to the optional function</returns>
    [Pure]
    public static TryOption<B> Apply<A, B>(this TryOption<Func<A, B>> x, TryOption<A> y) =>
        apply<MTryOption<Func<A, B>>, MTryOption<A>, MTryOption<B>, TryOption<Func<A, B>>, TryOption<A>, TryOption<B>, A, B>(x, y);

    /// <summary>
    /// Apply two TryOptional arguments to a TryOptional function of arity 2
    /// </summary>
    /// <param name="TryOption">TryOptional function</param>
    /// <param name="x">Argument to apply</param>
    /// <param name="y">Argument to apply</param>
    /// <returns>Returns the result of applying the TryOptional argument to the TryOptional function</returns>
    [Pure]
    public static TryOption<C> Apply<A, B, C>(this TryOption<Func<A, B, C>> x, TryOption<A> y, TryOption<B> z) =>
        apply<MTryOption<Func<A, B, C>>, MTryOption<A>, MTryOption<B>, MTryOption<C>, TryOption<Func<A, B, C>>, TryOption<A>, TryOption<B>, TryOption<C>, A, B, C>(x, y, z);

    /// <summary>
    /// Apply one TryOptional arguments to a TryOptional function of arity 2
    /// </summary>
    /// <param name="TryOption">TryOptional function</param>
    /// <param name="x">Argument to apply</param>
    /// <returns>Returns the result of applying the TryOptional argument to the TryOptional function</returns>
    [Pure]
    public static TryOption<Func<B, C>> Apply<A, B, C>(this TryOption<Func<A, B, C>> x, TryOption<A> y) =>
        apply<MTryOption<Func<A, B, C>>, MTryOption<A>, MTryOption<Func<B, C>>, TryOption<Func<A, B, C>>, TryOption<A>, TryOption<Func<B, C>>, A, B, C>(x, y);

    /// <summary>
    /// Apply one TryOptional arguments to a TryOptional function of arity 2
    /// </summary>
    /// <param name="TryOption">TryOptional function</param>
    /// <param name="x">Argument to apply</param>
    /// <returns>Returns the result of applying the TryOptional argument to the TryOptional function</returns>
    [Pure]
    public static TryOption<Func<B, C>> Apply<A, B, C>(this TryOption<Func<A, Func<B, C>>> x, TryOption<A> y) =>
        apply2<MTryOption<Func<A, Func<B, C>>>, MTryOption<A>, MTryOption<Func<B, C>>, TryOption<Func<A, Func<B, C>>>, TryOption<A>, TryOption<Func<B, C>>, A, B, C>(x, y);

    /// <summary>
    /// Partially apply a TryOptional argument to a curried TryOptional function
    /// </summary>
    [Pure]
    public static TryOption<B> Action<A, B>(this TryOption<A> x, TryOption<B> y) =>
        action<MTryOption<A>, MTryOption<B>, TryOption<A>, TryOption<B>, A, B>(x, y);

    /// <summary>
    /// Add the bound value of Try(x) to Try(y).  If either of the
    /// Trys are Fail then the result is Fail
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryOption<A> Add<NUM, A>(this TryOption<A> lhs, TryOption<A> rhs) where NUM : struct, Num<A> =>
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
    public static TryOption<A> Subtract<NUM, A>(this TryOption<A> lhs, TryOption<A> rhs) where NUM : struct, Num<A> =>
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
    public static TryOption<A> Product<NUM, A>(this TryOption<A> lhs, TryOption<A> rhs) where NUM : struct, Num<A> =>
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
    public static TryOption<A> Divide<NUM, A>(this TryOption<A> lhs, TryOption<A> rhs) where NUM : struct, Num<A> =>
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
    public static A? ToNullable<A>(this TryOption<A> ma) where A : struct
    {
        var x = ma.Try();
        return x.IsFaulted || x.Value.IsNone
            ? (A?)null
            : x.Value.Value;
    }
}