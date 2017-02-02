using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;

/// <summary>
/// Extension methods for Option
/// </summary>
public static class StateExtensions
{
    /// <summary>
    /// Runs the State monad and memoizes the result in a TryOption monad.  Use
    /// Match, IfSucc, IfNone, etc to extract.
    /// </summary>
    public static (TryOption<A> Value, S State) Run<S, A>(this State<S, A> self, S state)
    {
        try
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (state == null) throw new ArgumentNullException(nameof(state));
            var (a, s, b) = Eval(self, state);
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
            return (() => new TryOptionResult<A>(e), state);
        }
    }

    internal static (A Value, S State, bool IsBottom) Eval<S, A>(this State<S, A> self, S env) =>
        self == null || self.eval == null
            ? (default(A), default(S), true) // bottom
            : self.eval(env);

    [Pure]
    public static State<S, int> Sum<S>(this State<S, int> self) =>
        self;

    [Pure]
    public static State<S, IEnumerable<A>> AsEnumerable<S, A>(this State<S, A> self) =>
        self.Select(x => (new A[1] { x }).AsEnumerable());

    [Pure]
    public static IEnumerable<A> AsEnumerable<S, A>(this State<S, A> self, S state)
    {
        var (x, s, b) = self.Eval(state);
        if (!b)
        {
            yield return x;
        }
    }

    [Pure]
    public static State<S, int> Count<S>(this State<S, int> self) =>
        default(MState<S, int>).Return(state =>
        {
            var (x, s, b) = self.Eval(state);
            return b
                ? (0, state, false)
                : (1, s, false);
        });

    [Pure]
    public static State<S, bool> ForAll<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        default(MState<S, bool>).Return(state =>
        {
            var (x, s, b) = self.Eval(state);
            return b
                ? (false, state, false)
                : (pred(x), s, false);
        });

    [Pure]
    public static State<S, bool> Exists<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        default(MState<S, bool>).Return(state =>
        {
            var (x, s, b) = self.Eval(state);
            return b
                ? (false, state, false)
                : (pred(x), s, false);
        });

    [Pure]
    public static State<S, FState> Fold<FState, S, A>(this State<S, A> self, FState initialState, Func<FState, A, FState> f) =>
        default(MState<S, FState>).Return(state =>
        {
            var (x, s, b) = self.Eval(state);
            return b
                ? (default(FState), state, true)
                : (f(initialState, x), s, false);
        });

    [Pure]
    public static State<S, S> Fold<S, A>(this State<S, A> self, Func<S, A, S> f) =>
        default(MState<S, S>).Return(state =>
        {
            var (x, s, b) = self.Eval(state);
            return b
                ? (default(S), state, true)
                : (f(s, x), s, false);
        });

    [Pure]
    public static State<S, B> Map<S, A, B>(this State<S, A> self, Func<A, B> f) =>
        self.Select(f);

    /// <summary>
    /// modify::MonadState s m => (s -> s) -> m()
    /// 
    /// Monadic state transformer.
    /// 
    /// Maps an old state to a new state inside a state monad.The old state is thrown away.
    /// </summary>
    [Pure]
    public static State<S, Unit> Modify<S, A>(this State<S, A> self, Func<S, S> f) =>
        default(MState<S, Unit>).State(s => (unit, f(s), false));

    [Pure]
    public static State<S, B> Bind<S, A, B>(this State<S, A> self, Func<A, State<S, B>> f) =>
        default(MState<S, A>).Bind<MState<S, B>, State<S, B>, B>(self, f);

    [Pure]
    public static State<S, B> Select<S, A, B>(this State<S, A> self, Func<A, B> f) =>
        default(MState<S, A>).Bind<MState<S, B>, State<S, B>, B>(self, a =>
        default(MState<S, B>).Return(f(a)));

    [Pure]
    public static State<S, C> SelectMany<S, A, B, C>(
        this State<S, A> self,
        Func<A, State<S, B>> bind,
        Func<A, B, C> project) =>
            default(MState<S, A>).Bind<MState<S, C>, State<S, C>, C>(self, a =>
            default(MState<S, B>).Bind<MState<S, C>, State<S, C>, C>(bind(a), b =>
            default(MState<S, C>).Return(project(a, b))));

    [Pure]
    public static State<S, A> Filter<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        self.Where(pred);

    [Pure]
    public static State<S, A> Where<S, A>(this State<S, A> self, Func<A, bool> pred) =>
        default(MState<S, A>).Return(state => {
            var (x, s, b) = self.Eval(state);
            if (b || !pred(x)) return (default(A), state, true);
            return (x, s, b);
        });

    public static State<S, Unit> Iter<S, A>(this State<S, A> self, Action<A> action) =>
        default(MState<S, Unit>).Return(state => {
            var (x, s, b) = self.Eval(state);
            if (!b) action(x);
            var ns = b ? state : s;
            return (unit, ns, false);
        });

}
