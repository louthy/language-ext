using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

/// <summary>
/// Extension methods for Writer
/// </summary>
public static class WriterExtensions
{
    /// <summary>
    /// Runs the Writer monad and memoizes the result in a TryOption monad.  Use
    /// Match, IfSucc, IfNone, etc to extract.
    /// </summary>
    public static (TryOption<A> Value, W Output) Run<MonoidW, W, A>(this Writer<MonoidW, W, A> self)
        where MonoidW : struct, Monoid<W>
    {
        try
        {
            if (self == null) return (() => Option<A>.None, default(MonoidW).Empty());
            var (a, output2, b) = self();
            if (b)
            {
                return (() => Option<A>.None, default(MonoidW).Empty());
            }
            else
            {
                return (() => Optional(a), output2);
            }
        }
        catch (Exception e)
        {
            return (() => new OptionalResult<A>(e), default(MonoidW).Empty());
        }
    }

    [Pure]
    public static Writer<MonoidW, W, int> Sum<MonoidW, W>(this Writer<MonoidW, W, int> self)
        where MonoidW : struct, Monoid<W> =>
        self;

    [Pure]
    public static Writer<MonoidW, W, Seq<A>> ToSeq<MonoidW, W, A>(this Writer<MonoidW, W, A> self)
        where MonoidW : struct, Monoid<W> =>
            self.Select(x => x.Cons(Empty));

    [Pure]
    public static Writer<MonoidW, W, Seq<A>> AsEnumerable<MonoidW, W, A>(this Writer<MonoidW, W, A> self)
        where MonoidW : struct, Monoid<W> =>
            self.ToSeq();

    [Pure]
    public static Writer<MonoidW, W, int> Count<MonoidW, W>(this Writer<MonoidW, W, int> self)
        where MonoidW : struct, Monoid<W> =>
            () =>
            {
                var (x, s, b) = self();
                return b
                    ? (0, default(MonoidW).Empty(), false)
                    : (1, s, false);
            };

    [Pure]
    public static Writer<MonoidW, W, bool> ForAll<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            () =>
            {
                var (x, s, b) = self();
                return b
                    ? (false, default(MonoidW).Empty(), false)
                    : (pred(x), s, false);
            };

    [Pure]
    public static Writer<MonoidW, W, bool> Exists<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            () =>
            {
                var (x, s, b) = self();
                return b
                    ? (false, default(MonoidW).Empty(), false)
                    : (pred(x), s, false);
            };

    [Pure]
    public static Writer<MonoidW, W, FState> Fold<FState, MonoidW, W, A>(this Writer<MonoidW, W, A> self, FState initialState, Func<FState, A, FState> f)
        where MonoidW : struct, Monoid<W> =>
            () =>
            {
                var (x, s, b) = self();
                return b
                    ? (default(FState), default(MonoidW).Empty(), true)
                    : (f(initialState, x), s, false);
            };

    [Pure]
    public static Writer<MonoidW, W, W> Fold<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<W, A, W> f)
        where MonoidW : struct, Monoid<W> =>
            () =>
            {
                var (x, s, b) = self();
                if (b) return (default(MonoidW).Empty(), default(MonoidW).Empty(), true);
                return (f(s, x), s, false);
            };

    [Pure]
    public static Writer<MonoidW, W, B> Map<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
            self.Select(f);

    /// <summary>
    /// pass is an action that executes the monad, which
    /// returns a value and a function, and returns the value, applying
    /// the function to the output.
    /// </summary>
    [Pure]
    public static Writer<MonoidW, W, A> Pass<MonoidW, W, A>(this Writer<MonoidW, W, (A, Func<W, W>)> self)
        where MonoidW : struct, Monoid<W> =>
        () =>
        {
            var ((a, f), w, b) = self();
            if (b) return (default(A), default(MonoidW).Empty(), true);
            return (a, f(w), false);
        };

    /// <summary>
    /// listen is an action that executes the monad and adds
    /// its output to the value of the computation.
    /// </summary>
    [Pure]
    public static Writer<MonoidW, W, (A, B)> Listen<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<W, B> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<MonoidW, W, A>).Listen(self, f);

    /// <summary>
    /// Censor is an action that executes the writer monad and applies the function f 
    /// to its output, leaving the return value, leaving the return value
    /// unchanged.
    /// </summary>
    public static Writer<MonoidW, W, A> Censor<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<W, W> f)
        where MonoidW : struct, Monoid<W> =>
            Pass(
                default(MWriter<MonoidW, W, A>)
                    .Bind<MWriter<MonoidW, W, (A, Func<W, W>)>, Writer<MonoidW, W, (A, Func<W, W>)>, (A, Func<W, W>)>(self, a =>
                        default(MWriter<MonoidW, W, (A, Func<W, W>)>)
                            .Return(_ => (a, f))));

    [Pure]
    public static Writer<MonoidW, W, B> Bind<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, Writer<MonoidW, W, B>> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<MonoidW, W, A>)
                .Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(self, f);

    [Pure]
    public static Writer<MonoidW, W, B> Select<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<MonoidW, W, A>).Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(self, a =>
            default(MWriter<MonoidW, W, B>).Return(_ => f(a)));

    [Pure]
    public static Writer<MonoidW, W, C> SelectMany<MonoidW, W, A, B, C>(
        this Writer<MonoidW, W, A> self,
        Func<A, Writer<MonoidW, W, B>> bind,
        Func<A, B, C> project)
            where MonoidW : struct, Monoid<W> =>
                default(MWriter<MonoidW, W, A>).Bind<MWriter<MonoidW, W, C>, Writer<MonoidW, W, C>, C>(self, a =>
                default(MWriter<MonoidW, W, B>).Bind<MWriter<MonoidW, W, C>, Writer<MonoidW, W, C>, C>(bind(a), b =>
                default(MWriter<MonoidW, W, C>).Return(_ => project(a, b))));

    [Pure]
    public static Writer<MonoidW, W, A> Filter<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            self.Where(pred);

    [Pure]
    public static Writer<MonoidW, W, A> Where<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            () => {
                var (x, s, b) = self();
                if (b) return (default(A), default(MonoidW).Empty(), true);
                if (!pred(x)) return (default(A), default(MonoidW).Empty(), true);
                return (x, s, b);
            };

    public static Writer<MonoidW, W, Unit> Iter<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Action<A> action)
        where MonoidW : struct, Monoid<W> =>
            () => {
                var (x, s, b) = self();
                if (!b) action(x);
                return (unit, s, false);
            };
}
