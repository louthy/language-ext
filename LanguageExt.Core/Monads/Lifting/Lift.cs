
using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Lifts a function into a type that can be used with other
/// monadic types (in LINQ expressions, for example) and with implicit
/// conversions.
/// </summary>
public record Lift<A>(Func<A> Function)
{
    public K<F, A> ToApplicative<F>() 
        where F : Applicative<F> =>
        F.Pure(unit).Map(_ => Function());

    public IO<A> ToIO() =>
        IO.lift(_ => Function());

    public Eff<A> ToEff() =>
        Eff<A>.Lift(Function);

    public Eff<RT, A> ToEff<RT>() =>
        Eff<RT, A>.Lift(Function);

    public Lift<B> Map<B>(Func<A, B> f) =>
        new (() => f(Function()));

    public Lift<B> Bind<B>(Func<A, Lift<B>> f) =>
        new(() => f(Function()).Function());

    public Lift<B> Bind<B>(Func<A, Pure<B>> f) =>
        new (() => f(Function()).Value);

    public K<M, B> Bind<M, B>(Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Pure(unit).Map(_ => Function()).Bind(f);

    public IO<B> Bind<B>(Func<A, IO<B>> f) =>
        ToIO().Bind(f);

    public Lift<B> Select<B>(Func<A, B> f) =>
        Map(f);

    public IO<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) => 
        ToIO().SelectMany(bind, project);

    public K<M, C> SelectMany<M, B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project)  
        where M : Monad<M> =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Lift<C> SelectMany<B, C>(Func<A, Lift<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Lift<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}

/// <summary>
/// Lifts a function into a type that can be used with other
/// monadic types (in LINQ expressions, for example) and with implicit
/// conversions.
/// </summary>
public record Lift<A, B>(Func<A, B> Function)
{
    public Lift<A, C> Map<C>(Func<B, C> f) =>
        new (x => f(Function(x)));

    public Lift<A, C> Bind<C>(Func<B, Lift<A, C>> f) =>
        new(x => f(Function(x)).Function(x));

    public Lift<A, C> Bind<C>(Func<B, Pure<C>> f) =>
        new (x => f(Function(x)).Value);

    public Lift<A, C> Select<C>(Func<B, C> f) =>
        Map(f);

    public Lift<A, D> SelectMany<C, D>(Func<B, Pure<C>> bind, Func<B, C, D> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
}
