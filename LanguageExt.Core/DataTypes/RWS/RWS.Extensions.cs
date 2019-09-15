using System;
using LanguageExt;
using LanguageExt.Common;
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
    /// Monadic join
    /// </summary>
    [Pure]
    public static RWS<MonoidW, R, W, S, A> Flatten<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, RWS<MonoidW, R, W, S, A>> ma)
        where MonoidW : struct, Monoid<W> =>
        ma.Bind(identity);

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
            var res = self(env, state);
            if (res.IsFaulted)
            {
                return (() => Option<A>.None, default(MonoidW).Empty(), state);
            }
            else
            {
                return (() => Optional(res.Value), res.Output, res.State);
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
       where MonoidW : struct, Monoid<W> => self.Map(x => x.Cons());

    [Pure]
    public static RWS<MonoidW, R, W, S, Seq<A>> AsEnumerable<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self)
       where MonoidW : struct, Monoid<W> => ToSeq(self);

    [Pure]
    public static Seq<A> ToSeq<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, R env, S state)
       where MonoidW : struct, Monoid<W>
    {
        IEnumerable<A> Yield()
        {
            var res = self(env, state);
            if (!res.IsFaulted)
            {
                yield return res.Value;
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
            var res = self(env, state);
            return res.IsFaulted
                ? RWSResult<MonoidW, R, W, S, int>.New(res.Output, state, res.Error)
                : RWSResult<MonoidW, R, W, S, int>.New(res.Output, res.State, 1);
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, bool> Exists<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var res = self(env, state);
            return res.IsFaulted
                ? RWSResult<MonoidW, R, W, S, bool>.New(res.Output, state, false)
                : RWSResult<MonoidW, R, W, S, bool>.New(res.Output, res.State, pred(res.Value));
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, bool> ForAll<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> => Exists(self, pred);

    [Pure]
    public static RWS<MonoidW, R, W, S, B> Fold<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, B initialValue, Func<B, A, B> f)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var res = self(env, state);
            return res.IsFaulted
                ? RWSResult<MonoidW, R, W, S, B>.New(res.Output, state, res.Error)
                : RWSResult<MonoidW, R, W, S, B>.New(res.Output, res.State, f(initialValue, res.Value));
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, R> Fold<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<R, A, R> f)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var res = self(env, state);
            return res.IsFaulted
                ? RWSResult<MonoidW, R, W, S, R>.New(res.Output, state, res.Error)
                : RWSResult<MonoidW, R, W, S, R>.New(res.Output, res.State, f(env, res.Value));
        };

    /// <summary>
    /// Force evaluation of the monad (once only)
    /// </summary>
    [Pure]
    public static RWS<MonoidW, R, W, S, A> Strict<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> ma) where MonoidW : struct, Monoid<W>
    {
        Option<RWSResult<MonoidW, R, W, S, A>> cache = default;
        object sync = new object();
        return (env, state) =>
        {
            if (cache.IsSome) return cache.Value;
            lock (sync)
            {
                if (cache.IsSome) return cache.Value;
                cache = ma(env, state);
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
    public static RWS<MonoidW, R, W, S, A> Do<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> ma, Action<A> f) where MonoidW : struct, Monoid<W> =>
        (env, state) =>
        {
            var r = ma(env, state);
            if (!r.IsFaulted)
            {
                f(r.Value);
            }
            return r;
        };

    /// <summary>
    /// Monadic state transformer.
    /// Maps an old state to a new state inside a RWS monad.  The old state is thrown away.
    /// </summary>
    [Pure]
    public static RWS<MonoidW, R, W, S, Unit> Modify<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A> self, Func<S, S> f)
        where MonoidW : struct, Monoid<W> => (env, state) =>
            RWSResult<MonoidW, R, W, S, Unit>.New(default(MonoidW).Empty(), f(state), unit);

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
            var res = self(env, state);
            return res.IsFaulted
                ? RWSResult<MonoidW, R, W, S, A>.New(res.Output, state, res.Error)
                : RWSResult<MonoidW, R, W, S, A>.New(res.Value.Item2(res.Output), res.State, res.Value.Item1);
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
            var res = self(env, state);
            if (res.IsFaulted) return res;
            return pred(res.Value)
                ? res
                : RWSResult<MonoidW, R, W, S, A>.New(res.Output, state, Error.Bottom);
        };

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Where<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> => Filter(self, pred);

    public static RWS<MonoidW, R, W, S, Unit> Iter<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Action<A> action)
        where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var res = self(env, state);
            return res.IsFaulted
                ? RWSResult<MonoidW, R, W, S, Unit>.New(res.Output, state, res.Error)
                : RWSResult<MonoidW, R, W, S, Unit>.New(res.Output, res.State, fun(action)(res.Value));
        };
}
