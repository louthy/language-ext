using System;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

/// <summary>
/// Extension methods for State
/// </summary>
public static class StateExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static State<Env, A> Flatten<Env, A>(this State<Env, State<Env, A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Runs the State monad and memoizes the result in a TryOption monad.  Use
    /// Match, IfSucc, IfNone, etc to extract.
    /// </summary>
    public static (TryOption<A> Value, S State) Run<S, A>(this State<S, A> self, S state)
    {
        try
        {
            if (self == null) return (() => Option<A>.None, state);
            if (state == null) return (() => Option<A>.None, state);
            var (a, s, b) = self(state);
            if (b)
            {
                return (() => Option<A>.None, state);
            }
            else
            {
                return (() => Optional(a), s);
            }
        }
        catch (Exception e)
        {
            return (() => new OptionalResult<A>(e), state);
        }
    }

    [Pure]
    public static State<S, int> Sum<S>(this State<S, int> self) =>
        self;

    [Pure]
    public static State<S, Seq<A>> ToSeq<S, A>(this State<S, A> self) =>
        self.Select(x => x.Cons());

    [Pure]
    public static Seq<A> ToSeq<S, A>(this State<S, A> self, S state)
    {
        IEnumerable<A> Yield()
        {
            var (x, s, b) = self(state);
            if (!b)
            {
                yield return x;
            }
        }
        return Seq(Yield());
    }

    [Pure]
    public static State<S, Seq<A>> AsEnumerable<S, A>(this State<S, A> self) =>
        ToSeq(self);

    [Pure]
    public static Seq<A> AsEnumerable<S, A>(this State<S, A> self, S state) =>
        ToSeq(self, state);

    [Pure]
    public static State<S, int> Count<S>(this State<S, int> self) =>
        state =>
        {
            var (x, s, b) = self(state);
            return b
                ? (0, state, false)
                : (1, s, false);
        };

    [Pure]
    public static State<S, bool> ForAll<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        state =>
        {
            var (x, s, b) = self(state);
            return b
                ? (false, state, false)
                : (pred(x), s, false);
        };

    [Pure]
    public static State<S, bool> Exists<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        state =>
        {
            var (x, s, b) = self(state);
            return b
                ? (false, state, false)
                : (pred(x), s, false);
        };

    [Pure]
    public static State<S, FState> Fold<FState, S, A>(this State<S, A> self, FState initialState, Func<FState, A, FState> f) =>
        state =>
        {
            var (x, s, b) = self(state);
            return b
                ? (default(FState), state, true)
                : (f(initialState, x), s, false);
        };

    [Pure]
    public static State<S, S> Fold<S, A>(this State<S, A> self, Func<S, A, S> f) =>
        state =>
        {
            var (x, s, b) = self(state);
            return b
                ? (default(S), state, true)
                : (f(s, x), s, false);
        };

    /// <summary>
    /// Force evaluation of the monad (once only)
    /// </summary>
    [Pure]
    public static State<S, A> Strict<S, A>(this State<S, A> ma)
    {
        Option<(A, S, bool)> cache = default;
        object sync = new object();
        return state =>
        {
            if (cache.IsSome) return cache.Value;
            lock (sync)
            {
                if (cache.IsSome) return cache.Value;
                cache = ma(state);
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
    public static State<S, A> Do<S, A>(this State<S, A> ma, Action<A> f)
    {
        ma = ma.Strict();
        ma.Iter(f);
        return ma;
    }

    [Pure]
    public static State<S, B> Map<S, A, B>(this State<S, A> self, Func<A, B> f) =>
        self.Select(f);

    /// <summary>
    /// Monadic state transformer.
    /// Maps an old state to a new state inside a state monad.  The old state is thrown away.
    /// </summary>
    [Pure]
    public static State<S, Unit> Modify<S, A>(this State<S, A> self, Func<S, S> f) => state =>
        (unit, f(state), false);

    [Pure]
    public static State<S, B> Bind<S, A, B>(this State<S, A> self, Func<A, State<S, B>> f) =>
        default(MState<S, A>).Bind<MState<S, B>, State<S, B>, B>(self, f);

    [Pure]
    public static State<S, B> Select<S, A, B>(this State<S, A> self, Func<A, B> f) =>
        default(MState<S, A>).Bind<MState<S, B>, State<S, B>, B>(self, a =>
        default(MState<S, B>).Return(_ => f(a)));

    [Pure]
    public static State<S, C> SelectMany<S, A, B, C>(
        this State<S, A> self,
        Func<A, State<S, B>> bind,
        Func<A, B, C> project) =>
            default(MState<S, A>).Bind<MState<S, C>, State<S, C>, C>(self, a =>
            default(MState<S, B>).Bind<MState<S, C>, State<S, C>, C>(bind(a), b =>
            default(MState<S, C>).Return(_ => project(a, b))));

    [Pure]
    public static State<S, A> Filter<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        self.Where(pred);

    [Pure]
    public static State<S, A> Where<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        state => {
            var (x, s, b) = self(state);
            if (b || !pred(x)) return (default(A), state, true);
            return (x, s, b);
        };

    public static State<S, Unit> Iter<S, A>(this State<S, A> self, Action<A> action) =>
        state => {
            var (x, s, b) = self(state);
            if (!b) action(x);
            var ns = b ? state : s;
            return (unit, ns, false);
        };

}
