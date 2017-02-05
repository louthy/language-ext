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
    public static (TryOption<A> Value, W Output) Run<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Option<W> output = default(Option<W>))
        where MonoidW : struct, Monoid<W>
    {
        try
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (output == null) throw new ArgumentNullException(nameof(output));
            var (a, output2, b) = Eval(self, output.IfNone(default(MonoidW).Empty()));
            if (b)
            {
                return (() => Option<A>.None, output.IfNone(default(MonoidW).Empty()));
            }
            else
            {
                return (() => Optional(a), output2);
            }
        }
        catch (Exception e)
        {
            return (() => new TryOptionResult<A>(e), default(MonoidW).Empty());
        }
    }

    internal static (A Value, W State, bool IsBottom) Eval<MonoidW, W, A>(this Writer<MonoidW, W, A> self, W output)
        where MonoidW : struct, Monoid<W> =>
        self == null || self.eval == null
            ? (default(A), default(MonoidW).Empty(), true) // bottom
            : self.eval(output);

    [Pure]
    public static Writer<MonoidW, W, int> Sum<MonoidW, W>(this Writer<MonoidW, W, int> self)
        where MonoidW : struct, Monoid<W> =>
        self;

    [Pure]
    public static Writer<MonoidW, W, IEnumerable<A>> AsEnumerable<MonoidW, W, A>(this Writer<MonoidW, W, A> self)
        where MonoidW : struct, Monoid<W> =>
            self.Select(x => (new A[1] { x }).AsEnumerable());

    [Pure]
    public static IEnumerable<A> AsEnumerable<MonoidW, W, A>(this Writer<MonoidW, W, A> self, W output)
        where MonoidW : struct, Monoid<W>
    {
        var (x, w, b) = self.Eval(output);
        if (!b)
        {
            yield return x;
        }
    }

    [Pure]
    public static Writer<MonoidW, W, int> Count<MonoidW, W>(this Writer<MonoidW, W, int> self)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, int>, Writer<MonoidW, W, int>, MonoidW, W, int>).Return(state =>
            {
                var (x, s, b) = self.Eval(state);
                return b
                    ? (0, default(MonoidW).Empty(), false)
                    : (1, s, false);
            });

    [Pure]
    public static Writer<MonoidW, W, bool> ForAll<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, bool>, Writer<MonoidW, W, bool>, MonoidW, W, bool>).Return(state =>
            {
                var (x, s, b) = self.Eval(state);
                return b
                    ? (false, default(MonoidW).Empty(), false)
                    : (pred(x), s, false);
            });

    [Pure]
    public static Writer<MonoidW, W, bool> Exists<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, bool>, Writer<MonoidW, W, bool>, MonoidW, W, bool>).Return(state =>
            {
                var (x, s, b) = self.Eval(state);
                return b
                    ? (false, default(MonoidW).Empty(), false)
                    : (pred(x), s, false);
            });

    [Pure]
    public static Writer<MonoidW, W, FState> Fold<FState, MonoidW, W, A>(this Writer<MonoidW, W, A> self, FState initialState, Func<FState, A, FState> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, FState>, Writer<MonoidW, W, FState>, MonoidW, W, FState>).Return(state =>
            {
                var (x, s, b) = self.Eval(state);
                return b
                    ? (default(FState), default(MonoidW).Empty(), true)
                    : (f(initialState, x), s, false);
            });

    [Pure]
    public static Writer<MonoidW, W, W> Fold<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<W, A, W> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, W>, Writer<MonoidW, W, W>, MonoidW, W, W>).Return(state =>
            {
                var (x, s, b) = self.Eval(state);
                if (b) return (default(MonoidW).Empty(), default(MonoidW).Empty(), true);
                return (f(s, x), s, false);
            });

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
        default(MWriter<SWriter<MonoidW, W, A>, Writer<MonoidW, W, A>, MonoidW, W, A>).Return(output =>
        {
            var ((a, f), w, b) = self.Eval(output);
            if (b) return (default(A), default(MonoidW).Empty(), true);
            return (a, default(MonoidW).Append(output, f(w)), false);
        });

    /// <summary>
    /// listen is an action that executes the monad and adds
    /// its output to the value of the computation.
    /// </summary>
    [Pure]
    public static Writer<MonoidW, W, (A, W)> Listen<MonoidW, W, A>(this Writer<MonoidW, W, A> self)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, A>, Writer<MonoidW, W, A>, MonoidW, W, A>)
                .Listen<SWriter<MonoidW, W, (A, W)>, Writer<MonoidW, W, (A, W)>>(self);

    /// <summary>
    /// Censor is an action that executes the writer monad and applies the function f 
    /// to its output, leaving the return value, leaving the return value
    /// unchanged.
    /// </summary>
    public static Writer<MonoidW, W, A> Censor<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<W, W> f)
        where MonoidW : struct, Monoid<W> =>
            Pass(
                default(MWriter<SWriter<MonoidW, W, A>, Writer<MonoidW, W, A>, MonoidW, W, A>)
                    .Bind<MWriter<SWriter<MonoidW, W, (A, Func<W, W>)>, Writer<MonoidW, W, (A, Func<W, W>)>, MonoidW, W, (A, Func<W, W>)>, Writer<MonoidW, W, (A, Func<W, W>)>, (A, Func<W, W>)>(self, a =>
                        default(MWriter<SWriter<MonoidW, W, (A, Func<W, W>)>, Writer<MonoidW, W, (A, Func<W, W>)>, MonoidW, W, (A, Func<W, W>)>)
                            .Return((a, f))));

    /// <summary>
    /// listens f is an action that executes the writer monad and adds
    /// the result of applying f to the output to the value of the computation.
    /// </summary>
    [Pure]
    public static Writer<MonoidW, W, (A, B)> Listens<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<W, B> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, (A, W)>, Writer<MonoidW, W, (A, W)>, MonoidW, W, (A, W)>)
                .Bind<MWriter<SWriter<MonoidW, W, (A, B)>, Writer<MonoidW, W, (A, B)>, MonoidW, W, (A, B)>, Writer<MonoidW, W, (A, B)>, (A, B)>(
                    Listen(self), aw =>
                        default(MWriter<SWriter<MonoidW, W, (A, B)>, Writer<MonoidW, W, (A, B)>, MonoidW, W, (A, B)>)
                            .Return((aw.Item1, f(aw.Item2))));

    [Pure]
    public static Writer<MonoidW, W, B> Bind<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, Writer<MonoidW, W, B>> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, A>, Writer<MonoidW, W, A>, MonoidW, W, A>)
                .Bind<MWriter<SWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, MonoidW, W, B>, Writer<MonoidW, W, B>, B>(self, f);

    [Pure]
    public static Writer<MonoidW, W, B> Select<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, A>, Writer<MonoidW, W, A>, MonoidW, W, A>).Bind<MWriter<SWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, MonoidW, W, B>, Writer<MonoidW, W, B>, B>(self, a =>
            default(MWriter<SWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, MonoidW, W, B>).Return(f(a)));

    [Pure]
    public static Writer<MonoidW, W, C> SelectMany<MonoidW, W, A, B, C>(
        this Writer<MonoidW, W, A> self,
        Func<A, Writer<MonoidW, W, B>> bind,
        Func<A, B, C> project)
            where MonoidW : struct, Monoid<W> =>
                default(MWriter<SWriter<MonoidW, W, A>, Writer<MonoidW, W, A>, MonoidW, W, A>).Bind<MWriter<SWriter<MonoidW, W, C>, Writer<MonoidW, W, C>, MonoidW, W, C>, Writer<MonoidW, W, C>, C>(self, a =>
                default(MWriter<SWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, MonoidW, W, B>).Bind<MWriter<SWriter<MonoidW, W, C>, Writer<MonoidW, W, C>, MonoidW, W, C>, Writer<MonoidW, W, C>, C>(bind(a), b =>
                default(MWriter<SWriter<MonoidW, W, C>, Writer<MonoidW, W, C>, MonoidW, W, C>).Return(project(a, b))));

    [Pure]
    public static Writer<MonoidW, W, A> Filter<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            self.Where(pred);

    [Pure]
    public static Writer<MonoidW, W, A> Where<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, A>, Writer<MonoidW, W, A>, MonoidW, W, A>).Return(state => {
                var (x, s, b) = self.Eval(state);
                if (b) return (default(A), default(MonoidW).Empty(), true);
                if (!pred(x)) return (default(A), state, true);
                return (x, s, b);
            });

    public static Writer<MonoidW, W, Unit> Iter<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Action<A> action)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<SWriter<MonoidW, W, Unit>, Writer<MonoidW, W, Unit>, MonoidW, W, Unit>).Return(state => {
                var (x, s, b) = self.Eval(state);
                if (!b) action(x);
                var ns = b ? state : s;
                return (unit, ns, false);
            });
}
