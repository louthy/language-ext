using System;
using System.Linq;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using LanguageExt.ClassInstances;

/// <summary>
/// Reader monad extensions
/// </summary>
public static class ReaderExt
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Reader<Env, A> Flatten<Env, A>(this Reader<Env, Reader<Env, A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Runs the Reader monad and memoizes the result in a TryOption monad.  Use
    /// Match, IfSucc, IfNone, etc to extract.
    /// </summary>
    public static TryOption<A> Run<Env, A>(this Reader<Env, A> self, Env env)
    {
        try
        {
            if (self == null) return () => Option<A>.None; ;
            if (env == null) return () => Option<A>.None; ;
            var (a, b) = self(env);
            if(b)
            {
                return () => Option<A>.None;
            }
            else
            {
                return () => Optional(a);
            }
        }
        catch(Exception e)
        {
            return () => new OptionalResult<A>(e);
        }
    }

    [Pure]
    public static Reader<Env, Seq<A>> AsEnumerable<Env, A>(this Reader<Env, A> self) =>
        self.Map(x => x.Cons());

    [Pure]
    public static Seq<A> ToSeq<Env, A>(this Reader<Env, A> self, Env env) =>
        self.Map(x => x.Cons()).Run(env).IfNoneOrFail(Empty);

    [Pure]
    public static Seq<A> AsEnumerable<Env, A>(this Reader<Env, A> self, Env env) =>
        ToSeq(self, env);

    public static Reader<Env, Unit> Iter<Env, A>(this Reader<Env, A> self, Action<A> action) =>
        self.Map(x => { action(x); return unit; });

    [Pure]
    public static Reader<Env, int> Count<Env, T>(this Reader<Env, T> self) =>
        self.Map(x => 1);

    [Pure]
    public static Reader<Env, int> Sum<Env>(this Reader<Env, int> self) =>
        self;

    [Pure]
    public static Reader<Env, bool> ForAll<Env, A>(this Reader<Env, A> self, Func<A, bool> pred) =>
        self.Map(x => pred(x));

    [Pure]
    public static Reader<Env, bool> Exists<Env, A>(this Reader<Env, A> self, Func<A, bool> pred) =>
        self.Map(x => pred(x));

    [Pure]
    public static Reader<Env, S> Fold<Env, S, A>(this Reader<Env, A> self, S state, Func<S, A, S> folder) =>
        self.Map(x => folder(state, x));

    [Pure]
    public static Reader<Env, R> Map<Env, A, R>(this Reader<Env, A> self, Func<A, R> mapper) =>
        self.Select(mapper);

    [Pure]
    public static Reader<Env, A> Filter<Env, A>(this Reader<Env, A> self, Func<A, bool> pred) =>
        self.Where(pred);

    [Pure]
    public static Reader<Env, A> Where<Env, A>(this Reader<Env, A> self, Func<A, bool> pred) => env =>
    {
        var (a, faulted) = self(env);
        if (faulted || !pred(a)) return (a, true);
        return (a, false);
    };

    /// <summary>
    /// Force evaluation of the monad (once only)
    /// </summary>
    [Pure]
    public static Reader<Env, A> Strict<Env, A>(this Reader<Env, A> ma)
    {
        Option<(A, bool)> cache = default;
        object sync = new object();
        return env =>
        {
            if (cache.IsSome) return cache.Value;
            lock (sync)
            {
                if (cache.IsSome) return cache.Value;
                cache = ma(env);
                return cache.Value;
            }
        };
    }

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static Reader<Env, A> Do<Env, A>(this Reader<Env, A> ma, Action<A> f) =>
        env =>
        {
            var r = ma(env);
            if (!r.IsFaulted)
            {
                f(r.Value);
            }
            return r;
        };

    [Pure]
    public static Reader<Env, B> Bind<Env, A, B>(this Reader<Env, A> self, Func<A, Reader<Env, B>> binder) =>
        default(MReader<Env, A>)
            .Bind<MReader<Env, B>, Reader<Env, B>, B>(self, binder);

    /// <summary>
    /// Select
    /// </summary>
    [Pure]
    public static Reader<Env, B> Select<Env, A, B>(this Reader<Env, A> self, Func<A, B> map) =>
        default(MReader<Env, A>).Bind<MReader<Env, B>, Reader<Env, B>, B>(self, a =>
        default(MReader<Env, B>).Return(_ => map(a)));

    /// <summary>
    /// Select Many
    /// </summary>
    [Pure]
    public static Reader<Env, C> SelectMany<Env, A, B, C>(
        this Reader<Env, A> self,
        Func<A, Reader<Env, B>> bind,
        Func<A, B, C> project) =>
            default(MReader<Env, A>).Bind<MReader<Env, C>, Reader<Env, C>, C>(self, a =>
            default(MReader<Env, B>).Bind<MReader<Env, C>, Reader<Env, C>, C>(bind(a), b =>
            default(MReader<Env, C>).Return(_ => project(a, b))));

    [Pure]
    public static Reader<Env, Env> Fold<Env, A>(this Reader<Env, A> self, Func<Env, A, Env> f) =>
        env =>
        {
            var (x, b) = self(env);
            return b
                ? (default(Env), true)
                : (f(env, x), false);
        };
}

