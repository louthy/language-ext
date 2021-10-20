using System;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using LanguageExt.Common;

/// <summary>
/// Extension methods for Writer
/// </summary>
public static class WriterExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Writer<MSeq<W>, Seq<W>, A> Flatten<W, A>(this Writer<MSeq<W>, Seq<W>, Writer<MSeq<W>, Seq<W>, A>> ma) =>
        ma.Flatten<MSeq<W>, Seq<W>, A>();

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Writer<MonoidW, W, A> Flatten<MonoidW, W, A>(this Writer<MonoidW, W, Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
        ma.Bind(identity);

    /// <summary>
    /// Conversion from tuple to writer
    /// </summary>
    /// <typeparam name="W">Type of the writer monad's output</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Tuple to convert to writeer</param>
    /// <returns>Writer monad</returns>
    public static Writer<MSeq<W>, Seq<W>, A> ToWriter<W, A>(this (A Value, Seq<W> Output) ma)=> () => 
        ma.Add(false);

    /// <summary>
    /// Conversion from tuple to writer
    /// </summary>
    /// <typeparam name="W">Type of the writer monad's output</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Tuple to convert to writeer</param>
    /// <returns>Writer monad</returns>
    public static Writer<MonoidW, W, A> ToWriter<MonoidW, W, A>(this (A Value, W Output) ma)
        where MonoidW : struct, Monoid<W> =>
            () => ma.Add(false);

    /// <summary>
    /// Runs the Writer monad and memoizes the result in a TryOption monad.  Use
    /// Match, IfSucc, IfNone, etc to extract.
    /// </summary>
    public static (TryOption<A> Value, Seq<W> Output) Run<W, A>(this Writer<MSeq<W>, Seq<W>, A> self) =>
        self.Run<MSeq<W>, Seq<W>, A>();

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
    public static Writer<MSeq<W>, Seq<W>, Seq<A>> ToSeq<W, A>(this Writer<MSeq<W>, Seq<W>, A> self) =>
        self.ToSeq<MSeq<W>, Seq<W>, A>();

    [Pure]
    public static Writer<MonoidW, W, Seq<A>> ToSeq<MonoidW, W, A>(this Writer<MonoidW, W, A> self)
        where MonoidW : struct, Monoid<W> =>
            self.Select(x => x.Cons());

    [Pure]
    public static Writer<MonoidW, W, Seq<A>> AsEnumerable<MonoidW, W, A>(this Writer<MonoidW, W, A> self)
        where MonoidW : struct, Monoid<W> =>
            self.ToSeq();

    [Pure]
    public static Writer<MSeq<W>, Seq<W>, int> Count<W>(this Writer<MSeq<W>, Seq<W>, int> self) =>
        self.Count<MSeq<W>, Seq<W>>();

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
    public static Writer<MSeq<W>, Seq<W>, bool> ForAll<W, A>(this Writer<MSeq<W>, Seq<W>, A> self, Func<A, bool> pred) =>
        self.ForAll<MSeq<W>, Seq<W>, A>(pred);

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
    public static Writer<MSeq<W>, Seq<W>, bool> Exists<W, A>(this Writer<MSeq<W>, Seq<W>, A> self, Func<A, bool> pred) =>
        self.Exists<MSeq<W>, Seq<W>, A>(pred);

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
    public static Writer<MSeq<W>, Seq<W>, FState> Fold<FState, W, A>(this Writer<MSeq<W>, Seq<W>, A> self, FState initialState, Func<FState, A, FState> f) =>
        self.Fold<FState, MSeq<W>, Seq<W>, A>(initialState, f);

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
    public static Writer<MSeq<W>, Seq<W>, Seq<W>> Fold<W, A>(this Writer<MSeq<W>, Seq<W>, A> self, Func<Seq<W>, A, Seq<W>> f) =>
        self.Fold<MSeq<W>, Seq<W>, A>(f);

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
    public static Writer<MSeq<W>, Seq<W>, B> Map<W, A, B>(this Writer<MSeq<W>, Seq<W>, A> self, Func<A, B> f) =>
        self.Map<MSeq<W>, Seq<W>, A, B>(f);

    [Pure]
    public static Writer<MonoidW, W, B> Map<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
            self.Select(f);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static Writer<MSeq<W>, Seq<W>, A> Do<W, A>(this Writer<MSeq<W>, Seq<W>, A> ma, Action<A> f) =>
        ma.Do<MSeq<W>, Seq<W>, A>(f);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static Writer<MonoidW, W, A> Do<MonoidW, W, A>(this Writer<MonoidW, W, A> ma, Action<A> f) where MonoidW : struct, Monoid<W>
    {
        ma = ma.Strict();
        ma.Iter(f);
        return ma;
    }

    /// <summary>
    /// Force evaluation of the Writer 
    /// </summary>
    public static Writer<MSeq<W>, Seq<W>, A> Strict<W, A>(this Writer<MSeq<W>, Seq<W>, A> ma) =>
        ma.Strict<MSeq<W>, Seq<W>, A>();

    /// <summary>
    /// Force evaluation of the Writer 
    /// </summary>
    public static Writer<MonoidW, W, A> Strict<MonoidW, W, A>(this Writer<MonoidW, W, A> ma) where MonoidW : struct, Monoid<W>
    {
        var r = ma();
        return () => r;
    }

    /// <summary>
    /// pass is an action that executes the monad, which
    /// returns a value and a function, and returns the value, applying
    /// the function to the output.
    /// </summary>
    [Pure]
    public static Writer<MSeq<W>, Seq<W>, A> Pass<W, A>(this Writer<MSeq<W>, Seq<W>, (A, Func<Seq<W>, Seq<W>>)> self) =>
        self.Pass<MSeq<W>, Seq<W>, A>();

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
    public static Writer<MSeq<W>, Seq<W>, (A, B)> Listen<W, A, B>(this Writer<MSeq<W>, Seq<W>, A> self, Func<Seq<W>, B> f) =>
        default(MWriter<MSeq<W>, Seq<W>, A>).Listen(self, f);

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
    /// to its output,  leaving the return value unchanged.
    /// </summary>
    public static Writer<MSeq<W>, Seq<W>, A> Censor<W, A>(this Writer<MSeq<W>, Seq<W>, A> self, Func<Seq<W>, Seq<W>> f) =>
        self.Censor<MSeq<W>, Seq<W>, A>(f);

    /// <summary>
    /// Censor is an action that executes the writer monad and applies the function f 
    /// to its output,  leaving the return value unchanged.
    /// </summary>
    public static Writer<MonoidW, W, A> Censor<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<W, W> f)
        where MonoidW : struct, Monoid<W> =>
            Pass(
                default(MWriter<MonoidW, W, A>)
                    .Bind<MWriter<MonoidW, W, (A, Func<W, W>)>, Writer<MonoidW, W, (A, Func<W, W>)>, (A, Func<W, W>)>(self, a =>
                        default(MWriter<MonoidW, W, (A, Func<W, W>)>)
                            .Return(_ => (a, f))));


    [Pure]
    public static Writer<MSeq<W>, Seq<W>, B> Bind<W, A, B>(this Writer<MSeq<W>, Seq<W>, A> self, Func<A, Writer<MSeq<W>, Seq<W>, B>> f) =>
        self.Bind<MSeq<W>, Seq<W>, A, B>(f);

    [Pure]
    public static Writer<MonoidW, W, B> Bind<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, Writer<MonoidW, W, B>> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<MonoidW, W, A>)
                .Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(self, f);

    [Pure]
    public static Writer<MSeq<W>, Seq<W>, B> Select<W, A, B>(this Writer<MSeq<W>, Seq<W>, A> self, Func<A, B> f) =>
        self.Select<MSeq<W>, Seq<W>, A, B>(f);

    [Pure]
    public static Writer<MonoidW, W, B> Select<MonoidW, W, A, B>(this Writer<MonoidW, W, A> self, Func<A, B> f)
        where MonoidW : struct, Monoid<W> =>
            default(MWriter<MonoidW, W, A>).Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(self, a =>
            default(MWriter<MonoidW, W, B>).Return(_ => f(a)));

    [Pure]
    public static Writer<MSeq<W>, Seq<W>, C> SelectMany<W, A, B, C>(
        this Writer<MSeq<W>, Seq<W>, A> self,
        Func<A, Writer<MSeq<W>, Seq<W>, B>> bind,
        Func<A, B, C> project) =>
        self.SelectMany<MSeq<W>, Seq<W>, A, B, C>(bind, project);

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
    public static Writer<MSeq<W>, Seq<W>, A> Filter<W, A>(this Writer<MSeq<W>, Seq<W>, A> self, Func<A, bool> pred) =>
        self.Where(pred);

    [Pure]
    public static Writer<MonoidW, W, A> Filter<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            self.Where(pred);

    [Pure]
    public static Writer<MSeq<W>, Seq<W>, A> Where<W, A>(this Writer<MSeq<W>, Seq<W>, A> self, Func<A, bool> pred) =>
        self.Where<MSeq<W>, Seq<W>, A>(pred);

    [Pure]
    public static Writer<MonoidW, W, A> Where<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Func<A, bool> pred)
        where MonoidW : struct, Monoid<W> =>
            () => {
                var (x, s, b) = self();
                if (b) return (default(A), default(MonoidW).Empty(), true);
                if (!pred(x)) return (default(A), default(MonoidW).Empty(), true);
                return (x, s, b);
            };

    public static Writer<MSeq<W>, Seq<W>, Unit> Iter<W, A>(this Writer<MSeq<W>, Seq<W>, A> self, Action<A> action) =>
        self.Iter<MSeq<W>, Seq<W>, A>(action);

    public static Writer<MonoidW, W, Unit> Iter<MonoidW, W, A>(this Writer<MonoidW, W, A> self, Action<A> action)
        where MonoidW : struct, Monoid<W> =>
            () => {
                var (x, s, b) = self();
                if (!b) action(x);
                return (unit, s, false);
            };
}
