using System;
using System.Linq;
using System.Reactive.Linq;
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
        self.Map(x => x.Cons(Empty));

    [Pure]
    public static Seq<A> ToSeq<Env, A>(this Reader<Env, A> self, Env env) =>
        self.Map(x => x.Cons(Empty)).Run(env).IfNoneOrFail(Empty);

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
    public static Reader<Env, S> Fold<Env, A, S>(this Reader<Env, A> self, S initialState, Func<S, A, S> f) =>
        env =>
        {
            var (x, b) = self(env);
            return b
                ? (default(S), true)
                : (f(initialState, x), false);
        };

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

