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
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (env == null) throw new ArgumentNullException(nameof(env));
            var (a, _, b) = Eval(self, env);
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
            return () => new TryOptionResult<A>(e);
        }
    }

    internal static (A Value, Env Environment, bool IsBottom) Eval<Env, A>(this Reader<Env, A> self, Env env) =>
        self == null || self.eval == null
            ? (default(A), default(Env), true)
            : self.eval(env);

    [Pure]
    public static Reader<Env, IEnumerable<A>> AsEnumerable<Env, A>(this Reader<Env, A> self) =>
        self.Map(x => (new A[1] { x }).AsEnumerable());

    [Pure]
    public static IEnumerable<A> AsEnumerable<Env, A>(this Reader<Env, A> self, Env env)
    {
        var res = self.Eval(env);
        if (!res.IsBottom)
        {
            yield return res.Value;
        }
    }

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
    public static Reader<Env, A> Where<Env, A>(this Reader<Env, A> self, Func<A, bool> pred) =>
        default(MReader<SReader<Env,A>, Reader<Env, A>, Env, A>).Return(env => {
            var (x, _, b) = self.Eval(env);
            if (b || !pred(x)) return (default(A), env, true);
            return (x, env, b);
        });

    [Pure]
    public static Reader<Env, B> Bind<Env, A, B>(this Reader<Env, A> self, Func<A, Reader<Env, B>> binder) =>
        default(MReader<SReader<Env, A>, Reader<Env, A>, Env, A>)
            .Bind<MReader<SReader<Env, B>, Reader<Env, B>, Env, B>, Reader<Env, B>, B>(self, binder);

    /// <summary>
    /// Select
    /// </summary>
    [Pure]
    public static Reader<Env, B> Select<Env, A, B>(this Reader<Env, A> self, Func<A, B> map) =>
        default(MReader<SReader<Env, A>, Reader<Env, A>, Env, A>).Bind<MReader<SReader<Env, B>, Reader<Env, B>, Env, B>, Reader<Env, B>, B>(self, a =>
        default(MReader<SReader<Env, B>, Reader<Env, B>, Env, B>).Return(map(a)));

    /// <summary>
    /// Select Many
    /// </summary>
    [Pure]
    public static Reader<Env, C> SelectMany<Env, A, B, C>(
        this Reader<Env, A> self,
        Func<A, Reader<Env, B>> bind,
        Func<A, B, C> project) =>
            default(MReader<SReader<Env, A>, Reader<Env, A>, Env, A>).Bind<MReader<SReader<Env, C>, Reader<Env, C>, Env, C>, Reader<Env, C>, C>(self, a =>
            default(MReader<SReader<Env, B>, Reader<Env, B>, Env, B>).Bind<MReader<SReader<Env, C>, Reader<Env, C>, Env, C>, Reader<Env, C>, C>(bind(a), b =>
            default(MReader<SReader<Env, C>, Reader<Env, C>, Env, C>).Return(project(a, b))));

    [Pure]
    public static Reader<Env, S> Fold<Env, A, S>(this Reader<Env, A> self, S initialState, Func<S, A, S> f) =>
        default(MReader<SReader<Env, S>, Reader<Env, S>, Env, S>).Return(env =>
        {
            var (x, _, b) = self.Eval(env);
            return b
                ? (default(S), env, true)
                : (f(initialState, x), env, false);
        });

    [Pure]
    public static Reader<Env, Env> Fold<Env, A>(this Reader<Env, A> self, Func<Env, A, Env> f) =>
        default(MReader<SReader<Env, Env>, Reader<Env, Env>, Env, Env>).Return(env =>
        {
            var (x, _, b) = self.Eval(env);
            return b
                ? (default(Env), env, true)
                : (f(env, x), env, false);
        });
}

