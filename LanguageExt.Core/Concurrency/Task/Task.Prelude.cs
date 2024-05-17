using LanguageExt;
using LanguageExt.ClassInstances;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Convert a value to a Task that completes immediately
    /// </summary>
    [Pure]
    public static Task<A> TaskSucc<A>(A self) =>
        Task.FromResult(self);

    /// <summary>
    /// Convert a value to a Task that completes immediately
    /// </summary>
    [Pure]
    public static Task<A> TaskFail<A>(Exception ex) =>
        Task.FromException<A>(ex);

    /// <summary>
    /// Flatten the nested Task type
    /// </summary>
    [Pure]
    public static Task<A> flatten<A>(Task<Task<A>> self) =>
        self.Flatten();

    /// <summary>
    /// Flatten the nested Task type
    /// </summary>
    [Pure]
    public static Task<A> flatten<A>(Task<Task<Task<A>>> self) =>
        self.Flatten();

    /// <summary>
    /// Get the Count of a Task T.  Returns either 1 or 0 if cancelled or faulted.
    /// </summary>
    [Pure]
    public static Task<int> count<A>(Task<A> self) =>
        self.Count();

    /// <summary>
    /// Monadic bind operation for Task
    /// </summary>
    [Pure]
    public static Task<B> bind<A, B>(Task<A> self, Func<A, Task<B>> bind) =>
        self.Bind(bind);

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static Task<bool> exists<A>(Task<A> self, Func<A, bool> pred) =>
        self.Exists(pred);

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static Task<bool> existsAsync<A>(Task<A> self, Func<A, Task<bool>> pred) =>
        self.ExistsAsync(pred);

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static Task<bool> forall<A>(Task<A> self, Func<A, bool> pred) =>
        self.ForAll(pred);

    /// <summary>
    /// Returns false if the Task is cancelled or faulted, otherwise
    /// it returns the result of pred(Result)
    /// </summary>
    [Pure]
    public static Task<bool> forallAsync<A>(Task<A> self, Func<A, Task<bool>> pred) =>
        self.ForAllAsync(pred);

    /// <summary>
    /// Filters the task.  This throws a BottomException when pred(Result)
    /// returns false
    /// </summary>
    [Pure]
    public static Task<A> filter<A>(Task<A> self, Func<A, bool> pred) =>
        self.Filter(pred);

    /// <summary>
    /// Iterates the Task.  Invokes f(Result) if not faulted or cancelled
    /// </summary>
    public static Task<Unit> iter<A>(Task<A> self, Action<A> f) =>
        self.Iter(f);

    /// <summary>
    /// Returns map(Result) if not faulted or cancelled.
    /// </summary>
    [Pure]
    public static Task<B> map<A, B>(Task<A> self, Func<A, B> map) =>
        self.Map(map);

    /// <summary>
    /// Returns map(Result) if not faulted or cancelled.
    /// </summary>
    [Pure]
    public static Task<B> mapAsync<A, B>(Task<A> self, Func<A, Task<B>> map) =>
        self.MapAsync(map);

    [Pure]
    public static Task<A> plus<A>(this Task<A> ma, Task<A> mb) =>
        ma.Plus(mb);

    [Pure]
    public static Task<A> plusFirst<A>(this Task<A> ma, Task<A> mb) =>
        ma.PlusFirst(mb);

    /// <summary>
    /// Returns the first successful computation 
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    /// <param name="ma">The first computation to run</param>
    /// <param name="tail">The rest of the computations to run</param>
    /// <returns>The first computation that succeeds</returns>
    [Pure]
    public static Task<A> choice<A>(Task<A> ma, params Task<A>[] tail) =>
        choice(Cons(ma, tail));

    /// <summary>
    /// Returns the first successful computation 
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    /// <param name="xs">Sequence of computations to run</param>
    /// <returns>The first computation that succeeds</returns>
    [Pure]
    public static async Task<A> choice<A>(Seq<Task<A>> xs)
    {
        foreach (var x in xs)
        {
            try
            {
                return await x.ConfigureAwait(false);
            }
            catch
            {
                // ignore
            }
        }
        throw new BottomException();
    }

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static async Task<B> apply<A, B>(Task<Func<A, B>> fab, Task<A> fa)
    {
        await Task.WhenAll(fab, fa).ConfigureAwait(false);
        return fab.Result(fa.Result);
    }

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Task<B> apply<A, B>(Func<A, B> fab, Task<A> fa) =>
        fa.Map(fab);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static async Task<C> apply<A, B, C>(Task<Func<A, B, C>> fabc, Task<A> fa, Task<B> fb)
    {
        await Task.WhenAll(fabc, fa, fb).ConfigureAwait(false);
        return fabc.Result(fa.Result, fb.Result);
    }

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static async Task<C> apply<A, B, C>(Func<A, B, C> fabc, Task<A> fa, Task<B> fb)
    {
        await Task.WhenAll(fa, fb).ConfigureAwait(false);
        return fabc(fa.Result, fb.Result);
    }

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static async Task<Func<B, C>> apply<A, B, C>(Task<Func<A, B, C>> fabc, Task<A> fa)
    {
        await Task.WhenAll(fabc, fa).ConfigureAwait(false);
        return curry(fabc.Result)(fa.Result);
    }

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Task<Func<B, C>> apply<A, B, C>(Func<A, B, C> fabc, Task<A> fa) =>
        fa.Map(curry(fabc));

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static async Task<Func<B, C>> apply<A, B, C>(Task<Func<A, Func<B, C>>> fabc, Task<A> fa)
    {
        await Task.WhenAll(fabc, fa).ConfigureAwait(false);
        return fabc.Result(fa.Result);
    }

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Task<Func<B, C>> apply<A, B, C>(Func<A, Func<B, C>> fabc, Task<A> fa) =>
        fa.Map(fabc);
}
