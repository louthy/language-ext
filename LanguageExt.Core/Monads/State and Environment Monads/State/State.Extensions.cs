using System;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

/// <summary>
/// Extension methods for State
/// </summary>
public static class StateExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static State<S, A> Flatten<S, A>(this State<S, State<S, A>> ma) =>
        new(ma.Bind(x => x));

    [Pure]
    public static State<S, int> Sum<S>(this State<S, int> self) =>
        self;

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
