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
/// RWS monad extensions
/// </summary>
public static class RWSExt
{
    /// <summary>
    /// Runs the RWS monad and memoizes the result in a TryOption monad.  Use
    /// Match, IfSucc, IfNone, etc to extract.
    /// </summary>
    public static (TryOption<A> Value, W Output, S State) Run<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, R env, S state, Option<W> output = default(Option<W>))
        where MonoidW : struct, Monoid<W>
    {
        try
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (output == null) throw new ArgumentNullException(nameof(output));
            var (a, (r,w,s), b) = Eval(self, env, output.IfNone(default(MonoidW).Empty()), state);
            if (b)
            {
                return (() => Option<A>.None, default(MonoidW).Empty(), state);
            }
            else
            {
                return (() => new TryOptionResult<A>(Optional(a)), w, s);
            }
        }
        catch (Exception e)
        {
            return (() => new TryOptionResult<A>(e), default(MonoidW).Empty(), state);
        }
    }


    internal static (A Value, (R Reader, W Output, S State), bool IsBottom) Eval<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, R env, W output, S state)
        where MonoidW : struct, Monoid<W> =>
            self == null || self.eval == null
                ? (default(A), (env, default(MonoidW).Empty(), state), true) // bottom
                : self.eval((env, output, state));

    [Pure]
    public static RWS<MonoidW, R, W, S, IEnumerable<T>> AsEnumerable<MonoidW, R, W, S, T>(this RWS<MonoidW, R, W, S, T> self)
        where MonoidW : struct, Monoid<W> =>
            self.Select(x => (new T[1] { x }).AsEnumerable());

    [Pure]
    public static IEnumerable<A> AsEnumerable<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, R env, S state)
        where MonoidW : struct, Monoid<W> =>
            self.Run(env, state).Value.AsEnumerable().Rights();

    public static RWS<MonoidW, R, W, S, Unit> Iter<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Action<A> action)
        where MonoidW : struct, Monoid<W> =>
            self.Select(x => { action(x); return unit; });

    [Pure]
    public static RWS<MonoidW, R, W, S, bool> ForAll<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            self.Select(pred);

    [Pure]
    public static RWS<MonoidW, R, W, S, bool> Exists<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            self.Select(pred);

    [Pure]
    public static RWS<MonoidW, R, W, S, FState> Fold<MonoidW, R, W, S, A, FState>(this RWS<MonoidW, R, W, S, A> self, FState state, Func<FState, A, FState> f)
        where MonoidW : struct, Monoid<W> =>
            self.Select(x => f(state, x));

    [Pure]
    public static RWS<MonoidW, R, W, S, S> Fold<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<S, A, S> f)
        where MonoidW : struct, Monoid<W> =>
            from a in self
            from s in default(SRWS<MonoidW, R, W, S, S>).Lift(((R,W,S) rws) => (rws.Item3, (rws), false))
            select f(s, a);

    [Pure]
    public static RWS<MonoidW, R, W, S, B> Map<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
            self.Select(f);

    [Pure]
    public static RWS<MonoidW, R, W, S, B> Bind<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, Func<A, RWS<MonoidW, R, W, S, B>> f)
        where MonoidW : struct, Monoid<W> =>
            default(MRWS<SRWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, A>, MonoidW, R, W, S, A>)
                .Bind<MRWS<SRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(self, f);

    [Pure]
    public static RWS<MonoidW, R, W, S, B> Select<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
            default(MRWS<SRWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, A>, MonoidW, R, W, S, A>)
                .Bind<MRWS<SRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(self, a =>
                    default(MRWS<SRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, MonoidW, R, W, S, B>).Return(f(a)));

    [Pure]
    public static RWS<MonoidW, R, W, S, C> SelectMany<MonoidW, R, W, S, A, B, C>(
        this RWS<MonoidW, R, W, S, A> self,
        Func<A, RWS<MonoidW, R, W, S, B>> bind,
        Func<A, B, C> project
        )
        where MonoidW : struct, Monoid<W> =>
            default(MRWS<SRWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, A>, MonoidW, R, W, S, A>)
                .Bind<MRWS<SRWS<MonoidW, R, W, S, C>, RWS<MonoidW, R, W, S, C>, MonoidW, R, W, S, C>, RWS<MonoidW, R, W, S, C>, C>(self, a =>
                    default(MRWS<SRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, MonoidW, R, W, S, B>)
                        .Bind<MRWS<SRWS<MonoidW, R, W, S, C>, RWS<MonoidW, R, W, S, C>, MonoidW, R, W, S, C>, RWS<MonoidW, R, W, S, C>, C>(bind(a), b =>
                            default(MRWS<SRWS<MonoidW, R, W, S, C>, RWS<MonoidW, R, W, S, C>, MonoidW, R, W, S, C>).Return(project(a, b))));

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Filter<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            self.Where(pred);

    [Pure]
    public static RWS<MonoidW, R, W, S, A> Where<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            new RWS<MonoidW, R, W, S, A>(rws =>
            {
                var (a, (r, w, s), b) = self.Eval(rws);
                return (b || !pred(a))
                    ? (default(A), rws, true)
                    : (a, (r, w, s), false);
            });

    [Pure]
    public static RWS<MonoidW, R, W, S, int> Sum<MonoidW, R, W, S>(this RWS<MonoidW, R, W, S, int> self)
        where MonoidW : struct, Monoid<W> =>
            self;
}