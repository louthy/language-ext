using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Collections.Generic;

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
    public static Fin<A> Run<Env, A>(this Reader<Env, A> self, Env env)
    {
        try
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (env == null) throw new ArgumentNullException(nameof(env));
            return self(env);
        }
        catch(Exception e)
        {
            return Error.New(e);
        }
    }

    [Pure]
    public static Reader<Env, Seq<A>> AsEnumerable<Env, A>(this Reader<Env, A> self) =>
        self.Map(x => x.Cons());

    [Pure]
    public static Seq<A> ToSeq<Env, A>(this Reader<Env, A> self, Env env) =>
        self.Run(env).ToSeq();

    [Pure]
    public static Lst<A> ToList<Env, A>(this Reader<Env, A> self, Env env) =>
        self.Run(env).ToList();

    [Pure]
    public static Option<A> ToOption<Env, A>(this Reader<Env, A> self, Env env) =>
        self.Run(env).ToOption();

    [Pure]
    public static Either<Error, A> ToEither<Env, A>(this Reader<Env, A> self, Env env) =>
        self.Run(env).ToEither();

    [Pure]
    public static Either<L, A> ToEither<Env, L, A>(this Reader<Env, A> self, Env env, Func<Error, L> Left) =>
        self.Run(env).ToEither().MapLeft(Left);

    [Pure]
    public static IEnumerable<A> AsEnumerable<Env, A>(this Reader<Env, A> self, Env env) =>
        ToSeq(self, env);

    public static Reader<Env, Unit> Iter<Env, A>(this Reader<Env, A> self, Action<A> action) =>
        self.Map(x => { action(x); return unit; });

    [Pure]
    public static Reader<Env, int> Count<Env, T>(this Reader<Env, T> self) =>
        self.Map(_ => 1);

    [Pure]
    public static Reader<Env, int> Sum<Env>(this Reader<Env, int> self) =>
        self;

    [Pure]
    public static Reader<Env, bool> ForAll<Env, A>(this Reader<Env, A> self, Func<A, bool> pred) =>
        self.Map(pred);

    [Pure]
    public static Reader<Env, bool> Exists<Env, A>(this Reader<Env, A> self, Func<A, bool> pred) =>
        self.Map(pred);

    [Pure]
    public static Reader<Env, S> Fold<Env, S, A>(this Reader<Env, A> self, S state, Func<S, A, S> folder) =>
        self.Map(x => folder(state, x));

    [Pure]
    public static Reader<Env, R> Map<Env, A, R>(this Reader<Env, A> self, Func<A, R> mapper) =>
        self.Select(mapper);

    /// <summary>
    /// Force evaluation of the monad (once only)
    /// </summary>
    [Pure]
    public static Reader<Env, A> Strict<Env, A>(this Reader<Env, A> ma)
    {
        Option<Fin<A>> cache = default;
        var sync = new object();
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
            if (r.IsSucc)
            {
                f(r.Value);
            }
            return r;
        };

    [Pure]
    public static Reader<Env, B> Bind<Env, A, B>(this Reader<Env, A> self, Func<A, Reader<Env, B>> binder) =>
        env => self(env).Map(x => binder(x)(env)).Flatten();

    /// <summary>
    /// Select
    /// </summary>
    [Pure]
    public static Reader<Env, B> Select<Env, A, B>(this Reader<Env, A> self, Func<A, B> map) =>
        env => self(env).Map(map);

    /// <summary>
    /// Select Many
    /// </summary>
    [Pure]
    public static Reader<Env, C> SelectMany<Env, A, B, C>(
        this Reader<Env, A> self,
        Func<A, Reader<Env, B>> bind,
        Func<A, B, C> project) =>
        self.Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    public static Reader<Env, Env> Fold<Env, A>(this Reader<Env, A> self, Func<Env, A, Env> f) =>
        env =>
        {
            var resA = self(env);
            return resA.IsFail
                ? resA.Error
                : f(env, resA.Value);
        };
}

