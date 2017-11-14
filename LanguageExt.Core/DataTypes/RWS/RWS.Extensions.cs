using System;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

/// <summary>
/// Extension methods for RWS
/// </summary>
public static class RWSExtensions
{
    /// <summary>
    /// Runs the RWS monad and memoizes the result in a TryOption monad.  Use
    /// Match, IfSucc, IfNone, etc to extract.
    /// </summary>
    public static (TryOption<A> Value, W Output, S State) Run<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, R env, S state)
        where MonoidW : struct, Monoid<W>
    {
        try
        {
            if (self == null) return (() => Option<A>.None, default(MonoidW).Empty(), state);
            if (state == null) return (() => Option<A>.None, default(MonoidW).Empty(), state);
            var (a, w, s, faulted) = self(env, state);
            if (faulted)
            {
                return (() => Option<A>.None, default(MonoidW).Empty(), state);
            }
            else
            {
                return (() => Optional(a), w, s);
            }
        }
        catch (Exception e)
        {
            return (() => new OptionalResult<A>(e), default(MonoidW).Empty(), state);
        }
    }

    [Pure]
    public static RWS<MonoidW, R, W, S, int> Sum<MonoidW, R, W, S>(this RWS<MonoidW, R, W, S, int> self)
        where MonoidW : struct, Monoid<W> => self;

    [Pure]
    public static RWS<MonoidW, R, W, S, Seq<A>> ToSeq<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self)
       where MonoidW : struct, Monoid<W> => self.Map(x => x.Cons(Empty));

    [Pure]
    public static RWS<MonoidW, R, W, S, Seq<A>> AsEnumerable<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self)
       where MonoidW : struct, Monoid<W> => ToSeq(self);

    [Pure]
    public static Seq<A> ToSeq<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, R env, S state)
       where MonoidW : struct, Monoid<W>
    {
        IEnumerable<A> Yield()
        {
            var (a, w, s, faulted) = self(env, state);
            if (!faulted)
            {
                yield return a;
            }
        }
        return Seq(Yield());
    }

    [Pure]
    public static Seq<A> AsEnumerable<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, R env, S state)
       where MonoidW : struct, Monoid<W> => ToSeq(self, env, state);

    [Pure]
    public static RWS<MonoidW, R, W, S, int> Count<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var (a, w, s, faulted) = self(env, state);
            return faulted
                ? (0, default(MonoidW).Empty(), state, false)
                : (1, w, s, false);
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, bool> Exists<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var (a, w, s, faulted) = self(env, state);
            return faulted
                ? (false, default(MonoidW).Empty(), state, false)
                : (pred(a), w, s, false);
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, bool> ForAll<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> => Exists(self, pred);

    [Pure]
    public static RWS<MonoidW, R, W, S, B> Fold<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, B initialValue, Func<B, A, B> f)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var (a, w, s, faulted) = self(env, state);
            return faulted
                ? (default(B), default(MonoidW).Empty(), state, faulted)
                : (f(initialValue, a), w, s, faulted);
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, R> Fold<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<R, A, R> f)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var (a, w, s, faulted) = self(env, state);
            return faulted
                ? (default(R), default(MonoidW).Empty(), state, faulted)
                : (f(env, a), w, s, faulted);
        };

    /// <summary>
    /// Monadic state transformer.
    /// Maps an old state to a new state inside a RWS monad.  The old state is thrown away.
    /// </summary>
    [Pure]
    public static RWS<MonoidW, R, W, S, Unit> Modify<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self, Func<S, S> f)
        where MonoidW : struct, Monoid<W> => (env, state) =>
            (unit, default(MonoidW).Empty(), f(state), false);
    [Pure]
    public static RWS<MonoidW, R, W, S, B> Map<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, Func<A, B> project)
        where MonoidW: struct, Monoid<W> => self.Select(project);

    /// <summary>
    /// Pass is an action that executes the monad, which
    /// returns a value and a function, and returns the value, applying
    /// the function to the output.
    /// </summary>
    [Pure]
    public static RWS<MonoidW, R, W, S, A> Pass<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, (A, Func<W, W>)> self)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var ((a, f), w, s, faulted) = self(env, state);
            return faulted
                ? (default(A), default(MonoidW).Empty(), state, faulted)
                : (a, f(w), s, faulted);
        };

    /// <summary>
    /// Listen is an action that executes the monad and adds
    /// its output to the value of the computation.
    /// </summary>
    [Pure]
    public static RWS<MonoidW, R, W, S, (A, B)> Listen<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, Func<W, B> f)
        where MonoidW : struct, Monoid<W> =>
            default(MRWS<MonoidW, R, W, S, A>).Listen(self, f);

    /// <summary>
    /// Censor is an action that executes the monad and applies the function f 
    /// to its output,  leaving the return value unchanged.
    /// </summary>
    [Pure]
    public static RWS<MonoidW, R, W, S, A> Censor<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<W, W> f)
        where MonoidW : struct, Monoid<W> =>
            Pass(
                default(MRWS<MonoidW, R, W, S, A>)
                    .Bind<MRWS<MonoidW, R, W, S, (A, Func<W, W>)>, RWS<MonoidW, R, W, S, (A, Func<W, W>)>, (A, Func<W, W>)>(
                    self, a =>
                        default(MRWS<MonoidW, R, W, S, (A, Func<W, W>)>)
                            .Return(_ => (a, f))));
    [Pure]
    public static RWS<MonoidW, R, W, S, B> Bind<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, Func<A, RWS<MonoidW, R, W, S, B>> f)
        where MonoidW : struct, Monoid<W> =>
        default(MRWS<MonoidW, R, W, S, A>).Bind<MRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(self, f); 

    [Pure]
    public static RWS<MonoidW, R, W, S, B> Select<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
        default(MRWS<MonoidW, R, W, S, A>).Bind<MRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(self, a =>
        default(MRWS<MonoidW, R, W, S, B>).Return(_ => f(a)));

    [Pure]
    public static RWS<MonoidW, R, W, S, C> SelectMany<MonoidW, R, W, S, A, B, C>(
        this RWS<MonoidW, R, W, S, A> self,
        Func<A, RWS<MonoidW, R, W, S, B>> bind,
        Func<A, B, C> project)
        where MonoidW : struct, Monoid<W> =>
            default(MRWS<MonoidW, R, W, S, A>).Bind<MRWS<MonoidW, R, W, S, C>, RWS<MonoidW, R, W, S, C>, C>(self, a =>
            default(MRWS<MonoidW, R, W, S, B>).Bind<MRWS<MonoidW, R, W, S, C>, RWS<MonoidW, R, W, S, C>, C>(bind(a), b =>
            default(MRWS<MonoidW, R, W, S, C>).Return(_ => project(a, b))));

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Filter<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var (a, w, s, faulted) = self(env, state);
            return (!pred(a) || faulted)
                ? (default(A), default(MonoidW).Empty(), state, true)
                : (a, w, s, faulted);
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Where<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> => Filter(self, pred);

    public static RWS<MonoidW, R, W, S, Unit> Iter<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Action<A> action)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var (a, w, s, faulted) = self(env, state);
            return faulted
                ? (unit, default(MonoidW).Empty(), state, false)
                : (fun(action)(a), w, s, false);
        };
}
